using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Club_X_International.Models
{
    public class DataContext: IdentityDbContext<ApplicationUser>
    {
        public DataContext():base("ClubX", throwIfV1Schema: false) { }

        public static DataContext Create()
        {
            return new DataContext();
        }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}