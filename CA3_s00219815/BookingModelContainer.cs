using CA3_s00219815;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace workwise_app
{
    public partial class BookingModelContainer : DbContext
    {
        public BookingModelContainer()
            : base("name=BookingModelContainer")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Workspace> Workspaces { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
