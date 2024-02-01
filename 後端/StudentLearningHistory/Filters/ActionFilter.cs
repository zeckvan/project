using DocumentFormat.OpenXml.InkML;
using Irony.Parsing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.UserProfiles;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using static StudentLearningHistory.Services.StuTurnService;

namespace StudentLearningHistory.Filters
{
    public class ActionFilter : IActionFilter
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public ActionFilter(IConfiguration configuration, IDapperContext context)
        {
            _context = context;
            _configuration = configuration;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var args = context.ActionArguments.Where(a => a.Key == "token" || a.Key == "arg").Select(a => $"{JsonConvert.SerializeObject(a.Value, Formatting.Indented)}");
            //var getdata = args.FirstOrDefault();
            //var token = "";

            //if (JToken.Parse(getdata).Count() > 0)
            //{
            //    if (getdata.GetType().IsArray)
            //    {
            //        token = JToken.Parse(getdata).ToArray()[0].ToString();
            //    }
            //    //else if ((getdata.StartsWith("{") && getdata.EndsWith("}")) || 
            //    //             (getdata.StartsWith("[") && getdata.EndsWith("]")))
            //    else if ((getdata.StartsWith("[") && getdata.EndsWith("]")))
            //    {
            //        token = JToken.Parse(getdata).ToArray()[0].ToString();
            //    }
            //    else
            //    {
            //        token = JToken.Parse(getdata).ToString();
            //    }
            //    //if (getdata.Contains("["))
            //    //{
            //    //    token = JToken.Parse(getdata).ToArray()[0].ToString();
            //    //}
            //    //else
            //    //{
            //    //    token = JToken.Parse(getdata).ToString();
            //    //}

            //    token = token.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
            //    JObject obj = JObject.Parse(token);
            //    token = (string)obj["token"];
            //}
            //else
            //{
            //    token = JToken.Parse(getdata).ToString();
            //}
            if (_context.is_401 == "Y")
            {
                context.HttpContext.Response.StatusCode = 401;
                context.HttpContext.Response.WriteAsync("401");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
    public class GetToken
    {
        public string? Token { get; set; }
    }
}
