namespace StudentLearningHistory.Models.StuAttestation.DbModels
{
    public class HeaderList
    {
        public string a { get; set; } //學校代碼
        public int b { get; set; }//年度
        public int c { get; set; }//學期
        public string d { get; set; }//班級
        public string e { get; set; }//科目代碼
        public string f { get; set; }//分組
        public string g { get; set; }//教師代碼
        public string h { get; set; }//學生
        public int i { get; set; }//序號
        public int j { get; set; }//學分數
        public string k { get; set; }//修習方式
        public string l { get; set; }//送出認証日期
        public string m { get; set; }//確認認証日期
        public string n { get; set; }//認証狀態
        public string o { get; set; }//資料來源
        public string p { get; set; }//備註
        public string q { get; set; }//科目名稱
        public string r { get; set; }//教師名稱
        public string s { get; set; }//檔案名稱
        public string t { get; set; } = "學期";
        public string u { get; set; } //確認課程學習成果
        public string v { get; set; } //課程學習成果勾選狀態
        public string w { get; set; } //未通過原因
        public string x { get; set; } //發佈
        public int x_cnt { get; set; }//檔案數量
        public int x_cnt2 { get; set; }//檔案數量
        public int x_total { get; set; }
        public string? x_status { get; set; }
        public int x_file_cnt { get; set; }//可上傳檔案數
        public int x_file_center_cnt { get; set; }//可上傳國教暑檔案數
    }
}