namespace Trace.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int? ParentFolderId { get; set; }
        public Folder ParentFolder { get; set; }
        public ICollection<Folder> SubFolders { get; set; }
        public ICollection<File> Files { get; set; }
    }

}
