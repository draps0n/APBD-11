using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT.Exceptions;
using JWT.Helpers;
using JWT.Models;
using JWT.RequestModels;
using JWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.CompilerServices;

namespace JWT.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAppUserService appUserService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterNewAppUser([FromBody] RegisterRequestModel requestModel)
    {
        var hashedPasswordAndSalt = SecurityHelpers.GetHashedPasswordAndSalt(requestModel.Password);
        try
        {
            await appUserService.RegisterNewUserAsync(
                requestModel.Username,
                hashedPasswordAndSalt.Item1,
                hashedPasswordAndSalt.Item2
            );
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }


        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAppUser([FromBody] LoginRequestModel2 requestModel)
    {
        var user = await appUserService.GetUserByUsernameAsync(requestModel.Username);

        var passwordFromDb = user.Password;
        var passwordFromReq = SecurityHelpers.GetHashedPasswordWithSalt(requestModel.Password, user.Salt);

        if (passwordFromDb != passwordFromReq) return Unauthorized();

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.IdUser.ToString())
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescription = new SecurityTokenDescriptor
        {
            Issuer = configuration["JWT:Issuer"],
            Audience = configuration["JWT:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(15),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)),
                SecurityAlgorithms.HmacSha256
            )
        };
        var token = tokenHandler.CreateToken(tokenDescription);
        var stringToken = tokenHandler.WriteToken(token);

        var refTokenDescription = new SecurityTokenDescriptor
        {
            Issuer = configuration["JWT:RefIssuer"],
            Audience = configuration["JWT:RefAudience"],
            Expires = DateTime.UtcNow.AddDays(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:RefKey"]!)),
                SecurityAlgorithms.HmacSha256
            )
        };
        var refToken = tokenHandler.CreateToken(refTokenDescription);
        var stringRefToken = tokenHandler.WriteToken(refToken);

        await appUserService.AddNewRefreshToken(user, stringRefToken);

        return Ok(new ResponseModels.LoginResponseModel
        {
            JwtToken = stringToken,
            RefToken = stringRefToken
        });
    }

    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestModel2 requestModel)
    {
        var userId = IntegerType.FromString(User.FindFirstValue(ClaimTypes.NameIdentifier));
        AppUser user;
        try
        {
            user = await appUserService.GetUserByIdAsync(userId);
        }
        catch (UnauthorizedException)
        {
            return Unauthorized("Invalid token");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(requestModel.RefreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:RefIssuer"],
                ValidAudience = configuration["JWT:RefAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:RefKey"]!))
            }, out var validatedToken);
        }
        catch
        {
            return Unauthorized("Invalid token");
        }

        var tokenDescription = new SecurityTokenDescriptor
        {
            Issuer = configuration["JWT:Issuer"],
            Audience = configuration["JWT:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)),
                SecurityAlgorithms.HmacSha256
            )
        };
        var token = tokenHandler.CreateToken(tokenDescription);
        var stringToken = tokenHandler.WriteToken(token);

        var refTokenDescription = new SecurityTokenDescriptor
        {
            Issuer = configuration["JWT:RefIssuer"],
            Audience = configuration["JWT:RefAudience"],
            Expires = DateTime.UtcNow.AddDays(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:RefKey"]!)),
                SecurityAlgorithms.HmacSha256
            )
        };
        var refToken = tokenHandler.CreateToken(refTokenDescription);
        var stringRefToken = tokenHandler.WriteToken(refToken);

        await appUserService.AddNewRefreshToken(user, stringRefToken);

        return Ok(new LoginResponseModel
        {
            Token = stringToken,
            RefreshToken = stringRefToken
        });
    }

    [HttpGet("secret-data")]
    [Authorize]
    public IActionResult GetSecretData()
    {
        return Ok("Kocham PJATK!");
    }
}