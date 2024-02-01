using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.OutputJsonDTO
{
    public class 修課紀錄外層_出
    {
        public string 名稱 { get; set; }
        public string 名冊資訊 { get; set; }
        public string 上傳人員 { get; set; }
        public string 開放確認期限 { get; set; }
        public string kind { get; set; }
        public 修課紀錄內層_出 修課紀錄 { get; set; } = new();
    }
}
