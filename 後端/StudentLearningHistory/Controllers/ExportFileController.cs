using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Irony.Parsing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentLearningHistory.Models.Public.DbModels;
using StudentLearningHistory.Models.Public.Parameters;
using StudentLearningHistory.Models.UserProfiles;
using StudentLearningHistory.Services;

namespace StudentLearningHistory.Controllers
{
    [ApiController]
    [Route("api")]
    //[Authorize]
    public class ExportFileController : Controller
    {
        private readonly PublicService _service;
        private readonly GetUserProfilesService _GetUserProfilesService;
        private readonly IConfiguration _configuration;
        public ExportFileController(PublicService service, GetUserProfilesService getUserProfilesService, IConfiguration configuration)
        {
            _service = service;
            _GetUserProfilesService = getUserProfilesService;
            _configuration = configuration;
        }

        #region
        [HttpGet("ExportFile/{file_id}")]
        public async Task<IActionResult> GetS90Year()
        {
            IEnumerable<S90_Year> data = await _service.Get_s90_year();
            return Json(new { dataset = data, status = (data.Count() <= 0 ? "N" : "Y") });
        }
        #endregion
    }
}
