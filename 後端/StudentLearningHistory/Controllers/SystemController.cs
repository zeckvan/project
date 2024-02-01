using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpCompress;
using StudentLearningHistory.Models.System.DbModels;
using StudentLearningHistory.Models.UserProfiles;
//using StudentLearningHistory.Models.System.Parameters;
using StudentLearningHistory.Services;

namespace StudentLearningHistory.Controllers
{
    [ApiController]
    [Route("api")]
    //[Authorize]
    public class SystemController : Controller
    {
        private readonly SystemService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        public SystemController(SystemService service, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _GetUserProfilesService = getUserProfilesService;
        }

        #region L01_setup
        [HttpGet("Get_L01_setup")]
        public async Task<IActionResult> Get_L01_setup()
        {
            //TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            IEnumerable<L01_setup> data = await _service.Get_L01_setup();
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        [HttpPut("Update_L01_setup")]
        public async Task<IActionResult> UpdateDataSetUp([FromForm] L01_setup_array arg)
        {
           // TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            int data = await _service.UpdateDataSetUp(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpGet("Get_L01_operate")]
        public async Task<IActionResult> Get_L01_operate()
        {
            IEnumerable<L01_operate> data = await _service.Get_L01_operate();
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }

        [HttpPut("Update_L01_operate")]
        public async Task<IActionResult> UpdateDataOperate([FromForm] L01_operate_array arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);

            //var i = 0;
            //foreach (L01_operate_array item in arg.AsEnumerable() ) 
            //{
            //    item.year_id[i] = token_data.result.now_year;
            //    item.sms_id[i] = token_data.result.now_sms;
            //    i++;
            //}
            int data = await _service.UpdateDataOperate(arg);
            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpGet("Get_L01_operate_open")]
        public async Task<IActionResult> Get_L01_operate_open([FromQuery] L01_operate arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            //arg.year_id = token_data.result.now_year;
            //arg.year_id = 111;
            //if (arg.type_id == "02" || arg.type_id == "01")
            //{
            //    arg.sms_id = token_data.result.now_sms;
            //}

           L01_operate data = await _service.Get_L01_operate_open(arg);
            return Json(new { dataset = data, status = "Y" });
            //return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }

        [HttpGet("Get_Diverse_Total")]
        public async Task<IActionResult> Get_Diverse_Total([FromQuery]  L01_Diverse_Total arg)
        {
            IEnumerable<L01_Diverse_Total> data = await _service.Get_Diverse_Total(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
    }
}
