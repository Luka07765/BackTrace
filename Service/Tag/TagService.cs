
namespace Trace.Service.Tag
{
    using Trace.Models.TagSystem;
    using Trace.Models.Logic;
    using Trace.Repository.TagSystem.Tag;

    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Task<Tag> GetTagByIdAsync(Guid tagId) => _tagRepository.GetTagByIdAsync(tagId);
        public Task<IEnumerable<Tag>> GetAllTagsAsync() => _tagRepository.GetAllTagsAsync();
        public Task CreateTagAsync(string title, string color = "#FFFFFF", int iconId = 1)
        {
            var tag = new Tag { Id = Guid.NewGuid(), Title = title, Color = color, IconId = iconId };
            return _tagRepository.AddTagAsync(tag);
        }
        public Task UpdateTagAsync(Tag tag) => _tagRepository.UpdateTagAsync(tag);
        public Task DeleteTagAsync(Guid tagId) => _tagRepository.DeleteTagAsync(tagId);

        public Task AssignTagToFileAsync(Guid fileId, Guid tagId) => _tagRepository.AssignTagToFileAsync(fileId, tagId);
        public Task RemoveTagFromFileAsync(Guid fileId, Guid tagId) => _tagRepository.RemoveTagFromFileAsync(fileId, tagId);
        public Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId) => _tagRepository.GetFilesByTagAsync(tagId);
    }
}
