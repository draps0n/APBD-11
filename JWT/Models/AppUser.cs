using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT.Models;

[Table("Users")]
public class AppUser
{
    [Key] public int IdUser { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string? RefreshToken { get; set; }
}