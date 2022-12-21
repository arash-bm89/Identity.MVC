using Microsoft.Build.Framework;

namespace Identity.MVC.Models.DTO
{
    public class UserDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
