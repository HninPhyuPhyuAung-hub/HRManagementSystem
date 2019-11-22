using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class RecircumentRepository : DataRepositoryBase<Recircument, HumanResourceContext>
    {
        protected override DbSet<Recircument> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.RecircumentSet;
        }

        protected override Expression<Func<Recircument, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.ReId == id);
        }
    }
}
    