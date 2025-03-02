using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AppSettings _appSettings;

        public AuthService(ApplicationDbContext context, ITokenService tokenService, IOptions<AppSettings> options)
        {
            _context = context;
            _tokenService = tokenService;
            _appSettings = options.Value;
        }

        public AuthResult Register(RegisterRequest request)
        {
            // Check if the user with the same email or username already exists
            var existingUser = _context.Users.SingleOrDefault(x => x.Email == request.Email || x.UserName == request.UserName);

            if (existingUser != null)
            {
                return new AuthResult
                {
                    StatusCode = 409,
                    ErrorMessage = "Email or Username already exists"
                };
            }

            // Hash the password before storing it
            string hashedPassword = HashPassword(request.Password);

            // Create a new user object with the hashed password
            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = hashedPassword, // Store hashed password
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.MinValue // Default value for new users
            };

            // Add the new user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Generate JWT and Refresh tokens
            var claims = new List<Claim>
            {
                new Claim("UserID", newUser.Id.ToString()),
                new Claim("email", newUser.Email.ToString()),
                new Claim("UserName", newUser.UserName ?? "")
            };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Update the user with the refresh token and expiry
            newUser.RefreshToken = refreshToken;
            newUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(_appSettings.JwtRefreshExpireDays);
            _context.SaveChanges();

            return new AuthResult
            {
                StatusCode = 201,
                AuthenticateResponse = new AuthenticateResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public AuthResult Login(AuthenticateRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserName == request.UserName);

            if (user == null)
            {
                return new AuthResult
                {
                    StatusCode = 404,
                    ErrorMessage = "Email does not exist"
                };
            }
            else if (!VerifyPassword(request.Password, user.Password)) // Verify hashed password
            {
                return new AuthResult
                {
                    StatusCode = 401,
                    ErrorMessage = "Email and password are incorrect"
                };
            }
            else
            {
                var claims = new List<Claim>
            {
                new Claim("UserID", user.Id.ToString()),
                new Claim("email", user.Email.ToString()),
                new Claim("UserName", user.UserName ?? "")
            };
                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Save refresh token and expiry time
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_appSettings.JwtRefreshExpireDays);
                _context.SaveChanges();

                return new AuthResult
                {
                    StatusCode = 200,
                    AuthenticateResponse = new AuthenticateResponse
                    {
                        Token = accessToken,
                        RefreshToken = refreshToken
                    }
                };
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
        }
    }
}
