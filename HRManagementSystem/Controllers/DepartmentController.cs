using HRManagementSystem.Models;
using HRManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        DepartmentRepository deprepo = new DepartmentRepository();
        // GET: Department
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
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

        public ActionResult DepartmentList()
        {
            HumanResourceContext context = new HumanResourceContext();
            List<DeptListViewModel> result = new List<DeptListViewModel>();
            DepartmentRepository deptrepo = new DepartmentRepository();
            EmployeeRepository emprepo = new EmployeeRepository();
            List<string> deptlist = deptrepo.Get().Select(a => a.DpName).ToList();
            foreach (var dept in deptlist)
            {
                DeptListViewModel dlvm = new DeptListViewModel();
                dlvm.Id = deprepo.Get().Where(a => a.DpName == dept).Select(a => a.Id).FirstOrDefault();
                dlvm.Department = dept;
                dlvm.Count = emprepo.Get().Where(a => a.Department == dept).Count();
                result.Add(dlvm);
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
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
    }
}