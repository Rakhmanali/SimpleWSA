## The work with the data access web service by the SimpleWSA library

The short description

  The data access web service allows executing the PostgreSql functions by the HTTP 
requests. To perform it a client should have the appropriate credentials. If credentials
okay, then the data access web service creates a session, and returns a token.
  To perform the request the client should select the PostgreSql function, fill parameters
with appropriate values, attach the token (this library does it itself), and make the
HTTP request.
  The SimpleWSA library has been created to simplify this work.

### 1. Session

  The data access web service can provide data access to the group of databases. And there 
  can be different instances of web service for different groups of databases.

  If it is known the database and the web service address extracting data from it, then it is possible
  to create a session by the web service address:

  ```csharp
      ...
      Session session = new Session("<login>",
                                    "<password>",
                                    false,
                                    <app id>,
                                    "<app version>",
                                    "<domain>",
                                    <web proxy>);
      await session.CreateByRestServiceAddressAsync("<replace it with the web service address>");
	  ...
  ```

  The second case is the case when known the domain name (the company name) and the connection provider 
  address. Then the following code creates the session:

  ```csharp
      ...
      Session session = new Session("<login>",
                                    "<password>",
                                    false,
                                    <app code>,
                                    "<app version>",
                                    "<domain>",
                                    <web proxy>);
      await session.CreateByConnectionProviderAddressAsync("<replace it with the connection provider address>");
	  ...
  ```

### 2. How to call a PostgreSql function returning the scalar data

   The following code is an example of how to get the scalar data:

   ```csharp
      ...
      Command command = new Command("clientmanager_findclientbyemailbusiness");
      command.Parameters.Add("_businessid", PgsqlDbType.Integer).Value = 1;
      command.Parameters.Add("_email", PgsqlDbType.Text).Value = "femkedijkema@hotmail.com";
      command.Parameters.Add("_personid", PgsqlDbType.Integer).Value = 3;
      command.WriteSchema = WriteSchema.FALSE;
      Console.WriteLine(Command.Execute(command,
                                        RoutineType.Scalar,
                                        HttpMethod.GET,
                                        ResponseFormat.XML));
	  ...
   ```

### 3. How to call the PostgreSql function returning the data in the out parameters

```csharp
      ...
      Command command = new Command("brandmanager_hidebrand");
      command.Parameters.Add("_brandid", PgsqlDbType.Integer, 13);
      command.Parameters.Add("_ishidden", PgsqlDbType.Boolean).Value = false;
      command.Parameters.Add("_returnvalue", PgsqlDbType.Integer);

	  command.WriteSchema = WriteSchema.FALSE;
      Console.WriteLine(Command.Execute(command,
                                        RoutineType.NonQuery,
                                        HttpMethod.GET,
                                        ResponseFormat.XML));
	  ...
```

### 4. How to call the PostgreSql function returning the data set

```csharp
      ...
      Command command = new Command("companymanager_getresellers");
      command.Parameters.Add("_businessid", PgsqlDbType.Integer).Value = 1;
      command.Parameters.Add("_companyid", PgsqlDbType.Integer).Value = 13;
	  command.WriteSchema = WriteSchema.TRUE;
      string xmlResult = Command.Execute(command,
                                         RoutineType.DataSet,
                                         httpMethod: HttpMethod.GET,
                                         responseFormat: ResponseFormat.XML);
      ...
```

### 5. How to call more than one the same type of PostgreSql functions

   For example, let's describe how to call two PostgreSql functions eveach of them 
   returns the set of data:

```csharp
      ...

	  // the same type of routines in one HTTP request

      Command command1 = new Command("companymanager_getresellers");
      command1.Parameters.Add("_businessid", PgsqlDbType.Integer).Value = 1;
      command1.Parameters.Add("_companyid", PgsqlDbType.Integer).Value = 13;
      command1.WriteSchema = WriteSchema.TRUE;

      Command command2 = new Command("currencymanager_getbusinessessuppliers");
      command2.Parameters.Add("_currencyid", PgsqlDbType.Integer).Value = 1;
      command2.WriteSchema = WriteSchema.TRUE;

      string xmlResult = Command.ExecuteAll(new List<Command> { command1, command2 },
                                            RoutineType.DataSet,
                                            ResponseFormat.XML,
                                            parallelExecution: ParallelExecution.TRUE);
      ...
```

	  The value "ParallelExecution.TRUE" of the parameter "parallelExecution" instructs the server
	  to execute the PostgreSql functions "companymanager_getresellers" and 
	  "currencymanager_getbusinessessuppliers" parallely.

### 6. There is possible to execute different type of PostgreSql functions 

```csharp
      ...
      // the different type of routines in one HTTP request

      CommandEx command1 = new CommandEx("companymanager_getresellers");
      command1.Parameters.Add("_businessid", PgsqlDbType.Integer).Value = 1;
      command1.Parameters.Add("_companyid", PgsqlDbType.Integer).Value = 13;
      command1.WriteSchema = WriteSchema.TRUE;
      command1.RoutineType = RoutineType.DataSet;

      CommandEx command2 = new CommandEx("brandmanager_hidebrand");
      command2.Parameters.Add("_brandid", PgsqlDbType.Integer, 13);
      command2.Parameters.Add("_ishidden", PgsqlDbType.Boolean).Value = false;
      command2.Parameters.Add("_returnvalue", PgsqlDbType.Integer);
      command2.RoutineType = RoutineType.NonQuery;

      CommandEx command3 = new CommandEx("clientmanager_findclientbyemailbusiness");
      command3.Parameters.Add("_businessid", PgsqlDbType.Integer).Value = 1;
      command3.Parameters.Add("_email", PgsqlDbType.Text).Value = "femkedijkema@hotmail.com";
      command3.Parameters.Add("_personid", PgsqlDbType.Integer).Value = 3;
      command3.RoutineType = RoutineType.Scalar;

      string xmlResult = CommandEx.ExecuteAll(new List<CommandEx> { command1, command2, command3 },
                                              ResponseFormat.XML,
                                              parallelExecution: ParallelExecution.TRUE);
      ...
```

### 7. Again about session
   
   In the above was described how to create the session. If during for 20 minutes no one request 
   was done the session will be expired, and the next request issues an exception.
   The following code solves this problem, i.e. creates a new session and continues the work:

```csharp
      ...
      while (true)
      {

        try
        {
          Command command = new Command("brandmanager_hidebrand");
          command.Parameters.Add("_brandid", PgsqlDbType.Integer, 13);
          command.Parameters.Add("_ishidden", PgsqlDbType.Boolean).Value = false;
          command.Parameters.Add("_returnvalue", PgsqlDbType.Integer);

          string xmlResult = Command.Execute(command,
                                             RoutineType.NonQuery,
                                             httpMethod: HttpMethod.POST,
                                             responseFormat: ResponseFormat.XML);
          Console.WriteLine(xmlResult);
        }
        catch (RestServiceException ex)
        {
          Console.WriteLine($"the rest service exception, code: {ex.Code}, message: {ex.Message}");
          if (ex.Code == "MI008")
          {
            await SessionContext.Refresh();
            continue;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }

        Console.WriteLine("press q to exit, or any other key to continue the work...");
        ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
        if (consoleKeyInfo.KeyChar == 'q' || consoleKeyInfo.KeyChar == 'Q')
        {
          Console.WriteLine("bye, bye...");
          break;
        }
      }
	  ...
```