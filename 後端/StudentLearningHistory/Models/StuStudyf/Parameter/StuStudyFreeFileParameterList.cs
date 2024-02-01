
namespace StudentLearningHistory.Models.StuStudyf.Parameter
{
    public class StuStudyFreeFileParameterList
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public int ser_id { get; set; }
        public int? sRowNun { get; set; } = 0;
        public int? eRowNun { get; set; } = 9999;
    }
}
