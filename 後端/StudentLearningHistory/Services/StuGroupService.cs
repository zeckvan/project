using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuGroup.DbModels;
using StudentLearningHistory.Models.StuGroup.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuCollege.Parameters;
using StudentLearningHistory.Models.StuCompetition.Parameters;

namespace StudentLearningHistory.Services
{
    public class StuGroupService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public StuGroupService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public StringBuilder GetStructure(string table_name)
        {
            string str_sql = $@"
                                select distinct
                                    a.Table_schema +'.'+a.Table_name   as table_name,
                                    b.COLUMN_NAME                     as col_name, 
                                    b.DATA_TYPE                       as col_type,
                                    isnull(b.CHARACTER_MAXIMUM_LENGTH,'') as col_length,
                                    isnull(b.COLUMN_DEFAULT,'')           as col_default,
                                    b.IS_NULLABLE                         as col_isnull,   
                                    ( SELECT value   
                                        FROM fn_listextendedproperty (NULL, 'schema', a.Table_schema, 'table', a.TABLE_NAME, 'column', default)   
                                        WHERE name='MS_Description' and objtype='COLUMN'    
                                        and objname Collate Chinese_Taiwan_Stroke_CI_AS = b.COLUMN_NAME   
                                    ) as col_describe,
                                    d.COLUMN_NAME as col_primay,
                                    b.ORDINAL_POSITION
                                FROM INFORMATION_SCHEMA.TABLES  a   
                                left join INFORMATION_SCHEMA.COLUMNS b ON a.TABLE_NAME = b.TABLE_NAME   
                                left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS c on c.TABLE_NAME = a.TABLE_NAME and c.CONSTRAINT_TYPE='PRIMARY KEY'
                                left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE d on c.CONSTRAINT_NAME = d.CONSTRAINT_NAME and d.COLUMN_NAME = b.COLUMN_NAME 
                                WHERE a.TABLE_TYPE='BASE TABLE'
                                and a.TABLE_NAME ='{table_name}'
                                order by b.ORDINAL_POSITION
             ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                var dr = conn.ExecuteReader(str_sql);
                var dt = new DataTable();
                int i = 1;

                StringBuilder sb = new StringBuilder();
                dt.Load(dr);
                sb.Append("select\n");

                foreach (DataRow row in dt.Rows)
                {
                    if (row.Table.Rows.Count == i)
                    {
                        sb.Append(row.ItemArray[1]);
                        sb.Append("\n");
                    }
                    else
                    {
                        sb.Append(row.ItemArray[1]);
                        sb.Append(",\n");
                    }
                    i++;
                }

                sb.Append("from " + table_name);
                sb.Append("\n");
                sb.Append("where");
                sb.Append("\n");

                return sb;
            }

