namespace StudentLearningHistory.Models.StudentInfo.DbModels
{
    public class V_AbsenceRecord
    {
        public int mat_year { get; set; }
        public int mat_sms { get; set; }
        public string std_no { get; set; }
        public string mat_name { get; set; }
        public string mat_id { get; set; }
        public DateTime? abs_sdt { get; set; }
        public DateTime? abs_edt { get; set; }
        public int? mat_sum { get; set; }
        public int? grd_id { get; set; }
    }
}
