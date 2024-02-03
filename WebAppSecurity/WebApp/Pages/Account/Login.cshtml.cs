using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        [BindProperty]
        public CredentialVeiwModel Credential { get; set; } = new CredentialVeiwModel();

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public async Task OnGetAsync()
        {
            // signInManager knows we have external provider because we have configured it in Program.cs
            this.ExternalLoginProviders = await signInManager.GetExternalAuthenticationSchemesAsync(); 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            var result = await signInManager.PasswordSignInAsync(
                              this.Credential.Email, 
                              this.Credential.Password, 
                              this.Credential.RememberMe, 
                              false);

            if(result.Succeeded)
                return RedirectToPage("/Index");
            else
            {
                //if (result.RequiresTwoFactor) return RedirectToPage("/Account/LoginTwoFactor", 
                //    new { Email = this.Credential.Email,
                //          RememberMe = this.Credential.RememberMe
                //    }); // as parameters in get method
                    
                if (result.RequiresTwoFactor) return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", 
                    new { this.Credential.RememberMe }); // as parameters in get method
                if (result.IsLockedOut) ModelState.AddModelError("Login", "You are Locked Out.");
                else ModelState.AddModelError("Login", "Failed To Login");
                return Page();
            }

        }

        // provider look for attribute name in button
        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, null); // contains Informations
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");
            return Challenge(properties, provider);
        }
    }

    public class CredentialVeiwModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }



}
