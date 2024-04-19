using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace RealDolmenAPI.Services
{
    public class AuthService
{
    private string _token;

    public void SetToken(string token)
    {
        _token = token;
    }

    public string GetToken()
    {
        return _token;
    }

    public void ClearToken()
    {
        _token = null;
    }
        public string GetUsername()
        {
            var token = GetToken().Trim('\"');
            if (string.IsNullOrEmpty(token))
            {
                return "Geen naam";
            }

            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);
            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return usernameClaim?.Value;
        }
    }
}
