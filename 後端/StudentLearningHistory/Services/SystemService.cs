using Dapper;
using DocumentFormat.OpenXml.Drawing;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.Public.DbModels;
using StudentLearningHistory.Models.StuWorkplace.DbModels;
using StudentLearningHistory.Models.System.DbModels;
using System.Collections.Generic;
//using StudentLearningHistory.Models.System.Parameters;
using System.Data;
using System.Text;

namespace StudentLearningHistory.Services
{
    public class SystemService
    {
        private readonly IDapperContext _context;
        private string updteDate() => $"{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        public SystemService(IDapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<L01_setup>> Get_L01_setup()
        {
            string str_sql = "select * from L01_setup order by ID";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_setup>(str_sql);
            }
        }

        public async Task<int> UpdateDataSetUp(L01_setup_array arg)
        {
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            int k = 1;
            int l = 1;
            foreach (var item in arg.Name)
            {
                sb.Append(string.Format(@"
                                       update L01_setup
                                       set 
                                           value =  @arg1_{0},
                                            unit = @arg2_{1},
                                           memo = @arg3_{2}
                                        WHERE name = @arg4_{3}
                                   ", i, j, k, l));
                dynamicParams.Add("arg1_" + i, arg.Value[i - 1]);
                dynamicParams.Add("arg2_" + j, arg.Unit[j - 1]);
                dynamicParams.Add("arg3_" + k, arg.Memo[k - 1]);
                dynamicParams.Add("arg4_" + l, item);
                i++;
                j++;
                k++;
                l++;
            };

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
            }
        }

        public async Task<IEnumerable<L01_operate>> Get_L01_operate()
        {
            int year_id = _context.now_year;
            string str_sql = @"
                               select   year_id,
                                        sms_id,
                                        grade_id,
                                        type_id,
                                        startdate =                     
                                                    substring(startdate,1,4)+
                                                    '-'+
                                                    substring(startdate,5,2)+
                                                    '-'+
                                                    substring(startdate,7,2)+
                                                    ' '+
                                                    substring(startdate,9,2)+
                                                    ':'+
                                                    substring(startdate,11,2)+
                                                    ':'+
                                                    substring(startdate,13,2),
                                        enddate = 
                                                    substring(enddate,1,4)+
                                                    '-'+
                                                    substring(enddate,5,2)+
                                                    '-'+
                                                    substring(enddate,7,2)+
                                                    ' '+
                                                    substring(enddate,9,2)+
                                                    ':'+
                                                    substring(enddate,11,2)+
                                                    ':'+
                                                    substring(enddate,13,2)
                               from L01_operate
                               order by L01_operate.year_id,L01_operate.sms_id,TYPE_ID
                              ";
            //string str_sql = @"
            //                   select   year_id,
            //                            sms_id,
            //                            grade_id,
            //                            type_id,
            //                            startdate =                     
            //                                        substring(startdate,1,4)+
            //                                        '-'+
            //                                        substring(startdate,5,2)+
            //                                        '-'+
            //                                        substring(startdate,7,2)+
            //                                        ' '+
            //                                        substring(startdate,9,2)+
            //                                        ':'+
            //                                        substring(startdate,11,2)+
            //                                        ':'+
            //                                        substring(startdate,13,2),
            //                            enddate = 
            //                                        substring(enddate,1,4)+
            //                                        '-'+
            //                                        substring(enddate,5,2)+
            //                                        '-'+
            //                                        substring(enddate,7,2)+
            //                                        ' '+
            //                                        substring(enddate,9,2)+
            //                                        ':'+
            //                                        substring(enddate,11,2)+
            //                                        ':'+
            //                                        substring(enddate,13,2)
            //                   from L01_operate,s90_yms
            //                  where L01_operate.year_id = s90_yms.yms_year and s90_yms.yms_mark = 'Y' order by L01_operate.year_id,L01_operate.sms_id,TYPE_ID
            //                  ";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_operate>(str_sql, new { year_id });
            }
        }

