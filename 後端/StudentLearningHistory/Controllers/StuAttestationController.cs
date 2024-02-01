using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.StuAttestation.DbModels;
using StudentLearningHistory.Models.StuAttestation.Parameters;
using StudentLearningHistory.Services;
using AutoMapper;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.UserProfiles;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuAttestationController : Controller
    {
        private readonly StuAttestationService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public StuAttestationController(StuAttestationService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _GetUserProfilesService = getUserProfilesService;
        }
        [HttpGet("courselist")]
        public async Task<IActionResult> GetCourseList([FromQuery] StuAttestationQueryList arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id;
            }
            StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetCourseList(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] StuAttestationQueryList arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id;
            }
            StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetList(arg);
            return Json(new { 
                                            dataset = data, 
                                            status = (data.Count() <= 0 ? "N" : "Y"),
                                            total = 0}
                        );
        }

        [HttpGet("listconfirm")]
        public async Task<IActionResult> GetListConfirm([FromQuery] StuAttestationQueryList arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id;
            }

            StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetListConfirm(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }

        [HttpGet("listcentraldb")]
        public async Task<IActionResult> GetListCentraldb([FromQuery] StuAttestationQueryList arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id;
            }
            StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetListCentraldb(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }

        [HttpGet]
        public async Task<IActionResult> GetFormData([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<L01_StuAttestation> data = await _service.GetFormData(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteData(StuAttestationQueryList arg)
        {
            int data = await _service.DeleteData(arg);
            return Json(new { dataset = "", message = (data > 0 ? "刪除成功" : "刪除失敗"), status = (data > 0 ? "Y" : "N") });
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateData([FromForm] L01_StuAttestation arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut("centraldb")]
        public async Task<IActionResult> UpdateDataCentraldb([FromForm] L01_StuAttestationList arg)
        {
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataCentraldb(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut("fileCheckYN")]
        public async Task<IActionResult> UpdateDataCheckYN([FromForm] L01_StuAttestationCheck arg)
        {
            int data = await _service.UpdateDataCheckYN(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "異動失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> UpdateDataConfirm([FromForm] L01_StuAttestationList arg)
        {
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataConfirm(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromForm] L01_StuAttestation arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            //QueryList cadre = _mapper.Map<QueryList>(arg);
            var dict = new Dictionary<string, string>();
            dict = await _service.InsertData(arg);

            return Json(new { 
                                dataset = "", 
                                message = (dict["haveData"] == "Y" ? "資料已存在" : dict["result"] == "0" ? "存檔失敗" : "存檔成功"), 
                                status = (dict["result"] == "0" ? "N" : "Y") 
                            }
                        );
        }
    }
}
