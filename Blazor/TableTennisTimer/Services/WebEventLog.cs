
using System.ComponentModel.DataAnnotations.Schema;

namespace TableTennisTimer.Services;

internal class WebEventLog
{
  [NotMapped] public string[] Sub { get; set; }
  [NotMapped] public string BrowserSignature { get; set; } = "Copy";
  [NotMapped] public string Nickname { get; set; } = "Copy";      // don't change order!!! :parsing depends on it.
  [NotMapped] public string FirstVisitId { get; set; } = "Copy";  // don't change order!!! :parsing depends on it.
  public int Id { get; set; }
  public int WebsiteUserId { get; set; }
  public string EventName { get; set; } = "Copy";
  public DateTime DoneAt { get; set; }
}