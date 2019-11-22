using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class SalaryRepository : DataRepositoryBase<Salary, HumanResourceContext>
    {
        protected override DbSet<Salary> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.SalaryCheckSet;
        }

        protected override Expression<Func<Salary, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.salaryid== id);
        }
    }
}
