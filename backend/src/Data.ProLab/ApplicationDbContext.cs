using Ardalis.EFCore.Extensions;
using ProLab.Data.Entities;
using ProLab.Data.Entities.Users;
using ProLab.Data.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProLab.Data.Entities.Orders;
using ProLab.Data.Entities.Clients;
using ProLab.Data.Entities.Couriers;
using ProLab.Data.Entities.Routes;

namespace ProLab.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{

	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
	}

	public DbSet<LogEntry> LogEntries => Set<LogEntry>();
		public DbSet<Order> Orders { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Route> Routes { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyAllConfigurationsFromCurrentAssembly();

		SetCreatedAtDefaults(builder);
		MapUserTableNames(builder);

		builder.Entity<Order>(e =>
		{
			e.HasKey(x => x.Id);

			e.HasOne<Client>()
				.WithMany()
				.HasForeignKey(o => o.ClientId)
				.OnDelete(DeleteBehavior.Restrict);

			e.HasOne<Courier>()
				.WithMany()
				.HasForeignKey(o => o.CourierId)
				.OnDelete(DeleteBehavior.Restrict);

			e.HasOne<Route>()
				.WithMany()
				.HasForeignKey(o => o.RouteId)
				.OnDelete(DeleteBehavior.SetNull);
		});

		//SetUtcDateTimeConverter(builder);
	}

	private static void MapUserTableNames(ModelBuilder builder)
	{
		builder.Entity<User>().ToTable("Users");
		builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims");
		builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins");
		builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims");
		builder.Entity<IdentityUserToken<long>>().ToTable("UserTokens");
	}

	public override int SaveChanges()
	{
		AddTimestamps();
		var result = base.SaveChanges();
		//this.PushEventsOnChange().RunSynchronously();
		return result;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		AddTimestamps();
		var result = await base.SaveChangesAsync(cancellationToken);
		return result;
	}

	private void AddTimestamps()
	{
		var entries = ChangeTracker.Entries();
		foreach (var entry in entries)
		{
			if (entry.Entity is ITrackable trackable)
			{
				var now = DateTimeOffset.UtcNow;
				switch (entry.State)
				{
					case EntityState.Modified:
						trackable.UpdatedAt = now;
						break;

					case EntityState.Added:
						trackable.CreatedAt = now;
						trackable.UpdatedAt = now;
						break;
				}
			}
		}
	}

	private static void SetCreatedAtDefaults(ModelBuilder builder)
	{
		foreach (var entityType in builder.Model.GetEntityTypes())
		{
			if (typeof(ITrackable).IsAssignableFrom(entityType.ClrType))
			{
				builder.Entity(entityType.ClrType).Property<DateTimeOffset>(nameof(ITrackable.CreatedAt)).HasDefaultValueSql("getutcdate()");
			}
			if (typeof(ITrackable).IsAssignableFrom(entityType.ClrType))
			{
				builder.Entity(entityType.ClrType).Property<DateTimeOffset>(nameof(ITrackable.CreatedAt)).HasDefaultValueSql("getutcdate()");
			}
		}
	}

	//private static void SetUtcDateTimeConverter(ModelBuilder builder)
	//{
	//  var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
	//  foreach (var entityType in builder.Model.GetEntityTypes())
	//  {
	//    foreach (var property in entityType.GetProperties())
	//    {
	//      if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
	//        property.SetValueConverter(dateTimeConverter);
	//    }
	//  }
	//}

	public async Task<int> SaveChangesWithIdentityInsertAsync<TEnt>(CancellationToken token = default)
	{
		await using var transaction = await Database.BeginTransactionAsync(token);
		await SetIdentityInsertAsync<TEnt>(true, token);
		int ret = await SaveChangesAsync(token);
		await SetIdentityInsertAsync<TEnt>(false, token);
		await transaction.CommitAsync(token);

		return ret;
	}

	private async Task SetIdentityInsertAsync<TEnt>(bool enable, CancellationToken token)
	{
		var entityType = Model.FindEntityType(typeof(TEnt));
		var value = enable ? "ON" : "OFF";
		string query = $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}";
		await Database.ExecuteSqlRawAsync(query, token);
	}
}
