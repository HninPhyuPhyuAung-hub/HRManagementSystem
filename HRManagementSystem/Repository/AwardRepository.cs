using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class AwardRepository : DataRepositoryBase<Award, HumanResourceContext>
    {
        protected override DbSet<Award> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.AwardSet;
        }

        protected override Expression<Func<Award, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.ID == id);
        }
    }
}

