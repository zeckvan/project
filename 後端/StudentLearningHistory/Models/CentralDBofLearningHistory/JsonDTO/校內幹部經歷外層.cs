using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO
{
    public class 校內幹部經歷外層
    {
        [JsonConverter(typeof(InfoToStringConverterAttribute))]
        public string 名冊資訊 { get; set; }
        public List<校內幹部經歷內層> 校內幹部經歷 { get; set; }
    }
}
