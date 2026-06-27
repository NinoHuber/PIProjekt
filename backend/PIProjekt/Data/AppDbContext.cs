using Microsoft.EntityFrameworkCore;
using PIProjekt.Models;

namespace PIProjekt.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        public DbSet<PartnerCategory> PartnerCategories { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<WeddingPartner> WeddingPartners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeddingPartner>()
                .HasKey(wp => new { wp.WeddingId, wp.PartnerId });

            modelBuilder.Entity<WeddingPartner>()
                .HasOne(wp => wp.Wedding)
                .WithMany(w => w.WeddingPartners)
                .HasForeignKey(wp => wp.WeddingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WeddingPartner>()
                .HasOne(wp => wp.Partner)
                .WithMany(p => p.WeddingPartners)
                .HasForeignKey(wp => wp.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partner>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Wedding>()
                .HasIndex(w => w.WeddingDate);
        }
    }
}