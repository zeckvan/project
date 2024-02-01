using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StudentLearningHistory.Models.StudentInfo.DbModels;
using StudentLearningHistory.Models.StudentInfo.DTOs;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Services;

namespace StudentLearningHistory.Controllers
{
    [ApiController]
    [Route("api")]
    //[Authorize]
    public class StudentInfoController : Controller
    {
        private readonly StudentInfoService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly IMapper _mapper;
        public StudentInfoController(StudentInfoService service, IMapper mapper, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _GetUserProfilesService = getUserProfilesService;
        }

        #region 學生資訊
        [HttpGet("studentinfo/{arg}")]
        public async Task<IActionResult> GetInfo(string arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg);
            arg = token_data.result.user_id;
            try
            {
                V_Student std = await _service.queryByStdNo<V_Student>(arg);
                if (std == null)
                {
                    return Json(new { dataset = std, status = "N" });
                }
                return Json(new { dataset = std, status = "Y" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        #endregion

        #region 學生資訊2
        [HttpPost("studentinfo")]
        public async Task<IActionResult> InsertData([FromForm] StudentInfo arg)
        {
            int rtn = await _service.InserStuInfo(arg);

            return Json(new
            {
                dataset = "",
                message = (rtn < 0 ? "存檔失敗" : "存檔成功"),
                status = (rtn < 0 ? "N" : "Y")
            });
        }
        #endregion

        #region 學生缺曠
        [HttpGet("absenceRecord/{arg}")]
        public async Task<IActionResult> GetAbsenceRecord([FromQuery] int year, [FromQuery] int sms, [FromQuery] string? mat_id, string arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg);
            arg = token_data.result.user_id;
            try
            {
                IEnumerable<V_AbsenceRecord> absenceRecords = await _service.queryByYearSmsStdNo<V_AbsenceRecord>(year, sms, mat_id, arg);
                if (absenceRecords.Count() == 0)
                {
                    return Json(new { dataset = absenceRecords, status = "N" });
                }
                return Json(new { dataset = absenceRecords, status = "Y" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        #endregion

        #region 學生獎懲
        [HttpGet("stuphrRecord/{arg}")]
        public async Task<IActionResult> GetStuphrRecord([FromQuery] int year, [FromQuery] int sms, string arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg);
            arg = token_data.result.user_id;
            try
            {
                IEnumerable<V_StuphrRecord> stuphrRecords = await _service.queryByYearSmsStdNoStuphr<V_StuphrRecord>(year, sms, arg);
                List<StuphrRecordDTO> VM = _mapper.Map<List<StuphrRecordDTO>>(stuphrRecords);
                if (VM.Count() == 0)
                {
                    return Json(new { dataset = VM, status = "N" });
                }
                return Json(new { dataset = VM, status = "Y" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        #endregion
    }
}
