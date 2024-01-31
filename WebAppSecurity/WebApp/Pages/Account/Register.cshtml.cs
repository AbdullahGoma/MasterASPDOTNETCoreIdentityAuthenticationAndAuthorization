using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace WebApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validate Email Address(optional)

            // Create the user 
            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
            };
            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);
            if (result.Succeeded)
            {
                // Generate the token
                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user); // User Id will generated after var result
                var ConfirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                                               values: new { userId = user.Id, token = confirmationToken });
                
                var message = new MailMessage("MyEmail@gmail.com", user.Email, "Please Confirm Your Email"
                    , $"Please click on this link to confirm your email: {ConfirmationLink}");

                using (var emailClient = new SmtpClient("smtp-relay.brevo.com", 587))
                {
                    emailClient.Credentials = new NetworkCredential("MyEmail@gmail.com", "MyPass");
                    emailClient.EnableSsl = true;
                    await emailClient.SendMailAsync(message);
                }

                return RedirectToPage("/Account/Login");
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
        //        var user = new IdentityUser
        //        {
        //            Email = RegisterViewModel.Email,
        //            UserName = RegisterViewModel.Email,
        //        };

        //        var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);

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
        public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}
