using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Trace.Models.Auth;

namespace Trace.Models.Logic
{
    public class Folder
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        // User association
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Parent-Child relationship
        public Guid? ParentFolderId { get; set; }
        public Folder ParentFolder { get; set; }
        public ICollection<Folder> SubFolders { get; set; }

        // Files in the folder
        public ICollection<File> Files { get; set; }

        public int FolderPosition { get; set; }

        public int IconId { get; set; } = 1;
    }
}