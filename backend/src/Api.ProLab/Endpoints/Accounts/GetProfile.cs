using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using ProLab.Data.Entities.Users;
using ProLab.Api.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProLab.Api.Endpoints.Accounts;

public class GetProfile : EndpointBaseAsync
	.WithoutRequest
	.WithActionResult<GetProfileInfoResponse>
{
	private readonly UserManager<User> userManager;
	private readonly ILogger<GetProfile> logger;

	public GetProfile(UserManager<User> userManager, ILogger<GetProfile> logger)
	{
		this.userManager = userManager;
		this.logger = logger;
	}

	[JwtAuthorize]
	[HttpGet($"api/{Constants.ACCOUNTS}/profile")]
	[OpenApiTag(Constants.ACCOUNTS)]
	[OpenApiOperation(Constants.ACCOUNTS + "_" + nameof(GetProfile))]
	public override async Task<ActionResult<GetProfileInfoResponse>> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var userId = User.GetUserId();
			var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
			{
				ModelState.AddModelError("", "User not found.");
				return ValidationProblem(ModelState);
			}

			return Ok(new GetProfileInfoResponse()
			{
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Country = user.Country,
				City = user.City,
				ZipCode = user.ZipCode,
				Address = user.Address,
			});
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "GetProfile failed");
			return BadRequest();
		}
	}
}

public class GetProfileInfoResponse
{
	public required string Email { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string Country { get; set; }
	public required string? City { get; set; }
	public required string? ZipCode { get; set; }
	public required string? Address { get; set; }
}
