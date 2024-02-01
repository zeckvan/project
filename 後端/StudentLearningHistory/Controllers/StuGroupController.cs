using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.StuGroup.DbModels;
using StudentLearningHistory.Models.StuGroup.Parameters;
using StudentLearningHistory.Services;
using AutoMapper;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StudCadre.DTOs;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StuCollege.Parameters;
using StudentLearningHistory.Models.UserProfiles;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuGroupController : Controller
    {
        private readonly StuGroupService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public StuGroupController(StuGroupService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
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
            IEnumerable<L01_StuGroup> data = await _service.GetFormData(arg);
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
        public async Task<IActionResult> UpdateData([FromForm] L01_StuGroup arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            L01_StuGroup cadre = _mapper.Map<L01_StuGroup>(arg);
            int data = await _service.UpdateData(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }

        [HttpPost]
        public async Task<IActionResult> InsertData([FromForm] L01_StuGroup arg)
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

        [HttpPut("centraldb")]
        public async Task<IActionResult> UpdateDataCentraldb([FromForm] StuCollegeQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataCentraldb(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }
        /*
        [HttpGet("files")]
        public async Task<IActionResult> GetFileList([FromQuery] QueryList arg)
        {
            try
            {
                QueryList parameter = _mapper.Map<QueryList>(arg);
                IEnumerable<L01_std_public_filehub_DTO> data = await _service.GetFileList(arg);
                return Json(new { 
                                    dataset = data, 
                                    status = (data.Count() <= 0 ? "N" : "Y"),
                                    total = data.Count()
                                }
                            );
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("file")]
        public async Task<IActionResult> GetFile([FromQuery] QueryList arg)
        {
            try
            {
                QueryList parameter = _mapper.Map<QueryList>(arg);
                var file = await _service.GetFile(arg);

                if (file != null)
                {
                    string contentType = string.Empty;

                    switch (file.file_extension.ToLower())
                    {
                        case "jpg":
                            contentType = "image/jpeg";
                            break;
                        case "png":
                            contentType = "image/png";
                            break;
                        case "pdf":
                            contentType = "application/pdf";
                            break;
                        case "mp3":
                            contentType = "audio/mp3";
                            break;
                        case "mp4":
                            contentType = "video/mp4";
                            break;
                    }

                    return File(_zip.UnzipData(file.file_blob), contentType, $"{file.file_name}.{file.file_extension}");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
 
        [HttpPost("file")]
        public async Task<IActionResult> PostFiles([FromForm] QueryFile arg)
        {
            try
            {
                QueryFile parameter = _mapper.Map<QueryFile>(arg);


                int isOK = 0;

                string schno = _configuration.GetSection("setting").GetValue<string>("SchNo");
                parameter.sch_no = schno;
                        
                List<L01_std_public_filehub> files = new List<L01_std_public_filehub>();
                foreach (IFormFile file in arg.files)
                {
                    using (MemoryStream memoryStream = new())
                    {
                        await file.CopyToAsync(memoryStream);

                        if (_file.CheckFileExtensoinWithByte(memoryStream, out string extension))
                        {
                            string[] getName = file.FileName.Split(".");
                            getName = getName.Where((e, index) => index != getName.Length - 1).ToArray();
                            string file_name = string.Join("", getName);
                            L01_std_public_filehub insert = new()
                            {
                                complex_key = parameter.complex_key,
                                class_name = "stugroup",
                                file_name = file_name,
                                type_id = 0,
                                file_extension = extension,
                                file_blob = _zip.ZipData(memoryStream, $"{file_name}.{extension}"),
                                upd_name = arg.std_no
                            };

                            files.Add(insert);
                        }
                        else
                        {
                            return Json(new { dataset = $"{file.FileName}不符合規範", status = "N" });
                        }
                    }
                }
                if (files.Count > 0)
                {
                    isOK = await _service.InsertFile(files);
                }

                return Json(new { dataset = "", message= (isOK > 0 ? "新增成功" : "新增失敗"), status = (isOK > 0 ? "Y" : "N") });

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
   
        [HttpDelete("file")]
        public async Task<IActionResult> DeleteFiles(QueryList arg)
        {
            try
            {
                QueryList parameter = _mapper.Map<QueryList>(arg);
                int isOK = await _service.DeleteFile(arg);
                return Json(new { dataset = "", message = (isOK > 0 ? "刪除成功" : "刪除失敗"), status = (isOK > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }         
         */
    }
}
