using Ionic.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.ExportExcel.Parameters;
using StudentLearningHistory.Services;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportExcelController : Controller
    {
        private enum RosterType
        {
            多元學習表現 = 25,
            第一梯次多元學習表現 = 35,
            第二梯次多元學習表現 = 75
        }

        private enum LearningResult
        {
            學生課程學習成果 = 26,
            進修部學生課程學習成果 = 28,
            第一梯次學生課程學習成果 = 36,
            第一梯次進修部學生課程學習成果 = 38,
            第二梯次學生課程學習成果 = 76,
            第二梯次進修部學生課程學習成果 = 78
        }

        private enum StdPositionFromSch
        {
            校內幹部經歷名冊 = 24,
            第一梯次高三校內幹部經歷名冊 = 34,
            第二梯次高三校內幹部經歷名冊 = 74,
        }

        private readonly IConfiguration _configuration;
        private readonly ExportExcelService _service;
        private readonly string _schno;

        public ExportExcelController(IConfiguration configuration, ExportExcelService service)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpGet("MultipleLearning")]
        public async Task<IActionResult> GetMultipleLearning([FromQuery] Parameter arg)
        {
            try
            {
                MemoryStream mStream = await _service.GetMultipleLearningExcelStream(arg);

                return File(mStream.ToArray(), "application/vnd.ms-excel", $"{(RosterType)arg.type}.xlsx");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("LearningResult")]
        public async Task<IActionResult> GetLearningResult([FromQuery] Parameter arg)
        {
            try
            {
                MemoryStream mStream = await _service.GetLearningResultExcelStream(arg);

                return File(mStream.ToArray(), "application/vnd.ms-excel", $"{(LearningResult)arg.type}.xlsx");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("StdPositionFromSch")]
        public async Task<IActionResult> GetStdPositionFromSch([FromQuery] StdPositionFromSchParameter arg)
        {
            try
            {
                MemoryStream mStream = await _service.GetStdPositionFromSchExcelStream(arg);

                return File(mStream.ToArray(), "application/vnd.ms-excel", $"{(StdPositionFromSch)arg.type}.xlsx");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("test")]
        public void test()
        {
            string folderPath = @"D:\SoftCode\vue\demo_toDoList_Vue";
            string zipPath = @"D:\test.zip";

            using (var zip = new ZipFile())
            {
                zip.Password = "123";

                zip.AddDirectory(folderPath);

                zip.Save(zipPath);
            }
        }
    }
}
