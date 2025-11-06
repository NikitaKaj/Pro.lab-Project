using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProLab.Data.Entities.Users;

namespace ProLab.Api.Infrastructure;

public static class JwtTokenHelper
{
  public static string GenerateJwtToken(User user, string secret, int seconds = 60 * 60)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(secret);

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Name, user.UserName.ToString()),
      new Claim(ClaimTypes.Email, user.Email.ToString()),
      new Claim(JwtRegisteredClaimNames.Sub, user.Email.ToString()),
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddSeconds(seconds),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  public static string GenerateJwtToken(string wallet, string secret)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(secret);

    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, wallet),
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddMinutes(60),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  public static UserRefreshToken GenerateRefreshToken(string ipAddress)
  {
    using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
    {
      var randomBytes = new byte[64];
      rngCryptoServiceProvider.GetBytes(randomBytes);
      return new UserRefreshToken
      {
        Token = Convert.ToBase64String(randomBytes),
        Expires = DateTime.UtcNow.AddDays(7),
        Created = DateTime.UtcNow,
        CreatedByIp = ipAddress
      };
    }
  }
}
