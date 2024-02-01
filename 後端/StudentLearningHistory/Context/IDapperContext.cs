using System.Data;

namespace StudentLearningHistory.Context
{
    public interface IDapperContext
    {
        public IDbConnection CreateCommand();
        public string online { get; set; }
        public string SchNo { get; set; }
        public int now_year { get; set; }
        public int now_sms { get; set; }
        public string user_id { get; set; }
        public string is_401 { get; set; }
    }
}