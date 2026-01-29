using Microsoft.AspNetCore.Mvc;
using QWK;
using SharpQWKReader.Web.Models;
using SharpQWKReader.Web.Services;
using System.IO;

namespace SharpQWKReader.Web.Controllers;

public class QWKController : Controller
{
    private readonly IQWKService _qwkService;
    private readonly ILogger<QWKController> _logger;
    private readonly IWebHostEnvironment _env;

    public QWKController(IQWKService qwkService, ILogger<QWKController> logger, IWebHostEnvironment env)
    {
        _qwkService = qwkService;
        _logger = logger;
        _env = env;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadPackage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Please select a file");
            return View("Index");
        }

        try
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            _qwkService.OpenQWKPacket(filePath);
            var bbsInfo = _qwkService.GetBBSInfo();
            
            HttpContext.Session.SetString("PackagePath", filePath);

            return RedirectToAction(nameof(Package), new { bbsId = bbsInfo.BbsId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading package");
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return View("Index");
        }
    }

    public IActionResult Package(string? bbsId)
    {
        try
        {
            var packagePath = HttpContext.Session.GetString("PackagePath");
            if (string.IsNullOrEmpty(packagePath) || !System.IO.File.Exists(packagePath))
            {
                return RedirectToAction(nameof(Index));
            }

            var bbsInfo = _qwkService.GetBBSInfo();
            var forums = _qwkService.GetForums();

            var model = new PackageViewModel
            {
                BBSInfo = bbsInfo,
                Forums = forums
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading package");
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Forum(string forumId)
    {
        try
        {
            var messages = _qwkService.GetForumMessages(forumId);
            
            var model = new ForumViewModel
            {
                ForumId = forumId,
                Messages = messages
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading forum");
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Message(ulong messageNumber)
    {
        try
        {
            var message = _qwkService.GetMessage(messageNumber);
            return View(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading message");
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }
}
