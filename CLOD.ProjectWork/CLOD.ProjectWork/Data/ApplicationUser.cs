using Microsoft.AspNetCore.Identity;

namespace CLOD.ProjectWork.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Plate { get; set; }
    }

}
