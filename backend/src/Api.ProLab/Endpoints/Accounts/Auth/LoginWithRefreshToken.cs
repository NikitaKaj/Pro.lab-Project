using Ardalis.ApiEndpoints;
using ProLab.Data.Entities.Users;
using ProLab.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using ProLab.Data;

namespace ProLab.Api.Endpoints.Accounts.Auth;

public class LoginWithRefreshToken : EndpointBaseAsync
  .WithRequest<LoginWithRefreshTokenRequest>
  .WithActionResult<LoginResponse>
{
  private readonly UserManager<User> userManager;
  private readonly AppSettings appSettings;
  private readonly ILogger<LoginWithRefreshToken> logger;

  public LoginWithRefreshToken(
    UserManager<User> userManager,
    AppSettings appSettings,
    ILogger<LoginWithRefreshToken> logger)
  {
    this.userManager = userManager;
    this.appSettings = appSettings;
    this.logger = logger;
  }

  [AllowAnonymous]
  [HttpPost($"api/{Constants.ACCOUNTS}/refresh")]
  [OpenApiTag(Constants.ACCOUNTS)]
  [OpenApiOperation(Constants.ACCOUNTS + "_" + nameof(LoginWithRefreshToken))]
  public override async Task<ActionResult<LoginResponse>> HandleAsync(LoginWithRefreshTokenRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      var ipAddress = this.ipAddress();

      var refreshTokenString = request.RefreshToken;

      var user = await userManager.Users
        .Include(x => x.RefreshTokens.Where(t => t.Token == refreshTokenString))
        .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshTokenString));

      var refreshToken = user?.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenString);

      if (user == null || refreshToken == null)
      {
        throw new Exception("Invalid token");
      }

      var newRefreshToken = JwtTokenHelper.GenerateRefreshToken(ipAddress);
      refreshToken.Revoked = DateTimeOffset.UtcNow;
      refreshToken.RevokedByIp = ipAddress;
      refreshToken.ReplacedByToken = newRefreshToken.Token;
      user.RefreshTokens.Add(newRefreshToken);
      await userManager.UpdateAsync(user);

      var jwtToken = JwtTokenHelper.GenerateJwtToken(user, appSettings.Secret);

      var result = new LoginResponse(jwtToken, newRefreshToken.Token);

      return Ok(result);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "LoginWithRefreshToken failed");
      return BadRequest();
    }
  }

  private string ipAddress()
  {
    if (Request.Headers.ContainsKey("X-Forwarded-For"))
      return Request.Headers["X-Forwarded-For"];
    else
      return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
  }
}
public class LoginWithRefreshTokenRequest
{
  public string RefreshToken { get; set; }
}