using QWK;
namespace SharpQWKReader.Web.Services;

public interface IQWKService
{
    void OpenQWKPacket(string packetPath);
    BBSInfo GetBBSInfo();
    List<Forum> GetForums();
    List<Message> GetForumMessages(string forumId);
    Message GetMessage(ulong messageNumber);
}

public class QWKService : IQWKService
{
    private readonly string _tmpDir;
    private readonly ILogger<QWKService> _logger;

    public QWKService(ILogger<QWKService> logger)
    {
        _logger = logger;
        _tmpDir = Path.Combine("uploads/", "qwk_temp_" + Guid.NewGuid()+"/");
    }

    public void OpenQWKPacket(string packetPath)
    {
        try
        {
            if (!File.Exists(packetPath))
                throw new FileNotFoundException($"QWK packet not found: {packetPath}");

            Methods.OpenQWKPacket(packetPath, _tmpDir);
            _logger.LogInformation($"QWK packet opened: {packetPath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening QWK packet");
            throw;
        }
    }

    public BBSInfo GetBBSInfo()
    {
        return Methods.GetBBSInfo(_tmpDir);
    }

    public List<Forum> GetForums()
    {
        var bbsinfo = GetBBSInfo();
        var lines = Methods.OpenControlDat(_tmpDir);
        return Methods.GetForuns(_tmpDir, bbsinfo, lines);
    }

    public List<Message> GetForumMessages(string forumId)
    {
        return Methods.GetForumMessages(_tmpDir, forumId);
    }

    public Message GetMessage(ulong messageNumber)
    {
        return Methods.GetMessage(_tmpDir, messageNumber);
    }

    ~QWKService()
    {
        if (Directory.Exists(_tmpDir))
        {
            try
            {
                Directory.Delete(_tmpDir, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to delete temp directory: {ex.Message}");
            }
        }
    }
}
