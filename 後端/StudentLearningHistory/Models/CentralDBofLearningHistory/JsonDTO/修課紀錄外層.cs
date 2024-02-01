using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 修課紀錄外層
    {
        [JsonConverter(typeof(InfoToStringConverterAttribute))]
        public string 名冊資訊 { get; set; }

        public List<修課記錄內層> 修課紀錄 { get; set; }
    }
}
