using Microsoft.AspNetCore.Identity;
using Trace.Models.Auth;

namespace Trace.Models.Logic
{
    public class File
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        // User association
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

       
        public Guid FolderId { get; set; }
        public Folder Folder { get; set; }
        public string Colors { get; set; } = "[]";


    }
}
