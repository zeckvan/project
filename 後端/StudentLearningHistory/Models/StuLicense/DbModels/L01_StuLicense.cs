namespace StudentLearningHistory.Models.StuLicense.DbModels
{
    public class L01_StuLicense
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string? std_no { get; set; }//學號
        public int? ser_id { get; set; }//序號
        public string license_id { get; set; }//證照代碼
        public string? license_memo { get; set; }//證照備註
        public decimal? license_grade { get; set; }//分數
        public string? license_result { get; set; }//分項結果
        public string license_date { get; set; }//取得證照日期
        public string? license_doc { get; set; }//證照字號
        public string? license_group { get; set; }//檢定組別
        public string content { get; set; }//內容簡述
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public string? token { get; set; }
    }
}
