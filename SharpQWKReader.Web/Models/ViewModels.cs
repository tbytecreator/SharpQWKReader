using QWK;

namespace SharpQWKReader.Web.Models;

public class PackageViewModel
{
    public BBSInfo? BBSInfo { get; set; }
    public List<Forum>? Forums { get; set; }
}

public class ForumViewModel
{
    public string? ForumId { get; set; }
    public List<Message>? Messages { get; set; }
}
