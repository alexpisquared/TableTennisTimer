namespace TableTennisTimer.Models;

public class ModelCourt
{
    DateTimeOffset nextTime = DateTimeOffset.Now;
    object? wakeLock_OLD;
    public string countdownString = "↑ Select";
    public string report = "";
    public string error = "";
    public double progress = 0, regress = 0;
    public IJSRuntime JSRuntime { get; set; }
    int selectPeriodInMin;
#if DEBUG
    const bool isDebug = true;
#else
    const bool isDebug = false;
#endif

    public int SelectPeriodInMin
    {
        get { return selectPeriodInMin; }
        set
        {
            selectPeriodInMin = value;
            report = $"{value} min    IsLooping: {IsLooping}";
            Task.Run(async () =>
            {
                Initiated = true;
                SetWakeLockOn.Invoke();
              await PlayResource("Chirp", 0.2); await Task.Delay(99);
              //await PlayResource("Intro", 1.0); await Task.Delay(99);
              //await PlayResource("Power", 0.1); await Task.Delay(99);
              //await PlayResource("Fanfr", 0.1);
              if (IsLooping != true)
                    await MainLoopTask();
            });
        }
    }

    public List<PlayPeriod> PlayPeriods { get; set; } = new List<PlayPeriod>() { new PlayPeriod(10), new PlayPeriod(15), new PlayPeriod(30) };

    [Parameter] public bool Initiated { get; set; } = false;
    [Parameter] public bool IsAudible { get; set; } = true;
    [Parameter] public bool IsLooping { get; set; }
    [Parameter] public bool IsDebug { get; set; } = isDebug;

    public ModelCourt(
      Action stateHasChanged,
      Action setWakeLockOn,
      IJSRuntime jsRuntime)
    {
        StateHasChanged = stateHasChanged;
        SetWakeLockOn = setWakeLockOn;
        JSRuntime = jsRuntime;
        IsAudible = !IsDebug;
    }

    Action SetWakeLockOn;
    Action StateHasChanged;

    public void CheckboxChanged(bool e) { report = $"Audio is {((IsAudible = e) ? "ON" : "Off")}."; }

    public async Task MainLoopTask()
    {
        IsLooping = true;

        while (IsLooping)
        {
            var now = DateTimeOffset.Now;
            SetAndShowNextTime();

            while (IsLooping && now < nextTime)
            {
                int prev = selectPeriodInMin;
                await Task.Delay(999);
                if (prev != selectPeriodInMin) // if the user changed the time, then reset the timer
                {
                    SetAndShowNextTime();
                }

                now = DateTimeOffset.Now;
                double secondsLeft = (nextTime - now).TotalSeconds;
                countdownString = $"{(nextTime - now):m\\:ss}";
                progress = ((100 * (selectPeriodInMin * 60 - secondsLeft) / (selectPeriodInMin * 60)));
                regress = 100 - progress;

                StateHasChanged(); // await InvokeAsync(StateHasChanged);

                if (58 <= secondsLeft && secondsLeft <= 60)
                {
                    await PlayWavFilesAsync("Power", 1410, GetLastMinute());
                    await Task.Delay(1_640);
                }
            } // while (now < nextTime)

            if (IsLooping)
            {
                countdownString = "Rotate";
                StateHasChanged(); // await InvokeAsync(StateHasChanged);
                await PlayWavFilesAsync("Fanfr", 5_590, GetTimeToChange());
            }
            else
            {
                countdownString = "■ ■";
                error = "·";
            }
        } // while (IsLooping)

        await Task.Delay(250); // collides with the "Wake Lock released" sound. ...on NG.

        await PlayResource("Chirp", .5);
    }

    void SetAndShowNextTime()
    {
        var now = DateTimeOffset.Now;
        nextTime = now.AddMinutes(selectPeriodInMin - now.Minute % selectPeriodInMin).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    }

