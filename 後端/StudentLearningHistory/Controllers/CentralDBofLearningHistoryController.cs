using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;
using StudentLearningHistory.Models.CentralDBofLearningHistory.JsonDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.DateSetupDTO;
using StudentLearningHistory.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackParameters;
using StudentLearningHistory.Models.CentralDBofLearningHistory.Std;
using StudentLearningHistory.Models.UserProfiles;

namespace StudentLearningHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentralDBofLearningHistoryController : Controller
    {
        private readonly ZipHelper _zip;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly CentralDBofLearningHistoryService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly string _schno;

        public CentralDBofLearningHistoryController(IMapper mapper, IConfiguration configuration, ZipHelper zip, CentralDBofLearningHistoryService service, GetUserProfilesService getUserProfilesService)
        {
            _mapper = mapper;
            _configuration = configuration;
            _zip = zip;
            _service = service;
            _schno = _configuration.GetSection("setting").GetValue<string>("SchNo");
            _GetUserProfilesService = getUserProfilesService;
        }
        #region 承辦人員
        [HttpGet("GetYms")]
        public async Task<IActionResult> GetYms()
        {
            IEnumerable<string> list = await _service.GenYmsList();

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }

        //匯入資料查詢 - 班級學生下拉清單
        [HttpGet("GetClsStd")]
        public async Task<IActionResult> GetClsStd(short year, short sms)
        {
            var list = await _service.GenClsStdList(year, sms);
            return Json(new { dataset = list, status = (list != null ? "Y" : "N") });
        }


        //匯入中央資料zip
        [HttpPost("Import")]
        public async Task<IActionResult> Post([FromForm] short year, [FromForm] short sms, [FromForm] string password, IFormFile file)
        {
            int data = 0;
            try
            {
                //讀取上傳ZIP
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                //解壓縮ZIP，整理成tupo（name， 記憶體串流）
                var dity = _zip.UnzipData(memoryStream.ToArray(), password);
                memoryStream.Close();

                //找出中央資料庫歷程SHA，整理成list
                List<string[]> shaList = new List<string[]>();
                var shaTxT = dity.SingleOrDefault(item => (item.name ?? "").ToLower() == "SHA.txt".ToLower());
                using MemoryStream SHAmemoryStream = new(shaTxT.fileBytes);
                using StreamReader reader = new(SHAmemoryStream);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fields = line.Split(',');

                    shaList.Add(fields);
                }
                SHAmemoryStream.Close();
                reader.Close();

                //轉SHA265並核對json是否正確，將正確的回傳
                List<string> correctJSON = new List<string>();
                foreach (var fInfo in dity.Where(item => (item.name ?? "").ToLower() != "SHA.txt".ToLower()))
                {
                    string[] findSHA_data = shaList.SingleOrDefault(item => item.Any(index => (index ?? "").ToLower() == (fInfo.name ?? "").ToLower()));
                    if (findSHA_data != null)
                    {
                        string isUsing = _configuration.GetSection("setting").GetSection("CentralDB").GetValue<string>("isUsingHSA256onUpload") ?? "";

                        if (isUsing == "Y")
                        {
                            //計算json的雜湊碼
                            using SHA256 mySHA256 = SHA256.Create();
                            byte[] hashValue = mySHA256.ComputeHash(fInfo.fileBytes);
                            StringBuilder stringBuilder = new();
                            for (int i = 0; i < hashValue.Length; i++)
                            {
                                stringBuilder.Append($"{hashValue[i]:X2}");
                            }
                            string sha256 = stringBuilder.ToString().ToLower();

                            //比對雜湊碼
                            if (!string.IsNullOrWhiteSpace(sha256))
                            {
                                if (findSHA_data.Any(index => (index ?? "").ToLower() == sha256.ToLower()))
                                {
                                    correctJSON.Add(fInfo.name);
                                }
                            }
                            correctJSON.Add(fInfo.name);
                        }
                        else
                        {
                            correctJSON.Add(fInfo.name);
                        }

                        correctJSON.Add(fInfo.name);
                    }
                }

                //只留下正確資料
                var finalyData = dity.Where(tupo => correctJSON.Contains(tupo.name ?? "")).ToList();
                //清理記憶體把無用的先清除
                shaList = null;
                correctJSON = null;
                dity = null;

                //轉換成進DB的類別
                var allModel = new List<List<L01_centraldb_learning_history_temp>>();
                finalyData.ForEach(tupo => {
                    //拆解出kind
                    string[] getName = tupo.name.Split(".");
                    getName = getName.Where((e, index) => index != getName.Length - 1).ToArray();
                    string name = string.Join("", getName);

                    //排除不做迴圈
                    string[] passProp = new[] { "身分證號", "出生日期" };

                    // 移除 UTF-8 BOM不然反轉json會出錯
                    string jsonString = Encoding.UTF8.GetString(tupo.fileBytes).TrimStart('\uFEFF');

                    var insertModel = new List<L01_centraldb_learning_history_temp>();

                    int serId = 0;
                    if (tupo.name.Contains("多元表現"))
                    {
                        //解析json
                        var mr = JsonSerializer.Deserialize<多元表現外層>(jsonString);
                        if (mr != null)
                        {
                            foreach (var std in mr.多元表現)
                            {
                                foreach (var prop in std.GetType().GetProperties().Where(p => !passProp.Contains(p.Name)).ToList())
                                {
                                    List<string> list = prop.GetValue(std) as List<string>;
                                    if (list == null) { continue; }
                                    foreach (var val in list)
                                    {
                                        L01_centraldb_learning_history_temp model = new()
                                        {
                                            year_id = year,
                                            sms_id = sms,
                                            sch_no = _schno,
                                            kind = name,
                                            cls = prop.Name,
                                            idno = std.身分證號,
                                            ser_id = ++serId,
                                            birthday = std.出生日期,
                                            json_head = mr.名冊資訊,
                                            json_content = val,
                                            upd_name = "管理者",
                                            zip_name = file.FileName
                                        };
                                        insertModel.Add(model);
                                    }
                                }
                            }
                        }
                    }
                    else if (tupo.name.Contains("修課紀錄"))
                    {
                        var mr = JsonSerializer.Deserialize<修課紀錄外層>(jsonString);
                        if (mr != null)
                        {
                            foreach (var std in mr.修課紀錄)
                            {
                                foreach (var prop in std.GetType().GetProperties().Where(p => !passProp.Contains(p.Name)).ToList())
                                {
                                    List<string> list = prop.GetValue(std) as List<string>;
                                    if (list == null) { continue; }
                                    foreach (var val in list)
                                    {
                                        L01_centraldb_learning_history_temp model = new()
                                        {
                                            year_id = year,
                                            sms_id = sms,
                                            sch_no = _schno,
                                            kind = name,
                                            cls = prop.Name,
                                            idno = std.身分證號,
                                            ser_id = ++serId,
                                            birthday = std.出生日期,
                                            json_head = mr.名冊資訊,
                                            json_content = val,
                                            upd_name = "管理者",
                                            zip_name = file.FileName
                                        };
                                        insertModel.Add(model);
                                    }
                                }
                            }
                        }
                    }
                    else if (tupo.name.Contains("校內幹部經歷"))
                    {
                        var mr = JsonSerializer.Deserialize<校內幹部經歷外層>(jsonString);
                        if (mr != null)
                        {
                            foreach (var std in mr.校內幹部經歷)
                            {
                                foreach (var prop in std.GetType().GetProperties().Where(p => !passProp.Contains(p.Name)).ToList())
                                {
                                    List<string> list = prop.GetValue(std) as List<string>;
                                    if (list == null) { continue; }
                                    foreach (var val in list)
                                    {
                                        L01_centraldb_learning_history_temp model = new()
                                        {
                                            year_id = year,
                                            sms_id = sms,
                                            sch_no = _schno,
                                            kind = name,
                                            cls = prop.Name,
                                            idno = std.身分證號,
                                            ser_id = ++serId,
                                            birthday = std.出生日期,
                                            json_head = mr.名冊資訊,
                                            json_content = val,
                                            upd_name = "管理者",
                                            zip_name = file.FileName
                                        };
                                        insertModel.Add(model);
                                    }
                                }
                            }
                        }
                    }
                    else if (tupo.name.Contains("課程學習成果"))
                    {
                        var mr = JsonSerializer.Deserialize<課程學習成果外層>(jsonString);
                        if (mr != null)
                        {
                            foreach (var std in mr.課程學習成果)
                            {
                                foreach (var prop in std.GetType().GetProperties().Where(p => !passProp.Contains(p.Name)).ToList())
                                {
                                    List<string> list = prop.GetValue(std) as List<string>;
                                    if (list == null) { continue; }
                                    foreach (var val in list)
                                    {
                                        L01_centraldb_learning_history_temp model = new()
                                        {
                                            year_id = year,
                                            sms_id = sms,
                                            sch_no = _schno,
                                            kind = name,
                                            cls = prop.Name,
                                            idno = std.身分證號,
                                            ser_id = ++serId,
                                            birthday = std.出生日期,
                                            json_head = mr.名冊資訊,
                                            json_content = val,
                                            upd_name = "管理者",
                                            zip_name = file.FileName
                                        };
                                        insertModel.Add(model);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }

                    if (insertModel.Count > 0)
                    {
                        allModel.Add(insertModel);
                    }
                });

                if (allModel.Count > 0)
                {
                    data = await _service.InsertJson(allModel);
                }
                return Json(new { dataset = "", message = (data > 0 ? "匯入成功" : "匯入失敗"), status = (data > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {
                return Json(new { dataset = "", message = e.Message, status = (data > 0 ? "Y" : "N") });
            }
        }


        //開放時間設定 - 開放清單
        [HttpGet("GetDateTimeSetup")]
        public async Task<IActionResult> DateTimeSetup(short year, short sms)
        {
            IEnumerable<Datetime_setupDTO> list = await _service.GenDateTimeSetupList(year, sms);

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }
        //開放時間設定 - 設定
        [HttpPost("GetDateTimeSetup")]
        public async Task<IActionResult> DateTimeSetup(List<Datetime_setupDTO> arg)
        {
            try
            {
                var check_data = arg.Where(e => e.check == 1);
                var model = _mapper.Map<List<L01_centraldb_learning_history_datetime_setup>>(check_data);

                var setup = await _service.UpsetDateTimeSetup(model);
                return Json(new { dataset = "", message = (setup > 0 ? "設定成功" : "設定失敗"), status = (setup > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {

                return Json(new { dataset = "", message = e.Message, status = "N" });
            }
        }


        //學生問題回報查看 - 清單
        [HttpGet("FeedBack")]
        public async Task<IActionResult> GetFeedBackList([FromQuery] FeedBackListParameter arg)
        {
            IEnumerable<FeedBackListDTO> list = await _service.GenFeedBackList(arg);

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }
        //學生問題回報查看 - 答覆
        [HttpPut("FeedBack")]
        public async Task<IActionResult> FeedBackUpdataAnwser(FeedBackUpdataAnswerParameter arg)
        {
            try
            {
                int count = await _service.UpDataFeedBackAnswer(arg);
                return Json(new { dataset = "", message = (count > 0 ? "成功" : "失敗"), status = (count > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {

                return Json(new { dataset = "", message = e.Message, status = "N" });
            }
        }


        //資料匯總查詢 - 班級下拉清單
        [HttpGet("GetCls")]
        public async Task<IActionResult> GetCls(short year, short sms)
        {
            var list = await _service.GenClsList(year, sms);
            return Json(new { dataset = list, status = (list != null ? "Y" : "N") });
        }
        //資料匯總查詢 - 班級
        [HttpGet("QueryCount")]
        public async Task<IActionResult> GetQueryCount(short year, short sms, string cls)
        {
            object list = await _service.GenQueryCount(year, sms, cls);

            return Json(new { dataset = list, status = (list != null ? "Y" : "N") });
        }
        #endregion  承辦人員

        #region 學生
        [HttpGet("GetYmsStd")]
        public async Task<IActionResult> GetYmsStd(string arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg);
            arg = token_data.result.user_id;
            IEnumerable<string> list = await _service.GenYmsListStd(arg);

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }

        //學生匯入資料查看 - 確認資料無誤
        [HttpPut("PutStdCheckData")]
        public async Task<IActionResult> PutStdCheckData(StdCheckDataParameter arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std = token_data.result.user_id;

            try
            {
                int count = await _service.UpDataCheckFlag(arg);
                return Json(new { dataset = "", message = (count > 0 ? "確認成功" : "確認失敗"), status = (count > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {

                return Json(new { dataset = "", message = e.Message, status = "N" });
            }
        }

        //問題回報 - 清單
        [HttpGet("StdFeedBack")]
        public async Task<IActionResult> GetStdFeedBackList([FromQuery] StdFeedBackListParameter arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std = token_data.result.user_id;

            IEnumerable<StdFeedBackListDTO> list = await _service.GenStdFeedBackList(arg);

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }
        //問題回報 - 新增
        [HttpPost("StdFeedBack")]
        public async Task<IActionResult> PostStdFeedBackList(L01_centraldb_learning_history_stdfeedback arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;

            try
            {
                int count = await _service.InsertStdFeedBack(arg);
                return Json(new { dataset = "", message = (count == -1 ? "超過時間！" : count > 0 ? "新增成功" : "新增失敗"), status = (count > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {

                return Json(new { dataset = "", message = e.Message, status = "N" });
            }
        }
        //問題回報 - 更新
        [HttpPut("StdFeedBack")]
        public async Task<IActionResult> PutStdFeedBackList(L01_centraldb_learning_history_stdfeedback arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;

            try
            {
                int count = await _service.UpdateStdFeedBack(arg);
                return Json(new { dataset = "", message = (count == -1 ? "超過時間！" : count > 0 ? "更新成功" : "更新失敗"), status = (count > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {

                return Json(new { dataset = "", message = e.Message, status = "N" });
            }
        }
        //問題回報 - 刪除
        [HttpDelete("StdFeedBack")]
        public async Task<IActionResult> DelStdFeedBackList(StdFeedBackListDel arg)
        {
            TokenResult? token_data = _GetUserProfilesService.GetUserData(arg.token);
            arg.std_no = token_data.result.user_id;
            try
            {
                int count = await _service.DelStdFeedBack(arg);
                return Json(new { dataset = "", message = (count == -1 ? "超過時間！" : count > 0 ? "刪除成功" : "刪除失敗"), status = (count > 0 ? "Y" : "N") });
            }
            catch (Exception e)
            {

                return Json(new { dataset = "", message = e.Message, status = "N" });
            }
        }
        //問題回報 - 開放項目
        [HttpGet("FeedBackOpenKind/{year}/{sms}")]
        public async Task<IActionResult> GetFeedBackOpenKind(short year, short sms)
        {
            IEnumerable<string> list = await _service.GenFeedBackOpenKindList(year, sms);

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }
        //問題回報 - 錯誤別
        [HttpGet("FeedBackErrorCode")]
        public async Task<IActionResult> FeedBackErrorCode()
        {
            IEnumerable<object> list = await _service.GenFeedBackErrorCodeList();

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }
        #endregion 學生

        #region 共用
        //問題回報 - 項目類別下拉
        [HttpGet("FeedBackKindCls/{year}/{sms}")]
        public async Task<IActionResult> GetFeedBackKindCls(short year, short sms)
        {
            IEnumerable<GenFeedBackKindClsList> list = await _service.GenFeedBackKindClsList(year, sms);

            return Json(new { dataset = list, status = (list.Count() <= 0 ? "N" : "Y") });
        }
        //查看學生匯入資料
        [HttpPost("GetClsStd")]
        public async Task<IActionResult> PostClsStd([FromForm] short year, [FromForm] short sms, [FromForm] string std)
        {
            var list = await _service.GenStdList(year, sms, std);
            return Json(new { dataset = list, status = (list.Count > 0 ? "Y" : "N") });
        }
        #endregion 共用
    }
}
