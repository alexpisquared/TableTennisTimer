
using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTimer.Services;

public class WebEventLog
{
  [NotMapped] public string[] Sub { get; set; }
  [NotMapped] public string BrowserSignature { get; set; } = "New TTTimer";
  [NotMapped] public string Nickname { get; set; } = "New TTTimer";      // don't change order!!! :parsing depends on it.
  [NotMapped] public string FirstVisitId { get; set; } = "New TTTimer";  // don't change order!!! :parsing depends on it.
  public int Id { get; set; }
  public int WebsiteUserId { get; set; }
  public string EventName { get; set; } = "New TTTimer";
  public DateTime DoneAt { get; set; }
}