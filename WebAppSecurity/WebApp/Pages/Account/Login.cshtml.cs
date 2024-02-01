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
        public void OnGet()
        {
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
                if (result.IsLockedOut) ModelState.AddModelError("Login", "You are Locked Out.");
                else ModelState.AddModelError("Login", "Failed To Login");
                return Page();
            }

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
