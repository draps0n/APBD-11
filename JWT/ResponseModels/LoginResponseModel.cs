namespace JWT.ResponseModels;

public class LoginResponseModel
{
    public string JwtToken { get; set; }
    public string RefToken { get; set; }
}