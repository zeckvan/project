namespace StudentLearningHistory.Models.StuCompetition.DbModels
{
    public class L01_StuCompetition
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string? std_no { get; set; }//學號
        public int? ser_id { get; set; }//序號
        public string competition_name { get; set; }//競賽名稱
        public string? competition_item { get; set; }//競賽項目
        public string competition_area { get; set; }//競賽領域
        public string competition_grade { get; set; }//競賽等級
        public string competition_result { get; set; }//獎項
        public string competition_date { get; set; }//結果公布日期
        public string competition_type { get; set; }//參與類型
        public string content { get; set; }//內容簡述
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public string? token { get; set; }
    }
}
