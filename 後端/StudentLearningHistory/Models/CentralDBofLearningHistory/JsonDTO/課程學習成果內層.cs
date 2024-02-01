using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 課程學習成果內層
    {
        public string 身分證號 { get; set; }
        public string 出生日期 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 學期課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 補修課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 重修課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 重讀課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 轉學轉科課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 借讀課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 進修部學期課程學習成果 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 進修部轉學 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 進修部借讀課程學習成果 { get; set; }
    }
}
