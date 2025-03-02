using backend.Dtos;
using backend.Models;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IAuthService
    {
        AuthResult Register(RegisterRequest request);
        AuthResult Login(AuthenticateRequest request);
    }
}
