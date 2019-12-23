using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class ExpenseController : Controller
    {
        ExpenseRepository exprepo = new ExpenseRepository();
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
        // GET: Expense
        public ActionResult Index()
        {
            return View();
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

    }
}