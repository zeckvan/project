namespace StudentLearningHistory.Models.CentralDBofLearningHistory.Std
{
    public class StdCheckDataParameter
    {
        public short year_id {  get; set; }
        public short sms_id {  get; set; }
        public string kind { get; set; }
        public string std { get; set; }
        public string? token { get; set; }
    }
}
