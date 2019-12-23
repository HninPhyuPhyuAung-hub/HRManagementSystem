using HRManagementSystem.Manager;
using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class LeaveController : Controller
    {
        EmployeeManager empmgr;
        LeaveRepository emprepo = new LeaveRepository();

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
        // GET: Leave
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Leave()
        {
            return View();
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

        public ActionResult UpdateLeave(Leave leave)
        {
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
       
        public ActionResult detailchartdate(int EmpId, DateTime? fromdate = null, DateTime? todate = null, string key = null)
        {
            var result = new List<chartmodel>();
            fromdate = (fromdate == null) ? new DateTime(2019, 1, 1) : fromdate;
            todate = (todate == null) ? DateTime.UtcNow : todate;
            key = (key == null) ? key = "M" : key;

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

        public ActionResult getDepartment()
        {
            HumanResourceContext context = new HumanResourceContext();
            {
                List<SelectListItem> droplist = new List<SelectListItem>();
                string[] department = context.DepartmentSet
                                                   .Select(e => e.DpName).ToArray();
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
                string[] name = context.Employeeset.Select(e => e.Name).ToArray();
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
                string[] position = context.Employeeset.Select(e => e.Position).ToArray();
                foreach (var item in position)
                {
                    droplist.Add(new SelectListItem { Text = item, Value = item });
                }
                return Json(droplist, JsonRequestBehavior.AllowGet);
            }
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

    }
}