namespace StudentLearningHistory.Models.UserProfiles
{
    public class UserData
    {
        public string sch_no { get; set; }
        public int now_year { get; set; }
        public int now_sms { get; set; }
        public int default_year { get; set; }
        public int default_sms { get; set; }
        public string on_line { get; set; }
        public string user_id { get; set; }
    }

    public class TokenResult
    {
        public string message { get; set; }
        public int code { get; set; }
        public UserData result { get; set; }
    }
}
