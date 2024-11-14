﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Trace.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // User association
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // Parent-Child relationship
        public int? ParentFolderId { get; set; }
        public Folder ParentFolder { get; set; }
        public ICollection<Folder> SubFolders { get; set; }

        // Files in the folder
        public ICollection<File> Files { get; set; }
    }
}