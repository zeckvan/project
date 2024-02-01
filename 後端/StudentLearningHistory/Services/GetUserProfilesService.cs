using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuAttestation.DbModels;
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

namespace StudentLearningHistory.Services
{
    public class GetUserProfilesService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;

        public GetUserProfilesService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public TokenResult GetUserData(string token)
        {
            var builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json");
            var config = builder.Build();
            HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.Add("X-Token", token);
            TokenResult? Data = httpClient.GetFromJsonAsync<TokenResult>(config["setting:CheckURL"].ToString()).Result;
            string Find = "edudb_190406";
            string ConnectionString = _configuration.GetConnectionString("edudb");
            int Place = ConnectionString.IndexOf(Find);
            string result = ConnectionString.Remove(Place, Find.Length).Insert(Place, string.Format("edudb_{0}", Data.result.sch_no));
            _context.online = result;
            _context.SchNo = Data.result.sch_no;
            _context.now_year = Data.result.default_year;
            _context.now_sms = Data.result.default_sms;
            _context.user_id = Data.result.user_id;
            return Data;
        }
    }
}
