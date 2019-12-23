using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HRManagementSystem.Controllers
{
    public class PayslipController : Controller
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
        // GET: Payslip
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Paysliplist()
        {
            return View();
        }

        public ActionResult Pay()
        {
            HumanResourceContext context = new HumanResourceContext();
            MemberCookie mc = Getmember();
            if (mc.Role == "Admin")
            {
                var result = context.SalaryCheckSet.ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = context.SalaryCheckSet.Where(e => e.EmpId == mc.EmpId).ToList();
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SendPaySlip(Salary fc, int page = 1, int pagesize = 10)
        {
            // dynamic mymodel = new ExpandoObject();
            Salary mymodel = new Salary();
            HumanResourceContext context = new HumanResourceContext();
            var casual = 0;
            var paid = 0;
            var sick = 0;
            var unpaid = 0;
            var result1 = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Casual Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).FirstOrDefault();
            var result2 = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Paid Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).FirstOrDefault();
            var result3 = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Sick Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).FirstOrDefault();
            var result4 = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Unpaid Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).FirstOrDefault();
            if (result1 != null)
            {
                casual = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Casual Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).Sum(e => e.Days);
            }
            if (result2 != null)
            {
                paid = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Paid Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).Sum(e => e.Days);
            }
            if (result3 != null)
            {
                sick = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Sick Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).Sum(e => e.Days);
            }
            if (result4 != null)
            {
                unpaid = context.LeaveSet.Where(e => e.EmpId == fc.EmpId && e.Reason == "Unpaid Leave" && e.FromDate.Year == fc.date.Value.Year && e.ToDate.Month == fc.date.Value.Month).Sum(e => e.Days);
            }
            var position = context.Employeeset.Where(e => e.EmpId == fc.EmpId).Select(e => e.Position).FirstOrDefault();
            ViewBag.casual = casual;
            ViewBag.paid = paid;
            ViewBag.sick = sick;
            ViewBag.unpaid = unpaid;
            ViewBag.position = position;
            return View(mymodel);
        }

        public ActionResult LeaveStatement(leavedetail viewData)
        {
            leavedetail data = new leavedetail();
            HumanResourceContext context = new HumanResourceContext();
            return View(viewData);
        }
        public static string RenderViewToString(string controllerName, string viewName, Salary viewData)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new PayslipController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);
                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

        public static string RenderViewToString1(string controllerName, string viewName, leavedetail viewData)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new PayslipController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);
                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }
    }
}