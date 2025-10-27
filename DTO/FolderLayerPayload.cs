
using System.Collections.Generic;

namespace Trace.DTO
{
    using Trace.Models.Logic;

    public class FolderLayerPayload
    {
        public int Depth { get; }
        public IEnumerable<Folder> Folders { get; }   
        public IEnumerable<File> Files { get; }     

        public FolderLayerPayload(int depth, IEnumerable<Folder> folders, IEnumerable<File> files)
        {
            Depth = depth;
            Folders = folders;
            Files = files;
        }
    }
}

