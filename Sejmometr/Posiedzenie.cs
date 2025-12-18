using Newtonsoft.Json;

namespace Sejmometr
{
    public class Posiedzenie
    {
        [JsonProperty("number")]
        public int Number { get; set; }
        public string OpisPosiedzenia => $"Posiedzenie nr {Number}";
    }
}