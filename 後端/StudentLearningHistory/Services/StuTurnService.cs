using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuFileInfo.Parameters;
using StudentLearningHistory.Models.StuAttestation.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuCollege.Parameters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Irony.Parsing;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;
using DocumentFormat.OpenXml.Math;
using static StudentLearningHistory.Services.StuTurnService;
using Ionic.Zip;
using static StudentLearningHistory.Services.StuFileInfoService;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Helpers;
using StudentLearningHistory.Models.StuTurn.Parameter;
using SharpCompress;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using StudentLearningHistory.Models.System.DbModels;
using System.Runtime.Intrinsics.X86;
using System.Linq;
using System.IO;
using Azure.Core;
using SharpCompress.Common;
using System.Security.Cryptography;
using StudentLearningHistory.Models.StuPublickFileHub.DTO;

namespace StudentLearningHistory.Services
{
    public class StuTurnService
    {
        private readonly MD5Helper _md5;
        private readonly FileHelper _file;
        private readonly ZipHelper _zip;
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly StuFileInfoService _service;
        private readonly string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        public string _schno;
        public int _file1 = 0;
        public int _file2 = 0;
        public string DiverseKey = string.Empty;
        public string StudyKey = string.Empty;
        public Dictionary<string, string> dictCoures = new Dictionary<string, string>();
        public Dictionary<string, string> dictDiverse = new Dictionary<string, string>();
        public JObject StudyJson = new JObject();
        public JObject DiverseJson = new JObject();
        List<DiverseDict> diversedict = new List<DiverseDict>();
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";
        private string getDate = DateTime.Now.ToString("yyyy")
                                      + '_' + DateTime.Now.ToString("MM")
                                      + '_' + DateTime.Now.ToString("dd")
                                      + '_' + DateTime.Now.ToString("HH")
                                      + '_' + DateTime.Now.ToString("mm")
                                      + '_' + DateTime.Now.ToString("ss");
        public class Title
        {
            [JsonPropertyName("基本資料")] public Basic Basic { get; set; }
            [JsonPropertyName("課程學習成果紀錄")] public List<Course> Study { get; set; }
            [JsonPropertyName("多元表現")] public Diverse Diverse { get; set; }
        }
        //基本資料
        public class Basic
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生中文姓名")] public string std_name { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("擔任學校幹部經歷紀錄")] public List<Cadre>? cadre { get; set; }
        }
        //課程學習
        public class Course
        {
            [JsonPropertyName("修課學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("修課單位代碼")] public string unit { get; set; }
            [JsonPropertyName("應修課學年度")] public int year_id { get; set; }
            [JsonPropertyName("應修課學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("課程代碼")] public string course_code { get; set; }
            [JsonPropertyName("科目名稱")] public string sub_name { get; set; }
            [JsonPropertyName("學分數")] public decimal credit { get; set; }
            [JsonPropertyName("開課年級")] public int grd_id { get; set; }
            [JsonPropertyName("開課學期")] public int in_sms_id { get; set; }
            [JsonPropertyName("是否為借讀生時的課程學習成果")] public bool borrow_yn { get; set; }
            [JsonPropertyName("是否為重修時的課程學習成果")] public bool reread_yn { get; set; }
            [JsonPropertyName("重修學年度及學期")] public string reread_yms { get; set; }
            [JsonPropertyName("重修方式")] public string reread_type { get; set; }
            [JsonPropertyName("是否為補修時的課程學習成果")] public bool repair_yn { get; set; }
            [JsonPropertyName("補修學年度及學期")] public string repair_yms { get; set; }
            [JsonPropertyName("補修方式")] public string repair_type { get; set; }
            [JsonPropertyName("是否為重讀時的課程學習成果")] public bool reread2_yn { get; set; }
            [JsonPropertyName("重讀學年度及學期")] public string reread2_yms { get; set; }
            [JsonPropertyName("是否為轉學轉科前的課程學習成果")] public bool turn_yn { get; set; }
            [JsonPropertyName("課程學習成果文件檔案連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("文件檔案MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("課程學習成果影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("課程學習成果簡述")] public string? content { get; set; }
            [JsonPropertyName("認證狀態")] public string status { get; set; }
            [JsonPropertyName("本課程學習成果上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期及時間")] public string cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [JsonPropertyName("認證時間")] public string attestation_date { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [JsonPropertyName("修課學校代碼")] public string org_schno { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [JsonPropertyName("認證教師")] public string all_empname { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [JsonPropertyName("認證說明")] public string attestation_content { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [JsonPropertyName("送審時間")] public string attestation_send { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public string? filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        //多元表現
        public class Diverse
        {
            [JsonPropertyName("幹部經歷暨事蹟紀錄")] public List<Position> Position { get; set; }
            [JsonPropertyName("競賽參與紀錄")] public List<Competition> Competition { get; set; }
            [JsonPropertyName("服務學習紀錄")] public List<Volunteer> Volunteer { get; set; }
            [JsonPropertyName("團體活動時間紀錄")] public List<Group> Group { get; set; }
            [JsonPropertyName("其他多元表現")] public List<Other> Other { get; set; }
            [JsonPropertyName("檢定證照紀錄")] public List<License> License { get; set; }
            [JsonPropertyName("彈性學習時間紀錄")] public List<Study> Study { get; set; }
            [JsonPropertyName("職場學習紀錄")] public List<Workplace> Workplace { get; set; }
            [JsonPropertyName("作品成果紀錄")] public List<Result> Result { get; set; }
            [JsonPropertyName("大學及技專校院先修課程紀錄")] public List<College> College { get; set; }
        }
        //擔任學校幹部經歷紀錄
        public class Cadre
        {
            [JsonPropertyName("擔任學校幹部期間就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("擔任學校幹部期間就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("單位名稱")] public string unit_name { get; set; }
            [JsonPropertyName("開始日期")] public string sdate { get; set; }
            [JsonPropertyName("結束日期")] public string edate { get; set; }
            [JsonPropertyName("擔任職務名稱")] public string position_name { get; set; }
            [JsonPropertyName("幹部等級代碼")] public string position_id { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
        }
        public class Position
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("單位名稱")] public string? unit_name { get; set; }
            [JsonPropertyName("開始日期")] public string? sdate { get; set; }
            [JsonPropertyName("結束日期")] public string? edate { get; set; }
            [JsonPropertyName("擔任職務名稱")] public string? position_name { get; set; }
            [JsonPropertyName("幹部等級代碼")] public string? type_id { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
        }
        public class Competition
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("競賽名稱")] public string? competition_name { get; set; }
            [JsonPropertyName("競賽等級代碼")] public string? competition_grade { get; set; }
            [JsonPropertyName("競賽獎項")] public string? competition_result { get; set; }
            [JsonPropertyName("結果公布日期")] public string? competition_date { get; set; }
            [JsonPropertyName("競賽項目組別")] public string? competition_item { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("團體或個人參與代碼")] public string? competition_type { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class License
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("檢定證照代碼")] public string? license_id { get; set; }
            [JsonPropertyName("檢定證照類型代碼")] public string? license_memo { get; set; }
            [JsonPropertyName("檢定證照結果分數")] public decimal? license_grade { get; set; }
            [JsonPropertyName("取得檢定證照日期")] public string? license_date { get; set; }
            [JsonPropertyName("分項結果")] public string? license_result { get; set; }
            [JsonPropertyName("檢定證照字號")] public string? license_doc { get; set; }
            [JsonPropertyName("檢定證照組別")] public string? license_group { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class Volunteer
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("服務名稱")] public string? volunteer_name { get; set; }
            [JsonPropertyName("服務單位名稱")] public string? volunteer_unit { get; set; }
            [JsonPropertyName("開始日期")] public string? startdate { get; set; }
            [JsonPropertyName("結束日期")] public string? enddate { get; set; }
            [JsonPropertyName("服務學習時數")] public decimal? hours { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class Study
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("彈性學習時間類別(種類)代碼")] public string? type_id { get; set; }
            [JsonPropertyName("內容(開設名稱)")] public string? open_name { get; set; }
            [JsonPropertyName("開設單位")] public string? open_unit { get; set; }
            [JsonPropertyName("每週節數")] public decimal? hours { get; set; }
            [JsonPropertyName("開設週數")] public decimal? weeks { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class Workplace
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("職場學習類別代碼")] public string? type_id { get; set; }
            [JsonPropertyName("職場學習單位")] public string? unit_name { get; set; }
            [JsonPropertyName("職場學習職稱")] public string? type_title { get; set; }
            [JsonPropertyName("開始日期")] public string? startdate { get; set; }
            [JsonPropertyName("結束日期")] public string? enddate { get; set; }
            [JsonPropertyName("時數")] public decimal? hours { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class Result
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("名稱")] public string? result_name { get; set; }
            [JsonPropertyName("作品日期")] public string? result_date { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("作品成果連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("作品成果MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class Group
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("團體活動時間類別代碼")] public string? time_id { get; set; }
            [JsonPropertyName("辦理單位")] public string? unit_name { get; set; }
            [JsonPropertyName("團體活動內容名稱")] public string? group_content { get; set; }
            [JsonPropertyName("節數")] public decimal? hours { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class College
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("計畫專案")] public string? project_name { get; set; }
            [JsonPropertyName("開設單位")] public string? unit_name { get; set; }
            [JsonPropertyName("課程名稱")] public string? course_name { get; set; }
            [JsonPropertyName("開始日期")] public string? startdate { get; set; }
            [JsonPropertyName("結束日期")] public string? enddate { get; set; }
            [JsonPropertyName("學分數")] public decimal? credit { get; set; }
            [JsonPropertyName("總時數")] public decimal? hours { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        public class Other
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("國民身分證統一編號(或居留證統一證號)")] public string std_identity { get; set; }
            [JsonPropertyName("學生出生年月日")] public string std_birth_dt { get; set; }
            [JsonPropertyName("名稱")] public string? other_name { get; set; }
            [JsonPropertyName("主辦單位")] public string? unit_name { get; set; }
            [JsonPropertyName("開始日期")] public string? startdate { get; set; }
            [JsonPropertyName("結束日期")] public string? enddate { get; set; }
            [JsonPropertyName("時數")] public decimal? hours { get; set; }
            [JsonPropertyName("內容簡述")] public string? content { get; set; }
            [JsonPropertyName("證明文件連結")] public string? doc1_path { get; set; }
            [JsonPropertyName("證明文件MD5")] public string? doc1_md5 { get; set; }
            [JsonPropertyName("影音檔案連結")] public string? doc2_path { get; set; }
            [JsonPropertyName("影音檔案MD5")] public string? doc2_md5 { get; set; }
            [JsonPropertyName("外部影音連結")] public string? outfile { get; set; }
            [JsonPropertyName("本紀錄上傳學年度及學期")] public string upyms { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("提交紀錄(學年度及學期)")] public string yms { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        //課程諮詢
        public class Consult
        {
            [JsonPropertyName("就讀學校代碼")] public string sch_no { get; set; }
            [JsonPropertyName("就讀單位代碼")] public string unit { get; set; }
            [JsonPropertyName("學年度")] public int year_id { get; set; }
            [JsonPropertyName("學期")] public int sms_id { get; set; }
            [JsonPropertyName("教師帳號")] public string? emp_id { get; set; }
            [JsonPropertyName("諮詢地點")] public string? consult_area { get; set; }
            [JsonPropertyName("諮詢類別")] public string? consult_type { get; set; }
            [JsonPropertyName("諮詢主題")] public string? consult_subject { get; set; }
            [JsonPropertyName("諮詢教師")] public decimal? emp_name { get; set; }
            [JsonPropertyName("諮詢內容")] public string? consult_content { get; set; }
            [JsonPropertyName("諮詢日期")] public string? consult_date { get; set; }
            [JsonPropertyName("諮詢附件")] public string? doc_path { get; set; }
            [JsonPropertyName("建立日期")] public string? cdate { get; set; }
            [JsonPropertyName("諮詢學生")] public List<ConsultStu>? stdlist { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string filename { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string file_extension { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? class_name { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? complex_key { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? number_id { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public string? std_no { get; set; }
            [System.Text.Json.Serialization.JsonIgnore]
            public int? ser_id { get; set; }
        }
        //課程諮詢學生清單
        public class ConsultStu
        {
            public string? doc_path { get; set; }
            public string? std_no { get; set; }
        }
        public class DiverseFileMapping
        {
            public string key { get; set; }
            public string value { get; set; }
            public string std_no { get; set; }
        }
        public class DiverseFile
        {
            public string key { get; set; }
            public byte[] value { get; set; }
        }
        public class DiverseDict
        {
            public string key { get; set; }
            public string value { get; set; }
            public string md5 { get; set; }
            public string class_name { get; set; }
            public int ser_id { get; set; }
            public string std_no { get; set; }
        }
        public class InsertFileInfo
        {
            public string file_name { get; set; }
            public string file_extension { get; set; }
            public byte[] file_blob { get; set; }
            public string content { get; set; }
            public string file_md5 { get; set; }
            public string zipcode { get; set; }
            public string stulist { get; set; }
            public string upd_name { get; set; }
            public string upd_dt { get; set; }
        }

        public StuTurnService(IDapperContext context, IConfiguration configuration, StuFileInfoService service, ZipHelper zip, FileHelper file, MD5Helper md5)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
            _service = service;
            _zip = zip;
            _file = file;
            _md5 = md5;
        }
        public async Task<string> ExportJson(StdList arg)
        {
            List<Title> title = new List<Title>();
            string std_identity = string.Empty;
            string json = string.Empty;
            string str_sql = string.Empty;
            string folderPath = string.Empty;
            string zipcode = string.Empty;
            string stulist = string.Empty;
            int rt = 0;

            //string getDate = DateTime.Now.ToString("yyyy")
            //                            + '_' + DateTime.Now.ToString("MM")
            //                            + '_' + DateTime.Now.ToString("dd")
            //                            + '_' + DateTime.Now.ToString("HH")
            //                            + '_' + DateTime.Now.ToString("mm")
            //                            + '_' + DateTime.Now.ToString("ss");

            using (IDbConnection conn = _context.CreateCommand())
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                foreach (var item in arg.std_no)
                {
                    stulist = string.Join(",", arg.std_no);
                    _file1 = 0;
                    _file2 = 0;
                    StudyJson = new JObject();
                    DiverseJson = new JObject();
                    Basic basic = new Basic();
                    List<Cadre> cadre = new List<Cadre>();

                    str_sql = string.Format(@"select 
                                                                                    sch_no,
                                                                                    unit = '000000000',
                                                                                    year_id = {1},
                                                                                    sms_id = {2},
                                                                                    std_identity,
                                                                                    std_name,
                                                                                    std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2)                                                
                                                                        from s04_student
                                                                        where std_no = '{0}'", item.ToString(), _context.now_year, _context.now_sms);
                    foreach (var itemM in await conn.QueryAsync<Basic>(str_sql))
                    {
                        std_identity = itemM.std_identity;
                        folderPath = string.Format(@"{0}\{1}\{2}\課程學習成果", root, _context.SchNo + '_' + getDate, itemM.std_identity);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        folderPath = string.Format(@"{0}\{1}\{2}\多元表現", root, _context.SchNo + '_' + getDate, itemM.std_identity);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        str_sql = string.Format(@"select 
                                                                                a.sch_no,
                                                                                unit = '000000000',
                                                                                a.year_id,
                                                                                a.sms_id,
                                                                                b.std_identity,
                                                                                std_birth_dt = convert(varchar,convert(integer,left(b.std_birth_dt,3))+1911)+'/'+substring(b.std_birth_dt,4,2)+'/'+right(b.std_birth_dt,2),
                                                                                a.unit_name,
                                                                                sdate = left(a.startdate,4)+'/'+substring(a.startdate,5,2)+'/'+right(a.startdate,2) ,
                                                                                edate = left(a.enddate,4)+'/'+substring(a.enddate,5,2)+'/'+right(a.enddate,2),
                                                                                a.position_name,
                                                                                position_id = '2',
                                                                                cdate= '',
                                                                                yms= CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id)
                                                                        from L01_std_position a,s04_student b
                                                                        where 
                                                                        a.std_no = b.std_no and
                                                                        a.std_no = '{0}' and
                                                                        a.is_sys = 1", item.ToString());

                        foreach (var itemD in await conn.QueryAsync<Cadre>(str_sql))
                        {
                            cadre.Add(new Cadre
                            {
                                sch_no = itemD.sch_no,
                                unit = itemD.unit,
                                year_id = itemD.year_id,
                                sms_id = itemD.sms_id,
                                std_identity = itemD.std_identity,
                                std_birth_dt = itemD.std_birth_dt,
                                unit_name = itemD.unit_name,
                                sdate = itemD.sdate,
                                edate = itemD.edate,
                                position_name = itemD.position_name,
                                position_id = itemD.position_id,
                                cdate = itemD.cdate,
                                yms = itemD.yms,
                            });
                        }

                        basic.sch_no = itemM.sch_no;
                        basic.unit = itemM.unit;
                        basic.year_id = itemM.year_id;
                        basic.sms_id = itemM.sms_id;
                        basic.std_identity = itemM.std_identity;
                        basic.std_name = itemM.std_name;
                        basic.std_birth_dt = itemM.std_birth_dt;
                        basic.cadre = cadre;
                    }
                    title.Add(new Title { Basic = basic, Diverse = await GetDiverse(conn, item.ToString(), std_identity), Study = await GetCourse(conn, item.ToString(), std_identity) });

                    json = System.Text.Json.JsonSerializer.Serialize(title, options);

                    using (StreamWriter sw = new StreamWriter(string.Format(@"{2}\{0}\{1}.json", _context.SchNo + '_' + getDate, _context.SchNo + '_' + getDate, root)))
                    {
                        sw.WriteLine(json);
                    }

                    using (StreamWriter sw = new StreamWriter(string.Format(@"{2}\{0}\{1}\課程學習成果\filename_mapping.json", _context.SchNo + '_' + getDate, std_identity, root)))
                    {
                        sw.WriteLine(StudyJson.ToString());
                    }

                    using (StreamWriter sw = new StreamWriter(string.Format(@"{2}\{0}\{1}\多元表現\filename_mapping.json", _context.SchNo + '_' + getDate, std_identity, root)))
                    {
                        sw.WriteLine(DiverseJson.ToString());
                    }
                }

                str_sql = @"
                                    select value
                                    from L01_setup
                                    where id = 3
                                    ";
                var dr = await conn.ExecuteReaderAsync(str_sql);
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Load(dr);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["value"] == DBNull.Value)
                    {
                        zipcode = "default";
                    }
                    else
                    {
                        zipcode = dt.Rows[0]["value"].ToString();
                    }
                }
                else
                {
                    zipcode = "default";
                }
                var uuid = Guid.NewGuid().ToString();
                string zipPath = string.Format(@"{1}\{0}.zip", _context.SchNo + '_' + getDate, root);
                using (var zip = new ZipFile(Encoding.Default))
                {
                    zip.Password = zipcode;
                    zip.AddDirectory(string.Format(@"{1}\{0}\", _context.SchNo + '_' + getDate, root));
                    zip.Save(zipPath);


                    using MemoryStream memoryStream = new();
                    //using FileStream stream = new FileStream(zipPath, FileMode.Create);
                    using FileStream stream = System.IO.File.OpenRead(zipPath);
                    await stream.CopyToAsync(memoryStream);

                    byte[] file_blob = _zip.ZipData(memoryStream, _context.SchNo + '_' + getDate + ".zip");
                    string file_md5 = _md5.AbstractFile(file_blob);
                    memoryStream.Close();
                    stream.Close();

                    InsertFileInfo insert = new()
                    {
                        file_name = _context.SchNo + '_' + getDate,
                        file_extension = "zip",
                        file_blob = file_blob,
                        content = "",
                        file_md5 = file_md5,
                        zipcode = zipcode,
                        stulist = stulist,
                        upd_name = "系統新增",
                        upd_dt = updteDate()
                    };

                    str_sql = @"
                                    insert into L01_std_export
                                    (file_name,file_extension,file_blob,content,file_md5,zipcode,stulist,upd_name,upd_dt)
                                    values
                                    (@file_name,@file_extension,@file_blob,@content,@file_md5,@zipcode,@stulist,@upd_name,@upd_dt)
                                    ";
                    rt = await conn.ExecuteAsync(str_sql, insert);
                }
                return rt.ToString();
            }
        }
        public async Task<string> ImportJson(Models.StuTurn.Parameter.FileInfo arg)
        {
            List<Basic> basic = new List<Basic>();
            List<Cadre> cadre = new List<Cadre>();
            List<Course> course = new List<Course>();
            List<Position> position = new List<Position>();
            List<Competition> competition = new List<Competition>();
            List<License> license = new List<License>();
            List<Volunteer> volunteer = new List<Volunteer>();
            List<Study> study = new List<Study>();
            List<Result> result = new List<Result>();
            List<Group> group = new List<Group>();
            List<College> college = new List<College>();
            List<Other> other = new List<Other>();
            List<Consult> consult = new List<Consult>();
            List<Workplace> workplace = new List<Workplace>();
            List<DiverseFileMapping> diverseFileMapping = new List<DiverseFileMapping>();
            List<DiverseFile> diverseFile = new List<DiverseFile>();

            string basic_std_id = string.Empty;
            string check_std = string.Empty;
            string str_sql = string.Empty;
            int num = 0;

            foreach (IFormFile file in arg.files)
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                string filename = file.FileName.Substring(0, file.FileName.IndexOf(".zip")) + ".json";

                //解壓縮ZIP，整理成tupo（name， 記憶體串流）
                var dity = _zip.UnzipData(memoryStream.ToArray(), arg.Password);
                //byte[] dity = memoryStream.ToArray();
                memoryStream.Close();
                try
                {
                    foreach (var listitem in dity)
                    {
                        string listitem_name = listitem.name;
                        byte[] listitem_byte = listitem.fileBytes;

                        using MemoryStream SHAmemoryStream = new(listitem_byte);
                        using StreamReader reader = new(SHAmemoryStream);

                        var jsonSerializerOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };

                        var data = reader.ReadToEnd();


                        if (listitem_name.IndexOf(".json") > 0)
                        {
                            if (listitem_name.Contains(filename))
                            {
                                foreach (var parse_result in JToken.Parse(data))
                                {
                                    check_std = "N";
                                    JObject obj = JObject.Parse(parse_result.ToString());

                                    #region 基本資料
                                    string basic_sch_no = (string)obj["基本資料"]["就讀學校代碼"];
                                    string basic_unit = (string)obj["基本資料"]["就讀單位代碼"];
                                    int basic_year_id = (int)obj["基本資料"]["學年度"];
                                    int basic_sms_id = (int)obj["基本資料"]["學期"];
                                    basic_std_id = (string)obj["基本資料"]["國民身分證統一編號(或居留證統一證號)"];
                                    string basic_std_name = (string)obj["基本資料"]["學生中文姓名"];
                                    string basic_std_bd = (string)obj["基本資料"]["學生出生年月日"];
                                    string x_std = string.Empty;

                                    str_sql = string.Format(@"
                                                                            select std_no
                                                                            from s04_student
                                                                            where std_identity = '{0}'
                                                                            ", basic_std_id);
                                    using (IDbConnection conn = _context.CreateCommand())
                                    {
                                        var dr = await conn.ExecuteReaderAsync(str_sql);
                                        System.Data.DataTable dt = new System.Data.DataTable();
                                        dt.Load(dr);

                                        if (dt.Rows.Count > 0)
                                        {
                                            if (dt.Rows[0]["std_no"] == DBNull.Value)
                                            {
                                                check_std = "Y";
                                                break;
                                            }
                                            else
                                            {
                                                x_std = dt.Rows[0]["std_no"].ToString();
                                            }
                                        }
                                        else
                                        {
                                            check_std = "Y";
                                            break;
                                        }

                                    }
                                    JArray categories = (JArray)obj["基本資料"]["擔任學校幹部經歷紀錄"];
                                    foreach (var item_1 in categories)
                                    {
                                        cadre.Add(new Cadre
                                        {
                                            std_no = x_std,
                                            sch_no = _context.SchNo,
                                            unit = (string)item_1["擔任學校幹部期間就讀單位代碼"],
                                            year_id = (int)item_1["學年度"],
                                            sms_id = (int)item_1["學期"],
                                            std_identity = (string)item_1["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = (string)item_1["學生出生年月日"],
                                            unit_name = (string)item_1["單位名稱"],
                                            sdate = (string)item_1["開始日期"],
                                            edate = (string)item_1["結束日期"],
                                            position_name = (string)item_1["擔任職務名稱"],
                                            position_id = (string)item_1["幹部等級代碼"],
                                            cdate = (string)item_1["建立日期"],
                                            yms = (string)item_1["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    basic.Add(new Basic
                                    {
                                        sch_no = _context.SchNo,
                                        unit = basic_unit,
                                        year_id = basic_year_id,
                                        sms_id = basic_sms_id,
                                        std_identity = basic_std_id,
                                        std_name = basic_std_name,
                                        std_birth_dt = basic_std_bd,
                                        cadre = cadre
                                    });
                                    #endregion 基本資料

                                    #region 課程學習成果紀錄
                                    categories = (JArray)obj["課程學習成果紀錄"];
                                    foreach (var item_2 in categories)
                                    {
                                        course.Add(new Course
                                        {
                                            sch_no = _context.SchNo,
                                            std_no = x_std,
                                            unit = (string)item_2["修課單位代碼"],
                                            year_id = (int)item_2["應修課學年度"],
                                            sms_id = (int)item_2["應修課學期"],
                                            std_identity = (string)item_2["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = (string)item_2["學生出生年月日"],
                                            course_code = (string)item_2["課程代碼"],
                                            sub_name = (string)item_2["科目名稱"],
                                            credit = (decimal)item_2["學分數"],
                                            grd_id = (int)item_2["開課年級"],
                                            in_sms_id = (int)item_2["開課學期"],
                                            borrow_yn = (bool)item_2["是否為借讀生時的課程學習成果"],
                                            reread_yn = (bool)item_2["是否為重修時的課程學習成果"],
                                            reread_yms = (string)item_2["重修學年度及學期"],
                                            reread_type = (string)item_2["重修方式"],
                                            repair_yn = (bool)item_2["是否為補修時的課程學習成果"],
                                            repair_yms = (string)item_2["補修學年度及學期"],
                                            repair_type = (string)item_2["補修方式"],
                                            reread2_yn = (bool)item_2["是否為重讀時的課程學習成果"],
                                            reread2_yms = (string)item_2["重讀學年度及學期"],
                                            turn_yn = (bool)item_2["是否為轉學轉科前的課程學習成果"],
                                            doc1_path = (string)item_2["課程學習成果文件檔案連結"],
                                            doc1_md5 = (string)item_2["文件檔案MD5"],
                                            doc2_path = (string)item_2["課程學習成果影音檔案連結"],
                                            doc2_md5 = (string)item_2["影音檔案MD5"],
                                            content = (string)item_2["課程學習成果簡述"],
                                            status = (string)item_2["認證狀態"],
                                            upyms = (string)item_2["本課程學習成果上傳學年度及學期"],
                                            cdate = (string)item_2["建立日期及時間"],
                                            yms = (string)item_2["提交紀錄(學年度及學期)"],
                                            attestation_date = (string)item_2["認證時間"],
                                            org_schno = (string)item_2["修課學校代碼"],
                                            all_empname = (string)item_2["認證教師"],
                                            attestation_content = (string)item_2["認證說明"],
                                            attestation_send = (string)item_2["送審時間"],
                                        });
                                    }
                                    #endregion 課程學習成果紀錄

                                    #region 幹部經歷暨事蹟紀錄
                                    categories = (JArray)obj["多元表現"]["幹部經歷暨事蹟紀錄"];
                                    foreach (var item_3 in categories)
                                    {
                                        position.Add(new Position
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_3["就讀學校代碼"],
                                            unit = (string)item_3["就讀單位代碼"],
                                            year_id = (int)item_3["學年度"],
                                            sms_id = (int)item_3["學期"],
                                            std_identity = (string)item_3["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_3["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_3["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_3["學生出生年月日"].ToString().Substring(5, 2) + (string)item_3["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            unit_name = (string)item_3["單位名稱"],
                                            sdate = (string)item_3["開始日期"],
                                            edate = (string)item_3["結束日期"],
                                            position_name = (string)item_3["擔任職務名稱"],
                                            type_id = (string)item_3["幹部等級代碼"],
                                            content = (string)item_3["內容簡述"],
                                            doc1_path = (string)item_3["證明文件連結"],
                                            doc1_md5 = (string)item_3["證明文件MD5"],
                                            doc2_path = (string)item_3["影音檔案連結"],
                                            doc2_md5 = (string)item_3["影音檔案MD5"],
                                            outfile = (string)item_3["外部影音連結"],
                                            upyms = (string)item_3["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_3["建立日期"],
                                            yms = (string)item_3["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 幹部經歷暨事蹟紀錄

                                    #region 競賽參與紀錄
                                    categories = (JArray)obj["多元表現"]["競賽參與紀錄"];
                                    foreach (var item_4 in categories)
                                    {
                                        competition.Add(new Competition
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_4["就讀學校代碼"],
                                            unit = (string)item_4["就讀單位代碼"],
                                            year_id = (int)item_4["學年度"],
                                            sms_id = (int)item_4["學期"],
                                            std_identity = (string)item_4["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_4["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_4["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_4["學生出生年月日"].ToString().Substring(5, 2) + (string)item_4["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            competition_name = (string)item_4["競賽名稱"],
                                            competition_grade = (string)item_4["競賽等級代碼"],
                                            competition_result = (string)item_4["競賽獎項"],
                                            competition_date = !string.IsNullOrEmpty((string)item_4["結果公布日期"]) ? (string)item_4["結果公布日期"] : "",
                                            competition_item = (string)item_4["競賽項目組別"],
                                            competition_type = (string)item_4["團體或個人參與代碼"],
                                            content = (string)item_4["內容簡述"],
                                            doc1_path = (string)item_4["證明文件連結"],
                                            doc1_md5 = (string)item_4["證明文件MD5"],
                                            doc2_path = (string)item_4["影音檔案連結"],
                                            doc2_md5 = (string)item_4["影音檔案MD5"],
                                            outfile = (string)item_4["外部影音連結"],
                                            upyms = (string)item_4["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_4["建立日期"],
                                            yms = (string)item_4["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 競賽參與紀錄

                                    #region 服務學習紀錄
                                    categories = (JArray)obj["多元表現"]["服務學習紀錄"];
                                    foreach (var item_5 in categories)
                                    {
                                        volunteer.Add(new Volunteer
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_5["就讀學校代碼"],
                                            unit = (string)item_5["就讀單位代碼"],
                                            year_id = (int)item_5["學年度"],
                                            sms_id = (int)item_5["學期"],
                                            std_identity = (string)item_5["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_5["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_5["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_5["學生出生年月日"].ToString().Substring(5, 2) + (string)item_5["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            volunteer_name = (string)item_5["服務名稱"],
                                            volunteer_unit = (string)item_5["服務單位名稱"],
                                            startdate = !string.IsNullOrEmpty((string)item_5["開始日期"]) ? (string)item_5["開始日期"] : "",
                                            enddate = !string.IsNullOrEmpty((string)item_5["結束日期"]) ? (string)item_5["結束日期"] : "",
                                            hours = (decimal)item_5["服務學習時數"],
                                            content = (string)item_5["內容簡述"],
                                            doc1_path = (string)item_5["證明文件連結"],
                                            doc1_md5 = (string)item_5["證明文件MD5"],
                                            doc2_path = (string)item_5["影音檔案連結"],
                                            doc2_md5 = (string)item_5["影音檔案MD5"],
                                            outfile = (string)item_5["外部影音連結"],
                                            upyms = (string)item_5["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_5["建立日期"],
                                            yms = (string)item_5["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 服務學習紀錄

                                    #region 團體活動時間紀錄
                                    categories = (JArray)obj["多元表現"]["團體活動時間紀錄"];
                                    foreach (var item_7 in categories)
                                    {
                                        group.Add(new Group
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_7["就讀學校代碼"],
                                            unit = (string)item_7["就讀單位代碼"],
                                            year_id = (int)item_7["學年度"],
                                            sms_id = (int)item_7["學期"],
                                            std_identity = (string)item_7["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_7["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_7["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_7["學生出生年月日"].ToString().Substring(5, 2) + (string)item_7["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            time_id = (string)item_7["團體活動時間類別代碼"],
                                            unit_name = (string)item_7["辦理單位"],
                                            group_content = (string)item_7["團體活動內容名稱"],
                                            hours = (decimal)item_7["節數"],
                                            content = (string)item_7["內容簡述"],
                                            doc1_path = (string)item_7["證明文件連結"],
                                            doc1_md5 = (string)item_7["證明文件MD5"],
                                            doc2_path = (string)item_7["影音檔案連結"],
                                            doc2_md5 = (string)item_7["影音檔案MD5"],
                                            outfile = (string)item_7["外部影音連結"],
                                            upyms = (string)item_7["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_7["建立日期"],
                                            yms = (string)item_7["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 團體活動時間紀錄

                                    #region 其他多元表現
                                    categories = (JArray)obj["多元表現"]["其他多元表現"];
                                    foreach (var item_6 in categories)
                                    {
                                        other.Add(new Other
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_6["就讀學校代碼"],
                                            unit = (string)item_6["就讀單位代碼"],
                                            year_id = (int)item_6["學年度"],
                                            sms_id = (int)item_6["學期"],
                                            std_identity = (string)item_6["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_6["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_6["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_6["學生出生年月日"].ToString().Substring(5, 2) + (string)item_6["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            other_name = (string)item_6["名稱"],
                                            unit_name = (string)item_6["主辦單位"],
                                            startdate = !string.IsNullOrEmpty((string)item_6["開始日期"]) ? (string)item_6["開始日期"] : "",
                                            enddate = !string.IsNullOrEmpty((string)item_6["結束日期"]) ? (string)item_6["結束日期"] : "",
                                            hours = (decimal)item_6["時數"],
                                            content = (string)item_6["內容簡述"],
                                            doc1_path = (string)item_6["證明文件連結"],
                                            doc1_md5 = (string)item_6["證明文件MD5"],
                                            doc2_path = (string)item_6["影音檔案連結"],
                                            doc2_md5 = (string)item_6["影音檔案MD5"],
                                            outfile = (string)item_6["外部影音連結"],
                                            upyms = (string)item_6["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_6["建立日期"],
                                            yms = (string)item_6["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 其他多元表現

                                    #region 檢定證照紀錄
                                    categories = (JArray)obj["多元表現"]["檢定證照紀錄"];
                                    foreach (var item_8 in categories)
                                    {
                                        license.Add(new License
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_8["就讀學校代碼"],
                                            unit = (string)item_8["就讀單位代碼"],
                                            year_id = (int)item_8["學年度"],
                                            sms_id = (int)item_8["學期"],
                                            std_identity = (string)item_8["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_8["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_8["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_8["學生出生年月日"].ToString().Substring(5, 2) + (string)item_8["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            license_id = (string)item_8["檢定證照代碼"],
                                            license_memo = (string)item_8["檢定證照類型代碼"],
                                            license_grade = (decimal)item_8["檢定證照結果分數"],
                                            license_date = !string.IsNullOrEmpty((string)item_8["取得檢定證照日期"]) ? (string)item_8["取得檢定證照日期"] : "",
                                            license_result = (string)item_8["分項結果"],
                                            license_doc = (string)item_8["檢定證照字號"],
                                            license_group = (string)item_8["檢定證照組別"],
                                            content = (string)item_8["內容簡述"],
                                            doc1_path = (string)item_8["證明文件連結"],
                                            doc1_md5 = (string)item_8["證明文件MD5"],
                                            doc2_path = (string)item_8["影音檔案連結"],
                                            doc2_md5 = (string)item_8["影音檔案MD5"],
                                            outfile = (string)item_8["外部影音連結"],
                                            upyms = (string)item_8["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_8["建立日期"],
                                            yms = (string)item_8["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 檢定證照紀錄

                                    #region 彈性學習時間紀錄
                                    categories = (JArray)obj["多元表現"]["彈性學習時間紀錄"];
                                    foreach (var item_9 in categories)
                                    {
                                        study.Add(new Study
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_9["就讀學校代碼"],
                                            unit = (string)item_9["就讀單位代碼"],
                                            year_id = (int)item_9["學年度"],
                                            sms_id = (int)item_9["學期"],
                                            std_identity = (string)item_9["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_9["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_9["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_9["學生出生年月日"].ToString().Substring(5, 2) + (string)item_9["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            type_id = (string)item_9["彈性學習時間類別(種類)代碼"],
                                            open_name = (string)item_9["內容(開設名稱)"],
                                            open_unit = (string)item_9["開設單位"],
                                            hours = (decimal)item_9["每週節數"],
                                            weeks = (decimal)item_9["開設週數"],
                                            content = (string)item_9["內容簡述"],
                                            doc1_path = (string)item_9["證明文件連結"],
                                            doc1_md5 = (string)item_9["證明文件MD5"],
                                            doc2_path = (string)item_9["影音檔案連結"],
                                            doc2_md5 = (string)item_9["影音檔案MD5"],
                                            outfile = (string)item_9["外部影音連結"],
                                            upyms = (string)item_9["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_9["建立日期"],
                                            yms = (string)item_9["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 彈性學習時間紀錄

                                    #region 職場學習紀錄
                                    categories = (JArray)obj["多元表現"]["職場學習紀錄"];
                                    foreach (var item_10 in categories)
                                    {
                                        workplace.Add(new Workplace
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_10["就讀學校代碼"],
                                            unit = (string)item_10["就讀單位代碼"],
                                            year_id = (int)item_10["學年度"],
                                            sms_id = (int)item_10["學期"],
                                            std_identity = (string)item_10["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_10["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_10["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_10["學生出生年月日"].ToString().Substring(5, 2) + (string)item_10["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            type_id = (string)item_10["職場學習類別代碼"],
                                            unit_name = (string)item_10["職場學習單位"],
                                            startdate = !string.IsNullOrEmpty((string)item_10["開始日期"]) ? (string)item_10["開始日期"] : "",
                                            enddate = !string.IsNullOrEmpty((string)item_10["結束日期"]) ? (string)item_10["結束日期"] : "",
                                            type_title = (string)item_10["職場學習職稱"],
                                            hours = (decimal)item_10["時數"],
                                            content = (string)item_10["內容簡述"],
                                            doc1_path = (string)item_10["證明文件連結"],
                                            doc1_md5 = (string)item_10["證明文件MD5"],
                                            doc2_path = (string)item_10["影音檔案連結"],
                                            doc2_md5 = (string)item_10["影音檔案MD5"],
                                            outfile = (string)item_10["外部影音連結"],
                                            upyms = (string)item_10["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_10["建立日期"],
                                            yms = (string)item_10["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 職場學習紀錄

                                    #region 作品成果紀錄
                                    categories = (JArray)obj["多元表現"]["作品成果紀錄"];
                                    foreach (var item_11 in categories)
                                    {
                                        result.Add(new Result
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_11["就讀學校代碼"],
                                            unit = (string)item_11["就讀單位代碼"],
                                            year_id = (int)item_11["學年度"],
                                            sms_id = (int)item_11["學期"],
                                            std_identity = (string)item_11["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_11["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_11["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_11["學生出生年月日"].ToString().Substring(5, 2) + (string)item_11["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            result_name = (string)item_11["名稱"],
                                            result_date = (string)item_11["作品日期"],
                                            content = (string)item_11["內容簡述"],
                                            doc1_path = (string)item_11["作品成果連結"],
                                            doc1_md5 = (string)item_11["作品成果MD5"],
                                            doc2_path = (string)item_11["影音檔案連結"],
                                            doc2_md5 = (string)item_11["影音檔案MD5"],
                                            outfile = (string)item_11["外部影音連結"],
                                            upyms = (string)item_11["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_11["建立日期"],
                                            yms = (string)item_11["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 作品成果紀錄

                                    #region 大學及技專校院先修課程紀錄
                                    categories = (JArray)obj["多元表現"]["大學及技專校院先修課程紀錄"];
                                    foreach (var item_12 in categories)
                                    {
                                        college.Add(new College
                                        {
                                            std_no = x_std,
                                            sch_no = (string)item_12["就讀學校代碼"],
                                            unit = (string)item_12["就讀單位代碼"],
                                            year_id = (int)item_12["學年度"],
                                            sms_id = (int)item_12["學期"],
                                            std_identity = (string)item_12["國民身分證統一編號(或居留證統一證號)"],
                                            std_birth_dt = !string.IsNullOrEmpty((string)item_12["學生出生年月日"]) ? Convert.ToString(Convert.ToInt32((string)item_12["學生出生年月日"].ToString().Substring(0, 4)) - 1911) + (string)item_12["學生出生年月日"].ToString().Substring(5, 2) + (string)item_12["學生出生年月日"].ToString().Substring(8, 2) : "",
                                            project_name = (string)item_12["計畫專案"],
                                            unit_name = (string)item_12["開設單位"],
                                            course_name = (string)item_12["課程名稱"],
                                            startdate = !string.IsNullOrEmpty((string)item_12["開始日期"]) ? (string)item_12["開始日期"] : "",
                                            enddate = !string.IsNullOrEmpty((string)item_12["結束日期"]) ? (string)item_12["結束日期"] : "",
                                            credit = (decimal)item_12["學分數"],
                                            hours = (decimal)item_12["總時數"],
                                            content = (string)item_12["內容簡述"],
                                            doc1_path = (string)item_12["證明文件連結"],
                                            doc1_md5 = (string)item_12["證明文件MD5"],
                                            doc2_path = (string)item_12["影音檔案連結"],
                                            doc2_md5 = (string)item_12["影音檔案MD5"],
                                            outfile = (string)item_12["外部影音連結"],
                                            upyms = (string)item_12["本紀錄上傳學年度及學期"],
                                            cdate = (string)item_12["建立日期"],
                                            yms = (string)item_12["提交紀錄(學年度及學期)"],
                                        });
                                    }
                                    #endregion 大學及技專校院先修課程紀錄
                                }
                            }
                            else if (listitem_name.Contains("課程諮詢.json"))
                            {
                                foreach (var parse_result in JToken.Parse(data))
                                {
                                    JObject obj = JObject.Parse(parse_result.ToString());
                                }
                            }
                            else
                            {
                                string[] sArray = listitem_name.Split('/');
                                var parse_result = JToken.Parse(data);
                                foreach (var item in parse_result)
                                {
                                    diverseFileMapping.Add(new DiverseFileMapping
                                    {
                                        key = item.ToObject<JProperty>().Name,
                                        value = item.ToObject<JProperty>().Value.ToString(),
                                        std_no = sArray[1].ToString()
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (listitem_byte.Length > 0)
                            {
                                diverseFile.Add(new DiverseFile { key = listitem_name, value = listitem_byte });
                            }
                        }

                        SHAmemoryStream.Close();
                        reader.Close();

                        if (check_std == "Y")
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "匯入失敗(json)" + ex.Message;
                }

                if (check_std == "Y")
                {
                    break;
                }
            }

            if (check_std == "Y")
            {
                return string.Format("匯入失敗!!身份字號{0}，校務系統查無建檔資料，請確認。", basic_std_id);
            }

            using (IDbConnection conn = _context.CreateCommand())
            {
                conn.Open();
                using (IDbTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        #region 課程學習成果紀錄
                        if (course.Count() > 0) 
                        {
                            str_sql = @"
                                            delete from  L01_std_attestation where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);

                            string temp = string.Empty;
                            int no = 1;
                            course.OrderBy(x => x.std_no).
                                        ThenBy(x => x.year_id).
                                        ThenBy(x => x.sms_id).
                                        ThenBy(x => x.course_code).
                                        ForEach(x =>
                                        {
                                            if (Convert.ToString(x.year_id) + "-" + Convert.ToString(x.sms_id) + "-" +x.std_no+"-" +x.course_code != temp)
                                            {
                                                no = 1;
                                                x.ser_id = 1;
                                            }
                                            else 
                                            {
                                                x.ser_id = no;
                                            }
                                            no++;
                                            temp = Convert.ToString(x.year_id) + "-" + Convert.ToString(x.sms_id) + "-" + x.std_no + "-" + x.course_code;
                                        });


                            str_sql = @"
										insert into L01_std_attestation
										(	sch_no,year_id,sms_id,cls_id,sub_id,src_dup,emp_id,std_no,ser_id,
											attestation_send,attestation_date,attestation_status,attestation_centraldb,
											content,is_sys,
											credit,grd_id,in_sms_id,borrow_yn,
											reread_yn,reread_yms,reread_type,repair_yn,repair_yms,repair_type,
											reread2_yn,reread2_yms,turn_yn,upyms,create_dt,yms,actually_year,actually_sms,sub_name,all_empname,stu_status,cls_name,is_import,attestation_content,org_schno)
                                         VALUES
                                          (@sch_no,@year_id,@sms_id,@cls_id,@sub_id,@src_dup,@emp_id,@std_no,@ser_id,
											@attestation_send,@attestation_date,@attestation_status,@attestation_centraldb,
											@content,@is_sys,
											@credit,@grd_id,@in_sms_id,@borrow_yn,
											@reread_yn,@reread_yms,@reread_type,@repair_yn,@repair_yms,@repair_type,
											@reread2_yn,@reread2_yms,@turn_yn,@upyms,@create_dt,@yms,@actually_year,@actually_sms,@sub_name,@all_empname,@stu_status,@cls_name,@is_import,@attestation_content,@org_schno)
                                        ";

                            await conn.ExecuteAsync(str_sql, course.Select(x => new
                            {
                                sch_no = x.sch_no,
                                year_id = x.year_id,
                                sms_id = x.sms_id,
                                cls_id = "00000000",
                                sub_id = "xxxx",
                                src_dup = x.course_code,
                                emp_id = x.all_empname,
                                std_no = x.std_no,
                                ser_id = x.ser_id,
                                attestation_send = Convert.ToString(Convert.ToInt64(x.attestation_send.Replace(@"-", "").Replace(@" ", "").Replace(@":", "").Substring(0, 4)) - 1911) + x.attestation_send.Replace(@"-", "").Replace(@" ", "").Replace(@":", "").Substring(4, 10),
                                attestation_date = Convert.ToString(Convert.ToInt64(x.attestation_date.Replace(@"-", "").Replace(@" ", "").Replace(@":", "").Substring(0, 4)) - 1911) + x.attestation_date.Replace(@"-", "").Replace(@" ", "").Replace(@":", "").Substring(4, 10),
                                attestation_status = "",
                                attestation_centraldb = "N",
                                content = x.content,
                                is_sys = "2",
                                credit = x.credit,
                                grd_id = x.grd_id,
                                in_sms_id = x.in_sms_id,
                                borrow_yn = x.borrow_yn ? "Y" : "N",
                                reread_yn = x.reread_yn ? "Y" : "N",
                                reread_yms = x.reread_yms,
                                reread_type = x.reread_type,
                                repair_yn = x.repair_yn ? "Y" : "N",
                                repair_yms = x.repair_yms,
                                repair_type = x.repair_type,
                                reread2_yn = x.reread2_yn ? "Y" : "N",
                                reread2_yms = x.reread2_yms,
                                turn_yn = x.turn_yn ? "Y" : "N",
                                upyms = x.upyms,
                                create_dt = Convert.ToString(Convert.ToInt64(x.cdate.Replace(@"-", "").Replace(@" ", "").Replace(@":", "").Substring(0, 4)) - 1911) + x.cdate.Replace(@"-", "").Replace(@" ", "").Replace(@":", "").Substring(4, 10),
                                yms = x.yms,
                                actually_year = x.reread_yn ? Convert.ToInt64(x.reread_yms.Substring(0, 3)) : x.repair_yn ? Convert.ToInt64(x.repair_yms.Substring(0, 3)) : 0,
                                actually_sms = x.reread_yn ? Convert.ToInt64(x.reread_yms.Substring(4, 1)) : x.repair_yn ? Convert.ToInt64(x.repair_yms.Substring(4, 1)) : 0,
                                sub_name = x.sub_name,
                                all_empname = x.all_empname,
                                stu_status = "",
                                cls_name = "",
                                is_import = "Y",
                                upd_name = "資料匯入",
                                upd_dt = updteDate(),
                                org_schno = x.org_schno,
                                attestation_content = x.attestation_content
                            }), transaction: tran);
                        }

                        foreach (var x in course)
                        {
                            DictDiverse(x.sch_no + '_' +
                                                Convert.ToString(x.year_id) + '_' +
                                                Convert.ToString(x.sms_id) + '_' +
                                                "00000000_xxxx_"+x.course_code+'_'+
                                                x.all_empname + "_" + 
                                                x.std_no + "_0", x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuAttestation",Convert.ToInt32(x.ser_id), x.std_no);
                        }
                        #endregion

                        #region 校務幹部經歷暨事蹟紀錄
                        if (cadre.Count() > 0) 
                        {
                            str_sql = @"
                                            delete from  L01_std_position where isnull(is_import,'N') = 'Y'  and isnull(is_sys,'') = '1'
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);

                            str_sql = @"
                                                insert into L01_std_position
                                                ( sch_no,std_no,year_id,sms_id,unit_name,startdate,enddate,position_name,type_id,is_sys,upd_name,upd_dt,check_centraldb,is_import)
                                                values
                                                ( @sch_no,@std_no,@year_id,@sms_id,@unit_name,@startdate,@enddate,@position_name,@type_id,@is_sys,@upd_name,@upd_dt,@check_centraldb,@is_import)
                                                ";

                            await conn.ExecuteAsync(str_sql, cadre.Select(x => new
                            {
                                sch_no = x.sch_no,
                                std_no = x.std_no,
                                year_id = x.year_id,
                                sms_id = x.sms_id,
                                unit_name = x.unit_name,
                                startdate = x.sdate.Replace(@"/",""),
                                enddate = x.edate.Replace(@"/", ""),
                                position_name = x.position_name,
                                type_id = x.position_id,
                                is_sys = "1",
                                upd_name = "資料匯入",
                                upd_dt = updteDate(),
                                check_centraldb = "N",
                                is_import = "Y"
                            }), transaction: tran);
                        }
                        #endregion

                        #region 多元學習幹部經歷暨事蹟紀錄
                        if (position.Count() > 0)
                        {
                            str_sql = @"
                                                delete from  L01_std_position where isnull(is_import,'N') = 'Y'   and isnull(is_sys,'') = '2'
                                            ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);

                            position.ForEach(x => x.sch_no = _context.SchNo);

                            str_sql = @"
                                            INSERT INTO L01_std_position
                                            ( [sch_no], [std_no], [year_id], [sms_id], [unit_name], [position_name], [startdate], [enddate], [type_id], [is_sys], [upd_name], [upd_dt],is_import)
                                            VALUES
                                            (@sch_no, @std_no, @year_id, @sms_id, @unit_name, @position_name, @startdate, @enddate, @type_id, @is_sys, @upd_name, @upd_dt,@is_import)
                                        ";

                            await conn.ExecuteAsync(str_sql, position.Select(x => new
                            {
                                sch_no = x.sch_no,
                                std_no = x.std_no,
                                year_id = x.year_id,
                                sms_id = x.sms_id,
                                unit_name = x.unit_name,
                                position_name = x.position_name,
                                startdate = x.sdate.Replace(@"/", ""),
                                enddate = x.edate.Replace(@"/", ""),
                                type_id = x.type_id,
                                is_sys = "2",
                                upd_name = "資料匯入",
                                upd_dt = updteDate(),
                                is_import = "Y"
                            }), transaction: tran);

                            foreach (var x in position)
                            {
                                DictDiverse(x.sch_no + '_' +
                                                        Convert.ToString(x.year_id) + '_' +
                                                        Convert.ToString(x.sms_id) + '_' +
                                                        x.std_no + '_' +
                                                        x.unit_name + '_' +
                                                        x.position_name, x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StudCadre",0, x.std_no);
                            }
                        }
                        #endregion 幹部經歷暨事蹟紀錄             

                        #region 競賽參與紀錄
                        if (competition.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_competition where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            competition.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = competition.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                    from L01_stu_competition 
                                                    where year_id = @year_id
                                                    and sms_id = @sms_id
                                                    and std_no = @std_no
                                                    and sch_no = @sch_no
                                                   ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, competition.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);

                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                competition.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);

                                str_sql = @"
                                                    insert L01_stu_competition
                                                    (sch_no,year_id,sms_id,ser_id,std_no,competition_name,competition_item,competition_grade,
                                                    competition_result,competition_date,competition_type,content,is_sys,upd_name,upd_dt,is_import)
                                                    values
                                                    (@sch_no,@year_id,@sms_id,@ser_id,@std_no,@competition_name,@competition_item,@competition_grade,
                                                    @competition_result,@competition_date,@competition_type,@content,@is_sys,@upd_name,@upd_dt,@is_import)
                                            ";

                                await conn.ExecuteAsync(str_sql, competition.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    competition_name = x.competition_name,
                                    competition_grade = x.competition_grade,
                                    competition_result = x.competition_result,
                                    competition_date = x.competition_date,
                                    competition_item = x.competition_item,
                                    competition_type = x.competition_type,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }


                            foreach (var x in competition)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuCompetition", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 競賽參與紀錄       

                        #region 服務學習紀錄
                        if (volunteer.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_volunteer where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            volunteer.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = volunteer.Select(x => x.std_no).Distinct().ToArray();
                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_volunteer 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, volunteer.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);

                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }

                                num++;
                                volunteer.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                insert L01_stu_volunteer
                                                (sch_no,year_id,sms_id,ser_id,std_no,
                                                volunteer_name,
                                                volunteer_unit,
                                                startdate,
                                                enddate,
                                                hours,
                                                content,is_sys,upd_name,upd_dt)
                                                values
                                                (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                                                 @volunteer_name,
                                                 @volunteer_unit,
                                                 @startdate,
                                                 @enddate,
                                                 @hours,
                                                 @content,@is_sys,@upd_name,@upd_dt)
                                        ";

                                await conn.ExecuteAsync(str_sql, volunteer.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    volunteer_name = x.volunteer_name,
                                    volunteer_unit = x.volunteer_unit,
                                    startdate = x.startdate,
                                    enddate = x.enddate,
                                    hours = x.hours,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate()
                                }), transaction: tran);
                            }


                            foreach (var x in volunteer)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuVolunteer", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 服務學習紀錄       

                        #region 團體活動時間紀錄
                        if (group.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_group where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            group.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = group.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_group 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, group.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);
                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                group.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                insert L01_stu_group
                                                (sch_no,year_id,sms_id,ser_id,std_no,time_id,unit_name,group_content,hours,content,is_sys,upd_name,upd_dt,is_import)
                                                values
                                                (@sch_no,@year_id,@sms_id,@ser_id,@std_no,@time_id,@unit_name,@group_content,@hours,@content,@is_sys,@upd_name,@upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, group.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    time_id = x.time_id,
                                    unit_name = x.unit_name,
                                    group_content = x.group_content,
                                    hours = x.hours,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in group)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuGroup", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 團體活動時間紀錄       

                        #region 其他多元表現
                        if (other.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_other where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            other.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = other.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_other 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, other.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);
                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                other.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                    insert L01_stu_other
                                                    (sch_no,year_id,sms_id,ser_id,std_no,
                                                    other_name,
                                                    unit_name,
                                                    startdate,
                                                    enddate,
                                                    hours,
                                                    content,is_sys,upd_name,upd_dt,is_import)
                                                    values
                                                    (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                                                     @other_name,
                                                     @unit_name,
                                                     @startdate,
                                                     @enddate,
                                                     @hours,
                                                     @content,@is_sys,@upd_name,@upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, other.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    other_name = x.other_name,
                                    unit_name = x.unit_name,
                                    startdate = x.startdate,
                                    enddate = x.enddate,
                                    hours = x.hours,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in other)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuOther", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 其他多元表現       

                        #region 檢定證照紀錄
                        if (license.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_license where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            license.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = license.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_license 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, license.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);
                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                license.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                    insert L01_stu_license
                                                    (sch_no,year_id,sms_id,ser_id,std_no,
                                                    license_id,
                                                    license_memo,
                                                    license_grade,
                                                    license_result,
                                                    license_date,
                                                    license_doc,
                                                    license_group,
                                                    content,is_sys,upd_name,upd_dt,is_import)
                                                    values
                                                    (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                                                     @license_id,
                                                     @license_memo,
                                                     @license_grade,
                                                     @license_result,
                                                     @license_date,
                                                     @license_doc,
                                                     @license_group,
                                                     @content,@is_sys,@upd_name,@upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, license.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    license_id = x.license_id,
                                    license_memo = x.license_memo,
                                    license_grade = x.license_grade,
                                    license_result = x.license_result,
                                    license_date = x.license_date,
                                    license_doc = x.license_doc,
                                    license_group = x.license_group,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in license)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuLicense", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 檢定證照紀錄       

                        #region 彈性學習時間紀錄
                        if (study.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_study_free where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            study.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = study.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_study_free 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, study.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);
                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                study.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                INSERT INTO L01_stu_study_free
                                                 (sch_no, year_id, sms_id, std_no, ser_id, is_sys, type_id, open_name, open_unit, hours, weeks, content, upd_name, upd_dt,is_import)
                                                VALUES
                                                (@sch_no, @year_id, @sms_id, @std_no, @ser_id, @is_sys, @type_id, @open_name, @open_unit, @hours, @weeks, @content, @upd_name, @upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, study.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    type_id = x.type_id,
                                    open_name = x.open_name,
                                    open_unit = x.open_unit,
                                    hours = x.hours,
                                    weeks = x.weeks,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in study)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuStudyf", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 彈性學習時間紀錄       

                        #region 職場學習紀錄
                        if (workplace.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_workplace where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            workplace.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = workplace.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_workplace 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, workplace.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);
                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                workplace.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                        insert L01_stu_workplace
                                                        (sch_no,year_id,sms_id,ser_id,std_no,
                                                        type_id,
                                                        unit_name,
                                                        type_title,
                                                        startdate,
                                                        enddate,
                                                        hours,
                                                        content,is_sys,upd_name,upd_dt,is_import)
                                                        values
                                                        (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                                                         @type_id,
                                                         @unit_name,
                                                         @type_title,
                                                         @startdate,
                                                         @enddate,
                                                         @hours,
                                                         @content,@is_sys,@upd_name,@upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, workplace.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    type_id = x.type_id,
                                    unit_name = x.unit_name,
                                    type_title = x.type_title,
                                    startdate = x.startdate,
                                    enddate = x.enddate,
                                    hours = x.hours,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in workplace)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuWorkPlace", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 職場學習紀錄       

                        #region 作品成果紀錄
                        if (result.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_result where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            result.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = result.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_result 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, result.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);

                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }
                                num++;
                                result.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                insert L01_stu_result
                                                (sch_no,year_id,sms_id,ser_id,std_no,
                                                result_name,
                                                result_date,
                                                content,is_sys,upd_name,upd_dt,is_import)
                                                values
                                                (@sch_no,@year_id,@sms_id,@ser_id,@std_no,
                                                    @result_name,
                                                    @result_date,
                                                    @content,@is_sys,@upd_name,@upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, result.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    result_name = x.result_name,
                                    result_date = x.result_date,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in result)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuResult", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 作品成果紀錄       

                        #region 大學及技專校院先修課程紀錄
                        if (college.Count() > 0)
                        {
                            str_sql = @"
                                            delete from  L01_stu_college where isnull(is_import,'N') = 'Y' 
                                        ";
                            await conn.ExecuteAsync(str_sql, transaction: tran);
                            college.ForEach(x => x.sch_no = _context.SchNo);
                            var datalist = college.Select(x => x.std_no).Distinct().ToArray();

                            foreach (var std_no in datalist)
                            {
                                str_sql = @"select  x_ser = max(ser_id) 
                                                from L01_stu_college 
                                                where year_id = @year_id
                                                and sms_id = @sms_id
                                                and std_no = @std_no
                                                and sch_no = @sch_no
                                               ";
                                var dr = await conn.ExecuteReaderAsync(str_sql, college.Select(x => new { x.year_id, x.sms_id, std_no, x.sch_no }).First(), transaction: tran);

                                System.Data.DataTable dt = new System.Data.DataTable();
                                dt.Load(dr);

                                num = 0;
                                if (dt.Rows[0]["x_ser"] == DBNull.Value)
                                {

                                }
                                else
                                {
                                    var numData = dt.AsEnumerable().Select(x => x.Field<short>("x_ser"));
                                    foreach (var x in numData)
                                    {
                                        num = x;
                                    }
                                }

                                num++;
                                college.Where(x => x.std_no == std_no).ForEach(x => x.ser_id = num++);
                                str_sql = @"
                                                    insert L01_stu_college
                                                    (sch_no,year_id,sms_id,ser_id,std_no,project_name,unit_name,course_name,startdate,
                                                    enddate,credit,hours,content,is_sys,upd_name,upd_dt,is_import)
                                                    values
                                                    (@sch_no,@year_id,@sms_id,@ser_id,@std_no,@project_name,@unit_name,@course_name,@startdate,
                                                    @enddate,@credit,@hours,@content,@is_sys,@upd_name,@upd_dt,@is_import)
                                        ";

                                await conn.ExecuteAsync(str_sql, college.Where(x => x.std_no == std_no).Select(x => new
                                {
                                    sch_no = x.sch_no,
                                    std_no = x.std_no,
                                    year_id = x.year_id,
                                    sms_id = x.sms_id,
                                    ser_id = x.ser_id,
                                    project_name = x.project_name,
                                    unit_name = x.unit_name,
                                    course_name = x.course_name,
                                    startdate = x.startdate,
                                    enddate = x.enddate,
                                    credit = x.credit,
                                    hours = x.hours,
                                    content = x.content,
                                    is_sys = "2",
                                    upd_name = "資料匯入",
                                    upd_dt = updteDate(),
                                    is_import = "Y"
                                }), transaction: tran);
                            }

                            foreach (var x in college)
                            {
                                DictDiverse(x.sch_no + '_' +
                                            Convert.ToString(x.year_id) + '_' +
                                            Convert.ToString(x.sms_id) + '_' +
                                            x.std_no + '_' +
                                            Convert.ToString(x.ser_id), x.doc1_path, x.doc2_path, x.doc1_md5, x.doc2_md5, "StuCollege", Convert.ToInt32(x.ser_id), x.std_no);
                            }
                        }
                        #endregion 大學及技專校院先修課程紀錄       

                        #region 檔案上傳
                        string getFileName = string.Empty;
                        List<L01_std_public_filehub> files = new List<L01_std_public_filehub>();

                        foreach (var itemM in diversedict)
                        {
                            foreach (var itemMapping in diverseFileMapping)
                            {
                                if (itemM.value.Contains(itemMapping.key) && itemM.std_no.Equals(itemMapping.std_no))
                                {
                                    getFileName = itemMapping.value;
                                    break;
                                }
                            }

                            foreach (var itemD in diverseFile)
                            {
                                if (itemD.key.Contains(itemM.value))
                                {
                                    using MemoryStream FilememoryStream = new(itemD.value);
                                    byte[] file_blob = _zip.ZipData(FilememoryStream, itemM.value);

                                    L01_std_public_filehub insert = new()
                                    {
                                        complex_key = itemM.key,
                                        class_name = itemM.class_name,
                                        file_name = getFileName,
                                        type_id = 0,
                                        file_extension = getFileName.Substring(getFileName.IndexOf(".") + 1, getFileName.Length - (getFileName.IndexOf(".") + 1)),
                                        file_blob = file_blob,
                                        upd_name = "資料匯入",
                                        file_md5 = itemM.md5,
                                        number_id = itemM.ser_id
                                    };
                                    files.Add(insert);
                                    break;
                                }
                            }
                        }

                        if (files.Count > 0)
                        {
                            await InsertFile(files, conn, tran);
                        }
                        #endregion 檔案上傳
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return "匯入失敗(ins)"+ex.Message;
                    }
                    finally
                    {

                    }
                }
            }
            return "匯入成功";
        }
        public async Task<GetFileInfo> ExportFile(GetFileInfo arg)
        {
            string sql_str = @"
                                            select *
                                            from L01_std_export
                                            where id = @id
                                            order by id desc
                                            ";
            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<GetFileInfo>(sql_str, arg);
            }
        }
        public async Task<IEnumerable<GetFileInfo>> ExportFileList()
        {
            string sql_str = @"
                                            select 
                                                        id,
                                                        file_name,
                                                        file_extension,
                                                        content,
                                                        file_md5,
                                                        zipcode,
                                                        stulist,
                                                        upd_name,
                                                        upd_dt
                                            from L01_std_export
                                            order by id desc
                                            ";
            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<GetFileInfo>(sql_str);
            }
        }
        public async Task<List<Course>> GetCourse(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Course> course = new List<Course>();
            string str_sql = string.Format(@"
                                                                            SELECT 
                                                                            s04_stuhcls.sch_no ,
                                                                            unit = '000000000',
                                                                            s04_stuhcls.year_id,
                                                                            s04_stuhcls.sms_id,
                                                                            std_identity = convert(varchar(10),s04_student.std_identity),
                                                                            std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                                            s04_stddbgo.course_code,
                                                                            sub_name = s04_108subject.sub108_name,
                                                                            credit = convert(integer, case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
                                                                            --credit = convert(nvarchar(5), case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
                                                                            s04_noropenc.grd_id,
                                                                            s04_noropenc.in_sms_id,
                                                                            borrow_yn = 'false',
                                                                            reread_yn = 'false',
                                                                            reread_yms = '0000',
                                                                            reread_type = '0',
                                                                            repair_yn = 'false',
                                                                            repair_yms = '0000',
                                                                            repair_type = '0',
                                                                            reread2_yn = 'false',
                                                                            reread2_yms = '0000',
                                                                            turn_yn = 'false',
                                                                            doc1_path = '',
                                                                            doc1_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                                            ELSE L01_std_public_filehub.file_md5
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            doc2_path = '',
                                                                            doc2_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                                            ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            content = '',
                                                                            status = isnull(case a.attestation_centraldb when 'Y' then '1' when 'N' then '2' when 'F' then '0' end,'2'),
                                                                            upyms = CONVERT(varchar,s04_stuhcls.year_id)+CONVERT(varchar,s04_stuhcls.sms_id),
                                                                            cdate = isnull((
                                                                                    case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                                                    convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                                                    substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                                                    substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                                                    right(L01_std_public_filehub.upd_dt,2) end),''),
                                                                            yms = case when isnull(a.attestation_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                                            filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                                            file_extension,
                                                                            L01_std_public_filehub.class_name,
                                                                            L01_std_public_filehub.complex_key,
                                                                            L01_std_public_filehub.number_id
	                                                                            FROM s04_student
	                                                                            inner join s04_stuhcls on
		                                                                            s04_student.std_no = s04_stuhcls.std_no and												
		                                                                            s04_student.std_no = '{0}'
	                                                                            inner join s90_cha_id on
		                                                                            s04_stuhcls.std_status = s90_cha_id.cha_id and
		                                                                            s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
	                                                                            inner join s04_norstusc on
		                                                                            s04_student.std_no = s04_norstusc.std_no and
		                                                                            s04_norstusc.in_year = s04_stuhcls.year_id and
		                                                                            s04_norstusc.in_sms = s04_stuhcls.sms_id and
		                                                                            s04_norstusc.stu_status = 1 --新修
	                                                                            inner join s04_noropenc on
		                                                                            s04_norstusc.noroc_id = s04_noropenc.noroc_id and
		                                                                            isnull(s04_noropenc.course_code,'') <> '' and
		                                                                            isnull(s04_noropenc.is_learn,'N') = 'Y'
	                                                                            inner join s04_subject on
		                                                                            s04_noropenc.sub_id = s04_subject.sub_id
	                                                                            inner join s04_stddbgo on
		                                                                            s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
		                                                                            s04_stuhcls.sms_id = s04_stddbgo.sms_id and
		                                                                            s04_stuhcls.deg_id = s04_stddbgo.deg_id and
		                                                                            s04_stuhcls.dep_id = s04_stddbgo.dep_id and
		                                                                            s04_stuhcls.bra_id = s04_stddbgo.bra_id and
		                                                                            s04_stuhcls.grd_id = s04_stddbgo.grd_id and
		                                                                            s04_stuhcls.cg_code = s04_stddbgo.cg_code and
		                                                                            isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
		                                                                            s04_noropenc.sub_id = s04_stddbgo.sub_id and
		                                                                            ( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
	                                                                            inner join s04_108subject on
		                                                                            s04_stddbgo.course_code = s04_108subject.course_code
	                                                                            left join L01_std_attestation a on
		                                                                            a.year_id = s04_stuhcls.year_id and
		                                                                            a.sms_id = s04_stuhcls.sms_id and
		                                                                            a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
				                                                                            convert(varchar(1),s04_noropenc.dep_id) + 
				                                                                            case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
				                                                                            convert(varchar(1),s04_noropenc.grd_id) + 
				                                                                            case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
		                                                                            a.sub_id = s04_subject.sub_id and
		                                                                            a.src_dup = s04_stddbgo.course_code and
		                                                                            a.emp_id =  s04_noropenc.all_empid and 
		                                                                            isnull(a.attestation_send,'') <>'' and
		                                                                            isnull(a.attestation_status,'') = 'Y'and
		                                                                            isnull(a.attestation_release,'') = 'Y'and
		                                                                            isnull(a.attestation_centraldb, '')  = 'Y' and
		                                                                            a.std_no = s04_student.std_no
	                                                                            left join L01_std_attestation_file b on 
		                                                                            b.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
		                                                                            b.class_name = 'StuAttestation' and
		                                                                            b.type_id = 0 and
		                                                                            b.check_yn = 'Y'
                                                                                left join L01_std_public_filehub on
                                                                                    L01_std_public_filehub.class_name = 'StuAttestation' and
                                                                                    L01_std_public_filehub.complex_key = b.complex_key and
	                                                                                L01_std_public_filehub.class_name = b.class_name and
	                                                                                L01_std_public_filehub.type_id = b.type_id and
	                                                                                L01_std_public_filehub.number_id = b.number_id
	                                                                            left join s90_organization on
		                                                                            s04_noropenc.org_id = s90_organization.org_id
	                                                                            left join s90_branch on
		                                                                            s04_noropenc.bra_id = s90_branch.bra_id
	                                                                            left join s04_hstusubjscoreterm on
		                                                                            s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
		                                                                            s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
		                                                                            s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
		                                                                            s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
		                                                                            s04_hstusubjscoreterm.hstusst_status = 1
	                                                                            left join s04_hstusixscoreterm on
		                                                                            s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
		                                                                            s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
		                                                                            s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
	                                                                            left join s04_stubuterm on
		                                                                            s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
		                                                                            s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
		                                                                            s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
		                                                                            s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
	                                                                            where s04_student.std_hisdatayear >= 108

                                                                            union

                                                                            select 
                                                                            s04_stuhcls.sch_no ,
                                                                            unit = '000000000',
                                                                            year_id = convert(smallint , s04_hstusubjscoreterm.in_year ),
                                                                            sms_id = convert(smallint , s04_hstusubjscoreterm.in_sms ),
                                                                            std_identity = convert(varchar(10),s04_student.std_identity),
                                                                            std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                                            s04_stddbgo.course_code,
                                                                            sub_name = s04_108subject.sub108_name,
                                                                            convert(integer, case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
                                                                            --convert(nvarchar(5), case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
                                                                            s04_noropenc.grd_id,
                                                                            s04_noropenc.in_sms_id,
                                                                            borrow_yn = 'false',
                                                                            reread_yn = 'true',
                                                                            reread_yms = convert(varchar , s04_hstusubjscoreterm.in_year )+convert(varchar , s04_hstusubjscoreterm.in_sms ),
                                                                            reread_type = '1',
                                                                            repair_yn = 'false',
                                                                            repair_yms = '0000',
                                                                            repair_type = '0',
                                                                            reread2_yn = 'false',
                                                                            reread2_yms = '0000',
                                                                            turn_yn = 'false',
                                                                            doc1_path = '',
                                                                            doc1_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                                            ELSE L01_std_public_filehub.file_md5
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            doc2_path = '',
                                                                            doc2_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                                            ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            content = '',
                                                                            status = isnull(case a.attestation_centraldb when 'Y' then '1' when 'N' then '2' when 'F' then '0' end,'2'),
                                                                            upyms = CONVERT(varchar,s04_stuhcls.year_id)+CONVERT(varchar,s04_stuhcls.sms_id),
                                                                            cdate = isnull((
                                                                                    case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                                                    convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                                                    substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                                                    substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                                                    right(L01_std_public_filehub.upd_dt,2) end),''),
                                                                            yms = case when isnull(a.attestation_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                                            filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                                            file_extension,
                                                                            L01_std_public_filehub.class_name,
                                                                            L01_std_public_filehub.complex_key,
                                                                            L01_std_public_filehub.number_id
	                                                                            FROM s04_student
	                                                                            inner join s04_stuhcls on
		                                                                            s04_student.std_no = s04_stuhcls.std_no and												
		                                                                            s04_student.std_no = '{1}'
	                                                                            inner join s90_cha_id on
		                                                                            s04_stuhcls.std_status = s90_cha_id.cha_id and
		                                                                            s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
	                                                                            inner join s04_noropenc on
		                                                                            s04_noropenc.year_id = s04_stuhcls.year_id and
		                                                                            s04_noropenc.sms_id = s04_stuhcls.sms_id
	                                                                            inner join s04_norstusc on
		                                                                            s04_norstusc.noroc_id = s04_noropenc.noroc_id and
		                                                                            s04_student.std_no = s04_norstusc.std_no and
		                                                                            s04_norstusc.stu_status = 2 --重修
	                                                                            inner join s04_subject on
		                                                                            s04_noropenc.sub_id = s04_subject.sub_id
	                                                                            inner join s04_hstusubjscoreterm on
		                                                                            s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
		                                                                            s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
		                                                                            s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
		                                                                            s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
		                                                                            s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
		                                                                            s04_hstusubjscoreterm.hstusst_status = 2
	                                                                            inner join s04_stddbgo on
		                                                                            s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
		                                                                            s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
		                                                                            s04_stuhcls.deg_id = s04_stddbgo.deg_id and
		                                                                            s04_stuhcls.dep_id = s04_stddbgo.dep_id and
		                                                                            s04_stuhcls.bra_id = s04_stddbgo.bra_id and
		                                                                            s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
		                                                                            s04_stuhcls.cg_code = s04_stddbgo.cg_code and
		                                                                            isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
		                                                                            s04_noropenc.sub_id = s04_stddbgo.sub_id and
		                                                                            ( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
	                                                                            inner join s04_108subject on
		                                                                            s04_stddbgo.course_code = s04_108subject.course_code
	                                                                            inner join s90_organization on
		                                                                            s04_noropenc.org_id = s90_organization.org_id
	                                                                            inner join s90_branch on
		                                                                            s04_noropenc.bra_id = s90_branch.bra_id
	                                                                            inner join s04_hstusixscoreterm on
		                                                                            s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
		                                                                            s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
		                                                                            s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
	                                                                            left join L01_std_attestation a on
		                                                                            a.year_id = s04_stuhcls.year_id and
		                                                                            a.sms_id = s04_stuhcls.sms_id and
		                                                                            a.cls_id = s04_noropenc.noroc_id and
		                                                                            a.sub_id = s04_subject.sub_id and
		                                                                            a.src_dup = s04_stddbgo.course_code and
		                                                                            a.emp_id =  s04_noropenc.all_empid and 
		                                                                            isnull(a.attestation_send,'') <>'' and 
		                                                                            --isnull(a.attestation_confirm,'') = 'Y' and  
		                                                                            isnull(a.attestation_status,'') = 'Y' and
		                                                                            isnull(a.attestation_release,'') = 'Y'and
		                                                                            isnull(a.attestation_centraldb, '') = 'Y' and
		                                                                            a.std_no = s04_student.std_no
	                                                                            left join L01_std_attestation_file b on 
		                                                                            b.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
		                                                                            b.class_name = 'StuAttestation' and
		                                                                            b.type_id = 0 and
		                                                                            b.check_yn = 'Y'
                                                                                left join L01_std_public_filehub on
                                                                                    L01_std_public_filehub.class_name = 'StuAttestation' and
                                                                                    L01_std_public_filehub.complex_key = b.complex_key and
	                                                                                L01_std_public_filehub.class_name = b.class_name and
	                                                                                L01_std_public_filehub.type_id = b.type_id and
	                                                                                L01_std_public_filehub.number_id = b.number_id
	                                                                            left join s04_stubuterm on
		                                                                            s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
		                                                                            s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
		                                                                            s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
		                                                                            s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
	                                                                            inner join s04_hstusubjscoreterm hstusst_new on
		                                                                            s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
		                                                                            s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
		                                                                            s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
		                                                                            s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
		                                                                            hstusst_new.hstusst_status = 1
	                                                                            inner join s04_norstusc norstu_o on
		                                                                            hstusst_new.std_no = norstu_o.std_no and
		                                                                            hstusst_new.in_year = norstu_o.in_year and
		                                                                            hstusst_new.in_sms = norstu_o.in_sms and
		                                                                            norstu_o.stu_status = 1
	                                                                            inner join s04_noropenc noro_o on
		                                                                            norstu_o.noroc_id = noro_o.noroc_id and
		                                                                            hstusst_new.sub_id = noro_o.sub_id 		
	                                                                            inner join s90_organization org_o on
		                                                                            noro_o.org_id = org_o.org_id
	                                                                            where s04_student.std_hisdatayear >= 108 

                                                                            union

                                                                            select 
                                                                            s04_stuhcls.sch_no ,
                                                                            unit = '000000000',
                                                                            year_id = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
                                                                            sms_id = convert(varchar , s04_stddbgo.sms_id ),
                                                                            std_identity = convert(varchar(10),s04_student.std_identity),
                                                                            std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                                            s04_stddbgo.course_code,
                                                                            sub_name = s04_108subject.sub108_name,
                                                                            convert(integer, case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
                                                                            --convert(nvarchar(5), case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
                                                                            s04_noropenc.grd_id,
                                                                            s04_noropenc.in_sms_id,
                                                                            borrow_yn = 'false',
                                                                            reread_yn = 'false',
                                                                            reread_yms = '0000',
                                                                            reread_type = '0',
                                                                            repair_yn = 'true',
                                                                            repair_yms = convert(varchar,convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ))+convert(varchar , s04_stddbgo.sms_id ),
                                                                            repair_type = '1',
                                                                            reread2_yn = 'false',
                                                                            reread2_yms = '0000',
                                                                            turn_yn = 'false',
                                                                            doc1_path = '',
                                                                            doc1_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                                            ELSE L01_std_public_filehub.file_md5
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            doc2_path = '',
                                                                            doc2_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                                            ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            content = '',
                                                                            status = isnull(case a.attestation_centraldb when 'Y' then '1' when 'N' then '2' when 'F' then '0' end,'2'),
                                                                            upyms = CONVERT(varchar,s04_stuhcls.year_id)+CONVERT(varchar,s04_stuhcls.sms_id),
                                                                            cdate = isnull((
                                                                                    case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                                                    convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                                                    substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                                                    substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                                                    right(L01_std_public_filehub.upd_dt,2) end),''),
                                                                            yms = case when isnull(a.attestation_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                                            filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                                            file_extension,
                                                                            L01_std_public_filehub.class_name,
                                                                            L01_std_public_filehub.complex_key,
                                                                            L01_std_public_filehub.number_id
	                                                                            FROM s04_student
	                                                                            inner join s04_stuhcls on
		                                                                            s04_student.std_no = s04_stuhcls.std_no and												
		                                                                            s04_student.std_no = '{2}'
	                                                                            inner join s90_cha_id on
		                                                                            s04_stuhcls.std_status = s90_cha_id.cha_id and
		                                                                            s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
	                                                                            inner join s04_noropenc on
		                                                                            s04_noropenc.year_id = s04_stuhcls.year_id and
		                                                                            s04_noropenc.sms_id = s04_stuhcls.sms_id 
	                                                                            inner join s04_norstusc on
		                                                                            s04_norstusc.noroc_id = s04_noropenc.noroc_id and
		                                                                            s04_student.std_no = s04_norstusc.std_no and
		                                                                            s04_norstusc.stu_status = 3 --補修
	                                                                            inner join s04_subject on
		                                                                            s04_noropenc.sub_id = s04_subject.sub_id
	                                                                            inner join s90_organization on
		                                                                            s04_noropenc.org_id = s90_organization.org_id
	                                                                            inner join s04_hstusubjscoreterm on
		                                                                            s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
		                                                                            s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
		                                                                            s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
		                                                                            s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
		                                                                            s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
		                                                                            s04_hstusubjscoreterm.hstusst_status = 3
	                                                                            left join s04_hstusixscoreterm on
		                                                                            s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
		                                                                            s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
		                                                                            s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
	                                                                            left join s04_stubuterm on
		                                                                            s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
		                                                                            s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
		                                                                            s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
		                                                                            s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
	                                                                            inner join s04_stddbgo on
		                                                                            s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
		                                                                            s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
		                                                                            s04_stuhcls.deg_id = s04_stddbgo.deg_id and
		                                                                            s04_stuhcls.dep_id = s04_stddbgo.dep_id and
		                                                                            s04_stuhcls.bra_id = s04_stddbgo.bra_id and
		                                                                            s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
		                                                                            s04_stuhcls.cg_code = s04_stddbgo.cg_code and
		                                                                            isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
		                                                                            s04_noropenc.sub_id = s04_stddbgo.sub_id and
		                                                                            ( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
	                                                                            inner join s04_108subject on
		                                                                            s04_stddbgo.course_code = s04_108subject.course_code
	                                                                            inner join s04_ytdbgoc on
		                                                                            s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
		                                                                            s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
		                                                                            s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
		                                                                            s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
		                                                                            s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
		                                                                            s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
		                                                                            s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
	                                                                            inner join s90_organization org_o on
		                                                                            s04_ytdbgoc.org_id = org_o.org_id
	                                                                            inner join s90_branch on
		                                                                            s04_ytdbgoc.bra_id = s90_branch.bra_id
	                                                                            inner join s04_108subject subj108_o on
		                                                                            s04_stddbgo.course_code = subj108_o.course_code
	                                                                            left join L01_std_attestation a on
		                                                                            a.year_id = s04_stuhcls.year_id and
		                                                                            a.sms_id = s04_stuhcls.sms_id and
		                                                                            a.cls_id = s04_noropenc.noroc_id and
		                                                                            a.sub_id = s04_subject.sub_id and
		                                                                            a.src_dup = s04_stddbgo.course_code and
		                                                                            a.emp_id =  s04_noropenc.all_empid and
		                                                                            isnull(a.attestation_send,'') <>'' and
		                                                                            isnull(a.attestation_status,'') = 'Y' and
		                                                                            isnull(a.attestation_release,'') = 'Y'and
		                                                                            isnull(a.attestation_centraldb, '')  = 'Y' and
		                                                                            a.std_no =s04_student.std_no
	                                                                            left join L01_std_attestation_file b on 
		                                                                            b.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
		                                                                            b.class_name = 'StuAttestation' and
		                                                                            b.type_id = 0 and
		                                                                            b.check_yn = 'Y'
                                                                                left join L01_std_public_filehub on
                                                                                    L01_std_public_filehub.class_name = 'StuAttestation' and
                                                                                    L01_std_public_filehub.complex_key = b.complex_key and
	                                                                                L01_std_public_filehub.class_name = b.class_name and
	                                                                                L01_std_public_filehub.type_id = b.type_id and
	                                                                                L01_std_public_filehub.number_id = b.number_id
	                                                                            where s04_student.std_hisdatayear >= 108
                                                                        ", std_no, std_no, std_no);

            foreach (var item in conn.Query<Course>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);

                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file2++;
                    item.doc1_path = string.Format("{0}/課程學習成果/C{1}.{2}", std_identity, _file2 < 10 ? "0" + _file2.ToString() : _file2.ToString(), item.file_extension);
                    StudyJson.Add(new JProperty(string.Format("C{0}.{1}", _file2 < 10 ? "0" + _file2.ToString() : _file2.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\課程學習成果\C{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file2 < 10 ? "0" + _file2.ToString() : _file2.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file2++;
                    item.doc2_path = string.Format("{0}/課程學習成果/C{1}.{2}", std_identity, _file2 < 10 ? "0" + _file2.ToString() : _file2.ToString(), item.file_extension);
                    StudyJson.Add(new JProperty(string.Format("C{0}.{1}", _file2 < 10 ? "0" + _file2.ToString() : _file2.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\課程學習成果\C{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file2 < 10 ? "0" + _file2.ToString() : _file2.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }

                course.Add(new Course
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    course_code = item.course_code,
                    sub_name = item.sub_name,
                    credit = item.credit,
                    grd_id = item.grd_id,
                    in_sms_id = item.in_sms_id,
                    borrow_yn = item.borrow_yn,
                    reread_yn = item.reread_yn,
                    reread_yms = item.reread_yms,
                    reread_type = item.reread_type,
                    repair_yn = item.repair_yn,
                    repair_yms = item.repair_yms,
                    repair_type = item.repair_type,
                    reread2_yn = item.reread2_yn,
                    reread2_yms = item.reread2_yms,
                    turn_yn = item.turn_yn,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    content = item.content,
                    status = item.status,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }
            return course;
        }
        public async Task<Diverse> GetDiverse(IDbConnection conn, string std_no, string std_identity)
        {
            Diverse diverse = new Diverse();
            diverse.Position = await GetPosition(conn, std_no, std_identity);
            diverse.Competition = await GetCompetition(conn, std_no, std_identity);
            diverse.License = await GetLicense(conn, std_no, std_identity);
            diverse.Volunteer = await GetVolunteer(conn, std_no, std_identity);
            diverse.Study = await GetStudy(conn, std_no, std_identity);
            diverse.Workplace = await GetWorkplace(conn, std_no, std_identity);
            diverse.Result = await GetResult(conn, std_no, std_identity);
            diverse.Group = await GetGroup(conn, std_no, std_identity);
            diverse.College = await GetCollege(conn, std_no, std_identity);
            diverse.Other = await GetOther(conn, std_no, std_identity);
            return diverse;
        }
        public async Task<List<Position>> GetPosition(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Position> position = new List<Position>();
            string str_sql = string.Format(@"
                                                                        select 
                                                                            sch_no = a.sch_no,
                                                                            unit = '000000000',
                                                                            a.year_id,
                                                                            a.sms_id,
                                                                            s04_student.std_identity,
                                                                            std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                                            unit_name = isnull(a.unit_name,''),
                                                                            startdate = case when isnull(a.startdate,'') = '' then '' else left(a.startdate,4)+'/'+substring(a.startdate,5,2)+'/'+right(a.startdate,2) end ,
                                                                            enddate = case when isnull(a.enddate,'') = '' then '' else left(a.enddate,4)+'/'+substring(a.enddate,5,2)+'/'+right(a.enddate,2) end ,
                                                                            position_name = isnull(a.position_name,''),
                                                                            a.type_id,
                                                                            content = '',
                                                                            doc1_path = '',
                                                                            doc1_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                                            ELSE L01_std_public_filehub.file_md5
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            doc2_path = '',
                                                                            doc2_md5 = 
                                                                                        isnull((
			                                                                            CASE
				                                                                            WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                                            ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                                            END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                                            outfile = '',
                                                                            upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                                            cdate = isnull((
                                                                                    case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                                                    convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                                                    substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                                                    substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                                                    substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                                                    right(L01_std_public_filehub.upd_dt,2) end),''),
                                                                            yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                                            filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                                            file_extension,
                                                                            L01_std_public_filehub.class_name,
                                                                            L01_std_public_filehub.complex_key,
                                                                            L01_std_public_filehub.number_id
                                                                        FROM L01_std_position a
                                                                        inner join s04_student on s04_student.std_no = a.std_no
                                                                        inner join s04_stuhcls on 
	                                                                        s04_stuhcls.std_no = s04_student.std_no and
	                                                                        s04_stuhcls.sch_no = a.sch_no and
	                                                                        s04_stuhcls.year_id = a.year_id and
	                                                                        s04_stuhcls.sms_id = a.sms_id
                                                                        inner join s04_ytdbgoc on 
	                                                                        s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                                        s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                                        s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                                        s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                                        s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                                        s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                                        s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                                        left join L01_std_public_filehub on
                                                                            L01_std_public_filehub.class_name = 'StudCadre' and
                                                                            L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+a.unit_name+'_'+a.position_name
                                                                        where
                                                                        a.std_no = '{0}' and
                                                                        a.is_sys='2' 
                                                                        ", std_no);
            foreach (var item in conn.Query<Position>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);

                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }

                position.Add(new Position
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    unit_name = item.unit_name,
                    sdate = item.sdate,
                    edate = item.edate,
                    position_name = item.position_name,
                    type_id = item.type_id,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return position;
        }
        public async Task<List<Competition>> GetCompetition(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Competition> competition = new List<Competition>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.competition_name,
                                                a.competition_grade,
                                                a.competition_result,
                                                competition_date = left(a.competition_date,4)+'/'+substring(a.competition_date,5,2)+'/'+right(a.competition_date,2) ,
                                                competition_item = isnull( a.competition_item,''),
                                                a.content,
                                                a.competition_type,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_competition a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuCompetition' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Competition>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                competition.Add(new Competition
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    competition_name = item.competition_name,
                    competition_grade = item.competition_grade,
                    competition_result = item.competition_result,
                    competition_date = item.competition_date,
                    competition_item = item.competition_item,
                    content = item.content,
                    competition_type = item.competition_type,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return competition;
        }
        public async Task<List<License>> GetLicense(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<License> license = new List<License>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.license_id,
                                                a.license_memo,
                                                a.license_grade,
                                                license_date = case when isnull(a.license_date,'') = '' then '' else left(a.license_date,4)+'/'+substring(a.license_date,5,2)+'/'+right(a.license_date,2) end ,
                                                license_result= isnull( a.license_result,''),
                                                license_doc= isnull( a.license_doc,''),
                                                license_group = isnull( a.license_group,''),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_license a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuLicense' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<License>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                license.Add(new License
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    license_id = item.license_id,
                    license_memo = item.license_memo,
                    license_grade = item.license_grade,
                    license_date = item.license_date,
                    license_result = item.license_result,
                    license_doc = item.license_doc,
                    license_group = item.license_group,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return license;
        }
        public async Task<List<Volunteer>> GetVolunteer(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Volunteer> volunteer = new List<Volunteer>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.volunteer_name,
                                                a.volunteer_unit,
                                                startdate = case when isnull(a.startdate,'') = '' then '' else left(a.startdate,4)+'/'+substring(a.startdate,5,2)+'/'+right(a.startdate,2) end ,
                                                enddate = case when isnull(a.enddate,'') = '' then '' else left(a.enddate,4)+'/'+substring(a.enddate,5,2)+'/'+right(a.enddate,2) end ,
                                                hours = isnull( a.hours,-1),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_volunteer a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuVolunteer' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Volunteer>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                volunteer.Add(new Volunteer
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    volunteer_name = item.volunteer_name,
                    volunteer_unit = item.volunteer_unit,
                    startdate = item.startdate,
                    enddate = item.enddate,
                    hours = item.hours,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return volunteer;
        }
        public async Task<List<Study>> GetStudy(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Study> study = new List<Study>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                                std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.type_id,
                                                a.open_name,
                                                a.open_unit,
                                                a.hours,
                                                a.weeks,
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_study_free a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuStudyf' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Study>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                study.Add(new Study
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    type_id = item.type_id,
                    open_name = item.open_name,
                    open_unit = item.open_unit,
                    hours = item.hours,
                    weeks = item.weeks,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return study;
        }
        public async Task<List<Workplace>> GetWorkplace(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Workplace> workplace = new List<Workplace>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                                std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.type_id,
                                                a.unit_name,
                                                a.type_title,
                                                startdate = case when isnull(a.startdate,'') = '' then '' else left(a.startdate,4)+'/'+substring(a.startdate,5,2)+'/'+right(a.startdate,2) end ,
                                                enddate = case when isnull(a.enddate,'') = '' then '' else left(a.enddate,4)+'/'+substring(a.enddate,5,2)+'/'+right(a.enddate,2) end ,
                                                hours = isnull(convert(decimal(8,3),a.hours),-1),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_workplace a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuWorkplace' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Workplace>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                workplace.Add(new Workplace
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    type_id = item.type_id,
                    unit_name = item.unit_name,
                    type_title = item.type_title,
                    startdate = item.startdate,
                    enddate = item.enddate,
                    hours = item.hours,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return workplace;
        }
        public async Task<List<Result>> GetResult(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Result> result = new List<Result>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.result_name,
                                                result_date = left(a.result_date,4)+'/'+substring(a.result_date,5,2)+'/'+right(a.result_date,2),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_result a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuResult' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no ='{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Result>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                result.Add(new Result
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    result_name = item.result_name,
                    result_date = item.result_date,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return result;
        }
        public async Task<List<Group>> GetGroup(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Group> group = new List<Group>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.time_id,
                                                a.unit_name,
                                                a.group_content,
                                                hours = isnull( a.hours,-1),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = 'd41d8cd98f00b204e9800998ecf8427e',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_group a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuGroup' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Group>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                group.Add(new Group
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    time_id = item.time_id,
                    unit_name = item.unit_name,
                    group_content = item.group_content,
                    hours = item.hours,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return group;
        }
        public async Task<List<College>> GetCollege(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<College> college = new List<College>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                a.project_name,
                                                a.unit_name,
                                                a.course_name,
                                                startdate = case when isnull(a.startdate,'') = '' then '' else left(a.startdate,4)+'/'+substring(a.startdate,5,2)+'/'+right(a.startdate,2) end ,
                                                enddate = case when isnull(a.enddate,'') = '' then '' else left(a.enddate,4)+'/'+substring(a.enddate,5,2)+'/'+right(a.enddate,2) end ,
                                                credit = isnull(a.credit,-1),
                                                hours = isnull(a.hours,-1),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_college a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuCollege' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no ='{0}'
                                            ", std_no);
            foreach (var item in conn.Query<College>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                college.Add(new College
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    project_name = item.project_name ?? "",
                    unit_name = item.unit_name,
                    course_name = item.course_name,
                    startdate = item.startdate,
                    enddate = item.enddate,
                    credit = Convert.ToInt32(item.credit),
                    hours = item.hours,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return college;
        }
        public async Task<List<Other>> GetOther(IDbConnection conn, string std_no, string std_identity)
        {
            StuFileInfoQueryList arg = new StuFileInfoQueryList();
            List<Other> other = new List<Other>();
            string str_sql = string.Format(@"
                                                select
                                                sch_no = a.sch_no,
                                                unit = '000000000',
                                                a.year_id,
                                                a.sms_id,
                                                s04_student.std_identity,
                                               std_birth_dt = convert(varchar,convert(integer,left(s04_student.std_birth_dt,3))+1911)+'/'+substring(s04_student.std_birth_dt,4,2)+'/'+right(s04_student.std_birth_dt,2),
                                                other_name = isnull(a.other_name,''),
                                                unit_name = isnull(a.unit_name,''),
                                                startdate = case when isnull(a.startdate,'') = '' then '' else left(a.startdate,4)+'/'+substring(a.startdate,5,2)+'/'+right(a.startdate,2) end ,
                                                enddate = case when isnull(a.enddate,'') = '' then '' else left(a.enddate,4)+'/'+substring(a.enddate,5,2)+'/'+right(a.enddate,2) end ,
                                                hours = isnull( a.hours,-1),
                                                a.content,
                                                doc1_path = '',
                                                doc1_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN 'd41d8cd98f00b204e9800998ecf8427e'
				                                                ELSE L01_std_public_filehub.file_md5
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                doc2_path = '',
                                                doc2_md5 = 
                                                            isnull((
			                                                CASE
				                                                WHEN L01_std_public_filehub.file_extension = 'mp3' or file_extension = 'mp4' THEN L01_std_public_filehub.file_md5
				                                                ELSE 'd41d8cd98f00b204e9800998ecf8427e'
			                                                END),'d41d8cd98f00b204e9800998ecf8427e'),
                                                outfile = '',
                                                upyms = CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id),
                                                cdate = isnull((
                                                        case where isnull(L01_std_public_filehub.upd_dt,'') = '' then '' else
                                                        convert(varchar,convert(integer,left(L01_std_public_filehub.upd_dt,3))+1911)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,4,2)+'-'+
                                                        substring(L01_std_public_filehub.upd_dt,6,2)+' '+
                                                        substring(L01_std_public_filehub.upd_dt,8,2)+':'+
                                                        substring(L01_std_public_filehub.upd_dt,10,2)+':'+
                                                        right(L01_std_public_filehub.upd_dt,2) end),''),
                                                yms = case when isnull(a.check_centraldb,'') = 'Y' then CONVERT(varchar,a.year_id)+CONVERT(varchar,a.sms_id) else '0000' end,
                                                filename = L01_std_public_filehub.file_name+'.'+file_extension,
                                                file_extension,
                                                L01_std_public_filehub.class_name,
                                                L01_std_public_filehub.complex_key,
                                                L01_std_public_filehub.number_id
                                                FROM L01_stu_other a
                                                inner join s04_student on s04_student.std_no = a.std_no
                                                inner join s04_stuhcls on 
	                                                s04_stuhcls.std_no = s04_student.std_no and
	                                                s04_stuhcls.sch_no = a.sch_no and
	                                                s04_stuhcls.year_id = a.year_id and
	                                                s04_stuhcls.sms_id = a.sms_id
                                                inner join s04_ytdbgoc on 
	                                                s04_ytdbgoc.sch_no = s04_stuhcls.sch_no and
	                                                s04_ytdbgoc.year_id = s04_stuhcls.year_id and
	                                                s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
	                                                s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
	                                                s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
	                                                s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
	                                                s04_ytdbgoc.cls_id = s04_stuhcls.cls_id
                                                left join L01_std_public_filehub on
                                                    L01_std_public_filehub.class_name = 'StuOther' and
                                                    L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.std_no+'_'+convert(varchar,a.ser_id)
                                                where
                                                a.std_no = '{0}'
                                            ", std_no);
            foreach (var item in conn.Query<Other>(str_sql))
            {
                arg.number_id = item.number_id;
                arg.complex_key = item.complex_key;
                arg.class_name = item.class_name;

                var file = await _service.GetFile(arg);
                if (!string.IsNullOrEmpty(item.doc1_md5))
                {
                    _file1++;
                    item.doc1_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                if (!string.IsNullOrEmpty(item.doc2_md5))
                {
                    _file1++;
                    item.doc2_path = string.Format("{0}/多元表現/M{1}.{2}", std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension);
                    DiverseJson.Add(new JProperty(string.Format("M{0}.{1}", _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), item.filename));

                    if (file != null)
                    {
                        System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}\{2}\多元表現\M{3}.{4}", root, _context.SchNo + '_' + getDate, std_identity, _file1 < 10 ? "0" + _file1.ToString() : _file1.ToString(), item.file_extension), _zip.UnzipData(file.file_blob));
                    }
                }
                other.Add(new Other
                {
                    sch_no = item.sch_no,
                    unit = item.unit,
                    year_id = item.year_id,
                    sms_id = item.sms_id,
                    std_identity = item.std_identity,
                    std_birth_dt = item.std_birth_dt,
                    other_name = item.other_name,
                    unit_name = item.unit_name,
                    startdate = item.startdate,
                    enddate = item.enddate,
                    hours = item.hours,
                    content = item.content,
                    doc1_path = item.doc1_path,
                    doc1_md5 = item.doc1_md5,
                    doc2_path = item.doc2_path,
                    doc2_md5 = item.doc2_md5,
                    upyms = item.upyms,
                    cdate = item.cdate,
                    yms = item.yms
                });
            }

            return other;
        }
        public void DictDiverse(string key, string doc1, string doc2, string md5_1, string md5_2, string class_name,int ser_id,string std_no)
        {
            if (!string.IsNullOrEmpty(doc1))
            {
                diversedict.Add(new DiverseDict { key = key, value = doc1, md5 = md5_1, class_name = class_name,ser_id =ser_id, std_no = std_no });
                //dictDiverse.Add(key, doc1);
            }

            if (!string.IsNullOrEmpty(doc2))
            {
                diversedict.Add(new DiverseDict { key = key, value = doc1, md5 = md5_1, class_name = class_name, ser_id = ser_id, std_no = std_no });
                //dictDiverse.Add(key, doc2);
            }
        }
        public async Task<int> InsertFile(IEnumerable<L01_std_public_filehub> files, IDbConnection conn, IDbTransaction tran)
        {
            int rt = 0;
            int number_id = 0;

            string sql = @"
                            delete from  L01_std_public_filehub where is_import = 'Y'
                          ";
            await conn.ExecuteAsync(sql, transaction: tran);

            //sql = @"
            //                        delete from  L01_std_attestation_file where complex_key=@complex_key and  class_name=@class_name
            //                    ";
            //await conn.ExecuteAsync(sql, new { files.First().complex_key, files.First().class_name }, transaction: tran);

            //string sql = @"
            //                SELECT max(number_id) 
            //                FROM L01_std_public_filehub
            //                WHERE class_name=@class_name
            //                and type_id = 0 
            //                and complex_key=@complex_key
            //            ";

            //number_id = await conn.ExecuteScalarAsync<int>(sql, new { files.First().complex_key, files.First().class_name }, transaction: tran);

            foreach (L01_std_public_filehub file in files)
            {
                //if (file.class_name != "StuAttestation") 
                //{
                //    number_id++;
                //    file.number_id = number_id;
                //}

                file.upd_dt = updteDate();
                file.file_id = Guid.NewGuid().ToString();
            }

            string insert = @"
            INSERT INTO L01_std_public_filehub
            VALUES
            (
                @complex_key, @class_name, @type_id, @number_id, @file_name, @file_extension, @file_blob, @upd_name, @upd_dt,@content,@file_md5,'Y',@file_id
            )
            ";

            rt = await conn.ExecuteAsync(insert, files, transaction: tran);

            return rt;
        }
    }
}
