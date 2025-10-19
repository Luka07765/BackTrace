

namespace Trace.Repository.Files.Modify
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;

    public class FileModifyRepository : IFileModifyRepository
    {
        private readonly ApplicationDbContext _context;

        public FileModifyRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

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
