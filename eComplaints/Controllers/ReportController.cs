using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using eComplaints.DBModels;
using Microsoft.AspNetCore.Identity;
using eComplaints.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using eComplaints.Models.ReportViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using eComplaints.Models.jsReport;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using eComplaints.Services;

namespace eComplaints.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {

        private IHostingEnvironment Environment;
        private eComplaintsCTX ctx;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _accesor;

        public ReportController(IHostingEnvironment environment, UserManager<ApplicationUser> userManager, IHttpContextAccessor accesor)
        {
            Environment = environment;
            _accesor = accesor;
            ctx = new eComplaintsCTX();
            _userManager = userManager;
        }

        [Authorize(Roles = "Originator")]
        public async Task<IActionResult> Originator(string message = null)
        {
            ViewBag.Message = message;

            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var departmentId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == userId).DepartmentId;


            //trebuie sa trimit in View datele de autocomplet (linie, echipament etc)
            var Areas = ctx.Area.Where(x => x.DepartmentId == departmentId).ToList();
            var AreaList = new SelectList(Areas, "Id", "Name");
            ViewBag.AreaList = AreaList;

            var qCategories = ctx.Qcategory.Where(x => x.DepartmentId == departmentId).ToList();
            var Categorylist = new SelectList(qCategories, "Id", "Name");
            ViewBag.CategoryList = Categorylist;

            //var zones = ctx.Zone.ToList();
            //var zonesList = new SelectList(zones, "Id", "Name");
            //ViewBag.ZoneList = zonesList;

