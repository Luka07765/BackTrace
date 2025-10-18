namespace Trace.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trace.Data.Configurations;
using Trace.Models.Auth;
using Trace.Models.Logic;
using Trace.Models.TagSystem;

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
    public DbSet<Tag> Tag{ get; set; }    
    public DbSet<TagAssignment> TagAssignments { get; set; }    



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());





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



        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>()
            .Ignore(r => r.IsActive) // Exclude IsActive if it's a calculated property
            .Property(r => r.RevokedByIp)
            .HasMaxLength(45); // Set max length for IP addresses


        // -------------------------------
        // Tag system configuration
        // -------------------------------

        modelBuilder.Entity<TagAssignment>()
    .HasKey(ta => new { ta.FileId, ta.TagId });

        modelBuilder.Entity<TagAssignment>()
            .HasOne(ta => ta.File)
            .WithMany(f => f.TagAssignments)
            .HasForeignKey(ta => ta.FileId);

        modelBuilder.Entity<TagAssignment>()
            .HasOne(ta => ta.Tag)
            .WithMany(t => t.TagAssignments)
            .HasForeignKey(ta => ta.TagId);


    }
}
