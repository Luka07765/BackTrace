namespace Trace.GraphQL.Inputs
{
    public class FolderInput
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid? DomainId { get; set; }
        public int? IconId { get; set; }
    }

}
