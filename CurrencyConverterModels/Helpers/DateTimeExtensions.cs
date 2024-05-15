namespace CurrencyConverterModels.Helpers
{
    public static class DateTimeExtensions
    {
        public static string GetDateForHistoricalApi(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
    
}
