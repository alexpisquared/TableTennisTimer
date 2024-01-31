
namespace TableTennisTimer.Services;

public interface IWebEventLoggerService
{
  Task<string> LogEventAsync(/*string eventName,*/ WebEventLog eventData);
}