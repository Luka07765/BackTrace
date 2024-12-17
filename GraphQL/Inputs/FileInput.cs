namespace Trace.GraphQL.Inputs
{
    public class CreateFileInput
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int FolderId { get; set; }
    }

    public class UpdateFileInput
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? FolderId { get; set; }
    }
}

