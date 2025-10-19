namespace Trace.Repository.TagSystem.Tag
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Trace.Data;
    using Trace.Models.Logic;
    using Trace.Models.TagSystem;

    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> GetTagByIdAsync(Guid tagId, string userId)
        {
            return await _context.Tag
                .Include(t => t.TagAssignments)
                .ThenInclude(ta => ta.File)
                .FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(string userId)
        {
            return await _context.Tag
                .Where(f => f.UserId == userId)
                .Include(t => t.TagAssignments)
                .ToListAsync();
        }

        public async Task AddTagAsync(Tag tag)
        {
            if (tag.Id == Guid.Empty)
                tag.Id = Guid.NewGuid();  // ensure valid Id

            _context.Tag.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Tag.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(Guid tagId, string userId)
        {
            var tag = await _context.Tag.FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId);

            if (tag != null)
            {
                _context.Tag.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignTagToFileAsync(Guid fileId, Guid tagId)
        {
            var exists = await _context.TagAssignments.AnyAsync(ta => ta.FileId == fileId && ta.TagId == tagId);
            if (!exists)
            {
                _context.TagAssignments.Add(new TagAssignment { FileId = fileId, TagId = tagId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveTagFromFileAsync(Guid fileId, Guid tagId)
        {
            var assignment = await _context.TagAssignments
                .FirstOrDefaultAsync(ta => ta.FileId == fileId && ta.TagId == tagId);
            if (assignment != null)
            {
                _context.TagAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId)
        {
            return await _context.TagAssignments
                .Where(ta => ta.TagId == tagId)
                .Include(ta => ta.File)
                .Select(ta => ta.File)
                .ToListAsync();
        }
    }
}
