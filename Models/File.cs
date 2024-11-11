namespace Trace.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int FolderId { get; set; }
        public Folder Folder { get; set; }
    }

}
