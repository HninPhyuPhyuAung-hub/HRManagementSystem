using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class HumanResourceContext: DbContext
    {
        public HumanResourceContext(): base("name=hr")
        {
            Database.SetInitializer<HumanResourceContext>(null);
        }
        public DbSet<Employees> Employeeset { get; set; }
        public DbSet<Departments> DepartmentSet { get; set; }
       public DbSet<Leave> LeaveSet { get; set; }
        public DbSet<SalaryCheck> SalarySet { get; set; }
        public DbSet<Salary> SalaryCheckSet { get; set; }
        public DbSet<Recircument> RecircumentSet { get; set; }
        public DbSet<Application> ApplicationSet { get; set; }
        public DbSet<Award> AwardSet { get; set; }
        public DbSet<Expense> ExpenseSet { get; set; }
        public DbSet<notice> NoticeSet { get; set; }
        public DbSet<Message> MessageSet { get; set; }
        public DbSet<Account> AccountSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           // modelBuilder.Ignore<ExtensionDataObject>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Employees>().HasKey<int>(e => e.EmpId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Departments>().HasKey<int>(e => e.Id).Ignore(e => e.EntityId);
            modelBuilder.Entity<Leave>().HasKey<int>(e => e.LeaveId).Ignore(e => e.EntityId);
            modelBuilder.Entity<SalaryCheck>().HasKey<int>(e => e.payId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Salary>().HasKey<int>(e => e.salaryid).Ignore(e => e.EntityId);
            modelBuilder.Entity<Recircument>().HasKey<int>(e => e.ReId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Application>().HasKey<int>(e => e.AppId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Award>().HasKey<int>(e => e.ID).Ignore(e => e.EntityId);
            modelBuilder.Entity<Expense>().HasKey<int>(e => e.ID).Ignore(e => e.EntityId);
            modelBuilder.Entity<notice>().HasKey<int>(e => e.ID).Ignore(e => e.EntityId);
            modelBuilder.Entity<Message>().HasKey<int>(e => e.ID).Ignore(e => e.EntityId);
            modelBuilder.Entity<Account>().HasKey<int>(e => e.ID).Ignore(e => e.EntityId);
        }
    }
}