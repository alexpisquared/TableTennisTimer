namespace TableTennisTimer.Services;

public interface IScreenWakeLockService
{
  Task<bool> IsSupportedAsync();
  Task ReleaseWakeLockAsync(WakeLockSentinel sentinel);
  Task<WakeLockSentinel> RequestWakeLockAsync();
}