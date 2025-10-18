using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Models.Auth;

namespace Trace.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> entity)
        {
            entity.Ignore(e => e.PhoneNumber);
            entity.Ignore(e => e.PhoneNumberConfirmed);
            entity.Ignore(e => e.EmailConfirmed);
            entity.Ignore(e => e.TwoFactorEnabled);
            entity.Ignore(e => e.LockoutEnd);
            entity.Ignore(e => e.LockoutEnabled);
            entity.Ignore(e => e.AccessFailedCount);
        }
    }
}
