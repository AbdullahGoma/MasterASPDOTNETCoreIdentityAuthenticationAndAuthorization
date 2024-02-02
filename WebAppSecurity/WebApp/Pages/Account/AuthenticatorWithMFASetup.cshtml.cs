using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public SetupMFAViewModel ViewModel { get; set; }
        [BindProperty]
        public bool Succeeded { get; set; } 
        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.ViewModel = new SetupMFAViewModel();
            this.Succeeded = false;

        }


        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                var key = await userManager.GetAuthenticatorKeyAsync(user); // In First time was return null with me
                // Fix the problem
                if (string.IsNullOrEmpty(key))
                {
                    // if no key exists, generate one and persist it
                    await userManager.ResetAuthenticatorKeyAsync(user);
                    // get the key we just created
                    key = await userManager.GetAuthenticatorKeyAsync(user);
                }
                this.ViewModel.Key = key?? string.Empty;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await userManager.GetUserAsync(User);
            if (user != null && await userManager.VerifyTwoFactorTokenAsync(user, 
                userManager.Options.Tokens.AuthenticatorTokenProvider,
                this.ViewModel.SecurityCode))
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
                this.Succeeded = true;
            }
            else
                ModelState.AddModelError("AuthenticaterSetup", "Something went wrong with authenticator setup.");

            return Page();
        }

    }

    public class SetupMFAViewModel
    {
        public string Key { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; } = string.Empty;

        public Byte[]? QRCodeBytes { get; set; }
    }
}
