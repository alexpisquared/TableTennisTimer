using Microsoft.JSInterop;

namespace ScreenWakeLockDemo.Services;

public class WakeLockSentinel
{
  public WakeLockSentinel(int id, IJSObjectReference jsObjectReference)
  {
    Id = id;
    JsObjectReference = jsObjectReference;
  }

  public int Id { get; }

  public IJSObjectReference JsObjectReference { get; }
}