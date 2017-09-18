using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eComplaints.Models;
using eComplaints.Models.AccountViewModels;
using eComplaints.Services;
using eComplaints.DBModels;
using Novell.Directory.Ldap;
using Microsoft.AspNetCore.Http;

namespace eComplaints.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private eComplaintsCTX ctx = new eComplaintsCTX();
        private string defaultAssignedPassword = ")1~=M[y%t(Dhn!=d$0b,;WT:DJPuwQ";

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }


        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToAction("ShowLandingPage");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        //
        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LDAP()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LDAP(LDAPAuthentication model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = ctx.AspNetUsers.FirstOrDefault(x => x.UserName == model.UserName);

                if (user != null)
                {
                    var cn = new LdapConnection();
                    cn.Connect("pg.com", 389);
                    try
                    {
                        var usr = "EU\\" + model.UserName;
                        var psw = model.Password;
                        cn.Bind(usr, psw);

                        var result = await _signInManager.PasswordSignInAsync(model.UserName, defaultAssignedPassword, false, lockoutOnFailure: false);

                        return RedirectToAction("ShowLandingPage");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, "Incercare de autentificare invalida!");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Incercare de autentificare invalida!");

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }


        public async Task<IActionResult> ShowLandingPage()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Admin"))
                return RedirectToAction("ViewInvestigators", "Manage");
            if (userRoles.Contains("Investigator"))
                return RedirectToAction("Investigator", "Report");
            if (userRoles.Contains("Originator"))
                return RedirectToAction("Originator", "Report");

            return RedirectToAction("Index", "Home");

        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