            return View(new MOriginatorReport());
        }


        //retrieving json from database, so you can change in real time the dropdowns lists
        public JsonResult RetrieveEquipment(int areaId)
        {
            var zones = ctx.Zone.Where(arr => arr.AreaId == areaId).ToList();

            List<Equipment> EquipmentList = new List<Equipment>();
            foreach (var z in zones)
            {
                var equipment = ctx.Equipment.Where(arr => arr.ZoneId == z.Id).ToList();
                foreach (var eq in equipment)
                {
                    if (!EquipmentList.Contains(eq))
                        EquipmentList.Add(eq);
                }
            }

            List<EquipmentJSON> json2Send = new List<EquipmentJSON>();
            foreach (var e in EquipmentList)
                json2Send.Add(new EquipmentJSON { Id = e.Id, EquipmentName = e.Name });

            return Json(json2Send);
        }

        public JsonResult RetrieveQuestions(int categoryId)
        {
            var Questions = ctx.Question.Where(x => x.CategoryId == categoryId).ToList();

            List<QuestionJSON> json2Send = new List<QuestionJSON>();
            foreach (var c in Questions)
                json2Send.Add(new QuestionJSON { Id = c.Id, QuestionName = c.Question1 });

            return Json(json2Send);
        }

        public JsonResult RetrieveAreas(int departmentId)
        {
            var Areas = ctx.Area.Where(x => x.DepartmentId == departmentId).ToList();

            List<EquipmentJSON> json2Send = new List<EquipmentJSON>();
            foreach (var c in Areas)
                json2Send.Add(new EquipmentJSON { Id = c.Id, EquipmentName = c.Name });

            return Json(json2Send);
        }

        public JsonResult RetrieveSuppliers(int departmentId)
        {
            var Supplier = ctx.Supplier.Where(x => x.DepartmentId == departmentId).ToList();

            List<EquipmentJSON> json2Send = new List<EquipmentJSON>();
            foreach (var s in Supplier)
                json2Send.Add(new EquipmentJSON { Id = s.Id, EquipmentName = s.Name });

            return Json(json2Send);
        }

        public JsonResult RetrieveIssues(int departmentId)
        {
            var phenomenaCategories = ctx.Qcategory.Where(x => x.DepartmentId == departmentId).ToList();
            List<EquipmentJSON> json2Send = new List<EquipmentJSON>();

            foreach (var category in phenomenaCategories)
            {
                json2Send.Add(new EquipmentJSON { Id = category.Id, EquipmentName = category.Name });
            }

            return Json(json2Send);
        }

        public async Task<string> UploadImage(IFormFile image, bool material = false)
        {
            Random rnd = new Random();
            string uploadPath = Path.Combine(Environment.WebRootPath, "uploads");

            string fileName = image.FileName;

            if (fileName.Contains('\\'))
            {
                fileName = fileName.Split('\\').Last();
            }

            var time = DateTime.Now.ToString("yyyyMMddhhmmss") + rnd.Next(100, 999) + Path.GetExtension(fileName);

            if (material == true)
            {
                uploadPath = Path.Combine(uploadPath, "materials");
            }

            var path2Upload = Path.Combine(uploadPath, time);

            using (FileStream fs = new FileStream(path2Upload, FileMode.Create))
            {
                await image.CopyToAsync(fs);
            }

            return path2Upload;
        }

        [HttpPost]
        [Authorize(Roles = "Originator")]
        public async Task<IActionResult> Originator(MOriginatorReport model)
        {
            OutlookHandler emailSender = new OutlookHandler();
            var httpContext = _accesor.HttpContext;
            var host = httpContext.Request.Host.Value;

            var path = "http://" + host + "/Report/Inestigator";

            if (ModelState.IsValid)
            {
                model.ServerPath = await UploadImage(model.ImagePath);

                if (model.ImagePath1 != null)
                    model.ServerPath1 = await UploadImage(model.ImagePath1);
                else
                    model.ServerPath1 = null;
                if (model.ImagePath2 != null)
                    model.ServerPath2 = await UploadImage(model.ImagePath2);
                else
                    model.ServerPath2 = null;

                model.EtiquetePath = await UploadImage(model.EtiqueteImagePath, true);

                TimeSpan timedown = new TimeSpan(model.Hours, model.Minutes, model.Seconds);

                var count = 1;
                var Reports = ctx.Report.ToList();
                foreach (var reporting in Reports)
                    if (reporting.Gcas == model.GCAS && reporting.DateHour.Date == DateTime.Now.Date)
                        count++;
                model.IdentificationNumber = "CF" + model.GCAS.ToUpper() + "/" + DateTime.Now.ToString("dd.MM.yyyy") + "/" + count.ToString("00");


                Report Report2Upload = new Report
                {
                    ImagePath = model.ServerPath,
                    ImagePath1 = model.ServerPath1,
                    ImagePath2 = model.ServerPath2,
                    EtiqueteImagePath = model.EtiquetePath,
                    Originator = (await _userManager.FindByNameAsync(User.Identity.Name)).FullName,
                    EquipmentId = Convert.ToInt32(model.Equipment),
                    LineCoordinator = ctx.LineCoordinator.FirstOrDefault(x => x.AreaId == Convert.ToInt32(model.Area)).LineCoordinatorName,
                    AreaId = Convert.ToInt32(model.Area),
                    DepartmentId = ctx.Area.FirstOrDefault(x => x.Id == Convert.ToInt32(model.Area)).DepartmentId,
                    DateHour = DateTime.Now,
                    Gcas = model.GCAS,
                    IdentitifcationNumber = model.IdentificationNumber,
                    BatchSap = model.BatchSAP,
                    Po = model.PO,
                    DownTime = timedown,
                    NumberOfStops = model.NumberOfStops,
                    Sample = model.HasSample,
                    BlockedBatch = model.BlockedBatch,
                    BatchNo = model.BatchNo,
                    Quantity = model.Quantity,
                    PhenomenaCategory = Convert.ToInt32(model.PhenomenaCategory),
                    PendingApproval = true,
                };


                ctx.Report.Add(Report2Upload);
                ctx.SaveChanges();

                var report = ctx.Report.FirstOrDefault(x => x.ImagePath == model.ServerPath);
                Questions2Reports entry2Upload = new Questions2Reports { QuestionId = Convert.ToInt32(model.QuestionName), ReportId = report.Id };
                ctx.Questions2Reports.Add(entry2Upload);
                ctx.SaveChanges();

                var department = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == _userManager.FindByNameAsync(User.Identity.Name).Result.Id);
                var phenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name;
                var problemId = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == report.Id).QuestionId;
                var problem = ctx.Question.FirstOrDefault(x => x.Id == problemId).Question1;

                var receivers = ctx.Investigator2Department.Where(x => x.IsInvestigator && x.DepartmentId == department.DepartmentId).Select(x => x.InvestigatorId).ToList();

                foreach (var receiver in receivers)
                {
                    var emailReceiver = _userManager.FindByIdAsync(receiver).Result.Email;
                    emailSender.SendEmail(phenomenaCategory, problem, _userManager.FindByNameAsync(User.Identity.Name).Result.FullName, report.IdentitifcationNumber, emailReceiver, path);
                }

                return RedirectToAction("Originator", new { message = "Plangere inregistrata cu succes!"});
            }
            else
                return View(model);
        }

        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> Investigator()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var departmentId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == userId).DepartmentId;

            var reports = ctx.Report.Where(x => x.DepartmentId == departmentId).ToList();
            List<InvestigatorListItem> listView = new List<InvestigatorListItem>();
            //will populate the list only with the items pending approval
            foreach (var r in reports)
                if (r.PendingApproval == true)
                {
                    var qId = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == r.Id).QuestionId;
                    listView.Add(new InvestigatorListItem
                    {
                        ReportId = r.Id,
                        IdentificationNumber = r.IdentitifcationNumber,
                        Line = ctx.Area.FirstOrDefault(x => x.Id == r.AreaId).Name,
                        Date = r.DateHour,
                        PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == r.PhenomenaCategory).Name,
                        Problem = ctx.Question.FirstOrDefault(x => x.Id == qId).Question1
                    });
                }


            return View(listView);
        }

        [Authorize(Roles = "Investigator")]
        public IActionResult ReportDetails(int? reportId)
        {
            if (reportId == null)
                return BadRequest();
            else
            {
                var report = ctx.Report.FirstOrDefault(x => x.Id == reportId);
                var eqId = ctx.Report.FirstOrDefault(x => x.Id == reportId).EquipmentId;
                var equipment = ctx.Equipment.FirstOrDefault(x => x.Id == eqId).Name;
                var areaId = ctx.Report.FirstOrDefault(x => x.Id == reportId).AreaId;
                var area = ctx.Area.FirstOrDefault(x => x.Id == areaId).Name;
                var pcID = ctx.Report.FirstOrDefault(x => x.Id == reportId).PhenomenaCategory;
                var phenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == pcID).Name;
                var qId = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == reportId).QuestionId;
                var question = ctx.Question.FirstOrDefault(x => x.Id == qId).Question1;

                MDisplayReport model2Show = new MDisplayReport
                {
                    Id = report.Id,
                    BatchSAP = report.BatchSap,
                    DateHour = report.DateHour,
                    BlockedBatch = report.BlockedBatch,
                    ImagePath = getRelativePath(report.ImagePath),
                    ImagePath1 = getRelativePath(report.ImagePath1),
                    ImagePath2 = getRelativePath(report.ImagePath2),
                    EtiqueteImage = getRelativePath(report.EtiqueteImagePath, true),
                    DownTime = report.DownTime,
                    Equipment = equipment,
                    GCAS = report.Gcas,
                    HasSample = report.Sample,
                    Line = area,
                    LineCoordinator = report.LineCoordinator,
                    NumberOfStops = report.NumberOfStops,
                    Originator = report.Originator,
                    PhenomenaCategory = phenomenaCategory,
                    PO = report.Po,
                    Problem = question,
                    Quantity = report.Quantity
                };
                ViewBag.ReportId = model2Show.Id;
                return View(model2Show);
            }
        }

        public string getRelativePath(string absolutePath, bool etiquete = false)
        {
            string path;

            if (absolutePath != null)
            {
                if (etiquete == false)
                    path = "~/uploads/" + absolutePath.Split('\\').Last();
                else
                    path = "~/uploads/materials/" + absolutePath.Split('\\').Last();
            }
            else
                return null;

            return path;
        }

        [Authorize(Roles = "Investigator")]
        public IActionResult ReportDetailsUnaccepted(int? reportId)
        {
            if (reportId == null)
                return BadRequest();
            else
            {
                var report = ctx.Report.FirstOrDefault(x => x.Id == reportId);
                var eqId = ctx.Report.FirstOrDefault(x => x.Id == reportId).EquipmentId;
                var equipment = ctx.Equipment.FirstOrDefault(x => x.Id == eqId).Name;
                var areaId = ctx.Report.FirstOrDefault(x => x.Id == reportId).AreaId;
                var area = ctx.Area.FirstOrDefault(x => x.Id == areaId).Name;
                var pcID = ctx.Report.FirstOrDefault(x => x.Id == reportId).PhenomenaCategory;
                var phenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == pcID).Name;
                var qId = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == reportId).QuestionId;
                var question = ctx.Question.FirstOrDefault(x => x.Id == qId).Question1;


                MDisplayReport model2Show = new MDisplayReport
                {
                    Id = report.Id,
                    BatchSAP = report.BatchSap,
                    DateHour = report.DateHour,
                    BlockedBatch = report.BlockedBatch,
                    ImagePath = getRelativePath(report.ImagePath),
                    ImagePath1 = getRelativePath(report.ImagePath1),
                    ImagePath2 = getRelativePath(report.ImagePath2),
                    EtiqueteImage = getRelativePath(report.EtiqueteImagePath, true),
                    DownTime = report.DownTime,
                    Equipment = equipment,
                    GCAS = report.Gcas,
                    HasSample = report.Sample,
                    Line = area,
                    LineCoordinator = report.LineCoordinator,
                    NumberOfStops = report.NumberOfStops,
                    Originator = report.Originator,
                    PhenomenaCategory = phenomenaCategory,
                    PO = report.Po,
                    Problem = question,
                    Quantity = report.Quantity,
                    Reason = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Reason
                };
                ViewBag.ReportId = model2Show.Id;
                return View(model2Show);
            }
        }

        [Authorize(Roles = "Investigator")]
        public IActionResult DismissReport(int? reportId)
        {
            if (reportId == null)
                return RedirectToAction("Index", "Home");
            else
            {
                var departmentId = ctx.Report.FirstOrDefault(x => x.Id == reportId).DepartmentId;
                var suppliers = ctx.Supplier.Where(x => x.DepartmentId == departmentId).ToList();
                ViewBag.Supplier = new SelectList(suppliers, "Id", "Name");

                var Brands = ctx.Brand.Select(x => x.Name).ToList();
                ViewBag.Brands = new SelectList(Brands);

                var Size = new List<string>
                {
                    "180", "200", "225", "250", "270", "300", "360", "380", "400", "450", "500", "540", "550", "600", "650", "675", "700", "750"
                };
                ViewBag.Size = new SelectList(Size);

                var status = new List<string>
                {
                    "Work in Progress", "Closed", "Conform with tehnical standard"
                };
                ViewBag.Status = new SelectList(status, "Conform with tehnical standard");

                return View(new ReportDismissalModel { ReportId = Convert.ToInt32(reportId) });
            }
                
            //{

            //}
        }

        [HttpPost]
        public IActionResult DismissReport(ReportDismissalModel model)
        {
            var report = ctx.Report.FirstOrDefault(rep => rep.Id == Convert.ToInt32(model.ReportId));
            report.PendingApproval = false;
            ctx.Update(report);
            ctx.SaveChanges();

            ReportApproval repApp = new ReportApproval { ReportId = Convert.ToInt32(report.Id), Approved = false, Reason = model.Reason, ApprovalDate = DateTime.Now };
            ctx.ReportApproval.Add(repApp);
            ctx.SaveChanges();

            var test = _userManager.FindByNameAsync(User.Identity.Name).Result.FullName;
            Tracking trk = new Tracking
            {
                BrandSize = model.Brand + model.Size,
                Chronic = model.Chronic,
                BlockedMaterialDecision = model.BlockedMaterialDecision,
                ClaimedDt = report.DownTime,
                ClaimedOfStops = report.NumberOfStops,
                Comments = model.Comments,
                ConfirmedDt = new TimeSpan(model.Hours, model.Minutes, model.Seconds),
                ConfirmedOfStops = model.ConfirmedStops,
                DueDate = model.DueDate,
                Issue = model.Issue,
                LotProductionDate = model.LotProductionDate.Value,
                Owner = _userManager.FindByNameAsync(User.Identity.Name).Result.FullName,
                RepeteadIssue = model.RepeatedIssue,
                ReportId = report.Id,
                Status = model.Status,
                Supplier = Convert.ToInt32(model.Supplier)
            };
            ctx.Tracking.Add(trk);
            ctx.SaveChanges();

            return RedirectToAction("Investigator", "Report");
        }

        [Authorize(Roles = "Investigator")]
        public async Task<IActionResult> FinalReport(int? reportId)
        {
            var httpContext = _accesor.HttpContext;
            var test = httpContext.Request.Host;

            if (reportId == null)
                return BadRequest();
            else
            {
                var report = ctx.Report.FirstOrDefault(x => x.Id == reportId);

                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                var departmentId = ctx.Investigator2Department.FirstOrDefault(x => x.InvestigatorId == userId).DepartmentId;

                var status = new List<string>
                {
                    "Work in Progress", "Closed", "Conform with tehnical standard"
                };
                ViewBag.Status = new SelectList(status, "Work in Progress");

                var suppliers = ctx.Supplier.Where(x => x.DepartmentId == departmentId).ToList();
                ViewBag.Supplier = new SelectList(suppliers, "Id", "Name");

                var Brands = ctx.Brand.Select(x => x.Name).ToList();
                ViewBag.Brands = new SelectList(Brands);

                var Size = new List<string>
                {
                    "180", "200", "225", "250", "270", "300", "360", "380", "400", "450", "500", "540", "550", "600", "650", "675", "700", "750"
                };
                ViewBag.Size = new SelectList(Size);

                var qCategories = ctx.Qcategory.Where(x => x.DepartmentId == departmentId).ToList();
                var Categorylist = new SelectList(qCategories, "Id", "Name", report.PhenomenaCategory);
                ViewBag.CategoryList = Categorylist;

                return View(new MInvestigatorReport { ReportId = Convert.ToInt32(reportId) });
            }

        }


        [HttpPost]
        [Authorize(Roles = "Investigator")]
        public IActionResult FinalReport(MInvestigatorReport model)
        {
            if (ModelState.IsValid)
            {
                OutlookHandler meetingCreator = new OutlookHandler();
                if (model.AdditionalProblem != null)
                {
                    ctx.Question.Add(new Question
                    {
                        CategoryId = Convert.ToInt32(model.PhenomenaCategory),
                        IsActive = true,
                        Question1 = model.AdditionalProblem.ToUpper(),
                        QuestionEnglish = model.AdditionalProblemEnglish.ToUpper()
                    });
                    ctx.SaveChanges();

                    var report = ctx.Report.FirstOrDefault(x => x.Id == model.ReportId);
                    if (model.PhenomenaCategory != null)
                        report.PhenomenaCategory = Convert.ToInt32(model.PhenomenaCategory);
                    if (model.VendorBatch != null)
                        report.VendorBatch = model.VendorBatch;
                    if (model.BatchSAP != null)
                        report.BatchSap = model.BatchSAP;
                    if (model.BatchSAP != null)
                        report.BatchNo = model.BatchNo;
                    report.PendingApproval = false;
                    ctx.Report.Update(report);
                    ctx.SaveChanges();
                    if (model.PhenomenaCategory != null)
                    {
                        var question = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == report.Id);
                        question.QuestionId = ctx.Question.FirstOrDefault(x => x.Question1 == model.AdditionalProblem.ToUpper() && x.CategoryId == Convert.ToInt32(model.PhenomenaCategory)).Id;
                        ctx.Questions2Reports.Update(question);
                        ctx.SaveChanges();
                    }
                    ctx.ReportApproval.Add(new ReportApproval
                    {
                        Approved = true,
                        ReportId = report.Id,
                        Reason = model.InvestigationDetails,
                        ApprovalDate = DateTime.Now
                    });
                    ctx.SaveChanges();

                    Tracking trk = new Tracking
                    {
                        BrandSize = model.Brand + model.Size,
                        Chronic = model.Chronic,
                        BlockedMaterialDecision = model.BlockedMaterialDecision,
                        ClaimedDt = report.DownTime,
                        ClaimedOfStops = report.NumberOfStops,
                        Comments = model.Comments,
                        ConfirmedDt = new TimeSpan(model.Hours, model.Minutes, model.Seconds),
                        ConfirmedOfStops = model.ConfirmedStops,
                        DueDate = model.DueDate,
                        Issue = model.Issue,
                        LotProductionDate = model.LotProductionDate.Value,
                        Owner = _userManager.FindByNameAsync(User.Identity.Name).Result.FullName,
                        RepeteadIssue = model.RepeatedIssue,
                        ReportId = report.Id,
                        Status = model.Status,
                        Supplier = Convert.ToInt32(model.Supplier)
                    };
                    ctx.Tracking.Add(trk);
                    ctx.SaveChanges();

                    var investigator = _userManager.FindByNameAsync(User.Identity.Name).Result.Email;
                    meetingCreator.SendAppointment(report.IdentitifcationNumber, model.DueDate.Value, investigator);

                    return RedirectToAction("Investigator");
                }
                else
                {
                    var report = ctx.Report.FirstOrDefault(x => x.Id == model.ReportId);
                    if (model.PhenomenaCategory != null)
                        report.PhenomenaCategory = Convert.ToInt32(model.PhenomenaCategory);
                    if (model.VendorBatch != null)
                        report.VendorBatch = model.VendorBatch;
                    if (model.BatchSAP != null)
                        report.BatchSap = model.BatchSAP;
                    if (model.BatchSAP != null)
                        report.BatchNo = model.BatchNo;
                    report.PendingApproval = false;
                    ctx.Report.Update(report);
                    ctx.SaveChanges();
                    if (model.PhenomenaCategory != null)
                    {
                        var question = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == report.Id);
                        question.QuestionId = Convert.ToInt32(model.QuestionName);
                        ctx.Questions2Reports.Update(question);
                        ctx.SaveChanges();
                    }

                    ctx.ReportApproval.Add(new ReportApproval
                    {
                        Approved = true,
                        ReportId = report.Id,
                        ApprovalDate = DateTime.Now,
                        Reason = model.InvestigationDetails
                    });
                    ctx.SaveChanges();

                    Tracking trk = new Tracking
                    {
                        BrandSize = model.Brand + model.Size,
                        Chronic = model.Chronic,
                        BlockedMaterialDecision = model.BlockedMaterialDecision,
                        ClaimedDt = report.DownTime,
                        ClaimedOfStops = report.NumberOfStops,
                        Comments = model.Comments,
                        ConfirmedDt = new TimeSpan(model.Hours, model.Minutes, model.Seconds),
                        ConfirmedOfStops = model.ConfirmedStops,
                        DueDate = model.DueDate,
                        Issue = model.Issue,
                        LotProductionDate = model.LotProductionDate.Value,
                        Owner = _userManager.FindByNameAsync(User.Identity.Name).Result.FullName,
                        RepeteadIssue = model.RepeatedIssue,
                        ReportId = report.Id,
                        Status = model.Status,
                        Supplier = Convert.ToInt32(model.Supplier)
                    };
                    ctx.Tracking.Add(trk);
                    ctx.SaveChanges();

                    var investigator = _userManager.FindByNameAsync(User.Identity.Name).Result.Email;
                    meetingCreator.SendAppointment(report.IdentitifcationNumber, model.DueDate.Value, investigator);

                    return RedirectToAction("Investigator");
                }
            }
            else
                return View(model);
        }



        [Authorize(Roles = "Investigator")]
        public IActionResult SolvedReports()
        {
            var fReports = ctx.ReportApproval.ToList();
            List<InvestigatorListItem> model = new List<InvestigatorListItem>();
            foreach (var f in fReports)
            {
                var report = ctx.Report.FirstOrDefault(x => x.Id == f.ReportId);
                var qID = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == report.Id).QuestionId;
                model.Add(new InvestigatorListItem
                {
                    ReportId = report.Id,
                    IdentificationNumber = report.IdentitifcationNumber,
                    Date = report.DateHour,
                    Line = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                    PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                    Problem = ctx.Question.FirstOrDefault(x => x.Id == qID).Question1,
                    Approved = f.Approved == true ? "da" : "nu",
                    TrackingStatus = ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).Status
                });
            }

            return View(model.OrderByDescending(x => x.Date));
        }

        public JsonResult RetrieveReportInformationRo(int reportId)
        {
            var report = ctx.Report.FirstOrDefault(rep => rep.Id == reportId);
            jsReport report2Send = new jsReport
            {
                IdentificationNumber = report.IdentitifcationNumber,
                Date = report.DateHour.ToString("dd.MM.yyyy"),
                Originator = report.Originator,
                Equipment = ctx.Equipment.FirstOrDefault(x => x.Id == report.EquipmentId).Name,
                Area = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                LineCoordinator = report.LineCoordinator,
                DateHour = report.DateHour.ToString("dd.MM.yyyy HH:mm:ss"),
                Gcas = report.Gcas,
                BatchSap = report.BatchSap ?? "-",
                VendorBatch = report.VendorBatch ?? "-",
                Po = report.Po,
                Downtime = report.DownTime,
                IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                NumberOfStops = report.NumberOfStops,
                HasSample = report.Sample == true ? "da" : "nu",
                BlockedBatch = report.BlockedBatch == true ? "da" : "nu",
                BatchNo = report.BatchNo ?? "-",
                Quantity = report.Quantity,
                ImagePath = report.ImagePath,
                ImagePath1 = report.ImagePath1,
                ImagePath2 = report.ImagePath2,
                EtiqueteImagePath = report.EtiqueteImagePath,
                PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                Problem = ctx.Questions2Reports.Include("Question").FirstOrDefault(x => x.ReportId == report.Id).Question.Question1
            };

            report2Send.PhenomenaDescription = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Reason;

            return Json(report2Send);
        }

        public JsonResult RetrieveReportInformationEn(int reportId)
        {
            var report = ctx.Report.FirstOrDefault(rep => rep.Id == reportId);
            jsReport report2Send = new jsReport
            {
                IdentificationNumber = report.IdentitifcationNumber,
                Date = report.DateHour.ToString("dd.MM.yyyy"),
                Originator = report.Originator,
                Equipment = ctx.Equipment.FirstOrDefault(x => x.Id == report.EquipmentId).Name,
                Area = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                LineCoordinator = report.LineCoordinator,
                DateHour = report.DateHour.ToString("dd.MM.yyyy HH:mm:ss"),
                Gcas = report.Gcas,
                BatchSap = report.BatchSap ?? "-",
                VendorBatch = report.VendorBatch ?? "-",
                Po = report.Po,
                Downtime = report.DownTime,
                IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                NumberOfStops = report.NumberOfStops,
                HasSample = report.Sample == true ? "yes" : "no",
                BlockedBatch = report.BlockedBatch == true ? "yes" : "no",
                BatchNo = report.BatchNo ?? "-",
                Quantity = report.Quantity,
                ImagePath = report.ImagePath,
                ImagePath1 = report.ImagePath1,
                ImagePath2 = report.ImagePath2,
                EtiqueteImagePath = report.EtiqueteImagePath,
                PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).EnglishName,
                Problem = ctx.Questions2Reports.Include("Question").FirstOrDefault(x => x.ReportId == report.Id).Question.QuestionEnglish
            };

            report2Send.PhenomenaDescription = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Reason;

            return Json(report2Send);
        }

        public async Task<IActionResult> GenerateReport([FromServices] INodeServices nodeServices, int? reportId, string lang = "ro")
        {
            JsonResult data;
            if (lang == "ro")
            {
                data = RetrieveReportInformationRo(Convert.ToInt32(reportId));
            }
            else
            {
                if (lang == "en")
                    data = RetrieveReportInformationEn(Convert.ToInt32(reportId));
                else
                    return BadRequest();
            }
               

            try
            {
                byte[] result = new byte[100];
                if(lang == "ro")
                    result = await nodeServices.InvokeAsync<byte[]>("./ReportGenerator/documentGeneratorRo.js", data.Value);
                else
                    result = await nodeServices.InvokeAsync<byte[]>("./ReportGenerator/documentGeneratorEn.js", data.Value);


                string filename = @"report.pdf";
                HttpContext.Response.Headers.Add("x-filename", filename);
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
                HttpContext.Response.Headers.Add("Content-type", "application/pdf");
                HttpContext.Response.Body.Write(result, 0, result.Length);
                return new ContentResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        public async Task<FileResult> DownloadTest([FromServices] INodeServices nodeServices, int? reportId, string lang = null)
        {
            var data = RetrieveReportInformationRo(Convert.ToInt32(reportId));

            var result = new byte[100];
            try
            {
                result = await nodeServices.InvokeAsync<byte[]>("./ReportGenerator/documentGeneratorRo.js", data.Value);
            }
            catch (Exception e)
            {
                result = null;
            }
            var filename = "test.pdf";

            return File(result, "application/pdf", filename);
        }

        public IActionResult ReportCreator()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            ViewBag.IsAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;

            var department = ctx.Department.ToList();
            ViewBag.Departments = new SelectList(department, "Id", "Name");

            var status = new List<string>
                {
                    "Work in Progress", "Closed", "Conform with tehnical standard"
                };
            ViewBag.Status = new SelectList(status);

            var Issues = ctx.Tracking.Select(x => x.Issue).ToList();
            ViewBag.Issue = new SelectList(Issues);

            var MaterialBlockedDecisions = ctx.Tracking.Select(x => x.BlockedMaterialDecision).Distinct().ToList();
            ViewBag.MaterialBlockedCauses = new SelectList(MaterialBlockedDecisions);

            var Brand = ctx.Brand.ToList();
            ViewBag.Brand = new SelectList(Brand,"Id","Name");

            var Size = new List<string>
                {
                    "180", "200", "225", "250", "270", "300", "360", "380", "400", "450", "500", "540", "550", "600", "650", "675", "700", "750"
                };
            ViewBag.Size = new SelectList(Size);


            return View(new ReportCreatorModel { Reports = new List<ReportListItem>()});
        }

        [HttpPost]
        public IActionResult ReportCreator(ReportCreatorModel model)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            ViewBag.IsAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;

            var department = ctx.Department.ToList();
            ViewBag.Departments = new SelectList(department, "Id", "Name", model.Department);

            var status1 = new List<string>
                {
                    "Work in Progress", "Closed", "Conform with tehnical standard"
                };
            ViewBag.Status = new SelectList(status1);

            var Issues = ctx.Tracking.Select(x => x.Issue).ToList();
            ViewBag.Issue = new SelectList(Issues);

            var MaterialBlockedDecisions = ctx.Tracking.Select(x => x.BlockedMaterialDecision).Distinct().ToList();
            ViewBag.MaterialBlockedCauses = new SelectList(MaterialBlockedDecisions);

            var Brand = ctx.Brand.Select(x => x.Name).ToList();
            ViewBag.Brand = new SelectList(Brand);

            var Size = new List<string>
                {
                    "180", "200", "225", "250", "270", "300", "360", "380", "400", "450", "500", "540", "550", "600", "650", "675", "700", "750"
                };
            ViewBag.Size = new SelectList(Size);

            var reports = ctx.Report.Where(x => x.DateHour.CompareTo(model.BeginDate) >= 0 && x.DateHour.CompareTo(model.EndDate) <= 0).ToList();
            foreach (var report in reports.ToList())
                if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id) == null)
                    reports.Remove(report);

            if (model.GCAS != null)
                foreach (var report in reports.ToList())
                    if (report.Gcas != model.GCAS)
                        reports.Remove(report);

            if(model.Status != null)
                foreach(var report in reports.ToList())
                {
                    var status = ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).Status;
                    if (status != model.Status)
                        reports.Remove(report);
                }

            if (model.Issue != null)
                foreach (var report in reports.ToList())
                {
                    var issue = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name;
                    if (issue != model.Issue)
                        reports.Remove(report);
                }

            if (model.Suppplier != null)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).Supplier != Convert.ToInt32(model.Suppplier))
                        reports.Remove(report);

            if(model.Brand != null)
                foreach(var report in reports.ToList())
                {
                    if (model.Size == null)
                    {
                        if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).BrandSize.Contains(model.Brand) != true)
                            reports.Remove(report);
                }
                    else
                    {
                        var brandSize = model.Brand + model.Size;
                        if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).BrandSize != brandSize)
                            reports.Remove(report);
                    }
                }

            if (model.Line != null)
                foreach (var report in reports.ToList())
                    if (report.AreaId != model.Line)
                        reports.Remove(report);
            if (model.Approved != null)
                foreach (var report in reports.ToList())
                    if (ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved != Convert.ToBoolean(model.Approved))
                        reports.Remove(report);

            if (model.Repeated != null)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).RepeteadIssue != Convert.ToBoolean(model.Repeated))
                        reports.Remove(report);

            if (model.MaterialBlocked != null)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).BlockedMaterialDecision != model.MaterialBlocked)
                        reports.Remove(report);

            List<ReportListItem> newReportList = new List<ReportListItem>();
            foreach (var report in reports)
            {
                var questionId = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == report.Id).QuestionId;
                var supId = ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).Supplier;

                newReportList.Add(new ReportListItem
                {
                    IdentitficationNumber = report.IdentitifcationNumber,
                    IssuedDate = report.DateHour,
                    PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                    Problem = ctx.Question.FirstOrDefault(x => x.Id == questionId).Question1,
                    ReportId = report.Id,
                    Supplier = ctx.Supplier.FirstOrDefault(x => x.Id == supId).Name,
                    IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu"
                });
            }

            model.Reports = newReportList;

            return View(model);
        }

        public IActionResult Test()
        {
            var reports = ctx.Report.ToList();
            List<jsReport> list2send = new List<jsReport>();
            foreach (var report in reports)
            {
                list2send.Add(new jsReport
                {
                    Id = report.Id,
                    IdentificationNumber = report.IdentitifcationNumber,
                    Date = report.DateHour.ToString("dd.MM.yyyy"),
                    Originator = report.Originator,
                    Equipment = ctx.Equipment.FirstOrDefault(x => x.Id == report.EquipmentId).Name,
                    Area = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                    LineCoordinator = report.LineCoordinator,
                    DateHour = report.DateHour.ToString("dd.MM.yyyy HH:mm:ss"),
                    Gcas = report.Gcas,
                    BatchSap = report.BatchSap ?? "-",
                    VendorBatch = report.VendorBatch ?? "-",
                    Po = report.Po,
                    Downtime = report.DownTime,
                    IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                    NumberOfStops = report.NumberOfStops,
                    HasSample = report.Sample == true ? "da" : "nu",
                    BlockedBatch = report.BlockedBatch == true ? "da" : "nu",
                    BatchNo = report.BatchNo ?? "-",
                    Quantity = report.Quantity,
                    ImagePath = report.ImagePath,
                    ImagePath1 = report.ImagePath1,
                    ImagePath2 = report.ImagePath2,
                    EtiqueteImagePath = report.EtiqueteImagePath,
                    PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                    Problem = ctx.Questions2Reports.Include("Question").FirstOrDefault(x => x.ReportId == report.Id).Question.Question1
                });
            }
            return View(list2send);
        }

        public IActionResult Test1(string search)
        {
            return View();
        }

        public IActionResult IndexGrid(string search)
        {
            if (search == null)
            {
                var list2Send = new List<jsReport>();
                var reports = ctx.Report.ToList();
                foreach (var report in reports)
                {
                    list2Send.Add(new jsReport
                    {
                        Id = report.Id,
                        IdentificationNumber = report.IdentitifcationNumber,
                        Date = report.DateHour.ToString("dd.MM.yyyy"),
                        Originator = report.Originator,
                        Equipment = ctx.Equipment.FirstOrDefault(x => x.Id == report.EquipmentId).Name,
                        Area = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                        LineCoordinator = report.LineCoordinator,
                        DateHour = report.DateHour.ToString("dd.MM.yyyy HH:mm:ss"),
                        Gcas = report.Gcas,
                        BatchSap = report.BatchSap ?? "-",
                        VendorBatch = report.VendorBatch ?? "-",
                        Po = report.Po,
                        Downtime = report.DownTime,
                        IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                        NumberOfStops = report.NumberOfStops,
                        HasSample = report.Sample == true ? "da" : "nu",
                        BlockedBatch = report.BlockedBatch == true ? "da" : "nu",
                        BatchNo = report.BatchNo ?? "-",
                        Quantity = report.Quantity,
                        ImagePath = report.ImagePath,
                        ImagePath1 = report.ImagePath1,
                        ImagePath2 = report.ImagePath2,
                        EtiqueteImagePath = report.EtiqueteImagePath,
                        PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                        Problem = ctx.Questions2Reports.Include("Question").FirstOrDefault(x => x.ReportId == report.Id).Question.Question1
                    });
                }

                return PartialView("IndexGrid", list2Send);
            }

            else
            {
                var list2Send = new List<jsReport>();
                var reports = ctx.Report.ToList();
                foreach (var report in reports)
                {
                    list2Send.Add(new jsReport
                    {
                        Id = report.Id,
                        IdentificationNumber = report.IdentitifcationNumber,
                        Date = report.DateHour.ToString("dd.MM.yyyy"),
                        Originator = report.Originator,
                        Equipment = ctx.Equipment.FirstOrDefault(x => x.Id == report.EquipmentId).Name,
                        Area = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                        LineCoordinator = report.LineCoordinator,
                        DateHour = report.DateHour.ToString("dd.MM.yyyy HH:mm:ss"),
                        Gcas = report.Gcas,
                        BatchSap = report.BatchSap ?? "-",
                        VendorBatch = report.VendorBatch ?? "-",
                        Po = report.Po,
                        Downtime = report.DownTime,
                        IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                        NumberOfStops = report.NumberOfStops,
                        HasSample = report.Sample == true ? "da" : "nu",
                        BlockedBatch = report.BlockedBatch == true ? "da" : "nu",
                        BatchNo = report.BatchNo ?? "-",
                        Quantity = report.Quantity,
                        ImagePath = report.ImagePath,
                        ImagePath1 = report.ImagePath1,
                        ImagePath2 = report.ImagePath2,
                        EtiqueteImagePath = report.EtiqueteImagePath,
                        PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                        Problem = ctx.Questions2Reports.Include("Question").FirstOrDefault(x => x.ReportId == report.Id).Question.Question1
                    });
                }
                return PartialView("IndexGrid", list2Send.Where(x => x.IdentificationNumber.Contains(search) || x.Area.Contains(search) || x.DateHour.Contains(search) || x.PhenomenaCategory.Contains(search) || x.Problem.Contains(search) || x.IsRelevant.Contains(search)));
            }
        }
        
        public IActionResult EditTracking(int reportId)
        {
            var report = ctx.Report.FirstOrDefault(x => x.Id == reportId);

            var status = new List<string>
                {
                    "Work in Progress", "Closed", "Conform with tehnical standard"
                };
            ViewBag.Status = new SelectList(status, "Closed");

            if (report != null)
            {
                var supplierId = ctx.Tracking.FirstOrDefault(x => x.ReportId == reportId).Supplier;
                var tracking = ctx.Tracking.FirstOrDefault(x => x.ReportId == reportId);
                TrackingEditViewModel model = new TrackingEditViewModel
                {
                    IdentificationNumber = report.IdentitifcationNumber,
                    ReportId = reportId,
                    Supplier = ctx.Supplier.FirstOrDefault(x => x.Id == supplierId).Name,
                    MaterialType = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                    DueDate = tracking.DueDate.HasValue ? tracking.DueDate.Value.ToString("dd.MM.yyyy") : "-",
                    Issue = tracking.Issue
                };

                return View(model);
            }
            else
                return BadRequest();
        }

        [HttpPost]
        public IActionResult EditTracking(TrackingEditViewModel model)
        {
            var status = new List<string>
                {
                    "Work in Progress", "Closed", "Conform with tehnical standard"
                };
            ViewBag.Status = new SelectList(status, "Closed");

            if (ModelState.IsValid)
            {
                var tracking = ctx.Tracking.FirstOrDefault(x => x.ReportId == model.ReportId);

                tracking.Status = model.Status;
                tracking.ActionForClosingIssue = model.ActionsForClosing;
                tracking.Objects = model.Objects;
                tracking.Comments = model.Comments;

                ctx.Tracking.Update(tracking);
                ctx.SaveChanges();

                return RedirectToAction("SolvedReports");
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult TrackingDetails(int reportId)
        {
            var report = ctx.Report.FirstOrDefault(x => x.Id == reportId);

            if (report != null)
            {
                var tracking = ctx.Tracking.FirstOrDefault(x => x.ReportId == reportId);

                var model = new TrackingDetailsViewModel
                {
                    ActionsForClosing = tracking.ActionForClosingIssue ?? "-",
                    BlockedMaterialDecision = tracking.BlockedMaterialDecision ?? "-",
                    BrandSize = tracking.BrandSize,
                    Chronic = tracking.Chronic != true ? "nu" : "da",
                    ClaimedDT = report.DownTime,
                    ClaimedNrOfStops = report.NumberOfStops,
                    Comments = tracking.Comments ?? "-",
                    ComplaintConfirmed = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                    ConfirmedDT = tracking.ConfirmedDt ?? new TimeSpan(0, 0, 0),
                    ConfirmedNrOfStops = tracking.ConfirmedOfStops ?? 0,
                    DueDate = tracking.DueDate.HasValue ? tracking.DueDate.Value.ToString("dd.MM.yyyy") : "-",
                    GCAS = report.Gcas,
                    IdentificationNumber = report.IdentitifcationNumber,
                    Issue = tracking.Issue,
                    Line = ctx.Area.FirstOrDefault(x => x.Id == report.AreaId).Name,
                    LotProductionDate = tracking.LotProductionDate.ToString("dd.MM.yyyy"),
                    MaterialBlocked = report.BlockedBatch == true ? "da" : "nu",
                    MaterialType = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                    Month = GetMonth(report.DateHour.Month),
                    Objects = tracking.Objects ?? "-",
                    Owner = tracking.Owner,
                    PO = report.Po,
                    QuantityBlocked = report.Quantity,
                    Repeated = tracking.RepeteadIssue == true ? "da" : "nu",
                    ReportId = report.Id,
                    Status = tracking.Status,
                    Supplier = ctx.Supplier.FirstOrDefault(x => x.Id == tracking.Supplier).Name
                };

                return View(model);
            }
            else
                return BadRequest();
        }

        public string GetMonth(int monthNumber)
        {
            switch (monthNumber)
            {
                case 1:
                    return "Ianuarie";
                case 2:
                    return "Februarie";
                case 3:
                    return "Martie";
                case 4:
                    return "Aprilie";
                case 5:
                    return "Mai";
                case 6:
                    return "Iunie";
                case 7:
                    return "Iulie";
                case 8:
                    return "August";
                case 9:
                    return "Septembrie";
                case 10:
                    return "Octombrie";
                case 11:
                    return "Noiembrie";
                default:
                    return "-";
            }
        }

        public ActionResult _ReportGrid(DateTime beginDate, DateTime endDate, int department, int line, int supplier, string status, int? approved, int? repeated, string materialBlocked, int? issue, int? brand, string size, string gcas)
        {
            var reports = ctx.Report.Where(x => x.DateHour.CompareTo(beginDate) >= 0 && x.DateHour.CompareTo(endDate) <= 0).ToList();

            foreach (var report in reports.ToList())
                if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id) == null)
                    reports.Remove(report);

            if (department != 0)
                foreach (var report in reports.ToList())
                    if (report.DepartmentId != department)
                        reports.Remove(report);

            if (line != 0)
                foreach (var report in reports.ToList())
                    if (report.AreaId != line)
                        reports.Remove(report);

            if (supplier != 0)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).Supplier != supplier)
                        reports.Remove(report);

            if (status != null)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).Status != status)
                        reports.Remove(report);

            if (approved != null)
                foreach (var report in reports.ToList())
                    if (ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved != Convert.ToBoolean(approved.Value))
                        reports.Remove(report);

            if (repeated != null)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).RepeteadIssue != Convert.ToBoolean(repeated.Value))
                        reports.Remove(report);

            if (materialBlocked != null)
                foreach (var report in reports.ToList())
                    if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).BlockedMaterialDecision != materialBlocked)
                        reports.Remove(report);

            if (issue != null)
                foreach (var report in reports.ToList())
                    if (report.PhenomenaCategory != issue)
                        reports.Remove(report);

            if(brand != null)
                if(size != null)
                {
                    var brandSize = ctx.Brand.FirstOrDefault(x => x.Id == brand).Name + size;
                    foreach (var report in reports.ToList())
                        if (ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).BrandSize != brandSize)
                            reports.Remove(report);
                }
                else
                {
                    var branding = ctx.Brand.FirstOrDefault(x => x.Id == brand).Name;
                    foreach (var report in reports.ToList())
                        if (!ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id).BrandSize.Contains(branding))
                            reports.Remove(report);
                }

            if (gcas != null)
                foreach (var report in reports.ToList())
                    if (report.Gcas != gcas)
                        reports.Remove(report);

            var model = new List<ReportListItem>();
            foreach (var report in reports)
            {
                var questionId = ctx.Questions2Reports.FirstOrDefault(x => x.ReportId == report.Id).QuestionId;
                var tracking = ctx.Tracking.FirstOrDefault(x => x.ReportId == report.Id);
                model.Add(new ReportListItem
                {
                    IdentitficationNumber = report.IdentitifcationNumber,
                    IsRelevant = ctx.ReportApproval.FirstOrDefault(x => x.ReportId == report.Id).Approved == true ? "da" : "nu",
                    IssuedDate = report.DateHour,
                    PhenomenaCategory = ctx.Qcategory.FirstOrDefault(x => x.Id == report.PhenomenaCategory).Name,
                    ReportId = report.Id,
                    Supplier = ctx.Supplier.FirstOrDefault(x => x.Id == tracking.Supplier).Name,
                    Problem = ctx.Question.FirstOrDefault(x => x.Id == questionId).Question1
                });
            }

            return PartialView("_ReportGrid", model);
        }


    }

}