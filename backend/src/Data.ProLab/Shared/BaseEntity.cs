using ProLab.Data.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProLab.Data.Shared
{
	// This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
	public abstract class BaseEntity<TKey> : ITrackable
	{
		public TKey Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset? UpdatedAt { get; set; }
	}
}