    public async void ClickHandlerCs1() { await PlayWavFilesAsync("Power", 360, GetLastMinute()); }
    public async void ClickHandlerCs2() { await PlayWavFilesAsync("Fanfr", 4_500, GetTimeToChange()); }
    async Task PlayWavFilesAsync(string name, int delay, string speech)
    {
        await PlayResource(name);
        await Task.Delay(delay); // will not play speech if delay is too short.
        await PlayResource(speech);
    }
    public async Task PlayResource(string filePath, double volume = 1.0)
    {
        if (IsAudible)
        {
            report = $"{filePath}  ++playing...";
            //     await JSRuntime.InvokeAsync<string>("PlayAudio", filePath);
            // using (await JSRuntime.InvokeAsync<Task>("PlayAudio", $"<audio controls volume=\"{volume}\"><source src=\"{filePath}\" type=\"audio/mpeg\"></audio>"))
            // await JSRuntime.InvokeVoidAsync("setVolume", filePath, volume);
            using (await JSRuntime.InvokeAsync<Task>("PlayAudio", filePath, volume))
            {
                report = $"{filePath}  ++playing... has finished!";
            }
            report = $"{filePath}  ++playing... has finished! +++++++++++++++++";
        }
        else
        {
            report = $"{filePath} ...but Audio is off.";
        }
    }

    string GetLastMinute()
    {
        string[] stringArr = new string[] {
      "angryLastMinute",      //      "Audio\\zh-CN-XiaomoNeural~1.00~100~angry~Last minute! EQAQJ！.7.mp3",
      "calmLastMinute",       //      "Audio\\zh-CN-XiaomoNeural~1.00~100~calm~Last minute! EQAQJ！.7.mp3",
      "cheerfulLastMinute",   //      "Audio\\zh-CN-XiaomoNeural~1.00~100~cheerful~Last minute! EQAQJ！.7.mp3",
      "gentleLastMinute",     //      "Audio\\zh-CN-XiaomoNeural~1.00~100~gentle~Last minute! EQAQJ！.7.mp3",
      "sadLastMinute",        //      "Audio\\zh-CN-XiaomoNeural~1.00~100~sad~Last minute! EQAQJ！.7.mp3",
      "seriousLastMinute" };  //      "Audio\\zh-CN-XiaomoNeural~1.00~100~serious~Last minute! EQAQJ！.7.mp3"};
        return stringArr[new Random().Next(stringArr.Length)];
    }
    string GetTimeToChange()
    {
        string[] stringArr = new string[] {
      "angryRotate",      //       "Audio\\zh-CN-XiaomoNeural~1.00~100~angry~Time to rotate! DYRGOE！.7.mp3",
      "calmRotate",       //       "Audio\\zh-CN-XiaomoNeural~1.00~100~calm~Time to rotate! DYRGOE！.7.mp3",
      "cheerfulRotate",   //       "Audio\\zh-CN-XiaomoNeural~1.00~100~cheerful~Time to rotate! DYRGOE！.7.mp3",
      "gentleRotate",     //       "Audio\\zh-CN-XiaomoNeural~1.00~100~gentle~Time to rotate! DYRGOE！.7.mp3",
      "sadRotate",        //       "Audio\\zh-CN-XiaomoNeural~1.00~100~sad~Time to rotate! DYRGOE！.7.mp3",
      "seriousRotate" };  //       "Audio\\zh-CN-XiaomoNeural~1.00~100~serious~Time to rotate! DYRGOE！.7.mp3"};
        return stringArr[new Random().Next(stringArr.Length)];
    }

    async Task RequestWakeLock_nogoOnIPhone()
    {
        try
        {
            wakeLock_OLD = await JSRuntime.InvokeAsync<object>("navigator.wakeLock.request", "screen"); //todo: if nogo: https://dev.to/this-is-learning/how-to-prevent-the-screen-turn-off-after-a-while-in-blazor-4b29
            report = "Wake Lock is  active -- !";
        }
        catch (Exception err) { error = $"{err.GetType().Name}.{nameof(RequestWakeLock_nogoOnIPhone)}, {err.Message}"; WriteLine(error); }
    }
}
