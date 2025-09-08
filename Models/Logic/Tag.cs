using System;
using System.Collections.Generic;

namespace Trace.Models.Logic
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Color { get; set; } = "#FFFFFF"; 
        public int IconId { get; set; } = 1;

        // Relation to Files (many-to-many)
        public ICollection<FileTag> FileTags { get; set; }

  
        public ICollection<TagSpecialGroup> TagSpecialGroups { get; set; }
    }

    public class SpecialGroup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Color { get; set; } = "#FFFFFF";
        public int IconId { get; set; } = 1;

       
        public ICollection<TagSpecialGroup> TagSpecialGroups { get; set; }
    }

   
    public class FileTag
    {
        public Guid FileId { get; set; }
        public File File { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }

   
    public class TagSpecialGroup
    {
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }

        public Guid SpecialGroupId { get; set; }
        public SpecialGroup SpecialGroup { get; set; }
    }
}
