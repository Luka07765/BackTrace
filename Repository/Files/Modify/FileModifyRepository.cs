

namespace Trace.Repository.Files.Modify
{

    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;

    using Microsoft.Extensions.Configuration;
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

    }
}
