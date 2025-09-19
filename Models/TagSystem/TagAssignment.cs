
namespace Trace.Models.TagSystem
{
    public class TagAssignment
    {
        public Guid FileId { get; set; }
        public Trace.Models.Logic.File File { get; set; }


        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
