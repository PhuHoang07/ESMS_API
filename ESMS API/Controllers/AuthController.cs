using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpGet("accesstoken")]
        public IActionResult GetAccessToken()
        {
            var authenticateResult = HttpContext.AuthenticateAsync().Result;

            if (authenticateResult.Succeeded)
            {
                var accessToken = authenticateResult.Properties.GetTokenValue("access_token");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    return Ok(new { AccessToken = accessToken });
                }
            }

            return BadRequest("Access token not found.");
        }
    }


}
