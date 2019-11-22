using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class DepartmentRepository : DataRepositoryBase<Departments, HumanResourceContext>
    {
        protected override DbSet<Departments> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.DepartmentSet;
        }

        protected override Expression<Func<Departments, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.Id == id);
        }
    }
}
   
