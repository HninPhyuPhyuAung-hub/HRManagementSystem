using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class LeaveRepository : DataRepositoryBase<Leave, HumanResourceContext>
    {
        protected override DbSet<Leave> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.LeaveSet;
        }

        protected override Expression<Func<Leave, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.LeaveId == id);
        }
    }
}
