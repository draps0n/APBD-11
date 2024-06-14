using JWT.Context;
using JWT.Exceptions;
using JWT.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT.Services;

public class AppUserService(DatabaseContext context) : IAppUserService
{
    public async Task RegisterNewUserAsync(string username, string passwordSaltHash, string salt)
    {
        await CheckIfUsernameIsUnique(username);

        var user = new AppUser
        {
            Username = username,
            Password = passwordSaltHash,
            Salt = salt
        };

        await context.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        var user = await context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        if (user is null) throw new UnauthorizedException("Błędny login lub hasło!");

        return user;
    }

    public async Task AddNewRefreshToken(AppUser user, string refToken)
    {
        user.RefreshToken = refToken;
        await context.SaveChangesAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int userId)
    {
        var user = await context.Users.Where(u => u.IdUser == userId).FirstOrDefaultAsync();
        if (user is null) throw new UnauthorizedException("Nieuprawniony dostęp");

        return user;
    }

    private async Task CheckIfUsernameIsUnique(string username)
    {
        var user = await context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        if (user is not null) throw new BadRequestException("Użytkownik o takim nicku już istnieje!");
    }
}