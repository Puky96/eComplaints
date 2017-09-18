using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eComplaints.Models;
using eComplaints.Models.ManageViewModels;
using eComplaints.Services;
using eComplaints.DBModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eComplaints.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly eComplaintsCTX ctx = new eComplaintsCTX();
        private string defaultAssignedPassword = ")1~=M[y%t(Dhn!=d$0b,;WT:DJPuwQ";

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          RoleManager<IdentityRole> roleManager,
          ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddInvestigator(string message = null)
        {
            var departments = ctx.Department.ToList();
            var Departments = new SelectList(departments, "Id", "Name");
            ViewBag.Departments = Departments;

            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddInvestigator(AddInvestigatorModel model)
        {
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                return AddInvestigator("Utilizator deja inregistrat ca investigator!");
            }
            else
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.UserName + "@pg.com", FullName = model.FullName };
                    var result = await _userManager.CreateAsync(user, defaultAssignedPassword);
                    await _userManager.AddToRoleAsync(user, "Investigator");

                    var resUser = await _userManager.FindByNameAsync(model.UserName);
                    ctx.Investigator2Department.Add(new Investigator2Department
                    {
                        DepartmentId = Convert.ToInt32(model.Department),
                        InvestigatorId = resUser.Id,
                        IsInvestigator = true
                    });
                    ctx.SaveChanges();

                    return RedirectToAction("ViewInvestigators");
                }
                catch
                {
                    return AddInvestigator("A aparut o eroare. Incercati din nou!");
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewInvestigators(string Message = null)
        {
            ViewBag.Message = Message;

            var users = ctx.AspNetUsers.Where(us => us.UserName != "URLT-Admin").ToList();
            List<ViewInvestigatorItem> modelList = new List<ViewInvestigatorItem>();
            foreach (var usr in users)
            {
                var user = await _userManager.FindByNameAsync(usr.UserName);
                var role = await _userManager.GetRolesAsync(user);
                if (role.Contains("Investigator"))
                {
                    var depId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == usr.Id).DepartmentId;
                    modelList.Add(new ViewInvestigatorItem
                    {
                        Id = usr.Id,
                        Department = ctx.Department.FirstOrDefault(dep => dep.Id == depId).Name,
                        Username = usr.UserName
                    });
                }
            }

            return View(modelList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInvestigator(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.DeleteAsync(user);

            return RedirectToAction("ViewInvestigators");
        }

        [Authorize(Roles = "Investigator")]
        public IActionResult AddOriginator(string message = null)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> AddOriginator(AddOriginatorModel model)
        {
            if (await _userManager.FindByNameAsync(model.OriginatorName) != null)
            {
                return AddOriginator("Utilizator deja existent!");
            }
            else
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.OriginatorName, Email = model.OriginatorName + "@pg.com", FullName = model.FullName };
                    var result = await _userManager.CreateAsync(user, defaultAssignedPassword);
                    await _userManager.AddToRoleAsync(user, "Originator");

                    var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    var depId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == loggedUser.Id).DepartmentId;
                    var orig = await _userManager.FindByNameAsync(model.OriginatorName);

                    ctx.Investigator2Department.Add(new Investigator2Department
                    {
                        DepartmentId = depId,
                        InvestigatorId = orig.Id,
                        IsInvestigator = false
                    });
                    ctx.SaveChanges();


                    return RedirectToAction("ViewOriginators");

                }
                catch
                {
                    return AddOriginator("A aparut o eroare! Incercati din nou!");
                }
            }
        }

        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> ViewOriginators()
        {
            var users = ctx.AspNetUsers.Where(x => x.UserName != "URLT-Admin").ToList();
            List<ViewInvestigatorItem> modelList = new List<ViewInvestigatorItem>();
            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var depId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == loggedUser.Id).DepartmentId;

            foreach (var usr in users)
            {
                var user = await _userManager.FindByNameAsync(usr.UserName);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Originator") && ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == user.Id && x.DepartmentId == depId) != null)
                {
                    modelList.Add(new ViewInvestigatorItem
                    {
                        Id = usr.Id,
                        Department = ctx.Department.FirstOrDefault(x => x.Id == depId).Name,
                        Username = usr.UserName
                    });
                }
            }


            return View(modelList);
        }

        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> DeleteOriginator(string originatorId)
        {
            var user = await _userManager.FindByIdAsync(originatorId);
            await _userManager.DeleteAsync(user);

            return RedirectToAction("ViewOriginators");
        }

        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> AddLineCoordinator(string message = null)
        {
            ViewBag.Message = message;

            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var depId = ctx.Investigator2Department.FirstOrDefault(dep => dep.InvestigatorId == loggedUser.Id).DepartmentId;
            var areas = ctx.Area.Where(x => x.DepartmentId == depId).ToList();
            var Areas = new SelectList(areas, "Id", "Name");
            ViewBag.Areas = Areas;


            return View();
        }

        [Authorize(Roles = "Investigator")]
        [HttpPost]
        public async Task<IActionResult> AddLineCoordinator(LineCoordinatorModel model)
        {
            var exists = ctx.LineCoordinator.FirstOrDefault(arr => arr.AreaId == Convert.ToInt32(model.Area));

            if (exists != null)
            {
                return await AddLineCoordinator("Coordonator deja existent pentru linia respectiva!");
            }
            else
            {
                ctx.LineCoordinator.Add(new LineCoordinator
                {
                    AreaId = Convert.ToInt32(model.Area),
                    LineCoordinatorName = model.CoordinatorName
                });
                ctx.SaveChanges();

                return RedirectToAction("ViewLineCoordinators");
            }
        }

        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> ViewLineCoordinators()
        {
            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var depId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == loggedUser.Id).DepartmentId;
            var areasId = ctx.Area.Where(ar => ar.DepartmentId == depId).Select(ar => ar.Id).ToList();

            var coordinators = ctx.LineCoordinator.OrderBy(crd => crd.AreaId).ToList();
            List<LineCoordinatorItem> modelList = new List<LineCoordinatorItem>();
            foreach (var coordinator in coordinators)
            {
                if (areasId.Contains(coordinator.AreaId))
                {
                    modelList.Add(new LineCoordinatorItem
                    {
                        Id = coordinator.Id,
                        Area = ctx.Area.FirstOrDefault(ar => ar.Id == coordinator.AreaId).Name,
                        LineCoordinatorName = coordinator.LineCoordinatorName
                    });
                }
            }

            return View(modelList);
        }

        [Authorize(Roles = "Investigator")]
        public IActionResult EditLineCoordinator(int lineCoordinator)
        {
            var coordinator = ctx.LineCoordinator.FirstOrDefault(crd => crd.Id == lineCoordinator);
            EditLineCoordinatorViewModel model = new EditLineCoordinatorViewModel { AreaId = coordinator.AreaId, LineCoordinatorName = coordinator.LineCoordinatorName, Id = coordinator.Id, AreaName = ctx.Area.FirstOrDefault(x => x.Id == coordinator.AreaId).Name };

            return View(model);
        }

        [Authorize(Roles = "Investigator")]
        [HttpPost]
        public IActionResult EditLineCoordinator(EditLineCoordinatorViewModel model)
        {
            var coordinator = ctx.LineCoordinator.FirstOrDefault(x => x.Id == model.Id);
            coordinator.LineCoordinatorName = model.LineCoordinatorName;

            ctx.LineCoordinator.Update(coordinator);
            ctx.SaveChanges();

            return RedirectToAction("ViewLineCoordinators");
        }



    }

}
