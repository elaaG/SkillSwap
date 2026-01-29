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
                
                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserSkills)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
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
        }
    }
}