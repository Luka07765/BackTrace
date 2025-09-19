namespace Trace.Service.Tag
{
    using Trace.Models.TagSystem;
    using Trace.Models.Logic;
    public interface ITagService
    {
        Task<Tag> GetTagByIdAsync(Guid tagId);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task CreateTagAsync(string title, string color = "#FFFFFF", int iconId = 1);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(Guid tagId);

        Task AssignTagToFileAsync(Guid fileId, Guid tagId);
        Task RemoveTagFromFileAsync(Guid fileId, Guid tagId);
        Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId);
    }
}
