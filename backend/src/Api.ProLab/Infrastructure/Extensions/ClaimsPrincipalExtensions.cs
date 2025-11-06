using System.Security.Claims;

namespace ProLab.Api.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static long GetUserId(this ClaimsPrincipal principal)
  {
    if (principal == null)
      throw new ArgumentNullException(nameof(principal));

    var selfWalletAdress = long.Parse(principal.FindAll(ClaimTypes.NameIdentifier).First().Value);
    return selfWalletAdress;
  }
}
