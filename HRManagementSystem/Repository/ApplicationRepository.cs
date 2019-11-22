using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class ApplicationRepository : DataRepositoryBase<Application, HumanResourceContext>
    {
        protected override DbSet<Application> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.ApplicationSet;
        }

        protected override Expression<Func<Application, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.AppId == id);
        }
    }
}