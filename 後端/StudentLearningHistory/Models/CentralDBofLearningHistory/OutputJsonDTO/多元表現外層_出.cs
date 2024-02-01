
namespace StudentLearningHistory.Models.CentralDBofLearningHistory.OutputJsonDTO
{
    public class 多元表現外層_出
    {
        public string 名稱 { get; set; }
        public string 名冊資訊 { get; set; }
        public string 上傳人員 { get; set; }
        public string 開放確認期限 { get; set; }
        public string kind { get; set; }
        public 多元表現內層_出 多元表現 { get; set; } = new();
    }
}
