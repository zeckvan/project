namespace StudentLearningHistory.Models.StuFileInfo.Parameters
{
    public class StuFileInfoQueryFile
    {
        public string? std_no { get; set; }
        public string? emp_id { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string? sch_no { get; set; }
        public int? ser_id { get; set; } = 0;
        public int? sRowNun { get; set; }
        public int? eRowNun { get; set; }
        public int? number_id { get; set; }
        /*
        public string? complex_key
        {
            get { return $"{sch_no}_{year_id}_{sms_id}_{std_no}_{ser_id}"; }

        }
        */
        //public string? complex_key
        //{
        //    get { return class_name.Substring(0, 3) == "Tea" ? $"{sch_no}_{year_id}_{sms_id}_{emp_id}_{ser_id}" : $"{sch_no}_{year_id}_{sms_id}_{std_no}_{ser_id}" ; }
        //}
        public string? complex_key { get; set; }
        public IFormFileCollection? files { get; set; }
        public string class_name{get;set;} = "stugroup";
        public string? content { get; set; }
        public string? token { get; set; }
    }
}
