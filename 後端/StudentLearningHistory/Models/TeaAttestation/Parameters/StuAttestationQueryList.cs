using StudentLearningHistory.Controllers;

namespace StudentLearningHistory.Models.TeaAttestation.Parameters
{
    public class StuAttestationQueryList
    {
        public string? cls_id { get; set; }
        public string? std_name { get; set; }
        public string? std_no { get; set; }
        public string? emp_id { get; set; }
        public int? year_id { get; set; }
        public int? sms_id { get; set; }
        public string? sch_no { get; set; }
        public int? ser_id { get; set; }
        public int? sRowNun { get; set; }
        public int? eRowNun { get; set; }
        public int? number_id { get; set; }
        public string? complex_key
        {
            get { return $"{sch_no}_{year_id}_{sms_id}_{emp_id}_{ser_id}"; }
        }
        public string class_name { get; set; } = "StuAttestation";
        public string? kind { get; set; }
        public string? consult_emp { get; set; }
        public string? token { get; set; }
        public string? grade_id { get; set; }
    }
}
