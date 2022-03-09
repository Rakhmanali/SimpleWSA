using System;
using System.Globalization;
using System.Linq;
using System.Text;
using SimpleWSA.Extensions;

namespace SimpleWSA.Services
{
  public class ConvertingService : IConvertingService
  {
    private object ConvertScalarObjectToDb(PgsqlDbType pgsqlDbType, object value, EncodingType outgoingEncodingType)
    {
      object result = null;

      if (value.IsNullOrDBNull() == true)
      {
        return result;
      }

      switch (pgsqlDbType)
      {
        case PgsqlDbType.Time:
          {
            if (value is TimeSpan timeSpan)
            {
              //result = timeSpan.ToString(@"hh\:mm\:ss");

              /*
               * PostgreSql time:
               * description: time of day (no date)
               * low value: 00:00:00
               * high value: 24:00:00
               * example 16:50:12.768915
               * https://www.postgresql.org/docs/9.1/datatype-datetime.html
               *
               * .NET TimeSpan supports DAY too, but we can ignore it, because PostgreSql time
               * is not support it
               */
              if (timeSpan.Milliseconds > 0)
              {
                result = timeSpan.ToString(@"hh\:mm\:ss\.ffff");
              }
              else
              {
                result = timeSpan.ToString(@"hh\:mm\:ss");
              }
            }
            else
            {
              result = Convert.ToDateTime(value).ToString("HH:mm:ss");
            }
            break;
          }
        case PgsqlDbType.TimeTZ:
          {
            if (value is DateTimeOffset dateTimeOffset)
            {
              //result = dateTimeOffset.ToString(@"HH:mm:sszzz");
              if (dateTimeOffset.Millisecond > 0)
              {
                result = dateTimeOffset.ToString(@"yyyy-MM-dd HH:mm:ss.ffffzzzz");
              }
              else
              {
                result = dateTimeOffset.ToString(@"yyyy-MM-dd HH:mm:sszzzz");
              }
            }
            else
            {
              DateTime dateTime = (DateTime)value;
              if (dateTime.Kind == DateTimeKind.Utc)
              {
                result = dateTime.ToString("HH:mm:ss+00:00");
              }
              else
              {
                result = dateTime.ToString("HH:mm:sszzz");
              }
            }
            break;
          }
        case PgsqlDbType.Timestamp:
          {
            //result = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            DateTime dateTime = Convert.ToDateTime(value);
            if (dateTime.Millisecond > 0)
            {
              result = dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            }
            else
            {
              result = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            break;
          }
        case PgsqlDbType.TimestampTZ:
          {
            //result = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:sszzz");
            DateTime dateTime = Convert.ToDateTime(value);
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
              throw new Exception("NpgsqlDbType.TimestampTz type waits .NET DateTime object where DateTimeKind is Local");
            }

            if (dateTime.Kind == DateTimeKind.Utc)
            {
              dateTime = dateTime.ToLocalTime();
            }

            if (dateTime.Millisecond > 0)
            {
              result = dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffzzz");
            }
            else
            {
              result = dateTime.ToString("yyyy-MM-dd HH:mm:sszzz");
            }

            break;
          }
        case PgsqlDbType.Date:
          {
            result = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
            break;
          }
        case PgsqlDbType.Bigint:
          {
            result = Convert.ToInt64(value);
            break;
          }
        case PgsqlDbType.Smallint:
          {
            result = Convert.ToInt16(value);
            break;
          }
        case PgsqlDbType.Integer:
          {
            result = Convert.ToInt32(value);
            break;
          }
        case PgsqlDbType.Real:
          {
            result = Convert.ToSingle(value);
            break;
          }
        case PgsqlDbType.Double:
          {
            result = Convert.ToDouble(value);
            break;
          }
        case PgsqlDbType.Money:
        case PgsqlDbType.Numeric:
          {
            result = Convert.ToDecimal(value);
            break;
          }
        case PgsqlDbType.Varchar:
        case PgsqlDbType.Text:
        case PgsqlDbType.Xml:
        case PgsqlDbType.Json:
        case PgsqlDbType.Jsonb:
          {
            string t = Convert.ToString(value, CultureInfo.InvariantCulture).SparseString();
            if (!string.IsNullOrEmpty(t) && outgoingEncodingType == EncodingType.NONE)
            {
              t = $"<![CDATA[{t}]]>";
            }
            result = t;

            break;
          }
        case PgsqlDbType.Bytea:
          {
            result = Convert.ToBase64String((byte[])value);
            break;
          }
        default:
          {
            result = value;
            break;
          }
      }

      return result;
    }

    public object[] ConvertObjectToDb(PgsqlDbType pgsqlDbType, object value, EncodingType outgoingEncodingType)
    {
      object[] result = null;

      if (value.IsNullOrDBNull() == true)
      {
        return result;
      }

      if ((pgsqlDbType & PgsqlDbType.Array) == 0)
      {
        result = new object[1];
        result[0] = this.ConvertScalarObjectToDb(pgsqlDbType, value, outgoingEncodingType);
        return result;
      }

      result = value.ToArray<object>();
      if (result.Length > 0)
      {
        PgsqlDbType itemPgsqlDbType = pgsqlDbType ^ PgsqlDbType.Array;
        for (int i = 0; i < result.Length; i++)
        {
          if (result[i] != null)
          {
            result[i] = this.ConvertScalarObjectToDb(itemPgsqlDbType, result[i], outgoingEncodingType);
          }
        }
      }

      return result;
    }

    public string EncodeTo(object value, EncodingType outgoingEncodingType)
    {
      if (value == null)
      {
        value = "null";
      }

      string result = Convert.ToString(value, CultureInfo.InvariantCulture);

      switch (outgoingEncodingType)
      {
        case EncodingType.BASE64:
          {
            result = Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
            break;
          }
        case EncodingType.NONE:
        default:
          {
            break;
          }
      }

      return result;
    }
  }
}
