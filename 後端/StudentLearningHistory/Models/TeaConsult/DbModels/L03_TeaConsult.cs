namespace StudentLearningHistory.Models.TeaConsult.DbModels
{
    public class L03_TeaConsult
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string? emp_id { get; set; }//教師
        public int? ser_id { get; set; }//序號
        public string consult_date { get; set; }//諮詢日期
        public string consult_area { get; set; }//諮詢地點
        public string consult_type { get; set; }//諮詢方式
        public string consult_subject { get; set; }//諮詢主題
        public string consult_content { get; set; }//諮詢內容        
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public string? token { get; set; }
    }
}
