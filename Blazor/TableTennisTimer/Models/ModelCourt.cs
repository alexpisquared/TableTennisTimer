﻿namespace TableTennisTimer.Models;

public class ModelCourt
{
  const int _min = 50;
  int _ms = 200, _a;
  object? wakeLock_OLD;
  DateTimeOffset _nextTime = DateTimeOffset.Now;
  public string CountdownString = "";
  string lowerLog = "";
  public string WLStatus = "WLStatus";
  public string ErrorMsg = "";
  public double Progress = 50, Regress = 50, version = 20240722.2200;

  public IWebEventLoggerService? WebEventLoggerService { get; set; }
  public IJSRuntime? JSRuntime { get; set; }
  public WebEventLog? WebEventLog { get; set; }

#if DEBUG
  const bool isDebug = true;
#else
  const bool isDebug = false;
#endif

  int _selectPeriodInMin; public int SelectPeriodInMin
  {
    get => _selectPeriodInMin;
    set { _selectPeriodInMin = value; IsSelected = true; SetNextTimesString(); }
  }
  bool _isRoundedMode = true; public bool IsRoundedMode { get => _isRoundedMode; set { _isRoundedMode = value; SetNextTimesString(); } }

  void SetNextTimesString()
  {
    _nextTime = CalculateNextTime(IsRoundedMode);
    NextTime_String = $"{_nextTime:HH:mm:ss}";
    NextTimesString =
        $" {_nextTime.AddMinutes(SelectPeriodInMin * 1):HH:mm:ss} \n"
      + $" {_nextTime.AddMinutes(SelectPeriodInMin * 2):HH:mm:ss} \n"
      + $" {_nextTime.AddMinutes(SelectPeriodInMin * 3):HH:mm:ss} \n"
      + $" {_nextTime.AddMinutes(SelectPeriodInMin * 4):HH:mm:ss} \n"
      + $" {_nextTime.AddMinutes(SelectPeriodInMin * 5):HH:mm:ss} \n"
      + $" {_nextTime.AddMinutes(SelectPeriodInMin * 6):HH:mm:ss} \n ...";
  }

  async Task StartAgain()
  {
    Initiated = IsSelected = true;
    await PlayResource("Intro");
    SetWakeLockOn.Invoke();
    await PlayResource("LastM", 10); // preload!!!
    await PlayResource("Rotat", 10); // preload!!!
    //tmi: _ = Task.Run(async () => await LogToAzureLog($"ttt·Start-{(_isRoundedMode ? "Round" : "Dirty")}")); // :too slow, thus: Fire and forget.
    if (IsLooping != true)
      await MainLoopTask();
  }

  public async void StartAsync() => await StartAgain();
  public async void StopButton()
  {
    IsLooping = false;
    CountdownString = "Stop"; await Task.Delay(333);
    Initiated = IsSelected = false;
    SetWakeLockOff.Invoke();
    _ = Task.Run(async () => await LogToAzureLog($"ttt·Stop-{(_isRoundedMode ? "Round" : "Dirty")}")); // :too slow, thus: Fire and forget.
  }

  public List<PlayPeriod> PlayPeriods { get; set; } = [new(10), new(15)];

  [Parameter] public bool Initiated { get; set; } = false;
  [Parameter] public bool IsSelected { get; set; } = false;
  [Parameter] public bool IsLooping { get; set; }
  public bool IsAudible { get; set; } = true;
  public bool IsDebug { get; set; } = isDebug;
  public string NextTime_String { get; private set; } = "";
  public string NextTimesString { get; private set; } = "";
  public string LowerLog { get => lowerLog; set => lowerLog += $"\n{value}"; }

  public void StopTimer() => IsLooping = false;
  public ModelCourt(Action stateHasChanged, Action setWakeLockOn, Action setWakeLockOff)
  {
    StateHasChanged = stateHasChanged;
    SetWakeLockOn = setWakeLockOn;
    SetWakeLockOff = setWakeLockOff;
    //IsAudible = true; // !IsDebug;
  }
  public async Task OnInitializedAsync()
  {
    SelectPeriodInMin = 10;
    await Task.Delay(0);
  }

  readonly Action SetWakeLockOn;
  readonly Action SetWakeLockOff;
  readonly Action StateHasChanged;

  public void CheckboxChanged(bool e) => LowerLog = $"Audio is {((IsAudible = e) ? "ON" : "Off")}.";

