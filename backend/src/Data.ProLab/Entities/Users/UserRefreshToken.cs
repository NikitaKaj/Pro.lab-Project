using System.ComponentModel.DataAnnotations;

namespace ProLab.Data.Entities.Users;

public class UserRefreshToken
{
	[Key]
	//[JsonIgnore]
	public int Id { get; set; }

	public long UserId { get; set; }
	public User? User { get; set; }

	[StringLength(1000)]
	public string Token { get; set; }
	public DateTimeOffset Expires { get; set; }
	public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
	public DateTimeOffset Created { get; set; }
	[StringLength(390)]
	public string CreatedByIp { get; set; }
	public DateTimeOffset? Revoked { get; set; }
	[StringLength(390)]
	public string? RevokedByIp { get; set; }
	[StringLength(1000)]
	public string? ReplacedByToken { get; set; }
	public bool IsActive => Revoked == null && !IsExpired;
}
