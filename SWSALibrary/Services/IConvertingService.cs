namespace SimpleWSA.Services
{
  public interface IConvertingService
  {
    object[] ConvertObjectToDb(PgsqlDbType pgsqlDbType, object value, EncodingType outgoingEncodingType);
    //object ConvertObjectFromDb(PgsqlDbType pgsqlDbType, object value, EncodingType returnEncodingType);
    string EncodeTo(object value, EncodingType outgoingEncodingType);
    //object DecodeFrom(object value, EncodingType returnEncodingType);
  }
}
