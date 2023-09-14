using SimpleWSA.WSALibrary;
using SimpleWSA.WSALibrary.Services;
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
      var expected = new TimeSpan(0, 0, 0);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, expected, EncodingType.NONE);
      var actual = TimeSpan.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void NetTimeSpan_00_00_00_456_To_DbTimeString()
    {
      var expected = new TimeSpan(0, 0, 0, 0, 456);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, expected, EncodingType.NONE);
      var actual = TimeSpan.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void NetTimeSpan_02_03_04_To_DbTimeString()
    {
      var expected = new TimeSpan(2, 3, 4);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, expected, EncodingType.NONE);
      var actual = TimeSpan.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
    }


    [Fact]
    public void NetTimeSpan_23_59_59_To_DbTimeString()
    {
      var expected = new TimeSpan(23, 59, 59);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, expected, EncodingType.NONE);
      var actual = TimeSpan.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void NetTimeSpan_23_59_59_456_To_DbTimeString()
    {
      var expected = new TimeSpan(0, 23, 59, 59, 456);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Time, expected, EncodingType.NONE);
      var actual = TimeSpan.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
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

    /*
      The timetz data type is one of the data types in PostgreSQL used to store time with time zone information. 
      Its data format is very similar to the time data type, but it also includes information about the time zone. 
      The timetz data type can be used to store a specific moment in time during a day or a time range.
    */

    [Fact]
    public void NetDateTimeOffset_WithTimeAndOffset_To_DbTimeTzString()
    {
      var expected = new DateTimeOffset(default(DateTime).Add(new TimeSpan(23, 59, 59)), new TimeSpan(2, 32, 00));
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimeTZ, expected, EncodingType.NONE);
      var actual = DateTimeOffset.Parse(objects[0] as string);
      Assert.Equal(expected.TimeOfDay, actual.TimeOfDay);
    }

    [Fact]
    public void NetDateTimeOffset_WithTimeAndMsAndOffset_To_DbTimeTzString()
    {
      var expected = new DateTimeOffset(default(DateTime).Add(new TimeSpan(0, 23, 59, 59, 678)), new TimeSpan(2, 32, 00));
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimeTZ, expected, EncodingType.NONE);
      var actual = DateTimeOffset.Parse(objects[0] as string);
      Assert.Equal(expected.TimeOfDay, actual.TimeOfDay);
    }

    [Fact]
    public void NetDateTimeOffset_23_59_59_00_00_00_To_DbTimeTZString()
    {
      var expected = new DateTimeOffset(default(DateTime).Add(new TimeSpan(23, 59, 59)), new TimeSpan(0, 0, 00));
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimeTZ, expected, EncodingType.NONE);
      var actual = DateTimeOffset.Parse(objects[0] as string);
      Assert.Equal(expected.TimeOfDay, actual.TimeOfDay);
    }
    #endregion NpgsqlDbType.TimeTZ

    #region NpgsqlDbType.Timestamp
    // https://www.npgsql.org/doc/types/basic.html
    [Fact]
    public void NetDateTime_Unspecified_To_DbTimestampString()
    {
      var expected = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Unspecified);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Timestamp, expected, EncodingType.NONE);
      var actual = DateTime.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
    }

    /*
       The following tests show that WSA sends the exact date and time to the web service
       by ignoring DateTimeKind.Local, or Utc i.e.
       whether DateTimeKind specified or not WSA will generate the same result for
       NpgsqlDbType.Timestamp

       https://www.npgsql.org/doc/types/basic.html
     */

    [Fact]
    public void NetDateTime_Local_To_DbTimestampString()
    {
      var expected = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Local);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Timestamp, expected, EncodingType.NONE);
      var actual = DateTime.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
      // The Kind parameters value - DateTimeKind.Local of a source DateTime will be ignored and specified
      // as DateTimeKind.Unspecified
      Assert.Equal(DateTimeKind.Unspecified, actual.Kind);
    }

    [Fact]
    public void NetDateTime_Utc_To_DbTimestampString()
    {
      var expected = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Utc);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.Timestamp, expected, EncodingType.NONE);
      var actual = DateTime.Parse(objects[0] as string);
      Assert.Equal(expected, actual);
      // The Kind parameters value - DateTimeKind.Utc of a source DateTime will be ignored and specified
      // as DateTimeKind.Unspecified
      Assert.Equal(DateTimeKind.Unspecified, actual.Kind);
    }
    #endregion NpgsqlDbType.Timestamp

    #region NpgsqlDbType.TimestampTz
    [Fact]
    public void NetDateTime_Utc_To_DbTimestampTzString()
    {
      var expected = new DateTime(2021, 8, 26, 18, 51, 35, 456, DateTimeKind.Utc);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimestampTZ, expected, EncodingType.NONE);
      var actual = DateTime.Parse(objects[0] as string);
      Assert.Equal(expected.ToLocalTime(), actual.ToLocalTime());
    }

    [Fact]
    public void NetDateTime_Local_To_DbTimestampTzString()
    {
      var expected = new DateTime(2021, 8, 26, 23, 51, 35, 456, DateTimeKind.Local);
      object[] objects = convertingService.ConvertObjectToDb(PgsqlDbType.TimestampTZ, expected, EncodingType.NONE);
      var actual = DateTime.Parse(objects[0] as string);
      Assert.Equal(expected.ToLocalTime(), actual.ToLocalTime());
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
