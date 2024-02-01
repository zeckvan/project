using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.TeaAttestation.DbModels;
using StudentLearningHistory.Models.TeaAttestation.Parameters;
using StudentLearningHistory.Services;
using AutoMapper;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.UserProfiles;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeaAttestationController : Controller
    {
        private readonly TeaAttestationService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TeaAttestationController(TeaAttestationService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _GetUserProfilesService = getUserProfilesService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
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
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
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

        [HttpGet("listrelease")]
        public async Task<IActionResult> GetListRelease([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
            StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetListRelease(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }

        [HttpGet("clslist")]
        public async Task<IActionResult> GetListCls([FromQuery] StuAttestationQueryList arg)
        {
            //TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            //arg.emp_id = token_data.result.user_id;
            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<ClsList> data = await _service.GetListCls(arg);
            return Json(new
                                            {
                                                dataset = data,
                                                status = (data.Count() <= 0 ? "N" : "Y"),
                                                total = 0
                                            }
                        );
        }

        [HttpGet("attestationResult")]
        public async Task<IActionResult> GetListStuStatus([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetListStuStatus(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }

        [HttpGet("LearningResult")]
        public async Task<IActionResult> GetListLearningResult([FromQuery] StuAttestationQueryList arg)
        {
            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<LearningResult> data = await _service.GetListLearningResult(arg);
            return Json(new
                                        {
                                            dataset = data,
                                            status = (data.Count() <= 0 ? "N" : "Y"),
                                            total = 0
                                        }
                                   );
        }

        [HttpGet("MultipleLearning")]
        public async Task<IActionResult> GetListMultipleLearning([FromQuery] StuAttestationQueryList arg)
        {
            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<MultipleLearning> data = await _service.GetListMultipleLearning(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }

        [HttpGet("AttestationNotYet")]
        public async Task<IActionResult> GetListAttestationNotYet([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);

            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<AttestationNotYet> data = await _service.GetListAttestationNotYet(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }

        [HttpGet("AttestationStdNotYet")]
        public async Task<IActionResult> GetListAttestationStdNotYet([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);

            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<AttestationNotYet> data = await _service.GetListAttestationStdNotYet(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = 0
            }
                        );
        }
        [HttpGet("teaTutor")]
        public async Task<IActionResult> GetListTutor([FromQuery] StuAttestationQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
            arg.consult_emp = token_data.result.user_id;
            //StuAttestationQueryList cadre = _mapper.Map<StuAttestationQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetListTutor(arg);
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

        [HttpPut]
        public async Task<IActionResult> UpdateData([FromForm] L01_StuAttestation arg)
        {
            L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut("confirmReason")]
        public async Task<IActionResult> UpdateDataConfirmReason([FromForm] L01_StuAttestationList arg)
        {
           // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataConfirmReason(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> UpdateDataConfirm([FromForm] L01_StuAttestationList arg)
        {
          // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataConfirm(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPut("confirmRelease")]
        public async Task<IActionResult> UpdateDataConfirmRelease([FromForm] L01_StuAttestationList arg)
        {
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataConfirmRelease(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromForm] L01_StuAttestation arg)
        {
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
