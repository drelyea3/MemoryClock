using Common;
using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace MemoryClock.Sensors
{
    public class LuxSensor
    {
        public enum Sensitivity
        {
            // 13 ms
            Low = 0x00,
            // 101 ms
            Medium = 0x01,
            // 402 ms
            High = 0x02,
        }

        // Registers
        private const int TSL2561_CMD = 0x80;
        private const int TSL2561_REG_CONTROL = 0x00;
        private const int TSL2561_REG_TIMING = 0x01;

        private const int TSL2561_REG_DATA_0 = 0x0C;
        private const int TSL2561_REG_DATA_1 = 0x0E;

        private const bool DEFAULT_GAIN = false;
        private const Sensitivity DEFAULT_SENSITIVITY = Sensitivity.Medium;

        I2cDevice device;

        private bool gain;
        private uint integrationTime;

        private LuxSensor() { }

        public static LuxSensor Create(bool gain = true, Sensitivity sensitivity = Sensitivity.High)
        {
            var result = new LuxSensor();

            try
            {
                string aqs = I2cDevice.GetDeviceSelector();
                var disTask = DeviceInformation.FindAllAsync(aqs).AsTask();
                disTask.Wait();
                var dis = disTask.Result;
                Logger.Log($"Found {dis.Count} devices");
                if (dis.Count == 0)
                {
                    // No devices found -- was this run on a PC?
                    return null;
                }

                var settings = new I2cConnectionSettings(0x39);
                settings.BusSpeed = I2cBusSpeed.FastMode;
                settings.SharingMode = I2cSharingMode.Shared;
                var did = dis[0].Id;
                Logger.Log($"Device {did}");
                var deviceTask = I2cDevice.FromIdAsync(did, settings).AsTask();
                deviceTask.Wait();
                result.device = deviceTask.Result;

                result.Write8(TSL2561_REG_CONTROL, 0x03);

                result.SetTiming(gain, sensitivity);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        // Write byte
        private void Write8(byte addr, byte cmd)
        {
            byte[] Command = new byte[] { (byte)((addr) | TSL2561_CMD), cmd };

            device.Write(Command);
        }

        // Read byte
        private byte Read8(byte addr)
        {
            byte[] aaddr = new byte[] { (byte)((addr) | TSL2561_CMD) };
            byte[] data = new byte[1];

            device.WriteRead(aaddr, data);

            return data[0];
        }

        // Read integer
        private ushort Read16(byte addr)
        {
            byte[] aaddr = new byte[] { (byte)((addr) | TSL2561_CMD) };
            byte[] data = new byte[2];

            device.WriteRead(aaddr, data);

            return (ushort)((data[1] << 8) | (data[0]));
        }

        // Get channel data
        private uint[] GetData()
        {
            uint[] data = new uint[2];
            data[0] = Read16(TSL2561_REG_DATA_0);
            data[1] = Read16(TSL2561_REG_DATA_1);
            return data;
        }

        public void SetTiming(Boolean gain = DEFAULT_GAIN, Sensitivity sensitivity = DEFAULT_SENSITIVITY)
        {
            this.gain = gain;

            switch (sensitivity)
            {
                case Sensitivity.Low:
                    integrationTime = 14;
                    break;
                case Sensitivity.High:
                    integrationTime = 402;
                    break;
                default:
                    integrationTime = 101;
                    break;
            }

            unchecked
            {
                uint timing = Read8(TSL2561_REG_TIMING);

                // Set gain (0 or 1)
                if (gain)
                    timing |= 0x10;
                else
                    timing &= (uint)~0x10;

                // Set integration time (0 to 3) 
                timing &= (uint)~0x03;
                timing |= ((uint)sensitivity & 0x03);

                Write8(TSL2561_REG_TIMING, (byte)timing);
            }
        }

        // Calculate Lux
        public double GetLux()
        {
            uint[] Data = GetData();
            uint CH0 = Data[0];
            uint CH1 = Data[1];

            //Logger.Log($"CH0 {CH0} CH1 {CH1}");
            double ratio, d0, d1;
            double lux = 0.0;

            // Determine if either sensor saturated (0xFFFF)
            if ((CH0 == 0xFFFF) || (CH1 == 0xFFFF))
            {
                lux = 0.0;
                return lux;
            }

            // Convert from unsigned integer to floating point
            d0 = CH0; d1 = CH1;

            // We will need the ratio for subsequent calculations
            ratio = d1 / d0;

            // Normalize for integration time
            d0 *= (402.0 / integrationTime);
            d1 *= (402.0 / integrationTime);

            // Normalize for gain
            if (!gain)
            {
                d0 *= 16;
                d1 *= 16;
            }

            // Determine lux per datasheet equations:
            if (ratio < 0.5)
                lux = 0.0304 * d0 - 0.062 * d0 * Math.Pow(ratio, 1.4);
            else if (ratio < 0.61)
                lux = 0.0224 * d0 - 0.031 * d1;
            else if (ratio < 0.80)
                lux = 0.0128 * d0 - 0.0153 * d1;
            else if (ratio < 1.30)
                lux = 0.00146 * d0 - 0.00112 * d1;
            else
                lux = 0.0;

            return Math.Round(lux, 2);
        }
    }

}
