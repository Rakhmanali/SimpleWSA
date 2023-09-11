using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    [Test]
    public void GetOutBigint()
    {
      WSALibrary.Command command = new Command("migration.get_out_bigint");
      command.Parameters.Add("p_parameter", PgsqlDbType.Bigint);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_bigint\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":9223372036854775807}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_bigint"]!["arguments"]!["p_parameter"]!;
      long actual = Convert.ToInt64(p);
      long expected = 9223372036854775807;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutBoolean()
    {
      var command = new Command("migration.get_out_boolean");
      command.Parameters.Add("p_parameter", PgsqlDbType.Boolean);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_boolean\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":true}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_boolean"]!["arguments"]!["p_parameter"]!;
      bool actual = Convert.ToBoolean(p);
      bool expected = true;
      Assert.That(actual, Is.EqualTo(expected));
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
      var command = new Command("migration.get_out_double_precision");
      command.Parameters.Add("p_parameter", PgsqlDbType.Double);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_double_precision\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":1234567890.12345}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_double_precision"]!["arguments"]!["p_parameter"]!;
      double actual = Convert.ToDouble(p);
      double expected = 1234567890.12345;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutInt()
    {
      var command = new Command("migration.get_out_int");
      command.Parameters.Add("p_parameter", PgsqlDbType.Integer);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_int\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":2147483647}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_int"]!["arguments"]!["p_parameter"]!;
      int actual = Convert.ToInt32(p);
      int expected = 2147483647;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutMoney()
    {
      var command = new Command("migration.get_out_money");
      command.Parameters.Add("p_parameter", PgsqlDbType.Money);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_money\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":92233720368547758.07}}}
      var ppv = ExtractFieldFromJsonString(response, "p_parameter");

      var actual = Convert.ToDecimal(ppv);
      decimal expected = 92233720368547758.07m;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutNumeric()
    {
      var command = new Command("migration.get_out_numeric");
      command.Parameters.Add("p_parameter", PgsqlDbType.Numeric);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_numeric\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":123456789012345678.1234567890}}}
      var ppv = ExtractFieldFromJsonString(response, "p_parameter");

      decimal actual = Convert.ToDecimal(ppv);
      decimal expected = 123456789012345678.1234567890m;
      Assert.That(actual, Is.EqualTo(expected));
    }

    private static string ExtractFieldFromJsonString(string source, string fieldName)
    {
      int startIndex = source.IndexOf(fieldName) + fieldName.Length + 2;
      int length = source.Length - 3 - startIndex;
      return source.Substring(startIndex, length);
    }

    private static string ExtractArrayFieldFromJsonString(string source, string fieldName)
    {
      int startIndex = source.IndexOf(fieldName) + fieldName.Length + 3;
      int length = source.Length - 4 - startIndex;
      return source.Substring(startIndex, length);
    }

    [Test]
    public void GetOutReal()
    {
      var command = new Command("migration.get_out_real");
      command.Parameters.Add("p_parameter", PgsqlDbType.Real);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_real\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":1.234568E+09}}}
      var ppv = ExtractFieldFromJsonString(response, "p_parameter");

      var actual = Convert.ToSingle(ppv);
      float expected = 1.234568E+09f;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutSmallint()
    {
      var command = new Command("migration.get_out_smallint");
      command.Parameters.Add("p_parameter", PgsqlDbType.Smallint);
      var response = Command.Execute(command, RoutineType.NonQuery);

      // {\"migration.get_out_smallint\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":32767}}}
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_smallint"]!["arguments"]!["p_parameter"]!;
      short actual = Convert.ToInt16(p);
      short expected = 32767;
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutText()
    {
      var command = new Command("migration.get_out_text");
      command.Parameters.Add("p_parameter", PgsqlDbType.Text);

      // {\"migration.get_out_text\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_text"]!["arguments"]!["p_parameter"]!;
      string? actual = Convert.ToString(p);
      var expected = "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.";
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutDate()
    {
      var ñommand = new Command("migration.get_out_date");
      ñommand.Parameters.Add("p_parameter", PgsqlDbType.Date);

      // {\"migration.get_out_date\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"2021-05-18T00:00:00\"}}}
      var response = Command.Execute(ñommand, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_date"]!["arguments"]!["p_parameter"]!;
      DateTime actual = Convert.ToDateTime(p);
      DateTime expected = DateTime.Parse("2021-05-18T00:00:00");
      Assert.That(actual, Is.EqualTo(expected));
    }

    // TO DO: although the dawa returns TimeSpan npgsql 2.2.7 converts it to DateTime to consume. By taking in mind,
    // that PostgreSQL time is the time span during a day, selecting Ticks of the DateTime is the right decision
    [Test]
    public void GetOutTime()
    {
      var command = new Command("migration.get_out_time");
      command.Parameters.Add("p_parameter", PgsqlDbType.Time);

      // {\"migration.get_out_time\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"13:44:46.9876000\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_time"]!["arguments"]!["p_parameter"]!;
      var actual = TimeSpan.Parse(p.ToString());
      TimeSpan expected = TimeSpan.Parse("13:44:46.9876000");
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutTimetz()
    {
      var command = new Command("migration.get_out_timetz");
      command.Parameters.Add("p_parameter", PgsqlDbType.TimeTZ);

      // {\"migration.get_out_timetz\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"0001-01-02T14:41:45.1234+03:00\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractFieldFromJsonString(response, "p_parameter").Replace("\"", string.Empty);
      var actual = DateTimeOffset.Parse(ppv);

      DateTimeOffset expected = DateTimeOffset.Parse("0001-01-02T11:41:45.1234Z");
      Assert.That(actual.UtcDateTime, Is.EqualTo(expected.UtcDateTime));
    }

    [Test]
    public void GetOutTimestamp()
    {
      var command = new Command("migration.get_out_timestamp");
      command.Parameters.Add("p_parameter", PgsqlDbType.Timestamp);

      // {\"migration.get_out_timestamp\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"2022-03-18T12:42:46.1234\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_timestamp"]!["arguments"]!["p_parameter"]!;
      var actual = Convert.ToDateTime(p);
      var expected = DateTime.Parse("2022-03-18T12:42:46.1234");
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutTimestamptz()
    {
      var command = new Command("migration.get_out_timestamptz");
      command.Parameters.Add("p_parameter", PgsqlDbType.TimestampTZ);

      // {\"migration.get_out_timestamptz\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"2021-04-18T12:43:47.1234Z\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_timestamptz"]!["arguments"]!["p_parameter"]!;
      var actual = Convert.ToDateTime(p).ToLocalTime();
      var expected = DateTime.Parse("2021-04-18T12:43:47.1234Z");
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutVarchar()
    {
      var command = new Command("migration.get_out_varchar");
      command.Parameters.Add("p_parameter", PgsqlDbType.Varchar);

      // {\"migration.get_out_varchar\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"PostgreSQL change column type examples\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_varchar"]!["arguments"]!["p_parameter"]!;
      var actual = Convert.ToString(p);
      var expected = "PostgreSQL change column type examples";
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutUuid()
    {
      var command = new Command("migration.get_out_uuid");
      command.Parameters.Add("p_parameter", PgsqlDbType.Uuid);

      // {\"migration.get_out_uuid\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":\"79130b53-3113-41d1-99ec-26e41b238394\"}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_uuid"]!["arguments"]!["p_parameter"]!;
      var actual = Guid.Parse(Convert.ToString(p));
      var expected = Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394");
      Assert.That(actual, Is.EqualTo(expected));
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

    [Test]
    public void GetOutBigintArray()
    {
      var command = new Command("migration.get_out_bigint_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Bigint | PgsqlDbType.Array);

      // {\"migration.get_out_bigint_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[9223372036854775807,9223372036854775806,9223372036854775805]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_bigint_array"]!["arguments"]!["p_parameter"]!;

      if (p is JArray ja)
      {
        var actual = new List<long>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToInt64(item));
        }
        Assert.That(actual, Is.EqualTo(new long[3] { 9223372036854775807, 9223372036854775806, 9223372036854775805 }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutBooleanArray()
    {
      var command = new Command("migration.get_out_boolean_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Boolean | PgsqlDbType.Array);

      // {\"migration.get_out_boolean_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[true,false,true]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_boolean_array"]!["arguments"]!["p_parameter"]!;

      if (p is JArray ja)
      {
        var actual = new List<bool>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToBoolean(item));
        }
        Assert.That(actual, Is.EqualTo(new bool[3] { true, false, true }));
        return;
      }

      Assert.Fail();
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
      var command = new Command("migration.get_out_double_precision_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Double | PgsqlDbType.Array);

      // {\"migration.get_out_double_precision_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[1234567890.12345,1234567889.6789,1234567888.01478]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_double_precision_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<double>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToDouble(item));
        }
        Assert.That(actual, Is.EqualTo(new double[3] { 1234567890.12345, 1234567889.6789, 1234567888.01478 }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutIntArray()
    {
      var command = new Command("migration.get_out_int_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Integer | PgsqlDbType.Array);

      // {\"migration.get_out_int_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[2147483647,2147483646,2147483645]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_int_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<int>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToInt32(item));
        }
        Assert.That(actual, Is.EqualTo(new int[3] { 2147483647, 2147483646, 2147483645 }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutMoneyArray()
    {
      var command = new Command("migration.get_out_money_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Money | PgsqlDbType.Array);

      // {\"migration.get_out_money_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[92233720368547758.07,92233720368547757.05,92233720368547756.06]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractArrayFieldFromJsonString(response, "p_parameter");
      var actual = ppv.Split(',').Select(v => Convert.ToDecimal(v)).ToList();
      Assert.That(actual, Is.EqualTo(new decimal[3] { 92233720368547758.07m, 92233720368547757.05m, 92233720368547756.06m }));
    }

    [Test]
    public void GetOutNumericArray()
    {
      var command = new Command("migration.get_out_numeric_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Numeric | PgsqlDbType.Array);

      // {\"migration.get_out_numeric_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[123456789012345678.1234567890,123456789012345677.1234567889,123456789012345676.1234567888]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractArrayFieldFromJsonString(response, "p_parameter");
      var actual = ppv.Split(',').Select(v => Convert.ToDecimal(v)).ToList();
      Assert.That(actual, Is.EqualTo(new decimal[3] { 123456789012345678.1234567890m, 123456789012345677.1234567889m, 123456789012345676.1234567888m }));
    }

    [Test]
    public void GetOutRealArray()
    {
      var command = new Command("migration.get_out_real_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Real | PgsqlDbType.Array);

      // {\"migration.get_out_real_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[1.234568E+09,1.234568E+09,1.234568E+09]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractArrayFieldFromJsonString(response, "p_parameter");
      var actual = ppv.Split(',').Select(v => Convert.ToSingle(v)).ToList();
      Assert.That(actual, Is.EqualTo(new float[3] { 1.234568E+09f, 1.234568E+09f, 1.234568E+09f }));
    }

    [Test]
    public void GetOutSmallintArray()
    {
      var command = new Command("migration.get_out_smallint_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Smallint | PgsqlDbType.Array);

      // {\"migration.get_out_smallint_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[32767,32766,32765]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_smallint_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<short>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToInt16(item));
        }
        Assert.That(actual, Is.EqualTo(new short[3] { 32767, 32766, 32765 }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutTextArray()
    {
      var command = new Command("migration.get_out_text_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Text | PgsqlDbType.Array);

      // {\"migration.get_out_text_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.\",\"DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.\",\"DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date.\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_text_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<string?>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToString(item));
        }
        Assert.That(actual, Is.EqualTo(new string[3] { "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
                                                       "DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.",
                                                       "DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date."
        }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutDateArray()
    {
      var command = new Command("migration.get_out_date_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Date | PgsqlDbType.Array);

      // {\"migration.get_out_date_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"2021-05-18T00:00:00\",\"2020-04-17T00:00:00\",\"2019-03-16T00:00:00\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_date_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<DateTime>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToDateTime(item));
        }
        Assert.That(actual, Is.EqualTo(new DateTime[3] { DateTime.Parse("2021-05-18T00:00:00"),
                                                         DateTime.Parse("2020-04-17T00:00:00"),
                                                         DateTime.Parse("2019-03-16T00:00:00")
              }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutTimeArray()
    {
      var command = new Command("migration.get_out_time_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Time | PgsqlDbType.Array);

      // {\"migration.get_out_time_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"13:44:46.9876000\",\"11:43:45.9875000\",\"11:42:44.9874000\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_time_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<TimeSpan>();
        foreach (var item in ja)
        {
          actual.Add(TimeSpan.Parse(Convert.ToString(item)!));
        }
        Assert.That(actual, Is.EqualTo(new TimeSpan[3] { TimeSpan.Parse("13:44:46.9876000"),
                                                         TimeSpan.Parse("11:43:45.9875000"),
                                                         TimeSpan.Parse("11:42:44.9874000")
              }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutTimetzArray()
    {
      var command = new Command("migration.get_out_timetz_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.TimeTZ | PgsqlDbType.Array);

      // {\"migration.get_out_timetz_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"0001-01-02T14:41:45.1234+03:00\",\"0001-01-02T13:39:44.1233+02:00\",\"0001-01-02T11:38:42.1232+01:00\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractArrayFieldFromJsonString(response, "p_parameter");
      var actual = ppv.Replace("\"", string.Empty).Split(',').Select(x => DateTimeOffset.Parse(x)).ToArray();
      Assert.That(actual, Is.EqualTo(new DateTimeOffset[3] { DateTimeOffset.Parse("0001-01-02T14:41:45.1234+03:00"),
                                                             DateTimeOffset.Parse("0001-01-02T13:39:44.1233+02:00"),
                                                             DateTimeOffset.Parse("0001-01-02T11:38:42.1232+01:00")
      }));
    }


    [Test]
    public void GetOutTimestampArray()
    {
      var command = new Command("migration.get_out_timestamp_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Timestamp | PgsqlDbType.Array);

      // {\"migration.get_out_timestamp_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"2022-03-18T12:42:46.1234\",\"2020-01-16T10:40:44.1232\",\"2019-09-15T09:39:43.1231\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractArrayFieldFromJsonString(response, "p_parameter");
      var actual = ppv.Replace("\"", string.Empty).Split(',').Select(x => DateTime.Parse(x)).ToArray();
      Assert.That(actual, Is.EqualTo(new DateTime[3] { DateTime.Parse("2022-03-18T12:42:46.1234"),
                                                       DateTime.Parse("2020-01-16T10:40:44.1232"),
                                                       DateTime.Parse("2019-09-15T09:39:43.1231")
      }));
    }

    [Test]
    public void GetOutTimestamptzArray()
    {
      var command = new Command("migration.get_out_timestamptz_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.TimestampTZ | PgsqlDbType.Array);

      // {\"migration.get_out_timestamptz_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"2021-04-18T12:43:47.1234Z\",\"2018-01-15T10:40:44.1231Z\",\"2017-01-14T07:39:44.123Z\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      var ppv = ExtractArrayFieldFromJsonString(response, "p_parameter");
      var actual = ppv.Replace("\"", string.Empty).Split(',').Select(x => DateTime.Parse(x)).ToArray();
      Assert.That(actual, Is.EqualTo(new DateTime[3] { DateTime.Parse("2021-04-18T12:43:47.1234Z"),
                                                       DateTime.Parse("2018-01-15T10:40:44.1231Z"),
                                                       DateTime.Parse("2017-01-14T07:39:44.123Z")
      }));
    }

    [Test]
    public void GetOutVarcharArray()
    {
      var command = new Command("migration.get_out_varchar_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Varchar | PgsqlDbType.Array);

      // {\"migration.get_out_varchar_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"PostgreSQL change column type examples\",\"What is the PostgreSQL Function?\",\"PostgreSQL change column type examples\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_varchar_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<string>();
        foreach (var item in ja)
        {
          actual.Add(Convert.ToString(item)!);
        }
        Assert.That(actual, Is.EqualTo(new string[3] { "PostgreSQL change column type examples",
                                                       "What is the PostgreSQL Function?",
                                                       "PostgreSQL change column type examples"
              }));
        return;
      }

      Assert.Fail();
    }

    [Test]
    public void GetOutUuidArray()
    {
      var command = new Command("migration.get_out_uuid_array");
      command.Parameters.Add("p_parameter", PgsqlDbType.Uuid | PgsqlDbType.Array);

      // {\"migration.get_out_uuid_array\":{\"returnValue\":-1,\"arguments\":{\"p_parameter\":[\"79130b53-3113-41d1-99ec-26e41b238394\",\"f0c180ba-e291-4089-91b4-3d8d122b5c77\",\"670c4c79-521c-40e2-8442-0248a93f8737\"]}}}
      var response = Command.Execute(command, RoutineType.NonQuery);
      JObject jobject = JObject.Parse(response);
      object p = jobject["migration.get_out_uuid_array"]!["arguments"]!["p_parameter"]!;
      if (p is JArray ja)
      {
        var actual = new List<Guid>();
        foreach (var item in ja)
        {
          actual.Add(Guid.Parse(Convert.ToString(item)!));
        }
        Assert.That(actual, Is.EqualTo(new Guid[3] { Guid.Parse("79130b53-3113-41d1-99ec-26e41b238394"),
                                                     Guid.Parse("f0c180ba-e291-4089-91b4-3d8d122b5c77"),
                                                     Guid.Parse("670c4c79-521c-40e2-8442-0248a93f8737")
        }));
        return;
      }

      Assert.Fail();
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
                           .Select(x => x.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty)).ToArray();
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

    public void GetInOutTest<T>(string postgreSQLFunctionName, object value, PgsqlDbType pgsqlDbType)
    {
      var command = new Command(postgreSQLFunctionName);
      command.Parameters.Add("p_parameter1", pgsqlDbType, value);
      command.Parameters.Add("p_parameter2", pgsqlDbType);
      var response = Command.Execute(command, RoutineType.NonQuery);
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
      GetInOutTest<long>("migration.get_in_and_out_bigint", 9223372036854775807, PgsqlDbType.Bigint);
    }

    [Test]
    public void GetInOutBoolean()
    {
      GetInOutTest<bool>("migration.get_in_and_out_boolean", true, PgsqlDbType.Boolean);
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
      GetInOutTest<double>("migration.get_in_and_out_double_precision", 1234567888.01478, PgsqlDbType.Double);
    }

    [Test]
    public void GetInOutInteger()
    {
      GetInOutTest<int>("migration.get_in_and_out_integer", 2147483645, PgsqlDbType.Integer);
    }

    [Test]
    public void GetInOutMoney()
    {
      GetInOutTest<decimal>("migration.get_in_and_out_money", 92233720368547756.06m, PgsqlDbType.Money);
    }

    [Test]
    public void GetInOutNumeric()
    {
      GetInOutTest<decimal>("migration.get_in_and_out_numeric", 123456789012345676.1234567888m, PgsqlDbType.Numeric);
    }

    [Test]
    public void GetInOutReal()
    {
      GetInOutTest<float>("migration.get_in_and_out_real", 1234567888.12343f, PgsqlDbType.Real);
    }

    [Test]
    public void GetInOutSmallint()
    {
      GetInOutTest<short>("migration.get_in_and_out_smallint", (short)32765, PgsqlDbType.Smallint);
    }

    [Test]
    public void GetInOutText()
    {
      GetInOutTest<string>("migration.get_in_and_out_text", "Hello world from Text testing", PgsqlDbType.Text);
    }

    [Test]
    public void GetInOutDate()
    {
      GetInOutTest<DateTime>("migration.get_in_and_out_date", DateTime.Parse("2019.03.16"), PgsqlDbType.Date);
    }

    [Test]
    public void GetInOutTime()
    {
      GetInOutTest<TimeSpan>("migration.get_in_and_out_time", TimeSpan.Parse("10:10:12.123"), PgsqlDbType.Time);
    }

    [Test]
    public void GetInOutTimetz()
    {
      GetInOutTest<DateTimeOffset>("migration.get_in_and_out_timetz", DateTimeOffset.Parse("0001-01-02T10:10:12.256+03"), PgsqlDbType.TimeTZ);
    }

    [Test]
    public void GetInOutTimestamp()
    {
      GetInOutTest<DateTime>("migration.get_in_and_out_timestamp", DateTime.Parse("2023-05-23 10:10:12.256"), PgsqlDbType.Timestamp);
    }

    [Test]
    public void GetInOutTimestamptz()
    {
      GetInOutTest<DateTime>("migration.get_in_and_out_timestamptz", DateTime.Parse("2023-05-23 10:10:12.256+05"), PgsqlDbType.TimestampTZ);
    }

    [Test]
    public void GetInOutVachar()
    {
      GetInOutTest<string>("migration.get_in_and_out_varchar", "hello world!", PgsqlDbType.Varchar);
    }

    [Test]
    public void GetInOutUuid()
    {
      GetInOutTest<string>("migration.get_in_and_out_uuid", "79130b53-3113-41d1-99ec-26e41b238394", PgsqlDbType.Uuid);
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

    //    #region array

    //    [Test]
    //    public void GetInOutBigintArray()
    //    {
    //      GetInOutArrayTest<long>("migration.get_in_and_out_bigint_array", new long[] { 9223372036854775701, 9223372036854775704, 9223372036854775703 }, NpgsqlDbType.Bigint | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutBooleanArray()
    //    {
    //      GetInOutArrayTest<bool>("migration.get_in_and_out_boolean_array", new bool[] { true, false, true }, NpgsqlDbType.Boolean | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutByteaArray()
    //    {
    //      using var npgsqlCommand = new NpgsqlCommand("migration.get_in_and_out_bytea_array");
    //      var npgsqlParameter1 = new NpgsqlParameter("p_parameter1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
    //      byte[][] expected = new byte[][] { System.Text.Encoding.UTF8.GetBytes("hello world!"),
    //                                              System.Text.Encoding.UTF8.GetBytes("There are three methods used to adjust a DateOnly structure: AddDays, AddMonths, and AddYears. Each method takes an integer parameter, and increases the date by that measurement."),
    //                                              System.Text.Encoding.UTF8.GetBytes("If a negative number is provided, the date is decreased by that measurement. The methods return a new instance of DateOnly, as the structure is immutable.")
    //      };
    //      npgsqlParameter1.Value = expected;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter1);

    //      var npgsqlParameter2 = new NpgsqlParameter("p_parameter2", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
    //      npgsqlParameter2.Direction = ParameterDirection.Output;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter2);

    //      npgsqlCommand.WExecuteNonQuery(returnCompressionType: CompressionType.GZip);

    //      var value2 = npgsqlCommand.Parameters["p_parameter2"].Value;
    //      if (value2 != null && value2.GetType().IsArray)
    //      {
    //        var objects2 = value2 as object[];
    //        if (objects2 != null)
    //        {
    //          var result = objects2.Select(x => Convert.ToString(x))
    //                               .Select(x => Convert.FromBase64String(x))
    //                               .ToArray();
    //          Assert.That(result, Is.EqualTo(expected));
    //          return;
    //        }
    //      }

    //      Assert.Fail();
    //    }

    //    [Test]
    //    public void GetInOutDoublePrecisionArray()
    //    {
    //      GetInOutArrayTest<double>("migration.get_in_and_out_double_precision_array", new double[] { 1234567890.12345, 1234567889.67891, 1234567888.01478 }, NpgsqlDbType.Double | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutIntegerArray()
    //    {
    //      GetInOutArrayTest<int>("migration.get_in_and_out_integer_array", new int[] { 2147483647, 2147483646, 2147483645 }, NpgsqlDbType.Integer | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutMoneyArray()
    //    {
    //      GetInOutArrayTest<decimal>("migration.get_in_and_out_money_array", new decimal[] { 92233720368547758.07m, 92233720368547757.05m, 92233720368547756.06m }, NpgsqlDbType.Money | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutNumericArray()
    //    {
    //      GetInOutArrayTest<decimal>("migration.get_in_and_out_numeric_array", new decimal[] { 123456789012345678.1234567890m, 123456789012345677.1234567889m, 123456789012345676.1234567888m }, NpgsqlDbType.Numeric | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutRealArray()
    //    {
    //      GetInOutArrayTest<float>("migration.get_in_and_out_real_array", new float[] { 123456789012345678.1234567890f, 123456789012345677.1234567889f, 123456789012345676.1234567888f }, NpgsqlDbType.Real | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutSmallintArray()
    //    {
    //      GetInOutArrayTest<short>("migration.get_in_and_out_smallint_array", new short[] { 32767, 32766, 32765 }, NpgsqlDbType.Smallint | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutTextArray()
    //    {
    //      GetInOutArrayTest<string>("migration.get_in_and_out_text_array", new string[] { "PostgreSQL is like a Swiss Army Knife for data storage – it’s a popular open-source relational database management system (RDBMS) that can handle just about anything you throw at it. But with great power comes great responsibility, and in this case, that responsibility is choosing the right data type.",
    //                                                                                      "DateOnly can be parsed from a string, just like the DateTime structure. All of the standard .NET date-based parsing tokens work with DateOnly.",
    //                                                                                      "DateOnly can be compared with other instances. For example, you can check if a date is before or after another, or if a date today matches a specific date." },
    //                                NpgsqlDbType.Text | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutDateArray()
    //    {
    //      GetInOutArrayTest<DateTime>("migration.get_in_and_out_date_array", new DateTime[] { DateTime.Parse("2021.05.18"),
    //                                                                                          DateTime.Parse("2020.04.17"),
    //                                                                                          DateTime.Parse("2020.04.17") },
    //                                NpgsqlDbType.Date | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutTimeArray()
    //    {
    //      GetInOutArrayTest<TimeSpan>("migration.get_in_and_out_time_array", new TimeSpan[] { TimeSpan.Parse("13:44:46.9876"),
    //                                                                                          TimeSpan.Parse("11:43:45.9875"),
    //                                                                                          TimeSpan.Parse("11:42:44.9874") },
    //                                NpgsqlDbType.Time | NpgsqlDbType.Array);
    //    }

    //    // TO DO: The strange behavior of npgsql 2.2.7:
    //    //        In this case, when the npgsqlParameter1 is NpgsqlDbType.TimeTZ array, this parameter can accept
    //    //        DateTimeOffset array as value, but for scalar it is not possible, please see GetInOutTimetz()
    //    [Test]
    //    public void GetInOutTimetzArray()
    //    {
    //      using var npgsqlCommand = new NpgsqlCommand("migration.get_in_and_out_timetz_array");
    //      var npgsqlParameter1 = new NpgsqlParameter("p_parameter1", NpgsqlDbType.TimeTZ | NpgsqlDbType.Array);
    //      npgsqlParameter1.Value = new DateTimeOffset[] { DateTimeOffset.Parse("001-01-01T14:41:45.1234+03"),
    //                                                      DateTimeOffset.Parse("001-01-01T13:39:44.1233+02"),
    //                                                      DateTimeOffset.Parse("001-01-01T11:38:42.1232+01")};

    //      npgsqlCommand.Parameters.Add(npgsqlParameter1);

    //      var npgsqlParameter2 = new NpgsqlParameter("p_parameter2", NpgsqlDbType.TimeTZ | NpgsqlDbType.Array);
    //      npgsqlParameter2.Direction = ParameterDirection.Output;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter2);

    //      npgsqlCommand.WExecuteNonQuery(returnCompressionType: CompressionType.GZip);

    //      var value2 = npgsqlCommand.Parameters["p_parameter2"].Value;
    //      if (value2 != null && value2.GetType().IsArray)
    //      {
    //        var objects2 = value2 as object[];
    //        if (objects2 != null)
    //        {
    //          var result = objects2.Select(x => (TimeSpan)x).ToArray();
    //          var expected = (npgsqlCommand.Parameters["p_parameter1"].Value as DateTimeOffset[])!
    //                         .Select(x => new TimeSpan(x.UtcTicks)).ToArray();
    //          Assert.That(result, Is.EqualTo(expected));
    //          return;
    //        }
    //      }

    //      Assert.Fail();
    //    }

    //    [Test]
    //    public void GetInOutTimestampArray()
    //    {
    //      GetInOutArrayTest<DateTime>("migration.get_in_and_out_timestamp_array", new DateTime[] { DateTime.Parse("2022.03.18 12:42:46.1234"),
    //                                                                                               DateTime.Parse("2020.01.16 10:40:44.1232"),
    //                                                                                               DateTime.Parse("2019.09.15 09:39:43.1231") },
    //                                  NpgsqlDbType.Timestamp | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutTimestamptzArray()
    //    {
    //      GetInOutArrayTest<DateTime>("migration.get_in_and_out_timestamptz_array", new DateTime[] { DateTime.Parse("2021.04.18 14:43:47.1234+02"),
    //                                                                                               DateTime.Parse("2018.01.15 11:40:44.1231+01"),
    //                                                                                               DateTime.Parse("2017.01.14 10:39:44.1230+03") },
    //                                  NpgsqlDbType.TimestampTZ | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutVarcharArray()
    //    {
    //      GetInOutArrayTest<string>("migration.get_in_and_out_varchar_array", new string[] { "PostgreSQL change column type examples",
    //                                                                                         "What is the PostgreSQL Function?",
    //                                                                                         "PostgreSQL change column type examples" },
    //                                NpgsqlDbType.Varchar | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutUuidArray()
    //    {
    //      GetInOutArrayTest<string>("migration.get_in_and_out_uuid_array", new string[] { "79130b53-3113-41d1-99ec-26e41b238394",
    //                                                                                      "f0c180ba-e291-4089-91b4-3d8d122b5c77",
    //                                                                                      "670c4c79-521c-40e2-8442-0248a93f8737" },
    //                                NpgsqlDbType.Uuid | NpgsqlDbType.Array);
    //    }

    //    [Test]
    //    public void GetInOutXmlArray()
    //    {
    //      using var npgsqlCommand = new NpgsqlCommand("migration.get_in_and_out_xml_array");
    //      var npgsqlParameter1 = new NpgsqlParameter("p_parameter1", NpgsqlDbType.Xml | NpgsqlDbType.Array);
    //      npgsqlParameter1.Value = new string[] { @"<CATALOG>
    //  <PLANT>
    //    <COMMON>Bloodroot</COMMON>
    //    <BOTANICAL>Sanguinaria canadensis</BOTANICAL>
    //    <ZONE>4</ZONE>
    //    <LIGHT>Mostly Shady</LIGHT>
    //    <PRICE>$2.44</PRICE>
    //    <AVAILABILITY>031599</AVAILABILITY>
    //  </PLANT>
    //  <PLANT>
    //    <COMMON>Columbine</COMMON>
    //    <BOTANICAL>Aquilegia canadensis</BOTANICAL>
    //    <ZONE>3</ZONE>
    //    <LIGHT>Mostly Shady</LIGHT>
    //    <PRICE>$9.37</PRICE>
    //    <AVAILABILITY>030699</AVAILABILITY>
    //  </PLANT>
    //</CATALOG>",
    //                                              @"<CATALOG>
    //<CD>
    //<TITLE>Empire Burlesque</TITLE>
    //<ARTIST>Bob Dylan</ARTIST>
    //<COUNTRY>USA</COUNTRY>
    //<COMPANY>Columbia</COMPANY>
    //<PRICE>10.90</PRICE>
    //<YEAR>1985</YEAR>
    //</CD>
    //<CD>
    //<TITLE>Hide your heart</TITLE>
    //<ARTIST>Bonnie Tyler</ARTIST>
    //<COUNTRY>UK</COUNTRY>
    //<COMPANY>CBS Records</COMPANY>
    //<PRICE>9.90</PRICE>
    //<YEAR>1988</YEAR>
    //</CD></CATALOG>",
    //                                              @"<breakfast_menu>
    //  <food>
    //    <name>Belgian Waffles</name>
    //    <price>$5.95</price>
    //    <description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
    //    <calories>650</calories>
    //  </food>
    //  <food>
    //    <name>Strawberry Belgian Waffles</name>
    //    <price>$7.95</price>
    //    <description>Light Belgian waffles covered with strawberries and whipped cream</description>
    //    <calories>900</calories>
    //  </food>
    //</breakfast_menu>"};

    //      npgsqlCommand.Parameters.Add(npgsqlParameter1);

    //      var npgsqlParameter2 = new NpgsqlParameter("p_parameter2", NpgsqlDbType.Xml | NpgsqlDbType.Array);
    //      npgsqlParameter2.Direction = ParameterDirection.Output;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter2);

    //      npgsqlCommand.WExecuteNonQuery(returnCompressionType: CompressionType.GZip);

    //      var value2 = npgsqlCommand.Parameters["p_parameter2"].Value;
    //      if (value2 != null && value2.GetType().IsArray)
    //      {
    //        var objects2 = value2 as object[];
    //        if (objects2 != null)
    //        {
    //          var result = objects2.Select(x => Convert.ToString(x)!
    //                               .Replace("\r", string.Empty)
    //                               .Replace("\n", string.Empty)
    //                               .Replace("\t", string.Empty)
    //                               .Replace(" ", string.Empty))
    //            .ToArray();
    //          var expected = (npgsqlCommand.Parameters["p_parameter1"].Value as string[])!
    //                         .Select(x => Convert.ToString(x)
    //                         .Replace("\r", string.Empty)
    //                         .Replace("\n", string.Empty)
    //                         .Replace("\t", string.Empty)
    //                         .Replace(" ", string.Empty))
    //                         .ToArray();
    //          Assert.That(result, Is.EqualTo(expected));
    //          return;
    //        }
    //      }

    //      Assert.Fail();
    //    }

    //    [Test]
    //    public void GetInOutJsonArray()
    //    {
    //      using var npgsqlCommand = new NpgsqlCommand("migration.get_in_and_out_json_array");
    //      var npgsqlParameter1 = new NpgsqlParameter("p_parameter1", NpgsqlDbType.Json | NpgsqlDbType.Array);
    //      npgsqlParameter1.Value = new string[] { @"{
    //  ""fruit"": ""Apple"",
    //  ""size"":  ""Large"",
    //  ""color"": ""Red""
    //}",
    //                                              @"{""name"":""mkyong.com"",""messages"":[""msg 1"",""msg 2"",""msg 3""],""age"":100}",
    //                                              @"{
    //    ""name"": ""Morpheush"",
    //    ""job"": ""Leader"",
    //    ""id"": ""199"",
    //    ""createdAt"": ""2020-02-20T11:00:28.107Z""
    //}"};

    //      npgsqlCommand.Parameters.Add(npgsqlParameter1);

    //      var npgsqlParameter2 = new NpgsqlParameter("p_parameter2", NpgsqlDbType.Json | NpgsqlDbType.Array);
    //      npgsqlParameter2.Direction = ParameterDirection.Output;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter2);

    //      npgsqlCommand.WExecuteNonQuery(returnCompressionType: CompressionType.GZip);

    //      var value2 = npgsqlCommand.Parameters["p_parameter2"].Value;
    //      if (value2 != null && value2.GetType().IsArray)
    //      {
    //        var objects2 = value2 as object[];
    //        if (objects2 != null)
    //        {
    //          var result = objects2.Select(x => Convert.ToString(x)!
    //                               .Replace("\r", string.Empty)
    //                               .Replace("\n", string.Empty)
    //                               .Replace("\t", string.Empty)
    //                               .Replace(" ", string.Empty))
    //            .ToArray();
    //          var expected = (npgsqlCommand.Parameters["p_parameter1"].Value as string[])!
    //                         .Select(x => Convert.ToString(x)
    //                         .Replace("\r", string.Empty)
    //                         .Replace("\n", string.Empty)
    //                         .Replace("\t", string.Empty)
    //                         .Replace(" ", string.Empty))
    //                         .ToArray();
    //          Assert.That(result, Is.EqualTo(expected));
    //          return;
    //        }
    //      }

    //      Assert.Fail();
    //    }

    //    [Test]
    //    public void GetInOutJsonbArray()
    //    {
    //      using var npgsqlCommand = new NpgsqlCommand("migration.get_in_and_out_jsonb_array");
    //      var npgsqlParameter1 = new NpgsqlParameter("p_parameter1", NpgsqlDbType.Jsonb | NpgsqlDbType.Array);
    //      npgsqlParameter1.Value = new string[] { @"{
    //  ""fruit"": ""Apple"",
    //  ""size"":  ""Large"",
    //  ""color"": ""Red""
    //}",
    //                                              @"{""name"":""mkyong.com"",""messages"":[""msg 1"",""msg 2"",""msg 3""],""age"":100}",
    //                                              @"{
    //    ""name"": ""Morpheush"",
    //    ""job"": ""Leader"",
    //    ""id"": ""199"",
    //    ""createdAt"": ""2020-02-20T11:00:28.107Z""
    //}"};

    //      npgsqlCommand.Parameters.Add(npgsqlParameter1);

    //      var npgsqlParameter2 = new NpgsqlParameter("p_parameter2", NpgsqlDbType.Jsonb | NpgsqlDbType.Array);
    //      npgsqlParameter2.Direction = ParameterDirection.Output;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter2);

    //      npgsqlCommand.WExecuteNonQuery(returnCompressionType: CompressionType.GZip);

    //      var value2 = npgsqlCommand.Parameters["p_parameter2"].Value;
    //      if (value2 != null && value2.GetType().IsArray)
    //      {
    //        var objects2 = value2 as object[];
    //        if (objects2 != null)
    //        {

    //          // we need to convert both json strings to JObject, and after that to order JObject by values,
    //          // because PostgreSQL keeps jsonb objects by their inner optimization, which violates the order of key - values of the json object
    //          var result = objects2.Select(x => ((string)x))
    //                               .Select(x => new JObject(JObject.Parse(x).Properties().OrderByDescending(i => Convert.ToString(i.Value))))
    //                               .ToArray();

    //          var expected = (npgsqlCommand.Parameters["p_parameter1"].Value as string[])!
    //            .Select(x => new JObject(JObject.Parse(x).Properties().OrderByDescending(i => Convert.ToString(i.Value))))
    //            .ToArray();

    //          Assert.That(result, Is.EqualTo(expected));
    //          return;
    //        }
    //      }

    //      Assert.Fail();
    //    }

    //    public void GetInOutArrayTest<T>(string postgreSQLFunctionName, object value, NpgsqlDbType npgsqlDbType)
    //    {
    //      using var npgsqlCommand = new NpgsqlCommand(postgreSQLFunctionName);
    //      var npgsqlParameter1 = new NpgsqlParameter("p_parameter1", npgsqlDbType);
    //      npgsqlParameter1.Value = value;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter1);

    //      var npgsqlParameter2 = new NpgsqlParameter("p_parameter2", npgsqlDbType);
    //      npgsqlParameter2.Direction = ParameterDirection.Output;
    //      npgsqlCommand.Parameters.Add(npgsqlParameter2);

    //      npgsqlCommand.WExecuteNonQuery(returnCompressionType: CompressionType.GZip);

    //      var value2 = npgsqlCommand.Parameters["p_parameter2"].Value;
    //      if (value2 != null && value2.GetType().IsArray)
    //      {
    //        var objects2 = value2 as object[];
    //        if (objects2 != null)
    //        {
    //          var result = objects2.Select(x => Convert.ChangeType(x, typeof(T))).ToArray();
    //          var expected = npgsqlCommand.Parameters["p_parameter1"].Value as T[];
    //          Assert.That(result, Is.EqualTo(expected));
    //          return;
    //        }
    //      }

    //      Assert.Fail();
    //    }
    //    #endregion array
    #endregion with in and out parameters
    #endregion non query


    #region return set

    #region special cases

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