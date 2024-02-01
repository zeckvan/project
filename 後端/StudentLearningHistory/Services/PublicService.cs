using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.Public.DbModels;
using StudentLearningHistory.Models.Public.Parameters;
using System.Data;

namespace StudentLearningHistory.Services
{
    public class PublicService
    {
        private readonly IDapperContext _context;
        public PublicService(IDapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<S90_Yms>> Get_s90_yms(string yms_mark)
        {
            string str_sql = "select * from s90_yms where yms_mark like @yms_mark order by yms_year desc,yms_smester";

            if (yms_mark == "All")
            {
                yms_mark = "%";
            }
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S90_Yms>(str_sql, new { yms_mark = yms_mark });
            }
        }
        public async Task<IEnumerable<S90_Year>> Get_s90_year()
        {
            string str_sql = "select * from s90_year order by year_id desc";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S90_Year>(str_sql);
            }
        }
        public async Task<IEnumerable<S90_Sms>> Get_s90_sms()
        {
            string str_sql = "select * from s90_sms where sms_id <> 99 order by sms_id";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S90_Sms>(str_sql);
            }
        }
        public async Task<IEnumerable<S90_employee>> Get_s90_employee(S90_employee_Param arg)
        {
            string str_sql = @"
                                select *,
                                        x_total = 
                                        (
                                            select count(*)
                                            from s90_employee 
                                            where s90_employee.is_leave = 'N'
                                        ),
                                        x_status = ''
                                from (
                                        select
                                            ROW_NUMBER() OVER(ORDER BY emp_name) AS RowNum,
                                            s90_employee.emp_id,	--教師內碼
                                            s90_employee.emp_code,	--教師代碼
                                            s90_employee.emp_name,	--教師姓名
                                            s90_employee.emp_email	--教師e-mail
                                        from
                                            s90_employee
                                        where s90_employee.is_leave = 'N') AS NewTable
                                  WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S90_employee>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<S04_ytdbgoc>> Get_s04_ytdbgoc(S04_ytdbgoc arg)
        {
            arg.year_id = _context.now_year;
            arg.sms_id = _context.now_sms;
            string str_sql = @"
                                                select cls_id,cls_abr
                                                from (
                                                            select 
                                                                  cls_id =
                                                                                         convert(varchar(1),s04_ytdbgoc.dep_id) + 
                                                                                         case when s04_ytdbgoc.bra_id >= 10 then convert(varchar(2),s04_ytdbgoc.bra_id) else '0' + convert(varchar(1),s04_ytdbgoc.bra_id) end + 
                                                                                         convert(varchar(1),s04_ytdbgoc.grd_id) + 
                                                                                         case when s04_ytdbgoc.cls_id >= 10 then convert(varchar(2),s04_ytdbgoc.cls_id) else '0' + convert(varchar(1),s04_ytdbgoc.cls_id) end,
                                                                 cls_abr
                                                            from s04_ytdbgoc
                                                            where year_id = @year_id and sms_id = @sms_id
 
                                                            union

                                                            select
                                                                cls_id =convert(varchar(6) , s04_noropenc.noroc_id ),
                                                                cls_abr = noroc_clsname
                                                            from s04_noropenc 
                                                            where year_id = @year_id and sms_id = @sms_id and noroc_bclass = 'N'
                                                            ) a order by cls_id
                                 ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S04_ytdbgoc>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<S04_ytdbgoc>> Get_s04_ytdbgoc_page(S04_ytdbgoc arg)
        {
            arg.year_id = _context.now_year;
            arg.sms_id = _context.now_sms;
            string str_sql = @"
                                                    select *,
		                                                    x_total = 
					                                                    (
						                                                    select  count(*)
						                                                    from s04_ytdbgoc
						                                                     where year_id = @year_id and sms_id = @sms_id
					                                                    ),
                                                            year_id = @year_id,
                                                            sms_id = @sms_id
                                                    from 
                                                    (
	                                                       select *,
				                                                    RowNum = ROW_NUMBER() OVER(ORDER BY cls_code)
		                                                    from (
				                                                    select 
						                                                    cls_code =
												                                                    convert(varchar(1),s04_ytdbgoc.dep_id) + 
												                                                    case when s04_ytdbgoc.bra_id >= 10 then convert(varchar(2),s04_ytdbgoc.bra_id) else '0' + convert(varchar(1),s04_ytdbgoc.bra_id) end + 
												                                                    convert(varchar(1),s04_ytdbgoc.grd_id) + 
												                                                    case when s04_ytdbgoc.cls_id >= 10 then convert(varchar(2),s04_ytdbgoc.cls_id) else '0' + convert(varchar(1),s04_ytdbgoc.cls_id) end,
						                                                    cls_abr,
		                                                                    deg_id,
		                                                                    dep_id,
		                                                                    bra_id,
		                                                                    grd_id,
		                                                                    cls_id
				                                                    from s04_ytdbgoc
				                                                    where year_id = @year_id and sms_id = @sms_id
			                                                    )a
                                                    )NewTable
                                                    WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                                    order by cls_code
                                 ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S04_ytdbgoc>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<S04_ytdbgoc>> Get_s04_ytdbgocAll_page(S04_ytdbgoc arg)
        {
            string str_sql = @"
                                                    select *,
		                                                    x_total = 
					                                                    (
						                                                    select  count(*)
						                                                    from s04_ytdbgoc
						                                                     where year_id = @year_id and sms_id = @sms_id
					                                                    )
					                                                    +
					                                                    (
						                                                    select  count(*)
						                                                    from s04_noropenc 
						                                                     where year_id = @year_id and sms_id = @sms_id and noroc_bclass = 'N'
					                                                    )
                                                    from 
                                                    (
	                                                       select	cls_id,
				                                                    cls_abr,
				                                                    RowNum = ROW_NUMBER() OVER(ORDER BY cls_id)
		                                                    from (
				                                                    select 
						                                                    cls_id =
												                                                    convert(varchar(1),s04_ytdbgoc.dep_id) + 
												                                                    case when s04_ytdbgoc.bra_id >= 10 then convert(varchar(2),s04_ytdbgoc.bra_id) else '0' + convert(varchar(1),s04_ytdbgoc.bra_id) end + 
												                                                    convert(varchar(1),s04_ytdbgoc.grd_id) + 
												                                                    case when s04_ytdbgoc.cls_id >= 10 then convert(varchar(2),s04_ytdbgoc.cls_id) else '0' + convert(varchar(1),s04_ytdbgoc.cls_id) end,
						                                                    cls_abr
				                                                    from s04_ytdbgoc
				                                                    where year_id = @year_id and sms_id = @sms_id
 
				                                                    union

				                                                    select
					                                                    cls_id =convert(varchar(6) , s04_noropenc.noroc_id ),
					                                                    cls_abr = noroc_clsname
				                                                    from s04_noropenc 
				                                                    where year_id = @year_id and sms_id = @sms_id and noroc_bclass = 'N'
			                                                    )a
                                                    )NewTable
                                                    WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                                    order by cls_id
                                 ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S04_ytdbgoc>(str_sql, arg);
            }
        }
        public async Task<IEnumerable<S04_student>> Get_04_student(S04_student arg)
        {
            arg.std_no = arg.std_no_q ?? "%";
            arg.std_name = arg.std_name_q ?? "%";
            arg.cls_id = arg.cls_id_q ?? "%";
            arg.year_id = _context.now_year;
            arg.sms_id = _context.now_sms;
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                    select 
                                        x_status = '',
	                                    s04_student.std_no,
                                        s04_student.std_name,
	                                    s04_ytdbgoc.cls_abr,
	                                    RowNum = ROW_NUMBER() OVER(ORDER BY s04_ytdbgoc.cls_abr,s04_student.std_no),
                                        sch_no = @sch_no
                                    from s04_student
                                    inner join s04_stuhcls on 
	                                    s04_stuhcls.std_no = s04_student.std_no
                                    inner join s04_ytdbgoc on 
	                                    s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                    s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                    s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                    s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                    s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                    s04_ytdbgoc.cls_id = s04_stuhcls.cls_id and
	                                    ((@cls_id = '%') or 
	                                        (
		                                    convert(varchar(1),s04_ytdbgoc.dep_id) + 
		                                    case when s04_ytdbgoc.bra_id >= 10 then convert(varchar(2),s04_ytdbgoc.bra_id) else '0' + convert(varchar(1),s04_ytdbgoc.bra_id) end + 
		                                    convert(varchar(1),s04_ytdbgoc.grd_id) + 
		                                    case when s04_ytdbgoc.cls_id >= 10 then convert(varchar(2),s04_ytdbgoc.cls_id) else '0' + convert(varchar(1),s04_ytdbgoc.cls_id) end = @cls_id
	                                        ))
                                    where 
	                                    s04_stuhcls.year_id = @year_id and
	                                    s04_stuhcls.sms_id = @sms_id and
	                                    s04_stuhcls.std_no like @std_no and
	                                    s04_student.std_name like @std_name
                                    order by
                                    s04_ytdbgoc.cls_abr,s04_student.std_no
                                 ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<S04_student>(str_sql, arg);
            }
        }
    }
}
