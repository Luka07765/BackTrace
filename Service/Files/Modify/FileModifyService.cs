
namespace Trace.Service.Files.Modify
{
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
  
    using Trace.Repository.Files.Modify;

    public class FileModifyService : IFileModifyService
    {
        private readonly IFileModifyRepository _fileModifyRepository;


        public FileModifyService(IFileModifyRepository fileModifyRepository)
        {
            _fileModifyRepository = fileModifyRepository;
        }
        public async Task<File> CreateFileAsync(CreateFileInput input, string userId)
        {
    
            var file = new File
            {
                Title = input.Title,
                Content = input.Content ?? "",
                FolderId = input.FolderId,
                UserId = userId,
                Colors = input.Colors ?? "Green",
                FilePosition = input.FilePosition,
                IconId = input.IconId == 0 ? 1 : input.IconId 
            };


            return await _fileModifyRepository.CreateFileAsync(file);
        }
        public async Task<File?> UpdateFileAsync(Guid id, UpdateFileInput input)
        {
            return await _fileModifyRepository.UpdateFileAsync(id, input);
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var result = await _fileModifyRepository.DeleteFileAsync(id);
            return result > 0; 
        }
    }
}
