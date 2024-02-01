namespace StudentLearningHistory.Models.StuCompetition.Parameters
{
    public class QueryFile
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string? sch_no { get; set; }
        public int? ser_id { get; set; }
        public int? sRowNun { get; set; }
        public int? eRowNun { get; set; }
        public int? number_id { get; set; }
        public string? complex_key
        {
            get { return $"{sch_no}_{year_id}_{sms_id}_{std_no}_{ser_id}"; }
        }
        public IFormFileCollection? files { get; set; }
        public string class_name{get;set;} = "StuCompetition";
    }
}
