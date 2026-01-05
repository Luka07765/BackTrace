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

        public Task<Tag> GetTagByIdAsync(Guid tagId, string userId)
            => _tagRepository.GetTagByIdAsync(tagId, userId);

        public Task<IEnumerable<Tag>> GetAllTagsAsync(string userId)
            => _tagRepository.GetAllTagsAsync(userId);

        public Task CreateTagAsync(Guid id,string userId, string title, string color = "#FFFFFF", int iconId = 1)
        {
            var tag = new Tag
            {
                Id = id,

                Title = title,
                Color = color,
                IconId = iconId,
                UserId = userId  
            };

            return _tagRepository.AddTagAsync(tag);
        }

        public Task UpdateTagAsync(Tag tag)
            => _tagRepository.UpdateTagAsync(tag);

        public Task DeleteTagAsync(Guid tagId, string userId)
            => _tagRepository.DeleteTagAsync(tagId, userId);

        public async Task AssignTagToFileAsync(Guid fileId, Guid tagId, string userId)
        {
            // ✅ validate user owns both file and tag
            var tag = await _tagRepository.GetTagByIdAsync(tagId, userId);
            if (tag == null)
                throw new UnauthorizedAccessException("Tag not found or does not belong to this user.");

            // you should also fetch the File (from FileRepository) and check its UserId == userId here

            await _tagRepository.AssignTagToFileAsync(fileId, tagId);
        }

        public Task RemoveTagFromFileAsync(Guid fileId, Guid tagId, string userId)
            => _tagRepository.RemoveTagFromFileAsync(fileId, tagId);

        public Task<IEnumerable<File>> GetFilesByTagAsync(Guid tagId, string userId)
            => _tagRepository.GetFilesByTagAsync(tagId); // optionally filter by userId here
    }
}
