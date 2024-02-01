namespace StudentLearningHistory.Models.StuAttestation.DbModels
{
    public class L01_StuAttestation
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string cls_id { get; set; }//班級
        public string sub_id { get; set; }//科目
        public string src_dup { get; set; }//分組
        public string emp_id { get; set; }//教師
        public string? std_no { get; set; }//學號
        public int ser_id { get; set; }//序號
        public string? attestation_send { get; set; }//送出認証
        public string? attestation_date { get; set; }//認証日期
        public string? attestation_status { get; set; }//認証狀態
        public string? content { get; set; }//資料來源
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public string[]? complex_key { get; set; }
        public string? token { get; set; }
    }
}