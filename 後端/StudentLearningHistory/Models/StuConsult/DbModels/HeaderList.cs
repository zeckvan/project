namespace StudentLearningHistory.Models.StuConsult.DbModels
{
    public class HeaderList
    {
        public string a { get; set; } //學校代碼
        public int b { get; set; }//年度
        public int c { get; set; }//學期
        public string d { get; set; }//教師
        public int e { get; set; }//序號
        public string f { get; set; }//諮詢日期
        public string g { get; set; }//諮詢地點
        public string h { get; set; }//諮詢方式
        public string i { get; set; }//諮詢主題
        public string j { get; set; }//諮詢內容    
        public string k { get; set; }//資料來源
        public int x_cnt { get; set; }//檔案數量
        public int x_stucnt { get; set; }//諮詢學生總量
        public string? x_status { get; set; }
    }
}
