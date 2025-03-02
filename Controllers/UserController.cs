using backend.Data;
using backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("personal/me")]
        public ActionResult<UserDto> GetPersonalDetails()
        {
            int userID = GetUserId();

            // Fetch the user details from the database
            var user = _context.Users.SingleOrDefault(u => u.Id == userID);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Create a DTO to return only necessary fields
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return Ok(userDto);
        }
    }
}
