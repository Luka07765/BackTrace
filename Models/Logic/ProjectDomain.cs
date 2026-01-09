using Trace.Models.Auth;

namespace Trace.Models.Logic
{
    public class ProjectDomain
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Folder> RootFolders { get; set; }
    }
}
