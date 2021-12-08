using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecNet.Web.Controllers
{
    public class Xss : Controller
    {
        [HttpGet]
        public IActionResult Reflected()
        {
            AddInsecureCookie();
            //AddMoreSecureCookie();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reflected(string untrustedData)
        {
            ViewBag.UntrustedData = untrustedData;
            return View("ReflectedResponse");
        }

        private void AddInsecureCookie()
        {
            HttpContext.Response.Cookies.Append("InsecureCookie", "This Content Is Vulnerable");
        }

        private void AddMoreSecureCookie()
        {
            HttpContext.Response.Cookies.Append("MyMoreSecureCookie", "ThisIsTheTargetContent",
                new CookieOptions()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                }
            );
        }

    }
}
