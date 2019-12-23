using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class NoticeBoardController : Controller
    {
        NoticeRepository notrepo = new NoticeRepository();
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
        // GET: NoticeBoard
        public ActionResult Index()
        {
            return View();
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

    }
}