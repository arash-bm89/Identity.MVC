using Microsoft.AspNetCore.Identity;

namespace Identity.MVC.Models
{
    public class User: ModelBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
