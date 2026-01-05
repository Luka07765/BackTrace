namespace Trace.GraphQL.Inputs
{
    public class TagInput
    {
        public class CreateTagInput
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Color { get; set; } = "#FFFFFF";
            public int IconId { get; set; } = 1;
        }

        public class UpdateTagInput
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Color { get; set; }
            public int IconId { get; set; } = 1;
        }

        public class AssignTagInput
        {
            public Guid FileId { get; set; }
            public Guid TagId { get; set; }
        }
    }
}
