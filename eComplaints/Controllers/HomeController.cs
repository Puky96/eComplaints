using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eComplaints.Models;
using Microsoft.AspNetCore.Authorization;

namespace eComplaints.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("LDAP", "Account");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
