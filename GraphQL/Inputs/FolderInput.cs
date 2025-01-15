namespace Trace.GraphQL.Inputs
{
    public class FolderInput
    {
        public string Title { get; set; }
        public Guid? ParentFolderId { get; set; }
    }

}
