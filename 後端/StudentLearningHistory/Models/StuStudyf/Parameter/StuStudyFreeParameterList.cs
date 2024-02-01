
namespace StudentLearningHistory.Models.StuStudyf.Parameters
{
    public class StuStudyFreeParameterList
    {
        public string? std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public int? sRowNun { get; set; } = 0;
        public int? eRowNun { get; set; } = 9999;
        public string? token { get; set; }
    }
}
