using Microsoft.AspNetCore.Identity;

namespace WorkoutWebApplication.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