            /*
                       StringBuilder table_structure = GetStructure(table_name);
                       var properties = arg.GetType().GetProperties();
                       StringBuilder sb = new StringBuilder();
                       StringBuilder sb1 = new StringBuilder();
                       int i = 1;
                       table_structure.Insert(1,"ROW_NUMBER() OVER(ORDER BY year_id, sms_id, startdate, enddate) AS RowNum,\n");
                       foreach (var p in properties)
                       {
                           if (i <= 3)
                           {
                               table_structure.Append($"{p.Name} = @{p.Name} and \n");
                           }
                           if (i == 4) 
                           {
                               table_structure.Append($"{p.Name} = @{p.Name} \n");
                           }
                           i++;                
                       }

                       i = 1;
                       foreach (var p in properties)
                       {
                           if (i == 5)
                           {
                               sb1.Append($"{p.Name} >= @{p.Name} and \n");
                           }
                           if (i == 6)
                           {
                               sb1.Append($"{p.Name} <= @{p.Name} \n");
                           }
                           i++;
                       }

                       string str_sql = $@"
                                           SELECT *
                                           FROM ({table_structure.ToString()}) AS NewTable
                                           WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";
                       arg.sch_no = _schno;
           */
        }

        public async Task<IEnumerable<HeaderList>> GetList(StuCollegeQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L01_std_public_filehub a
                                    where a.class_name = 'StuGroup'
                                    and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+convert(varchar,NewTable.e)) as x_cnt,
                                '' as x_status
                                FROM(
                                        SELECT ROW_NUMBER() OVER(ORDER BY year_id, sms_id, startdate, enddate) AS RowNum,
                                                sch_no as a,
                                                year_id as b,
                                                sms_id as c,
                                                std_no as d,
                                                ser_id as e,
                                                time_id as f,
                                                unit_name as g,
                                                group_content as h,
                                                startdate as i,
                                                enddate as j,
                                                hours as k,
                                                content as l,
                                                is_sys as m,
                                                check_centraldb as zz
                                        FROM L01_stu_group
                                        WHERE sch_no = @sch_no and year_id = @year_id and sms_id = @sms_id and std_no = @std_no
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<L01_StuGroup>> GetFormData(StuCollegeQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *
                                FROM L01_stu_group
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_StuGroup>(str_sql, arg);
            }
        }

        public async Task<int> DeleteData(StuCollegeQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                delete                                
                                FROM L01_stu_group
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

        public async Task<int> UpdateData(L01_StuGroup arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;

            string str_sql = @"
                                update L01_stu_group
                                set 
                                    time_id = @time_id,
                                    unit_name = @unit_name,
                                    group_content = @group_content,
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

        public async Task<Dictionary<string, string>> InsertData(L01_StuGroup arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L01_stu_group 
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
                              where r.Field<string>("time_id") == arg.time_id &&
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
                            insert L01_stu_group
                            (sch_no,year_id,sms_id,ser_id,std_no,time_id,unit_name,group_content,startdate,
                            enddate,hours,content,is_sys,upd_name,upd_dt)
                            values
                            (@sch_no,@year_id,@sms_id,@ser_id,@std_no,@time_id,@unit_name,@group_content,@startdate,
                            @enddate,@hours,@content,@is_sys,@upd_name,@upd_dt)
                        ";
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "Y");
                    dict.Add("result", result_count.ToString());
                    
                    return dict;
                }
            }
        }

        public async Task<int> UpdateDataCentraldb(StuCollegeQueryList arg)
        {
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            foreach (var item in arg.complex_array)
            {
                sb.Append(string.Format(@"
                                update L01_stu_group
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
        /*
                public async Task<IEnumerable<L01_std_public_filehub_DTO>> GetFileList(QueryList arg)
                {
                    arg.sch_no = _schno;
                    string sqlfile = $@"
                                        select *
                                        FROM (
                                            SELECT 
                                                ROW_NUMBER() OVER (ORDER BY number_id) AS RowNum,
                                                number_id ,file_name ,file_extension
                                                ,CASE
                                                    WHEN file_extension = 'mp3' or file_extension = 'mp4' THEN 2
                                                    ELSE 1
                                                END AS file_class
                                                FROM L01_std_public_filehub
                                                WHERE complex_key=@complex_key and class_name='stugroup'
                                        ) AS NewTable
                                        WHERE
                                            RowNum >= @sRowNun AND RowNum <= @eRowNun
                                        ORDER BY file_class, file_extension
                                    ";

                    using (IDbConnection con = _context.CreateCommand())
                    {
                        return await con.QueryAsync<L01_std_public_filehub_DTO>(sqlfile, arg);
                    }
                }

                public async Task<L01_std_public_filehub> GetFile(QueryList arg)
                {
                    arg.sch_no = _schno;
                    string sqlfile = @"
                                        SELECT *
                                        FROM L01_std_public_filehub
                                        WHERE
                                            class_name='stugroup' and type_id=0 and complex_key=@complex_key and number_id=@number_id
                                    ";

                    using (IDbConnection con = _context.CreateCommand())
                    {
                        return await con.QuerySingleOrDefaultAsync<L01_std_public_filehub>(sqlfile, arg);
                    }
                }
        */
        /*
                public async Task<int> InsertFile(IEnumerable<L01_std_public_filehub> files)
                {
                    int rt = 0;

                    int number_id = 0;
                    string sql = @"
                                    SELECT max(number_id) 
                                    FROM L01_std_public_filehub
                                    WHERE class_name='stugroup' 
                                    and type_id = 0 
                                    and complex_key=@complex_key
                                ";

                    using (IDbConnection con = _context.CreateCommand())
                    {
                        number_id = await con.ExecuteScalarAsync<int>(sql, new { files.First().complex_key });

                        foreach (L01_std_public_filehub file in files)
                        {
                            number_id++;
                            file.number_id = number_id;
                            file.upd_dt = updteDate();
                        }

                        string insert = @"
                        INSERT INTO L01_std_public_filehub
                        VALUES
                        (
                            @complex_key, @class_name, @type_id, @number_id, @file_name, @file_extension, @file_blob, @upd_name, @upd_dt
                        )
                        ";

                        rt = await con.ExecuteAsync(insert, files);
                    }
                    /*
                        foreach (L01_std_public_filehub file in files)
                        {
                            number_id++;
                            file.number_id = number_id;
                            file.upd_dt = updteDate();
                        }

                        string insert = @"
                            INSERT INTO L01_std_public_filehub
                            VALUES
                            (
                                @complex_key, @class_name, @type_id, @number_id, @file_name, @file_extension, @file_blob, @upd_name, @upd_dt
                            )
                            ";

                        using (IDbConnection con = _context.CreateCommand())
                        {
                            con.Open();
                            using (IDbTransaction tran = con.BeginTransaction())
                            {
                                rt = await con.ExecuteAsync(insert, files, tran);

                                tran.Commit();
                            }
                        }


                    return rt;
                }
        */
        /*
                public async Task<int> DeleteFile(QueryList arg)
                {
                    arg.sch_no = _schno;
                    string sqlfile = @"
                                        DELETE L01_std_public_filehub
                                        WHERE class_name='stugroup' 
                                        and type_id = 0 
                                        and complex_key=@complex_key 
                                        and number_id=@number_id
                                    ";

                    using (IDbConnection con = _context.CreateCommand())
                    {
                        return await con.ExecuteAsync(sqlfile, arg);
                    }
                }
        */
    }
}
