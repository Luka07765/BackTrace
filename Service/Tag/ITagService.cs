namespace Trace.Service.Tag
{
    using Trace.Models.TagSystem;
    using Trace.Models.Logic;

    public interface ITagService
    {
        Task<Tag> GetTagByIdAsync(Guid tagId, string userId);
        Task<IEnumerable<Tag>> GetAllTagsAsync(string userId);
        Task CreateTagAsync(Guid id,string userId, string title, string color = "#FFFFFF", int iconId = 1);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(Guid tagId, string userId);

        Task AssignTagToFileAsync(Guid fileId, Guid tagId, string userId);
        Task RemoveTagFromFileAsync(Guid fileId, Guid tagId, string userId);
        Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId, string userId);
    }
}
