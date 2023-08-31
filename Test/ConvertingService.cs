using SimpleWSA;
using SimpleWSA.Services;
using System;
using Xunit;

/* Please, note:

   xUnit, the test class is instantiated – it runs the single test method and 
   discards the class. It recreates the class again to run the next test method.

   // https://www.clariontech.com/blog/why-should-you-use-xunit-a-unit-testing-framework-for-.net#:~:text=xUnit%20also%20allows%20you%20to,the%20new%20attribute%20called%20Skip.
*/

namespace Test
{
  public class ConvertingServiceTests
  {
    ConvertingService convertingService = new ConvertingService();

    #region NpgsqlDbType.Time
    [Fact]
    public void NetTimeSpan_00_00_00_To_DbTimeString()
    {
      TimeSpan timeSpan = new TimeSpan(0, 0, 0);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, timeSpan, EncodingType.NONE);

      TimeSpan timeSpan1 = TimeSpan.Parse(objects[0] as string);

      Assert.Equal("00:00:00", objects[0]);
    }

    [Fact]
    public void NetTimeSpan_00_00_00_456_To_DbTimeString()
    {
      TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 456);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, timeSpan, EncodingType.NONE);

      TimeSpan timeSpan1 = TimeSpan.Parse(objects[0] as string);

      Assert.Equal("00:00:00.4560", objects[0]);
    }

    [Fact]
    public void NetTimeSpan_02_03_04_To_DbTimeString()
    {
      TimeSpan timeSpan = new TimeSpan(2, 3, 4);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, timeSpan, EncodingType.NONE);

      TimeSpan timeSpan1 = TimeSpan.Parse(objects[0] as string);

      Assert.Equal("02:03:04", objects[0]);
    }


    [Fact]
    public void NetTimeSpan_23_59_59_To_DbTimeString()
    {
      TimeSpan timeSpan = new TimeSpan(23, 59, 59);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, timeSpan, EncodingType.NONE);

      TimeSpan timeSpan1 = TimeSpan.Parse(objects[0] as string);

      Assert.Equal("23:59:59", objects[0]);
    }

    [Fact]
    public void NetTimeSpan_23_59_59_456_To_DbTimeString()
    {
      TimeSpan timeSpan = new TimeSpan(0, 23, 59, 59, 456);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, timeSpan, EncodingType.NONE);

      TimeSpan timeSpan1 = TimeSpan.Parse(objects[0] as string);

      Assert.Equal("23:59:59.4560", objects[0]);
    }
    #endregion NpgsqlDbType.Time

    #region NpgsqlDbType.TimeTZ
    /*
      1. Choose between DateTime, DateTimeOffset, TimeSpan, and TimeZoneInfo
         https://docs.microsoft.com/en-us/dotnet/standard/datetime/choosing-between-datetime

      The DateTimeOffset structure represents a date and time value, together with an offset
      that indicates how much that value differs from UTC. Thus, the value always unambiguously
      identifies a single point in time.

    */

    [Fact]
    public void NetDateTimeOffset_WithTimeAndOffset_To_DbTimeTzString()
    {
      DateTimeOffset dateTimeOffset = new DateTimeOffset(default(DateTime).Add(new TimeSpan(23, 59, 59)), new TimeSpan(2, 32, 00));
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimeTZ, dateTimeOffset, EncodingType.NONE);

      DateTimeOffset dateTimeOffset1 = DateTimeOffset.Parse(objects[0] as string);

      Assert.Equal("23:59:59+02:32", objects[0]);
    }

    [Fact]
    public void NetDateTimeOffset_WithTimeAndMsAndOffset_To_DbTimeTzString()
    {
      DateTimeOffset dateTimeOffset = new DateTimeOffset(default(DateTime).Add(new TimeSpan(0, 23, 59, 59, 678)), new TimeSpan(2, 32, 00));
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimeTZ, dateTimeOffset, EncodingType.NONE);

      DateTimeOffset dateTimeOffset1 = DateTimeOffset.Parse(objects[0] as string);

      Assert.Equal("23:59:59.6780+02:32", objects[0]);
    }

    [Fact]
    public void NetDateTimeOffset_23_59_59_00_00_00_To_DbTimeTZString()
    {
      DateTimeOffset dateTimeOffset = new DateTimeOffset(default(DateTime).Add(new TimeSpan(23, 59, 59)), new TimeSpan(0, 0, 00));
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimeTZ, dateTimeOffset, EncodingType.NONE);

      DateTimeOffset dateTimeOffset1 = DateTimeOffset.Parse(objects[0] as string);

      Assert.Equal("23:59:59+00:00", objects[0]);
    }
    #endregion NpgsqlDbType.TimeTZ

    #region NpgsqlDbType.Timestamp
    // npgsql migration from 2 to 5
    // https://www.npgsql.org/doc/types/basic.html
    [Fact]
    public void NetDateTime_Unspecified_To_DbTimestampString()
    {
      DateTime dateTime = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Unspecified);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Timestamp, dateTime, EncodingType.NONE);

      DateTime dateTime1 = DateTime.Parse(objects[0] as string);

      Assert.Equal("2021-08-26 23:51:35.4560", objects[0]);
    }

    /*
     * The following tests show that WSA sends the exact date and time to the web service
     * by ignoring DateTimeKind.Local, or Utc i.e.
     * whether DateTimeKind specified or not WSA will generate the same result for
     * NpgsqlDbType.Timestamp
     */

    [Fact]
    public void NetDateTime_Local_To_DbTimestampString()
    {
      DateTime dateTime = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Local);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Timestamp, dateTime, EncodingType.NONE);

      DateTime dateTime1 = DateTime.Parse(objects[0] as string);

      Assert.Equal("2021-08-26 23:51:35.4560", objects[0]);
    }

    [Fact]
    public void NetDateTime_Utc_To_DbTimestampString()
    {
      DateTime dateTime = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Utc);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Timestamp, dateTime, EncodingType.NONE);

      DateTime dateTime1 = DateTime.Parse(objects[0] as string);

      Assert.Equal("2021-08-26 23:51:35.4560", objects[0]);
    }
    #endregion NpgsqlDbType.Timestamp

    #region NpgsqlDbType.TimestampTz
    [Fact]
    public void NetDateTime_Utc_To_DbTimestampTzString()
    {
      DateTime dateTime = new DateTime(2021, 8, 26, 18, 51, 35, 456, DateTimeKind.Utc);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimestampTZ, dateTime, EncodingType.NONE);

      DateTime dateTime1 = DateTime.Parse(objects[0] as string);

      Assert.Equal("2021-08-26 23:51:35.4560+05:00", objects[0]);
    }

    [Fact]
    public void NetDateTime_Local_To_DbTimestampTzString()
    {
      DateTime dateTime = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Local);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimestampTZ, dateTime, EncodingType.NONE);

      DateTime dateTime1 = DateTime.Parse(objects[0] as string);

      Assert.Equal("2021-08-26 23:51:35.4560+05:00", objects[0]);
    }

    [Fact]
    public void NetDateTime_Unspecified_To_DbTimestampTzString()
    {
      DateTime dateTime = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Unspecified);
      Assert.Throws<Exception>(() => convertingService.ConvertObjectToDb(PgsqlDbType.TimestampTZ, dateTime, EncodingType.NONE));
    }
    #endregion NpgsqlDbType.TimestampTz
  }
}
