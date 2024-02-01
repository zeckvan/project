namespace StudentLearningHistory.Models.StuWorkplace.DbModels
{
    public class L01_StuWorkplace
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string? std_no { get; set; }//學號
        public int? ser_id { get; set; }//序號
        public string type_id { get; set; }//類別
        public string unit_name { get; set; }//單位
        public string? type_title { get; set; }//職稱
        public string startdate { get; set; }//開始時間
        public string enddate { get; set; }//結束時間
        public decimal? hours { get; set; }//時數
        public string? content { get; set; }//內容簡述
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public string? token { get; set; }
    }
}
