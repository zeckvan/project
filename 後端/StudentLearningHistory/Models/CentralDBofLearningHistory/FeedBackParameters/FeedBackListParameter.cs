namespace StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackParameters
{
    public class FeedBackListParameter
    {
        public short year_id {  get; set; }
        public short sms_id {  get; set; }
        public string kind { get; set; } = "All";
        public string cls { get; set; } = "All";
        public int sRow { get; set; } = 1;
        public int eRow { get; set; } = 10;
        public string? token { get; set; }
    }
}
