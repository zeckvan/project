using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuVolunteer.DbModels;
using StudentLearningHistory.Models.StuVolunteer.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuResult.Parameters;

namespace StudentLearningHistory.Services
{
    public class StuVolunteerService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public StuVolunteerService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public async Task<IEnumerable<HeaderList>> GetList(StuVolunteerQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L01_std_public_filehub a
                                    where a.class_name = 'StuVolunteer'
                                    and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+convert(varchar,NewTable.e)) as x_cnt,
                                    '' as x_status
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY year_id, sms_id, startdate,enddate) AS RowNum,
                                                sch_no as a,
                                                year_id as b,
                                                sms_id as c,
                                                std_no as d,
                                                ser_id as e,
                                                volunteer_name as f,
                                                volunteer_unit as g,
                                                startdate as h,
                                                enddate as i,
                                                hours as j,
                                                content as k,
                                                is_sys as l,
                                                check_centraldb as zz
                                        FROM L01_stu_volunteer
                                        WHERE sch_no = @sch_no and year_id = @year_id and sms_id = @sms_id and std_no = @std_no
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<L01_StuVolunteer>> GetFormData(StuVolunteerQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *
                                FROM L01_stu_volunteer
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_StuVolunteer>(str_sql, arg);
            }
        }

        public async Task<int> DeleteData(StuVolunteerQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                delete                                
                                FROM L01_stu_volunteer
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

        public async Task<int> UpdateData(L01_StuVolunteer arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;

            string str_sql = @"
                                update L01_stu_volunteer
                                set 
                                    volunteer_name = @volunteer_name,
                                    volunteer_unit = @volunteer_unit,
                                    startdate = @startdate,
                                    enddate = @enddate,
                                    hours = @hours,
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

        public async Task<Dictionary<string, string>> InsertData(L01_StuVolunteer arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L01_stu_volunteer 
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
                              where r.Field<string>("volunteer_name") == arg.volunteer_name &&
                                    r.Field<string>("volunteer_unit") == arg.volunteer_unit &&
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
                            insert L01_stu_volunteer
                            (sch_no,year_id,sms_id,ser_id,std_no,
                            volunteer_name,
                            volunteer_unit,
                            startdate,
                            enddate,
                            hours,
                            content,is_sys,upd_name,upd_dt)
                            values
                            (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                             @volunteer_name,
                             @volunteer_unit,
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

        public async Task<int> UpdateDataCentraldb(StuVolunteerQueryList arg)
        {
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            foreach (var item in arg.complex_array)
            {
                sb.Append(string.Format(@"
                                update L01_stu_volunteer
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
