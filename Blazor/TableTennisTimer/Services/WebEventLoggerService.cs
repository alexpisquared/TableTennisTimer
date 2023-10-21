using System.Text;
using System.Text.Json;

public class WebEventLoggerService
{
    private readonly HttpClient _httpClient;

    public WebEventLoggerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LogEventAsync(string eventName, IDictionary<string, object> eventData)
    {
        var logData = new Dictionary<string, object>
        {
            { "EventName", eventName },
            { "EventData", eventData }
        };

        var json = JsonSerializer.Serialize(logData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await _httpClient.PostAsync("api/LogEvent", content);
    }
}
