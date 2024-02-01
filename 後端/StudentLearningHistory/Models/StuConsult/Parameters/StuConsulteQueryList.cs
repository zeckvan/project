namespace StudentLearningHistory.Models.StuConsult.Parameters
{
    public class StuConsulteQueryList
    {
        public string? std_name { get; set; }
        public string? cls_id { get; set; }
        public string? emp_id { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string? sch_no { get; set; }
        public int? ser_id { get; set; }
        public int? sRowNun { get; set; }
        public int? eRowNun { get; set; }
        public int? number_id { get; set; }
        public string? complex_key
        {
            get { return $"{sch_no}_{year_id}_{sms_id}_{emp_id}_{ser_id}"; }
        }
        public string class_name{get;set;} = "TeaConsult";
        public string? std_no{ get; set; }
        public string? token { get; set; }      
        
    }
}
