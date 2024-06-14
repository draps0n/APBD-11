using System.ComponentModel.DataAnnotations;

namespace JWT.RequestModels;

public class RefreshTokenRequestModel
{
    [Required] public string RefreshToken { get; set; } = null!;
}