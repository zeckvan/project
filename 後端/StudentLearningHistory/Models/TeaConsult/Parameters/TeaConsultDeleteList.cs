namespace StudentLearningHistory.Models.TeaConsult.Parameters
{
    public class TeaConsultDeleteList
    {
        public string? sch_no { get; set; } //學校代碼
        public int[]? year_id { get; set; }//年度
        public int[]? sms_id { get; set; }//學期
        public string[]? emp_id { get; set; }//教師
        public string? emp_name { get; set; }//姓名
        public string? cls_abr { get; set; } //班級
        public int[] ? deg_id { get; set; }//
        public int[]? dep_id { get; set; }//
        public int[]? bra_id { get; set; }//
        public int[]? grd_id { get; set; }//
        public int[]? cls_id { get; set; }//
        public int? sRowNun { get; set; }
        public int? eRowNun { get; set; }
        public int? x_total { get; set; }
        public int? RowNum { get; set; }
        public string? x_status { get; set; }
        public string? token { get; set; }
    }
}
