using System.Linq;

namespace Nova;

public static class ObjectExtension
{
  public static bool NotEqualToAnyOf(this object obj, params object[] values)
  {
    return values.Any(value => obj != value);
  }
  
  public static bool NotEqualToAllOf(this object obj, params object[] values)
  {
    return values.All(value => obj != value);
  }
}
