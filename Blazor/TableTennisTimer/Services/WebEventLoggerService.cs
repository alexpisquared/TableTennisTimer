﻿using System.Security.Principal;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TableTennisTimer.Services;
public class WebEventLoggerService : IWebEventLoggerService
{
  readonly HttpClient _httpClient;
  const string _33 = "https://localhost:5001/api/WebEventLogs"; // = isDevMode() ? "https://localhost:5001" : "https://alex-pi-api.azurewebsites.net";

  public WebEventLoggerService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<string> LogEventAsync(string eventName, string eventData) //nor4wasm: use AzurePseudoLogger
  {
    try
    {
      var logData = new Dictionary<string, object> { { "EventName", eventName }, { "EventData", eventData } };
      var json = JsonSerializer.Serialize(logData);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      WriteLine($"■ ■ ■ {eventName} {eventData} ==> {json}");

      using var client = new HttpClient();
      // For a solution in Blazor WASM, NOT on Serverside, this code snippet     https://stackoverflow.com/questions/64858434/net5-0-blazor-wasm-cors-client-exception /// but: // CORS is a browser feature that needs to be applied by the web servers that a browsers calls. WebAssembly runs in the browser. So you cannot set CORS policies on Blazor WebAssembly. This needs to be allowed on the server application you are calling. – 

      var httpRequestMessage = new HttpRequestMessage { RequestUri = new Uri(_33), Method = HttpMethod.Get };
      WebAssemblyHttpRequestMessageExtensions.SetBrowserRequestMode(httpRequestMessage, BrowserRequestMode.NoCors);
      var response = await client.SendAsync(httpRequestMessage); // .....PostAsync(_33, content);
      if (response.IsSuccessStatusCode)
      {
        var strResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        WriteLine($"■ ■ ■ {strResult}");
        return strResult;
      }
      else
      {
        throw new Exception($"Failed to get Domain for service. Error {response.StatusCode}");
      }
    }
    catch (Exception ex)
    {
      WriteLine($"■ ■ ■ {ex}");
      return ex.Message;
    }
  }

  static async Task GetTest(HttpClient client)
  {
    var result = await client.GetAsync(_33).ConfigureAwait(false);
    if (result.IsSuccessStatusCode)
    {
      var strResult = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
      WriteLine($"■ ■ ■ {strResult}");
    }
    else
    {
      throw new Exception($"Failed to get Domain for service. Error {result.StatusCode}");
    }
  }

  private bool isDevMode() => true;
}
