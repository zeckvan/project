using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.StuCollege.DbModels;
using StudentLearningHistory.Models.StuCollege.Parameters;
using StudentLearningHistory.Services;
using AutoMapper;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StudCadre.DTOs;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.UserProfiles;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuCollegeController : Controller
    {
        private readonly StuCollegeService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public StuCollegeController(StuCollegeService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _GetUserProfilesService = getUserProfilesService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] StuCollegeQueryList arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id;
            }
            StuCollegeQueryList cadre = _mapper.Map<StuCollegeQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetList(arg);
            return Json(new { 
                                dataset = data, 
                                status = (data.Count() <= 0 ? "N" : "Y"),
                                total = data.Count()
                            }
                        );
        }

        [HttpGet]
        public async Task<IActionResult> GetFormData([FromQuery] StuCollegeQueryList arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id;
            }
            StuCollegeQueryList cadre = _mapper.Map<StuCollegeQueryList>(arg);
            IEnumerable<L01_StuCollege> data = await _service.GetFormData(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteData(StuCollegeQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            StuCollegeQueryList cadre = _mapper.Map<StuCollegeQueryList>(arg);
            int data = await _service.DeleteData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "刪除成功" : "刪除失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateData([FromForm] L01_StuCollege arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            L01_StuCollege cadre = _mapper.Map<L01_StuCollege>(arg);
            int data = await _service.UpdateData(arg);
            try
            {
                return Json(new { 
                                    dataset = "", 
                                    message = (data > 0 ? "存檔成功" : "存檔失敗"),
                                    status = (data > 0 ? "Y" : "N")
                                }
                            );
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromForm] L01_StuCollege arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            var dict = new Dictionary<string, string>();
            dict = await _service.InsertData(arg);

            return Json(new { 
                                dataset = "", 
                                message = (dict["haveData"] == "Y" ? "資料已存在" : dict["result"] == "0" ? "存檔失敗" : "存檔成功"), 
                                status = (dict["result"] == "0" ? "N" : "Y") 
                            }
                        );
        }

        [HttpPut("centraldb")]
        public async Task<IActionResult> UpdateDataCentraldb([FromForm] StuCollegeQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataCentraldb(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }
    }
}
