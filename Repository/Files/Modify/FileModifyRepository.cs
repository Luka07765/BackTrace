

namespace Trace.Repository.Files.Modify
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    public class FileModifyRepository : IFileModifyRepository
    {
        private readonly ApplicationDbContext _context;

        public FileModifyRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

        }

        public async Task<File?> UpdateFileAsync(Guid fileId, UpdateFileInput input)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId);
            if (file == null) return null;

            if (input.Title != null) file.Title = input.Title;
            if (input.Content != null) file.Content = input.Content;
            if (input.Colors != null) file.Colors = input.Colors;
            if (input.FilePosition.HasValue) file.FilePosition = input.FilePosition.Value;
            if (input.IconId.HasValue) file.IconId = input.IconId.Value;
            if (input.FolderId.HasValue) file.FolderId = input.FolderId.Value;

            await _context.SaveChangesAsync();
            return file;
        }



        public async Task<File> CreateFileAsync(File file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }
        public async Task<int> DeleteFileAsync(Guid id)
        {
            return await _context.Files
                .Where(f => f.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
