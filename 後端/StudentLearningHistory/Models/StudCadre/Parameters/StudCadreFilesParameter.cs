
namespace StudentLearningHistory.Models.StudCadre.Parameters
{
    public class StudCadreFilesParameter
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string unit_name { get; set; }
        public string position_name { get; set; }
        public int? sRowNun { get; set; } = 0;
        public int? eRowNun { get; set; } = 9999;
    }
}
