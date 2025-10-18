namespace Trace.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Trace.Models.Logic;
    public class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> entity)
        {
            entity.HasOne(f => f.Folder)
                  .WithMany(f => f.Files)
                  .HasForeignKey(f => f.FolderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.User)
                  .WithMany()
                  .HasForeignKey(f => f.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

