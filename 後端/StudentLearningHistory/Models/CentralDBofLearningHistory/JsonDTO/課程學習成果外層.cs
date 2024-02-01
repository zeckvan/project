using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 課程學習成果外層
    {
        [JsonConverter(typeof(InfoToStringConverterAttribute))]
        public string 名冊資訊 { get; set; }
        public List<課程學習成果內層> 課程學習成果 { get; set; }
    }
}
