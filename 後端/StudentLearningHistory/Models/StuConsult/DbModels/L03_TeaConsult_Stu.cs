namespace StudentLearningHistory.Models.StuConsult.DbModels
{
    public class L03_TeaConsult_Stu
    {
        public string? sch_no { get; set; } //學校代碼
        public int? year_id { get; set; }//年度
        public int? sms_id { get; set; }//學期
        public string? emp_id { get; set; }//教師
        public int? ser_id { get; set; }//序號
        public string? cls_name { get; set; } //班級
        public string? stu_name { get; set; }//姓名
        public string[]? std_no { get; set; }//學號
       // public string? std_no { get; set; }//學號
    }
}
