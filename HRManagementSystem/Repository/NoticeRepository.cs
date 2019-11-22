using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class NoticeRepository : DataRepositoryBase<notice, HumanResourceContext>
    {
        protected override DbSet<notice> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.NoticeSet;
        }

        protected override Expression<Func<notice, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.ID == id);
        }
    }
}

