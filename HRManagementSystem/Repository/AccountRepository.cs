using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class AccountRepository : DataRepositoryBase<Account, HumanResourceContext>
    {
        protected override DbSet<Account> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.AccountSet;
        }

        protected override Expression<Func<Account, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.ID == id);
        }
    }
}
  