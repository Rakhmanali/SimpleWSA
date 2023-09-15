using SimpleWSA.WSALibrary.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleWSA.WSALibrary.Services
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
          if (value is TimeSpan timeSpan)
          {
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
          /*
             although npgsql 2.2.7 can accept .NET TimeSpan object as the source for a parameter 
             having the type as NpgsqlDbType.Time, npgsql 2.2.7 converts and keeps it as the DataTime type.
          */
          else if (value is DateTime dateTime)
          {
            if (dateTime.Millisecond > 0)
            {
              result = dateTime.ToString("HH:mm:ss.ffff");
            }
            else
            {
              result = dateTime.ToString("HH:mm:ss");
            }
          }
          else
          {
            throw new Exception("the TimeSpan is required");
          }
          break;
        case PgsqlDbType.TimeTZ:
          /*
             TO DO: The strange behavior of the npgsql6 discovered during work with PostgreSQL function:
             
                    migration.get_in_and_out_timetz(p_parameter1 time with time zone,	OUT p_parameter2 time with time zone)
          
                    . The SimpleWSA sends the time part as is and replaces the date part by "0001.01.01" of the source DateTimeOffset, and 
                    npgsql returns the same time part, but date there is corrupted: "0001.01.02". Why? I don't know yet.

          */
          if (value is DateTimeOffset dateTimeOffset)
          {
            if (dateTimeOffset.Millisecond > 0)
            {
              result = dateTimeOffset.ToString(@"0001-01-01 HH:mm:ss.ffffzzzz");
            }
            else
            {
              result = dateTimeOffset.ToString(@"0001-01-01 HH:mm:sszzzz");
            }
          }
          /*
             although npgsql 2.2.7 can accept .NET TimeSpan object as the source for a parameter 
             having the type as NpgsqlDbType.TimeTZ, npgsql 2.2.7 converts and keeps it as the DataTime type.
          */
          else if (value is DateTime dateTime)
          {
            if (dateTime.Millisecond > 0)
            {
              result = dateTime.ToString(@"0001-01-01 HH:mm:ss.ffff+00:00");
            }
            else
            {
              result = dateTime.ToString(@"0001-01-01 HH:mm:ss+00:00");
            }
          }
          else
          {
            throw new Exception("the DateTimeOffset is required");
          }
          break;
        case PgsqlDbType.Timestamp:
          {
            DateTime dateTime = Convert.ToDateTime(value);
            if (dateTime.Millisecond > 0)
            {
              result = dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            }
            else
            {
              result = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
          }
          break;
        case PgsqlDbType.TimestampTZ:
          {
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
