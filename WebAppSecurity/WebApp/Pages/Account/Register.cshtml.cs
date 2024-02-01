using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Services;

namespace WebApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegisterModel(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [BindProperty]
        public UserProfile UserProfile { get; set; } = new UserProfile();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validate Email Address(optional)

            //var user = new User
            //{
            //    Email = UserProfile.Email,
            //    UserName = UserProfile.Email,
            //    Department = UserProfile.Department,
            //    Position = UserProfile.Position,
            //};

            // Create the user 
            var user = new User
            {
                Email = UserProfile.Email,
                UserName = UserProfile.Email,
            };

            var claimDepartment = new Claim("Department", UserProfile.Department);
            var claimPosition = new Claim("Position", UserProfile.Position);

            var result = await this.userManager.CreateAsync(user, UserProfile.Password);
            if (result.Succeeded)
            {
                await this.userManager.AddClaimAsync(user, claimDepartment);
                await this.userManager.AddClaimAsync(user, claimPosition);

                // Generate the token
                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user); // User Id will generated after var result

                // We will Comment this -------------------------
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                                               values: new { userId = user.Id, token = confirmationToken })?? "");

                //-----------------------------------------------
                // We will UnComment this -----------------------
                // To Send Email To New Users
                //-----------------------------------------------
                //var ConfirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                //                               values: new { userId = user.Id, token = confirmationToken });

                //await emailService.SendAsync("abdullah.goma.010170@gmail.com", user.Email, "Please Confirm Your Email"
                //    , $"Please click on this link to confirm your email: {ConfirmationLink}");

                //return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Register", error.Description);
                return Page();
            }
        }




        //    public async Task<IActionResult> OnPostAsync()
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Page();
        //        }

        //        // Validate Email Address (optional)

        //        // Create the user
        //        var user = new User
        //        {
        //            Email = UserProfile.Email,
        //            UserName = UserProfile.Email,
        //        };

        //        var result = await this.userManager.CreateAsync(user, UserProfile.Password);

        //        if (result.Succeeded)
        //        {
        //            var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        //            var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
        //                                               values: new { userId = user.Id, token = confirmationToken });

        //            await SendConfirmationEmail(user.Email, confirmationLink?? string.Empty);

        //            return RedirectToPage("/Account/Login");
        //        }
        //        else
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("Register", error.Description);
        //            }
        //            return Page();
        //        }
        //    }

        //    private async Task SendConfirmationEmail(string userEmail, string confirmationLink)
        //    {
        //        var message = new MailMessage
        //        {
        //            From = new MailAddress("MyEmail@gmail.com"),
        //            Subject = "Please Confirm Your Email",
        //            Body = $"Please click on this link to confirm your email: {confirmationLink}"
        //        };
        //        message.To.Add(userEmail);

        //        using (var emailClient = new SmtpClient("smtp-relay.brevo.com", 587))
        //        {
        //            emailClient.Credentials = new NetworkCredential("MyEmail@gmail.com", "MyPass");
        //            emailClient.EnableSsl = true;
        //            try
        //            {
        //                await emailClient.SendMailAsync(message);
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log or print the exception details for further analysis
        //                Console.WriteLine($"Error sending email: {ex.Message}");
        //                throw; // Rethrow the exception to maintain the original behavior
        //            }
        //        }
        //    }
    }
    public class UserProfile
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;

    }
}
