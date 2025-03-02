using backend.Dtos;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public ActionResult<AuthenticateResponse> Register([FromBody, Required] RegisterRequest request)
        {
            var response = _authService.Register(request);
            if (response == null)
            {
                return NotFound(new { Message = "Registration failed" });
            }
            else if (response.StatusCode == 409)
            {
                return Conflict(new { Message = response.ErrorMessage });
            }
            else
            {
                return StatusCode(201, response.AuthenticateResponse);
            }
        }


        [HttpPost("login")]
        public ActionResult<AuthenticateResponse> Login([FromBody, Required] AuthenticateRequest request)
        {
            var response = _authService.Login(request);

            // Check if user not found
            if (response == null || response.StatusCode == 404)
            {
                return NotFound(new { Message = response.ErrorMessage });
            }
            // Check if unauthorized due to wrong password
            else if (response.StatusCode == 401)
            {
                return Unauthorized(new { Message = response.ErrorMessage });
            }
            // Success case
            else
            {
                return Ok(response.AuthenticateResponse);
            }
        }

    }
}
