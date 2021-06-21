namespace SimpleWSA
{
  public sealed class Parameter
  {
    public string Name { get; set; }
    public object Value { get; set; }
    public PgsqlDbType PgsqlDbType { get; set; }

    public Parameter(string name)
    {
      this.Name = name;
    }

    public Parameter(string name, PgsqlDbType pgsqlDbType)
    {
      this.Name = name;
      this.PgsqlDbType = pgsqlDbType;
    }

    public Parameter(string name, PgsqlDbType pgsqlDbType, object value)
    {
      this.Name = name;
      this.PgsqlDbType = pgsqlDbType;
      this.Value = value;
    }
  }
}
