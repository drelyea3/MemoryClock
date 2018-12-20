namespace MemoryClock.Converters
{
    class BooleanConverter : OneWayConverter<bool>
    {
        public object True { get; set; }
        public object False { get; set; }

        protected override object Convert(bool value)
        {
            return value ? True : False;
        }
    }
}
