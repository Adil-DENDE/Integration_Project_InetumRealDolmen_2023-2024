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
        // DE NAAM VAN DE USER OPHALEN //
        public string GetUsername()
        {
            var token = GetToken().Trim('\"');
            if (string.IsNullOrEmpty(token))
            {
                return "Error";
            }

            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);
            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return usernameClaim?.Value;
        }

        // DE ID VAN DE USER OPHALEN //
        public string GetIdFromLoggedUser()
        {
            var token = GetToken().Trim('\"');
            if (string.IsNullOrEmpty(token))
            {
                
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return usernameClaim?.Value;
        }
    }
}
