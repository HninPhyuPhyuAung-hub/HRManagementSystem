using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class ExpenseRepository : DataRepositoryBase<Expense, HumanResourceContext>
    {
        protected override DbSet<Expense> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.ExpenseSet;
        }

        protected override Expression<Func<Expense, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.ID == id);
        }
    }
}

