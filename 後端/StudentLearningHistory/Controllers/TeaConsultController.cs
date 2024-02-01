using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.TeaConsult.DbModels;
using StudentLearningHistory.Models.TeaConsult.Parameters;
using StudentLearningHistory.Services;
using AutoMapper;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StudCadre.DTOs;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.Public.DbModels;
using StudentLearningHistory.Models.UserProfiles;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeaConsultController : Controller
    {
        private readonly TeaConsultService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TeaConsultController(TeaConsultService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _GetUserProfilesService = getUserProfilesService;
        }

        [HttpGet("clsdata")]
        public async Task<IActionResult> GetCls([FromQuery] TeaConsultQueryStu arg)
        {
            TeaConsultQueryStu cadre = _mapper.Map<TeaConsultQueryStu>(arg);
            IEnumerable<S90_Class> data = await _service.GetCls(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = data.Count()
            }
                        );
        }

        [HttpGet("consultstulist")]
        public async Task<IActionResult> GetConsultStuList([FromQuery] TeaConsulteQueryList arg)
        {
            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            IEnumerable<SubHeaderList> data = await _service.GetConsultStuList(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y")
            }
                        );
        }

        [HttpGet("stulist")]
        public async Task<IActionResult> GetStuList([FromQuery] TeaConsulteQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);

            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            IEnumerable<SubHeaderList> data = await _service.GetStuList(arg);
            return Json(new
            {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = data.Count()
            }
                        );
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] TeaConsulteQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            IEnumerable<HeaderList> data = await _service.GetList(arg);
            return Json(new {
                dataset = data,
                status = (data.Count() <= 0 ? "N" : "Y"),
                total = data.Count()
            }
                        );
        }

        [HttpGet]
        public async Task<IActionResult> GetFormData([FromQuery] TeaConsulteQueryList arg)
        {
            //TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            //arg.emp_id = token_data.result.user_id;
            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            IEnumerable<L03_TeaConsult> data = await _service.GetFormData(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }

        [HttpDelete("stulist")]
        public async Task<IActionResult> DeleteStuData(TeaConsulteQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            
            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            int data = await _service.DeleteStuData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "刪除成功" : "刪除失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteData(TeaConsulteQueryList arg)
        {
            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            int data = await _service.DeleteData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "刪除成功" : "刪除失敗"), status = (data > 0 ? "Y" : "N") });
        }
        /*
        [HttpDelete("consultstulist")]
        public async Task<IActionResult> DeleteData(TeaConsulteQueryList arg)
        {
            TeaConsulteQueryList cadre = _mapper.Map<TeaConsulteQueryList>(arg);
            int data = await _service.DeleteData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "刪除成功" : "刪除失敗"), status = (data > 0 ? "Y" : "N") });
        }
        */
        [HttpPut]
        public async Task<IActionResult> UpdateData([FromForm] L03_TeaConsult arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
            L03_TeaConsult cadre = _mapper.Map<L03_TeaConsult>(arg);
            int data = await _service.UpdateData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromForm] L03_TeaConsult arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.emp_id = token_data.result.user_id;
            var dict = new Dictionary<string, string>();
            dict = await _service.InsertData(arg);

            return Json(new {
                dataset = "",
                message = (dict["haveData"] == "Y" ? "資料已存在" : dict["result"] == "0" ? "存檔失敗" : "存檔成功"),
                status = (dict["result"] == "0" ? "N" : "Y")
            }
                        );
        }

        [HttpPost("consultstulist")]
        public async Task<IActionResult> InsertConsultStu([FromForm] L03_TeaConsult_Stu arg)
        {
            var dict = new Dictionary<string, string>();
            dict = await _service.InsertConsultStu(arg);

            return Json(new {
                                            dataset = "",
                                            message = (dict["haveData"]),
                                            status = (dict["result"] == "0" ? "N" : "Y")
                                        }
                        );
        }
        #region s04_stuclsPage
        [HttpGet("Get_consult_setup")]
        public async Task<IActionResult> Get_consult_setup([FromQuery] TeaConsultQuerySetup arg)
        {
            IEnumerable<L03_TeaConsult_Stu> data = await _service.Get_consult_setup(arg);
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion

        [HttpDelete("Consult_SetUp")]
        public async Task<IActionResult> DeleteConsult_SetUp(TeaConsultDeleteList arg)
        {
            int data = await _service.DeleteConsult_SetUp(arg);

            return Json(new { dataset = "", message = (data > 0 ? "刪除成功" : "刪除失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPost("Consult_SetUp")]
        public async Task<IActionResult> InsertConsult_SetUp([FromForm] TeaConsultDeleteList arg)
        {
            var dict = new Dictionary<string, string>();
            dict = await _service.InsertConsult_SetUp(arg);

            return Json(new
            {
                dataset = "",
                message = (dict["haveData"]),
                status = (dict["result"] == "0" ? "N" : "Y")
            }
                        );
        }
    }
}
