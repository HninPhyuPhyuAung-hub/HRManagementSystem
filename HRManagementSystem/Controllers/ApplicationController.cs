using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class ApplicationController : Controller
    {

        ApplicationRepository apprepo = new ApplicationRepository();
        // GET: Application
        public ActionResult Index()
        {
            return View();
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
    }
}