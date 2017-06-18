using DotNetNancy.Model;
using NextGear.Generic.Data.Core;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Security.Policy;


namespace DotNetNancy.GeneralApps.Generic.DataLayer
{
    public partial class AuthenticationContextExample : ContextBase
    {
        public AuthenticationContextExample()
            : base("name=AuthenticationEntities")
        {
            Database.SetInitializer<AuthenticationContextExample>(null);
            Database.CommandTimeout = 3 * 60; // 3 minutes
            Configuration.UseDatabaseNullSemantics = true;
            Configuration.ProxyCreationEnabled = false;
            //comment this out if you don't want to see the sql in the debugger output for all things executed in this context
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            //NH if you want all the sql to be logged as debug statements to the log4net log file uncomment the following
            //if (Log.IsDebugEnabled)
            //{
            //    Database.Log = s => Log.Debug(s);
            //}
        }

        public virtual DbSet<AuthorizedUser> AuthorizedUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}

