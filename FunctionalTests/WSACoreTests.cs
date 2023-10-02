using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleWSA.WSALibrary.Exceptions;
using System.Data;

namespace SimpleWSA.WSALibrary
{
  public class WSACoreTests
  {
    private IConfiguration configuration;

    [SetUp]
    public async Task Setup()
    {
      var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
      configuration = builder.Build();

      Session session = new Session(configuration["Login"],
                                    configuration["Password"],
                                    34,
                                    "244",
                                    configuration["Domain"],
                                    null);
      await session.CreateByConnectionProviderAddressAsync("https://connectionprovider.naiton.com");
    }

    #region non query
    public static void TestNonQueryOutObject<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType, HttpMethod httpMethod = HttpMethod.GET)
    {
      var command = new Command(postgreSQLFunctionName);
      command.Parameters.Add("p_parameter", pgsqlDbType);
      var response = Command.Execute(command, RoutineType.NonQuery, httpMethod);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var actual = Convert.ChangeType(jobject[postgreSQLFunctionName]!["arguments"]!["p_parameter"]!, typeof(T));
      var expected = (T)value;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutBigint()
    {
      TestNonQueryOutObject<long>("migration.get_out_bigint", 9223372036854775807, PgsqlDbType.Bigint);
      TestNonQueryOutObject<long>("migration.get_out_bigint", 9223372036854775807, PgsqlDbType.Bigint, HttpMethod.POST);
    }

    [Test]
    public void GetOutBoolean()
    {
      TestNonQueryOutObject<bool>("migration.get_out_boolean", true, PgsqlDbType.Boolean);
      TestNonQueryOutObject<bool>("migration.get_out_boolean", true, PgsqlDbType.Boolean, HttpMethod.POST);
    }

    [Test]
    public void GetOutBytea()
    {
      var command = new Command("migration.get_out_bytea");
      command.Parameters.Add("p_parameter", PgsqlDbType.Bytea);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_bytea\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"aGVsbG8gd29ybGQh\"}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_bytea"]!["arguments"]!["p_parameter"]!;
      string? actual = Convert.ToString(p);
      if (actual != null)
      {
        var expected = "aGVsbG8gd29ybGQh";
        Assert.That(actual, Is.EqualTo(expected));
      }
      else
      {
        Assert.Fail();
      }
    }

    [Test]
    public void GetOutDoublePrecision()
    {
      TestNonQueryOutObject<double>("migration.get_out_double_precision", 1234567890.12345, PgsqlDbType.Double);
      TestNonQueryOutObject<double>("migration.get_out_double_precision", 1234567890.12345, PgsqlDbType.Double, HttpMethod.POST);
    }

    [Test]
    public void GetOutInt()
    {
      TestNonQueryOutObject<int>("migration.get_out_int", 2147483647, PgsqlDbType.Integer);
      TestNonQueryOutObject<int>("migration.get_out_int", 2147483647, PgsqlDbType.Integer, HttpMethod.POST);
    }

    [Test]
    public void GetOutMoney()
    {
      TestNonQueryOutObject<decimal>("migration.get_out_money", 92233720368547758.07m, PgsqlDbType.Money);
      TestNonQueryOutObject<decimal>("migration.get_out_money", 92233720368547758.07m, PgsqlDbType.Money, HttpMethod.POST);
    }

    [Test]
    public void GetOutNumeric()
    {
      TestNonQueryOutObject<decimal>("migration.get_out_numeric", 123456789012345678.1234567890m, PgsqlDbType.Numeric);
      TestNonQueryOutObject<decimal>("migration.get_out_numeric", 123456789012345678.1234567890m, PgsqlDbType.Numeric, HttpMethod.POST);
    }

    [Test]
    public void GetOutReal()
    {
      TestNonQueryOutObject<float>("migration.get_out_real", 1.234568E+09f, PgsqlDbType.Real);
      TestNonQueryOutObject<float>("migration.get_out_real", 1.234568E+09f, PgsqlDbType.Real, HttpMethod.POST);
    }

    [Test]
    public void GetOutSmallint()
    {
      TestNonQueryOutObject<short>("migration.get_out_smallint", (short)32767, PgsqlDbType.Smallint);
      TestNonQueryOutObject<short>("migration.get_out_smallint", (short)32767, PgsqlDbType.Smallint, HttpMethod.POST);
    }

