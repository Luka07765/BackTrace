

namespace Trace.Repository.Files.Modify
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.DTO;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    using Trace.Repository.Color;

    public class FileModifyRepository : IFileModifyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IColorRepository _colorRepository;
        public FileModifyRepository(ApplicationDbContext context, IConfiguration configuration, IColorRepository colorRepository)
        {
            _context = context;
            _colorRepository = colorRepository;
        }
    
        public async Task<File?> UpdateFileAsync(Guid fileId, UpdateFileInput input)
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    var file = await _context.Files
                        .AsTracking()
                        .FirstOrDefaultAsync(f => f.Id == fileId);

                    if (file == null)
                        return null;

                    var rvProp = _context.Entry(file).Property(f => f.RowVersion);

                    if (input.RowVersion != null)
                    {

                        rvProp.OriginalValue = input.RowVersion;
                    }
                    else
                    {

                        rvProp.OriginalValue = file.RowVersion;
                        rvProp.IsModified = false;
                    }

                    var oldColor = file.Colors;
                    var oldFolderId = file.FolderId;

                    if (input.Title != null) file.Title = input.Title;
                    if (input.Content != null) file.Content = input.Content;
                    if (input.Colors != null) file.Colors = input.Colors;
                    if (input.FilePosition.HasValue) file.FilePosition = input.FilePosition.Value;
                    if (input.IconId.HasValue) file.IconId = input.IconId.Value;
                    if (input.FolderId.HasValue) file.FolderId = input.FolderId.Value;


                    var newColor = file.Colors;
                    var newFolderId = file.FolderId;

                    bool colorChanged = oldColor != newColor;
                    bool folderChanged = oldFolderId != newFolderId;

                    if (colorChanged || folderChanged)
                    {
                        var oldAncestors = await _colorRepository.GetAncestorChainAsync(oldFolderId);
                        var newAncestors =
                            oldFolderId == newFolderId
                                ? oldAncestors
                                : await _colorRepository.GetAncestorChainAsync(newFolderId);

                        void ApplyDelta(IEnumerable<Folder> folders, string color, int delta)
                        {
                            foreach (var folder in folders)
                            {
                                if (color == "Red")
                                    folder.RedCount = Math.Max(0, folder.RedCount + delta);
                                else if (color == "Yellow")
                                    folder.YellowCount = Math.Max(0, folder.YellowCount + delta);
                            }
                        }

                        if (oldColor is "Red" or "Yellow")
                            ApplyDelta(oldAncestors, oldColor, -1);

                        if (newColor is "Red" or "Yellow")
                            ApplyDelta(newAncestors, newColor, +1);
                    }

                    await _context.SaveChangesAsync();
                    return file;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (attempt == 1)
                        throw;


                    foreach (var entry in _context.ChangeTracker.Entries())
                        entry.State = EntityState.Detached;
                }

            }

            return null;
        }
        public async Task<File> CreateFileAsync(File file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }



        public async Task<bool> DeleteFileAsync(Guid fileId)
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    var file = await _context.Files
                        .AsTracking()
                        .FirstOrDefaultAsync(f => f.Id == fileId);

                    if (file == null)
                        return false;

                    var color = file.Colors;
                    var folderId = file.FolderId;

                    if (color is "Red" or "Yellow")
                    {
                        var ancestors = await _colorRepository.GetAncestorChainAsync(folderId);

                        foreach (var folder in ancestors)
                        {
                            if (color == "Red")
                                folder.RedCount = Math.Max(0, folder.RedCount - 1);
                            else if (color == "Yellow")
                                folder.YellowCount = Math.Max(0, folder.YellowCount - 1);
                        }
                    }

                    _context.Files.Remove(file);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (attempt == 1)
                        throw;

                    foreach (var entry in _context.ChangeTracker.Entries())
                        entry.State = EntityState.Detached;
                }
            }

            return false;
        }


        public async Task<bool> SoftDeleteFileAsync(Guid fileId)
        {
            var file = await _context.Files
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == fileId && f.DeletedAt == null);

            if (file == null)
                return false;

            file.OriginalFolderId ??= file.FolderId;
            file.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }


        //The restore method it will have move kada opet radis restore seti se da ima move i restore lokacija.
        public async Task<bool> RestoreFileAsync(Guid fileId)
        {
            var file = await _context.Files
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == fileId && f.DeletedAt != null);

            if (file.OriginalFolderId == null)
                return false;


            file.DeletedAt = null;
            file.FolderId = file.OriginalFolderId.Value;

            await _context.SaveChangesAsync();
            return true;
        }

 
    }
}