using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Trace.Data.Configurations
{
    // ------------------------------------------------------------
    // IdentityRole configuration
    // ------------------------------------------------------------
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> entity)
        {
            entity.Property(r => r.ConcurrencyStamp).HasColumnType("text");
            entity.Property(r => r.Id).HasMaxLength(127);
        }
    }

    // ------------------------------------------------------------
    // IdentityUser configuration
    // ------------------------------------------------------------
    public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> entity)
        {
            entity.Property(u => u.ConcurrencyStamp).HasColumnType("text");
            entity.Property(u => u.Id).HasMaxLength(127);
        }
    }

    // ------------------------------------------------------------
    // IdentityUserClaim configuration
    // ------------------------------------------------------------
    public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> entity)
        {
            entity.Property(uc => uc.ClaimType).HasColumnType("text");
            entity.Property(uc => uc.ClaimValue).HasColumnType("text");
        }
    }

    // ------------------------------------------------------------
    // IdentityUserLogin configuration
    // ------------------------------------------------------------
    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> entity)
        {
            entity.Property(ul => ul.ProviderKey).HasMaxLength(127);
        }
    }

    // ------------------------------------------------------------
    // IdentityUserToken configuration
    // ------------------------------------------------------------
    public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> entity)
        {
            entity.Property(t => t.Value).HasColumnType("text");
            entity.Property(t => t.Name).HasMaxLength(127);
            entity.Property(t => t.LoginProvider).HasMaxLength(127);
        }
    }
}
