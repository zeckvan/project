using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.StuGroup.DbModels;
using StudentLearningHistory.Models.StuFileInfo.Parameters;
using StudentLearningHistory.Services;
using AutoMapper;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StudCadre.DTOs;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StuPublickFileHub.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using SharpCompress;
using System.IO;
using System;
using Azure;
using System.Web;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuFileInfoController : Controller
    {
        private readonly StuFileInfoService _service;
        private readonly ZipHelper _zip;
        private readonly FileHelper _file;
        private readonly MD5Helper _md5;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public StuFileInfoController(StuFileInfoService service, IMapper mapper, IConfiguration configuration, ZipHelper zip, FileHelper file, MD5Helper md5)
        {
            _service = service;
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _file = file;
            _md5 = md5;
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFileList([FromQuery] StuFileInfoQueryList arg)
        {
            try
            {
                StuFileInfoQueryList parameter = _mapper.Map<StuFileInfoQueryList>(arg);
                IEnumerable<L01_std_public_filehub_DTO> data = await _service.GetFileList(arg);
                return Json(new
                {
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
        public async Task<IActionResult> GetFile([FromQuery] StuFileInfoQueryList arg)
        {
            try
            {
                StuFileInfoQueryList parameter = _mapper.Map<StuFileInfoQueryList>(arg);
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
                    // return StatusCode(500, _md5.AbstractFile(file.file_blob));

                   //System.IO.File.WriteAllBytes(@"D:\MyFolder\zeck99.png", _zip.UnzipData(file.file_blob));

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

        [HttpGet("filedownload/{arg}")]
        public async Task<IActionResult> GetFileDownLoad(string arg)
        {
            try
            {
                var file = await _service.GetFileDownLoad(arg);

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
                    // return StatusCode(500, _md5.AbstractFile(file.file_blob));

                    //System.IO.File.WriteAllBytes(@"D:\MyFolder\zeck99.png", _zip.UnzipData(file.file_blob));
                    string filename = HttpUtility.UrlDecode(file.file_name);
                    return File(_zip.UnzipData(file.file_blob), contentType, $"{filename}.{file.file_extension}");
                    //return File(_zip.UnzipData(file.file_blob), contentType, $"{file.file_name}.{file.file_extension}");
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
        public async Task<IActionResult> PostFiles([FromForm] StuFileInfoQueryFile arg)
        {
            try
            {
                StuFileInfoQueryFile parameter = _mapper.Map<StuFileInfoQueryFile>(arg);


                int isOK = 0;
                string aa = string.Empty;
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
                            byte[] file_blob = _zip.ZipData(memoryStream, $"{file_name}.{extension}");
                             string file_md5 =  _md5.AbstractFile(file_blob);

                            L01_std_public_filehub insert = new()
                            {
                                //complex_key = parameter.complex_key,
                                complex_key = arg.complex_key,
                                class_name = arg.class_name,
                                content = arg.content,
                                file_name = file_name,
                                type_id = 0,
                                file_extension = extension,
                                file_blob = file_blob,
                                //_zip.ZipData(memoryStream, $"{file_name}.{extension}"),
                                upd_name = arg.std_no,
                                token = arg.token,
                                file_md5 = file_md5
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

                return Json(new { dataset = "", message = (isOK > 0 ? "新增成功" : "新增失敗"), status = (isOK > 0 ? "Y" : "N") });

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
       
        [HttpDelete("file")]
        public async Task<IActionResult> DeleteFiles(StuFileInfoQueryList arg)
        {
            try
            {
                StuFileInfoQueryList parameter = _mapper.Map<StuFileInfoQueryList>(arg);
                int isOK = await _service.DeleteFile(arg);
                return Json(new { dataset = "", message = (isOK > 0 ? "刪除成功" : "刪除失敗"), status = (isOK > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
