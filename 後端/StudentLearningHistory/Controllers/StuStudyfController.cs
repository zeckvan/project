using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Models.StuGroup.DbModels;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StuResult.Parameters;
using StudentLearningHistory.Models.StuStudyf.DB;
using StudentLearningHistory.Models.StuStudyf.DTO;
using StudentLearningHistory.Models.StuStudyf.Parameter;
using StudentLearningHistory.Models.StuStudyf.Parameters;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Services;


namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuStudyfController : Controller
    {
        private readonly StuStudyfService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public StuStudyfController(StuStudyfService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _GetUserProfilesService = getUserProfilesService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Gets([FromQuery] StuStudyFreeParameterList arg)
        {
            try
            {
                if (string.IsNullOrEmpty(arg.std_no))
                {
                    TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                    arg.std_no = token_data.result.user_id.Trim();
                }
                StuStudyFreeParameter_DB data = _mapper.Map<StuStudyFreeParameter_DB>(arg);
                IEnumerable<L01_stu_study_free> std = await _service.GetCadreList(data);
                IEnumerable<StuStudyFreeListDTO> DTO = _mapper.Map<IEnumerable<StuStudyFreeListDTO>>(std);

                foreach (StuStudyFreeListDTO item in DTO)
                {
                    StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(item);
                    item.files = await _service.GetFileList(parameter);
                }

                return Json(new { dataset = DTO, status = (DTO.Count() <= 0 ? "N" : "Y"), total = DTO.Count() });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] StuStudyFreeParameter arg)
        {
            try
            {
                if (string.IsNullOrEmpty(arg.std_no))
                {
                    TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                    arg.std_no = token_data.result.user_id.Trim();
                }
                StuStudyFreeParameter_DB data = _mapper.Map<StuStudyFreeParameter_DB>(arg);
                L01_stu_study_free std = await _service.GetCadre(data);
                L01_stu_study_free_DTO DTO = _mapper.Map<L01_stu_study_free_DTO>(std);

                if (std != null)
                {
                    return Json(new { dataset = DTO, status = "Y" });
                }
                else
                {

                    return Json(new { dataset = "", message= "資料不存在", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Post([FromForm] L01_stu_study_free_DTO DTO)
        //{
        //    /*
        //    少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
        //    */

        //    try
        //    {
        //        string errorMsg = CheckData(DTO);
        //        if (!string.IsNullOrWhiteSpace(errorMsg))
        //        {
        //            return Json(new { dataset = "", message = errorMsg, status = "N" });
        //        }

        //        StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(DTO);
        //        if (!await _service.CheckCadre(parameter))
        //        {
        //            L01_stu_study_free cadre = _mapper.Map<L01_stu_study_free>(DTO);

        //            int isOK = await _service.InsertCadre(cadre);

        //            return Json(new { dataset = "", message= (isOK > 0 ? "存檔成功" : "存檔失敗"), status = (isOK > 0 ? "Y" : "N") });
        //        }
        //        else
        //        {
        //            return Json(new { dataset = "", message="資料已存在", status = "N" });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, e.Message);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] L01_stu_study_free_2 arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id.Trim();
            var dict = new Dictionary<string, string>();
            dict = await _service.InsertData(arg);

            return Json(new
            {
                dataset = "",
                message = (dict["haveData"] == "Y" ? "資料已存在" : dict["result"] == "0" ? "存檔失敗" : "存檔成功"),
                status = (dict["result"] == "0" ? "N" : "Y")
            }
                        );
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] L01_stu_study_free_DTO arg)
        {
            try
            {
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = token_data.result.user_id.Trim();
                string errorMsg = CheckData(arg);
                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    return Json(new { dataset = "", message = errorMsg, status = "N" });
                }

                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(arg);
                parameter.std_no = token_data.result.user_id.Trim();
                if (await _service.CheckCadre(parameter))
                {
                    L01_stu_study_free cadre = _mapper.Map<L01_stu_study_free>(arg);

                    int isOK = await _service.UpdateCadre(cadre);

                    return Json(new { dataset = "", message = (isOK > 0 ? "更新成功" : "更新失敗"), status = (isOK > 0 ? "Y" : "N") });
                }
                else
                {
                    return Json(new { dataset = "", message= "資料不存在", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(StuStudyFreeParameter arg)
        {

            try
            {
                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(arg);
                TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
                parameter.std_no = token_data.result.user_id.Trim();

                if (await _service.CheckCadre(parameter))
                {
                    int isOK = await _service.DelectCadre(parameter);

                    return Json(new { dataset = "", message = (isOK > 0 ? "刪除成功" : "刪除失敗"), status = (isOK > 0 ? "Y" : "N") });
                }
                else
                {
                    return Json(new { dataset = "", message= "資料不存在", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles([FromQuery] StuStudyFreeFileParameterList parameterfiles)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(parameterfiles);
                var DTO = await _service.GetFileList(parameter);

                return Json(new { dataset = DTO, status = "Y" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("file")]
        public async Task<IActionResult> GetFile([FromQuery] StuStudyFreeFileParameter parameterfile)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(parameterfile);
                var file = await _service.GetFile(parameter);

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
        public async Task<IActionResult> PostFiles([FromForm] StuStudyFreeFile_insertDTO DTO)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(DTO);

                if (await _service.CheckCadre(parameter))
                {
                    int isOK = 0;

                    string schno = _configuration.GetSection("setting").GetValue<string>("SchNo");
                    parameter.sch_no = schno;

                    List<L01_std_public_filehub> files = new List<L01_std_public_filehub>();
                    foreach (IFormFile file in DTO.files)
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
                                    class_name = DTO.class_name,
                                    file_name = file_name,
                                    type_id = 0,
                                    file_extension = extension,
                                    file_blob = _zip.ZipData(memoryStream, $"{file_name}.{extension}"),
                                    upd_name = DTO.std_no,
                                    content = DTO.content
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
                else
                {
                    return Json(new { dataset = "", message="沒有主檔無法新增", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("file")]
        public async Task<IActionResult> PutFiles([FromForm] StuStudyFreeFile_updateDTO DTO)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(DTO);

                string schno = _configuration.GetSection("setting").GetValue<string>("SchNo");
                parameter.sch_no = schno;

                if (await _service.CheckCadre(parameter))
                {
                    if (await _service.CheckCadreFile(parameter))
                    {
                        int isOK = 0;
                        
                        using (MemoryStream memoryStream = new())
                        {
                            await DTO.file.CopyToAsync(memoryStream);

                            if (_file.CheckFileExtensoinWithByte(memoryStream, out string extension))
                            {
                                string[] getName = DTO.file.FileName.Split(".");
                                getName = getName.Where((e, index) => index != getName.Length - 1).ToArray();
                                string file_name = string.Join("", getName);
                                L01_std_public_filehub file = new()
                                {
                                    complex_key = parameter.complex_key,
                                    class_name = DTO.class_name,
                                    file_name = file_name,
                                    type_id = 0,
                                    number_id = DTO.number_id,
                                    file_extension = extension,
                                    file_blob = _zip.ZipData(memoryStream, $"{file_name}.{extension}"),
                                    upd_name = DTO.std_no
                                };

                                isOK = await _service.UpdateFile(file);
                            }
                            else
                            {
                                return Json(new { dataset = "", message = $"{DTO.file.FileName}不符合規範", status = "N" });
                            }
                        }

                        return Json(new { dataset = "", message = (isOK > 0 ? "更新成功" : "更新失敗"), status = (isOK > 0 ? "Y" : "N") });
                    }
                    else
                    {
                        return Json(new { dataset = "", message = "檔案不存在無法更新", status = "N" });
                    }
                }
                else
                {
                    return Json(new { dataset = "", message="沒有主檔無法更新", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("file")]
        public async Task<IActionResult> DeleteFiles(StuStudyFreeFileParameter parameter_del)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StuStudyFreeParameter_DB parameter = _mapper.Map<StuStudyFreeParameter_DB>(parameter_del);

                if (await _service.CheckCadre(parameter))
                {
                    if (await _service.CheckCadreFile(parameter))
                    {
                        int isOK = await _service.DeleteFile(parameter);

                        return Json(new { dataset = "", message = (isOK > 0 ? "刪除成功" : "刪除失敗"), status = (isOK > 0 ? "Y" : "N") });
                    }
                    else
                    {
                        return Json(new { dataset = "", message = "沒有檔案無法刪除", status = "N" });
                    }
                }
                else
                {
                    return Json(new { dataset = "", message= "沒有主檔無法刪除", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("centraldb")]
        public async Task<IActionResult> UpdateDataCentraldb([FromForm] StuCadreQueryList arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id.Trim();
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataCentraldb(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }
        private string CheckData(L01_stu_study_free_DTO DTO)
        {
            string rt = "";
            if (DTO.weeks > 18 || DTO.weeks < 1)
            {
                rt = "規格不符";
            }
            if (DTO.content.Length > 100)
            {
                rt = "規格不符";
            }
            return rt;
        }
    }
}
