using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuConsult.DbModels;
using StudentLearningHistory.Models.StuConsult.Parameters;
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
    public class StuConsultService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public StuConsultService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public async Task<IEnumerable<HeaderList>> GetList(StuConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L03_tea_consult_stu a
                                    where NewTable.a = a.sch_no 
                                        and NewTable.b = a.year_id 
                                        and NewTable.c = a.sms_id 
                                        and NewTable.d = a.emp_id
                                        and NewTable.e = a.ser_id) as x_stucnt,
                                    (select count(*)
                                    from L01_std_public_filehub a
                                    where a.class_name = 'TeaConsult'
                                    and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+convert(varchar,NewTable.e)) as x_cnt,
                                '' as x_status
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY L03_tea_consult.year_id, L03_tea_consult.sms_id, L03_tea_consult.consult_date) AS RowNum,
                                                L03_tea_consult.sch_no as a,
                                                L03_tea_consult.year_id as b,
                                                L03_tea_consult.sms_id as c,
                                                L03_tea_consult.emp_id as d,
                                                L03_tea_consult.ser_id as e,
                                                L03_tea_consult.consult_date as f,
                                                L03_tea_consult.consult_area as g,
                                                L03_tea_consult.consult_type as h,
                                                L03_tea_consult.consult_subject as i,
                                                L03_tea_consult.consult_content as j,
                                                L03_tea_consult.is_sys as k
                                        FROM L03_tea_consult
                                        join L03_tea_consult_stu on 
                                                L03_tea_consult_stu.sch_no = L03_tea_consult.sch_no and
                                                L03_tea_consult_stu.year_id = L03_tea_consult.year_id and
                                                L03_tea_consult_stu.sms_id = L03_tea_consult.sms_id and
                                                L03_tea_consult_stu.emp_id = L03_tea_consult.emp_id and
                                                L03_tea_consult_stu.ser_id = L03_tea_consult.ser_id
                                        WHERE L03_tea_consult_stu.sch_no = @sch_no 
                                        and L03_tea_consult_stu.year_id = @year_id 
                                        and L03_tea_consult_stu.sms_id = @sms_id 
                                        and L03_tea_consult_stu.std_no = @std_no
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<L03_TeaConsult>> GetFormData(StuConsulteQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT L03_tea_consult.*,
                                       s90_employee.emp_name
                                FROM L03_tea_consult
                                left join s90_employee on
                                    s90_employee.emp_id = L03_tea_consult.emp_id
                                WHERE L03_tea_consult.sch_no = @sch_no 
                                and L03_tea_consult.year_id = @year_id 
                                and L03_tea_consult.sms_id = @sms_id 
                                and L03_tea_consult.emp_id = @emp_id
                                and L03_tea_consult.ser_id = @ser_id;
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L03_TeaConsult>(str_sql, arg);
            }
        }

    }
}
