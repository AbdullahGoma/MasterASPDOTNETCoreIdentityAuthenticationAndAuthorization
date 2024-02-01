using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;
        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }
        [BindProperty]
        public string? SuccessMessage { get; set; } // ? must put

        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.UserProfile = new UserProfileViewModel();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
            if(user != null)
            {
                this.UserProfile.Email = User.Identity?.Name ?? string.Empty;
                this.UserProfile.Department = departmentClaim?.Value ?? string.Empty;
                this.UserProfile.Position = positionClaim?.Value ?? string.Empty;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            this.SuccessMessage = string.Empty;
            
            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

            try
            {
                if (user != null && departmentClaim != null)
                    await userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));

                if (user != null && positionClaim != null)
                    await userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
            }
            catch 
            {
                ModelState.AddModelError("UserProfile", "Error occured during updating user profile.");
            }

            this.SuccessMessage = "The User Profile is updated successfully.";

            return Page();
        }

        // Return Tuple
        private async Task<(User? user, Claim? departmentClaim, Claim? positionClaim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");
                return (user, departmentClaim, positionClaim);
            }
            return (null, null, null);
        }
    }

    public class UserProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }

}
