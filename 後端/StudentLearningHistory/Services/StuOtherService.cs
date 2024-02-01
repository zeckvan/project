using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuOther.DbModels;
using StudentLearningHistory.Models.StuOther.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuLicense.Parameters;

namespace StudentLearningHistory.Services
{
    public class StuOtherService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public StuOtherService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public async Task<IEnumerable<HeaderList>> GetList(StuOtherQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L01_std_public_filehub a
                                    where a.class_name = 'StuOther'
                                    and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+convert(varchar,NewTable.e)) as x_cnt,
                                '' as x_status
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY year_id, sms_id, startdate,enddate) AS RowNum,
                                                sch_no as a,
                                                year_id as b,
                                                sms_id as c,
                                                std_no as d,
                                                ser_id as e,
                                                other_name as f,
                                                unit_name as g,
                                                startdate as h,
                                                enddate as i,
                                                hours as j,
                                                content as k,
                                                is_sys as l,
                                                check_centraldb as zz
                                        FROM L01_stu_other
                                        WHERE sch_no = @sch_no and year_id = @year_id and sms_id = @sms_id and std_no = @std_no
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<L01_StuOhter>> GetFormData(StuOtherQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *
                                FROM L01_stu_other
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_StuOhter>(str_sql, arg);
            }
        }

        public async Task<int> DeleteData(StuOtherQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                delete                                
                                FROM L01_stu_other
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<int> UpdateData(L01_StuOhter arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;

            string str_sql = @"
                                update L01_stu_other
                                set 
                                    other_name = @other_name,
                                    unit_name =@unit_name,
                                    startdate =@startdate,
                                    enddate =@enddate,
                                    hours =@hours,
                                    content = @content,
                                    is_sys = @is_sys,
                                    upd_name = @upd_name,
                                    upd_dt = @upd_dt
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<Dictionary<string, string>> InsertData(L01_StuOhter arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L01_stu_other 
                        where year_id = @year_id
                        and sms_id = @sms_id
                        and std_no = @std_no
                        and sch_no = @sch_no
                        ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                var dr = await conn.ExecuteReaderAsync(str_sql, arg);
                DataTable dt = new DataTable();
                dt.Load(dr);
                var result = (from r in dt.AsEnumerable()
                              where r.Field<string>("other_name") == arg.other_name &&
                                    r.Field<string>("unit_name") == arg.unit_name &&
                                    r.Field<string>("startdate") == arg.startdate &&
                                    r.Field<string>("enddate") == arg.enddate
                              select r).Count();
                if (result > 0)
                {
                    dict.Add("haveData", "Y");
                    dict.Add("result", "0");
                    return dict;
                }
                else
                {
                    var num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("std_no") == arg.std_no &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Count();

                    if (num > 0)
                    {
                        num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("std_no") == arg.std_no &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Max(x => x.Field<Int16>("ser_id")) + 1;
                    }
                    else 
                    {
                        num = 1;
                    }
                    arg.ser_id = num;
                    str_sql = @"
                            insert L01_stu_other
                            (sch_no,year_id,sms_id,ser_id,std_no,
                            other_name,
                            unit_name,
                            startdate,
                            enddate,
                            hours,
                            content,is_sys,upd_name,upd_dt)
                            values
                            (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                             @other_name,
                             @unit_name,
                             @startdate,
                             @enddate,
                             @hours,
                             @content,@is_sys,@upd_name,@upd_dt)
                        ";
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "N");
                    dict.Add("result", result_count.ToString());
                    
                    return dict;
                }
            }
        }

        public async Task<int> UpdateDataCentraldb(StuOtherQueryList arg)
        {
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            foreach (var item in arg.complex_array)
            {
                sb.Append(string.Format(@"
                                update L01_stu_other
                                set 
                                    check_centraldb = case when @arg2_{0} ='true' then 'Y' else 'N' end
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+std_no+'_'+convert(varchar,ser_id) = @arg1_{1}
                            ", i, j));

                dynamicParams.Add("arg1_" + i, item.Split('@')[0]);
                dynamicParams.Add("arg2_" + j, item.Split('@')[1]);
                i++;
                j++;
            };
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
            }
        }
    }
}
