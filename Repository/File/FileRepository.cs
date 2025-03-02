namespace Trace.Repository.File
{
    using Microsoft.EntityFrameworkCore;
    using Dapper;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;
    using MySqlConnector;
    using Microsoft.Extensions.Configuration;

    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;      // For EF Core operations
        private readonly string _connectionString;          // For Dapper operations

        public FileRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // EF Core: Get all files for a user
        public async Task<IEnumerable<File>> GetAllFilesAsync(string userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId)
                .Include(f => f.Folder)
                .ToListAsync();
        }

        // EF Core: Get file by ID for a user
        public async Task<File> GetFileByIdAsync(Guid id, string userId)
        {
            return await _context.Files
                .Include(f => f.Folder)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        // EF Core: Create a new file
        public async Task<File> CreateFileAsync(File file)
        {
            file.Id = Guid.NewGuid();
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }
        public async Task<File> SaveFileDeltaAsync(Guid fileId, string? title, string? content, string userId)
        {
            using var connection = new MySqlConnection(_connectionString);

            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", fileId);
            parameters.Add("UserId", userId);

            if (title != null)
            {
                setClauses.Add("Title = @Title");
                parameters.Add("Title", title);
            }
            if (content != null)
            {
                setClauses.Add("Content = @Content");
                parameters.Add("Content", content);
            }

            if (setClauses.Count == 0)
            {
                // No fields to update, return the current file
                return await GetFileByIdAsync(fileId, userId);
            }

            var query = $@"
        UPDATE Files
        SET {string.Join(", ", setClauses)}
        WHERE Id = @Id AND UserId = @UserId;
        
        SELECT * FROM Files WHERE Id = @Id AND UserId = @UserId;
    ";

            return await connection.QueryFirstOrDefaultAsync<File>(query, parameters);
        }

        public async Task<bool> DeleteFileAsync(Guid id, string userId)
        {
            var file = await GetFileByIdAsync(id, userId);
            if (file == null) return false;

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
