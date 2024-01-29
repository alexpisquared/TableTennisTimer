using System.Collections.Concurrent;
using System.Diagnostics;

namespace TableTennisTimer.Services;

public class ScreenWakeLockService : IScreenWakeLockService
{
  readonly IJSRuntime _jsRuntime;
  readonly ConcurrentDictionary<int, WakeLockSentinel> _wakeLocks;
  int _nextId;

  public ScreenWakeLockService(IJSRuntime jsRuntime)
  {
    _jsRuntime = jsRuntime;
    _wakeLocks = new ConcurrentDictionary<int, WakeLockSentinel>();
    _nextId = 0;
  }

  public async Task<WakeLockSentinel> RequestWakeLockAsync()
  {
    // Check if the browser supports the screen wake lock API
    var isSupported = await IsSupportedAsync();
    if (!isSupported)
      throw new NotSupportedException("The browser does not support the screen wake lock API.");

    // Request a screen wake lock and get a JS object reference
    var jsObjectReference = await _jsRuntime.InvokeAsync<IJSObjectReference>("navigator.wakeLock.request", "screen");

    // Create a sentinel object and store it in a dictionary
    var id = Interlocked.Increment(ref _nextId);
    var sentinel = new WakeLockSentinel(id, jsObjectReference);

    Console.WriteLine($"■ Before Console. {_wakeLocks.Count}");
    Trace.WriteLine($"■ Before Trace. {_wakeLocks.Count}");
    Debug.WriteLine($"■ Before Debug. {_wakeLocks.Count}");
    _ = _wakeLocks.TryAdd(id, sentinel);
    Console.WriteLine($"■ After Console. {_wakeLocks.Count}");
    Trace.WriteLine($"■ After Trace. {_wakeLocks.Count}");
    Debug.WriteLine($"■ After Debug. {_wakeLocks.Count}");

    return sentinel;
  }

  public async Task ReleaseWakeLockAsync(WakeLockSentinel sentinel)
  {
    // Check if the sentinel object is valid
    if (sentinel == null || sentinel.JsObjectReference == null)
      throw new ArgumentNullException(nameof(sentinel));

    // Release the screen wake lock and dispose the JS object reference
    await sentinel.JsObjectReference.InvokeVoidAsync("release");
    await sentinel.JsObjectReference.DisposeAsync();

    // Remove the sentinel object from the dictionary
    _ = _wakeLocks.TryRemove(sentinel.Id, out _);
  }

  public async Task<bool> IsSupportedAsync() =>
    // Check if the navigator.wakeLock property exists
    await _jsRuntime.InvokeAsync<bool>("eval", "typeof navigator.wakeLock !== 'undefined'");
}

/// this is this:
/// https://dev.to/this-is-learning/how-to-prevent-the-screen-turn-off-after-a-while-in-blazor-4b29
///todo: also, see for PWA:
/// https://stackoverflow.com/questions/59917660/wake-lock-works-in-browser-but-not-as-pwa
/// https://reillyeon.github.io/scraps/wakelock.html
