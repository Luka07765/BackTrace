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

    public DbSet<Tag> Tags { get; set; }
    public DbSet<SpecialGroup> SpecialGroups { get; set; }
    public DbSet<FileTag> FileTags { get; set; }
    public DbSet<TagSpecialGroup> TagSpecialGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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


        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.Property(r => r.ConcurrencyStamp).HasColumnType("text");
            entity.Property(r => r.Id).HasMaxLength(127); // Adjust length for PostgreSQL
        });

        modelBuilder.Entity<IdentityUser>(entity =>
        {
            entity.Property(u => u.ConcurrencyStamp).HasColumnType("text");
            entity.Property(u => u.Id).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.Property(uc => uc.ClaimType).HasColumnType("text");
            entity.Property(uc => uc.ClaimValue).HasColumnType("text");
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.Property(ul => ul.ProviderKey).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.Property(t => t.Value).HasColumnType("text");
            entity.Property(t => t.Name).HasMaxLength(127);
            entity.Property(t => t.LoginProvider).HasMaxLength(127);
        });


        // Configure Folder entity
        modelBuilder.Entity<Folder>()
            .HasMany(f => f.SubFolders)
            .WithOne(f => f.ParentFolder)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);


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


        // -------------------------------
        // Tag system configuration
        // -------------------------------

        // FileTag: many-to-many File <-> Tag
        modelBuilder.Entity<FileTag>()
            .HasKey(ft => new { ft.FileId, ft.TagId });

        modelBuilder.Entity<FileTag>()
            .HasOne(ft => ft.File)
            .WithMany(f => f.FileTags)
            .HasForeignKey(ft => ft.FileId);

        modelBuilder.Entity<FileTag>()
            .HasOne(ft => ft.Tag)
            .WithMany(t => t.FileTags)
            .HasForeignKey(ft => ft.TagId);

        // TagSpecialGroup: many-to-many Tag <-> SpecialGroup
        modelBuilder.Entity<TagSpecialGroup>()
            .HasKey(tsg => new { tsg.TagId, tsg.SpecialGroupId });

        modelBuilder.Entity<TagSpecialGroup>()
            .HasOne(tsg => tsg.Tag)
            .WithMany(t => t.TagSpecialGroups)
            .HasForeignKey(tsg => tsg.TagId);

        modelBuilder.Entity<TagSpecialGroup>()
            .HasOne(tsg => tsg.SpecialGroup)
            .WithMany(sg => sg.TagSpecialGroups)
            .HasForeignKey(tsg => tsg.SpecialGroupId);
    }
}
