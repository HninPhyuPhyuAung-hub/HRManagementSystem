using HRManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HRManagementSystem.Repository
{
    public class MessageRepository : DataRepositoryBase<Message, HumanResourceContext>
    {
        protected override DbSet<Message> DbSet(HumanResourceContext entityContext)
        {
            return entityContext.MessageSet;
        }

        protected override Expression<Func<Message, bool>> IdentifierPredicate(HumanResourceContext entityContext, int id)
        {
            return (e => e.ID == id);
        }
    }
}

