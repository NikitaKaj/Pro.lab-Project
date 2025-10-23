using Ardalis.ApiEndpoints;
using ProLab.Data;
using ProLab.Data.Entities.Users;
using ProLab.Api.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ProLab.Api.Endpoints.Accounts.Auth;

public class Login : EndpointBaseAsync
  .WithRequest<LoginRequest>
  .WithActionResult<LoginResponse>
{
  private readonly ApplicationDbContext ctx;
  private readonly SignInManager<User> signInManager;
  private readonly UserManager<User> userManager;
  private readonly AppSettings appSettings;
  private readonly ILogger<Login> logger;

  public Login(ApplicationDbContext ctx,
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    AppSettings appSettings,
    ILogger<Login> logger)
  {
    this.ctx = ctx;
    this.signInManager = signInManager;
    this.userManager = userManager;
    this.appSettings = appSettings;
    this.logger = logger;
  }

  [AllowAnonymous]
  [HttpPost($"api/{Constants.ACCOUNTS}/login")]
  [OpenApiTag(Constants.ACCOUNTS)]
  [OpenApiOperation(Constants.ACCOUNTS + "_" + nameof(Login))]
  [ProducesResponseType(typeof(ActionResult<LoginResponse>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  public override async Task<ActionResult<LoginResponse>> HandleAsync([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await FindUser(request.Email);
      if (user == null)
      {
        ModelState.AddModelError("", "Incorrect password.");
        return ValidationProblem(ModelState);
      }

      var signInRes = await signInManager.PasswordSignInAsync(user.Email, request.Password, false, false);

      if (signInRes.Succeeded)
      {
        var accessAndrefreshTokens = await GenerateAccessAndRefreshTokensForUser(user);
        return Ok(new LoginResponse(accessAndrefreshTokens.accessToken, accessAndrefreshTokens.refreshToken));
      }
      else if (signInRes.RequiresTwoFactor)
      {
        return new LoginResponse();
      }
      else if (signInRes.IsNotAllowed)
      {
        ModelState.AddModelError("", "Please check your email and confirm your account");
        return ValidationProblem(ModelState);
      }
      else
      {
        ModelState.AddModelError("", "Unable to login. Check your Email or Password.");
        return ValidationProblem(ModelState);
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Login failed");
      return BadRequest();
    }
  }

  private async Task<User?> FindUser(string email) =>
    await ctx.Users
      .Include(x => x.RefreshTokens)
      .Where(u => u.Email == email.ToLower())
      .FirstOrDefaultAsync();

  private string IpAddress()
  {
    if (Request.Headers.ContainsKey("X-Forwarded-For"))
      return Request.Headers["X-Forwarded-For"];
    else
      return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
  }

  private async Task<(string accessToken, string refreshToken)> GenerateAccessAndRefreshTokensForUser(User user)
  {
    var jwtToken = JwtTokenHelper.GenerateJwtToken(user, appSettings.Secret);

    var refreshToken = JwtTokenHelper.GenerateRefreshToken(IpAddress());
    user.RefreshTokens.Add(refreshToken);
    await userManager.UpdateAsync(user); // todo do we need to revoke previous refresh tokens since they are active (?)

    return (jwtToken, refreshToken.Token);
  }
}

public class LoginRequest
{
  [Required]
  public string Email { get; set; }
  [Required]
  public string Password { get; set; }
}

public class LoginResponse
{
  public LoginResponse(string accessToken, string refreshToken)
  {
    AccessToken = accessToken;
    RefreshToken = refreshToken;
    Requires2Fa = false;
  }

  public LoginResponse()
  {
    Requires2Fa = true;
  }

  public string? AccessToken { get; set; }
  public string? RefreshToken { get; set; }
  public bool Requires2Fa { get; set; }
}