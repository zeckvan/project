namespace StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackParameters
{
    public class FeedBackUpdataAnswerParameter
    {
        public short year_id {  get; set; }
        public short sms_id {  get; set; }
        public int ser_id { get; set; }
        public string? answer { get; set; }
        public string? upd_name { get; set; }
        public string? upd_dt { get; set; }
        public string? token { get; set; }
    }
}
