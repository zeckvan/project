using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StudentLearningHistory.Context;
using System.Data;
using Microsoft.Extensions.Caching.Distributed;
using StudentLearningHistory.Auth.Models;
using Azure;

namespace StudentLearningHistory.Auth
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IDapperContext _context;
        private readonly IDistributedCache _cache;

        public AuthController(IDapperContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Login()
        {
            #region 簡單的數字驗證
            string token = Guid.NewGuid().ToString();
            Random rnd = new Random();
            int a = rnd.Next(25) + 1;
            int b = rnd.Next(25) + 1;
            int answer = a + b;
            string challenge = $"{a}+{b}=";
            _cache.SetString(token, answer.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            #endregion


            return Json(new { dataset = new { token, challenge }, status = "noLogin" });
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login loginData)
        {
            //數字驗證
            string answer = await _cache.GetStringAsync(loginData.Token);
            if (answer == null)
            {
                return NotFound();
            }
            //清除cache
            await _cache.RemoveAsync(loginData.Token);

            if (answer == loginData.Response)
            {
                //讀取DB判斷身份
                var user = 0;

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    //user資訊
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "test"),
                        new Claim("FullName", "test"),
                    };

                    //user權限
                    string[] role = { "1", "2" };
                    foreach (var temp in role)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, temp));
                    }

                    /* var authProperties = new AuthenticationProperties
                     {
                         ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(2)
                     };*/

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return Json(new { dataset = "", status = "OK" });
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public async void LogOut() => await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        [HttpGet("NoAccess")]
        public IActionResult NoAccess() => Json(new { dataset = "沒有權限", status = "Y" });
    }
}
