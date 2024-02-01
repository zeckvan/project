namespace StudentLearningHistory.Models.CentralDBofLearningHistory.Std
{
    public class StdFeedBackListParameter
    {
        public short year_id {  get; set; }
        public short sms_id {  get; set; }
        public string? std { get; set; }
        public int sRow { get; set; } = 1;
        public int eRow { get; set; } = 10;
        public string? token { get; set; }
    }
}
