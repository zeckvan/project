namespace StudentLearningHistory.Models.StudentInfo.DbModels
{
    public class V_Student
    {
        public string std_no { get; set; }
        public string std_name { get; set; }
        public string std_identity { get; set; }
        public DateTime? std_birth_dt { get; set; }
        public string std_email { get; set; }
        public string? std_fb { get; set; }
        public string? std_ig { get; set; }
        public string? std_nickname { get; set; }
        public string? std_memo { get; set; }
    }
}
