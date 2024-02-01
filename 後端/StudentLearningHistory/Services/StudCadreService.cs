using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuCollege.Parameters;
using StudentLearningHistory.Models.StudCadre.DbModels;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StuPublickFileHub.DTO;
using StudentLearningHistory.Models.StuStudyf.Parameter;
using System.Data;
using System.Text;

namespace StudentLearningHistory.Services
{
    public class StudCadreService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public string _schno;
        public string _schema;

        public StudCadreService(IDapperContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _schno = _context.SchNo;
            _schema ??= "dbo";
        }

        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public async Task<bool> CheckCadre(StudCadreParameter_DB cadre)
        {
            cadre.sch_no = _context.SchNo;
            string sql = $"""
                SELECT TOP 1 COUNT(*) FROM {_schema}.[L01_std_position]
                WHERE
                sch_no=@sch_no and std_no=@std_no
                and year_id=@year_id and sms_id=@sms_id
                and unit_name=@unit_name and position_name=@position_name
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteScalarAsync<bool>( sql, cadre);
            }
        }

        public async Task<bool> CheckCadreFile(StudCadreParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sql = $"""
                SELECT TOP 1 COUNT(*) FROM {_schema}.[L01_std_public_filehub]
                WHERE
                    class_name='StudCadre' and type_id=0 and complex_key=@complex_key and number_id=@number_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteScalarAsync<bool>( sql, parameter);
            }
        }

        public async Task<IEnumerable<L01_std_position>> GetCadreList(StudCadreParameter_DB cadre)
        {
            cadre.sch_no = _context.SchNo;
            string sql = $"""
                SELECT *,
                        (select count(*)
                        from L01_std_public_filehub a
                        where a.class_name = 'StudCadre'
                        and   a.complex_key = NewTable.sch_no +'_'+convert(varchar,NewTable.year_id)+'_'+convert(varchar,NewTable.sms_id)+'_'+NewTable.std_no+'_'+NewTable.unit_name+'_'+NewTable.position_name) as x_cnt,
            		    x_total =
            		             (select count(*)
            			            from L01_std_position a
            			            where a.year_id = NewTable.year_id
                                    and   a.sms_id = NewTable.sms_id 
                                    and   a.sch_no = NewTable.sch_no 
                                    and   a.std_no = NewTable.std_no 
                                    and ((@is_sys  = '2' and is_sys = 2) or (@is_sys = '0'))),
                           '' as x_status,
                           zz = isnull(NewTable.check_centraldb,'N')
                FROM (
                    SELECT ROW_NUMBER() OVER (ORDER BY year_id, sms_id, startdate, enddate) AS RowNum, * FROM {_schema}.[L01_std_position]
                    WHERE [sch_no]=@sch_no and [std_no]=@std_no and [year_id]=@year_id and [sms_id]=@sms_id
                    and ((@is_sys  = '2' and is_sys = 2) or (@is_sys = '0'))
                ) AS NewTable
                  WHERE
            	     RowNum >= @sRowNun AND RowNum <= @eRowNun
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<L01_std_position>(sql,  cadre);
            }
        }

        public async Task<L01_std_position> GetCadre(StudCadreParameter_DB cadre)
        {
            cadre.sch_no = _context.SchNo;
            string sql = $"""
                SELECT * FROM {_schema}.[L01_std_position]
                WHERE
                sch_no=@sch_no and std_no=@std_no
                and year_id=@year_id and sms_id=@sms_id
                and unit_name=@unit_name and position_name=@position_name
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<L01_std_position>( sql, cadre);
            }
        }

        public async Task<int> InsertCadre(L01_std_position cadre)
        {
            cadre.sch_no = _context.SchNo;
            cadre.upd_dt = updteDate();
            cadre.upd_name = cadre.std_no;

            string sql = $"""
                INSERT INTO {_schema}.[L01_std_position]
                ( [sch_no], [std_no], [year_id], [sms_id], [unit_name], [position_name], [startdate], [enddate], [type_id], [is_sys], [upd_name], [upd_dt])
                VALUES
                (@sch_no, @std_no, @year_id, @sms_id, @unit_name, @position_name, @startdate, @enddate, @type_id, @is_sys, @upd_name, @upd_dt)
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sql, cadre);
            }
        }

        public async Task<int> UpdateCadre(L01_std_position cadre)
        {
            cadre.sch_no = _context.SchNo;
            cadre.upd_dt = updteDate();
            cadre.upd_name = cadre.std_no;

            string sql = $"""
                UPDATE {_schema}.[L01_std_position] SET
                    [startdate]=@startdate, [enddate]=@enddate, [type_id]=@type_id, 
                    [is_sys]=@is_sys, [upd_name]=@upd_name, [upd_dt]=@upd_dt
                WHERE
                    [sch_no]=@sch_no and [std_no]=@std_no and [year_id]=@year_id and [sms_id]=@sms_id and 
                    [unit_name]=@unit_name and [position_name]=@position_name
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sql, cadre);
            }
        }

        public async Task<int> DelectCadre(StudCadreParameter_DB cadre)
        {
            cadre.sch_no = _context.SchNo;
            int main = 0, file = 0;

            string sql = $"""
                DELETE {_schema}.[L01_std_position]
                WHERE
                    [is_sys] <> 1
                    and [sch_no]=@sch_no and [std_no]=@std_no and [year_id]=@year_id and [sms_id]=@sms_id and 
                    [unit_name]=@unit_name and [position_name]=@position_name
            """;

            string complex_key = $"{_context.SchNo}_{cadre.std_no}_{cadre.year_id}_{cadre.sms_id}_{cadre.unit_name}_{cadre.position_name}";
            string sqlfile = $"""
                DELETE {_schema}.[L01_std_public_filehub]
                WHERE
                    class_name = 'StudCadre' and complex_key = @complex_key
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

        public async Task<IEnumerable<L01_std_public_filehub_DTO>> GetFileList(StudCadreParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sqlfile = $"""
                select *,           
                        (select count(*)
                        from L01_std_public_filehub a
                        where a.class_name = 'StudCadre'
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
                        WHERE [complex_key]=@complex_key and [class_name]='StudCadre'
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

        public async Task<L01_std_public_filehub> GetFile(StudCadreParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sqlfile = $"""
                SELECT *
                FROM {_schema}.[L01_std_public_filehub]
                WHERE
                    class_name='StudCadre' and type_id=0 and complex_key=@complex_key and number_id=@number_id
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
                    WHERE class_name='StudCadre' and type_id=0 and complex_key=@complex_key
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
                UPDATE {_schema}.[L01_std_public_filehub] SET file_name=@file_name, file_extension=@file_extension, file_blob=@file_blob
                WHERE complex_key=@complex_key and class_name=@class_name and type_id=@type_id and number_id=@number_id
            """;

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sqlfile, file);
            }
        }

        public async Task<int> DeleteFile(StudCadreParameter_DB parameter)
        {
            parameter.sch_no = _context.SchNo;
            string sqlfile = $"""
                DELETE {_schema}.[L01_std_public_filehub]
                WHERE class_name='StudCadre' and type_id = 0 and complex_key=@complex_key and number_id=@number_id
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
                                update L01_std_position
                                set 
                                    check_centraldb = case when @arg2_{0} ='true' then 'Y' else 'N' end
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+std_no+'_'+unit_name+'_'+position_name = @arg1_{1}
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
