using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Data
{
    public class WebApiException: Exception
    {
        public ErrorResponse? ErrorResponse { get; }
        public WebApiException(string errorJson) 
        {
            ErrorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorJson);
        
        }
    }
}
