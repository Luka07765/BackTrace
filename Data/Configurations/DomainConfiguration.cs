using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Models.Logic;

namespace Trace.Data.Configurations
{
    public class DomainConfiguration : IEntityTypeConfiguration<Domain>
    {
        public void Configure(EntityTypeBuilder<Domain> builder)
        {
            builder.ToTable("Domains");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(d => d.User)
                   .WithMany() 
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(d => new { d.UserId, d.Title }).IsUnique(false);
        }
    }
}
