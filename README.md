# The work with the data access web service by the SimpleWSA library

The short description

  The data access web service allows executing the PostgreSql functions by the HTTP 
requests. To perform it a client should have the appropriate credentials. If credentials
okay, then the data access web service creates a session, and returns a token.
  To perform the request the client should select the PostgreSql function, fill parameters
with appropriate values, attach the token, and make the HTTP request.
  The SimpleWSA library has been created to simplify this work.

Session

  The following code is an example of how to create the session:

      ...
      Session session = new Session("<the connection provider address>",
                                    "<login>",
                                    "<password>",
                                    false,
                                    <app code>,
                                    "<app version>",
                                    "<companyname>",
                                    <web proxy>);
      await session.CreateAsync();
	  ...

Calling the PostgreSql function returning a scalar data

  The following code is an example of how to get the scalar data:

      ...
      Command command = new Command("help_ws_es_int64_array_test");
      command.WriteSchema = WriteSchema.FALSE;
      Console.WriteLine(Command.Execute(command,
                                        RoutineType.Scalar,
                                        HttpMethod.GET,
                                        ResponseFormat.XML));
	  ...

Calling the PostgreSql function returning the data in the out parameters

      ...
      Command command = new Command("help_ws_en_inout_array");

	  // IN parameters
      command.Parameters.Add("p_parameter1", PgsqlDbType.Integer | PgsqlDbType.Array, new List<int> { 1, 2, 3, 4 });
      command.Parameters.Add("p_parameter2", PgsqlDbType.Varchar | PgsqlDbType.Array, new List<string> { "hello", "naiton", "clients" });

	  // OUT parameters
      command.Parameters.Add("p_parameter3", PgsqlDbType.Integer | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter4", PgsqlDbType.Varchar | PgsqlDbType.Array);
      command.Parameters.Add("p_parameter5", PgsqlDbType.Varchar);
      command.Parameters.Add("p_parameter6", PgsqlDbType.Integer);
      command.Parameters.Add("p_parameter7", PgsqlDbType.Timestamp);
      command.Parameters.Add("p_parameter8", PgsqlDbType.TimestampTZ | PgsqlDbType.Array);
      command.Parameters.Add("_returnvalue", PgsqlDbType.Integer);

	  command.WriteSchema = WriteSchema.FALSE;
      Console.WriteLine(Command.Execute(command,
                                        RoutineType.NonQuery,
                                        HttpMethod.GET,
                                        ResponseFormat.XML));
	  ...

Calling the PostgreSql function returning the data set

      Command command = new Command("salesinformationmanager_getsalesinformationfiltersvalues");
      command.Parameters.Add("_employeeid", PgsqlDbType.Integer, 3);
	  command.WriteSchema = WriteSchema.TRUE;
      string xmlResult = Command.Execute(command,
                                         RoutineType.DataSet,
                                         httpMethod: HttpMethod.GET,
                                         responseFormat: ResponseFormat.XML);
