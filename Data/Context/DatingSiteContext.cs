using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Enitities;
namespace Data.Context
{
    public class DatingSiteContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Timers> Timers { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<CallRecord> CallRecords { get; set; }
        public DbSet<FavouriteCamGirl> FavouriteCamGirls { get; set; }

        public DatingSiteContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CallRecord>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.CallRecords)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CallRecord>()
                .HasOne(cr => cr.Camgirl)
                .WithMany()
                .HasForeignKey(cr => cr.CamgirlId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
