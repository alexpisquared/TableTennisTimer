namespace TableTennisTimer.Models;

public class PlayPeriod
{
    public PlayPeriod(int min) { PeriodInMin = min; }
    public int PeriodInMin { get; set; } = 22;
    public bool IsSelected { get; set; }
}
