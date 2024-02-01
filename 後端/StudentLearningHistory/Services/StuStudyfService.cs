using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Models.StuOther.DbModels;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StuPublickFileHub.DTO;
using StudentLearningHistory.Models.StuStudyf.DB;
using StudentLearningHistory.Models.StuStudyf.Parameter;
using System.Data;
using System.Text;

namespace StudentLearningHistory.Services
{
    public class StuStudyfService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        public string _schema;

        public StuStudyfService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
            _schema ??= "dbo";
        }

        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public async Task<bool> CheckCadre(StuStudyFreeParameter_DB paramt)
        {
            paramt.sch_no = _context.SchNo;
            string sql = $"""
                SELECT TOP 1 COUNT(*) FROM {_schema}.[L01_stu_study_free]
                WHERE
                sch_no=@sch_no and std_no=@std_no
                and year_id=@year_id and sms_id=@sms_id
                and ser_id=@ser_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteScalarAsync<bool>( sql, paramt);
            }
        }

        public async Task<bool> CheckCadreFile(StuStudyFreeParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sql = $"""
                SELECT TOP 1 COUNT(*) FROM {_schema}.[L01_std_public_filehub]
                WHERE
                    class_name='stuStudyf' and type_id=0 and complex_key=@complex_key and number_id=@number_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteScalarAsync<bool>( sql, parameter);
            }
        }

        public async Task<IEnumerable<L01_stu_study_free>> GetCadreList(StuStudyFreeParameter_DB paramt)
        {
            paramt.sch_no = _context.SchNo;
            string sql = $"""
                SELECT *,
                        (select count(*)
                        from L01_std_public_filehub a
                        where a.class_name = 'StuStudyf'
                        and   a.complex_key = NewTable.sch_no +'_'+convert(varchar,NewTable.year_id)+'_'+convert(varchar,NewTable.sms_id)+'_'+NewTable.std_no+'_'+convert(varchar,NewTable.ser_id)) as x_cnt,
            		    x_total =
            		             (select count(*)
            			            from L01_stu_study_free a
            			            where a.year_id = NewTable.year_id
                                    and   a.sms_id = NewTable.sms_id 
                                    and   a.sch_no = NewTable.sch_no 
                                    and   a.std_no = NewTable.std_no ),
                           '' as x_status,
                           zz = NewTable.check_centraldb
                FROM (
                    SELECT ROW_NUMBER() OVER (ORDER BY year_id, sms_id, ser_id) AS RowNum, * FROM {_schema}.[L01_stu_study_free]
                    WHERE [sch_no]=@sch_no and [std_no]=@std_no and [year_id]=@year_id and [sms_id]=@sms_id
                ) AS NewTable
                  WHERE
            	     RowNum >= @sRowNun AND RowNum <= @eRowNun
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<L01_stu_study_free>(sql,  paramt);
            }
        }


        public async Task<L01_stu_study_free> GetCadre(StuStudyFreeParameter_DB paramt)
        {
            paramt.sch_no = _context.SchNo;
            string sql = $"""
                SELECT * FROM {_schema}.[L01_stu_study_free]
                WHERE
                sch_no=@sch_no and std_no=@std_no
                and year_id=@year_id and sms_id=@sms_id
                and ser_id=@ser_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<L01_stu_study_free>( sql, paramt);
            }
        }

        public async Task<int> InsertCadre(L01_stu_study_free paramt)
        {
            paramt.sch_no = _context.SchNo;

            short number_id = 0;
            string genID = $"""
                    SELECT max(ser_id) FROM {_schema}.[L01_stu_study_free]
                    WHERE
                    sch_no=@sch_no and std_no=@std_no
                    and year_id=@year_id and sms_id=@sms_id
                """;

            using (IDbConnection con = _context.CreateCommand())
            {
                number_id = await con.ExecuteScalarAsync<short>(genID, paramt);
            }
            number_id++;

            paramt.ser_id = number_id;
            paramt.upd_dt = updteDate();
            paramt.upd_name = paramt.std_no;

            string sql = $"""
                INSERT INTO {_schema}.[L01_stu_study_free]
                VALUES
                (@sch_no, @year_id, @sms_id, @std_no, @ser_id, @is_sys, @type_id, @open_name, @open_unit, @hours, @weeks, @content, @upd_name, @upd_dt)
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sql, paramt);
            }
        }

        public async Task<Dictionary<string, string>> InsertData(L01_stu_study_free_2 arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L01_stu_study_free 
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
                              where r.Field<string>("type_id") == arg.type_id &&
                                            r.Field<string>("open_name") == arg.open_name &&
                                            r.Field<string>("open_unit") == arg.open_unit &&
                                            r.Field<Int16>("hours") == arg.hours &&
                                            r.Field<Int16>("weeks") == arg.weeks
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
                                INSERT INTO L01_stu_study_free
                                (sch_no, year_id, sms_id, std_no, ser_id, is_sys, type_id, open_name, open_unit, hours, weeks, content, upd_name, upd_dt)
                                VALUES
                                (@sch_no, @year_id, @sms_id, @std_no, @ser_id, @is_sys, @type_id, @open_name, @open_unit, @hours, @weeks, @content, @upd_name, @upd_dt)
                        ";
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "N");
                    dict.Add("result", result_count.ToString());

                    return dict;
                }
            }
        }
        public async Task<int> UpdateCadre(L01_stu_study_free cadre)
        {
            cadre.sch_no = _context.SchNo;
            cadre.upd_dt = updteDate();
            cadre.upd_name = cadre.std_no;

            string sql = $"""
                UPDATE {_schema}.[L01_stu_study_free] SET
                    type_id=@type_id, open_name=@open_name, open_unit=@open_unit, hours=@hours,
                    weeks=@weeks, content=@content, upd_name=@upd_name, upd_dt=@upd_dt
                WHERE
                    sch_no=@sch_no and std_no=@std_no and year_id=@year_id and sms_id=@sms_id
                    and ser_id=@ser_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sql, cadre);
            }
        }

        public async Task<int> DelectCadre(StuStudyFreeParameter_DB cadre)
        {
            cadre.sch_no = _context.SchNo;
            int main = 0, file = 0;

            string sql = $"""
                DELETE {_schema}.[L01_stu_study_free]
                WHERE
                    [is_sys] <> 1
                    and sch_no=@sch_no and std_no=@std_no and year_id=@year_id and sms_id=@sms_id
                    and ser_id=@ser_id
            """;

            string sqlfile = $"""
                DELETE {_schema}.[L01_std_public_filehub]
                WHERE
                    class_name = 'stuStudyf' and complex_key = @complex_key
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                con.Open();
                using (IDbTransaction tran = con.BeginTransaction())
                {
                    main = await con.ExecuteAsync(sql, cadre, tran);

                    if (main > 0)
                    {
                        file = await con.ExecuteAsync(sqlfile, cadre, tran);

                        tran.Commit();
                    }
                }
            }
            return main + file;
        }

        public async Task<IEnumerable<L01_std_public_filehub_DTO>> GetFileList(StuStudyFreeParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sqlfile = $"""
                select *,           
                        (select count(*)
                        from L01_std_public_filehub a
                        where a.class_name = 'StuStudyf'
                        and   a.complex_key = @complex_key) as x_cnt
                FROM (
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY number_id) AS RowNum,
                        [number_id] ,[file_name] ,[file_extension]
                        ,CASE
                            WHEN file_extension = 'mp3' or file_extension = 'mp4' THEN 2
                            ELSE 1
                        END AS [file_class]
                        FROM {_schema}.[L01_std_public_filehub]
                        WHERE [complex_key]=@complex_key and [class_name]='stuStudyf'
                ) AS NewTable
                WHERE
                    RowNum >= @sRowNun AND RowNum <= @eRowNun
                ORDER BY file_class, file_extension
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<L01_std_public_filehub_DTO>(sqlfile, parameter);
            }
        }

        public async Task<L01_std_public_filehub> GetFile(StuStudyFreeParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sqlfile = $"""
                SELECT *
                FROM {_schema}.[L01_std_public_filehub]
                WHERE
                    class_name='stuStudyf' and type_id=0 and complex_key=@complex_key and number_id=@number_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<L01_std_public_filehub>(sqlfile, parameter);
            }
        }

        public async Task<int> InsertFile(IEnumerable<L01_std_public_filehub> files)
        {
            int rt = 0;

            int number_id = 0;
            string sql = $"""
                    SELECT max(number_id) FROM {_schema}.[L01_std_public_filehub]
                    WHERE class_name='stuStudyf' and type_id=0 and complex_key=@complex_key
                """;

            using (IDbConnection con = _context.CreateCommand())
            {
                number_id = await con.ExecuteScalarAsync<int>(sql, new { files.First().complex_key });
            }

            foreach (L01_std_public_filehub file in files)
            {
                number_id++;
                file.number_id = number_id;
                file.upd_dt = updteDate();
            }

            string insert = $"""
                INSERT INTO {_schema}.[L01_std_public_filehub]
                VALUES
                (
                    @complex_key, @class_name, @type_id, @number_id, @file_name, @file_extension, @file_blob, @upd_name, @upd_dt
                )
            """;

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

        public async Task<int> UpdateFile(L01_std_public_filehub file)
        {
            file.upd_dt = updteDate();

            string sqlfile = $"""
                UPDATE {_schema}.[L01_std_public_filehub] SET
                    file_name=@file_name, file_extension=@file_extension, file_blob=@file_blob, upd_name=@upd_name, upd_dt=@upd_dt
                WHERE
                    complex_key=@complex_key and class_name=@class_name and type_id=@type_id and number_id=@number_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sqlfile, file);
            }
        }

        public async Task<int> DeleteFile(StuStudyFreeParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sqlfile = $"""
                DELETE {_schema}.[L01_std_public_filehub]
                WHERE class_name='stuStudyf' and type_id = 0 and complex_key=@complex_key and number_id=@number_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sqlfile, parameter);
            }
        }

        public async Task<int> UpdateDataCentraldb(StuCadreQueryList arg)
        {
            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            foreach (var item in arg.complex_array)
            {
                sb.Append(string.Format(@"
                                update L01_stu_study_free
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
