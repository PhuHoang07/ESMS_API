using Business.Services.AuthService;
using ESMS_Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ESMS_API.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
        }

        [HttpGet]
        [Route("validate")]
        public IActionResult ValidateToken()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok("Valid");
            }
            else
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(jwtToken) as JwtSecurityToken;

                if (token.ValidTo < DateTime.UtcNow)
                {
                    return Ok("Token has expired, please log in again");
                }

                return Ok("Invalid");
            }
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserModel userModel)
        {
            var res = await _authService.Authenticate(userModel);

            if (res.Data != null && res.StatusCode == 200)
            {
                var user = (UserModel) res.Data;
                res = _authService.GenerateToken(user);
            }

            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }        
    }
}