        public async Task<int> UpdateDataOperate(L01_operate_array arg)
        {
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int a = 1;
            int b = 1;
            int c = 1;
            int d = 1;
            int e = 1;
            int f = 1;

            foreach (var item in arg.year_id)
            {
                sb.Append(string.Format(@"
                                       update L01_operate
                                       set 
                                           startdate =  @arg1_{0},
                                           enddate = @arg2_{1}
                                        WHERE year_id = @arg3_{2}
                                        and sms_id = @arg4_{3}
                                        and grade_id = @arg5_{4}
                                        and type_id = @arg6_{5}
                                   ", a, b, c, d, e, f));

                //sb.Append(string.Format(@"
                //                       update L01_operate
                //                       set 
                //                            type_id = @arg4_{3}
                //                        WHERE year_id = @arg1_{0}
                //                        and sms_id = @arg2_{1}
                //                        and grade_id = @arg3_{2}                                        
                //                   ", a, b, c, d));


                //dynamicParams.Add("arg1_" + a, item);
                //dynamicParams.Add("arg2_" + b, arg.sms_id[d - 1]);
                //dynamicParams.Add("arg3_" + c, arg.type_id[e - 1]);
                //dynamicParams.Add("arg4_" + d, arg.type_id[f - 1]);

                dynamicParams.Add("arg1_" + a, arg.startdate[a - 1]);
                dynamicParams.Add("arg2_" + b, arg.enddate[b - 1]);
                dynamicParams.Add("arg3_" + c, item);
                dynamicParams.Add("arg4_" + d, arg.sms_id[d - 1]);
                dynamicParams.Add("arg5_" + e, arg.grade_id[e - 1]);
                dynamicParams.Add("arg6_" + f, arg.type_id[f - 1]);
                a++;
                b++;
                c++;
                d++;
                e++;
                f++;
            };

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
            }
        }

        public async Task<L01_operate> Get_L01_operate_open(L01_operate arg)
        {
            string str_sql = string.Empty;
            arg.x_date = updteDate();
            if (arg.kind_id == "stu")
            {
                 str_sql = string.Format(@"select startdate,enddate
                                        from L01_operate 
                                        where year_id = 111
                                        and   type_id = @type_id
                                        and   ((@sms_id = 0) or (sms_id = @sms_id))
                                        and grade_id = (select distinct grd_id
                                                                        from s04_stuhcls
                                                                        where s04_stuhcls.year_id = @year_id
                                                                        and s04_stuhcls.sms_id in(1,2)
                                                                        and s04_stuhcls.std_no = '{0}')
                                                ", _context.user_id);
            }
            else 
            {
                 str_sql = @"select startdate,enddate
                                                from L01_operate 
                                                where year_id = 111
                                                and   grade_id = @grade_id
                                                and   type_id = @type_id
                                                and   ((@sms_id = 0) or (sms_id = @sms_id))
                                                ";
            }

            using (IDbConnection conn = _context.CreateCommand())
            {
                string start = string.Empty;
                string end = string.Empty;

                IEnumerable<L01_operate> data = await conn.QueryAsync<L01_operate>(str_sql, arg);

                L01_operate rtn = new L01_operate();

                foreach (var item in data)
                {
                    rtn.startdate = item.startdate;
                    rtn.enddate = item.enddate;

                    if (Convert.ToDecimal(arg.x_date) >= Convert.ToDecimal(item.startdate) && Convert.ToDecimal(arg.x_date) <= Convert.ToDecimal(item.enddate))
                    {
                        rtn.open_yn = "Y";
                    }
                    else
                    {
                        rtn.open_yn = "N";
                    }
                }
                return rtn;
            }
        }

        public async Task<IEnumerable<L01_Diverse_Total>> Get_Diverse_Total(L01_Diverse_Total arg)
        {
            arg.std_no = _context.user_id;
            string str_sql = @"
                                                select　
                                                x_total = 
                                                isnull((select value from L01_setup where id = 4),0),
                                                x_1　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_std_position
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
                                                    and is_sys = '2'
	                                                and　check_centraldb　=　'Y'),0),
                                                x_1_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_std_position
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
                                                    and is_sys = '2'),0),
                                                x_2　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_competition
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_2_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_competition
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                x_3　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_license
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_3_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_license
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                 x_4　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_volunteer
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_4_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_volunteer
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                 x_5　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_result
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_5_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_result
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                x_6　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_other
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_6_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_other
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                x_7　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_study_free
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),        
                                                x_7_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_study_free
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                x_8　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_workplace
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_8_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_workplace
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                x_9　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_college
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_9_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_college
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0),
                                                 x_10　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_group
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no
	                                                and　check_centraldb　=　'Y'),0),
                                                x_10_tot　=　
                                                isnull(
	                                                (select　count(*)
	　                                                from　L01_stu_group
	                                                where　year_id　=　@year_id
	                                                and　std_no　=　@std_no),0)
                                                from　s04_student
                                                where　std_no　=　@std_no
                                                ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_Diverse_Total>(str_sql, arg);
            }
        }
    }
}
