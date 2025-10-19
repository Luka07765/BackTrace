using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Models.TagSystem;

namespace Trace.Data.Configurations
{
    public class TagAssignmentConfiguration : IEntityTypeConfiguration<TagAssignment>
    {
        public void Configure(EntityTypeBuilder<TagAssignment> entity)
        {
            entity.HasKey(ta => new { ta.FileId, ta.TagId });

            entity.HasOne(ta => ta.File)
                  .WithMany(f => f.TagAssignments)
                  .HasForeignKey(ta => ta.FileId);

            entity.HasOne(ta => ta.Tag)
                  .WithMany(t => t.TagAssignments)
                  .HasForeignKey(ta => ta.TagId);
        }
    }
}
