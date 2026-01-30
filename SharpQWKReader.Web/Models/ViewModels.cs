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
    public List<MessagePointer>? MessagePointers { get; set; }
}

public class UploadedPackageViewModel
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
}

public class UploadedPackagesListViewModel
{
    public List<UploadedPackageViewModel> Packages { get; set; } = new();
}

public class OpenPackageViewModel
{
    public string PackageFileName { get; set; } = string.Empty;
    public BBSInfo? BBSInfo { get; set; }
}
