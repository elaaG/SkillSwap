using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSwap.API.Models;

namespace SkillSwap.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingSkill> ListingSkills { get; set; }
        public DbSet<ListingAvailability> ListingAvailabilities { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallets");
                
                entity.HasIndex(e => e.UserId)
                      .IsUnique()
                      .HasDatabaseName("IX_Wallets_UserId_Unique");
                
                entity.HasOne(w => w.User)
                      .WithOne(u => u.Wallet)
                      .HasForeignKey<Wallet>(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.Property(w => w.AvailableBalance)
                      .HasPrecision(18, 2);
                
                entity.Property(w => w.EscrowBalance)
                      .HasPrecision(18, 2);
            });
            
            builder.Entity<TransactionLog>(entity =>
            {
                entity.ToTable("TransactionLogs");
                
                entity.HasIndex(e => e.ToUserId)
                      .HasDatabaseName("IX_TransactionLogs_ToUserId");
                
                entity.HasIndex(e => e.FromUserId)
                      .HasDatabaseName("IX_TransactionLogs_FromUserId");
                
                entity.HasIndex(e => e.Timestamp)
                      .HasDatabaseName("IX_TransactionLogs_Timestamp");
                
                entity.HasIndex(e => e.TransactionReference)
                      .IsUnique()
                      .HasDatabaseName("IX_TransactionLogs_Reference_Unique");
                
                entity.HasOne(t => t.ToUser)
                      .WithMany()
                      .HasForeignKey(t => t.ToUserId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(t => t.FromUser)
                      .WithMany()
                      .HasForeignKey(t => t.FromUserId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.Property(t => t.Amount)
                      .HasPrecision(18, 2);
                
                entity.Property(t => t.Type)
                      .HasConversion<string>();
            });
            
            builder.Entity<UserSkill>(entity =>
            {
                entity.ToTable("UserSkills");
                
                entity.HasIndex(e => new { e.UserId, e.SkillId, e.SkillType })
                      .IsUnique()
                      .HasDatabaseName("IX_UserSkills_Unique");
                
                entity.HasIndex(e => e.UserId)
                      .HasDatabaseName("IX_UserSkills_UserId");
                
                entity.HasIndex(e => e.SkillId)
                      .HasDatabaseName("IX_UserSkills_SkillId");
                
                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserSkills)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(us => us.Skill)
                      .WithMany(u => u.UserSkills)
                      .HasForeignKey(us => us.SkillId)
                      .OnDelete(DeleteBehavior.Restrict);

                
                entity.Property(us => us.SkillType)
                      .HasConversion<string>();
                
                entity.Property(us => us.ProficiencyLevel)
                      .HasConversion<string>();
            });
            
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName)
                      .IsRequired()
                      .HasMaxLength(100);
                
                entity.Property(u => u.Bio)
                      .HasMaxLength(1000);
                
                entity.Property(u => u.ProfilePictureUrl)
                      .HasMaxLength(500);
                
                entity.Property(u => u.AverageRating)
                      .HasPrecision(3, 2);
            });
            
            builder.Entity<Skill>(entity =>
            {
                  entity.ToTable("Skills");

                  entity.Property(s => s.Name)
                        .IsRequired()
                        .HasMaxLength(80);

                  entity.Property(s => s.Description)
                        .HasMaxLength(200);

                  entity.HasIndex(s => s.Name)
                        .IsUnique()
                        .HasDatabaseName("IX_Skills_Name_Unique");
            });
            
            builder.Entity<Listing>(entity =>
            {
                  entity.ToTable("Listings");

                  entity.Property(l => l.Title)
                        .IsRequired()
                        .HasMaxLength(120);

                  entity.Property(l => l.Description)
                        .HasMaxLength(800);

                  entity.Property(l => l.CreditsPerHour)
                        .IsRequired();

                  entity.Property(l => l.Status)
                        .HasConversion<string>();

                  entity.HasIndex(l => l.ProviderId)
                        .HasDatabaseName("IX_Listings_ProviderId");

                  entity.HasIndex(l => l.Status)
                        .HasDatabaseName("IX_Listings_Status");

                    entity.HasIndex(l => l.CreatedAt)
                          .HasDatabaseName("IX_Listings_CreatedAt");

                    entity.HasOne(l => l.Provider)
                          .WithMany() 
                          .HasForeignKey(l => l.ProviderId)
                          .OnDelete(DeleteBehavior.Cascade);
            });
            
            builder.Entity<ListingSkill>(entity =>
            {
                  entity.ToTable("ListingSkills");

              entity.HasIndex(ls => new { ls.ListingId, ls.SkillId })
                    .IsUnique()
                    .HasDatabaseName("IX_ListingSkills_Unique");

              entity.HasIndex(ls => ls.SkillId)
                    .HasDatabaseName("IX_ListingSkills_SkillId");

              entity.HasOne(ls => ls.Listing)
                    .WithMany(l => l.ListingSkills)
                    .HasForeignKey(ls => ls.ListingId)
                    .OnDelete(DeleteBehavior.Cascade);

              entity.HasOne(ls => ls.Skill)
                    .WithMany()
                    .HasForeignKey(ls => ls.SkillId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.Property(ls => ls.ProficiencyLevel)
                          .HasConversion<string>();
            });
            
            builder.Entity<ListingAvailability>(entity =>
            {
                  entity.ToTable("ListingAvailabilities");

                    entity.HasIndex(a => a.ListingId)
                          .HasDatabaseName("IX_ListingAvailabilities_ListingId");

                    entity.HasIndex(a => a.StartTime)
                          .HasDatabaseName("IX_ListingAvailabilities_StartTime");

                    entity.HasIndex(a => a.EndTime)
                          .HasDatabaseName("IX_ListingAvailabilities_EndTime");

                    entity.HasOne(a => a.Listing)
                          .WithMany(l => l.ListingAvailabilities)
                          .HasForeignKey(a => a.ListingId)
                          .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}