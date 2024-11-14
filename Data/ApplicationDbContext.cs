namespace Trace.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trace.Models;


    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Folder> Folders { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Folder entity
        modelBuilder.Entity<Folder>()
            .HasMany(f => f.SubFolders)
            .WithOne(f => f.ParentFolder)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Folder>()
            .HasOne(f => f.User)
            .WithMany() // No navigation collection in IdentityUser for Folders
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
            .WithMany() // No navigation collection in IdentityUser for Files
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>()
            .Ignore(r => r.IsActive)  // Exclude IsActive if it's a calculated property
            .Property(r => r.RevokedByIp)
            .HasMaxLength(45); // Set max length for IP addresses
    }
}
