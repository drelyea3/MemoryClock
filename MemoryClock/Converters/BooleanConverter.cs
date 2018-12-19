namespace MemoryClock.Converters
{
    class BooleanConverter : OneWayConverter<bool>
    {
        public string True { get; set; }
        public string False { get; set; }

        protected override object Convert(bool value)
        {
            return value ? True : False;
        }
    }
}
