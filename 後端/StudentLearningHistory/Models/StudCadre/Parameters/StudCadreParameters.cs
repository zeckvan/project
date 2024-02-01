
namespace StudentLearningHistory.Models.StudCadre.Parameters
{
    public class StudCadreParameters
    {
        public string? std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public int? sRowNun { get; set; } = 0;
        public int? eRowNun { get; set; } = 9999;
        public string? is_sys { get; set; }
        public string? token { get; set;}
    }
}
