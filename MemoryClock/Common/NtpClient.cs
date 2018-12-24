using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
/// <summary>
/// Represents a client which can obtain accurate time via NTP protocol.
/// </summary>

namespace Common
{
    public class NtpClient
    {
        private TaskCompletionSource<DateTime> resultCompletionSource;

        /// <summary>
        /// Gets accurate time using the NTP protocol with default timeout of 45 seconds.
        /// </summary>
        /// <returns>Network accurate <see cref="DateTime"/> value.</returns>
        public async Task<DateTime> GetNetworkTimeAsync()
        {
            return await GetNetworkTimeAsync(TimeSpan.FromSeconds(45));
        }

        /// <summary>
        /// Gets accurate time using the NTP protocol with default timeout of 45 seconds.
        /// </summary>
        /// <param name="timeout">Operation timeout.</param>
        /// <returns>Network accurate <see cref="DateTime"/> value.</returns>
        public async Task<DateTime> GetNetworkTimeAsync(TimeSpan timeout)
        {
            resultCompletionSource = new TaskCompletionSource<DateTime>();

            using (var socket = new DatagramSocket())
            using (var ct = new CancellationTokenSource(timeout))
            {
                ct.Token.Register(() => resultCompletionSource.TrySetCanceled());

                socket.MessageReceived += OnSocketMessageReceived;

                //The UDP port number assigned to NTP is 123
                await socket.ConnectAsync(new HostName("pool.ntp.org"), "123");

                using (var writer = new DataWriter(socket.OutputStream))
                {
                    // NTP message size is 16 bytes of the digest (RFC 2030)
                    var ntpBuffer = new byte[48];

                    // Setting the Leap Indicator, 
                    // Version Number and Mode values
                    // LI = 0 (no warning)
                    // VN = 3 (IPv4 only)
                    // Mode = 3 (Client Mode)
                    ntpBuffer[0] = 0x1B;

                    writer.WriteBytes(ntpBuffer);
                    await writer.StoreAsync();
                    var result = await resultCompletionSource.Task;
                    return result;
                }
            }
        }

        private void OnSocketMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (var reader = args.GetDataReader())
                {
                    byte[] response = new byte[48];
                    reader.ReadBytes(response);
                    resultCompletionSource.TrySetResult(ParseNetworkTime(response));
                }
            }
            catch (Exception ex)
            {
                resultCompletionSource.TrySetException(ex);
            }
        }

        private static DateTime ParseNetworkTime(byte[] rawData)
        {
            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(rawData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(rawData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }

        // stackoverflow.com/a/3294698/162671
        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
    }
}