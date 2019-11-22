using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class EmployeeRepository : DataRepositoryBase<Employees, HumanResourceContext>
    {
        protected override DbSet<Employees> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.Employeeset;
        }

        protected override Expression<Func<Employees, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.EmpId == id);
        }
    }
}