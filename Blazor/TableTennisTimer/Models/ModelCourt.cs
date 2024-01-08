using System.Xml.Linq;

namespace TableTennisTimer.Models;

public class ModelCourt
{
  DateTimeOffset _nextTime = DateTimeOffset.Now;
  object? wakeLock_OLD;
  public string CountdownString = "↑ Select";
  public string Report = "";
  public string Error = "";
  public double Progress = 0, Regress = 0;

  public IJSRuntime? JSRuntime { get; set; }
  public IWebEventLoggerService? WebEventLoggerService { get; set; }
  public WebEventLog? WebEventLog { get; set; }

  int selectPeriodInMin;
#if DEBUG
  const bool isDebug = true;
#else
    const bool isDebug = false;
#endif

  public int SelectPeriodInMin
  {
    get => selectPeriodInMin;
    set {
      selectPeriodInMin = value;
      Report = $"{value} min    IsLooping: {IsLooping}";
      _ = Task.Run(async () =>
      {
        Initiated = true;
        SetWakeLockOn.Invoke();
        //await PlayResource("LastQ", 0.1); await Task.Delay(99);
        //await PlayResource("RotaQ", 0.1);
        await PlayResource("LastM", 10);
        await PlayResource("Rotat", 10);
        //still need this? await Task.Delay(10);
        await PlayResource("Intro");
        if (IsLooping != true)
          await MainLoopTask();
      });
    }
  }

  public List<PlayPeriod> PlayPeriods { get; set; } = [new(10), new(15), new(30)];

  [Parameter] public bool Initiated { get; set; } = false;
  [Parameter] public bool IsAudible { get; set; } = true;
  [Parameter] public bool IsLooping { get; set; }
  [Parameter] public bool IsDebug { get; set; } = isDebug;

  public void SetIsLooping(bool val) => IsLooping = val;
  public ModelCourt(Action stateHasChanged, Action setWakeLockOn)
  {
    StateHasChanged = stateHasChanged;
    SetWakeLockOn = setWakeLockOn;
    IsAudible = true; // !IsDebug;
  }

  readonly Action SetWakeLockOn;
  readonly Action StateHasChanged;

  public void CheckboxChanged(bool e) => Report = $"Audio is {((IsAudible = e) ? "ON" : "Off")}.";

  public async Task MainLoopTask()
  {
    IsLooping = true;

    while (IsLooping)
    {
      var now = DateTimeOffset.Now;
      _nextTime = SetAndShowNextTime();

      while (IsLooping && now < _nextTime)
      {
        var prev = selectPeriodInMin;
        await Task.Delay(991);
        if (prev != selectPeriodInMin) // if the user changed the time, then reset the timer
        {
          _nextTime = SetAndShowNextTime();
        }

        now = DateTimeOffset.Now;
        var secondsLeft = (_nextTime - now).TotalSeconds;
        CountdownString = $"{_nextTime - now:m\\:ss}";
        Progress = 100 * ((selectPeriodInMin * 60) - secondsLeft) / (selectPeriodInMin * 60);
        Regress = 100 - Progress;

        StateHasChanged(); // await InvokeAsync(StateHasChanged);

        if (secondsLeft is >= 59 and <= 61)
        {
          await PlayWavFilesAsync("LastM", 1410, GetLastMinute());
          await Task.Delay(1_640);
        }
        else if (secondsLeft > 60 && ((int)secondsLeft) % 60 == 0) // workaround for PWA mode, where the screen lock is not available.
        {
          await PlayResource("Intro", 100); // audible only on PC. Phone is silent but seems to ward off the screen lock.
        }
      } // while (now < _nextTime)

      if (IsLooping)
      {
        CountdownString = "Rotate";
        StateHasChanged(); // await InvokeAsync(StateHasChanged);
        await PlayWavFilesAsync("Rotat", 5_590, GetTimeToChange());

        ArgumentNullException.ThrowIfNull(WebEventLog, "@26");
        WebEventLog.EventName = "ttt-Rotation";
        WebEventLog.DoneAt = DateTime.Now;

        ArgumentNullException.ThrowIfNull(WebEventLoggerService, "@22");
        Report += await WebEventLoggerService.LogEventAsync("memberSince", WebEventLog);
      }
      else
      {
        CountdownString = "■ ■";
        Error = "·";
      }
    } // while (IsLooping)

    await Task.Delay(250); // collides with the "Wake Lock released" sound. ...on NG.

    await PlayResource("Chirp", 500);
  }

  DateTimeOffset SetAndShowNextTime()
  {
    var now = DateTimeOffset.Now;
    return now.AddMinutes(selectPeriodInMin - (now.Minute % selectPeriodInMin)).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
  }

