using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StuCompetition.Parameters;
using StudentLearningHistory.Models.StudCadre.DbModels;
using StudentLearningHistory.Models.StudCadre.DTOs;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StuStudyf.Parameter;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Services;
using System.IO.Compression;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudCadreController : Controller
    {
        private readonly StudCadreService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public StudCadreController(StudCadreService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, GetUserProfilesService getUserProfilesService)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _GetUserProfilesService = getUserProfilesService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Gets([FromQuery] StudCadreParameters arg)
        {
            //cadres
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = data.result.user_id.Trim();
            }
           
            try
            {
   
                StudCadreParameter_DB cadre = _mapper.Map<StudCadreParameter_DB>(arg);
                IEnumerable<L01_std_position> std = await _service.GetCadreList(cadre);
                IEnumerable<StudCadreListDTO> DTO = _mapper.Map<IEnumerable<StudCadreListDTO>>(std);

                foreach (StudCadreListDTO item in DTO)
                {
                    StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(item);
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
        public async Task<IActionResult> Get([FromQuery] StudCadreParameter arg)
        {
            if (string.IsNullOrEmpty(arg.std_no))
            {
                TokenResult? data = _GetUserProfilesService.GetUserData(arg.token);
                arg.std_no = data.result.user_id.Trim();
            }

            try
            {               
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(arg);
                L01_std_position std = await _service.GetCadre(parameter);
                StudCadreFormDTO DTO = _mapper.Map<StudCadreFormDTO>(std);

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

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] L01_std_position_DTO arg)
        {
            TokenResult? data = _GetUserProfilesService.GetUserData(arg.token);
            try
            {
                arg.std_no = data.result.user_id.Trim();
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(arg);
                if (!await _service.CheckCadre(parameter))
                {
                    L01_std_position cadre = _mapper.Map<L01_std_position>(arg);

                    int isOK = await _service.InsertCadre(cadre);

                    return Json(new { dataset = "", message= (isOK > 0 ? "存檔成功" : "存檔失敗"), status = (isOK > 0 ? "Y" : "N") });
                }
                else
                {
                    return Json(new { dataset = "", message="資料已存在", status = "N" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] L01_std_position_DTO arg)
        {
            TokenResult? data = _GetUserProfilesService.GetUserData(arg.token);
            try
            {
                arg.std_no = data.result.user_id.Trim();
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(arg);
                if (await _service.CheckCadre(parameter))
                {
                    L01_std_position cadre = _mapper.Map<L01_std_position>(arg);

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
        public async Task<IActionResult> Delete(StudCadreParameter_DEL arg)
        {
            TokenResult? data = _GetUserProfilesService.GetUserData(arg.token);
            try
            {
                arg.std_no = data.result.user_id.Trim();
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(arg);

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
        public async Task<IActionResult> GetFiles([FromQuery] StudCadreFilesParameter parameterfiles)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(parameterfiles);
                var DTO = await _service.GetFileList(parameter);

                return Json(new { dataset = DTO, status = "Y" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("file")]
        public async Task<IActionResult> GetFile([FromQuery] StudCadreFileParameter parameterfile)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(parameterfile);
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
        public async Task<IActionResult> PostFiles([FromForm] L01_std_File_insertDTO DTO)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(DTO);

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
        public async Task<IActionResult> PutFiles([FromForm] L01_std_File_updateDTO DTO)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(DTO);

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
        public async Task<IActionResult> DeleteFiles(StudCadreFileParameter_DEL parameter_del)
        {
            /*
            少一段，登入者跟進來的stdno是不是一樣的判斷，目前沒有寫因為沒有做登入，若沒有這個判斷只要知道學號就可以隨便查詢
            */

            try
            {
                StudCadreParameter_DB parameter = _mapper.Map<StudCadreParameter_DB>(parameter_del);

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
            // L01_StuAttestation cadre = _mapper.Map<L01_StuAttestation>(arg);
            int data = await _service.UpdateDataCentraldb(arg);

            return Json(new { dataset = "", message = (data > 0 ? "存檔成功" : "存檔失敗"), status = (data > 0 ? "Y" : "N") });
        }
    }
}
