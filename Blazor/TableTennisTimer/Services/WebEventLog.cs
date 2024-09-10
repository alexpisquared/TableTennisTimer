using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTimer.Services;

public class WebEventLog
{
  //[NotMapped] public string[] Sub { get; set; } = ["TTTimer"];
  [NotMapped] public string BrowserSignature { get; set; } = "TTTimer";
  [NotMapped] public string Nickname { get; set; } = "TTTimer";      // don't change order!!! :parsing depends on it.
  [NotMapped] public string FirstVisitId { get; set; } = "TTTimer";  // don't change order!!! :parsing depends on it.
  public int Id { get; set; }
  public int WebsiteUserId { get; set; }
  public string EventName { get; set; } = "TTTimer";
  public DateTime DoneAt { get; set; }
}