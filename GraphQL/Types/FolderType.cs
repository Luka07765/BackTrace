using Trace.Models.Logic;

namespace Trace.GraphQL.Types
{
    public class FolderType : ObjectType<Folder>
    {
        protected override void Configure(IObjectTypeDescriptor<Folder> descriptor)
        {
            descriptor.Field(f => f.Id);
            descriptor.Field(f => f.Title);
            descriptor.Field(f => f.Files);
   
            descriptor.Field(f => f.SubFolders)
                      .Type<ListType<FolderType>>(); 
        }
    }

}
