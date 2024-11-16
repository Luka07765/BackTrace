namespace Trace.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trace.Models.Auth;
using Trace.Models.Logic;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for additional entities
    public DbSet<Folder> Folders { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


          modelBuilder.Ignore<IdentityRole>();
        modelBuilder.Ignore<IdentityUserRole<string>>();
        modelBuilder.Ignore<IdentityRoleClaim<string>>();


        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Ignore(e => e.PhoneNumber);
            entity.Ignore(e => e.PhoneNumberConfirmed);
            entity.Ignore(e => e.EmailConfirmed);
            entity.Ignore(e => e.TwoFactorEnabled);
            entity.Ignore(e => e.LockoutEnd);
            entity.Ignore(e => e.LockoutEnabled);
            entity.Ignore(e => e.AccessFailedCount);
        });
        // Configure Folder entity
        modelBuilder.Entity<Folder>()
            .HasMany(f => f.SubFolders)
            .WithOne(f => f.ParentFolder)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Folder>()
            .HasOne(f => f.User)
            .WithMany() // No navigation collection in ApplicationUser for Folders
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure File entity
        modelBuilder.Entity<File>()
            .HasOne(f => f.Folder)
            .WithMany(f => f.Files)
            .HasForeignKey(f => f.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<File>()
            .HasOne(f => f.User)
            .WithMany() // No navigation collection in ApplicationUser for Files
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>()
            .Ignore(r => r.IsActive) // Exclude IsActive if it's a calculated property
            .Property(r => r.RevokedByIp)
            .HasMaxLength(45); // Set max length for IP addresses
    }
}
