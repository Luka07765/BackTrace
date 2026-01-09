using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trace.Models.Logic;

namespace Trace.Data.Configurations
{
    public class FolderConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> entity)
        {
            entity.HasMany(f => f.SubFolders)
                  .WithOne(f => f.ParentFolder)
                  .HasForeignKey(f => f.ParentFolderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.User)
                  .WithMany()
                  .HasForeignKey(f => f.UserId)
                  .OnDelete(DeleteBehavior.Cascade);


            entity.HasOne(f => f.Domain)
           .WithMany(d => d.RootFolders)
           .HasForeignKey(f => f.DomainId)
           .OnDelete(DeleteBehavior.SetNull);

        }

    }
}
