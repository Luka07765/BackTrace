using System;
using System.Collections.Generic;
using Trace.Models.Account;

namespace Trace.Models.Logic
{
    public class Folder
    {
        // Identity
        public Guid Id { get; set; }
        public string Title { get; set; }

        // Ownership
        public string UserId { get; set; }
        public User User { get; set; }


        // Hierarchy

   

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
