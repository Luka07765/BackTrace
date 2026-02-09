using Trace.Repository.Color;

using Trace.Repository.Files.Fetch;
using Trace.Repository.Files.Modify;
using Trace.Repository.Folder.Fetch.Progressive;
using Trace.Repository.Folder.Fetch.Query;
using Trace.Repository.Folder.Modify;
using Trace.Repository.TagSystem.Tag;

using Trace.Service.Files.Fetch;
using Trace.Service.Files.Modify;
using Trace.Service.Folder.Fetch.Progressive;
using Trace.Service.Folder.Fetch.Query;
using Trace.Service.Folder.Modify;
using Trace.Service.Profile;
using Trace.Service.Search;
using Trace.Service.Tag;
namespace Trace.Registrations
{
    public static class QueryModify_Registration
    {
        public static IServiceCollection Register_QueryAndModify(this IServiceCollection services)
        {
            services.AddScoped<IFileQueryRepository, FileQueryRepository>();
            services.AddScoped<IFileQueryService, FileQueryService>();
            services.AddScoped<IFileModifyRepository, FileModifyRepository>();
            services.AddScoped<IFileModifyService, FileModifyService>();

            services.AddScoped<IFolderProgressiveRepository, FolderProgressiveRepository>();
            services.AddScoped<IFolderQueryRepository, FolderQueryRepository>();
            services.AddScoped<IFolderProgressiveService, FolderProgressiveService>();
          
            services.AddScoped<IFolderQueryService, FolderQueryService>();
            services.AddScoped<IFolderModifyRepository, FolderModifyRepository>();
            services.AddScoped<IFolderModifyService, FolderModifyService>();
            services.AddScoped<IColorRepository, ColorRepository>();    
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ISearchQueryService, SearchQueryService>();
            services.AddScoped<IProfileService, ProfileService>();



            return services;
        }
    }
}
