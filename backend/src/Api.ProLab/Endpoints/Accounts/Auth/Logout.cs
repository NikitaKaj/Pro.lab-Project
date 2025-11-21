using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProLab.Api.Responses;
using ProLab.Data;
using ProLab.Data.Entities.Users;

namespace ProLab.Api.Endpoints.Accounts.Auth
{
    public class Logout : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult
    {
    private readonly ApplicationDbContext context;
    private readonly UserManager<User> userManager;
    private readonly ILogger<Logout> logger;

    public Logout(ApplicationDbContext context, UserManager<User> userManager, ILogger<Logout> logger)
    {
        this.context = context;
        this.userManager = userManager;
        this.logger = logger;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost($"api/{Constants.ACCOUNTS}/logout")]
    [OpenApiTag(Constants.ACCOUNTS)]
    [OpenApiOperation(Constants.ACCOUNTS + "_" + nameof(Logout))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("You're not authorized");
            }

            var query = context.Users
                .Include(u => u.RefreshTokens)
                .AsQueryable();

            User? user = null;

            if (!string.IsNullOrEmpty(userIdClaim) && long.TryParse(userIdClaim, out var userId))
            {
                user = await query.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            }

            if (user == null)
            {
                return Unauthorized("You're not authorized");
            }

            user.RefreshTokens.Clear();
            await context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Logout failed");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        }
    }
}