namespace StudentLearningHistory.Models.StuCollege.DbModels
{
    public class L01_StuCollege
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string? std_no { get; set; }//學號
        public int? ser_id { get; set; }//序號
        public string? project_name { get; set; }//計畫專案
        public string unit_name { get; set; }//開設單位
        public string course_name { get; set; }//課程名稱
        public string startdate { get; set; }//開始時間
        public string enddate { get; set; }//結束時間
        public decimal? credit { get; set; }//學分數
        public decimal? hours { get; set; }//總時數
        public string? content { get; set; }//內容簡述
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public string? token { get; set; }
    }
}
