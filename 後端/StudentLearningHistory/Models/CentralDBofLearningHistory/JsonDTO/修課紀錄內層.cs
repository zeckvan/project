using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 修課記錄內層
    {
        public string 身分證號 { get; set; }
        public string 出生日期 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 學期成績 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 補修成績 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 轉學轉科成績 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 重修成績 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 重讀成績 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 進修部學期成績 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 身分進修部補考成績證號 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 進修部轉學轉科成績 { get; set; }
    }
}
