using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kainos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kainos.Controllers
{
    public class HomeController : Controller
    {
        private Secrets _secrets { get; }

        public HomeController(IOptions<Secrets> secrets)
        {
            this._secrets = secrets.Value;
        }
        public IActionResult Index()
        {
           

            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Our super secret password is:";
            ViewData["UserSecret"] = string.IsNullOrEmpty(this._secrets.PublicKey)
                  ? "Are you in production?"
                  : this._secrets.PublicKey;
            ViewData["Message"] = "Our super secret password is:";
            ViewData["UserSecret2"] = string.IsNullOrEmpty(this._secrets.SecretKey)
                  ? "Are you in production?"
                  : this._secrets.SecretKey;

            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