  public async Task MainLoopTask()
  {
    IsLooping = true;
    _nextTime = CalculateNextTime(IsRoundedMode);
    NextTime_String = $"{_nextTime:HH:mm:ss}";

    while (IsLooping)
    {
      var now = DateTimeOffset.Now;

      while (IsLooping && now < _nextTime)
      {
        var prev = _selectPeriodInMin;
        await Task.Delay(991);
        if (prev != _selectPeriodInMin) // if the user changed the time, then reset the timer
        {
          _nextTime = CalculateNextTime(IsRoundedMode);
          NextTime_String = $"{_nextTime:HH:mm:ss}";
        }

        now = DateTimeOffset.Now;
        var secondsLeft = (_nextTime - now).TotalSeconds;
        CountdownString = $"{_nextTime - now:m\\:ss}";
        Progress = 100 * ((_selectPeriodInMin * 60) - secondsLeft) / (_selectPeriodInMin * 60);
        Regress = 100 - Progress;

        StateHasChanged(); // await InvokeAsync(StateHasChanged);

        if (secondsLeft is >= 59 and <= 61)
        {
          await PlayWavFilesAsync("LastM", 1410, "cheerfulLastMinute");
          await Task.Delay(1_640);
        }
        else
        if (((int)secondsLeft + 11) % 20 == 0) // workaround for PWA mode, where the screen lock is not available.
        {
          _ms = _ms > _min ? _ms - 50 : _min;
          await PlayResource(_audios[_a++ % _audios.Length], _ms);
          //await LogToAzureLog($"ttt·{_ms}");
        }
      } // while (now < _nextTime)

      _nextTime = CalculateNextTime(IsRoundedMode);

      NextTime_String = isDebug ? $"{NextTime_String}\n{_nextTime:HH:mm:ss.fff}" : $"{_nextTime:HH:mm:ss}";

      Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff}  {_nextTime:HH:mm:ss.fff}");

      if (IsLooping)
      {
        CountdownString = "Rotate";
        StateHasChanged(); // await InvokeAsync(StateHasChanged);
        await PlayWavFilesAsync("Rotat", 5_590, "cheerfulRotate");
        await LogToAzureLog($"ttt·Rotation v{version}");
      }
      else
      {
        CountdownString = "Start";
        ErrorMsg = "·";
      }
    } // while (IsLooping)

    await Task.Delay(250); // collides with the "Wake Lock released" sound. ...on NG.

    await PlayResource("Chirp", 500);
  }

  private async Task LogToAzureLog(string dd)
  {
    ArgumentNullException.ThrowIfNull(WebEventLog, "@26");
    WebEventLog.EventName = dd;
    WebEventLog.DoneAt = DateTime.Now;

    ArgumentNullException.ThrowIfNull(WebEventLoggerService, "@22");
    LowerLog = await WebEventLoggerService.LogEventAsync(/*"memberSince",*/ WebEventLog);
  }

  DateTimeOffset CalculateNextTime(bool isRounded)
  {
    var now = DateTimeOffset.Now;
    return isRounded ?
      now.AddMinutes(_selectPeriodInMin - (now.Minute % _selectPeriodInMin)).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond) :
      now.AddMinutes(_selectPeriodInMin).AddMilliseconds(-200);
  }

  public async void PlayIntro() => await PlayWavFilesAsync("Intro", 360);
  public async void PlayLastM() => await PlayWavFilesAsync("LastM", 0_500);
  public async void PlayRotat() => await PlayWavFilesAsync("Rotat", 4_500);
  public async void PlayChirp() => await PlayWavFilesAsync("Chirp", 0_500);

  readonly string[] _audios = { "Intro", "LastM", "Rotat", "cheerfulLastMinute", "cheerfulRotate", "LockReleased", "Chirp" }; /*"IntrQ", "LastQ", "RotaQ", "ChirQ", "angryLastMinute", "calmLastMinute", "gentleLastMinute", "sadLastMinute", "seriousLastMinute", "angryRotate", "calmRotate", "cheerfulRotate", "gentleRotate", "sadRotate", "seriousRotate",  */

  public async void PlayAllMs()
  {
    try
    {
      foreach (var item in _audios) await PlayResource(item, 800);      //tmi: LowerLog = $"{DateTime.Now:HH:mm:ss}  played _audios ■ ■ ■";
    }
    catch (Exception err) { ErrorMsg = $"{err.GetType().Name}.{nameof(RequestWakeLock_nogoOnIPhone)}, {err.Message}"; WriteLine(ErrorMsg); }
  }

  async Task PlayWavFilesAsync(string name, int delay, string? speech = null)
  {
    await PlayResource(name);
    await Task.Delay(delay); // will not play speech if delay is too short.
    if (speech is not null)
      await PlayResource(speech);
  }
  public async Task PlayResource(string filePath, int pauseAtMs = 0)
  {
    try
    {
      if (IsAudible)
      {
        ArgumentNullException.ThrowIfNull(JSRuntime, "@21");

        _ = await JSRuntime.InvokeAsync<Task>("PlayAudio", filePath);  //, volume); //todo: volume does not work here.

        LowerLog = $"{DateTime.Now:HH:mm:ss} {pauseAtMs,5} ms  of {filePath}";

        if (pauseAtMs == 0) return;

        await Task.Delay(pauseAtMs);
        _ = await JSRuntime.InvokeAsync<Task>("PauseAudio", filePath);
      }
      else
      {
        LowerLog = $"{filePath} ...but Audio is off.";
      }
    }
    catch (Exception err) { ErrorMsg = $"{err.GetType().Name}.{nameof(RequestWakeLock_nogoOnIPhone)}, {err.Message}"; WriteLine(ErrorMsg); }
  }

  async Task RequestWakeLock_nogoOnIPhone()
  {
    try
    {
      ArgumentNullException.ThrowIfNull(JSRuntime, "@20");
      wakeLock_OLD = await JSRuntime.InvokeAsync<object>("navigator.wakeLock.request", "screen"); //todo: if nogo: https://dev.to/this-is-learning/how-to-prevent-the-screen-turn-off-after-nxt-while-in-blazor-4b29
      WLStatus = "Wake Lock is  active -- !";
    }
    catch (Exception err) { ErrorMsg = $"{err.GetType().Name}.{nameof(RequestWakeLock_nogoOnIPhone)}, {err.Message}"; WriteLine(ErrorMsg); }
  }
}
///todo: remove logging of navigations to home page in favour of actual manipulations:
/// - period, timer selection
/// - actual goes offs
