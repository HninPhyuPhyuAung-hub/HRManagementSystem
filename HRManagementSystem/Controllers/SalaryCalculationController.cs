using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class SalaryCalculationController : Controller
    {
        SalaryRepository salaryrepo = new SalaryRepository();
        // GET: SalaryCalculation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Salary()
        {
            return View();
        }

        public ActionResult CreatePaySlip(Salary fc, int page = 1, int pagesize = 10)
        {
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
            if (fc.salaryid == 0)
            {
                mymodel = salaryrepo.Add(fc);
            }
            else
            {
                mymodel = salaryrepo.Update(fc);
            }

            if (mymodel != null)
            {
                return View(mymodel);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SalaryCheck()
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.Employeeset.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Payslip(int EmpId)
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.SalaryCheckSet.Where(e => e.EmpId == EmpId).OrderByDescending(e => e.date).ToList();
            paymentjson data = new paymentjson();
            data.EmpId = result.Select(a => a.EmpId).ToArray();
            data.Name = result.Select(a => a.Name).ToArray();
            data.date = result.Select(a => a.date.Value.ToString("MMMM dd/ yyyy")).ToArray();
            data.ot = result.Select(a => a.ot).ToArray();
            data.bonus = result.Select(a => a.bonus).ToArray();
            data.totalallownace = result.Select(a => a.totalallownace).ToArray();
            data.total_deduction = result.Select(a => a.total_deduction).ToArray();
            data.basic = result.Select(a => a.basic).ToArray();
            data.net_salary = result.Select(a => a.net_salary).ToArray();
            data.status = result.Select(a => a.status).ToArray();
            data.salaryid = result.Select(a => a.salaryid).ToArray();
            data.Bankacc = result.Select(a => a.Bankacc).ToArray();
            data.unpaid = result.Select(a => a.unpaid).ToArray();
            data.hour = result.Select(a => a.hour).ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CalculateWeekend(DateTime? date, int EmpId)
        {
            int year = date.Value.Year;
            int month = date.Value.Month;
            int daysInMonth = 0;
            int days = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= days; i++)
            {
                DateTime day = new DateTime(year, month, i);
                if (day.DayOfWeek != DayOfWeek.Sunday && day.DayOfWeek != DayOfWeek.Saturday)
                {
                    daysInMonth++;
                }
            }
            HumanResourceContext context = new HumanResourceContext();
            var result = context.Employeeset.Find(EmpId).Salary;
            result = (result / daysInMonth) / 8;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindByDate(DateTime? date, int EmpId)
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.LeaveSet.Count(a => a.EmpId == EmpId && a.FromDate.Month == date.Value.Month && a.FromDate.Year == date.Value.Year && a.Reason == "Unpaid Leave");
            if (result != 0)
            {
                result = context.LeaveSet.Where(a => a.EmpId == EmpId && a.FromDate.Month == date.Value.Month && a.FromDate.Year == date.Value.Year && a.Reason == "Unpaid Leave").Sum(a => a.Days);
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult sendmail(int id)
        {
            HumanResourceContext context = new HumanResourceContext();
            leavedetail data = new leavedetail();
            Salary customer = context.SalaryCheckSet.FirstOrDefault(c => c.salaryid == id);
            var casual = 0;
            var paid = 0;
            var sick = 0;
            var unpaid = 0;
            var result1 = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Casual Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).FirstOrDefault();
            var result2 = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Paid Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).FirstOrDefault();
            var result3 = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Sick Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).FirstOrDefault();
            var result4 = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Unpaid Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).FirstOrDefault();
            if (result1 != null)
            {
                casual = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Casual Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).Sum(e => e.Days);
            }
            if (result2 != null)
            {
                paid = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Paid Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).Sum(e => e.Days);
            }
            if (result3 != null)
            {
                sick = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Sick Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).Sum(e => e.Days);
            }
            if (result4 != null)
            {
                unpaid = context.LeaveSet.Where(e => e.EmpId == customer.EmpId && e.Reason == "Unpaid Leave" && e.FromDate.Year == customer.date.Value.Year && e.ToDate.Month == customer.date.Value.Month).Sum(e => e.Days);
            }
            data.casual = casual;
            data.paid = paid;
            data.sick = sick;
            data.unpaid = unpaid;
            var email = context.Employeeset.Where(e => e.EmpId == customer.EmpId).Select(b => b.EmailAddress).FirstOrDefault();
            String renderedHTML = Controllers.PayslipController.RenderViewToString("Payslip", "SendPaySlip", customer);
            String textBody = Controllers.PayslipController.RenderViewToString1("Payslip", "LeaveStatement", data);
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("hninphyuphyuaung1994@gmail.com", "minhtet13579");
            string body = renderedHTML + textBody;
            using (var message = new MailMessage("hninphyuphyuaung1994@gmail.com", email))
            {
                message.Subject = "PaySlip";
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);
            }
            return View(customer);
        }
    }
}