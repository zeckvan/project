using Dapper;
using Microsoft.Data.SqlClient;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StudentInfo.DbModels;
using System.Data;
using System.Text;

namespace StudentLearningHistory.Services
{
    public class StudentInfoService
    {
        private readonly IDapperContext _context;

        public string _schema;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";
        public StudentInfoService(IDapperContext context)
        {
            _context = context;
            _schema ??= "dbo";
        }

        public async Task<T> queryByStdNo<T>(string std_no) where T : class
        {
            string sql = $"SELECT * FROM {_schema}.[{typeof(T).Name}] WHERE std_no=@std_no";

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<T>(sql, new { std_no }); ;
            }
        }

        public async Task<int> InserStuInfo(StudentInfo arg)
        {
            arg.sch_no = _context.SchNo;
            arg.std_no = _context.user_id;
            string upd_dt = updteDate();
            string str_sql = "select count(*) as x_cnt  from L01_stu_info where std_no = {0}";
            using (IDbConnection conn = _context.CreateCommand())
            {
                var dr = await conn.ExecuteReaderAsync(string.Format(str_sql, arg.std_no));
                DataTable dt = new DataTable();
                dt.Load(dr);

                if (Convert.ToInt64(dt.Rows[0]["x_cnt"]) > 0)
                {
                    str_sql = @"
                            update L01_stu_info
                            set 
                                std_fb = @std_fb,
                                std_ig = @std_ig,
                                std_nickname = @std_nickname,
                                std_memo = @std_memo
                            where std_no = @std_no
                        ";
                }
                else
                {
                    str_sql = @"
                            insert L01_stu_info
                            (sch_no,std_no,std_fb,std_ig,std_nickname,std_memo)
                            values
                            (@sch_no,@std_no,@std_fb,@std_ig,@std_nickname,@std_memo)
                        ";
                }

                int rtn = await conn.ExecuteAsync(str_sql, arg);

                return rtn;
            }
        }
        public async Task<IEnumerable<T>> queryByYearSmsStdNo<T>(int year, int sms, string mat_id, string std_no) where T : class
        {
            StringBuilder sql = new StringBuilder($"SELECT * FROM {_schema}.[{typeof(T).Name}] WHERE std_no=@std_no ");
            List<SqlParameter> paramters = new List<SqlParameter>();
            paramters.Add(new SqlParameter() { ParameterName = "@stdno", Value = std_no, DbType = DbType.String });

            switch (typeof(T).Name)
            {
                case "V_AbsenceRecord":
                    sql.Append($" and mat_year=@year and mat_sms=@sms and mat_id like @mat_id");
                    break;
                case "V_StuphrRecord":
                    sql.Append($" and phr_year=@year and phr_sms=@sms");
                    break;
                default:
                    break;
            }

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<T>(sql.ToString(), new { std_no, year, sms, mat_id }); ;
            }
        }

        public async Task<IEnumerable<T>> queryByYearSmsStdNoStuphr<T>(int year, int sms, string std_no) where T : class
        {
            StringBuilder sql = new StringBuilder($"SELECT * FROM {_schema}.[{typeof(T).Name}] WHERE std_no=@std_no ");
            List<SqlParameter> paramters = new List<SqlParameter>();
            paramters.Add(new SqlParameter() { ParameterName = "@stdno", Value = std_no, DbType = DbType.String });

            switch (typeof(T).Name)
            {
                case "V_AbsenceRecord":
                    sql.Append($" and mat_year=@year and mat_sms=@sms");
                    break;
                case "V_StuphrRecord":
                    sql.Append($" and phr_year=@year and phr_sms=@sms");
                    break;
                default:
                    break;
            }

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<T>(sql.ToString(), new { std_no, year, sms }); ;
            }
        }
    }
}
