using Trace.Models.Account;

namespace Trace.Models.Logic
{
    public class Domain
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


        public ICollection<Folder> RootFolders { get; set; } = new List<Folder>();
    }
}
