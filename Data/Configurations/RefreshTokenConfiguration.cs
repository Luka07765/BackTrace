using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Models.Auth;

namespace Trace.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> entity)
        {
            entity.Ignore(r => r.IsActive);
            entity.Property(r => r.RevokedByIp).HasMaxLength(45);
        }
    }
}
