using HRManagementSystem.Manager;
using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using RazorEngine.Templating;
using System.Web.Routing;
using PhoneNumbers;

namespace HRManagementSystem.Controllers
{

    public class HumanResourceController : Controller
    {
       
        public MemberCookie Getmember()
        {
            MemberCookie mc = new MemberCookie();
            if (Request.Cookies["hrCookie"] != null)
            {
                mc.MemberID = Convert.ToInt32(Request.Cookies["hrCookie"]["AdminID"]);
                mc.Role = Request.Cookies["hrCookie"]["Role"];
                mc.EmpId = Convert.ToInt32(Request.Cookies["hrCookie"]["EmpId"]);
            }
            return mc;
        }

        public ActionResult StartView()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Employees employee, HttpPostedFileBase image, HttpPostedFileBase Resume)
        {
            HumanResourceContext context = new HumanResourceContext();
            if (Resume!=null)
            {
                Resume.SaveAs(Path.Combine(Server.MapPath("~/Resume"), Resume.FileName));
                employee.Resume = "~/Resume/" + Resume.FileName;
            }
            if (image != null)
            {
                image.SaveAs(HttpContext.Server.MapPath("~/Image/")+image.FileName);
                employee.Photo = "~/Image/" + image.FileName;
            }
            employee.Resume = employee.Resume.Replace("~", string.Empty);
            employee.Photo = employee.Photo.Replace("~", string.Empty);
            context.Employeeset.Add(employee);
            context.SaveChanges();
            ModelState.Clear();
            return View("Detail", employee);
        }


        public ActionResult Detail(int EmpId)
        {
            HumanResourceContext context = new HumanResourceContext();
            Employees result = context.Employeeset.Find(EmpId);
            return View(result);
        }

        public ActionResult EmployeeList(int page = 1, int pagesize = 10)
        {
            HumanResourceContext context = new HumanResourceContext();
            var model = new EmployeePaging();
            Employees[] employeelist = context.Employeeset.ToArray();
            var totalcount = employeelist.Count();
            var totalpage = (int)Math.Ceiling((double)totalcount / pagesize);
            var pagedList = new StaticPagedList<Employees>(employeelist, page, pagesize, totalcount);
            model.employeeList = pagedList;
            model.TotalCount = totalcount;
            model.TotalPages = totalpage;
            return View(model);
        }

        public ActionResult Download(string FileName)
        {
            var FileVirtualPath = "~" + FileName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public ActionResult DeleteEmployee(int EmpId)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete Employees WHERE EmpId={0}", EmpId);
                var sqlquery1 = String.Format("Delete Account WHERE EmpId={0}", EmpId);
                context.Database.ExecuteSqlCommand(sqlquery);
                context.Database.ExecuteSqlCommand(sqlquery1);
            }
            //return Json(new { result = "Redirect", url = Url.Action("EmployeeList", "HumanResource") });
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? EmpId)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                if (EmpId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employees employee = context.Employeeset.Find(EmpId);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpId,Name,MiddleName,LastName,PhoneNumber,EmailAddress,Salary,IsContract,Photo,Department,Resume,Address,Sex,MaritalStatus,StartDate,Education,NRC,Manager,Birthday,SpousePh,SpouseName,Position,ContactName,ContactPh")] Employees employee, HttpPostedFileBase image, HttpPostedFileBase Resume)
        {
            if (ModelState.IsValid)
            {
                HumanResourceContext context = new HumanResourceContext();
                if (Resume != null)
                {
                    Resume.SaveAs(Path.Combine(Server.MapPath("~/Resume"), Resume.FileName));
                    employee.Resume = "~/Resume/" + Resume.FileName;
                    employee.Resume = employee.Resume.Replace("~", string.Empty);
                }
                if (image != null)
                {
                    image.SaveAs(HttpContext.Server.MapPath("~/Image/")+image.FileName);
                    employee.Photo = "~/Image/" + image.FileName;
                    employee.Photo = employee.Photo.Replace("~", string.Empty);
                }
                context.Entry(employee).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("EmployeeList", "HumanResource");
            }
            return View(employee);
        }
   
        public ActionResult PaymentForm(int EmpId, decimal salary, string Position, int payId)
        {
            HumanResourceContext context = new HumanResourceContext();
            SalaryCheck result = new SalaryCheck();
            if (payId == 0)
            {
                return PartialView("PaymentForm", result);
            }
            else
            {
                result = context.SalarySet.Find(payId);
                return PartialView("PaymentForm", result);
            }
        }

        public ActionResult Dashboard()
        {
            MemberCookie pc = Getmember();
            ViewBag.Organization = pc.MemberID;
            return View();
        }

        public ActionResult totalemployee()
        {
            DateTime? fromdate = DateTime.UtcNow.Date;
            HumanResourceContext context = new HumanResourceContext();
            var totalemployee = context.Employeeset.Select(e => e.EmpId).Count();
            var leave = context.LeaveSet.Count(e => e.FromDate == fromdate);
            var present = totalemployee - leave;
            TotalEmployee data = new TotalEmployee();
            data.totalemployee = totalemployee;
            data.leave = leave;
            data.present = present;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }

    public class DeptListViewModel
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public int Count { get; set; }
    }
}





