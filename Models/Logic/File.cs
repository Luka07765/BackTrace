using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trace.Models.Auth;
using Trace.Models.TagSystem;

namespace Trace.Models.Logic
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [Timestamp]                
        public byte[] RowVersion { get; set; } = default!;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

       
        public Guid FolderId { get; set; }
        public Folder Folder { get; set; }
        public string Colors { get; set; } = "[]";

        public int? FilePosition { get; set; }

        public int IconId { get; set; } = 1;
        [NotMapped]
        public IEnumerable<Trace.Models.TagSystem.Tag> Tags => TagAssignments.Select(ta => ta.Tag);
        public ICollection<TagAssignment> TagAssignments { get; set; } = new List<TagAssignment>();

    }
}
