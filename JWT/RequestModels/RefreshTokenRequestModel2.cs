using System.ComponentModel.DataAnnotations;

namespace JWT.RequestModels;

public class RefreshTokenRequestModel2
{
    [Required] public string RefreshToken { get; set; } = null!;
}