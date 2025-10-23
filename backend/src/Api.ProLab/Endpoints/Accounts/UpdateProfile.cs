using Ardalis.ApiEndpoints;
using ProLab.Api.Endpoints.Accounts.Auth;
using ProLab.Api.Infrastructure.Extensions;
using ProLab.Data.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProLab.Api.Endpoints.Accounts;

public class UpdateProfile : EndpointBaseAsync
	.WithRequest<UpdateProfileRequest>
	.WithActionResult<SuccessResult>
{
	private readonly UserManager<User> userManager;

	public UpdateProfile(UserManager<User> userManager)
	{
		this.userManager = userManager;
	}

	[JwtAuthorize]
	[HttpPost("api/accounts/update")]
	[OpenApiTag(Constants.ACCOUNTS)]
	[OpenApiOperation(Constants.ACCOUNTS + "_" + nameof(UpdateProfile))]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(SuccessResult), StatusCodes.Status200OK)]
	public override async Task<ActionResult<SuccessResult>> HandleAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default)
	{
		var userId = User.GetUserId();

		var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
		if (user == null) return ValidationProblem(ModelState.AddModelErrorF("", "User not found."));

		user.FirstName = request.FirstName;
		user.LastName = request.LastName;
		user.Country = request.Country;
		user.City = request.City;
		user.ZipCode = request.ZipCode;
		user.Address = request.Address;

		await userManager.UpdateAsync(user);
    
		return Ok(new SuccessResult());
	}
}

public class UpdateProfileRequest
{
	[Required]
	[StringLength(100)]

	public required string FirstName { get; set; }

	[Required]
	[StringLength(100)]
	public required string LastName { get; set; }

	[Required]
	[StringLength(100)]
	public required string Country { get; set; }
	
	[StringLength(100)]
	public required string? City { get; set; }
	
	[StringLength(100)]
	public required string? ZipCode { get; set; }
	
	[StringLength(100)]
	public required string? Address { get; set; }
}