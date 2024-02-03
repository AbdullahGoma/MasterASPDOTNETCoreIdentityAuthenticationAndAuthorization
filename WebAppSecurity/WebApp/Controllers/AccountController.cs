using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }

        // Get Informations of user to sing in
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // return all claims
            var loginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (loginInfo != null)
            {
                var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                var userClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                if(emailClaim != null && userClaim != null)
                {
                    var user = new User { Email = emailClaim.Value, UserName = userClaim.Value };
                    await signInManager.SignInAsync(user, false);
                }
            }
            return RedirectToPage("/Index"); // If user does not sign in, it will redirect him to the login page because not sign in yet.
        }


    }
}
