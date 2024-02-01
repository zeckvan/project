namespace StudentLearningHistory.Models.StuResult.DbModels
{
    public class HeaderList
    {
        public string a { get; set; } //學校代碼
        public int b { get; set; }//年度
        public int c { get; set; }//學期
        public string d { get; set; }//學號
        public int e { get; set; }//序號
        public string f { get; set; }//名稱
        public string g { get; set; }//日期
        public string h { get; set; }//內容簡述
        public string i { get; set; }//資料來源
        public int x_cnt { get; set; }//檔案數量
        public int x_total { get; set; }
        public string? x_status { get; set; }
        public string? zz { get; set; }
    }
}
