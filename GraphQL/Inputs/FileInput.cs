namespace Trace.GraphQL.Inputs
{
    public class CreateFileInput
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid FolderId { get; set; }

        public string Colors { get; set; } = "Green";

        public int FilePosition { get; set; }

        public int IconId { get; set; }
    }

    public class UpdateFileInput
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public Guid? FolderId { get; set; }

        public string? Colors { get; set; }

        public int? FilePosition { get; set; }
        public int? IconId { get; set; }
    }
}

