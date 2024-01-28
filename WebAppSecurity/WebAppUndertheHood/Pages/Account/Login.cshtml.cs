using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebAppUndertheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
            //this.Credential = new Credential() { UserName = "admin"};
        }

        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid) return Page(); //Login

            // Verify the credential
            if(Credential.UserName == "admin" && Credential.Password == "password")
            {
                // Creating the security context
                var claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2024-01-01")
                };
                var identity = new ClaimsIdentity(claims,"MyCookieAuth");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                // Serialize the claims principal into string then encrypt that string and save that as a cookie
                await HttpContext.SignInAsync("MyCookieAuth", principal, authProperties); // Request Encapsulated into HttpContext
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }    
    }
}
