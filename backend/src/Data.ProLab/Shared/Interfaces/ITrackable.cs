namespace ProLab.Data.Shared.Interfaces
{
	public interface ITrackable
	{
		DateTimeOffset CreatedAt { get; set; }
		DateTimeOffset? UpdatedAt { get; set; }
	}
}
