using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Models.Global;
using Irony.Parsing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using StudentLearningHistory.Services;

namespace StudentLearningHistory.Filters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly GetUserProfilesService _GetUserProfilesService;
        public AuthorizationFilter(IConfiguration configuration, IDapperContext context, GetUserProfilesService getUserProfilesService)
        {
            _context = context;
            _configuration = configuration;
            _GetUserProfilesService = getUserProfilesService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                var exists = Directory.Exists(root);
                if (!exists)
                {
                    Directory.CreateDirectory(root);
                }

                var token = context.HttpContext.Request?.Headers["SkyGet"];
                  //var token = "f332703b";
                _GetUserProfilesService.GetUserData(token);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                {
                    _context.is_401 = "Y";
                }
            }
        }
    }
}
