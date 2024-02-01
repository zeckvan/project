using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Irony.Parsing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.Public.DbModels;
using StudentLearningHistory.Models.Public.Parameters;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Services;

namespace StudentLearningHistory.Controllers
{
    [ApiController]
    [Route("api")]
    //[Authorize]
    public class PublicController : Controller
    {
        private readonly PublicService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly IConfiguration _configuration;
        public PublicController(PublicService service, GetUserProfilesService getUserProfilesService, IConfiguration configuration)
        {
            _service = service;
            _GetUserProfilesService = getUserProfilesService;
            _configuration = configuration;
        }

        #region s90_yms
        [HttpGet("s90ymsinfo/{yms_mark}")]
        public async Task<IActionResult> GetS90Yms(string yms_mark)
        {
            // TokenResult? token_data = _GetUserProfilesService.GetUserData(token);

            IEnumerable<S90_Yms> data = await _service.Get_s90_yms(yms_mark);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        #region s90_year
        [HttpGet("s90yearinfo")]
        public async Task<IActionResult> GetS90Year()
        {
            IEnumerable<S90_Year> data = await _service.Get_s90_year();
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        #region s90_sms
        [HttpGet("s90smsinfo")]
        public async Task<IActionResult> GetS90Sms()
        {
            IEnumerable<S90_Sms> data = await _service.Get_s90_sms();
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        #region s04_stucls
        [HttpGet("S04_stucls")]
        public async Task<IActionResult> Get_s04_ytdbgoc([FromQuery] S04_ytdbgoc arg)
        {
            IEnumerable<S04_ytdbgoc> data = await _service.Get_s04_ytdbgoc(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        #region s04_stuclsPage
        [HttpGet("S04_stucls_page")]
        public async Task<IActionResult> Get_s04_ytdbgoc_page([FromQuery] S04_ytdbgoc arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.year_id = token_data.result.now_year;
            arg.sms_id = token_data.result.now_sms;

            IEnumerable<S04_ytdbgoc> data = await _service.Get_s04_ytdbgoc_page(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        #region S90_employee
        [HttpGet("S90_employee")]
        public async Task<IActionResult> Get_s90_employee([FromQuery] S90_employee_Param arg)
        {
            IEnumerable<S90_employee> data = await _service.Get_s90_employee(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y")
            }
                        );
        }
        #endregion

        #region S04_student
        [HttpGet("S04_student")]
        public async Task<IActionResult> Get_04_student([FromQuery] S04_student arg)
        {
            IEnumerable<S04_student> data = await _service.Get_04_student(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y")
            }
                        );
        }
        #endregion
    }
}
