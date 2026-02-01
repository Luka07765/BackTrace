using System.ComponentModel.DataAnnotations;

namespace Trace.Models.Data
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(32)]
        public string Title { get; set; } = default!;
    }
}
