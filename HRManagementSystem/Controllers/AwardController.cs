using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class AwardController : Controller
    {
        AwardRepository awarepo = new AwardRepository();

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
        // GET: Award
        public ActionResult Index()
        {
            return View();
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
            return RedirectToAction("Award", "Award");
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
            return RedirectToAction("Award", "Award");
        }

        public ActionResult getEmployee()
        {
            HumanResourceContext context = new HumanResourceContext();
            List<SelectListItem> droplist = new List<SelectListItem>();
            string[] towns = context.Employeeset.Select(e => e.Name).ToArray();
            foreach (var item in towns)
            {
                droplist.Add(new SelectListItem { Text = item, Value = item });
            }
            return Json(droplist, JsonRequestBehavior.AllowGet);
        }

    }
}