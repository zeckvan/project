using Microsoft.Data.SqlClient;
using System.Data;

namespace StudentLearningHistory.Context
{
    public class DapperReleaseContext : IDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _contextString = string.Empty;
        public string online { get; set; }
         public string SchNo { get; set; }
        public int now_year { get; set; }
        public int now_sms { get; set; }
        public string user_id { get; set; }
        public string is_401 { get; set; } = "N";
        public DapperReleaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _contextString = _configuration.GetConnectionString("edudb");
        }

        //public IDbConnection CreateCommand() => new SqlConnection(_contextString);
        public IDbConnection CreateCommand() => new SqlConnection(online);
    }
}
