

namespace Trace.Repository.TagSystem.Tag
{
    using Trace.Models.TagSystem;
    using Trace.Models.Logic;

    public interface ITagRepository
    {
        Task<Tag> GetTagByIdAsync(Guid tagId);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(Guid tagId);

        Task AssignTagToFileAsync(Guid fileId, Guid tagId);
        Task RemoveTagFromFileAsync(Guid fileId, Guid tagId);
        Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId);
    }
}