  public async void PlayIntro() => await PlayWavFilesAsync("Intro", 360);
  public async void PlayLastM() => await PlayWavFilesAsync("LastM", 0_500);
  public async void PlayRotat() => await PlayWavFilesAsync("Rotat", 4_500);
  public async void PlayChirp() => await PlayWavFilesAsync("Chirp", 0_500);
  public async void PlayIntrQ() => await PlayWavFilesAsync("IntrQ", 0_500);
  public async void PlayLastQ() => await PlayWavFilesAsync("LastQ", 0_500);
  public async void PlayRotaQ() => await PlayWavFilesAsync("RotaQ", 4_500);
  public async void PlayChirQ() => await PlayWavFilesAsync("ChirQ", 0_500);
  async Task PlayWavFilesAsync(string name, int delay, string? speech = null)
  {
    await PlayResource(name);
    await Task.Delay(delay); // will not play speech if delay is too short.
    if (speech is not null)
      await PlayResource(speech);
  }
  public async Task PlayResource(string filePath, int pauseAtMs = 0)
  {
    if (IsAudible)
    {
      ArgumentNullException.ThrowIfNull(JSRuntime, "@21");

      _ = await JSRuntime.InvokeAsync<Task>("PlayAudio", filePath);  //, volume); //todo: volume does not work here.

      Report = $"{DateTime.Now:HH:mm:ss.fff}  {filePath}  played";

      if (pauseAtMs == 0) return;

      await Task.Delay(pauseAtMs);
      _ = await JSRuntime.InvokeAsync<Task>("PauseAudio", filePath);
    }
    else
    {
      Report = $"{filePath} ...but Audio is off.";
    }
  }

  static string GetLastMinute()
  {
    string[] stringArr = [
      "angryLastMinute",      //      "Audio\\zh-CN-XiaomoNeural~1.00~100~angry~Last minute! EQAQJ！.7.mp3",
      "calmLastMinute",       //      "Audio\\zh-CN-XiaomoNeural~1.00~100~calm~Last minute! EQAQJ！.7.mp3",
      "cheerfulLastMinute",   //      "Audio\\zh-CN-XiaomoNeural~1.00~100~cheerful~Last minute! EQAQJ！.7.mp3",
      "gentleLastMinute",     //      "Audio\\zh-CN-XiaomoNeural~1.00~100~gentle~Last minute! EQAQJ！.7.mp3",
      "sadLastMinute",        //      "Audio\\zh-CN-XiaomoNeural~1.00~100~sad~Last minute! EQAQJ！.7.mp3",
      "seriousLastMinute" ];  //      "Audio\\zh-CN-XiaomoNeural~1.00~100~serious~Last minute! EQAQJ！.7.mp3"};
    return stringArr[new Random().Next(stringArr.Length)];
  }

  static string GetTimeToChange()
  {
    string[] stringArr = [
      "angryRotate",      //       "Audio\\zh-CN-XiaomoNeural~1.00~100~angry~Time to rotate! DYRGOE！.7.mp3",
      "calmRotate",       //       "Audio\\zh-CN-XiaomoNeural~1.00~100~calm~Time to rotate! DYRGOE！.7.mp3",
      "cheerfulRotate",   //       "Audio\\zh-CN-XiaomoNeural~1.00~100~cheerful~Time to rotate! DYRGOE！.7.mp3",
      "gentleRotate",     //       "Audio\\zh-CN-XiaomoNeural~1.00~100~gentle~Time to rotate! DYRGOE！.7.mp3",
      "sadRotate",        //       "Audio\\zh-CN-XiaomoNeural~1.00~100~sad~Time to rotate! DYRGOE！.7.mp3",
      "seriousRotate" ];  //       "Audio\\zh-CN-XiaomoNeural~1.00~100~serious~Time to rotate! DYRGOE！.7.mp3"};
    return stringArr[new Random().Next(stringArr.Length)];
  }

  async Task RequestWakeLock_nogoOnIPhone()
  {
    try
    {
      ArgumentNullException.ThrowIfNull(JSRuntime, "@20");
      wakeLock_OLD = await JSRuntime.InvokeAsync<object>("navigator.wakeLock.request", "screen"); //todo: if nogo: https://dev.to/this-is-learning/how-to-prevent-the-screen-turn-off-after-a-while-in-blazor-4b29
      Report = "Wake Lock is  active -- !";
    }
    catch (Exception err) { Error = $"{err.GetType().Name}.{nameof(RequestWakeLock_nogoOnIPhone)}, {err.Message}"; WriteLine(Error); }
  }
}
