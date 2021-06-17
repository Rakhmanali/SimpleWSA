using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SimpleWSA.Extensions
{
  internal static class CommonExtension
  {
    internal static string ExceptionMessage(this Exception ex)
    {
      return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
    }

    internal static Int16? NToNullInt16(this Object value)
    {
      if (value.IsNotNull())
      {
        if (Int16.TryParse(value.NToString(), out short result))
        {
          return result;
        }
      }

      return null;
    }

    internal static int NToInt32(this object value)
    {
      if (value is int _value)
      {
        return _value;
      }
      else if (value is null || ReferenceEquals(value, DBNull.Value))
      {
        return default(int);
      }

      if (int.TryParse(value.ToString(), out int result))
      {
        return result;
      }

      return default(int);
    }

    internal static Decimal? NToNullDecimal(this Object value)
    {
      var t = value.NToString().Trim();
      t = t.Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator, "");
      if (Decimal.TryParse(t, out decimal result))
      {
        return result;
      }

      return null;
    }

    internal static Double? NToNullDouble(this Object value)
    {
      var t = value.NToString().Trim();
      t = t.Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator, "");
      if (Double.TryParse(t, out double result))
      {
        return result;
      }

      return null;
    }

    internal static Single? NToNullSingle(this Object value)
    {
      var t = value.NToString().Trim();
      t = t.Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator, "");
      if (Single.TryParse(t, out float result))
      {
        return result;
      }

      return null;
    }

    internal static Int32? NToNullInt32(this Object value)
    {
      if (value.IsNotNull())
      {
        if (Int32.TryParse(value.NToString(), out int result))
        {
          return result;
        }
      }

      return null;
    }

    internal static Int64? NToNullInt64(this Object value)
    {
      if (value.IsNotNull())
      {
        if (Int64.TryParse(value.NToString(), out long result))
        {
          return result;
        }
      }

      return null;
    }

    internal static DateTime? NToNullDate(this Object value)
    {
      return value.NToNullDateTime()?.Date;
    }

    internal static DateTime? NToNullDateTime(this Object value)
    {
      if (DateTime.TryParse(value.NToString(), out DateTime result))
      {
        return result;
      }

      return null;
    }

    internal static DateTime? NToNullDateTimeUTC(this Object value)
    {
      if (DateTime.TryParse(value.NToString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime result))
      {
        return result;
      }

      return null;
    }

    internal static String NToString(this Object value, string cultureName = "")
    {
      CultureInfo ci = cultureName == "" ? CultureInfo.CurrentCulture : new CultureInfo(cultureName);

      if (value.IsNotNull())
      {
        return Convert.ToString(value, ci);
      }
      else
      {
        return String.Empty;
      }
    }

    internal static Boolean? NToNullBoolean(this Object value)
    {
      if (value.IsNotNull())
      {
        if (Boolean.TryParse(value.NToString(), out bool boolResult))
        {
          return boolResult;
        }
        else if (Int32.TryParse(value.NToString(), out int intResult))
        {
          return intResult > 0;
        }
      }

      return null;
    }

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
  }
}
