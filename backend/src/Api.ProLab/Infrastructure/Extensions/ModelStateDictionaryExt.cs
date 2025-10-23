using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProLab.Api.Infrastructure.Extensions;

public static class ModelStateDictionaryExt
{
  public static ModelStateDictionary AddModelErrorF(this ModelStateDictionary ms, string key, string errorMessage)
  {
    ms.AddModelError(key, errorMessage);
    return ms;
  }
}
