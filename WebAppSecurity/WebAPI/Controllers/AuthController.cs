using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration confeguration;

        public AuthController(IConfiguration confeguration)
        {
            this.confeguration = confeguration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody]Credential credential)
        {
            // Verify the credential
            if (credential.Username == "admin" && credential.Password == "password")
            {
                // Creating the security context
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2023-9-01")
                };
                
                var expiresAt = DateTime.UtcNow.AddMinutes(1);

                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });

            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access endpoint.");
            return Unauthorized(ModelState);

        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            var SecretKey = Encoding.ASCII.GetBytes(confeguration.GetValue<string>("SecretKey")?? "");

            // generate the Jason Web Token 'JWT'
            var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expireAt,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }




    public class Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
