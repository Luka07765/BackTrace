using Trace.Models.Data;

namespace Trace.Data.Seeds
{
    public static class RoleSeed
    {
        public static readonly Guid Skill = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Model = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static IEnumerable<Role> Get() => new[]
        {
        new Role { Id = Skill, Title = "Skill" },
        new Role { Id = Model, Title = "Model" }
    };
    }

}
