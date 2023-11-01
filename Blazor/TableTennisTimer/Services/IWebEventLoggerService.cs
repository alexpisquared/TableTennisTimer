
namespace TableTennisTimer.Services;

public interface IWebEventLoggerService
{
  Task LogEventAsync(string eventName, string eventData);
}