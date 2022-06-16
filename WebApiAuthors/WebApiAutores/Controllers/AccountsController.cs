using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HashService _hashService;
        private readonly IDataProtector _dataProtector;

        public AccountsController(UserManager<IdentityUser> userManager,
            IConfiguration configuration, SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _hashService = hashService;
            _dataProtector = dataProtectionProvider.CreateProtector("secret_value");
        }

        [HttpGet("encrypt")]
        public ActionResult Encrypt()
        {
            var text = "Alejandro Oliva";
            var textEncrypted = _dataProtector.Protect(text);
            var textDecrypted = _dataProtector.Unprotect(textEncrypted);

            return Ok(new { text, textEncrypted, textDecrypted });
        }

        [HttpGet("encryptByTime")]
        public ActionResult EncryptByTime()
        {
            var protectorByTime = _dataProtector.ToTimeLimitedDataProtector();

            var text = "Alejandro Oliva";
            var textEncrypted = protectorByTime.Protect(text, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textDecrypted = protectorByTime.Unprotect(textEncrypted);

            return Ok(new { text, textEncrypted, textDecrypted });
        }


        [HttpGet("hash/{text}")]
        public ActionResult HashText(string text)
        {
            var result1 = _hashService.Hash(text);
            var result2 = _hashService.Hash(text);

            return Ok(new { text, Hash1 = result1, Hash2 = result2 });
        }

        [HttpPost("register", Name = "registerUser")]
        public async Task<ActionResult<ResponseAuth>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser() { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login", Name = "loginUser")]
        public async Task<ActionResult<ResponseAuth>> Login(UserCredentials userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Login Error");
            }
        }

        [HttpGet("renewToken", Name = "renewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseAuth>> Renew()
        {
            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;
            var creds = new UserCredentials() { Email = email };

            return await BuildToken(creds);
        }

        [HttpPost("makeAdmin", Name = "makeAdmin")]
        public async Task<ActionResult> MakeAdmin(EditAdminDTO editAdminDto)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
            await _userManager.AddClaimAsync(user, new Claim("isAdmin", "true"));
            return NoContent();
        }

        [HttpPost("removeAdmin", Name = "removeAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDto)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDto.Email);
            await _userManager.RemoveClaimAsync(user, new Claim("isAdmin", "true"));
            return NoContent();
        }

        private async  Task<ResponseAuth> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await _userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["secretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // var expiration = DateTime.UtcNow.AddMinutes(30);
            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            return new ResponseAuth()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }
    }
}
