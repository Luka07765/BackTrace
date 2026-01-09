using System;
using System.Collections.Generic;

using Trace.Models.Auth;

namespace Trace.Models.Logic
{
    public class Folder
    {
        // Identity
        public Guid Id { get; set; }
        public string Title { get; set; }

        // Ownership
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        // Hierarchy

        public Guid? DomainId { get; set; }
        public Domain Domain { get; set; }


        public Guid? ParentFolderId { get; set; }
        public Folder ParentFolder { get; set; }
        public ICollection<Folder> SubFolders { get; set; } = new List<Folder>();

        // Contents
        public ICollection<File> Files { get; set; } = new List<File>();

        // UI / Metadata
        public int FolderPosition { get; set; }
        public int RedCount { get; set; }
        public int YellowCount { get; set; }
        public int IconId { get; set; } = 1;
    }
}
