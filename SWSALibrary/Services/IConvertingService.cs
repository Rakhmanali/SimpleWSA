namespace SimpleWSA.Services
{
  public interface IConvertingService
  {
    object[] ConvertObjectToDb(PgsqlDbType pgsqlDbType, object value, EncodingType outgoingEncodingType, bool isMigration);
    string EncodeTo(object value, EncodingType outgoingEncodingType);
  }
}
