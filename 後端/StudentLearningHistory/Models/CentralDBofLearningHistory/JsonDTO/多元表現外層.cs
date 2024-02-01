using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 多元表現外層
    {
        [JsonConverter(typeof(InfoToStringConverterAttribute))]
        public string 名冊資訊 { get; set; }
        public List<多元表現內層> 多元表現 { get; set; }
    }
}
