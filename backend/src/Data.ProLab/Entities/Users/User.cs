using ProLab.Data.Shared;
using ProLab.Data.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProLab.Data.Entities.Users;

public class User : IdentityUser<long>, IAggregateRoot, ITrackable
{
	public User()
	{

	}

	public User(string email)
	{
		UserName = email.ToLower();
		NormalizedUserName = email.ToUpper();
		Email = email.ToLower();
		NormalizedEmail = email.ToUpper();
	}

	public static User New(string firstName, string lastName, string email, string country, bool agreedToPrivacyPolicy)
	{
		var user = new User(email);
		user.FirstName = firstName;
		user.LastName = lastName;
		user.Country = country;
		user.AgreedToPrivacyPolicy = agreedToPrivacyPolicy;

		return user;
	}

	[StringLength(100)]
	public string FirstName { get; set; }

	[StringLength(100)]
	public string LastName { get; set; }

	[StringLength(100)]
	public string Country { get; set; }
	
	[StringLength(100)]
	public string? City { get; set; }

	[StringLength(100)]
	public string? ZipCode { get; set; }
	
	[StringLength(100)]
	public string? Address { get; set; }

	public bool AgreedToPrivacyPolicy { get; set; }

	public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? UpdatedAt { get; set; }
}