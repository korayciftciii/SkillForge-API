using Microsoft.EntityFrameworkCore;
using SkillForge.Domain.Common;
using SkillForge.Domain.Entities;
using SkillForge.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService? _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IDateTimeProvider dateTimeProvider,
            ICurrentUserService? currentUserService = null) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project entity configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.OwnerId).IsRequired();
                entity.Property(p => p.Status).HasConversion<string>();
                entity.Property(p => p.Category).HasMaxLength(50);
                entity.Property(p => p.IsPublic).HasDefaultValue(true);
                
                // One-to-many relationship with ProjectTag
                entity.HasMany(p => p.Tags)
                    .WithOne(t => t.Project)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // One-to-many relationship with ProjectMember
                entity.HasMany(p => p.TeamMembers)
                    .WithOne(m => m.Project)
                    .HasForeignKey(m => m.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // ProjectTag entity configuration
            modelBuilder.Entity<ProjectTag>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Description).HasMaxLength(200);
            });
            
            // ProjectMember entity configuration
            modelBuilder.Entity<ProjectMember>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.UserId).IsRequired();
                entity.Property(m => m.Role).HasConversion<string>();
            });
            
            // RefreshToken entity configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired();
                entity.Property(rt => rt.JwtId).IsRequired();
                entity.Property(rt => rt.UserId).IsRequired();
                entity.Property(rt => rt.IsUsed).HasDefaultValue(false);
                entity.Property(rt => rt.IsRevoked).HasDefaultValue(false);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleAuditableEntities();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            HandleAuditableEntities();
            return base.SaveChanges();
        }

        private void HandleAuditableEntities()
        {
            var userId = _currentUserService?.UserId;
            var now = _dateTimeProvider.UtcNow;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = userId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = userId;
                        break;
                }
            }
        }
    }
}
