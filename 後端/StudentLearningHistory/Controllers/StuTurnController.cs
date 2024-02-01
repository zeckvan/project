using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using StudentLearningHistory.Services;
using StudentLearningHistory.Models.StuTurn.Parameter;
using Microsoft.AspNetCore.Http.HttpResults;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using System.Security.Cryptography;
using StudentLearningHistory.Models.System.DbModels;
using Microsoft.AspNetCore.Authorization;
using StudentLearningHistory.Helpers;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Irony.Parsing;
using StudentLearningHistory.Models.StuFileInfo.Parameters;
using StudentLearningHistory.Models.StuGroup.DbModels;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuTurnController : Controller
    {
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly StuTurnService _TurnService;
        public StuTurnController(GetUserProfilesService getUserProfilesService, StuTurnService StuTurnService, FileHelper file, ZipHelper zip)
        {
            _GetUserProfilesService = getUserProfilesService;
            _TurnService = StuTurnService;
            _file = file;
            _zip = zip;
        }

        [HttpPost("Export")]
        public async Task<IActionResult> Turn([FromForm] StdList arg)
        {
            string data = await _TurnService.ExportJson(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = data.Count()
            }
            );
        }
        [HttpPost("Import")]
        public async Task<IActionResult> Import([FromForm] Models.StuTurn.Parameter.FileInfo arg)
        {
            var result = await _TurnService.ImportJson(arg);
            return Json(new
                {
                    dataset = "",
                    status = (result == "匯入成功" ? "Y" : "N"),
                    total = 0,
                    error_msg = result
            }
            );
        }
        [HttpGet("ExportFile")]
        public async Task<IActionResult> GetFile([FromQuery] GetFileInfo arg)
        {
            try
            {
                var file = await _TurnService.ExportFile(arg);
             
                string contentType = "application/x-zip-compressed";
                return File(_zip.UnzipData(file.file_blob), contentType, $"{file.file_name}.{file.file_extension}");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet("ExportFileList")]
        public async Task<IActionResult> GetFileList()
        {
            IEnumerable<GetFileInfo> data = await _TurnService.ExportFileList();
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
    }
}
