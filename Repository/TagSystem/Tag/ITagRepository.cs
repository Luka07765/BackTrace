namespace Trace.Repository.TagSystem.Tag
{
    using Trace.Models.TagSystem;
    using Trace.Models.Logic;

    public interface ITagRepository
    {
        Task<Tag> GetTagByIdAsync(Guid tagId, string userId);
        Task<IEnumerable<Tag>> GetAllTagsAsync(string userId);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(Guid tagId, string userId);

        Task AssignTagToFileAsync(Guid fileId, Guid tagId);
        Task RemoveTagFromFileAsync(Guid fileId, Guid tagId);
        Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId);
    }
}
