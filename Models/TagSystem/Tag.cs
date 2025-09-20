using Trace.Models.Auth;

namespace Trace.Models.TagSystem
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string Color { get; set; }
        public int IconId { get; set; } = 1;

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<TagAssignment> TagAssignments { get; set; } = new List<TagAssignment>();
    }
}
