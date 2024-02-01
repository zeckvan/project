using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 校內幹部經歷內層
    {
        public string 身分證號 { get; set; }
        public string 出生日期 { get; set; }

        [JsonConverter(typeof(InfoToStrListConverterAttribute))]
        public List<string> 幹部經歷 { get; set; }
    }
}
