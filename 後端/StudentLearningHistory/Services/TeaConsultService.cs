using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.TeaConsult.DbModels;
using StudentLearningHistory.Models.TeaConsult.Parameters;
using StudentLearningHistory.Models.Public.DbModels;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace StudentLearningHistory.Services
{
    public class TeaConsultService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public class GetEmpId
        {
            public string emp_id { get; set; }
        }
        public TeaConsultService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public async Task<IEnumerable<SubHeaderList>> GetConsultStuList(TeaConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L03_tea_consult_stu a
                                    where NewTable.sch_no = a.sch_no 
                                        and NewTable.year_id = a.year_id 
                                        and NewTable.sms_id = a.sms_id 
                                        and NewTable.emp_id = a.emp_id
                                        and NewTable.ser_id = a.ser_id) as x_cnt
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY s04_ytdbgoc.cls_abr,s04_student.std_no) AS RowNum,
                                                s04_ytdbgoc.cls_abr as a,
                                                s04_student.std_name as c,
                                                s04_student.std_no as b,
                                                d = s04_stuhcls.sit_num,
                                                L03_tea_consult_stu.*
                                        FROM L03_tea_consult_stu
                                        join s04_student on s04_student.std_no = L03_tea_consult_stu.std_no
                                        inner join s04_stuhcls on 
                                            s04_stuhcls.std_no = s04_student.std_no
                                        inner join s04_ytdbgoc on 
                                            s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                                            s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                                            s04_ytdbgoc.deg_id = s04_stuhcls.deg_id and
                                            s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                                            s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                                            s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                                            s04_ytdbgoc.cls_id = s04_stuhcls.cls_id AND 
                                            s04_ytdbgoc.year_id = L03_tea_consult_stu.year_id and
                                            s04_ytdbgoc.sms_id = L03_tea_consult_stu.sms_id
                                        WHERE L03_tea_consult_stu.sch_no = @sch_no 
                                        and L03_tea_consult_stu.year_id = @year_id 
                                        and L03_tea_consult_stu.sms_id = @sms_id 
                                        and L03_tea_consult_stu.emp_id = @emp_id
                                        and L03_tea_consult_stu.ser_id = @ser_id
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<SubHeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<SubHeaderList>> GetStuList(TeaConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from s04_stuhcls a
                                    join s04_student b on b.std_no = a.std_no and b.std_name like @std_name_q
                                    where  a.sch_no = a.sch_no and             
                                            a.year_id = NewTable.year_id and
                                            a.sms_id = NewTable.sms_id and
                                            a.dep_id = NewTable.dep_id and
                                            a.bra_id = NewTable.bra_id and
                                            a.grd_id = NewTable.grd_id and
                                            a.cls_id = NewTable.cls_id ) as x_cnt
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY s04_ytdbgoc.cls_abr,s04_student.std_no) AS RowNum,
                                                s04_ytdbgoc.cls_abr as a,
                                                s04_student.std_name as c,
                                                s04_student.std_no as b,
                                                d = s04_stuhcls.sit_num,
                                                L03_tea_consult_setup.*
                                        FROM s04_student
                                        inner join s04_stuhcls on 
                                            s04_stuhcls.std_no = s04_student.std_no
                                        inner join s04_ytdbgoc on 
                                            s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                                            s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                                            s04_ytdbgoc.deg_id = s04_stuhcls.deg_id and
                                            s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                                            s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                                            s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                                            s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                        join L03_tea_consult_setup on 
                                                s04_ytdbgoc.sch_no = L03_tea_consult_setup.sch_no and             
                                                s04_ytdbgoc.year_id = L03_tea_consult_setup.year_id and
                                                s04_ytdbgoc.sms_id = L03_tea_consult_setup.sms_id and
                                                s04_ytdbgoc.deg_id = L03_tea_consult_setup.deg_id and
                                                s04_ytdbgoc.dep_id = L03_tea_consult_setup.dep_id and
                                                s04_ytdbgoc.bra_id = L03_tea_consult_setup.bra_id and
                                                s04_ytdbgoc.grd_id = L03_tea_consult_setup.grd_id and
                                                s04_ytdbgoc.cls_id = L03_tea_consult_setup.cls_id 
                                        WHERE L03_tea_consult_setup.sch_no = @sch_no 
                                        and L03_tea_consult_setup.year_id = @year_id 
                                        and L03_tea_consult_setup.sms_id = @sms_id 
                                        and L03_tea_consult_setup.emp_id = @emp_id 
                                        and convert(varchar,s04_ytdbgoc.deg_id)+'_'+convert(varchar,s04_ytdbgoc.dep_id)+'_'+convert(varchar,s04_ytdbgoc.bra_id)+'_'+convert(varchar,s04_ytdbgoc.grd_id)+'_'+convert(varchar,s04_ytdbgoc.cls_id) like @cls_id_q
                                        and s04_student.std_name like @std_name_q
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<SubHeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<HeaderList>> GetList(TeaConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L01_std_public_filehub a
                                    where a.class_name = 'TeaConsult'
                                    and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+convert(varchar,NewTable.e)) as x_cnt
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY year_id, sms_id, consult_date) AS RowNum,
                                                sch_no as a,
                                                year_id as b,
                                                sms_id as c,
                                                emp_id as d,
                                                ser_id as e,
                                                consult_date as f,
                                                consult_area as g,
                                                case consult_type when '1' then '團體諮詢' when '2' then '個別諮詢' else '' end as h,
                                                consult_subject as i,
                                                consult_content as j,
                                                is_sys as k
                                        FROM L03_tea_consult
                                        WHERE sch_no = @sch_no and year_id = @year_id and sms_id = @sms_id and emp_id = @emp_id
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
            using (IDbConnection conn = _context.CreateCommand())
            {
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
                arg.emp_id = getemp.ElementAt(0).emp_id;
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<L03_TeaConsult>> GetFormData(TeaConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *
                                FROM L03_tea_consult
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and emp_id = @emp_id
                                and ser_id = @ser_id;
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L03_TeaConsult>(str_sql, arg);
                /*
                    using (var multi = conn.QueryMultiple(sql,arg))
                    {
                        var invoice = multi.First<L01_TeaConsult>();
                        var invoiceItems = multi.Read<L01_TeaConsult_Stu>();
                        ReadAsync
                    }
                */
            }
        }

        public async Task<IEnumerable<S90_Class>> GetCls(TeaConsultQueryStu arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                select distinct
                                    s04_ytdbgoc.cls_abr,
                                    convert(varchar,s04_ytdbgoc.deg_id)+'_'+convert(varchar,s04_ytdbgoc.dep_id)+'_'+convert(varchar,s04_ytdbgoc.bra_id)+'_'+convert(varchar,s04_ytdbgoc.grd_id)+'_'+convert(varchar,s04_ytdbgoc.cls_id) as cls_id
                                from s04_student
                                inner join s04_stuhcls on 
                                    s04_stuhcls.std_no = s04_student.std_no
                                inner join s04_ytdbgoc on 
                                    s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                                    s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                                    s04_ytdbgoc.deg_id = s04_stuhcls.deg_id and
                                    s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                                    s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                                    s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                                    s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
                                join L03_tea_consult_setup on 
                                     s04_ytdbgoc.sch_no = L03_tea_consult_setup.sch_no and             
                                     s04_ytdbgoc.year_id = L03_tea_consult_setup.year_id and
                                     s04_ytdbgoc.sms_id = L03_tea_consult_setup.sms_id and
                                     s04_ytdbgoc.deg_id = L03_tea_consult_setup.deg_id and
                                     s04_ytdbgoc.dep_id = L03_tea_consult_setup.dep_id and
                                     s04_ytdbgoc.bra_id = L03_tea_consult_setup.bra_id and
                                     s04_ytdbgoc.grd_id = L03_tea_consult_setup.grd_id and
                                     s04_ytdbgoc.cls_id = L03_tea_consult_setup.cls_id 
                                where 
                                    L03_tea_consult_setup.year_id = @year_id and
                                    L03_tea_consult_setup.sms_id = @sms_id and 
                                    L03_tea_consult_setup.emp_id = @emp_id and
                                    L03_tea_consult_setup.sch_no = @sch_no";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S90_Class>(str_sql, arg);
            }
        }

        public async Task<int> DeleteData(TeaConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                delete                                
                                FROM L03_tea_consult
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and emp_id = @emp_id
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<int> DeleteStuData(TeaConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                delete                                
                                FROM L03_tea_consult_stu
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and emp_id = @emp_id
                                and ser_id = @ser_id
                                and std_no in @std_no
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<int> UpdateData(L03_TeaConsult arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.emp_id;
            int rt = 0;

            string str_sql = @"
                                update L03_tea_consult
                                set 
                                    consult_date = @consult_date,
                                    consult_area = @consult_area,
                                    consult_type = @consult_type,
                                    consult_subject = @consult_subject,
                                    consult_content = @consult_content,
                                    is_sys = @is_sys,
                                    upd_name = @upd_name,
                                    upd_dt = @upd_dt
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and emp_id = @emp_id
                                and ser_id = @ser_id
                            ";
            string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
            using (IDbConnection conn = _context.CreateCommand())
            {
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
                arg.emp_id = getemp.ElementAt(0).emp_id;
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<Dictionary<string, string>> InsertData(L03_TeaConsult arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.emp_id;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L03_tea_consult 
                        where year_id = @year_id
                        and sms_id = @sms_id
                        and emp_id = @emp_id
                        and sch_no = @sch_no
                        ";
            string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
            using (IDbConnection conn = _context.CreateCommand())
            {
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
                arg.emp_id = getemp.ElementAt(0).emp_id;

                var dr = await conn.ExecuteReaderAsync(str_sql, arg);
                DataTable dt = new DataTable();
                dt.Load(dr);
                var result = (from r in dt.AsEnumerable()
                              where r.Field<string>("consult_date") == arg.consult_date &&
                                    r.Field<string>("consult_area") == arg.consult_area &&
                                    r.Field<string>("consult_type") == arg.consult_type &&
                                    r.Field<string>("consult_subject") == arg.consult_subject
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
                                     r.Field<string>("emp_id") == arg.emp_id &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Count();

                    if (num > 0)
                    {
                        num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("emp_id") == arg.emp_id &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Max(x => x.Field<Int16>("ser_id")) + 1;
                    }
                    else 
                    {
                        num = 1;
                    }
                    arg.ser_id = num;
                    str_sql = @"
                            insert L03_tea_consult
                            (sch_no,year_id,sms_id,ser_id,emp_id,
                            consult_date,
                            consult_area,
                            consult_type,
                            consult_subject,
                            consult_content,
                            is_sys,upd_name,upd_dt)
                            values
                            (@sch_no,@year_id,@sms_id,@ser_id,@emp_id,
                             @consult_date,
                             @consult_area,
                             @consult_type,
                             @consult_subject,
                             @consult_content,
                             @is_sys,@upd_name,@upd_dt)
                        ";
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "N");
                    dict.Add("result", result_count.ToString());
                    
                    return dict;
                }
            }
        }
     
        public async Task<Dictionary<string, string>>  InsertConsultStu(L03_TeaConsult_Stu arg)
        {
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.emp_id;
            string str_sql = "";
            var dict = new Dictionary<string, string>();

            using (IDbConnection conn = _context.CreateCommand())
            {               
                str_sql = @"
                            insert L03_tea_consult_stu
                            (sch_no,year_id,sms_id,ser_id,emp_id,std_no)
                            select 
                            @sch_no as sch_no,
                            @year_id as year_id,
                            @sms_id as sms_id,
                            @ser_id as ser_id,
                            @emp_id as emp_id,
                            s04_student.std_no as std_no
                            from s04_student
                            where std_no in @std_no
                            and not exists(select 1 from L03_tea_consult_stu a
                                            where a.sch_no = @sch_no
                                            and   a.year_id = @year_id
                                            and   a.sms_id = @sms_id
                                            and   a.emp_id = @emp_id
                                            and   a.ser_id = @ser_id
                                            and   a.std_no = s04_student.std_no)
                            ";
                try
                {
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "");
                    dict.Add("result", "1");
                }
                catch (Exception e) 
                {
                    dict.Add("haveData", "新增失敗:"+e.Message.ToString());
                    dict.Add("result", "0");
                }

                return dict;

            }
        }

        public async Task<IEnumerable<L03_TeaConsult_Stu>> Get_consult_setup(TeaConsultQuerySetup arg)
        {
            arg.year_id = _context.now_year;
            arg.sms_id = _context.now_sms;  

            string str_sql = @"
                                                    select *,
		                                                    x_total = 
					                                                    (
						                                                    select  count(*)
						                                                    from L03_tea_consult_setup
						                                                     where year_id = @year_id and sms_id = @sms_id
					                                                    )					                                                  
                                                    from 
                                                    (
	                                                       select	*,
				                                                    RowNum = ROW_NUMBER() OVER(ORDER BY emp_id,deg_id,dep_id,bra_id,grd_id,cls_id)
		                                                    from (
				                                                    select 
                                                                             s04_ytdbgoc.sch_no ,
                                                                             s04_ytdbgoc.year_id ,
                                                                             s04_ytdbgoc.sms_id ,
                                                                             s04_ytdbgoc.deg_id ,
                                                                            s04_ytdbgoc.dep_id ,
                                                                             s04_ytdbgoc.bra_id ,
                                                                             s04_ytdbgoc.grd_id ,
                                                                             s04_ytdbgoc.cls_id ,
                                                                            s04_ytdbgoc.cls_abr,
                                                                            s90_employee.emp_id,
                                                                            s90_employee.emp_name
				                                                    from s04_ytdbgoc
                                                                     join L03_tea_consult_setup on 
                                                                                                        L03_tea_consult_setup.sch_no = s04_ytdbgoc.sch_no and
                                                                                                        L03_tea_consult_setup.year_id = s04_ytdbgoc.year_id and
                                                                                                        L03_tea_consult_setup.sms_id = s04_ytdbgoc.sms_id and
                                                                                                        L03_tea_consult_setup.deg_id = s04_ytdbgoc.deg_id and
                                                                                                        L03_tea_consult_setup.dep_id = s04_ytdbgoc.dep_id and
                                                                                                        L03_tea_consult_setup.bra_id = s04_ytdbgoc.bra_id and
                                                                                                        L03_tea_consult_setup.grd_id = s04_ytdbgoc.grd_id and
                                                                                                        L03_tea_consult_setup.cls_id = s04_ytdbgoc.cls_id 
                                                                     join s90_employee on 
                                                                                                        s90_employee.emp_id = L03_tea_consult_setup.emp_id 
				                                                    where s04_ytdbgoc.year_id = @year_id and s04_ytdbgoc.sms_id = @sms_id
			                                                    )a
                                                    )NewTable
                                                    WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                                    order by emp_id,deg_id,dep_id,bra_id,grd_id,cls_id
                                 ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L03_TeaConsult_Stu>(str_sql, arg);
            }
        }

        public async Task<int> DeleteConsult_SetUp(TeaConsultDeleteList arg)
        {
            //arg.sch_no = _schno;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;

            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int a= 1;
            int b = 1;
            int c = 1;
            int d = 1;
            int e = 1;
            int f = 1;
            int g = 1;
            int h = 1;

            foreach (var item in arg.year_id)
            {
                sb.Append(string.Format(@"
                                delete 
                                from L03_tea_consult_setup
                                WHERE year_id = @arg0_{0}
                                and sms_id = @arg1_{1}
                                and deg_id = @arg2_{2}
                                and dep_id = @arg3_{3}
                                and bra_id = @arg4_{4}
                                and grd_id = @arg5_{5}
                                and cls_id = @arg6_{6}
                                and emp_id = @arg7_{7}
                            ", a,b,c,d,e,f,g,h));

                dynamicParams.Add("arg0_" + a, item);
                dynamicParams.Add("arg1_" + b, arg.sms_id[b-1]);
                dynamicParams.Add("arg2_" + c, arg.deg_id[c - 1]);
                dynamicParams.Add("arg3_" + d, arg.dep_id[d - 1]);
                dynamicParams.Add("arg4_" + e, arg.bra_id[e - 1]);
                dynamicParams.Add("arg5_" + f, arg.grd_id[f - 1]);
                dynamicParams.Add("arg6_" + g, arg.cls_id[g - 1]);
                dynamicParams.Add("arg7_" + h, arg.emp_id[h - 1]);


                a++;
                b++;
                c++;
                d++;
                e++;
                f++;
                g++;
                h++;
            };

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
            }
        }

        public async Task<Dictionary<string, string>> InsertConsult_SetUp(TeaConsultDeleteList arg)
        {
            arg.sch_no = _context.SchNo;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;
            var dict = new Dictionary<string, string>();
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int a = 1;
            int b = 1;
            int c = 1;
            int d = 1;
            int e = 1;
            int f = 1;
            int g = 1;
            int h = 1;
            int i = 1;
            int j = 1;

            foreach (var item in arg.year_id)
            {
                sb.Append(string.Format(@"
                                insert into L03_tea_consult_setup
                                (sch_no,year_id,sms_id,deg_id,dep_id,bra_id,grd_id,cls_id,emp_id)
				                select  distinct
                                            sch_no,
                                            year_id,
                                            sms_id,
                                            deg_id,
                                            dep_id,
                                            bra_id,
                                            grd_id,
                                            cls_id,
                                            @arg9_{9} 
				                from s04_ytdbgoc
				                where sch_no = @arg0_{0}
                                and year_id = @arg1_{1} 
                                and sms_id = @arg2_{2} 
                                and deg_id = @arg3_{3} 
                                and dep_id = @arg4_{4} 
                                and bra_id = @arg5_{5} 
                                and grd_id = @arg6_{6} 
                                and cls_id = @arg7_{7} 
                                and not exists(select 1 from L03_tea_consult_setup a
                                where a.year_id = s04_ytdbgoc.year_id
                                and a.sms_id = s04_ytdbgoc.sms_id
                                and a.deg_id = s04_ytdbgoc.deg_id
                                and a.dep_id = s04_ytdbgoc.dep_id
                                and a.bra_id = s04_ytdbgoc.bra_id
                                and a.grd_id = s04_ytdbgoc.grd_id
                                and a.cls_id = s04_ytdbgoc.cls_id
                                and a.emp_id =  @arg8_{8} )
                            ", a, b, c, d, e, f, g, h, i,j));

                dynamicParams.Add("arg0_" + a, _context.SchNo);
                dynamicParams.Add("arg1_" + b, item);
                dynamicParams.Add("arg2_" + c, arg.sms_id[c - 1]);
                dynamicParams.Add("arg3_" + d, arg.deg_id[d - 1]);
                dynamicParams.Add("arg4_" + e, arg.dep_id[e - 1]);
                dynamicParams.Add("arg5_" + f, arg.bra_id[f - 1]);
                dynamicParams.Add("arg6_" + g, arg.grd_id[g - 1]);
                dynamicParams.Add("arg7_" + h, arg.cls_id[h - 1]);
                dynamicParams.Add("arg8_" + i, arg.emp_id[i - 1]);
                dynamicParams.Add("arg9_" + j, arg.emp_id[i - 1]);

                a++;
                b++;
                c++;
                d++;
                e++;
                f++;
                g++;
                h++;
                i++;
                j++;
            };

            using (IDbConnection conn = _context.CreateCommand())
            {
                try
                {
                    var result_count = await conn.ExecuteAsync(sb.ToString(), dynamicParams);
                    dict.Add("haveData", "");
                    dict.Add("result", "1");
                }
                catch (Exception error)
                {                
                    dict.Add("haveData", "新增失敗:" + error.Message.ToString());
                    dict.Add("result", "0");
                }
                return dict;
            }
        }
    }
}
