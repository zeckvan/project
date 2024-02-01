using ClosedXML.Excel;
using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.ExportExcel.Parameters;
using System.Data;

namespace StudentLearningHistory.Services
{
    public class ExportExcelService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        public string _schema;

        public ExportExcelService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
            _schema ??= "dbo";
        }

        public async Task<MemoryStream> GetMultipleLearningExcelStream(Parameter parameter)
        {
            MemoryStream stream = new();
            DataTable sp_L01_std_position_excel = new();
            DataTable sp_L01_stu_competition_excel = new();
            DataTable sp_L01_stu_license_excel = new();
            DataTable sp_L01_stu_volunteer_excel = new();
            DataTable sp_L01_stu_study_free_excel = new();
            DataTable sp_L01_stu_group_excel = new();
            DataTable sp_L01_stu_workplace_excel = new();
            DataTable sp_L01_stu_result_excel = new();
            DataTable sp_L01_stu_college_excel = new();
            DataTable sp_L01_stu_other_excel = new();

            using (IDbConnection conn = _context.CreateCommand())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@sch_no", _context.SchNo, DbType.String, ParameterDirection.Input);
                parameters.Add("@year_id", parameter.year_id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@grd_id", parameter.grd_id, DbType.Int16, ParameterDirection.Input);
                //幹部
                IDataReader dr = await conn.ExecuteReaderAsync("sp_L01_std_position_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_std_position_excel.Load(dr);
                sp_L01_std_position_excel.TableName = "幹部經歷暨事蹟紀錄";
                //競賽
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_competition_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_competition_excel.Load(dr);
                sp_L01_stu_competition_excel.TableName = "競賽參與紀錄";
                //檢定
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_license_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_license_excel.Load(dr);
                sp_L01_stu_license_excel.TableName = "檢定證照紀錄";
                //服務學習
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_volunteer_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_volunteer_excel.Load(dr);
                sp_L01_stu_volunteer_excel.TableName = "服務學習紀錄";
                //彈性學習
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_study_free_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_study_free_excel.Load(dr);
                sp_L01_stu_study_free_excel.TableName = "彈性學習時間紀錄";
                //團體活動
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_group_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_group_excel.Load(dr);
                sp_L01_stu_group_excel.TableName = "團體活動時間紀錄";
                //職場
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_workplace_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_workplace_excel.Load(dr);
                sp_L01_stu_workplace_excel.TableName = "職場學習紀錄";
                //成果
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_result_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_result_excel.Load(dr);
                sp_L01_stu_result_excel.TableName = "作品成果紀錄";
                //大學及大專
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_college_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_college_excel.Load(dr);
                sp_L01_stu_college_excel.TableName = "大學及技專校院先修課程紀錄";
                //其他
                dr = await conn.ExecuteReaderAsync("sp_L01_stu_other_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_stu_other_excel.Load(dr);
                sp_L01_stu_other_excel.TableName = "其他多元表現紀錄";
            }

            using (var work = new XLWorkbook())
            {
                IXLWorksheet sheet = work.Worksheets.Add("封面");
                sheet.Cell(1, 1).SetValue("學校代碼");
                sheet.Cell(1, 2).SetValue("學年度");
                sheet.Cell(1, 3).SetValue("學期");
                sheet.Cell(1, 4).SetValue("名冊別");

                sheet.Cell(2, 1).SetValue(_context.SchNo);
                sheet.Cell(2, 2).SetValue(parameter.year_id);
                sheet.Cell(2, 3).SetValue(0);
                sheet.Cell(2, 4).SetValue(parameter.type);

                work.Worksheets.Add(sp_L01_std_position_excel);
                work.Worksheets.Add(sp_L01_stu_competition_excel);
                work.Worksheets.Add(sp_L01_stu_license_excel);
                work.Worksheets.Add(sp_L01_stu_volunteer_excel);
                work.Worksheets.Add(sp_L01_stu_study_free_excel);
                work.Worksheets.Add(sp_L01_stu_group_excel);
                work.Worksheets.Add(sp_L01_stu_workplace_excel);
                work.Worksheets.Add(sp_L01_stu_result_excel);
                work.Worksheets.Add(sp_L01_stu_college_excel);
                work.Worksheets.Add(sp_L01_stu_other_excel);

                work.SaveAs(stream);
            }


            return stream;
        }

        public async Task<MemoryStream> GetLearningResultExcelStream(Parameter parameter)
        {
            MemoryStream stream = new();
            DataTable sp_L01_learning_courses_excel = new();
            DataTable sp_L01_retake_courses_excel = new();
            DataTable sp_L01_remedial_courses_excel = new();

            using (IDbConnection conn = _context.CreateCommand())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@sch_no", _context.SchNo, DbType.String, ParameterDirection.Input);
                parameters.Add("@year_id", parameter.year_id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@grd_id", parameter.grd_id, DbType.Int16, ParameterDirection.Input);
                //修課
                IDataReader dr = await conn.ExecuteReaderAsync("sp_L01_learning_courses_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_learning_courses_excel.Load(dr);
                sp_L01_learning_courses_excel.TableName = "學期課程學習成果";
                //重修
                dr = await conn.ExecuteReaderAsync("sp_L01_retake_courses_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_retake_courses_excel.Load(dr);
                sp_L01_retake_courses_excel.TableName = "補修課程學習成果";
                //補修
                dr = await conn.ExecuteReaderAsync("sp_L01_remedial_courses_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_remedial_courses_excel.Load(dr);
                sp_L01_remedial_courses_excel.TableName = "重修課程學習成果";
            }

            using (var work = new XLWorkbook())
            {
                IXLWorksheet sheet = work.Worksheets.Add("封面");
                sheet.Cell(1, 1).SetValue("學校代碼");
                sheet.Cell(1, 2).SetValue("學年度");
                sheet.Cell(1, 3).SetValue("學期");
                sheet.Cell(1, 4).SetValue("名冊別");

                sheet.Cell(2, 1).SetValue(_context.SchNo);
                sheet.Cell(2, 2).SetValue(parameter.year_id);
                sheet.Cell(2, 3).SetValue(0);
                sheet.Cell(2, 4).SetValue(parameter.type);

                work.Worksheets.Add(sp_L01_learning_courses_excel);
                work.Worksheets.Add(sp_L01_retake_courses_excel);
                work.Worksheets.Add(sp_L01_remedial_courses_excel);

                work.SaveAs(stream);
            }


            return stream;
        }

        public async Task<MemoryStream> GetStdPositionFromSchExcelStream(StdPositionFromSchParameter parameter)
        {
            MemoryStream stream = new();
            DataTable sp_L01_std_position_from_sch_excel = new();

            using (IDbConnection conn = _context.CreateCommand())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@year_id", parameter.year_id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@sms_id", parameter.sms_id, DbType.Int16, ParameterDirection.Input);
                parameters.Add("@grd_id", parameter.grd_id, DbType.Int16, ParameterDirection.Input);
                //幹部
                IDataReader dr = await conn.ExecuteReaderAsync("sp_L01_std_position_from_sch_excel", parameters, commandType: CommandType.StoredProcedure);
                sp_L01_std_position_from_sch_excel.Load(dr);
                sp_L01_std_position_from_sch_excel.TableName = "幹部經歷紀錄";
            }

            using (var work = new XLWorkbook())
            {
                IXLWorksheet sheet = work.Worksheets.Add("封面");
                sheet.Cell(1, 1).SetValue("學校代碼");
                sheet.Cell(1, 2).SetValue("學年度");
                sheet.Cell(1, 3).SetValue("學期");
                sheet.Cell(1, 4).SetValue("名冊別");

                sheet.Cell(2, 1).SetValue(_context.SchNo);
                sheet.Cell(2, 2).SetValue(parameter.year_id);
                sheet.Cell(2, 3).SetValue(parameter.sms_id);
                sheet.Cell(2, 4).SetValue(parameter.type);

                work.Worksheets.Add(sp_L01_std_position_from_sch_excel);

                work.SaveAs(stream);
            }


            return stream;
        }
    }
}
