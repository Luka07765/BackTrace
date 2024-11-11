namespace Trace.GraphQL.Mutations
{
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models;
    using Trace.Service;
    using HotChocolate;

    public class Mutation
    {
        [GraphQLName("createFolder")]
        public async Task<Folder> CreateFolder(FolderInput input, [Service] IFolderService folderService)
        {
            return await folderService.CreateFolderAsync(input);
        }

        [GraphQLName("updateFolder")]
        public async Task<Folder> UpdateFolder(int id, FolderInput input, [Service] IFolderService folderService)
        {
            return await folderService.UpdateFolderAsync(id, input);
        }

        [GraphQLName("deleteFolder")]
        public async Task<bool> DeleteFolder(int id, [Service] IFolderService folderService)
        {
            return await folderService.DeleteFolderAsync(id);
        }

        [GraphQLName("createFile")]
        public async Task<File> CreateFile(FileInput input, [Service] IFileService fileService)
        {
            return await fileService.CreateFileAsync(input);
        }

        [GraphQLName("updateFile")]
        public async Task<File> UpdateFile(int id, FileInput input, [Service] IFileService fileService)
        {
            return await fileService.UpdateFileAsync(id, input);
        }

        [GraphQLName("deleteFile")]
        public async Task<bool> DeleteFile(int id, [Service] IFileService fileService)
        {
            return await fileService.DeleteFileAsync(id);
        }
    }
}
