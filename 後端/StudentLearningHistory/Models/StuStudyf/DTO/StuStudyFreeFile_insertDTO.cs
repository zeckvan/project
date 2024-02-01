

namespace StudentLearningHistory.Models.StuStudyf.DTO
{
    public class StuStudyFreeFile_insertDTO
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public int ser_id { get; set; }
        public IFormFileCollection files { get; set; }
        public string? class_name { get; set; } = "StuStudyf";
        public string? content { get; set; }
    }
}
