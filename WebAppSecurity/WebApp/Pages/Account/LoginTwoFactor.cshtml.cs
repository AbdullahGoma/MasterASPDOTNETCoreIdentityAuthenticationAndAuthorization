using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;
using WebApp.Services;
using WebApp.Settings;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        private readonly IOptions<STMPSetting> smtpSetting;
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; }

        public LoginTwoFactorModel(UserManager<User> userManager, 
            IEmailService emailService, 
            IOptions<STMPSetting> smtpSetting,
            SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.smtpSetting = smtpSetting;
            this.signInManager = signInManager;
            this.EmailMFA = new EmailMFA();
        }
        public async Task OnGetAsync(string email, bool rememberMe)
        {
            var user = await userManager.FindByEmailAsync(email);

            this.EmailMFA.SecurityCode = string.Empty;
            this.EmailMFA.RememberMe = rememberMe;

            // Generate the code
            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");


            // Send to the user
            await emailService.SendAsync(smtpSetting.Value.User,
                email,
                "My Web App's One Time Password", 
                $"Please use this code as the OTP: {securityCode}"
                );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorSignInAsync("Email", 
                EmailMFA.SecurityCode, 
                EmailMFA.RememberMe,
                false);

            if (result.Succeeded)
                return RedirectToPage("/Index");
            else
            {
                if (result.IsLockedOut) ModelState.AddModelError("Login2FA", "You are Locked Out.");
                else ModelState.AddModelError("Login2FA", "Failed To Login");
                return Page();
            }

        }
    }

    public class EmailMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; } 
    }

}
