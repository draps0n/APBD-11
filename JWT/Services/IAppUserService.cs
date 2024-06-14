using JWT.Models;

namespace JWT.Services;

public interface IAppUserService
{
    Task RegisterNewUserAsync(string username, string passwordSaltHash, string salt);
    Task<AppUser> GetUserByUsernameAsync(string username);

    Task AddNewRefreshToken(AppUser user, string refToken);
    Task<AppUser> GetUserByIdAsync(int userId);
}