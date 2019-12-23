using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class RecruitmentController : Controller
    {
        RecircumentRepository repo = new RecircumentRepository();
        // GET: Recruitment
        public ActionResult Index()
        {
            return View();
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
    }
}