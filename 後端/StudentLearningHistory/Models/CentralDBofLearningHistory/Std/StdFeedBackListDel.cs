namespace StudentLearningHistory.Models.CentralDBofLearningHistory.Std
{
    public class StdFeedBackListDel
    {
        public string? sch_no { get; set; }
        public short year_id { get; set; }
        public short sms_id { get; set; }
        public int ser_id { get; set; }
        public string? std_no { get; set; } = null!;
        public string kind { get; set; } = null!;
        public string? token { get; set; }

    }
}
