using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CurrencyConverterModels.ViewModels
{
    public class CurrencyBaseResult
    {
        public double Amount { get; set; }
        [JsonProperty("base")]
        [JsonPropertyName("base")]
        public string _base { get; set; }
    }
    public class CurrencyConversionResult : CurrencyBaseResult
    {
        public DateTime Date { get; set; }
        public Dictionary<string, string> Rates { get; set; }
    }
    public class CurrencyHistoryResult : CurrencyBaseResult
    {
        [JsonProperty("start_date")]
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [JsonProperty("end_date")]
        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
        public Dictionary<string, Dictionary<string, string>> Rates { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public int PageCount { get; set; }
        public int TotalResults { get; set; }
    }
}
