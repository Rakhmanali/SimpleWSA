using System;
using System.Collections;
using System.Linq;
using System.Xml;

namespace SimpleWSA.WSALibrary.Extensions
{
  internal static class CommonExtension
  {
    internal static T[] ToArray<T>(this Object value)
    {
      if (value is IEnumerable enumerableValue)
      {
        return enumerableValue.Cast<T>()
                             .Select(x => x)
                             .ToArray();
      }
      else
      {
        return new T[] { value.To<T>() };
      }
    }

    internal static T To<T>(this Object value)
    {
      if (value.IsNotNull())
      {
        return (T)value;
      }

      return default(T);
    }

    internal static bool IsNotNull(this object value)
    {
      return (value != null && value != DBNull.Value);
    }

    internal static bool IsNullOrDBNull(this object value)
    {
      return (value == null || value == DBNull.Value);
    }

    public static string SparseString(this string input)
    {
      char[] validXmlChars = input.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();

      return new string(validXmlChars);
    }
  }
}
