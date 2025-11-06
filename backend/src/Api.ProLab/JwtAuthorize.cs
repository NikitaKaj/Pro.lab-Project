using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ProLab.Api
{
	public class JwtAuthorizeAttribute : AuthorizeAttribute
	{
		public JwtAuthorizeAttribute() : base() => 
			this.AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
	}
}
