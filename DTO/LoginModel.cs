using System.ComponentModel.DataAnnotations;

namespace Trace.DTO
{
    public class LoginModel
    {
        [EmailAddress]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}