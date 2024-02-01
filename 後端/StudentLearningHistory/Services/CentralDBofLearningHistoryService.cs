using AutoMapper;
using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.CentralDBofLearningHistory.DateSetupDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;
using StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackParameters;
using StudentLearningHistory.Models.CentralDBofLearningHistory.OutputJsonDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.QueryCountDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.QueryStdDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.Std;
using System.Data;

namespace StudentLearningHistory.Services
{
    public class CentralDBofLearningHistoryService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public string _schno;
        public string _schema;

        public CentralDBofLearningHistoryService(IDapperContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _schno = _context.SchNo;
                //_configuration.GetSection("setting").GetValue<string>("SchNo");
            _schema ??= "dbo";
        }

        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        #region 承辦人員

        //依照匯入的資料過濾出學年期-給承辦人員作業共用
        public async Task<IEnumerable<string>> GenYmsList()
        {
            string sql = $"""
                SELECT distinct yms = cast([year_id] as nvarchar(3)) + cast([sms_id] as nvarchar(1))
                FROM {_schema}.[L01_centraldb_learning_history]
                where
                    sch_no = '{_context.SchNo}'
                order by
                    yms desc
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<string>(sql);
            }
        }

        //查看匯入資料學生清單
        public async Task<object> GenClsStdList(short year_id, short sms_id)
        {
            string sql = $"""
                select distinct
                    s04_ytdbgoc.cls_abr,
                    /*s04_stuhcls.dep_id,
                    s04_stuhcls.bra_id,
                    s04_stuhcls.grd_id,
                    s04_stuhcls.cls_id,*/
                    s04_student.std_no,
                    s04_student.std_name
                from L01_centraldb_learning_history 
                inner join s04_student on
                    s04_student.std_identity = L01_centraldb_learning_history.idno
                inner join s04_stuhcls on 
                    s04_stuhcls.std_no = s04_student.std_no and
                    s04_stuhcls.sch_no = L01_centraldb_learning_history.sch_no and
                    s04_stuhcls.year_id = L01_centraldb_learning_history.year_id and
                    s04_stuhcls.sms_id = L01_centraldb_learning_history.sms_id
                inner join s04_ytdbgoc on 
                    s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                    s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                    s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                    s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                    s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                    s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
                where 
                        L01_centraldb_learning_history.sch_no='{_context.SchNo}'
                    and L01_centraldb_learning_history.year_id =@year_id
                    and L01_centraldb_learning_history.sms_id =@sms_id
                order by s04_ytdbgoc.cls_abr, s04_student.std_no
            """;

            List<ClsStdDTO> clsstds;
            using (IDbConnection conn = _context.CreateCommand())
            {
                var temp = await conn.QueryAsync<ClsStdDTO>(sql, new { year_id, sms_id });
                clsstds = temp.ToList();
            }

            List<string> cls = clsstds.Select(x => x.cls_abr).Distinct().ToList();
            List<object> std = new();

            foreach (var c in cls)
            {
                var clsstd = clsstds.Where(row => row.cls_abr == c).Select(row => new { val = row.std_no, name = row.std_name }).ToList();
                std.Add(clsstd);
            }

            return new { cls, std };
        }

        //匯入
        public async Task<int> InsertJson(List<List<L01_centraldb_learning_history_temp>> models)
        {
            /*string del_sql = $"""
               DELETE {_schema}.L01_centraldb_learning_history
               WHERE
               sch_no=@sch_no and year_id=@year_id and sms_id=@sms_id and kind=@kind
            """;

            string sql = $"""
                INSERT INTO {_schema}.L01_centraldb_learning_history
                VALUES
                (@sch_no, @year_id, @sms_id, @kind, @_class, @idno, @birthday, @json_head, @json_content, @upd_name, '{updteDate()}', @zip_name)
            """;

            int count = 0;
            foreach (var item in models)
            {
                using (IDbConnection conn = _context.CreateCommand())
                {
                    conn.Open();
                    using IDbTransaction tran = conn.BeginTransaction();

                    await conn.ExecuteAsync(del_sql, item.FirstOrDefault(), tran);
                    int insertCount = await conn.ExecuteAsync(sql, item, tran);

                    if (insertCount > 0)
                    {
                        count += insertCount;
                        tran.Commit();
                    }
                }
            }

            return count;*/

            string del_temp = $"""
               DELETE {_schema}.L01_centraldb_learning_history_temp
            """;

            string insert_temp = $"""
                INSERT INTO {_schema}.L01_centraldb_learning_history_temp
                VALUES
                (@sch_no, @year_id, @sms_id, @kind, @cls, @idno, @ser_id, @birthday, @json_head, @json_content, @upd_name, '{updteDate()}', @zip_name, @std_freeback, @remark, @is_check, @import_ser)
            """;

            string insert_main = $"""
                INSERT INTO {_schema}.L01_centraldb_learning_history
                VALUES
                (@sch_no, @year_id, @sms_id, @kind, @cls, @idno, @ser_id, @birthday, @json_head, @json_content, @upd_name, '{updteDate()}', @zip_name, @std_freeback, @remark, @is_check, @import_ser)
            """;

            string maxSerID = $"""
                select isnull(max(ser_id), 0) from {_schema}.[L01_centraldb_learning_history] m
                where m.sch_no = '{_context.SchNo}' and m.year_id = @year_id and m.sms_id = @sms_id and m.kind = @kind
            """;

            string temp_not_exists_main = $"""
                SELECT t.*
                FROM {_schema}.[L01_centraldb_learning_history_temp] t
                where
                not exists(
                    select 1 from {_schema}.[L01_centraldb_learning_history] m
                    where
                        m.sch_no = t.sch_no and m.year_id = t.year_id and m.sms_id = t.sms_id and m.kind = t.kind
                    and m.cls = t.cls and m.idno = t.idno and m.json_content = t.json_content
                )
            """;

            string temp_exists_main_update = $"""
                update {_schema}.[L01_centraldb_learning_history]
                set 
                    zip_name = a.zip_name, json_head=a.json_head
                from (
                    select t.*
                    from {_schema}.[L01_centraldb_learning_history_temp] t
                    where
                        exists(
                            select 1 from {_schema}.[L01_centraldb_learning_history] m
                            where
                                    m.sch_no = t.sch_no
                                and m.year_id = t.year_id
                                and m.sms_id = t.sms_id
                                and m.kind = t.kind
                                and m.cls = t.cls
                                and m.idno = t.idno
                                and m.json_content = t.json_content
                        )
                        and t.kind=@kind
                ) as a
                where
                    [L01_centraldb_learning_history].sch_no = a.sch_no
                and [L01_centraldb_learning_history].year_id = a.year_id
                and [L01_centraldb_learning_history].sms_id = a.sms_id
                and [L01_centraldb_learning_history].kind = a.kind
                and [L01_centraldb_learning_history].cls = a.cls
                and [L01_centraldb_learning_history].idno = a.idno
                and [L01_centraldb_learning_history].json_content = a.json_content
            """;

            string main_import_ser_add = $"""
                update {_schema}.[L01_centraldb_learning_history]
                set 
                    import_ser = (
                        select isnull(max(import_ser), 0)+1 from {_schema}.[L01_centraldb_learning_history]
                        where sch_no='{_context.SchNo}' and year_id=@year_id and sms_id=@sms_id
                    ),
                    upd_name='管理者', upd_dt='{updteDate()}'
                where
                    exists(
                        select 1 from {_schema}.[L01_centraldb_learning_history_temp] t
                        where
                            [L01_centraldb_learning_history].sch_no = t.sch_no
                        and [L01_centraldb_learning_history].year_id = t.year_id
                        and [L01_centraldb_learning_history].sms_id = t.sms_id
                        and [L01_centraldb_learning_history].kind = t.kind
                        and [L01_centraldb_learning_history].cls = t.cls
                        and [L01_centraldb_learning_history].idno = t.idno
                        and [L01_centraldb_learning_history].json_content = t.json_content
                )
            """;

            short year_id = models.FirstOrDefault().FirstOrDefault().year_id;
            short sms_id = models.FirstOrDefault().FirstOrDefault().sms_id;

            //清理temp
            using (IDbConnection conn = _context.CreateCommand())
            {
                await conn.ExecuteAsync(del_temp);
            }

            foreach (var item in models)
            {
                //寫入temp
                using (IDbConnection conn = _context.CreateCommand())
                {
                    conn.Open();
                    using IDbTransaction tran = conn.BeginTransaction();

                    int insertCount = await conn.ExecuteAsync(insert_temp, item, tran);
                    tran.Commit();
                }

                string kind = item.FirstOrDefault().kind;
                int maxID = 0;

                //比對是否存在 與 找最大號
                List<L01_centraldb_learning_history_temp> list;
                using (IDbConnection conn = _context.CreateCommand())
                {
                    maxID = await conn.ExecuteScalarAsync<int>(maxSerID, new { year_id, sms_id, kind });

                    list = (List<L01_centraldb_learning_history_temp>)await conn.QueryAsync<L01_centraldb_learning_history_temp>(temp_not_exists_main);
                }

                //把整個檔案為一個基準，同一個transaction
                using (IDbConnection conn = _context.CreateCommand())
                {
                    conn.Open();
                    using IDbTransaction tran = conn.BeginTransaction();

                    //先把把存在的head、zipname更新
                    await conn.ExecuteAsync(temp_exists_main_update, new { kind }, tran);

                    //再把不存在的新增
                    foreach (var data in list)
                    {
                        data.ser_id = ++maxID;
                    }
                    await conn.ExecuteAsync(insert_main, list, tran);

                    tran.Commit();
                }
            }

            //把版號+1
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(main_import_ser_add, new { year_id, sms_id });
            }
        }

        #region 設定時間
        public async Task<IEnumerable<Datetime_setupDTO>> GenDateTimeSetupList(int year_id, int sms_id)
        {
            string sql = $"""
                SELECT distinct
                   m.[year_id]
                  ,m.[sms_id]
                  ,m.[kind]
                  ,isnull(m.[zip_name], '') as [zip_name]
                  ,isnull(CONVERT(nvarchar(20), s.[s_dt], 120), '') as [s_dt]
                  ,isnull(CONVERT(nvarchar(20), s.[e_dt], 120), '') as [e_dt]
              FROM {_schema}.[L01_centraldb_learning_history] m
              left join {_schema}.[L01_centraldb_learning_history_datetime_setup] s on
                s.[sch_no] = m.[sch_no] and s.[year_id] = m.year_id and s.[sms_id] = m.[sms_id] and s.[kind] = m.[kind]
              where
                m.[year_id]=@year_id and m.[sms_id]=@sms_id
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<Datetime_setupDTO>(sql, new { year_id, sms_id });
            }
        }

        public async Task<int> UpsetDateTimeSetup(List<L01_centraldb_learning_history_datetime_setup> models)
        {
            string sql = $"""
               SELECT
                   [year_id]
                  ,[sms_id]
                  ,[kind]
              FROM {_schema}.[L01_centraldb_learning_history_datetime_setup]
              where
                [sch_no]='{_context.SchNo}' and [year_id]=@year_id and [sms_id]=@sms_id
            """;

            string update_sql = $"""
               UPDATE {_schema}.L01_centraldb_learning_history_datetime_setup
               set
                    s_dt=@s_dt, e_dt=@e_dt, upd_name='管理者', upd_dt='{updteDate()}'
               WHERE
               sch_no='{_context.SchNo}' and year_id=@year_id and sms_id=@sms_id and kind=@kind
            """;

            string insert_sql = $"""
                INSERT INTO {_schema}.L01_centraldb_learning_history_datetime_setup
                VALUES
                ('{_context.SchNo}', @year_id, @sms_id, @kind, @s_dt, @e_dt, '管理者', '{updteDate()}')
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                conn.Open();
                using IDbTransaction tran = conn.BeginTransaction();

                var findDate = await conn.QueryAsync<L01_centraldb_learning_history_datetime_setup>(sql, new { models.First().year_id, models.First().sms_id }, tran);

                var update = models.Where(e => findDate.Any(f => $"{f.year_id}_{f.sms_id}_{f.kind}".Contains($"{e.year_id}_{e.sms_id}_{e.kind}")));

                var insert = models.Where(e => !findDate.Any(f => $"{f.year_id}_{f.sms_id}_{f.kind}".Contains($"{e.year_id}_{e.sms_id}_{e.kind}")));

                int count = await conn.ExecuteAsync(update_sql, update, tran) + await conn.ExecuteAsync(insert_sql, insert, tran);

                tran.Commit();

                return count;
            }
        }
        #endregion 設定時間

        #region 查看問題回報
        public async Task<IEnumerable<FeedBackListDTO>> GenFeedBackList(FeedBackListParameter parameter)
        {
            string sql = $"""
                SELECT NewTable.*
                FROM(
                    SELECT
                        ROW_NUMBER() OVER(ORDER BY f.year_id, f.sms_id, f.ser_id) AS RowNum
                        ,[ser_id]
                        ,[kind]
                        ,[cls]
                        ,s04_ytdbgoc.cls_abr
                        ,s04_student.std_name
                        ,s04_stuhcls.sit_num
                        ,eror.[name]
                        ,[error_code]
                        ,[std_feedback]
                        ,[answer]
                        ,f.[create_dt]
                        ,total = (
                            select count(1) from [L01_centraldb_learning_history_stdfeedback]
                            where
                                [L01_centraldb_learning_history_stdfeedback].year_id = f.year_id
                            and	[L01_centraldb_learning_history_stdfeedback].sms_id = f.sms_id
                            and ([L01_centraldb_learning_history_stdfeedback].kind=@kind or 'All'=@kind)
                            and ([L01_centraldb_learning_history_stdfeedback].cls=@cls or 'All'=@cls)
                        )
                    FROM [dbo].[L01_centraldb_learning_history_stdfeedback] f
                    inner join s04_student on
                        s04_student.std_no = f.std_no
                    inner join s04_stuhcls on 
                            s04_stuhcls.std_no = s04_student.std_no
                        and s04_stuhcls.year_id = f.year_id
                        and s04_stuhcls.sms_id = f.sms_id
                    inner join s04_ytdbgoc on 
                        s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                        s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                        s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                        s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                        s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                        s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
                    left join L01_centraldb_learning_history_error_code eror on
                        eror.id = f.[error_code]
                    where
                            f.year_id = @year_id
                        and	f.sms_id = @sms_id
                        and (f.kind=@kind or 'All'=@kind)
                        and (f.cls=@cls or 'All'=@cls)
                ) AS NewTable
                where
                    RowNum >= @sRow AND RowNum <= @eRow
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<FeedBackListDTO>(sql, parameter);
            }
        }
        public async Task<int> UpDataFeedBackAnswer(FeedBackUpdataAnswerParameter updata)
        {
            updata.upd_dt = updteDate();
            updata.upd_name = "管理者";

            string update_sql = $"""
               UPDATE {_schema}.L01_centraldb_learning_history_stdfeedback
               set
                    answer=@answer, upd_name=@upd_name, upd_dt=@upd_dt
               WHERE
                sch_no='{_context.SchNo}' and year_id=@year_id and sms_id=@sms_id and ser_id=@ser_id
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(update_sql, updata);
            }
        }
        #endregion 查看問題回報

        #region 匯總查詢
        //查看匯入資料學生清單
        public async Task<IEnumerable<string>> GenClsList(short year_id, short sms_id)
        {
            string sql = $"""
                select distinct
                    s04_ytdbgoc.cls_abr
                from L01_centraldb_learning_history 
                inner join s04_student on
                    s04_student.std_identity = L01_centraldb_learning_history.idno
                inner join s04_stuhcls on 
                    s04_stuhcls.std_no = s04_student.std_no and
                    s04_stuhcls.sch_no = L01_centraldb_learning_history.sch_no and
                    s04_stuhcls.year_id = L01_centraldb_learning_history.year_id and
                    s04_stuhcls.sms_id = L01_centraldb_learning_history.sms_id
                inner join s04_ytdbgoc on 
                    s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                    s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                    s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                    s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                    s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                    s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
                where 
                        L01_centraldb_learning_history.sch_no='{_context.SchNo}'
                    and L01_centraldb_learning_history.year_id =@year_id
                    and L01_centraldb_learning_history.sms_id =@sms_id
            """;


            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<string>(sql, new { year_id, sms_id });
            }
        }
        //班級查詢
        public async Task<object> GenQueryCount(short year_id, short sms_id, string cls)
        {
            if (cls == "All")
            {
                string count_sql = $"""
                    WITH
                    temp1 (year_id, sms_id, kind, x_cnt) AS
                    (  
                     -- 總數
                    select a.year_id, a.sms_id, a.kind, count(distinct a.idno) as x_cnt from L01_centraldb_learning_history a
                    where
                            a.year_id=@year_id and a.sms_id=@sms_id
                        and a.import_ser=(
                            select isnull(max(b.import_ser),0) from L01_centraldb_learning_history b where a.year_id=b.year_id and a.sms_id=b.sms_id and a.kind=b.kind
                        )
                    group by
                        a.year_id, a.sms_id, a.kind
                    ),
                    temp2 (year_id, sms_id, kind, x_check) AS
                    (
                    --確認數
                    select a.year_id, a.sms_id, a.kind, count(distinct a.is_check) as x_check from L01_centraldb_learning_history a
                    where
                            a.year_id=@year_id and a.sms_id=@sms_id
                        and a.is_check='Y' and a.import_ser=(
                            select isnull(max(b.import_ser),0) from L01_centraldb_learning_history b where a.year_id=b.year_id and a.sms_id=b.sms_id and a.kind=b.kind
                        )
                    group by
                        a.year_id, a.sms_id, a.kind
                    )

                    --輸出
                    select temp1.kind, isnull(temp2.x_check, 0) as x_check, isnull(temp1.x_cnt, 0) as x_cnt
                    from temp1
                    left join temp2 on
                        temp1.year_id=temp2.year_id and temp1.sms_id=temp2.sms_id and temp1.kind=temp2.kind
                """;

                string sql = $"""
                    with
                    temp1 (cls_abr, kind, x_feedback) as
                    (
                    --回報
                        SELECT distinct
                            s04_ytdbgoc.cls_abr, feed.kind, isnull(count(distinct feed.std_no), 0) as x_feedback
                        FROM [L01_centraldb_learning_history_stdfeedback] feed
                        inner join s04_student on s04_student.std_no = feed.std_no
                        inner join s04_stuhcls on
                                s04_stuhcls.std_no = s04_student.std_no
                            and s04_stuhcls.year_id = feed.year_id
                            and s04_stuhcls.sms_id = feed.sms_id
                        inner join s04_ytdbgoc on 
                            s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                            s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                            s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                            s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                            s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                            s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                        where
                            feed.year_id=@year_id and feed.sms_id=@sms_id
                        group by
                            feed.kind, s04_ytdbgoc.cls_abr
                    ),
                    temp2 (cls_abr, kind, x_check) as
                    (
                    --確認
                        SELECT distinct
                            s04_ytdbgoc.cls_abr, central.kind, isnull(count(distinct central.is_check), 0) as x_check
                        FROM [L01_centraldb_learning_history] central
                        inner join s04_student on s04_student.std_identity = central.idno
                        inner join s04_stuhcls on
                                s04_stuhcls.std_no = s04_student.std_no
                            and s04_stuhcls.year_id = central.year_id
                            and s04_stuhcls.sms_id = central.sms_id
                        inner join s04_ytdbgoc on 
                            s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                            s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                            s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                            s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                            s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                            s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                        where
                            central.year_id=@year_id and central.sms_id=@sms_id
                        group by
                            central.kind, s04_ytdbgoc.cls_abr, s04_student.std_no
                    )

                    select temp2.cls_abr, temp2.kind, isnull(x_check, 0) as x_check, isnull(x_feedback,0) as x_feedback from temp2
                    left join temp1 on
                        temp1.cls_abr = temp2.cls_abr and temp1.kind = temp2.kind
                    order by cls_abr
                """;

                string cls_sql = $"""
                    select cls_abr from s04_ytdbgoc
                    where
                        s04_ytdbgoc.year_id = @year_id and
                        s04_ytdbgoc.sms_id = @sms_id
                    order by cls_abr
                """;

                string kind_sql = $"""
                    select distinct kind from L01_centraldb_learning_history where year_id=@year_id and sms_id=@sms_id
                """;

                using (IDbConnection conn = _context.CreateCommand())
                {
                    List<Dictionary<string, object>> counts = new();
                    List<string> head = new();
                    List<Dictionary<string, object>> datas = new();

                    #region 字串提醒
                    var count_dr = await conn.ExecuteReaderAsync(count_sql, new { year_id, sms_id });
                    DataTable countDT = new();
                    countDT.Load(count_dr);

                    //塞資料
                    foreach (DataRow row in countDT.Rows)
                    {
                        Dictionary<string, object> data = new()
                    {
                        { "kind", row["kind"] },
                        { "x_check", row["x_check"] },
                        { "x_cnt", row["x_cnt"] }
                    };

                        counts.Add(data);
                    }
                    #endregion 字串提醒

                    List<string> clss = (List<string>)await conn.QueryAsync<string>(cls_sql, new { year_id, sms_id });
                    List<string> kinds = (List<string>)await conn.QueryAsync<string>(kind_sql, new { year_id, sms_id});
                    List<QureyCount_All> queryAll = (List<QureyCount_All>)await conn.QueryAsync<QureyCount_All>(sql, new { year_id, sms_id });

                    //tableHead
                    head.Add("班級");
                    foreach (string kind in kinds)
                    {
                        head.Add($"{kind}_確認人數");
                        head.Add($"{kind}_回報人數");
                    }
                    //tableData
                    foreach (string c in clss)
                    {
                        Dictionary<string, object> data = new();
                        data.Add("班級", c);
                        foreach (string kind in kinds)
                        {
                            var item = queryAll.SingleOrDefault(e => e.cls_abr.Equals(c) && e.kind.Equals(kind));
                            data.Add($"{kind}_確認人數", item?.x_check ?? 0);
                            data.Add($"{kind}_回報人數", item?.x_feedback ?? 0);
                        }
                        datas.Add(data);
                    }
                    
                    return new { head, datas, counts };
                }
            }
            else
            {
                string count_sql = $"""
                    WITH
                    temp(year_id, sms_id, kind) as
                    (
                    select distinct a.year_id, a.sms_id, a.kind  from L01_centraldb_learning_history a
                    where
                            a.year_id=@year_id and a.sms_id=@sms_id
                        and a.import_ser=(
                            select isnull(max(b.import_ser),0) from L01_centraldb_learning_history b where a.year_id=b.year_id and a.sms_id=b.sms_id and a.kind=b.kind
                        )
                    group by
                        a.year_id, a.sms_id, a.kind
                    ),
                    temp1 (year_id, sms_id, kind, x_cnt) AS
                    (  
                     -- 總數
                    select a.year_id, a.sms_id, a.kind, count(distinct a.idno) as x_cnt from L01_centraldb_learning_history a
                    inner join s04_student on s04_student.std_identity = a.idno
                    inner join s04_stuhcls on 
                            s04_stuhcls.std_no = s04_student.std_no
                        and s04_stuhcls.year_id = a.year_id
                        and s04_stuhcls.sms_id = a.sms_id
                    inner join s04_ytdbgoc on 
                        s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                        s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                        s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                        s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                        s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                        s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                    where
                            a.year_id=@year_id and a.sms_id=@sms_id and s04_ytdbgoc.cls_abr=@cls
                        and a.import_ser=(
                            select isnull(max(b.import_ser),0) from L01_centraldb_learning_history b where a.year_id=b.year_id and a.sms_id=b.sms_id and a.kind=b.kind
                        )
                    group by
                        a.year_id, a.sms_id, a.kind
                    ),
                    temp2 (year_id, sms_id, kind, x_check) AS
                    (
                    --確認數
                    select a.year_id, a.sms_id, a.kind, count(distinct a.is_check) as x_check from L01_centraldb_learning_history a
                    inner join s04_student on s04_student.std_identity = a.idno
                    inner join s04_stuhcls on 
                            s04_stuhcls.std_no = s04_student.std_no
                        and s04_stuhcls.year_id = a.year_id
                        and s04_stuhcls.sms_id = a.sms_id
                    inner join s04_ytdbgoc on 
                        s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                        s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                        s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                        s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                        s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                        s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                    where
                            a.year_id=@year_id and a.sms_id=@sms_id and s04_ytdbgoc.cls_abr=@cls
                        and a.is_check='Y' and a.import_ser=(
                            select isnull(max(b.import_ser),0) from L01_centraldb_learning_history b where a.year_id=b.year_id and a.sms_id=b.sms_id and a.kind=b.kind
                        )
                    group by
                        a.year_id, a.sms_id, a.kind
                    )

                    --輸出
                    select temp.kind, isnull(temp2.x_check, 0) as x_check, isnull(temp1.x_cnt, 0) as x_cnt
                    from temp
                    left join temp1 on
                        temp.year_id=temp1.year_id and temp.sms_id=temp1.sms_id and temp.kind=temp1.kind
                    left join temp2 on
                        temp.year_id=temp2.year_id and temp.sms_id=temp2.sms_id and temp.kind=temp2.kind
                """;

                string sql = $"""
                    DECLARE @columns NVARCHAR(MAX);
                    DECLARE @query NVARCHAR(MAX);
                    BEGIN
                        -- 設定橫轉直 轉置欄位
                        SET @columns = STUFF((SELECT distinct ',' + QUOTENAME(c.kind)
                            FROM L01_centraldb_learning_history c where year_id={year_id} and sms_id={sms_id}
                            FOR XML PATH(''), TYPE
                            ).value('.', 'NVARCHAR(MAX)') 
                        ,1,1,'');

                        SET @query = N'SELECT *
                        FROM (
                            SELECT distinct
                                s04_stuhcls.sit_num as [座號], s04_student.std_no as [學號], s04_student.std_name as [姓名], central.kind, central.is_check
                            FROM s04_student
                            inner join s04_stuhcls on 
                                    s04_stuhcls.std_no = s04_student.std_no
                            inner join s04_ytdbgoc on 
                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                            left join [L01_centraldb_learning_history] central on
                                    s04_student.std_identity = central.idno
                                and s04_stuhcls.year_id = central.year_id
                                and s04_stuhcls.sms_id = central.sms_id
                            where
                                s04_stuhcls.year_id={year_id} and s04_stuhcls.sms_id={sms_id} and s04_ytdbgoc.cls_abr=''{cls}''
                        ) t 
                        PIVOT (
                            max([is_check])
                            FOR [kind] IN (' + @columns + ')
                        ) p
                        order by [座號]
                        ';
                        exec sp_executesql @query;
                    END;
                """;

                using (IDbConnection conn = _context.CreateCommand())
                {
                    List<Dictionary<string, object>> counts = new();
                    List<string> head = new();
                    List<Dictionary<string, object>> datas = new();

                    #region 提醒字串
                    var count_dr = await conn.ExecuteReaderAsync(count_sql, new { year_id, sms_id, cls });
                    DataTable countDT = new();
                    countDT.Load(count_dr);

                    //塞資料
                    foreach (DataRow row in countDT.Rows)
                    {
                        Dictionary<string, object> data = new()
                        {
                            { "kind", row["kind"] },
                            { "x_check", row["x_check"] },
                            { "x_cnt", row["x_cnt"] }
                        };

                        counts.Add(data);
                    }
                    #endregion 提醒字串

                    //tableData
                    var dr = await conn.ExecuteReaderAsync(sql);
                    DataTable dt = new();
                    dt.Load(dr);

                    //動態抬頭
                    foreach (DataColumn col in dt.Columns)
                    {
                        head.Add(col.ColumnName);
                    }
                    //塞資料
                    foreach (DataRow row in dt.Rows)
                    {
                        Dictionary<string, object> data = new();
                        foreach (string col in head)
                        {
                            data.Add(col, row[col] == DBNull.Value ? "N" : row[col]);
                        }
                        datas.Add(data);
                    }

                    return new { head, datas, counts };
                }
            }

            return null;
        }
        #endregion 匯總查詢

        #endregion 承辦人員

        #region 學生

        #region 中央資料匯入查看與確認
        //依照匯入的資料過濾出學年期-給學生作業共用
        public async Task<IEnumerable<string>> GenYmsListStd(string std)
        {
            string sql = $"""
                SELECT distinct yms = cast(m.[year_id] as nvarchar(3)) + cast(m.[sms_id] as nvarchar(1))
                FROM {_schema}.[L01_centraldb_learning_history] m
                inner join {_schema}.s04_student s on
                    s.std_identity = m.idno
                where
                    m.sch_no = '{_context.SchNo}' and s.std_no=@std
                order by
                    yms desc
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<string>(sql, new { std });
            }
        }
        //確認資料無誤更新註記
        public async Task<int> UpDataCheckFlag(StdCheckDataParameter parameter)
        {
            string update_sql = $"""
               UPDATE {_schema}.[L01_centraldb_learning_history]
               set
                    is_check='Y', upd_name=@std, upd_dt='{updteDate()}'
               WHERE
                    sch_no='{_context.SchNo}' and year_id=@year_id and sms_id=@sms_id and kind=@kind
                and idno = (select std_identity from {_schema}.[s04_student] where std_no=@std)
                and import_ser = (
                    select isnull(max(a.import_ser), 0) from {_schema}.[L01_centraldb_learning_history] a
                    where
                        a.sch_no=[L01_centraldb_learning_history].sch_no and a.year_id=[L01_centraldb_learning_history].year_id 
                    and a.sms_id=[L01_centraldb_learning_history].sms_id and a.kind=[L01_centraldb_learning_history].kind
                )
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(update_sql, parameter);
            }
        }
        #endregion 中央資料匯入查看與確認

        #region 問題回報
        //錯誤比下拉
        public async Task<IEnumerable<object>> GenFeedBackErrorCodeList()
        {
            string sql = $"""
                SELECT [id], [name]
                FROM {_schema}.[L01_centraldb_learning_history_error_code]
                WHERE [is_useing]='Y'
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                var temp = await conn.QueryAsync<L01_centraldb_learning_history_error_code>(sql);
                return temp.Select(e => new { e.id, e.name });
            }
        }
        //該學年度開放的kind
        public async Task<IEnumerable<string>> GenFeedBackOpenKindList(short year_id, short sms_id)
        {
            string sql = $"""
                SELECT distinct m.kind
                FROM [L01_centraldb_learning_history] m
                inner join [L01_centraldb_learning_history_datetime_setup] s on
                    s.sch_no=m.sch_no and s.year_id=m.year_id and s.sms_id=m.sms_id and s.kind=m.kind
                where m.year_id=@year_id and m.sms_id=@sms_id and s.s_dt <= GETDATE() and GETDATE() <= s.e_dt
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<string>(sql, new { year_id, sms_id });
            }
        }
        //問題回報清單
        public async Task<IEnumerable<StdFeedBackListDTO>> GenStdFeedBackList(StdFeedBackListParameter parameter)
        {
            string sql = $"""
                SELECT NewTable.*
                FROM(
                    SELECT
                        ROW_NUMBER() OVER(ORDER BY f.year_id, f.sms_id, f.ser_id) AS RowNum
                        ,f.*
                        ,eror.[name]
                        ,total = (
                            select count(1) from [L01_centraldb_learning_history_stdfeedback]
                            where
                                [L01_centraldb_learning_history_stdfeedback].year_id = f.year_id
                            and	[L01_centraldb_learning_history_stdfeedback].sms_id = f.sms_id
                            and [L01_centraldb_learning_history_stdfeedback].std_no = f.std_no
                        )
                    FROM [dbo].[L01_centraldb_learning_history_stdfeedback] f
                    left join L01_centraldb_learning_history_error_code eror on eror.id = f.[error_code]
                    where
                            f.year_id = @year_id and f.sms_id = @sms_id and f.std_no=@std
                ) AS NewTable
                where
                    RowNum >= @sRow AND RowNum <= @eRow
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<StdFeedBackListDTO>(sql, parameter);
            }
        }
        //新增
        public async Task<int> InsertStdFeedBack(L01_centraldb_learning_history_stdfeedback data)
        {
            data.sch_no = _context.SchNo;
            data.upd_dt = updteDate();
            data.create_dt = updteDate();
            data.upd_name = data.std_no;
            data.create_name = data.std_no;

            string check_sql = $"""
                SELECT COUNT(1)
                FROM {_schema}.[L01_centraldb_learning_history_datetime_setup]
                where
                        sch_no='{_context.SchNo}' and [year_id]=@year_id and [sms_id]=@sms_id and [kind]=@kind
                    and [s_dt] <= GETDATE() and GETDATE() <= [e_dt]
            """;

            string max_sql = $"""
                SELECT isnull(max([ser_id]), 0) + 1
                FROM {_schema}.[L01_centraldb_learning_history_stdfeedback]
                where [year_id]=@year_id and [sms_id]=@sms_id
            """;

            string insert_sql = $"""
               insert into {_schema}.L01_centraldb_learning_history_stdfeedback
               values (@sch_no, @year_id, @sms_id, @ser_id, @kind, @cls, @std_no, @error_code, @std_feedback, @answer, @upd_name, @upd_dt, @create_name, @create_dt)
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                int check = await conn.ExecuteScalarAsync<int>(check_sql, data);

                if (check == 1)
                {
                    int ser_id = await conn.ExecuteScalarAsync<int>(max_sql, data);

                    data.ser_id = ser_id;

                    return await conn.ExecuteAsync(insert_sql, data);
                }
                    else
                {
                    return -1;
                }
        }
        }
        //更新
        public async Task<int> UpdateStdFeedBack(L01_centraldb_learning_history_stdfeedback data)
        {
            data.upd_dt = updteDate();
            data.upd_name = data.std_no;

            string check_sql = $"""
                SELECT COUNT(1)
                FROM {_schema}.[L01_centraldb_learning_history_datetime_setup]
                where
                        [year_id]=@year_id and [sms_id]=@sms_id and [kind]=@kind
                    and [s_dt] <= GETDATE() and GETDATE() <= [e_dt]
            """;

            string update_sql = $"""
               UPDATE {_schema}.L01_centraldb_learning_history_stdfeedback
               set
                    kind=@kind, cls=@cls, error_code=@error_code, std_feedback=@std_feedback, upd_name=@upd_name, upd_dt=@upd_dt
               WHERE
                sch_no='{_context.SchNo}' and year_id=@year_id and sms_id=@sms_id and ser_id=@ser_id and std_no=@std_no
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                int check = await conn.ExecuteScalarAsync<int>(check_sql, data);

                if (check == 1)
                {
                    return await conn.ExecuteAsync(update_sql, data);
                }
                else
                {
                    return -1;
                }
            }
        }
        //刪除
        public async Task<int> DelStdFeedBack(StdFeedBackListDel data)
        {
            string check_sql = $"""
                SELECT COUNT(1)
                FROM {_schema}.[L01_centraldb_learning_history_datetime_setup]
                where
                        [year_id]=@year_id and [sms_id]=@sms_id and [kind]=@kind
                    and [s_dt] <= GETDATE() and GETDATE() <= [e_dt]
            """;

            string update_sql = $"""
               delete {_schema}.L01_centraldb_learning_history_stdfeedback
               WHERE
                sch_no='{_context.SchNo}' and year_id=@year_id and sms_id=@sms_id and ser_id=@ser_id and std_no=@std_no
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                int check = await conn.ExecuteScalarAsync<int>(check_sql, data);

                if (check == 1)
                {
                    return await conn.ExecuteAsync(update_sql, data);
                }
                else
                {
                    return -1;
                }
            }
        }
        #endregion 問題回報

        #endregion 學生

        #region 共用
        //該學年度的kind & cls下拉
        public async Task<IEnumerable<GenFeedBackKindClsList>> GenFeedBackKindClsList(short year_id, short sms_id)
        {
            string sql = $"""
                SELECT distinct m.kind, m.cls
                FROM [L01_centraldb_learning_history] m
                inner join [L01_centraldb_learning_history_stdfeedback] f on
                    f.sch_no=m.sch_no and f.year_id=m.year_id and f.sms_id=m.sms_id
                    and f.kind=m.kind and f.cls=m.cls
                where m.year_id=@year_id and m.sms_id=@sms_id
            """;

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<GenFeedBackKindClsList>(sql, new { year_id, sms_id });
            }
        }
        //查看匯入的學生資料
        public async Task<List<object>> GenStdList(short year_id, short sms_id, string std_no)
        {
            string sql = $"""
                SELECT m.kind, m.cls, m.json_head, m.json_content, m.zip_name, m.upd_name,t.s_dt as _s_dt, t.e_dt as _e_dt
                FROM {_schema}.[L01_centraldb_learning_history] m
                inner join {_schema}.s04_student s on
                    s.std_identity = m.idno
                left join {_schema}.[L01_centraldb_learning_history_datetime_setup] t on
                        t.year_id = m.year_id
                    and t.sms_id = m.sms_id
                    and t.kind = m.kind
                where
                            m.year_id=@year_id and m.sms_id=@sms_id and s.std_no=@std_no
                    and m.import_ser = (
                        select isnull(max(import_ser), 0) from {_schema}.[L01_centraldb_learning_history]
                        where sch_no=m.sch_no and year_id=m.year_id and sms_id=m.sms_id
                    )
            """;

            List<object> stdList = new();
            using (IDbConnection conn = _context.CreateCommand())
            {
                var temp = await conn.QueryAsync<L01_centraldb_learning_history_outputjson>(sql, new { year_id, sms_id, std_no });

                //整理kind跟cls
                var kindList = temp.Select(row => new { row.kind, row.json_head, row.upd_name, row.s_dt, row.e_dt, row._s_dt, row._e_dt }).Distinct().ToList();
                var clsList = temp.Select(row => new { row.kind, row.cls }).Distinct().ToList();

                //組合輸出資料結構
                foreach (var kind in kindList)
                {
                    if (kind.kind.Contains("多元表現"))
                    {
                        多元表現外層_出 out_way = new();
                        string se_dt = !string.IsNullOrWhiteSpace(kind.s_dt) && !string.IsNullOrWhiteSpace(kind.e_dt) ? " ~ " : "";
                        out_way.名稱 = kind.kind;
                        out_way.名冊資訊 = kind.json_head;
                        out_way.上傳人員 = kind.upd_name;
                        out_way.開放確認期限 = $"{kind.s_dt}{se_dt}{kind.e_dt}";
                        if (kind._s_dt.HasValue && kind._e_dt.HasValue)
                        {
                            if (kind._s_dt.Value <= DateTime.Now && DateTime.Now <= kind._e_dt.Value)
                            {
                                out_way.kind = kind.kind;
                            }
                        }

                        foreach (string cls in clsList.Where(row => row.kind == kind.kind).Select(row => row.cls))
                        {
                            var stds = temp.Where(row => row.kind == kind.kind && row.cls == cls).Select(row => row.json_content).ToList();

                            var propList = typeof(多元表現內層_出).GetProperties();
                            var prop = propList.SingleOrDefault(p => p.Name == cls);
                            if (prop != null)
                            {
                                prop.SetValue(out_way.多元表現, stds);
                            }
                        }
                        stdList.Add(out_way);
                    }
                    else if (kind.kind.Contains("修課紀錄"))
                    {
                        修課紀錄外層_出 out_way = new();
                        string se_dt = !string.IsNullOrWhiteSpace(kind.s_dt) && !string.IsNullOrWhiteSpace(kind.e_dt) ? " ~ " : "";
                        out_way.名稱 = kind.kind;
                        out_way.名冊資訊 = kind.json_head;
                        out_way.上傳人員 = kind.upd_name;
                        out_way.開放確認期限 = $"{kind.s_dt}{se_dt}{kind.e_dt}";
                        if (kind._s_dt.HasValue && kind._e_dt.HasValue)
                        {
                            if (kind._s_dt.Value <= DateTime.Now && DateTime.Now <= kind._e_dt.Value)
                            {
                                out_way.kind = kind.kind;
                            }
                        }
                        foreach (string cls in clsList.Where(row => row.kind == kind.kind).Select(row => row.cls))
                        {
                            var stds = temp.Where(row => row.kind == kind.kind && row.cls == cls).Select(row => row.json_content).ToList();

                            var propList = typeof(修課紀錄內層_出).GetProperties();
                            var prop = propList.SingleOrDefault(p => p.Name == cls);
                            if (prop != null)
                            {
                                prop.SetValue(out_way.修課紀錄, stds);
                            }
                        }
                        stdList.Add(out_way);
                    }
                    else if (kind.kind.Contains("校內幹部經歷"))
                    {
                        校內幹部經歷外層_出 out_way = new();
                        string se_dt = !string.IsNullOrWhiteSpace(kind.s_dt) && !string.IsNullOrWhiteSpace(kind.e_dt) ? " ~ " : "";
                        out_way.名稱 = kind.kind;
                        out_way.名冊資訊 = kind.json_head;
                        out_way.上傳人員 = kind.upd_name;
                        out_way.開放確認期限 = $"{kind.s_dt}{se_dt}{kind.e_dt}";
                        if (kind._s_dt.HasValue && kind._e_dt.HasValue)
                        {
                            if (kind._s_dt.Value <= DateTime.Now && DateTime.Now <= kind._e_dt.Value)
                            {
                                out_way.kind = kind.kind;
                            }
                        }
                        foreach (string cls in clsList.Where(row => row.kind == kind.kind).Select(row => row.cls))
                        {
                            var stds = temp.Where(row => row.kind == kind.kind && row.cls == cls).Select(row => row.json_content).ToList();

                            var propList = typeof(校內幹部經歷內層_出).GetProperties();
                            var prop = propList.SingleOrDefault(p => p.Name == cls);
                            if (prop != null)
                            {
                                prop.SetValue(out_way.校內幹部經歷, stds);
                            }
                        }
                        stdList.Add(out_way);
                    }
                    else if (kind.kind.Contains("課程學習成果"))
                    {
                        課程學習成果外層_出 out_way = new();
                        string se_dt = !string.IsNullOrWhiteSpace(kind.s_dt) && !string.IsNullOrWhiteSpace(kind.e_dt) ? " ~ " : "";
                        out_way.名稱 = kind.kind;
                        out_way.名冊資訊 = kind.json_head;
                        out_way.上傳人員 = kind.upd_name;
                        out_way.開放確認期限 = $"{kind.s_dt}{se_dt}{kind.e_dt}";
                        if (kind._s_dt.HasValue && kind._e_dt.HasValue)
                        {
                            if (kind._s_dt.Value <= DateTime.Now && DateTime.Now <= kind._e_dt.Value)
                            {
                                out_way.kind = kind.kind;
                            }
                        }
                        foreach (string cls in clsList.Where(row => row.kind == kind.kind).Select(row => row.cls))
                        {
                            var stds = temp.Where(row => row.kind == kind.kind && row.cls == cls).Select(row => row.json_content).ToList();

                            var propList = typeof(課程學習成果內層_出).GetProperties();
                            var prop = propList.SingleOrDefault(p => p.Name == cls);
                            if (prop != null)
                            {
                                prop.SetValue(out_way.課程學習成果, stds);
                            }
                        }
                        stdList.Add(out_way);
                    }
                }
            }

            return stdList;
        }
        #endregion 共用
    }
}
