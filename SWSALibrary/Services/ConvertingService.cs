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
              result = timeSpan.ToString(@"hh\:mm\:ss");
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
              result = dateTimeOffset.ToString(@"HH:mm:sszzz");
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
            result = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            break;
          }
        case PgsqlDbType.TimestampTZ:
          {
            result = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:sszzz");
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
