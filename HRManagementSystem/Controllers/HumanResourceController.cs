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
        EmployeeManager empmgr;
        LeaveRepository emprepo = new LeaveRepository();
        SalaryRepository salaryrepo = new SalaryRepository();
        DepartmentRepository deprepo = new DepartmentRepository();
        RecircumentRepository repo = new RecircumentRepository();
        MessageRepository messrepo = new MessageRepository();
        ExpenseRepository exprepo = new ExpenseRepository();
        NoticeRepository notrepo = new NoticeRepository();
        AwardRepository awarepo = new AwardRepository();


        ApplicationRepository apprepo = new ApplicationRepository();
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
                image.SaveAs(HttpContext.Server.MapPath("~/Image/")
                                                      + image.FileName);
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
                    image.SaveAs(HttpContext.Server.MapPath("~/Image/")
                                                          + image.FileName);
                    employee.Photo = "~/Image/" + image.FileName;
                    employee.Photo = employee.Photo.Replace("~", string.Empty);
                }


                context.Entry(employee).State = EntityState.Modified;

                context.SaveChanges();
                return RedirectToAction("EmployeeList", "HumanResource");
            }
            return View(employee);
        }
        public ActionResult LeaveStaff()
        {

            MemberCookie mc = Getmember();
            int paid = 13;
            int casual = 6;
            int sick = 30;
            int EmpId = mc.EmpId;
            HumanResourceContext context = new HumanResourceContext();
            var result1 = context.LeaveSet.Where(e => e.EmpId == mc.EmpId && e.Reason == "Paid Leave").FirstOrDefault();
            if (result1 != null)
            {
                paid = context.LeaveSet.Where(e => e.EmpId == mc.EmpId && e.Reason == "Paid Leave").Sum(a => a.Days);
                paid = 13 - paid;
            }
            var result2 = context.LeaveSet.Where(e => e.EmpId == mc.EmpId && e.Reason == "Casual Leave").FirstOrDefault();
            if (result2 != null)
            {
                casual = context.LeaveSet.Where(e => e.EmpId == mc.EmpId && e.Reason == "Sick Leave").Sum(a => a.Days);
                casual = 6 - casual;
            }
            var result3 = context.LeaveSet.Where(e => e.EmpId == mc.EmpId && e.Reason == "Sick Leave").FirstOrDefault();
            if (result3 != null)
            {
                sick = context.LeaveSet.Where(e => e.EmpId == mc.EmpId && e.Reason == "Sick Leave").Sum(a => a.Days);
                sick = 30 - sick;
            }
            ViewBag.paid = paid;
            ViewBag.casual = casual;
            ViewBag.sick = sick;
            ViewBag.EmpId = EmpId;
            return View();
        }
        public ActionResult LeaveStaffList()
        {
            MemberCookie mc = Getmember();
            HumanResourceContext context = new HumanResourceContext();
            int[] total = new int[49];




            var result = context.LeaveSet.Where(e => e.EmpId == mc.EmpId).GroupBy(x => new { x.EmpId })
                .Select(g => new
                {
                    Days = g.Sum(p => p.Days),
                    Name = g.Select(b => b.Name).FirstOrDefault(),
                    Department = g.Select(b => b.Department).FirstOrDefault(),
                    Position = g.Select(b => b.Position).FirstOrDefault(),
                    Remain = 49 - g.Sum(p => p.Days),
                    Total = 49,
                    EmpId = g.Select(b => b.EmpId).FirstOrDefault()

                }).ToList();



            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            //var result = context.LeaveSet.Where(e=>e.EmpId==mc.EmpId).ToList();
            //return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendEmailToStaff(FormCollection form)
        {

            string leavetype = form["Reason"];
            string days = form["Days"];
            string leavereason = form["LeaveReason"];
            string Email = form["Email"];
            string Password = form["Password"];
            string fromdate = form["FromDate"];
            string todate = form["ToDate"];
            string EmpId = form["EmpId"];
            string ManagerEmail = form["ManagerEmail"];
            int id = Convert.ToInt32(EmpId);
            int day = Convert.ToInt32(days);
            int paid = 13;
            int casual = 6;
            int sick = 30;
            HumanResourceContext context = new HumanResourceContext();
            var result1 = context.LeaveSet.Where(e => e.EmpId == id && e.Reason == "Paid Leave").FirstOrDefault();
            if (result1 != null)
            {
                paid = context.LeaveSet.Where(e => e.EmpId == id && e.Reason == "Paid Leave").Sum(a => a.Days);
                paid = 13 - paid;
            }
            var result2 = context.LeaveSet.Where(e => e.EmpId == id && e.Reason == "Casual Leave").FirstOrDefault();
            if (result2 != null)
            {
                casual = context.LeaveSet.Where(e => e.EmpId == id && e.Reason == "Sick Leave").Sum(a => a.Days);
                casual = 6 - casual;
            }
            var result3 = context.LeaveSet.Where(e => e.EmpId == id && e.Reason == "Sick Leave").FirstOrDefault();
            if (result3 != null)
            {
                sick = context.LeaveSet.Where(e => e.EmpId == id && e.Reason == "Sick Leave").Sum(a => a.Days);
                sick = 30 - sick;
            }
            if (leavetype == "Paid Leave")
            {
                if (day > paid)
                {

                    ViewBag.Message = "You have not enough paid leave!";
                    ViewBag.leave = casual;
                    return View("LeaveStaff");
                }
            }
            else if (leavetype == "Sick Leave")
            {
                if (day > sick)
                {
                    ViewBag.Message = "You have not enough Sick leave!";
                    ViewBag.leave = sick;
                    return View("LeaveStaff");
                }
            }
            else if (leavetype == "Casual Leave")
            {
                if (day > casual)
                {
                    ViewBag.Message = "You have not enough casual leave!";
                    ViewBag.leave = casual;
                    return View("LeaveStaff");
                }
            }


            try
            {


                if (ModelState.IsValid)
                {
                    var senderEmail = Email;
                    var receiverEmail = ManagerEmail;
                    var password = Password;
                    var sub = "Leave Request";
                    var sendingmessage = "From" + fromdate;
                    var body1 = "To" + todate;
                    var body2 = leavereason;
                    var body = sendingmessage + body1 + body2;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = sub,
                        Body = body
                    })
                    {
                        mess.CC.Add("hninphyuphyuaung@koekoetech.com");
                        smtp.Send(mess);

                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }

        public ActionResult Leave()
        {
            return View();
        }
        public Action Result(string phonenumber)
        {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            PhoneNumber pn = phoneNumberUtil.Parse(phonenumber, "MM");
            string a = phoneNumberUtil.FormatNationalNumberWithCarrierCode(pn, "MM");
            return null;
        }
        public ActionResult LeaveForm(int LeaveId)
        {
            HumanResourceContext context = new HumanResourceContext();
            Leave result = new Leave();
            if (LeaveId == 0)
            {
                return PartialView("LeaveForm", result);
            }
            else
            {
                result = context.LeaveSet.Find(LeaveId);
                return PartialView("LeaveForm", result);
            }

        }
        public ActionResult UpdateLeave(Leave leave)

        {
            //LeaveId = 0;
            int sickleave = 30;
            int paidleave = 13;
            int cascualleave = 6;
            HumanResourceContext context = new HumanResourceContext();
            Leave updatevalue = null;
            var result = context.Employeeset.Where(e => e.Name == leave.Name && e.Position == leave.Position && e.Department == leave.Department).Select(e => e.EmpId).FirstOrDefault();

            if (result != 0)
            {
                if (leave.Reason == "Sick Leave")
                {
                    var sick = context.LeaveSet.Where(e => e.Reason == "Sick Leave").Select(e => e.Days).Sum();
                    var sick1 = sick + leave.Days;
                    var sick2 = 0;
                    if (sick1 <= sickleave)

                    {
                        leave.EmpId = result;
                        if (leave.LeaveId == 0)
                        {
                            updatevalue = emprepo.Add(leave);
                        }
                        else
                        {
                            updatevalue = emprepo.Update(leave);
                        }
                    }
                    else
                    {
                        sick2 = sickleave - sick;

                    }
                    if (updatevalue != null)
                    {
                        return Json("Success", JsonRequestBehavior.AllowGet);

                    }

                    else
                    {
                        return Json(sick2, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (leave.Reason == "Paid Leave")
                {
                    var paid2 = 0;
                    var paid = context.LeaveSet.Where(e => e.Reason == "Paid Leave").Select(e => e.Days).FirstOrDefault();
                    var paid1 = paid + leave.Days;
                    if (paid1 <= paidleave)

                    {
                        leave.EmpId = result;
                        if (leave.LeaveId == 0)
                        {
                            updatevalue = emprepo.Add(leave);
                        }
                        else
                        {
                            updatevalue = emprepo.Update(leave);
                        }
                    }
                    else
                    {
                        paid2 = paidleave - paid;

                    }
                    if (updatevalue != null)
                    {
                        return Json("Success", JsonRequestBehavior.AllowGet);

                    }

                    else
                    {
                        return Json(paid2, JsonRequestBehavior.AllowGet);
                    }
                }

                else if (leave.Reason == "Casual Leave")
                {
                    var cascual2 = 0;
                    var cascual = context.LeaveSet.Where(e => e.Reason == "Cascual Leave").Select(e => e.Days).FirstOrDefault();
                    var cascual1 = cascual + leave.Days;
                    if (cascual1 <= cascualleave)

                    {
                        leave.EmpId = result;
                        if (leave.LeaveId == 0)
                        {
                            updatevalue = emprepo.Add(leave);
                        }
                        else
                        {
                            updatevalue = emprepo.Update(leave);
                        }

                    }
                    else
                    {
                        cascual2 = cascualleave - cascual;
                        ViewBag.Message = cascual2;
                    }
                    if (updatevalue != null)
                    {
                        return Json("Success", JsonRequestBehavior.AllowGet);

                    }

                    else
                    {
                        return Json(cascual2, JsonRequestBehavior.AllowGet);
                    }
                }

                else
                {
                    leave.EmpId = result;
                    if (leave.LeaveId == 0)
                    {
                        // var shopowner = sorepo.GetShopOwner(so.ReferralPhoneNumber);
                        updatevalue = emprepo.Add(leave);
                    }
                    else
                    {
                        updatevalue = emprepo.Update(leave);
                    }




                    if (updatevalue != null)
                    {
                        return Json("Success", JsonRequestBehavior.AllowGet);

                    }

                    else
                    {
                        return Json("Fail", JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LeaveDetail()
        {
            int[] total = new int[49];
            HumanResourceContext context = new HumanResourceContext();
            var result = context.LeaveSet.GroupBy(x => new { x.EmpId })
                .Select(g => new
                {
                    Days = g.Sum(p => p.Days),
                    Name = g.Select(b => b.Name).FirstOrDefault(),
                    Department = g.Select(b => b.Department).FirstOrDefault(),
                    Position = g.Select(b => b.Position).FirstOrDefault(),
                    Remain = 49 - g.Sum(p => p.Days),
                    Total = 49,
                    EmpId = g.Select(b => b.EmpId).FirstOrDefault()

                }).ToList();



            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            //return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult detailchartdate(int EmpId, DateTime? fromdate = null, DateTime? todate = null, string key = null)
        {
            var result = new List<chartmodel>();
            fromdate = (fromdate == null) ? new DateTime(2019, 1, 1) : fromdate;
            todate = (todate == null) ? DateTime.UtcNow : todate;
            key = (key == null) ? key = "M" : key;
            // var aa = empmgr.getchart(EmpId, fromdate, todate, key);

            var param1 = new SqlParameter();
            param1.ParameterName = "@F";
            param1.SqlDbType = SqlDbType.DateTime;
            param1.SqlValue = fromdate;

            var param2 = new SqlParameter();
            param2.ParameterName = "@T";
            param2.SqlDbType = SqlDbType.DateTime;
            param2.SqlValue = todate;

            var param3 = new SqlParameter();
            param3.ParameterName = "@K";
            param3.SqlDbType = SqlDbType.VarChar;
            param3.SqlValue = key;

            HumanResourceContext context = new HumanResourceContext();
            var data = context.Database.SqlQuery<chartmodel>("LineChart @F,@T,@K", param1, param2, param3).Where(e => e.EmpID == EmpId).ToList();
            if (key == "Y")
            {
                var diff = todate.Value.Year - fromdate.Value.Year;
                if (diff > 0)
                {
                    int n = 0;
                    var comparedate = todate;
                    while (n <= diff)
                    {

                        chartmodel temp = new chartmodel();

                        int? c = data.Where(e => e.date.Value.Year == comparedate.Value.Year).Select(e => e.count).Sum();
                        temp.count = c ?? 0;
                        temp.date = comparedate;
                        result.Add(temp);
                        comparedate = comparedate.Value.AddYears(-1);
                        n++;
                    };
                }
                else
                {
                    chartmodel temp = new chartmodel();

                    int? c = data.Select(e => e.count).Sum();

                    temp.count = c ?? 0;
                    temp.EmpID = EmpId;
                    temp.date = todate;
                    result.Add(temp);

                }
            }
            else if (key == "M")
            {

                int diff = 12 * (todate.Value.Year - fromdate.Value.Year) + todate.Value.Month - fromdate.Value.Month;
                var diff1 = Math.Abs(diff);

                if (diff1 > 0)
                {
                    int n = 0;
                    var comparedate = todate;

                    while (n <= diff1)
                    {
                        chartmodel temp = new chartmodel();

                        int? c = data.Where(e => e.date.Value.Month == comparedate.Value.Month && e.date.Value.Year == comparedate.Value.Year).Select(e => e.count).Sum();
                        temp.count = c ?? 0;
                        temp.date = comparedate;
                        temp.EmpID = EmpId;
                        result.Add(temp);
                        comparedate = comparedate.Value.AddMonths(-1);
                        n++;
                    };
                }

                else
                {
                    chartmodel temp = new chartmodel();

                    int? c = data.Select(e => e.count).Sum();

                    temp.count = c ?? 0;
                    temp.date = todate;
                    result.Add(temp);

                }
            }
            result = result.OrderBy(e => e.date).ToList();
            chartjson chartdate = new chartjson();
            if (key == "Y")
            {
                chartdate.date = result.Select(e => e.date.Value.ToString("yyy")).ToArray();
            }
            else
            {
                chartdate.date = result.Select(e => e.date.Value.ToString("MMMM/yyyy")).ToArray();
            }
            chartdate.count = result.Select(e => e.count).ToArray();
            return Json(chartdate, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeaveDetailList(int EmpId)
        {

            HumanResourceContext context = new HumanResourceContext();
            List<Leave> result = null;

            result = context.LeaveSet.Where(e => e.EmpId == EmpId).OrderByDescending(e => e.FromDate).ToList();


            Leaejson data = new Leaejson();
            data.Name = result.Select(a => a.Name).ToArray();
            data.Department = result.Select(a => a.Department).ToArray();
            data.Position = result.Select(a => a.Position).ToArray();
            data.Reason = result.Select(a => a.Reason).ToArray();
            data.LeaveReason = result.Select(a => a.LeaveReason).ToArray();
            data.FromDate = result.Select(a => a.FromDate.ToString("MMMM dd/ yyyy")).ToArray();
            data.TodDate = result.Select(a => a.ToDate.ToString("MMMM dd/ yyyy")).ToArray();
            data.Days = result.Select(a => a.Days).ToArray();
            data.SupervisorProve = result.Select(a => a.SupervisorProve).ToArray();
            data.LeaveId = result.Select(a => a.LeaveId).ToArray();
            data.EmpId = result.Select(a => a.EmpId).ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salary()
        {
            return View();
        }
        public ActionResult SalaryCheck()
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.Employeeset.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
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
        public ActionResult CreatePaySlip(Salary fc, int page = 1, int pagesize = 10)
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
        //var basic = fc[0].ToString(); //user
        // var totalallownace = fc[1].ToString();
        // var total_deduction = fc[2].ToString();
        // var tax = fc[3].ToString();
        // var net_salary = fc[4].ToString();
        // var status = fc[5].ToString();
        // var EmpId = fc[6].ToString();
        // var unpaid = fc[7].ToString();
        // var hour = fc[8].ToString();
        // var date = fc[9].ToString();



        // myList.allowance = totalallownace;

        public ActionResult Paysliplist()
        {
            return View();
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

        public ActionResult Department()

        {
            return View();
        }
        public ActionResult DepartmentList()
        {
            HumanResourceContext context = new HumanResourceContext();

            //var result1 = (from person in context.Employeeset
            //               join pet in context.DepartmentSet on person.Department equals pet.DpName into gj
            //               from subpet in gj.DefaultIfEmpty()
            //               select new { subpet.DpName }
            //                 into g
            //               group g by g.DpName
            //               into grouped
            //               select new
            //               {
            //                   // Id = grouped.Select(e => e.Id).FirstOrDefault(),
            //                   Department = grouped.Select(e => e.DpName),
            //                   // Count = grouped.Select(e => e.EmpId).Count()

            //               }).ToList();



            //var result2 = (from e in context.DepartmentSet
            //              join s in context.Employeeset
            //on e.DpName equals s.Department
            //              select new
            //              {
            //                  Id = e.Id,
            //                  Department = e.DpName,
            //                  EmpId = s.EmpId
            //              }
            //               into g
            //              group g by g.Department
            //               into grouped
            //              select new
            //              {
            //                  Id = grouped.Select(e => e.Id).FirstOrDefault(),
            //                  Department = grouped.Select(e => e.Department).FirstOrDefault(),
            //                  Count = grouped.Select(e => e.EmpId).Count()

            //              }).ToList();
            List<DeptListViewModel> result = new List<DeptListViewModel>();
            DepartmentRepository deptrepo = new DepartmentRepository();
            EmployeeRepository emprepo = new EmployeeRepository();
            List<string> deptlist = deptrepo.Get().Select(a => a.DpName).ToList();
            foreach(var dept in deptlist)
            {
                DeptListViewModel dlvm = new DeptListViewModel();
                dlvm.Id = deprepo.Get().Where(a => a.DpName == dept).Select(a => a.Id).FirstOrDefault();
                dlvm.Department = dept;
                dlvm.Count = emprepo.Get().Where(a => a.Department == dept).Count();
                result.Add(dlvm);
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddDepartment(Departments deparment)
        {
            Departments updatedepartment = null;
            HumanResourceContext context = new HumanResourceContext();
            if (deparment.Id == 0)
            {
                updatedepartment = deprepo.Add(deparment);
            }
            else
            {
                updatedepartment = deprepo.Update(deparment);
            }

            return RedirectToAction("Department", "HumanResource");


        }
        public ActionResult DeleteDepartment(int DepId)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete Departments WHERE Id={0}", DepId);
                context.Database.ExecuteSqlCommand(sqlquery);
            }
            return RedirectToAction("Department", "HumanResource");
        }
        public ActionResult Vacancies()
        {
            return View();
        }
        public ActionResult AddVacancy(Recircument re)
        {
            Recircument updaterecircument = null;
            HumanResourceContext context = new HumanResourceContext();
            if (re.ReId == 0)
            {
                updaterecircument = repo.Add(re);
            }
            else
            {
                updaterecircument = repo.Update(re);
            }

            return RedirectToAction("Vacancies", "HumanResource");


        }
        public ActionResult VacancyList()
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.RecircumentSet.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteVacancy(int ReId)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete Recircument WHERE ReId={0}", ReId);
                context.Database.ExecuteSqlCommand(sqlquery);
            }
            return RedirectToAction("Vacancies", "HumanResource");
        }
        public ActionResult Application()
        {
            return View();
        }
        public ActionResult AddApplication(Application App)
        {
            Application updaterecircument = null;
            HumanResourceContext context = new HumanResourceContext();
            if (App.AppId == 0)
            {
                updaterecircument = apprepo.Add(App);
            }
            else
            {
                updaterecircument = apprepo.Update(App);
            }

            return RedirectToAction("Application", "HumanResource");


        }
        public ActionResult ApplicationList(string status = "")
        {
            List<Application> result = new List<Application>();
            HumanResourceContext context = new HumanResourceContext();
            if (status == "")
            {
                result = context.ApplicationSet.ToList();
            }
            else
            {
                result = context.ApplicationSet.Where(e => e.Status == status).ToList();
            }

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteApplication(int AppId)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete Application WHERE AppId={0}", AppId);
                context.Database.ExecuteSqlCommand(sqlquery);
            }
            return RedirectToAction("Application", "HumanResource");
        }
        public ActionResult Award()
        {
            MemberCookie mc = Getmember();
            ViewBag.memberRole = mc.Role;
            return View();
        }
        public ActionResult AwardList()
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.AwardSet.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteAward(int ID)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete Award WHERE ID={0}", ID);
                context.Database.ExecuteSqlCommand(sqlquery);
            }
            return RedirectToAction("Award", "HumanResource");
        }
        public ActionResult AddAward(Award award)
        {
            Award updateaward = null;
            HumanResourceContext context = new HumanResourceContext();
            if (award.ID == 0)
            {
                updateaward = awarepo.Add(award);
            }
            else
            {
                updateaward = awarepo.Update(award);
            }

            return RedirectToAction("Award", "HumanResource");


        }
        public ActionResult getEmployee()
        {
            HumanResourceContext context = new HumanResourceContext();
            List<SelectListItem> droplist = new List<SelectListItem>();

            string[] towns = context.Employeeset
                                               .Select(e => e.Name)

                                               .ToArray();
            foreach (var item in towns)
            {
                droplist.Add(new SelectListItem { Text = item, Value = item });
            }

            return Json(droplist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Expense()
        {
            MemberCookie mc = Getmember();
            ViewBag.memberRole = mc.Role;
            return View();
        }
        public ActionResult ExpenseList()
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.ExpenseSet.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteExpense(int ID)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete Expense WHERE ID={0}", ID);
                context.Database.ExecuteSqlCommand(sqlquery);
            }
            return RedirectToAction("Expense", "HumanResource");
        }
        public ActionResult AddExpense(Expense re)
        {
            Expense updateexpense = null;
            HumanResourceContext context = new HumanResourceContext();
            if (re.ID == 0)
            {
                updateexpense = exprepo.Add(re);
            }
            else
            {
                updateexpense = exprepo.Update(re);
            }

            return RedirectToAction("Expense", "HumanResource");


        }
        public ActionResult NoticeBoard()
        {
            MemberCookie mc = Getmember();
            ViewBag.memberRole = mc.Role;
            return View();
        }
        public ActionResult NoticeList()
        {
            HumanResourceContext context = new HumanResourceContext();
            var result = context.NoticeSet.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteNotice(int ID)
        {
            using (HumanResourceContext context = new HumanResourceContext())
            {
                var sqlquery = String.Format("Delete notice WHERE ID={0}", ID);
                context.Database.ExecuteSqlCommand(sqlquery);
            }
            return RedirectToAction("Expense", "HumanResource");
        }
        public ActionResult AddNotice(notice re)
        {
            notice updatenotice = null;
            HumanResourceContext context = new HumanResourceContext();
            if (re.ID == 0)
            {
                updatenotice = notrepo.Add(re);
            }
            else
            {
                updatenotice = notrepo.Update(re);
            }

            return RedirectToAction("NoticeBoard", "HumanResource");


        }
        public ActionResult Message()
        {
            MemberCookie mc = Getmember();
            ViewBag.Role = mc.Role;
            ViewBag.EmpId = mc.EmpId;
            return View();
        }
        public ActionResult getEmail(int EmpId, string Role)

        {
            HumanResourceContext context = new HumanResourceContext();
            if (Role != "Admin")
            {
                List<SelectListItem> droplist = new List<SelectListItem>();
                var result = context.Employeeset.Where(e => e.EmpId == EmpId).Select(e => e.Manager).FirstOrDefault();
                var department = context.Employeeset.Where(e => e.EmpId == EmpId).Select(e => e.Department).FirstOrDefault();
                var manageremail = context.Employeeset.Where(e => e.Name == result && e.Department == department && e.Role == "Manager").Select(e => e.EmailAddress).FirstOrDefault();
                var hremail = "hninphyuphyuaung@koekoetech.com";
                droplist.Add(new SelectListItem { Text = manageremail, Value = manageremail });
                droplist.Add(new SelectListItem { Text = hremail, Value = hremail });
                return Json(droplist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<SelectListItem> droplist = new List<SelectListItem>();

                string[] towns = context.Employeeset
                                                   .Select(e => e.EmailAddress)

                                                   .ToArray();
                foreach (var item in towns)
                {
                    droplist.Add(new SelectListItem { Text = item, Value = item });
                }

                return Json(droplist, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getDepartment()

        {
            HumanResourceContext context = new HumanResourceContext();
           
            {
                List<SelectListItem> droplist = new List<SelectListItem>();

                string[] department = context.DepartmentSet
                                                   .Select(e => e.DpName)

                                                   .ToArray();
                foreach (var item in department)
                {
                    droplist.Add(new SelectListItem { Text = item, Value = item });
                }

                return Json(droplist, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getName()

        {
            HumanResourceContext context = new HumanResourceContext();

            {
                List<SelectListItem> droplist = new List<SelectListItem>();

                string[] name = context.Employeeset
                                                   .Select(e => e.Name)

                                                   .ToArray();
                foreach (var item in name)
                {
                    droplist.Add(new SelectListItem { Text = item, Value = item });
                }

                return Json(droplist, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getPosition()

        {
            HumanResourceContext context = new HumanResourceContext();

            {
                List<SelectListItem> droplist = new List<SelectListItem>();

                string[] position = context.Employeeset
                                                   .Select(e => e.Position)

                                                   .ToArray();
                foreach (var item in position)
                {
                    droplist.Add(new SelectListItem { Text = item, Value = item });
                }

                return Json(droplist, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult SendMessage(Message m, FormCollection fc)
        {
            // string y = (TempData["Data1"]).ToString();
            string Role = fc["Role"];
            string Id = fc["EmpId"];
            string newpassword = fc["Password"];
            int EmpId = Convert.ToInt32(Id);
            HumanResourceContext context = new HumanResourceContext();
            context.MessageSet.Add(m);
            context.SaveChanges();
            var senderEmail1 = "";
            var staffpassword = "";
            if (Role != "Admin")
            {
                senderEmail1 = "hninphyuphyuaung1994@gmail.com";
                //senderEmail1 = context.Employeeset.Where(e => e.EmpId == EmpId).Select(a => a.EmailAddress).FirstOrDefault();
                staffpassword = newpassword;
            }
            else
            {
                senderEmail1 = "hninphyuphyuaung1994@gmail.com";
                staffpassword = "minhtet13579";
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = senderEmail1;
                    var receiverEmail = m.email;
                    var password = staffpassword;
                    var sub = m.subject;
                    var body = m.message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = sub,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View(m);
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }
        //public ActionResult PrintPartialViewToPdf(int id)
        //{
        //    HumanResourceContext db = new HumanResourceContext();

        //    var customer = db.SalaryCheckSet.FirstOrDefault(c => c.salaryid == id);
        //    return new Rotativa.ActionAsPdf("CreatePaySlip", customer)
        //    {
        //        FileName = Server.MapPath("~/Resume/payslip.pdf")
        //    };

        //    //return new ActionAsPdf("CreatePaySlip")
        //    //{
        //    //    FileName = Server.MapPath("/Resume/maymay.pdf")
        //    //};


        //    //    //}
        //    //    //    public ActionResult PrintPartialViewToPdf(int id)
        //    //    //{
        //    //    //    using (HumanResourceContext db = new HumanResourceContext())
        //    //    //    {
        //    //    //        Salary customer = db.SalaryCheckSet.FirstOrDefault(c => c.salaryid == id);

        //    //    //        var report = new PartialViewAsPdf("~/Views/HumanResource/CreatePaySlip.cshtml", customer);
        //    //    //        return report;
        //    //    //    }

        //    //    //}


        //    //    //[HttpPost]
        //    //    //[ValidateInput(false)]
        //    //    //public FileResult ExportPdf(string GridHtml)
        //    //    //{
        //    //    //    using (MemoryStream stream = new System.IO.MemoryStream())
        //    //    //    {
        //    //    //        StringReader sr = new StringReader(GridHtml);
        //    //    //        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
        //    //    //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
        //    //    //        pdfDoc.Open();
        //    //    //        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //    //    //        pdfDoc.Close();
        //    //    //        return File(stream.ToArray(), "application/pdf", "Grid.pdf");
        //    //    //    }
        //    //    //}
        //    //}

        //}
        //    [HttpPost]
        //    public ActionResult SendMessagetest(Message m)
        //    { 
        //    //var model = new UserModel() { Name = "Sarah", Email = "sarah@mail.example", IsPremiumUser = false };

        //    // Generate the email body from the template file.
        //    // 'templateFilePath' should contain the absolute path of your template file.
        //    var templateService = new TemplateService();
        //    var emailHtmlBody = templateService.Parse(File.ReadAllText(templateFilePath), model, null, null);
        //}
        //public ActionResult PrintPartialViewToPdftest(int id)
        //{
        //    HumanResourceContext db = new HumanResourceContext();

        //    var customer = db.SalaryCheckSet.FirstOrDefault(c => c.salaryid == id);

        //    string HtmlString = ConvertViewToString("~/Views/HumanResource/CreatePaySlip.cshtml");

        //    var senderEmail = "hninphyuphyuaung1994@gmail.com";
        //    var receiverEmail = "hninphyuphyuaung1994@gmail.com";
        //    var password = "HninPhyu216949";
        //    //   var sub = m.subject;
        //    var body = HtmlString;
        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(senderEmail, password)
        //    };
        //    using (var mess = new MailMessage(senderEmail, receiverEmail)
        //    {
        //        //Subject = sub,
        //        Body = body
        //    })
        //    {
        //        smtp.Send(mess);
        //    }
        //    return null;
        //}




        //private string ConvertViewToString(string viewName)
        //{
        //    ///ViewData.Model = model;
        //    using (StringWriter writer = new StringWriter())
        //    {
        //        ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        //        ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
        //        vResult.View.Render(vContext, writer);
        //        return writer.ToString();
        //    }
        //}
        public static string RenderViewToString(string controllerName, string viewName, Salary viewData)
        {
            // controllerName = "HumanResource";
            // viewName = "CreatePaySlip";
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new HumanResourceController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();

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
        public static string RenderViewToString1(string controllerName, string viewName, leavedetail viewData)
        {
            // controllerName = "HumanResource";
            // viewName = "CreatePaySlip";
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new HumanResourceController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewName, "", false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();

            }
        }
        public ActionResult LeaveStatement(leavedetail viewData)
        {
            leavedetail data = new leavedetail();
            HumanResourceContext context = new HumanResourceContext();

            return View(viewData);
        }
        public ActionResult sendmail(int id)
        {
            HumanResourceContext context = new HumanResourceContext();
            leavedetail data = new leavedetail();
            Salary customer = context.SalaryCheckSet.FirstOrDefault(c => c.salaryid == id);

            //    var  result = db.LeaveSet
            //           .Where(e => e.EmpId == customer.EmpId)
            //   .GroupBy(e => e.Reason)
            //.Select(cl => new LeaveResult
            //{
            //    leavetype = cl.Select(a => a.Reason).FirstOrDefault(),

            //    total = cl.Sum(a => a.Days),
            // }).FirstOrDefault();


            //      data.casual = result[0].total;
            //      data.paid = result[1].total;
            //      data.sick = result[2].total;
            //      data.unpaid = result[3].total;

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
            String renderedHTML = Controllers.HumanResourceController.RenderViewToString("HumanResource", "SendPaySlip", customer);
            String textBody = Controllers.HumanResourceController.RenderViewToString1("HumanResource", "LeaveStatement", data);

            //textBody += "</table>";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("hninphyuphyuaung1994@gmail.com", "minhtet13579");

            string body = renderedHTML + textBody;
            //body = textBody;

            using (var message = new MailMessage("hninphyuphyuaung1994@gmail.com", email))
            {
                message.Subject = "PaySlip";
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);
            }

            return View(customer);
        }

        //public ActionResult sendmail3()
        //{
        //    string textBody = "<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>Inventory Item</b></td> <td> <b> Required Qunatity </b> </td></tr>";

        //    textBody += "</table>";
        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Host = "smtp.gmail.com";
        //    smtp.Port = 587;
        //    smtp.EnableSsl = true;
        //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = new NetworkCredential("hninphyuphyuaung1994@gmail.com", "HninPhyu216949");

        //    string body = textBody;

        //    using (var message = new MailMessage("hninphyuphyuaung1994@gmail.com", "hninphyuphyuaung1994@gmail.com"))
        //    {
        //        message.Subject = "Test";
        //        message.Body = body;
        //        message.IsBodyHtml = true;
        //        smtp.Send(message);
        //    }

        //    return null;
        //}
    }

    public class DeptListViewModel
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public int Count { get; set; }
    }
}





