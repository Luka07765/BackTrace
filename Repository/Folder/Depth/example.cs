//namespace Trace.Repository.Folder.Depth
//{
//    public class example
//    {
//        public async Task<Folder> GetFolderWithDepthAsync(Guid folderId, string userId, int depth)
//        {
//            var root = await _context.Folders
//                .Where(f => f.Id == folderId && f.UserId == userId)
//                .Include(f => f.Files)
//                .FirstOrDefaultAsync();

//            if (root == null || depth <= 0)
//                return root;

//            // Load subfolders recursively up to desired depth
//            await LoadSubFoldersRecursively(root, depth - 1);

//            return root;
//        }

//    }private async Task LoadSubFoldersRecursively(Folder folder, int depth)
//        {
//            if (depth < 0)
//                return;

//            // Load subfolders + files for current folder
//            await _context.Entry(folder)
//                .Collection(f => f.SubFolders)
//                .Query()
//                .Include(sf => sf.Files)
//                .LoadAsync();

//            foreach (var subFolder in folder.SubFolders)
//            {
//                if (depth > 0)
//                {
//                    await LoadSubFoldersRecursively(subFolder, depth - 1);
//                }
//            }
//        }
//        var totalCount = await _context.Folders.CountAsync(f => f.UserId == userId);
//        int depth = totalCount < 200 ? 3 : totalCount < 1000 ? 2 : 1;

//return await GetFolderWithDepthAsync(folderId, userId, depth);


//    }
