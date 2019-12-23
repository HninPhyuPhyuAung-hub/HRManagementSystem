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
    public class MessageController : Controller
    {
        MessageRepository messrepo = new MessageRepository();

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

        // GET: Message
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Message()
        {
            MemberCookie mc = Getmember();
            ViewBag.Role = mc.Role;
            ViewBag.EmpId = mc.EmpId;
            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(Message m, FormCollection fc)
        {
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
                string[] email = context.Employeeset.Select(e => e.EmailAddress).ToArray();
                foreach (var item in email)
                {
                    droplist.Add(new SelectListItem { Text = item, Value = item });
                }
                return Json(droplist, JsonRequestBehavior.AllowGet);
            }
        }


    }
}