using SharedLib.DTOs;

namespace TLSWeb.Identity;

public interface IAccountManagement
{
    Task<AuthResult> LoginAsync(LoginModel credentials);
    Task<AuthResult> RegisterAsync(string email, string password);
    Task LogoutAsync();
}