    [Test]
    public void GetOutText()
    {
      TestNonQueryOutObject<string>("migration.get_out_text",
                                    "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                                    PgsqlDbType.Text);
      TestNonQueryOutObject<string>("migration.get_out_text",
                                    "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                                    PgsqlDbType.Text, HttpMethod.POST);
    }

    [Test]
    public void GetOutDate()
    {
      TestNonQueryOutObject<DateTime>("migration.get_out_date", DateTime.Parse("2021-05-18T00:00:00"), PgsqlDbType.Date);
      TestNonQueryOutObject<DateTime>("migration.get_out_date", DateTime.Parse("2021-05-18T00:00:00"), PgsqlDbType.Date, HttpMethod.POST);
    }

    [Test]
    public void GetOutTime()
    {
      TestNonQueryOutObject<TimeSpan>("migration.get_out_time", TimeSpan.Parse("13:44:46.9876000"), PgsqlDbType.Time);
      TestNonQueryOutObject<TimeSpan>("migration.get_out_time", TimeSpan.Parse("13:44:46.9876000"), PgsqlDbType.Time, HttpMethod.POST);
    }

    [Test]
    public void GetOutTimetz()
    {
      TestNonQueryOutObject<DateTimeOffset>("migration.get_out_timetz", DateTimeOffset.Parse("0001-01-02T16:41:45.1234+05:00"), PgsqlDbType.TimeTZ);
      TestNonQueryOutObject<DateTimeOffset>("migration.get_out_timetz", DateTimeOffset.Parse("0001-01-02T16:41:45.1234+05:00"), PgsqlDbType.TimeTZ, HttpMethod.POST);
    }

    [Test]
    public void GetOutTimestamp()
    {
      TestNonQueryOutObject<DateTime>("migration.get_out_timestamp", DateTime.Parse("2022-03-18T12:42:46.1234"), PgsqlDbType.Timestamp);
      TestNonQueryOutObject<DateTime>("migration.get_out_timestamp", DateTime.Parse("2022-03-18T12:42:46.1234"), PgsqlDbType.Timestamp, HttpMethod.POST);
    }

    [Test]
    public void GetOutTimestamptz()
    {
      TestNonQueryOutObject<DateTime>("migration.get_out_timestamptz", DateTime.Parse("2021-04-18T12:43:47.1234Z"), PgsqlDbType.TimestampTZ);
      TestNonQueryOutObject<DateTime>("migration.get_out_timestamptz", DateTime.Parse("2021-04-18T12:43:47.1234Z"), PgsqlDbType.TimestampTZ, HttpMethod.POST);
    }

    [Test]
    public void GetOutVarchar()
    {
      TestNonQueryOutObject<string>("migration.get_out_varchar", "PostgreSQL change column type examples", PgsqlDbType.Varchar);
      TestNonQueryOutObject<string>("migration.get_out_varchar", "PostgreSQL change column type examples", PgsqlDbType.Varchar, HttpMethod.POST);
    }

    [Test]
    public void GetOutUuid()
    {
      TestNonQueryOutObject<Guid>("migration.get_out_uuid", Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394"), PgsqlDbType.Uuid);
      TestNonQueryOutObject<Guid>("migration.get_out_uuid", Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394"), PgsqlDbType.Uuid, HttpMethod.POST);
    }

    [Test]
    public void GetOutXml()
    {
      var command = new Command("migration.get_out_xml");
      command.Parameters.Add("p_parameter", PgsqlDbType.Xml);

      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_xml"]!["arguments"]!["p_parameter"]!;

      var actual = Convert.ToString(p)?.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      var expected = @"<_routines>
        <_routine>
          <_name>formmanager_getfiltered</_name>
          <_arguments>
            <_formid>0</_formid>
            <_form></_form>
            <_businessids>1</_businessids>
            <_businessids>941</_businessids>
            <_businessids>942</_businessids>
            <_businessids>943</_businessids>
            <_businessids>944</_businessids>
            <_businessids>2006</_businessids>
            <_businessids>2129</_businessids>
            <_businessids>2135</_businessids>
            <_businessids>2137</_businessids>
            <_formtype>1</_formtype>
            <_formtype>2</_formtype>
            <_formtype>3</_formtype>
            <_formtype>4</_formtype>
            <_formtype>5</_formtype>
            <_formtype>6</_formtype>
            <_formtype>7</_formtype>
            <_formtype>8</_formtype>
            <_inactive>False</_inactive>
          </_arguments>
          <_options>
            <_writeSchema>1</_writeSchema>
          </_options>
        </_routine>
        <_compression>0</_compression>
        <_returnType>json</_returnType>
      </_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutJson()
    {
      var command = new Command("migration.get_out_json");
      command.Parameters.Add("p_parameter", PgsqlDbType.Json);

      // {\"migration.get_out_json\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"{\\n    \\\"formmanager_getfiltered\\\": []\\n}\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_json"]!["arguments"]!["p_parameter"]!;
      var actual = Convert.ToString(p)?.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      var expected = @"{ ""formmanager_getfiltered"": [] }".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty); ;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutJsonb()
    {
      var command = new Command("migration.get_out_jsonb");
      command.Parameters.Add("p_parameter", PgsqlDbType.Jsonb);

      // {\"migration.get_out_jsonb\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"{\\\"formmanager_getfiltered\\\": []}\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_jsonb"]!["arguments"]!["p_parameter"]!;
      var actual = Convert.ToString(p)?.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      var expected = @"{ ""formmanager_getfiltered"": [] }".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty); ;
      Assert.That(actual, Is.EqualTo(expected));
    }

    #region array
    public static void TestNonQueryOutArrayObject<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType, HttpMethod httpMethod = HttpMethod.GET)
    {
      var command = new Command(postgreSQLFunctionName);
      command.Parameters.Add("p_parameter", pgsqlDbType);
      var response = Command.Execute(command, RoutineType.NonQuery, httpMethod);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var ppv = jobject[command.Name]!["arguments"]!["p_parameter"];
      if (ppv is JArray ja)
      {
        var actual = ja.Select(x => Convert.ChangeType(x, typeof(T))).ToArray();
        var expected = value as T[];
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutBigintArray()
    {
      TestNonQueryOutArrayObject<long>("migration.get_out_bigint_array", new long[3] { 9223372036854775807, 9223372036854775806, 9223372036854775805 }, PgsqlDbType.Bigint | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<long>("migration.get_out_bigint_array", new long[3] { 9223372036854775807, 9223372036854775806, 9223372036854775805 }, PgsqlDbType.Bigint | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutBooleanArray()
    {
      TestNonQueryOutArrayObject<bool>("migration.get_out_boolean_array", new bool[3] { true, false, true }, PgsqlDbType.Boolean | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<bool>("migration.get_out_boolean_array", new bool[3] { true, false, true }, PgsqlDbType.Boolean | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutByteaArray()
    {
      var command = new Command("migration.get_out_bytea_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Bytea | PgsqlDbType.Array);

      // {\"migration.get_out_bytea_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"aGVsbG8gd29ybGQh\",\"VGhlcmUgYXJlIHRocmVlIG1ldGhvZHMgdXNlZCB0byBhZGp1c3QgYSBEYXRlT25seSBzdHJ1Y3R1cmU6IEFkZERheXMsIEFkZE1vbnRocywgYW5kIEFkZFllYXJzLiBFYWNoIG1ldGhvZCB0YWtlcyBhbiBpbnRlZ2VyIHBhcmFtZXRlciwgYW5kIGluY3JlYXNlcyB0aGUgZGF0ZSBieSB0aGF0IG1lYXN1cmVtZW50Lg==\",\"SWYgYSBuZWdhdGl2ZSBudW1iZXIgaXMgcHJvdmlkZWQsIHRoZSBkYXRlIGlzIGRlY3JlYXNlZCBieSB0aGF0IG1lYXN1cmVtZW50LiBUaGUgbWV0aG9kcyByZXR1cm4gYSBuZXcgaW5zdGFuY2Ugb2YgRGF0ZU9ubHksIGFzIHRoZSBzdHJ1Y3R1cmUgaXMgaW1tdXRhYmxlLg==\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_bytea_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<string?>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToString(item));
        }
        Assert.That(actual, Is.EqualTo(new string[3] { Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("hello world!")),
                                                       Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("There are three methods used to adjust a DateOnly structure: AddDays, AddMonths, and AddYears. Each method takes an integer parameter, and increases the date by that measurement.")),
                                                       Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("If a negative number is provided, the date is decreased by that measurement. The methods return a new instance of DateOnly, as the structure is immutable.")) }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutDoublePrecisionArray()
    {
      TestNonQueryOutArrayObject<double>("migration.get_out_double_precision_array", new double[3] { 1234567890.12345, 1234567889.6789, 1234567888.01478 },
        PgsqlDbType.Double | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<double>("migration.get_out_double_precision_array", new double[3] { 1234567890.12345, 1234567889.6789, 1234567888.01478 },
        PgsqlDbType.Double | PgsqlDbType.Array,
        HttpMethod.POST);
    }

    [Test]
    public void GetOutIntArray()
    {
      TestNonQueryOutArrayObject<int>("migration.get_out_int_array", new int[3] { 2147483647, 2147483646, 2147483645 }, PgsqlDbType.Integer | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<int>("migration.get_out_int_array", new int[3] { 2147483647, 2147483646, 2147483645 }, PgsqlDbType.Integer | PgsqlDbType.Array,
        HttpMethod.POST);
    }

    [Test]
    public void GetOutMoneyArray()
    {
      TestNonQueryOutArrayObject<decimal>("migration.get_out_money_array", new decimal[3] { 92233720368547758.07m, 92233720368547757.05m, 92233720368547756.06m },
        PgsqlDbType.Money | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<decimal>("migration.get_out_money_array", new decimal[3] { 92233720368547758.07m, 92233720368547757.05m, 92233720368547756.06m },
        PgsqlDbType.Money | PgsqlDbType.Array,
        HttpMethod.POST);
    }

    [Test]
    public void GetOutNumericArray()
    {
      TestNonQueryOutArrayObject<decimal>("migration.get_out_numeric_array", new decimal[3] { 123456789012345678.1234567890m, 123456789012345677.1234567889m, 123456789012345676.1234567888m },
        PgsqlDbType.Numeric | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<decimal>("migration.get_out_numeric_array", new decimal[3] { 123456789012345678.1234567890m, 123456789012345677.1234567889m, 123456789012345676.1234567888m },
        PgsqlDbType.Numeric | PgsqlDbType.Array,
        HttpMethod.POST);
    }

    [Test]
    public void GetOutRealArray()
    {
      TestNonQueryOutArrayObject<float>("migration.get_out_real_array", new float[3] { 1.234568E+09f, 1.234568E+09f, 1.234568E+09f },
        PgsqlDbType.Real | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<float>("migration.get_out_real_array", new float[3] { 1.234568E+09f, 1.234568E+09f, 1.234568E+09f },
        PgsqlDbType.Real | PgsqlDbType.Array,
        HttpMethod.POST);
    }

    [Test]
    public void GetOutSmallintArray()
    {
      TestNonQueryOutArrayObject<short>("migration.get_out_smallint_array", new short[3] { 32767, 32766, 32765 },
        PgsqlDbType.Smallint | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<short>("migration.get_out_smallint_array", new short[3] { 32767, 32766, 32765 },
        PgsqlDbType.Smallint | PgsqlDbType.Array,
        HttpMethod.POST);
    }

    [Test]
    public void GetOutTextArray()
    {
      TestNonQueryOutArrayObject<string>("migration.get_out_text_array",
        new string[3] { "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                        "DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.",
                        "DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date."
        }, PgsqlDbType.Text | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<string>("migration.get_out_text_array",
        new string[3] { "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                        "DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.",
                        "DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date."
        }, PgsqlDbType.Text | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutDateArray()
    {
      TestNonQueryOutArrayObject<DateTime>("migration.get_out_date_array", new DateTime[3] { DateTime.Parse("2021-05-18T00:00:00"),
                                                                                             DateTime.Parse("2020-04-17T00:00:00"),
                                                                                             DateTime.Parse("2019-03-16T00:00:00") },
                                            PgsqlDbType.Date | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<DateTime>("migration.get_out_date_array", new DateTime[3] { DateTime.Parse("2021-05-18T00:00:00"),
                                                                                             DateTime.Parse("2020-04-17T00:00:00"),
                                                                                             DateTime.Parse("2019-03-16T00:00:00") },
                                            PgsqlDbType.Date | PgsqlDbType.Array,
                                            HttpMethod.POST);
    }

    [Test]
    public void GetOutTimeArray()
    {
      TestNonQueryOutArrayObject<TimeSpan>("migration.get_out_time_array", new TimeSpan[3] { TimeSpan.Parse("13:44:46.9876000"),
                                                                                             TimeSpan.Parse("11:43:45.9875000"),
                                                                                             TimeSpan.Parse("11:42:44.9874000")
                                                                                           },
                                           PgsqlDbType.Time | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<TimeSpan>("migration.get_out_time_array", new TimeSpan[3] { TimeSpan.Parse("13:44:46.9876000"),
                                                                                             TimeSpan.Parse("11:43:45.9875000"),
                                                                                             TimeSpan.Parse("11:42:44.9874000")
                                                                                           },
                                           PgsqlDbType.Time | PgsqlDbType.Array,
                                           HttpMethod.POST);
    }

    [Test]
    public void GetOutTimetzArray()
    {
      TestNonQueryOutArrayObject<DateTimeOffset>("migration.get_out_timetz_array",
        new DateTimeOffset[3] { DateTimeOffset.Parse("0001-01-02T14:41:45.1234+03:00"),
                                DateTimeOffset.Parse("0001-01-02T13:39:44.1233+02:00"),
                                DateTimeOffset.Parse("0001-01-02T11:38:42.1232+01:00")
        }, PgsqlDbType.TimeTZ | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<DateTimeOffset>("migration.get_out_timetz_array",
        new DateTimeOffset[3] { DateTimeOffset.Parse("0001-01-02T14:41:45.1234+03:00"),
                                DateTimeOffset.Parse("0001-01-02T13:39:44.1233+02:00"),
                                DateTimeOffset.Parse("0001-01-02T11:38:42.1232+01:00")
        }, PgsqlDbType.TimeTZ | PgsqlDbType.Array, HttpMethod.POST);
    }


    [Test]
    public void GetOutTimestampArray()
    {
      TestNonQueryOutArrayObject<DateTime>("migration.get_out_timestamp_array",
        new DateTime[3] { DateTime.Parse("2022-03-18T12:42:46.1234"),
                          DateTime.Parse("2020-01-16T10:40:44.1232"),
                          DateTime.Parse("2019-09-15T09:39:43.1231") },
        PgsqlDbType.Timestamp | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<DateTime>("migration.get_out_timestamp_array",
        new DateTime[3] { DateTime.Parse("2022-03-18T12:42:46.1234"),
                          DateTime.Parse("2020-01-16T10:40:44.1232"),
                          DateTime.Parse("2019-09-15T09:39:43.1231") },
        PgsqlDbType.Timestamp | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutTimestamptzArray()
    {
      TestNonQueryOutArrayObject<DateTime>("migration.get_out_timestamptz_array",
        new DateTime[3] { DateTime.Parse("2021-04-18T14:43:47.1234+02:00"),
                          DateTime.Parse("2018-01-15T10:40:44.1231Z"),
                          DateTime.Parse("2017-01-14T07:39:44.123Z") },
        PgsqlDbType.TimestampTZ | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<DateTime>("migration.get_out_timestamptz_array",
        new DateTime[3] { DateTime.Parse("2021-04-18T14:43:47.1234+02:00"),
                          DateTime.Parse("2018-01-15T10:40:44.1231Z"),
                          DateTime.Parse("2017-01-14T07:39:44.123Z") },
        PgsqlDbType.TimestampTZ | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutVarcharArray()
    {
      TestNonQueryOutArrayObject<string>("migration.get_out_varchar_array",
        new string[3] { "PostgreSQL change column type examples",
                        "What is the PostgreSQL Function?",
                        "PostgreSQL change column type examples"
              }, PgsqlDbType.Varchar | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<string>("migration.get_out_varchar_array",
        new string[3] { "PostgreSQL change column type examples",
                        "What is the PostgreSQL Function?",
                        "PostgreSQL change column type examples"
              }, PgsqlDbType.Varchar | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutUuidArray()
    {
      TestNonQueryOutArrayObject<Guid>("migration.get_out_uuid_array",
        new Guid[3] { Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394"),
                      Guid.Parse("f0c180ba-e291-4089-91b4-3d8d122b5c77"),
                      Guid.Parse("670c4c79-521c-40e2-8442-0248a93f8737")
        }, PgsqlDbType.Uuid | PgsqlDbType.Array);
      TestNonQueryOutArrayObject<Guid>("migration.get_out_uuid_array",
        new Guid[3] { Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394"),
                      Guid.Parse("f0c180ba-e291-4089-91b4-3d8d122b5c77"),
                      Guid.Parse("670c4c79-521c-40e2-8442-0248a93f8737")
        }, PgsqlDbType.Uuid | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetOutXmlArray()
    {
      var command = new Command("migration.get_out_xml_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Xml | PgsqlDbType.Array);

      /*

      {\"migration.get_out_xml_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"<_routines>\\n  <_routine>\\n    <_name>formmanager_getfiltered</_name>\\n    <_arguments>\\n      <_formid>0</_formid>\\n      <_form></_form>\\n      <_businessids>1</_businessids>\\n      <_businessids>941</_businessids>\\n      <_businessids>942</_businessids>\\n      <_businessids>943</_businessids>\\n      <_businessids>944</_businessids>\\n      <_businessids>2006</_businessids>\\n      <_businessids>2129</_businessids>\\n      <_businessids>2135</_businessids>\\n      <_businessids>2137</_businessids>\\n      <_formtype>1</_formtype>\\n      <_formtype>2</_formtype>\\n      <_formtype>3</_formtype>\\n      <_formtype>4</_formtype>\\n      <_formtype>5</_formtype>\\n      <_formtype>6</_formtype>\\n      <_formtype>7</_formtype>\\n      <_formtype>8</_formtype>\\n      <_inactive>False</_inactive>\\n    </_arguments>\\n    <_options>\\n      <_writeSchema>1</_writeSchema>\\n    </_options>\\n  </_routine>\\n  <_compr
         ession>0</_compression>\\n  <_returnType>json</_returnType>\\n</_routines>\",\"<_routines>\\n          <_routine>\\n            <_name>InitializeSession</_name>\\n            <_arguments>\\n              <login>sadmin@upstairs.com</login>\\n              <password>George555#</password>\\n              <isEncrypt>0</isEncrypt>\\n              <timeout>20</timeout>\\n              <appId>38</appId>\\n              <appVersion>3.8.6</appVersion>\\n              <domain>naitonmaster</domain>\\n            </_arguments>\\n          </_routine>\\n          <_returnType>xml</_returnType>\\n        </_routines>\",\"<_routines>\\n  <_routine>\\n    <_name>companymanager_getfilteredcompanieslist</_name>\\n    <_arguments>\\n      <_companyid>0</_companyid>\\n      <_companyname></_companyname>\\n      <_countryid>0</_countryid>\\n      <_businessgroupid>0</_businessgroupid>\\n      <_businessid>1</_businessid>\\n      <_email></_email>\\n      <_zipcode></_zipcode>\\n      <_housenumber></_housenumber>\\n      <_statusi
         d>-3</_statusid>\\n      <_statusid>4</_statusid>\\n      <_statusid>6</_statusid>\\n      <_statusid>5</_statusid>\\n      <_iban></_iban>\\n      <_salesmanagerid>0</_salesmanagerid>\\n      <_onlyholding>False</_onlyholding>\\n      <_udffilter></_udffilter>\\n      <_holding></_holding>\\n      <_holdingalso>False</_holdingalso>\\n      <_companytypeid>2</_companytypeid>\\n      <_segmentid>0</_segmentid>\\n      <_segmentudf></_segmentudf>\\n      <_discountgroupid>-1</_discountgroupid>\\n      <_taxnumber></_taxnumber>\\n      <_chamberofcommerce></_chamberofcommerce>\\n      <_havechildonly>False</_havechildonly>\\n      <_reseller></_reseller>\\n      <_inactive>False</_inactive>\\n      <_companyids isNull=\\\"true\\\" />\\n      <_limit>200</_limit>\\n    </_arguments>\\n    <_options>\\n      <_writeSchema>1</_writeSchema>\\n    </_options>\\n  </_routine>\\n  <_compression>{{compression}}</_compression>\\n  <_returnType>json</_returnType>\\n</_routines>\"]}}}

      */

      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_xml_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<string>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToString(item)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty));
        }
        Assert.That(actual, Is.EqualTo(new string[3] { "<_routines>\n  <_routine>\n    <_name>formmanager_getfiltered</_name>\n    <_arguments>\n      <_formid>0</_formid>\n      <_form></_form>\n      <_businessids>1</_businessids>\n      <_businessids>941</_businessids>\n      <_businessids>942</_businessids>\n      <_businessids>943</_businessids>\n      <_businessids>944</_businessids>\n      <_businessids>2006</_businessids>\n      <_businessids>2129</_businessids>\n      <_businessids>2135</_businessids>\n      <_businessids>2137</_businessids>\n      <_formtype>1</_formtype>\n      <_formtype>2</_formtype>\n      <_formtype>3</_formtype>\n      <_formtype>4</_formtype>\n      <_formtype>5</_formtype>\n      <_formtype>6</_formtype>\n      <_formtype>7</_formtype>\n      <_formtype>8</_formtype>\n      <_inactive>False</_inactive>\n    </_arguments>\n    <_options>\n      <_writeSchema>1</_writeSchema>\n    </_options>\n  </_routine>\n  <_compression>0</_compression>\n  <_returnType>json</_returnType>\n</_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                                       "<_routines>\n          <_routine>\n            <_name>InitializeSession</_name>\n            <_arguments>\n              <login>sadmin@upstairs.com</login>\n              <password>George555#</password>\n              <isEncrypt>0</isEncrypt>\n              <timeout>20</timeout>\n              <appId>38</appId>\n              <appVersion>3.8.6</appVersion>\n              <domain>naitonmaster</domain>\n            </_arguments>\n          </_routine>\n          <_returnType>xml</_returnType>\n        </_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                                       "<_routines>\n  <_routine>\n    <_name>companymanager_getfilteredcompanieslist</_name>\n    <_arguments>\n      <_companyid>0</_companyid>\n      <_companyname></_companyname>\n      <_countryid>0</_countryid>\n      <_businessgroupid>0</_businessgroupid>\n      <_businessid>1</_businessid>\n      <_email></_email>\n      <_zipcode></_zipcode>\n      <_housenumber></_housenumber>\n      <_statusid>-3</_statusid>\n      <_statusid>4</_statusid>\n      <_statusid>6</_statusid>\n      <_statusid>5</_statusid>\n      <_iban></_iban>\n      <_salesmanagerid>0</_salesmanagerid>\n      <_onlyholding>False</_onlyholding>\n      <_udffilter></_udffilter>\n      <_holding></_holding>\n      <_holdingalso>False</_holdingalso>\n      <_companytypeid>2</_companytypeid>\n      <_segmentid>0</_segmentid>\n      <_segmentudf></_segmentudf>\n      <_discountgroupid>-1</_discountgroupid>\n      <_taxnumber></_taxnumber>\n      <_chamberofcommerce></_chamberofcommerce>\n      <_havechildonly>False</_havechildonly>\n      <_reseller></_reseller>\n      <_inactive>False</_inactive>\n      <_companyids isNull=\"true\" />\n      <_limit>200</_limit>\n    </_arguments>\n    <_options>\n      <_writeSchema>1</_writeSchema>\n    </_options>\n  </_routine>\n  <_compression>{{compression}}</_compression>\n  <_returnType>json</_returnType>\n</_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)
        }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutJsonArray()
    {
      var command = new Command("migration.get_out_json_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Json | PgsqlDbType.Array);

      /*

      {\"migration.get_out_json_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"{\\n    \\\"formmanager_getfiltered\\\": []\\n}\",\"{\\n    \\\"glossary\\\": {\\n        \\\"title\\\": \\\"example glossary\\\",\\n\\t\\t\\\"GlossDiv\\\": {\\n            \\\"title\\\": \\\"S\\\",\\n\\t\\t\\t\\\"GlossList\\\": {\\n                \\\"GlossEntry\\\": {\\n                    \\\"ID\\\": \\\"SGML\\\",\\n\\t\\t\\t\\t\\t\\\"SortAs\\\": \\\"SGML\\\",\\n\\t\\t\\t\\t\\t\\\"GlossTerm\\\": \\\"Standard Generalized Markup Language\\\",\\n\\t\\t\\t\\t\\t\\\"Acronym\\\": \\\"SGML\\\",\\n\\t\\t\\t\\t\\t\\\"Abbrev\\\": \\\"ISO 8879:1986\\\",\\n\\t\\t\\t\\t\\t\\\"GlossDef\\\": {\\n                        \\\"para\\\": \\\"A meta-markup language, used to create markup languages such as DocBook.\\\",\\n\\t\\t\\t\\t\\t\\t\\\"GlossSeeAlso\\\": [\\\"GML\\\", \\\"XML\\\"]\\n                    },\\n\\t\\t\\t\\t\\t\\\"GlossSee\\\": \\\"markup\\\"\\n                }\\n            }\\n        }\\n    }\\n}\",\"{\\\"menu\\\": {\
       \n  \\\"id\\\": \\\"file\\\",\\n  \\\"value\\\": \\\"File\\\",\\n  \\\"popup\\\": {\\n    \\\"menuitem\\\": [\\n      {\\\"value\\\": \\\"New\\\", \\\"onclick\\\": \\\"CreateNewDoc()\\\"},\\n      {\\\"value\\\": \\\"Open\\\", \\\"onclick\\\": \\\"OpenDoc()\\\"},\\n      {\\\"value\\\": \\\"Close\\\", \\\"onclick\\\": \\\"CloseDoc()\\\"}\\n    ]\\n  }\\n}}\"]}}}

      */

      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_json_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<string>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToString(item)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty));
        }
        Assert.That(actual, Is.EqualTo(new string[3] { "{\n    \"formmanager_getfiltered\": []\n}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                                       "{\n    \"glossary\": {\n        \"title\": \"example glossary\",\n\t\t\"GlossDiv\": {\n            \"title\": \"S\",\n\t\t\t\"GlossList\": {\n                \"GlossEntry\": {\n                    \"ID\": \"SGML\",\n\t\t\t\t\t\"SortAs\": \"SGML\",\n\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\n\t\t\t\t\t\"Acronym\": \"SGML\",\n\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\n\t\t\t\t\t\"GlossDef\": {\n                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\n\t\t\t\t\t\t\"GlossSeeAlso\": [\"GML\", \"XML\"]\n                    },\n\t\t\t\t\t\"GlossSee\": \"markup\"\n                }\n            }\n        }\n    }\n}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                                       "{\"menu\": {\n  \"id\": \"file\",\n  \"value\": \"File\",\n  \"popup\": {\n    \"menuitem\": [\n      {\"value\": \"New\", \"onclick\": \"CreateNewDoc()\"},\n      {\"value\": \"Open\", \"onclick\": \"OpenDoc()\"},\n      {\"value\": \"Close\", \"onclick\": \"CloseDoc()\"}\n    ]\n  }\n}}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)
              }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutJsonbArray()
    {
      var command = new Command("migration.get_out_jsonb_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Jsonb | PgsqlDbType.Array);

      /*

        {\"migration.get_out_jsonb_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"{\\\"formmanager_getfiltered\\\": []}\",\"{\\\"glossary\\\": {\\\"title\\\": \\\"example glossary\\\", \\\"GlossDiv\\\": {\\\"title\\\": \\\"S\\\", \\\"GlossList\\\": {\\\"GlossEntry\\\": {\\\"ID\\\": \\\"SGML\\\", \\\"Abbrev\\\": \\\"ISO 8879:1986\\\", \\\"SortAs\\\": \\\"SGML\\\", \\\"Acronym\\\": \\\"SGML\\\", \\\"GlossDef\\\": {\\\"para\\\": \\\"A meta-markup language, used to create markup languages such as DocBook.\\\", \\\"GlossSeeAlso\\\": [\\\"GML\\\", \\\"XML\\\"]}, \\\"GlossSee\\\": \\\"markup\\\", \\\"GlossTerm\\\": \\\"Standard Generalized Markup Language\\\"}}}}}\",\"{\\\"menu\\\": {\\\"id\\\": \\\"file\\\", \\\"popup\\\": {\\\"menuitem\\\": [{\\\"value\\\": \\\"New\\\", \\\"onclick\\\": \\\"CreateNewDoc()\\\"}, {\\\"value\\\": \\\"Open\\\", \\\"onclick\\\": \\\"OpenDoc()\\\"}, {\\\"value\\\": \\\"Close\\\", \\\"onclick\\\": \\\"CloseDoc()\\\"}]}, \\\"value\\\": \\\"File\\\"}}\"]}}}
      
      */

      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_jsonb_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<string>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToString(item)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty));
        }
        Assert.That(actual, Is.EqualTo(new string[3] { "{\"formmanager_getfiltered\": []}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                                       "{\"glossary\": {\"title\": \"example glossary\", \"GlossDiv\": {\"title\": \"S\", \"GlossList\": {\"GlossEntry\": {\"ID\": \"SGML\", \"Abbrev\": \"ISO 8879:1986\", \"SortAs\": \"SGML\", \"Acronym\": \"SGML\", \"GlossDef\": {\"para\": \"A meta-markup language, used to create markup languages such as DocBook.\", \"GlossSeeAlso\": [\"GML\", \"XML\"]}, \"GlossSee\": \"markup\", \"GlossTerm\": \"Standard Generalized Markup Language\"}}}}}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                                       "{\"menu\": {\"id\": \"file\", \"popup\": {\"menuitem\": [{\"value\": \"New\", \"onclick\": \"CreateNewDoc()\"}, {\"value\": \"Open\", \"onclick\": \"OpenDoc()\"}, {\"value\": \"Close\", \"onclick\": \"CloseDoc()\"}]}, \"value\": \"File\"}}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)
        }));
        return;
      }

      Assert.Fail();
    }
    #endregion array

    [Test]
    public void GetOutAll()
    {
      var command = new Command("migration.get_out_all");

      command.Parameters.Add("p_parameter1", PgsqlDbType.Bigint);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Boolean);
      command.Parameters.Add("p_parameter3", PgsqlDbType.Bytea);
      command.Parameters.Add("p_parameter4", PgsqlDbType.Double);
      command.Parameters.Add("p_parameter5", PgsqlDbType.Integer);
      command.Parameters.Add("p_parameter6", PgsqlDbType.Money);
      command.Parameters.Add("p_parameter7", PgsqlDbType.Numeric);
      command.Parameters.Add("p_parameter8", PgsqlDbType.Real);
      command.Parameters.Add("p_parameter9", PgsqlDbType.Smallint);
      command.Parameters.Add("p_parameter10", PgsqlDbType.Text);
      command.Parameters.Add("p_parameter11", PgsqlDbType.Date);
      command.Parameters.Add("p_parameter12", PgsqlDbType.Time);
      command.Parameters.Add("p_parameter13", PgsqlDbType.TimeTZ);
      command.Parameters.Add("p_parameter14", PgsqlDbType.Timestamp);
      command.Parameters.Add("p_parameter15", PgsqlDbType.TimestampTZ);
      command.Parameters.Add("p_parameter16", PgsqlDbType.Varchar);
      command.Parameters.Add("p_parameter17", PgsqlDbType.Uuid);
      command.Parameters.Add("p_parameter18", PgsqlDbType.Xml);
      command.Parameters.Add("p_parameter19", PgsqlDbType.Json);
      command.Parameters.Add("p_parameter20", PgsqlDbType.Jsonb);

      /*

      {\"migration.get_out_all\":{\"returnValue\":-1,\"arguments\":{\"p_parameter1\":9223372036854775807,\"p_parameter2\":true,\"p_parameter3\":\"aGVsbG8gd29ybGQh\",\"p_parameter4\":1234567890.12345,\"p_parameter5\":2147483647,\"p_parameter6\":92233720368547758.07,\"p_parameter7\":123456789012345678.1234567890,\"p_parameter8\":1.234568E+09,\"p_parameter9\":32767,\"p_parameter10\":\"PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.\",\"p_parameter11\":\"2021-05-18T00:00:00\",\"p_parameter12\":\"13:44:46.9876000\",\"p_parameter13\":\"0001-01-02T14:41:45.1234+03:00\",\"p_parameter14\":\"2022-03-18T12:42:46.1234\",\"p_parameter15\":\"2021-04-18T12:43:47.1234Z\",\"p_parameter16\":\"PostgreSQL change column type examples\",\"p_parameter17\":\"79130b53-3113-41d1-99ec-26e41b
       238394\",\"p_parameter18\":\"<_routines>\\n  <_routine>\\n    <_name>formmanager_getfiltered</_name>\\n    <_arguments>\\n      <_formid>0</_formid>\\n      <_form></_form>\\n      <_businessids>1</_businessids>\\n      <_businessids>941</_businessids>\\n      <_businessids>942</_businessids>\\n      <_businessids>943</_businessids>\\n      <_businessids>944</_businessids>\\n      <_businessids>2006</_businessids>\\n      <_businessids>2129</_businessids>\\n      <_businessids>2135</_businessids>\\n      <_businessids>2137</_businessids>\\n      <_formtype>1</_formtype>\\n      <_formtype>2</_formtype>\\n      <_formtype>3</_formtype>\\n      <_formtype>4</_formtype>\\n      <_formtype>5</_formtype>\\n      <_formtype>6</_formtype>\\n      <_formtype>7</_formtype>\\n      <_formtype>8</_formtype>\\n      <_inactive>False</_inactive>\\n    </_arguments>\\n    <_options>\\n      <_writeSchema>1</_writeSchema>\\n    </_options>\\n  </_routine>\\n  <_compression>0</_compression>\\n  <_returnType>json</_returnType>
       \\n</_routines>\",\"p_parameter19\":\"{\\n    \\\"formmanager_getfiltered\\\": []\\n}\",\"p_parameter20\":\"{\\\"formmanager_getfiltered\\\": []}\"}}}

      */

      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);

      object pp1v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter1"]!;
      var actual1 = Convert.ToInt64(pp1v);
      long expected1 = 9223372036854775807;
      Assert.That(actual1, Is.EqualTo(expected1));

      object pp2v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter2"]!;
      var actual2 = Convert.ToBoolean(pp2v);
      bool expected2 = true;
      Assert.That(actual2, Is.EqualTo(expected2));

      object pp3v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter3"]!;
      var actual3 = pp3v.ToString();
      string expected3 = "aGVsbG8gd29ybGQh";
      Assert.That(actual3, Is.EqualTo(expected3));

      object pp4v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter4"]!;
      var actual4 = Convert.ToDouble(pp4v);
      double expected4 = 1234567890.12345;
      Assert.That(actual4, Is.EqualTo(expected4));

      object pp5v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter5"]!;
      var actual5 = Convert.ToDouble(pp5v);
      int expected5 = 2147483647;
      Assert.That(actual5, Is.EqualTo(expected5));

      object pp6v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter6"]!;
      var actual6 = Convert.ToDecimal(pp6v);
      decimal expected6 = 92233720368547758.07m;
      Assert.That(actual6, Is.EqualTo(expected6));

      object pp7v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter7"]!;
      var actual7 = Convert.ToDecimal(pp7v);
      decimal expected7 = 123456789012345678.1234567890m;
      Assert.That(actual7, Is.EqualTo(expected7));

      object pp8v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter8"]!;
      var actual8 = Convert.ToSingle(pp8v);
      float expected8 = 1.234568E+09f;
      Assert.That(actual8, Is.EqualTo(expected8));

      object pp9v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter9"]!;
      var actual9 = Convert.ToInt16(pp9v);
      short expected9 = 32767;
      Assert.That(actual9, Is.EqualTo(expected9));

      object pp10v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter10"]!;
      var actual10 = Convert.ToString(pp10v);
      var expected10 = "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.";
      Assert.That(actual10, Is.EqualTo(expected10));

      object pp11v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter11"]!;
      var actual11 = Convert.ToDateTime(pp11v);
      var expected11 = DateTime.Parse("2021-05-18T00:00:00");
      Assert.That(actual11, Is.EqualTo(expected11));

      object pp12v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter12"]!;
      var actual12 = TimeSpan.Parse(Convert.ToString(pp12v)!);
      var expected12 = TimeSpan.Parse("13:44:46.9876000");
      Assert.That(actual12, Is.EqualTo(expected12));

      object pp13v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter13"]!;
      var actual13 = DateTimeOffset.Parse(Convert.ToString(pp13v)!);
      var expected13 = DateTimeOffset.Parse("0001-01-02T14:41:45.1234+03:00");
      Assert.That(actual13, Is.EqualTo(expected13));

      object pp14v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter14"]!;
      var actual14 = DateTime.Parse(Convert.ToString(pp14v)!);
      var expected14 = DateTime.Parse("2022-03-18T12:42:46.1234");
      Assert.That(actual14, Is.EqualTo(expected14));

      object pp15v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter15"]!;
      var actual15 = DateTime.Parse(Convert.ToString(pp15v)!);
      var expected15 = DateTime.Parse("2021-04-18T12:43:47.1234Z");
      Assert.That(actual15, Is.EqualTo(expected15));

      object pp16v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter16"]!;
      var actual16 = Convert.ToString(pp16v)!;
      var expected16 = "PostgreSQL change column type examples";
      Assert.That(actual16, Is.EqualTo(expected16));

      object pp17v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter17"]!;
      var actual17 = Guid.Parse(Convert.ToString(pp17v)!);
      var expected17 = Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394");
      Assert.That(actual17, Is.EqualTo(expected17));

      object pp18v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter18"]!;
      var actual18 = Convert.ToString(pp18v)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      var expected18 = @"<_routines>
        <_routine>
          <_name>formmanager_getfiltered</_name>
          <_arguments>
            <_formid>0</_formid>
            <_form></_form>
            <_businessids>1</_businessids>
            <_businessids>941</_businessids>
            <_businessids>942</_businessids>
            <_businessids>943</_businessids>
            <_businessids>944</_businessids>
            <_businessids>2006</_businessids>
            <_businessids>2129</_businessids>
            <_businessids>2135</_businessids>
            <_businessids>2137</_businessids>
            <_formtype>1</_formtype>
            <_formtype>2</_formtype>
            <_formtype>3</_formtype>
            <_formtype>4</_formtype>
            <_formtype>5</_formtype>
            <_formtype>6</_formtype>
            <_formtype>7</_formtype>
            <_formtype>8</_formtype>
            <_inactive>False</_inactive>
          </_arguments>
          <_options>
            <_writeSchema>1</_writeSchema>
          </_options>
        </_routine>
        <_compression>0</_compression>
        <_returnType>json</_returnType>
      </_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      Assert.That(actual18, Is.EqualTo(expected18));

      object pp19v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter19"]!;
      var actual19 = Convert.ToString(pp19v)!.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      var expected19 = @"{ ""formmanager_getfiltered"": [] }".Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty); ;
      Assert.That(actual19, Is.EqualTo(expected19));

      object pp20v = jobject["migration.get_out_all"]!["arguments"]!["p_parameter20"]!;
      var actual20 = Convert.ToString(pp20v)!.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
      var expected20 = @"{ ""formmanager_getfiltered"": [] }".Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty); ;
      Assert.That(actual20, Is.EqualTo(expected20));
    }

    private static void TestIt<T>(object source, object expected)
    {
      var ja = source as JArray;
      if (ja != null)
      {
        var actual1 = ja.Select(x => (T)Convert.ChangeType(x, typeof(T))).ToArray();
        Assert.That(actual1, Is.EqualTo(expected));
        return;
      }
      Assert.Fail();
    }

    [Test]
    public void GetOutAllArray()
    {
      var command = new Command("migration.get_out_all_array");

      command.Parameters.Add("p_parameter1", PgsqlDbType.Bigint | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Boolean | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter3", PgsqlDbType.Bytea | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter4", PgsqlDbType.Double | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter5", PgsqlDbType.Integer | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter6", PgsqlDbType.Money | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter7", PgsqlDbType.Numeric | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter8", PgsqlDbType.Real | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter9", PgsqlDbType.Smallint | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter10", PgsqlDbType.Text | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter11", PgsqlDbType.Date | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter12", PgsqlDbType.Time | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter13", PgsqlDbType.TimeTZ | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter14", PgsqlDbType.Timestamp | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter15", PgsqlDbType.TimestampTZ | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter16", PgsqlDbType.Varchar | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter17", PgsqlDbType.Uuid | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter18", PgsqlDbType.Xml | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter19", PgsqlDbType.Json | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter20", PgsqlDbType.Jsonb | PgsqlDbType.Array);

      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);

      TestIt<long>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter1"]!,
                   new long[] { 9223372036854775807, 9223372036854775806, 9223372036854775805 });

      TestIt<bool>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter2"]!,
                   new bool[] { true, false, true });

      TestIt<string>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter3"]!,
                     new string[] { "aGVsbG8gd29ybGQh", "VGhlcmUgYXJlIHRocmVlIG1ldGhvZHMgdXNlZCB0byBhZGp1c3QgYSBEYXRlT25seSBzdHJ1Y3R1cmU6IEFkZERheXMsIEFkZE1vbnRocywgYW5kIEFkZFllYXJzLiBFYWNoIG1ldGhvZCB0YWtlcyBhbiBpbnRlZ2VyIHBhcmFtZXRlciwgYW5kIGluY3JlYXNlcyB0aGUgZGF0ZSBieSB0aGF0IG1lYXN1cmVtZW50Lg==", "SWYgYSBuZWdhdGl2ZSBudW1iZXIgaXMgcHJvdmlkZWQsIHRoZSBkYXRlIGlzIGRlY3JlYXNlZCBieSB0aGF0IG1lYXN1cmVtZW50LiBUaGUgbWV0aG9kcyByZXR1cm4gYSBuZXcgaW5zdGFuY2Ugb2YgRGF0ZU9ubHksIGFzIHRoZSBzdHJ1Y3R1cmUgaXMgaW1tdXRhYmxlLg==" });

      TestIt<double>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter4"]!,
                     new double[] { 1234567890.12345, 1234567889.6789, 1234567888.01478 });

      TestIt<int>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter5"]!,
                  new int[] { 2147483647, 2147483646, 2147483645 });

      TestIt<decimal>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter6"]!,
                      new decimal[] { 92233720368547758.07m, 92233720368547757.05m, 92233720368547756.06m });

      TestIt<decimal>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter7"]!,
                      new decimal[] { 123456789012345678.1234567890m, 123456789012345677.1234567889m, 123456789012345676.1234567888m });

      TestIt<float>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter8"]!,
                      new float[] { 1.234568E+09f, 1.234568E+09f, 1.234568E+09f });

      TestIt<short>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter9"]!,
                new short[] { 32767, 32766, 32765 });

      TestIt<string>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter10"]!,
                new string[] { "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                                   "DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.",
                                   "DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date." });

      TestIt<DateTime>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter11"]!,
                new DateTime[] { DateTime.Parse("2021-05-18T00:00:00"), DateTime.Parse("2020-04-17T00:00:00"), DateTime.Parse("2019-03-16T00:00:00") });

      TestIt<TimeSpan>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter12"]!,
                new TimeSpan[] { TimeSpan.Parse("13:44:46.9876000"), TimeSpan.Parse("11:43:45.9875000"), TimeSpan.Parse("11:42:44.9874000") });

      TestIt<DateTimeOffset>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter13"]!,
                new DateTimeOffset[] { DateTimeOffset.Parse("0001-01-02T14:41:45.1234+03:00"), DateTimeOffset.Parse("0001-01-02T13:39:44.1233+02:00"), DateTimeOffset.Parse("0001-01-02T11:38:42.1232+01:00") });

      TestIt<DateTime>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter14"]!,
                new DateTime[] { DateTime.Parse("2022-03-18T12:42:46.1234"), DateTime.Parse("2020-01-16T10:40:44.1232"), DateTime.Parse("2019-09-15T09:39:43.1231") });

      TestIt<DateTime>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter15"]!,
          new DateTime[] { DateTime.Parse("2021-04-18T12:43:47.1234Z"), DateTime.Parse("2018-01-15T10:40:44.1231Z"), DateTime.Parse("2017-01-14T07:39:44.123Z") });

      TestIt<string>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter16"]!,
          new string[] { "PostgreSQL change column type examples", "What is the PostgreSQL Function?", "PostgreSQL change column type examples" });

      TestIt<string>(jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter17"]!,
          new string[] { "79130b53-3113-41d1-99ec-26e41b238394", "f0c180ba-e291-4089-91b4-3d8d122b5c77", "670c4c79-521c-40e2-8442-0248a93f8737" });

      var ja18 = jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter18"]! as JArray;
      if (ja18 != null)
      {
        var result18 = ja18.Select(x => Convert.ToString(x))
                         .Where(x => x != null)
                         .Select(x => x!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)).ToArray();
        var expected18 = new string[] { "<_routines>\n  <_routine>\n    <_name>formmanager_getfiltered</_name>\n    <_arguments>\n      <_formid>0</_formid>\n      <_form></_form>\n      <_businessids>1</_businessids>\n      <_businessids>941</_businessids>\n      <_businessids>942</_businessids>\n      <_businessids>943</_businessids>\n      <_businessids>944</_businessids>\n      <_businessids>2006</_businessids>\n      <_businessids>2129</_businessids>\n      <_businessids>2135</_businessids>\n      <_businessids>2137</_businessids>\n      <_formtype>1</_formtype>\n      <_formtype>2</_formtype>\n      <_formtype>3</_formtype>\n      <_formtype>4</_formtype>\n      <_formtype>5</_formtype>\n      <_formtype>6</_formtype>\n      <_formtype>7</_formtype>\n      <_formtype>8</_formtype>\n      <_inactive>False</_inactive>\n    </_arguments>\n    <_options>\n      <_writeSchema>1</_writeSchema>\n    </_options>\n  </_routine>\n  <_compression>0</_compression>\n  <_returnType>json</_returnType>\n</_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                        "<_routines>\n          <_routine>\n            <_name>InitializeSession</_name>\n            <_arguments>\n              <login>sadmin@upstairs.com</login>\n              <password>George555#</password>\n              <isEncrypt>0</isEncrypt>\n              <timeout>20</timeout>\n              <appId>38</appId>\n              <appVersion>3.8.6</appVersion>\n              <domain>naitonmaster</domain>\n            </_arguments>\n          </_routine>\n          <_returnType>xml</_returnType>\n        </_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                        "<_routines>\n  <_routine>\n    <_name>companymanager_getfilteredcompanieslist</_name>\n    <_arguments>\n      <_companyid>0</_companyid>\n      <_companyname></_companyname>\n      <_countryid>0</_countryid>\n      <_businessgroupid>0</_businessgroupid>\n      <_businessid>1</_businessid>\n      <_email></_email>\n      <_zipcode></_zipcode>\n      <_housenumber></_housenumber>\n      <_statusid>-3</_statusid>\n      <_statusid>4</_statusid>\n      <_statusid>6</_statusid>\n      <_statusid>5</_statusid>\n      <_iban></_iban>\n      <_salesmanagerid>0</_salesmanagerid>\n      <_onlyholding>False</_onlyholding>\n      <_udffilter></_udffilter>\n      <_holding></_holding>\n      <_holdingalso>False</_holdingalso>\n      <_companytypeid>2</_companytypeid>\n      <_segmentid>0</_segmentid>\n      <_segmentudf></_segmentudf>\n      <_discountgroupid>-1</_discountgroupid>\n      <_taxnumber></_taxnumber>\n      <_chamberofcommerce></_chamberofcommerce>\n      <_havechildonly>False</_havechildonly>\n      <_reseller></_reseller>\n      <_inactive>False</_inactive>\n      <_companyids isNull=\"true\" />\n      <_limit>200</_limit>\n    </_arguments>\n    <_options>\n      <_writeSchema>1</_writeSchema>\n    </_options>\n  </_routine>\n  <_compression>{{compression}}</_compression>\n  <_returnType>json</_returnType>\n</_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)};
        Assert.That(result18, Is.EqualTo(expected18));
      }
      else
      {
        Assert.Fail();
      }

      var ja19 = jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter19"]! as JArray;
      if (ja19 != null)
      {
        var result19 = ja19.Select(x => Convert.ToString(x))
                           .Where(x => x != null)
                           .Select(x => x!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)).ToArray();
        var expected19 = new string[] { "{\n    \"formmanager_getfiltered\": []\n}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                            "{\n    \"glossary\": {\n        \"title\": \"example glossary\",\n\t\t\"GlossDiv\": {\n            \"title\": \"S\",\n\t\t\t\"GlossList\": {\n                \"GlossEntry\": {\n                    \"ID\": \"SGML\",\n\t\t\t\t\t\"SortAs\": \"SGML\",\n\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\n\t\t\t\t\t\"Acronym\": \"SGML\",\n\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\n\t\t\t\t\t\"GlossDef\": {\n                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\n\t\t\t\t\t\t\"GlossSeeAlso\": [\"GML\", \"XML\"]\n                    },\n\t\t\t\t\t\"GlossSee\": \"markup\"\n                }\n            }\n        }\n    }\n}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                            "{\"menu\": {\n  \"id\": \"file\",\n  \"value\": \"File\",\n  \"popup\": {\n    \"menuitem\": [\n      {\"value\": \"New\", \"onclick\": \"CreateNewDoc()\"},\n      {\"value\": \"Open\", \"onclick\": \"OpenDoc()\"},\n      {\"value\": \"Close\", \"onclick\": \"CloseDoc()\"}\n    ]\n  }\n}}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)};
        Assert.That(result19, Is.EqualTo(expected19));
      }
      else
      {
        Assert.Fail();
      }

      var ja20 = jobject["migration.get_out_all_array"]!["arguments"]!["p_parameter20"]! as JArray;
      if (ja20 != null)
      {
        var result20 = ja20.Select(x => Convert.ToString(x))
                           .Where(x => x != null)
                           .Select(x => x!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)).ToArray();
        var expected20 = new string[] { "{\"formmanager_getfiltered\": []}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                            "{\"glossary\": {\"title\": \"example glossary\", \"GlossDiv\": {\"title\": \"S\", \"GlossList\": {\"GlossEntry\": {\"ID\": \"SGML\", \"Abbrev\": \"ISO 8879:1986\", \"SortAs\": \"SGML\", \"Acronym\": \"SGML\", \"GlossDef\": {\"para\": \"A meta-markup language, used to create markup languages such as DocBook.\", \"GlossSeeAlso\": [\"GML\", \"XML\"]}, \"GlossSee\": \"markup\", \"GlossTerm\": \"Standard Generalized Markup Language\"}}}}}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty),
                                            "{\"menu\": {\"id\": \"file\", \"popup\": {\"menuitem\": [{\"value\": \"New\", \"onclick\": \"CreateNewDoc()\"}, {\"value\": \"Open\", \"onclick\": \"OpenDoc()\"}, {\"value\": \"Close\", \"onclick\": \"CloseDoc()\"}]}, \"value\": \"File\"}}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)};
        Assert.That(result20, Is.EqualTo(expected20));
      }
      else
      {
        Assert.Fail();
      }
    }

    #region with in and out parameters
    public static void TestNonQueryInOutObject<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType, HttpMethod httpMethod = HttpMethod.GET)
    {
      var command = new Command(postgreSQLFunctionName);
      command.Parameters.Add("p_parameter1", pgsqlDbType, value);
      command.Parameters.Add("p_parameter2", pgsqlDbType);
      var response = Command.Execute(command, RoutineType.NonQuery, httpMethod);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var actual = Convert.ChangeType(jobject[postgreSQLFunctionName]!["arguments"]!["p_parameter2"]!, typeof(T));
      var expected = (T)value;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetInOutBigint()
    {
      TestNonQueryInOutObject<long>("migration.get_in_and_out_bigint", 9223372036854775807, PgsqlDbType.Bigint);
      TestNonQueryInOutObject<long>("migration.get_in_and_out_bigint", 9223372036854775807, PgsqlDbType.Bigint, HttpMethod.POST);
    }

    [Test]
    public void GetInOutBoolean()
    {
      TestNonQueryInOutObject<bool>("migration.get_in_and_out_boolean", true, PgsqlDbType.Boolean);
      TestNonQueryInOutObject<bool>("migration.get_in_and_out_boolean", true, PgsqlDbType.Boolean, HttpMethod.POST);
    }

    [Test]
    public void GetInOutBytea()
    {
      var command = new Command("migration.get_in_and_out_bytea");
      var expected = System.Text.Encoding.UTF8.GetBytes("hello world from bytea testing");
      command.Parameters.Add("p_parameter1", PgsqlDbType.Bytea, expected);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Bytea);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var actual = Convert.FromBase64String(Convert.ToString(jobject["migration.get_in_and_out_bytea"]!["arguments"]!["p_parameter2"]!)!);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetInOutDoublePrecision()
    {
      TestNonQueryInOutObject<double>("migration.get_in_and_out_double_precision", 1234567888.01478, PgsqlDbType.Double);
      TestNonQueryInOutObject<double>("migration.get_in_and_out_double_precision", 1234567888.01478, PgsqlDbType.Double, HttpMethod.POST);
    }

    [Test]
    public void GetInOutInteger()
    {
      TestNonQueryInOutObject<int>("migration.get_in_and_out_integer", 2147483645, PgsqlDbType.Integer);
      TestNonQueryInOutObject<int>("migration.get_in_and_out_integer", 2147483645, PgsqlDbType.Integer, HttpMethod.POST);
    }

    [Test]
    public void GetInOutMoney()
    {
      TestNonQueryInOutObject<decimal>("migration.get_in_and_out_money", 92233720368547756.06m, PgsqlDbType.Money);
      TestNonQueryInOutObject<decimal>("migration.get_in_and_out_money", 92233720368547756.06m, PgsqlDbType.Money, HttpMethod.POST);
    }

    [Test]
    public void GetInOutNumeric()
    {
      TestNonQueryInOutObject<decimal>("migration.get_in_and_out_numeric", 123456789012345676.1234567888m, PgsqlDbType.Numeric);
      TestNonQueryInOutObject<decimal>("migration.get_in_and_out_numeric", 123456789012345676.1234567888m, PgsqlDbType.Numeric, HttpMethod.POST);
    }

    [Test]
    public void GetInOutReal()
    {
      TestNonQueryInOutObject<float>("migration.get_in_and_out_real", 1234567888.12343f, PgsqlDbType.Real);
      TestNonQueryInOutObject<float>("migration.get_in_and_out_real", 1234567888.12343f, PgsqlDbType.Real, HttpMethod.POST);
    }

    [Test]
    public void GetInOutSmallint()
    {
      TestNonQueryInOutObject<short>("migration.get_in_and_out_smallint", (short)32765, PgsqlDbType.Smallint);
      TestNonQueryInOutObject<short>("migration.get_in_and_out_smallint", (short)32765, PgsqlDbType.Smallint, HttpMethod.POST);
    }

    [Test]
    public void GetInOutText()
    {
      TestNonQueryInOutObject<string>("migration.get_in_and_out_text", "Hello world from Text testing", PgsqlDbType.Text);
      TestNonQueryInOutObject<string>("migration.get_in_and_out_text", "Hello world from Text testing", PgsqlDbType.Text, HttpMethod.POST);
    }

    [Test]
    public void GetInOutDate()
    {
      TestNonQueryInOutObject<DateTime>("migration.get_in_and_out_date", DateTime.Parse("2019.03.16"), PgsqlDbType.Date);
      TestNonQueryInOutObject<DateTime>("migration.get_in_and_out_date", DateTime.Parse("2019.03.16"), PgsqlDbType.Date, HttpMethod.POST);
    }

    [Test]
    public void GetInOutTime()
    {
      TestNonQueryInOutObject<TimeSpan>("migration.get_in_and_out_time", TimeSpan.Parse("10:10:12.123"), PgsqlDbType.Time);
      TestNonQueryInOutObject<TimeSpan>("migration.get_in_and_out_time", TimeSpan.Parse("10:10:12.123"), PgsqlDbType.Time, HttpMethod.POST);
    }

    [Test]
    public void GetInOutTimetz()
    {
      TestNonQueryInOutObject<DateTimeOffset>("migration.get_in_and_out_timetz", DateTimeOffset.Parse("0001-01-02T10:10:12.256+03"), PgsqlDbType.TimeTZ);
      TestNonQueryInOutObject<DateTimeOffset>("migration.get_in_and_out_timetz", DateTimeOffset.Parse("0001-01-02T10:10:12.256+03"), PgsqlDbType.TimeTZ, HttpMethod.POST);
    }

    [Test]
    public void GetInOutTimestamp()
    {
      TestNonQueryInOutObject<DateTime>("migration.get_in_and_out_timestamp", DateTime.Parse("2023-05-23 10:10:12.256"), PgsqlDbType.Timestamp);
      TestNonQueryInOutObject<DateTime>("migration.get_in_and_out_timestamp", DateTime.Parse("2023-05-23 10:10:12.256"), PgsqlDbType.Timestamp, HttpMethod.POST);
    }

    [Test]
    public void GetInOutTimestamptz()
    {
      TestNonQueryInOutObject<DateTime>("migration.get_in_and_out_timestamptz", DateTime.Parse("2023-05-23 10:10:12.256+05"), PgsqlDbType.TimestampTZ);
      TestNonQueryInOutObject<DateTime>("migration.get_in_and_out_timestamptz", DateTime.Parse("2023-05-23 10:10:12.256+05"), PgsqlDbType.TimestampTZ, HttpMethod.POST);
    }

    [Test]
    public void GetInOutVachar()
    {
      TestNonQueryInOutObject<string>("migration.get_in_and_out_varchar", "hello world!", PgsqlDbType.Varchar);
      TestNonQueryInOutObject<string>("migration.get_in_and_out_varchar", "hello world!", PgsqlDbType.Varchar, HttpMethod.POST);
    }

    [Test]
    public void GetInOutUuid()
    {
      TestNonQueryInOutObject<string>("migration.get_in_and_out_uuid", "79130b53-3113-41d1-99ec-26e41b238394", PgsqlDbType.Uuid);
      TestNonQueryInOutObject<string>("migration.get_in_and_out_uuid", "79130b53-3113-41d1-99ec-26e41b238394", PgsqlDbType.Uuid, HttpMethod.POST);
    }

    [Test]
    public void GetInOutXml()
    {
      var command = new Command("migration.get_in_and_out_xml");
      var expected = @"<_routines>
      <_routine>
        <_name>formmanager_getfiltered</_name>
        <_arguments>
          <_formid>0</_formid>
          <_form></_form>
          <_businessids>1</_businessids>
          <_businessids>941</_businessids>
          <_businessids>942</_businessids>
          <_businessids>943</_businessids>
          <_businessids>944</_businessids>
          <_businessids>2006</_businessids>
          <_businessids>2129</_businessids>
          <_businessids>2135</_businessids>
          <_businessids>2137</_businessids>
          <_formtype>1</_formtype>
          <_formtype>2</_formtype>
          <_formtype>3</_formtype>
          <_formtype>4</_formtype>
          <_formtype>5</_formtype>
          <_formtype>6</_formtype>
          <_formtype>7</_formtype>
          <_formtype>8</_formtype>
          <_inactive>False</_inactive>
        </_arguments>
        <_options>
          <_writeSchema>1</_writeSchema>
        </_options>
      </_routine>
      <_compression>0</_compression>
      <_returnType>json</_returnType>
    </_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty);

      command.Parameters.Add("p_parameter1", PgsqlDbType.Xml, expected);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Xml);

      // TO DO: this test works when the http method is HttpMethod.POST, and does not work for HttpMethod.GET
      var response = Command.Execute(command, RoutineType.NonQuery, HttpMethod.POST);

      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var actual = Convert.ToString(jobject["migration.get_in_and_out_xml"]!["arguments"]!["p_parameter2"]!)!.Replace("\r", string.Empty).Replace("\n", string.Empty); ;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetInOutJson()
    {
      var command = new Command("migration.get_in_and_out_json");
      var expected = "{\r\n    \"glossary\": {\r\n        \"title\": \"example glossary\",\r\n\t\t\"GlossDiv\": {\r\n            \"title\": \"S\",\r\n\t\t\t\"GlossList\": {\r\n                \"GlossEntry\": {\r\n                    \"ID\": \"SGML\",\r\n\t\t\t\t\t\"SortAs\": \"SGML\",\r\n\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\r\n\t\t\t\t\t\"Acronym\": \"SGML\",\r\n\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\r\n\t\t\t\t\t\"GlossDef\": {\r\n                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\r\n\t\t\t\t\t\t\"GlossSeeAlso\": [\"GML\", \"XML\"]\r\n                    },\r\n\t\t\t\t\t\"GlossSee\": \"markup\"\r\n                }\r\n            }\r\n        }\r\n    }\r\n}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty); ;
      command.Parameters.Add("p_parameter1", PgsqlDbType.Json, expected);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Json);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var actual = Convert.ToString(jobject["migration.get_in_and_out_json"]!["arguments"]!["p_parameter2"]!)!.Replace("\r", string.Empty).Replace("\n", string.Empty); ;
      Assert.That(actual, Is.EqualTo(expected));
    }

    static void Sort(JObject jObj)
    {
      var props = jObj.Properties().ToList();
      foreach (var prop in props)
      {
        prop.Remove();
      }

      foreach (var prop in props.OrderBy(p => p.Name))
      {
        jObj.Add(prop);
        if (prop.Value is JObject)
          Sort((JObject)prop.Value);
        if (prop.Value is JArray)
        {
          Int32 iCount = prop.Value.Count();
          for (Int32 iIterator = 0; iIterator < iCount; iIterator++)
            if (prop.Value[iIterator] is JObject)
              Sort((JObject)prop.Value[iIterator]);
        }
      }
    }

    [Test]
    public void GetInOutJsonb()
    {
      var command = new Command("migration.get_in_and_out_jsonb");
      var value = "{\r\n    \"glossary\": {\r\n        \"title\": \"example glossary\",\r\n\t\t\"GlossDiv\": {\r\n            \"title\": \"S\",\r\n\t\t\t\"GlossList\": {\r\n                \"GlossEntry\": {\r\n                    \"ID\": \"SGML\",\r\n\t\t\t\t\t\"SortAs\": \"SGML\",\r\n\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\r\n\t\t\t\t\t\"Acronym\": \"SGML\",\r\n\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\r\n\t\t\t\t\t\"GlossDef\": {\r\n                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\r\n\t\t\t\t\t\t\"GlossSeeAlso\": [\"GML\", \"XML\"]\r\n                    },\r\n\t\t\t\t\t\"GlossSee\": \"markup\"\r\n                }\r\n            }\r\n        }\r\n    }\r\n}";
      command.Parameters.Add("p_parameter1", PgsqlDbType.Jsonb, value);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Jsonb);

      var response = Command.Execute(command, RoutineType.NonQuery, HttpMethod.POST);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);

      // we need to convert both json strings to JObject, and after that to order JObject by values,
      // because PostgreSQL keeps jsonb objects by their inner optimization, which violates the order of key - values of the json object

      var expected = JObject.Parse(value);
      Sort(expected);

      var actual = JObject.Parse(Convert.ToString(jobject["migration.get_in_and_out_jsonb"]!["arguments"]!["p_parameter2"]!)!);
      Sort(actual);

      Assert.That(actual, Is.EqualTo(expected));
    }

    #region array
    private static void GetInOutArrayTest<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType)
    {
      var command = new Command(postgreSQLFunctionName);
      command.Parameters.Add("p_parameter1", pgsqlDbType, value);
      command.Parameters.Add("p_parameter2", pgsqlDbType);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var pp2v = jobject[postgreSQLFunctionName]!["arguments"]!["p_parameter2"];
      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<T[]>();
        var expected = value as T[];
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetInOutBigintArray()
    {
      GetInOutArrayTest<long>("migration.get_in_and_out_bigint_array", new long[] { 9223372036854775701, 9223372036854775704, 9223372036854775703 }, PgsqlDbType.Bigint | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutBooleanArray()
    {
      GetInOutArrayTest<bool>("migration.get_in_and_out_boolean_array", new bool[] { true, false, true }, PgsqlDbType.Boolean | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutByteaArray()
    {
      var command = new Command("migration.get_in_and_out_bytea_array");
      byte[][] expected = new byte[][] { System.Text.Encoding.UTF8.GetBytes("hello world!"),
                                         System.Text.Encoding.UTF8.GetBytes("There are three methods used to adjust a DateOnly structure: AddDays, AddMonths, and AddYears. Each method takes an integer parameter, and increases the date by that measurement."),
                                         System.Text.Encoding.UTF8.GetBytes("If a negative number is provided, the date is decreased by that measurement. The methods return a new instance of DateOnly, as the structure is immutable.")
          };
      command.Parameters.Add("p_parameter1", PgsqlDbType.Bytea | PgsqlDbType.Array, expected);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Bytea | PgsqlDbType.Array);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["arguments"]!["p_parameter2"];
      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<string[]>()!.Select(x => Convert.FromBase64String(x)).ToArray();
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetInOutDoublePrecisionArray()
    {
      GetInOutArrayTest<double>("migration.get_in_and_out_double_precision_array", new double[] { 1234567890.12345, 1234567889.67891, 1234567888.01478 }, PgsqlDbType.Double | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutIntegerArray()
    {
      GetInOutArrayTest<int>("migration.get_in_and_out_integer_array", new int[] { 2147483647, 2147483646, 2147483645 }, PgsqlDbType.Integer | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutMoneyArray()
    {
      GetInOutArrayTest<decimal>("migration.get_in_and_out_money_array", new decimal[] { 92233720368547758.07m, 92233720368547757.05m, 92233720368547756.06m }, PgsqlDbType.Money | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutNumericArray()
    {
      GetInOutArrayTest<decimal>("migration.get_in_and_out_numeric_array", new decimal[] { 123456789012345678.1234567890m, 123456789012345677.1234567889m, 123456789012345676.1234567888m }, PgsqlDbType.Numeric | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutRealArray()
    {
      GetInOutArrayTest<float>("migration.get_in_and_out_real_array", new float[] { 123456789012345678.1234567890f, 123456789012345677.1234567889f, 123456789012345676.1234567888f }, PgsqlDbType.Real | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutSmallintArray()
    {
      GetInOutArrayTest<short>("migration.get_in_and_out_smallint_array", new short[] { 32767, 32766, 32765 }, PgsqlDbType.Smallint | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutTextArray()
    {
      GetInOutArrayTest<string>("migration.get_in_and_out_text_array", new string[] { "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                                                                                      "DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.",
                                                                                      "DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date." },
                                PgsqlDbType.Text | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutDateArray()
    {
      GetInOutArrayTest<DateTime>("migration.get_in_and_out_date_array", new DateTime[] { DateTime.Parse("2021.05.18"),
                                                                                          DateTime.Parse("2020.04.17"),
                                                                                          DateTime.Parse("2020.04.17") },
                                PgsqlDbType.Date | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutTimeArray()
    {
      GetInOutArrayTest<TimeSpan>("migration.get_in_and_out_time_array", new TimeSpan[] { TimeSpan.Parse("13:44:46.9876"),
                                                                                          TimeSpan.Parse("11:43:45.9875"),
                                                                                          TimeSpan.Parse("11:42:44.9874") },
                                PgsqlDbType.Time | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutTimetzArray()
    {
      GetInOutArrayTest<DateTimeOffset>("migration.get_in_and_out_timetz_array", new DateTimeOffset[] { DateTimeOffset.Parse("001-01-02T14:41:45.1234+03"),
                                                                                                        DateTimeOffset.Parse("001-01-02T13:39:44.1233+02"),
                                                                                                        DateTimeOffset.Parse("001-01-02T11:38:42.1232+01")},
                                PgsqlDbType.TimeTZ | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutTimestampArray()
    {
      GetInOutArrayTest<DateTime>("migration.get_in_and_out_timestamp_array", new DateTime[] { DateTime.Parse("2022.03.18 12:42:46.1234"),
                                                                                               DateTime.Parse("2020.01.16 10:40:44.1232"),
                                                                                               DateTime.Parse("2019.09.15 09:39:43.1231") },
                                  PgsqlDbType.Timestamp | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutTimestamptzArray()
    {
      var command = new Command("migration.get_in_and_out_timestamptz_array");
      var source = new DateTime[] { DateTime.Parse("2021.04.18 14:43:47.1234+02"),
                                    DateTime.Parse("2018.01.15 11:40:44.1231+01"),
                                    DateTime.Parse("2017.01.14 10:39:44.1230+03")
      };
      command.Parameters.Add("p_parameter1", PgsqlDbType.TimestampTZ | PgsqlDbType.Array, source);
      command.Parameters.Add("p_parameter2", PgsqlDbType.TimestampTZ | PgsqlDbType.Array);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      JObject jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["arguments"]!["p_parameter2"];
      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<DateTime[]>()!.Select(x => x.ToUniversalTime()).ToArray();
        var expected = (source as DateTime[]).Select(x => x.ToUniversalTime()).ToArray();
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetInOutVarcharArray()
    {
      GetInOutArrayTest<string>("migration.get_in_and_out_varchar_array", new string[] { "PostgreSQL change column type examples",
                                                                                         "What is the PostgreSQL Function?",
                                                                                         "PostgreSQL change column type examples" },
                                PgsqlDbType.Varchar | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutUuidArray()
    {
      GetInOutArrayTest<string>("migration.get_in_and_out_uuid_array", new string[] { "79130b53-3113-41d1-99ec-26e41b238394",
                                                                                      "f0c180ba-e291-4089-91b4-3d8d122b5c77",
                                                                                      "670c4c79-521c-40e2-8442-0248a93f8737" },
                                PgsqlDbType.Uuid | PgsqlDbType.Array);
    }

    [Test]
    public void GetInOutXmlArray()
    {
      var command = new Command("migration.get_in_and_out_xml_array");
      var source = new string[] { @"<CATALOG>
      <PLANT>
        <COMMON>Bloodroot</COMMON>
        <BOTANICAL>Sanguinaria canadensis</BOTANICAL>
        <ZONE>4</ZONE>
        <LIGHT>Mostly Shady</LIGHT>
        <PRICE>$2.44</PRICE>
        <AVAILABILITY>031599</AVAILABILITY>
      </PLANT>
      <PLANT>
        <COMMON>Columbine</COMMON>
        <BOTANICAL>Aquilegia canadensis</BOTANICAL>
        <ZONE>3</ZONE>
        <LIGHT>Mostly Shady</LIGHT>
        <PRICE>$9.37</PRICE>
        <AVAILABILITY>030699</AVAILABILITY>
      </PLANT>
    </CATALOG>",
                                                  @"<CATALOG>
    <CD>
    <TITLE>Empire Burlesque</TITLE>
    <ARTIST>Bob Dylan</ARTIST>
    <COUNTRY>USA</COUNTRY>
    <COMPANY>Columbia</COMPANY>
    <PRICE>10.90</PRICE>
    <YEAR>1985</YEAR>
    </CD>
    <CD>
    <TITLE>Hide your heart</TITLE>
    <ARTIST>Bonnie Tyler</ARTIST>
    <COUNTRY>UK</COUNTRY>
    <COMPANY>CBS Records</COMPANY>
    <PRICE>9.90</PRICE>
    <YEAR>1988</YEAR>
    </CD></CATALOG>",
                                                  @"<breakfast_menu>
      <food>
        <name>Belgian Waffles</name>
        <price>$5.95</price>
        <description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
        <calories>650</calories>
      </food>
      <food>
        <name>Strawberry Belgian Waffles</name>
        <price>$7.95</price>
        <description>Light Belgian waffles covered with strawberries and whipped cream</description>
        <calories>900</calories>
      </food>
    </breakfast_menu>"};
      command.Parameters.Add("p_parameter1", PgsqlDbType.Xml | PgsqlDbType.Array, source);
      command.Parameters.Add("p_parameter2", PgsqlDbType.Xml | PgsqlDbType.Array);
      var response = Command.Execute(command, RoutineType.NonQuery, HttpMethod.POST);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["arguments"]!["p_parameter2"];
      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<string[]>()!.Select(x => x.Replace("\r", string.Empty)
                                                           .Replace("\n", string.Empty)
                                                           .Replace("\t", string.Empty)
                                                           .Replace(" ", string.Empty));
        var expected = source.Select(x => x.Replace("\r", string.Empty)
                                           .Replace("\n", string.Empty)
                                           .Replace("\t", string.Empty)
                                           .Replace(" ", string.Empty));
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetInOutJsonArray()
    {
      var command = new Command("migration.get_in_and_out_json_array");
      var parameter1 = command.Parameters.Add("p_parameter1", PgsqlDbType.Json | PgsqlDbType.Array);
      var source = new string[] { @"{
      ""fruit"": ""Apple"",
      ""size"":  ""Large"",
      ""color"": ""Red""
    }",
                                                  @"{""name"":""mkyong.com"",""messages"":[""msg 1"",""msg 2"",""msg 3""],""age"":100}",
                                                  @"{
        ""name"": ""Morpheush"",
        ""job"": ""Leader"",
        ""id"": ""199"",
        ""createdAt"": ""2020-02-20T11:00:28.107Z""
    }"};
      parameter1.Value = source;
      command.Parameters.Add("p_parameter2", PgsqlDbType.Json | PgsqlDbType.Array);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["arguments"]!["p_parameter2"];
      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<string[]>()!.Select(x => x.Replace("\r", string.Empty)
                                                           .Replace("\n", string.Empty)
                                                           .Replace("\t", string.Empty)
                                                           .Replace(" ", string.Empty));
        var expected = source.Select(x => x.Replace("\r", string.Empty)
                                           .Replace("\n", string.Empty)
                                           .Replace("\t", string.Empty)
                                           .Replace(" ", string.Empty));
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }
      Assert.Fail();
    }

    [Test]
    public void GetInOutJsonbArray()
    {
      var command = new Command("migration.get_in_and_out_jsonb_array");
      var parameter1 = new Parameter("p_parameter1", PgsqlDbType.Jsonb | PgsqlDbType.Array);
      var source = new string[] { @"{
      ""fruit"": ""Apple"",
      ""size"":  ""Large"",
      ""color"": ""Red""
    }",
                                                  @"{""name"":""mkyong.com"",""messages"":[""msg 1"",""msg 2"",""msg 3""],""age"":100}",
                                                  @"{
        ""name"": ""Morpheush"",
        ""job"": ""Leader"",
        ""id"": ""199"",
        ""createdAt"": ""2020-02-20T11:00:28.107Z""
    }"};
      parameter1.Value = source;
      command.Parameters.Add(parameter1);

      command.Parameters.Add("p_parameter2", PgsqlDbType.Jsonb | PgsqlDbType.Array);
      var response = Command.Execute(command, RoutineType.NonQuery);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["arguments"]!["p_parameter2"];
      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<string[]>()!.Select(x => x.Replace("\r", string.Empty)
                                                           .Replace("\n", string.Empty)
                                                           .Replace("\t", string.Empty)
                                                           .Replace(" ", string.Empty));

        // we need to convert both json strings to JObject, and after that to order JObject by values,
        // because PostgreSQL keeps jsonb objects by their inner optimization, which violates the order of key - values of the json object

        var jola = new List<JObject>();
        foreach (var x in actual)
        {
          var jo = JObject.Parse(x);
          Sort(jo);
          jola.Add(jo);
        }


        var expected = source.Select(x => x.Replace("\r", string.Empty)
                                           .Replace("\n", string.Empty)
                                           .Replace("\t", string.Empty)
                                           .Replace(" ", string.Empty));

        var jole = new List<JObject>();
        foreach (var x in expected)
        {
          var jo = JObject.Parse(x);
          Sort(jo);
          jole.Add(jo);
        }

        Assert.That(jola, Is.EqualTo(jole));
        return;
      }
      Assert.Fail();
    }
    #endregion array
    #endregion with in and out parameters
    #endregion non query

    #region execute scalar
    private static void TestScalarObject<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType, HttpMethod httpMethod = HttpMethod.GET)
    {
      var command = new Command(postgreSQLFunctionName);
      var parameter = new Parameter("p_parameter", pgsqlDbType, value);
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar, httpMethod);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var rv = jobject[command.Name]!["returnValue"];
      var actual = Convert.ChangeType(rv, typeof(T));
      var expected = (T)value;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetBigint()
    {
      TestScalarObject<long>("migration.get_bigint", 9223372036854775807, PgsqlDbType.Bigint);
      TestScalarObject<long>("migration.get_bigint", 9223372036854775807, PgsqlDbType.Bigint, HttpMethod.POST);
    }

    [Test]
    public void GetBoolean()
    {
      TestScalarObject<bool>("migration.get_boolean", false, PgsqlDbType.Boolean);
      TestScalarObject<bool>("migration.get_boolean", false, PgsqlDbType.Boolean, HttpMethod.POST);
    }

    // Please, note although PostgreSQL function "migration.get_bytea" returns bytea, the dawa returns bytes wrapped to base64 string
    [Test]
    public void GetBytea()
    {
      var command = new Command("migration.get_bytea");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Bytea);
      var expected = System.Text.Encoding.UTF8.GetBytes("hello world!");
      parameter.Value = expected;
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];
      var actual = Convert.FromBase64String(Convert.ToString(pp2v)!);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetDoublePrecision()
    {
      TestScalarObject<double>("migration.get_double_precision", 1234567890.12345, PgsqlDbType.Double);
      TestScalarObject<double>("migration.get_double_precision", 1234567890.12345, PgsqlDbType.Double, HttpMethod.POST);
    }

    [Test]
    public void GetInt()
    {
      TestScalarObject<int>("migration.get_int", 2147483647, PgsqlDbType.Integer);
      TestScalarObject<int>("migration.get_int", 2147483647, PgsqlDbType.Integer, HttpMethod.POST);
    }

    [Test]
    public void GetMoney()
    {
      TestScalarObject<decimal>("migration.get_money", 92233720368547758.07m, PgsqlDbType.Money);
      TestScalarObject<decimal>("migration.get_money", 92233720368547758.07m, PgsqlDbType.Money, HttpMethod.POST);
    }

    [Test]
    public void GetNumeric()
    {
      TestScalarObject<decimal>("migration.get_numeric", 123456789012345678.1234567890m, PgsqlDbType.Numeric);
      TestScalarObject<decimal>("migration.get_numeric", 123456789012345678.1234567890m, PgsqlDbType.Numeric, HttpMethod.POST);
    }

    [Test]
    public void GetReal()
    {
      TestScalarObject<float>("migration.get_real", 1234567890.12345f, PgsqlDbType.Real);
      TestScalarObject<float>("migration.get_real", 1234567890.12345f, PgsqlDbType.Real, HttpMethod.POST);
    }

    [Test]
    public void GetSmallint()
    {
      TestScalarObject<short>("migration.get_smallint", (short)32767, PgsqlDbType.Smallint);
      TestScalarObject<short>("migration.get_smallint", (short)32767, PgsqlDbType.Smallint, HttpMethod.POST);
    }

    [Test]
    public void GetText()
    {
      string expected = "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular & open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.";
      TestScalarObject<string>("migration.get_text", expected, PgsqlDbType.Text);
      TestScalarObject<string>("migration.get_text", expected, PgsqlDbType.Text, HttpMethod.POST);
    }

    [Test]
    public void GetDate()
    {
      DateTime expected = DateTime.Parse("2021.05.18");
      TestScalarObject<DateTime>("migration.get_date", expected, PgsqlDbType.Date);
      TestScalarObject<DateTime>("migration.get_date", expected, PgsqlDbType.Date, HttpMethod.POST);
    }

    [Test]
    public void GetTime()
    {
      TimeSpan expected = TimeSpan.Parse("13:44:46.9876");
      TestScalarObject<TimeSpan>("migration.get_time", expected, PgsqlDbType.Time);
      TestScalarObject<TimeSpan>("migration.get_time", expected, PgsqlDbType.Time, HttpMethod.POST);
    }

    [Test]
    public void GetTimetz()
    {
      var expected = DateTimeOffset.Parse("0001-01-02T14:41:45.1234+03");
      TestScalarObject<DateTimeOffset>("migration.get_timetz", expected, PgsqlDbType.TimeTZ);
      TestScalarObject<DateTimeOffset>("migration.get_timetz", expected, PgsqlDbType.TimeTZ, HttpMethod.POST);
    }

    [Test]
    public void GetTimestamp()
    {
      var expected = DateTime.Parse("2022.03.18 12:42:46.1234");
      TestScalarObject<DateTime>("migration.get_timestamp", expected, PgsqlDbType.Timestamp);
      TestScalarObject<DateTime>("migration.get_timestamp", expected, PgsqlDbType.Timestamp, HttpMethod.POST);
    }

    [Test]
    public void GetTimestamptz()
    {
      var expected = DateTime.Parse("2021.04.18 14:43:47.1234+02");
      TestScalarObject<DateTime>("migration.get_timestamptz", expected, PgsqlDbType.TimestampTZ);
      TestScalarObject<DateTime>("migration.get_timestamptz", expected, PgsqlDbType.TimestampTZ, HttpMethod.POST);
    }

    [Test]
    public void GetVarchar()
    {
      var expected = "PostgreSQL change column type examples";
      TestScalarObject<string>("migration.get_varchar", expected, PgsqlDbType.Varchar);
      TestScalarObject<string>("migration.get_varchar", expected, PgsqlDbType.Varchar, HttpMethod.POST);
    }

    [Test]
    public void GetUuid()
    {
      var expected = Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394");
      TestScalarObject<Guid>("migration.get_uuid", expected, PgsqlDbType.Uuid);
      TestScalarObject<Guid>("migration.get_uuid", expected, PgsqlDbType.Uuid, HttpMethod.POST);
    }

    [Test]
    public void GetXml()
    {
      var command = new Command("migration.get_xml");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Xml);
      var expected = @"<_routines>
      <_routine>
        <_name>formmanager_getfiltered</_name>
        <_arguments>
          <_formid>0</_formid>
          <_form></_form>
          <_businessids>1</_businessids>
          <_businessids>941</_businessids>
          <_businessids>942</_businessids>
          <_businessids>943</_businessids>
          <_businessids>944</_businessids>
          <_businessids>2006</_businessids>
          <_businessids>2129</_businessids>
          <_businessids>2135</_businessids>
          <_businessids>2137</_businessids>
          <_formtype>1</_formtype>
          <_formtype>2</_formtype>
          <_formtype>3</_formtype>
          <_formtype>4</_formtype>
          <_formtype>5</_formtype>
          <_formtype>6</_formtype>
          <_formtype>7</_formtype>
          <_formtype>8</_formtype>
          <_inactive>False</_inactive>
        </_arguments>
        <_options>
          <_writeSchema>1</_writeSchema>
        </_options>
      </_routine>
      <_compression>0</_compression>
      <_returnType>json</_returnType>
    </_routines>".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
      parameter.Value = expected;
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];
      var actual = Convert.ToString(pp2v)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetJson()
    {
      var command = new Command("migration.get_json");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Json);
      var expected = @"{
        ""formmanager_getfiltered"": []
    }".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
      parameter.Value = expected;
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];
      var actual = Convert.ToString(pp2v)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetJsonb()
    {
      var command = new Command("migration.get_jsonb");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Jsonb);
      var expected = @"{
	""id"": ""0001"",
	""type"": ""donut"",
	""name"": ""Cake"",
	""image"":
		{
			""url"": ""images/0001.jpg"",
			""width"": 200,
			""height"": 200
		},
	""thumbnail"":
		{
			""url"": ""images/thumbnails/0001.jpg"",
			""width"": 32,
			""height"": 32
		}
}".Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);

      parameter.Value = expected;
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];
      var actual = Convert.ToString(pp2v)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);

      var aJObject = JObject.Parse(actual);
      Sort(aJObject);

      var eJObject = JObject.Parse(expected);
      Sort(eJObject);

      Assert.That(aJObject, Is.EqualTo(eJObject));
    }
    #region array
    public static void TestScalarArrayObject<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType, HttpMethod httpMethod = HttpMethod.GET)
    {
      var command = new Command(postgreSQLFunctionName);
      var parameter = new Parameter("p_parameter", pgsqlDbType, value);
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar, httpMethod);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var rv = jobject[command.Name]!["returnValue"];

      if (rv is JArray ja)
      {
        var actual = ja.Select(x => Convert.ChangeType(x, typeof(T))).ToArray();
        var expected = value as T[];
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetBigintArray()
    {
      TestScalarArrayObject<long>("migration.get_bigint_array", new long[] { 9223372036854775701, 9223372036854775704, 9223372036854775703 }, PgsqlDbType.Bigint | PgsqlDbType.Array);
      TestScalarArrayObject<long>("migration.get_bigint_array", new long[] { 9223372036854775701, 9223372036854775704, 9223372036854775703 }, PgsqlDbType.Bigint | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetBooleanArray()
    {
      TestScalarArrayObject<bool>("migration.get_boolean_array", new bool[] { true, false, true }, PgsqlDbType.Boolean | PgsqlDbType.Array);
      TestScalarArrayObject<bool>("migration.get_boolean_array", new bool[] { true, false, true }, PgsqlDbType.Boolean | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetByteaArray()
    {
      var command = new Command("migration.get_bytea_array");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Bytea | PgsqlDbType.Array,
                                    new byte[][] { System.Text.Encoding.UTF8.GetBytes("hello"),
                                                   System.Text.Encoding.UTF8.GetBytes("world"),
                                                   System.Text.Encoding.UTF8.GetBytes("fropm test code!")});
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];

      if (pp2v is JArray ja)
      {
        var actual = ja.Select(x => Convert.FromBase64String(Convert.ToString(x)!)).ToArray();
        var expected = parameter.Value as byte[][];
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetDoublePrecisionArray()
    {
      TestScalarArrayObject<double>("migration.get_double_precision_array", new double[] { 1234567788.12242, 1234567787.67686, 1234567786.01375 },
        PgsqlDbType.Double | PgsqlDbType.Array);
      TestScalarArrayObject<double>("migration.get_double_precision_array", new double[] { 1234567788.12242, 1234567787.67686, 1234567786.01375 },
        PgsqlDbType.Double | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetIntArray()
    {
      TestScalarArrayObject<int>("migration.get_int_array", new int[] { 2147483544, 2147483543, 2147483542 },
        PgsqlDbType.Integer | PgsqlDbType.Array);
      TestScalarArrayObject<int>("migration.get_int_array", new int[] { 2147483544, 2147483543, 2147483542 },
        PgsqlDbType.Integer | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetMoneyArray()
    {
      TestScalarArrayObject<decimal>("migration.get_money_array", new decimal[] { 92233720368547555.03m, 92233720368547654.02m, 92233720368547653.01m },
        PgsqlDbType.Money | PgsqlDbType.Array);
      TestScalarArrayObject<decimal>("migration.get_money_array", new decimal[] { 92233720368547555.03m, 92233720368547654.02m, 92233720368547653.01m },
        PgsqlDbType.Money | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetNumericArray()
    {
      TestScalarArrayObject<decimal>("migration.get_numeric_array", new decimal[] { 123456789012345575.1234567787m,
                                                                               123456789012345574.1234567786m,
                                                                               123456789012345573.1234567785m },
                                     PgsqlDbType.Numeric | PgsqlDbType.Array);
      TestScalarArrayObject<decimal>("migration.get_numeric_array", new decimal[] { 123456789012345575.1234567787m,
                                                                               123456789012345574.1234567786m,
                                                                               123456789012345573.1234567785m },
                                     PgsqlDbType.Numeric | PgsqlDbType.Array,
                                     HttpMethod.POST);
    }

    [Test]
    public void GetRealArray()
    {
      TestScalarArrayObject<float>("migration.get_real_array", new float[] { 1234567787.12242f, 1234567785.12241f, 1234567785.12240f },
                                   PgsqlDbType.Real | PgsqlDbType.Array);
      TestScalarArrayObject<float>("migration.get_real_array", new float[] { 1234567787.12242f, 1234567785.12241f, 1234567785.12240f },
                                   PgsqlDbType.Real | PgsqlDbType.Array,
                                   HttpMethod.POST);
    }

    [Test]
    public void GetSmallintArray()
    {
      TestScalarArrayObject<short>("migration.get_smallint_array", new short[] { 32664, 32663, 32662 }, PgsqlDbType.Real | PgsqlDbType.Array);
      TestScalarArrayObject<short>("migration.get_smallint_array", new short[] { 32664, 32663, 32662 }, PgsqlDbType.Real | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetTextArray()
    {
      TestScalarArrayObject<string>("migration.get_text_array", new string[] { "A Boolean data type can hold one of three possible values: true, false or null. You use boolean or bool keyword to declare a column with the Boolean data type.",
                                                                          "PostgreSQL provides three character data types: CHAR(n), VARCHAR(n), and TEXT",
                                                                          "When you select data from a Boolean column, PostgreSQL converts the values back e.g., t to true, f to false and space to null." },
                                    PgsqlDbType.Text | PgsqlDbType.Array);
      TestScalarArrayObject<string>("migration.get_text_array", new string[] { "A Boolean data type can hold one of three possible values: true, false or null. You use boolean or bool keyword to declare a column with the Boolean data type.",
                                                                          "PostgreSQL provides three character data types: CHAR(n), VARCHAR(n), and TEXT",
                                                                          "When you select data from a Boolean column, PostgreSQL converts the values back e.g., t to true, f to false and space to null." },
                                    PgsqlDbType.Text | PgsqlDbType.Array,
                                    HttpMethod.POST);
    }

    [Test]
    public void GetDateArray()
    {
      TestScalarArrayObject<DateTime>("migration.get_date_array", new DateTime[] { DateTime.Parse("1919.01.15"),
                                                                              DateTime.Parse("1918.01.14"),
                                                                              DateTime.Parse("1917.12.13") },
                                      PgsqlDbType.Date | PgsqlDbType.Array);
      TestScalarArrayObject<DateTime>("migration.get_date_array", new DateTime[] { DateTime.Parse("1919.01.15"),
                                                                              DateTime.Parse("1918.01.14"),
                                                                              DateTime.Parse("1917.12.13") },
                                      PgsqlDbType.Date | PgsqlDbType.Array,
                                      HttpMethod.POST);
    }

    [Test]
    public void GetTimeArray()
    {
      TestScalarArrayObject<TimeSpan>("migration.get_time_array", new TimeSpan[] { TimeSpan.Parse("10:41:43.9873"),
                                                                              TimeSpan.Parse("08:41:42.9872"),
                                                                              TimeSpan.Parse("08:39:41.9871") },
                                      PgsqlDbType.Time | PgsqlDbType.Array);
      TestScalarArrayObject<TimeSpan>("migration.get_time_array", new TimeSpan[] { TimeSpan.Parse("10:41:43.9873"),
                                                                              TimeSpan.Parse("08:41:42.9872"),
                                                                              TimeSpan.Parse("08:39:41.9871") },
                                      PgsqlDbType.Time | PgsqlDbType.Array,
                                      HttpMethod.POST);
    }

    [Test]
    public void GetTimetzArray()
    {
      TestScalarArrayObject<DateTimeOffset>("migration.get_timetz_array", new DateTimeOffset[]
      {
            DateTimeOffset.Parse("0001-01-02T10:38:41.1231+02"),
            DateTimeOffset.Parse("0001-01-02T10:36:41.1230+04"),
            DateTimeOffset.Parse("0001-01-02T08:35:39.1229+02")
      },
      PgsqlDbType.TimeTZ | PgsqlDbType.Array);
      TestScalarArrayObject<DateTimeOffset>("migration.get_timetz_array", new DateTimeOffset[]
      {
            DateTimeOffset.Parse("0001-01-02T10:38:41.1231+02"),
            DateTimeOffset.Parse("0001-01-02T10:36:41.1230+04"),
            DateTimeOffset.Parse("0001-01-02T08:35:39.1229+02")
      },
      PgsqlDbType.TimeTZ | PgsqlDbType.Array,
      HttpMethod.POST);
    }

    [Test]
    public void GetTimetstampArray()
    {
      TestScalarArrayObject<DateTime>("migration.get_timestamp_array", new DateTime[] { DateTime.Parse("2019.12.15 09:39:41.1231"),
                                                                                   DateTime.Parse("2017.08.13 07:37:41.1229"),
                                                                                   DateTime.Parse("2016.06.11 06:36:40.1228") },
                                      PgsqlDbType.Timestamp | PgsqlDbType.Array);
      TestScalarArrayObject<DateTime>("migration.get_timestamp_array", new DateTime[] { DateTime.Parse("2019.12.15 09:39:41.1231"),
                                                                                   DateTime.Parse("2017.08.13 07:37:41.1229"),
                                                                                   DateTime.Parse("2016.06.11 06:36:40.1228") },
                                      PgsqlDbType.Timestamp | PgsqlDbType.Array,
                                      HttpMethod.POST);
    }

    [Test]
    public void GetTimetstamptzArray()
    {
      TestScalarArrayObject<DateTime>("migration.get_timestamptz_array", new DateTime[] { DateTime.Parse("2018.01.15 11:40:44.1231+03"),
                                                                                     DateTime.Parse("2015.08.11 08:37:41.1228+02"),
                                                                                     DateTime.Parse("2014.08.11 07:36:41.1227+03") },
                                      PgsqlDbType.TimestampTZ | PgsqlDbType.Array);
      TestScalarArrayObject<DateTime>("migration.get_timestamptz_array", new DateTime[] { DateTime.Parse("2018.01.15 11:40:44.1231+03"),
                                                                                     DateTime.Parse("2015.08.11 08:37:41.1228+02"),
                                                                                     DateTime.Parse("2014.08.11 07:36:41.1227+03") },
                                      PgsqlDbType.TimestampTZ | PgsqlDbType.Array,
                                      HttpMethod.POST);
    }

    [Test]
    public void GetVarcharArray()
    {
      TestScalarArrayObject<string>("migration.get_varchar_array", new string[]
      {
            "SMALLINT: This is a 2-byte signed integer, with a range of -32768 to +32767.",
            "INTEGER: This is a 4-byte signed integer, with a range of -2147483648 to +2147483647.",
            "BIGINT: This is an 8-byte signed integer, with a range of -9223372036854775808 to +9223372036854775807."
      },
      PgsqlDbType.Varchar | PgsqlDbType.Array);
      TestScalarArrayObject<string>("migration.get_varchar_array", new string[]
      {
            "SMALLINT: This is a 2-byte signed integer, with a range of -32768 to +32767.",
            "INTEGER: This is a 4-byte signed integer, with a range of -2147483648 to +2147483647.",
            "BIGINT: This is an 8-byte signed integer, with a range of -9223372036854775808 to +9223372036854775807."
      },
      PgsqlDbType.Varchar | PgsqlDbType.Array,
      HttpMethod.POST);
    }

    [Test]
    public void GetUuidArray()
    {
      TestScalarArrayObject<Guid>("migration.get_uuid_array", new Guid[]
      {
            Guid.Parse("0573ac3f-1014-4469-a409-50590be0072d"),
            Guid.Parse("7dfe6902-fec0-4173-a9c4-9fd0b471eed3"),
            Guid.Parse("a0553c33-29af-42fb-bbd6-d9668dc08478")
      },
      PgsqlDbType.Uuid | PgsqlDbType.Array);
      TestScalarArrayObject<Guid>("migration.get_uuid_array", new Guid[]
      {
            Guid.Parse("0573ac3f-1014-4469-a409-50590be0072d"),
            Guid.Parse("7dfe6902-fec0-4173-a9c4-9fd0b471eed3"),
            Guid.Parse("a0553c33-29af-42fb-bbd6-d9668dc08478")
      },
      PgsqlDbType.Uuid | PgsqlDbType.Array,
      HttpMethod.POST);
    }

    [Test]
    public void GetXmlArray()
    {
      var command = new Command("migration.get_xml_array");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Xml | PgsqlDbType.Array);
      var expected = new string[]
      {
            @"<CATALOG>
    <PLANT>
    <COMMON>Bloodroot</COMMON>
    <BOTANICAL>Sanguinaria canadensis</BOTANICAL>
    <ZONE>4</ZONE>
    <LIGHT>Mostly Shady</LIGHT>
    <PRICE>$2.44</PRICE>
    <AVAILABILITY>031599</AVAILABILITY>
    </PLANT>
    <PLANT>
    <COMMON>Columbine</COMMON>
    <BOTANICAL>Aquilegia canadensis</BOTANICAL>
    <ZONE>3</ZONE>
    <LIGHT>Mostly Shady</LIGHT>
    <PRICE>$9.37</PRICE>
    <AVAILABILITY>030699</AVAILABILITY>
    </PLANT></CATALOG>",
            @"<CATALOG>
    <CD>
    <TITLE>Empire Burlesque</TITLE>
    <ARTIST>Bob Dylan</ARTIST>
    <COUNTRY>USA</COUNTRY>
    <COMPANY>Columbia</COMPANY>
    <PRICE>10.90</PRICE>
    <YEAR>1985</YEAR>
    </CD>
    <CD>
    <TITLE>Hide your heart</TITLE>
    <ARTIST>Bonnie Tyler</ARTIST>
    <COUNTRY>UK</COUNTRY>
    <COMPANY>CBS Records</COMPANY>
    <PRICE>9.90</PRICE>
    <YEAR>1988</YEAR>
    </CD></CATALOG>",
            @"<breakfast_menu>
    <food>
    <name>Belgian Waffles</name>
    <price>$5.95</price>
    <description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
    <calories>650</calories>
    </food>
    <food>
    <name>Strawberry Belgian Waffles</name>
    <price>$7.95</price>
    <description>Light Belgian waffles covered with strawberries and whipped cream</description>
    <calories>900</calories>
    </food></breakfast_menu>"
      };
      parameter.Value = expected;
      command.Parameters.Add(parameter);

      var response = Command.Execute(command, RoutineType.Scalar, HttpMethod.POST);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];

      if (pp2v is JArray ja)
      {
        var actual = ja.Select(x => Convert.ToString(x)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty)).ToArray();
        expected = expected.Select(x => Convert.ToString(x)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty)).ToArray();
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetJsonArray()
    {
      var command = new Command("migration.get_json_array");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Json | PgsqlDbType.Array);
      var expected = new string[]
      {
            @"{
        ""fruit"": ""Apple"",
        ""size"": ""Large"",
        ""color"": ""Red""
    }",
            @"{""name"":""mkyong.com"",""messages"":[""msg 1"",""msg 2"",""msg 3""],""age"":100}",
            @"{
        ""name"": ""Morpheush"",
        ""job"": ""Leader"",
        ""id"": ""199"",
        ""createdAt"": ""2020-02-20T11:00:28.107Z""
    }"
      };
      parameter.Value = expected;
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar, HttpMethod.POST);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];

      if (pp2v is JArray ja)
      {
        var actual = ja.Select(x => Convert.ToString(x)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty)).ToArray();
        expected = expected.Select(x => Convert.ToString(x)!.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty)).ToArray();
        Assert.That(actual, Is.EqualTo(expected));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetJsonbArray()
    {
      var command = new Command("migration.get_json_array");
      var parameter = new Parameter("p_parameter", PgsqlDbType.Jsonb | PgsqlDbType.Array);
      var expected = new string[]
      {
            @"{
        ""fruit"": ""Apple"",
        ""size"": ""Large"",
        ""color"": ""Red""
    }",
            @"{""name"":""mkyong.com"",""messages"":[""msg 1"",""msg 2"",""msg 3""],""age"":100}",
            @"{
        ""name"": ""Morpheush"",
        ""job"": ""Leader"",
        ""id"": ""199"",
        ""createdAt"": ""2020-02-20T11:00:28.107Z""
    }"
      };
      parameter.Value = expected;
      command.Parameters.Add(parameter);
      var response = Command.Execute(command, RoutineType.Scalar, HttpMethod.POST);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var pp2v = jobject[command.Name]!["returnValue"];

      if (pp2v is JArray ja)
      {
        var actual = ja.ToObject<string[]>()!.Select(x => x.Replace("\r", string.Empty)
                                                           .Replace("\n", string.Empty)
                                                           .Replace("\t", string.Empty)
                                                           .Replace(" ", string.Empty));

        // we need to convert both json strings to JObject, and after that to order JObject by values,
        // because PostgreSQL keeps jsonb objects by their inner optimization, which violates the order of key - values of the json object

        var jola = new List<JObject>();
        foreach (var x in actual)
        {
          var jo = JObject.Parse(x);
          Sort(jo);
          jola.Add(jo);
        }


        expected = expected.Select(x => x.Replace("\r", string.Empty)
                                         .Replace("\n", string.Empty)
                                         .Replace("\t", string.Empty)
                                         .Replace(" ", string.Empty)).ToArray();

        var jole = new List<JObject>();
        foreach (var x in expected)
        {
          var jo = JObject.Parse(x);
          Sort(jo);
          jole.Add(jo);
        }

        Assert.That(jola, Is.EqualTo(jole));
        return;
      }
      Assert.Fail();
    }

    #endregion array
    #endregion execute scalar

    #region special cases
    [Test]
    public void GetBigintArray_EmptyBigintArray()
    {
      GetTArray_EmptyTArray<bool>("migration.get_bigint_array", new long[] { }, PgsqlDbType.Bigint | PgsqlDbType.Array);
      GetTArray_EmptyTArray<bool>("migration.get_bigint_array", new long[] { }, PgsqlDbType.Bigint | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetBooleanArray_EmptyBooleanArray()
    {
      GetTArray_EmptyTArray<bool>("migration.get_boolean_array", new bool[] { }, PgsqlDbType.Boolean | PgsqlDbType.Array);
      GetTArray_EmptyTArray<bool>("migration.get_boolean_array", new bool[] { }, PgsqlDbType.Boolean | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetByteaArray_EmptyByteaArray()
    {
      GetTArray_EmptyTArray<byte[]>("migration.get_bytea_array", new byte[][] { }, PgsqlDbType.Bytea | PgsqlDbType.Array);
      GetTArray_EmptyTArray<byte[]>("migration.get_bytea_array", new byte[][] { }, PgsqlDbType.Bytea | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetDoublePrecisionArray_EmptyDoublePrecisionArray()
    {
      GetTArray_EmptyTArray<double>("migration.get_double_precision_array", new double[] { }, PgsqlDbType.Double | PgsqlDbType.Array);
      GetTArray_EmptyTArray<double>("migration.get_double_precision_array", new double[] { }, PgsqlDbType.Double | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetIntArray_EmptyIntArray()
    {
      GetTArray_EmptyTArray<int>("migration.get_int_array", new int[] { }, PgsqlDbType.Integer | PgsqlDbType.Array);
      GetTArray_EmptyTArray<int>("migration.get_int_array", new int[] { }, PgsqlDbType.Integer | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetNumericArray_EmptyNumericArray()
    {
      GetTArray_EmptyTArray<decimal>("migration.get_numeric_array", new decimal[] { }, PgsqlDbType.Numeric | PgsqlDbType.Array);
      GetTArray_EmptyTArray<decimal>("migration.get_numeric_array", new decimal[] { }, PgsqlDbType.Numeric | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetMoneyArray_EmptyMoneyArray()
    {
      GetTArray_EmptyTArray<decimal>("migration.get_money_array", new decimal[] { }, PgsqlDbType.Money | PgsqlDbType.Array);
      GetTArray_EmptyTArray<decimal>("migration.get_money_array", new decimal[] { }, PgsqlDbType.Money | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetRealArray_EmptyRealArray()
    {
      GetTArray_EmptyTArray<float>("migration.get_real_array", new float[] { }, PgsqlDbType.Real | PgsqlDbType.Array);
      GetTArray_EmptyTArray<float>("migration.get_real_array", new float[] { }, PgsqlDbType.Real | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetSmallintArray_EmptySmallintArray()
    {
      GetTArray_EmptyTArray<short>("migration.get_smallint_array", new short[] { }, PgsqlDbType.Smallint | PgsqlDbType.Array);
      GetTArray_EmptyTArray<short>("migration.get_smallint_array", new short[] { }, PgsqlDbType.Smallint | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetTextArray_EmptyTextArray()
    {
      GetTArray_EmptyTArray<string>("migration.get_text_array", new string[] { }, PgsqlDbType.Text | PgsqlDbType.Array);
      GetTArray_EmptyTArray<string>("migration.get_text_array", new string[] { }, PgsqlDbType.Text | PgsqlDbType.Array, HttpMethod.POST);
    }

    [Test]
    public void GetDateArray_EmptyDateArray()
    {
      GetTArray_EmptyTArray<DateTime>("migration.get_date_array", new DateTime[] { }, PgsqlDbType.Date | PgsqlDbType.Array);
      GetTArray_EmptyTArray<DateTime>("migration.get_date_array", new DateTime[] { }, PgsqlDbType.Date | PgsqlDbType.Array, HttpMethod.POST);
    }

    public void GetTArray_EmptyTArray<T>(string postgreSQLFunctionName, object value, PgsqlDbType npgsqlDbType, HttpMethod httpMethod = HttpMethod.GET)
    {
      var command = new Command(postgreSQLFunctionName);
      var parameter = new Parameter("p_parameter", npgsqlDbType);
      parameter.Value = value;
      command.Parameters.Add(parameter);

      var response = Command.Execute(command, RoutineType.Scalar, httpMethod);
      var reader = new JsonTextReader(new StringReader(response));
      reader.FloatParseHandling = FloatParseHandling.Decimal;
      reader.DateParseHandling = DateParseHandling.None;
      var jobject = JObject.Load(reader);
      var rv = jobject[command.Name]!["returnValue"];

      // If an array is empty then SimpleWSA defines it as null, please, look at Request.cs, line 89.
      // It is not correct, but it is a reality now
      if (rv?.Type == JTokenType.Null)
      {
        Assert.Pass();
        return;
      }

      Assert.Fail();
    }
    #endregion special cases

    #region return set
    [Test]
    public void GetScalarDataTypes()
    {
      var command = new Command("migration.get_scalar_data_types");
      var expected = Command.Execute(command, RoutineType.DataSet);
      Assert.That(HashString(expected), Is.EqualTo("D3453D495766DBDCC9BB7B0BBEBA56373FEF7FDF3C9C172B0BF6DF411EB5E8BC"));
    }

    [Test]
    public void GetArrayDataTypes()
    {
      var command = new Command("migration.get_array_data_types");
      var expected = Command.Execute(command, RoutineType.DataSet);
      Assert.That(HashString(expected), Is.EqualTo("336FFFB92E5D2C97C764F8B366115E4B8810C07111C9DFE2C7BE20796E09E3F2"));
    }

    #region special cases

    // PostgreSQL function migration.get_empty_setof_by_refcursor() returns the refcursor by definition, 
    // but there the refcursor refers to nothing

    [Test]
    public void GetEmptySetofByRefcursor()
    {
      var command = new Command("migration.get_empty_setof_by_refcursor");
      var expected = Command.Execute(command, RoutineType.DataSet);
      var actual = "{\"migration.get_empty_setof_by_refcursor\":[]}";
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetEmptySetof()
    {
      var command = new Command("migration.get_empty_setof");
      var expected = Command.Execute(command, RoutineType.DataSet);
      var actual = "{\"migration.get_empty_setof\":[]}";
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ThrowRestServiceException()
    {
      var command = new Command("migration.get_out_bytea");
      Assert.Throws<RestServiceException>(() => Command.Execute(command, RoutineType.DataSet));
    }

    [Test]
    public void ThrowRestServiceException_Async()
    {
      var command = new Command("migration.get_out_bytea");
      Assert.ThrowsAsync<RestServiceException>(async () => await Command.ExecuteAsync(command, RoutineType.DataSet));
    }
    #endregion special cases
    #endregion return set

    // https://www.sean-lloyd.com/post/hash-a-string/
    internal static string HashString(string text, string salt = "")
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      // Uses SHA256 to create the hash
      using (var sha = new System.Security.Cryptography.SHA256Managed())
      {
        // Convert the string to a byte array first, to be processed
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
        byte[] hashBytes = sha.ComputeHash(textBytes);

        // Convert back to a string, removing the '-' that BitConverter adds
        string hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", string.Empty);

        return hash;
      }
    }
  }
}
