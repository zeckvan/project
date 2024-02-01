using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.StuTurn.Parameter
{
    public class Import
    {
        public class Title
        {
            public Basic 基本資料 { get; set; }
        }
        public class Basic
        {
            public string 就讀學校代碼 { get; set; }
            public string 就讀單位代碼 { get; set; }
            public int 學年度 { get; set; }
            public int 學期 { get; set; }
            public string 國民身分證統一編號 { get; set; }
            public string 學生中文姓名 { get; set; }
            public string 學生出生年月日 { get; set; }
            public List<Cadre>? 擔任學校幹部經歷紀錄 { get; set; }
        }
        public class Cadre
        {
            public string 擔任學校幹部期間就讀學校代碼 { get; set; }
            public string 擔任學校幹部期間就讀單位代碼 { get; set; }
             public int 學年度 { get; set; }
             public int 學期 { get; set; }
            public string 國民身分證統一編號 { get; set; }
            public string 學生出生年月日 { get; set; }
            public string 單位名稱 { get; set; }
             public string 開始日期 { get; set; }
            public string 結束日期 { get; set; }
            public string 擔任職務名稱 { get; set; }
             public string 幹部等級代碼 { get; set; }
            public string? 建立日期 { get; set; }
            public string 提交紀錄 { get; set; }
        }
    }
}
