using Microsoft.AspNetCore.Identity;

namespace Trace.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        // User association
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // Folder association
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
    }
}
