using Trace.Models.Logic;

namespace Trace.DTO
{
    public class FolderLayerPayload
    {
        public int Depth { get; }
        public IEnumerable<Folder> Folders { get; }

        public FolderLayerPayload(int depth, IEnumerable<Folder> folders)
        {
            Depth = depth;
            Folders = folders;
        }
    }

}
