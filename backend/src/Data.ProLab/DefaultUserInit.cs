using ProLab.Data.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProLab.Data
{
	internal class DefaultUserInit : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			var hasher = new PasswordHasher<User>();
			var user = new User("admin");
			user.Id = 1;
			user.EmailConfirmed = true;
			user.PhoneNumberConfirmed = true;
			user.SecurityStamp = Guid.NewGuid().ToString();
			user.FirstName = "Admin";
			user.LastName = "Adminovs";
			user.PasswordHash = hasher.HashPassword(user, "admin");
			builder.HasData(user);
		}
	}

	internal class DefaultUserInit2 : IEntityTypeConfiguration<IdentityUserClaim<long>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserClaim<long>> builder)
		{
			builder.HasData(new IdentityUserClaim<long>()
			{
				Id=1,
				ClaimType="Role",
				ClaimValue="admin",
				UserId = 1,
			});
		}
	}
}
