using Microsoft.AspNetCore.Mvc;
using QWK;
using SharpQWKReader.Web.Configuration;
using SharpQWKReader.Web.Models;
using SharpQWKReader.Web.Services;

namespace SharpQWKReader.Web.Controllers;

public class QWKController : Controller
{
    private readonly IQWKService _qwkService;
    private readonly ILogger<QWKController> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IQWKConfig _qwkConfig;

    public QWKController(IQWKService qwkService, ILogger<QWKController> logger, IWebHostEnvironment env, IQWKConfig qwkConfig)
    {
        _qwkService = qwkService;
        _logger = logger;
        _env = env;
        _qwkConfig = qwkConfig;
    }

    public IActionResult Index()
    {
        try
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var model = new UploadedPackagesListViewModel();

            if (Directory.Exists(uploadsFolder))
            {
                var di = new DirectoryInfo(uploadsFolder);
                var files = di.GetFiles("*.qwk");

                foreach (var file in files.OrderByDescending(f => f.CreationTime))
                {
                    model.Packages.Add(new UploadedPackageViewModel
                    {
                        FileName = file.Name,
                        FilePath = file.FullName,
                        FileSize = file.Length,
                        UploadDate = file.CreationTime
                    });
                }
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading packages on index");
            return View(new UploadedPackagesListViewModel());
        }
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

            return RedirectToAction(nameof(UploadedPackages));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading package");
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return View("Index");
        }
    }

    public IActionResult UploadedPackages()
    {
        try
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var model = new UploadedPackagesListViewModel();

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                return View(model);
            }

            var di = new DirectoryInfo(uploadsFolder);
            var files = di.GetFiles("*.qwk");

            foreach (var file in files.OrderByDescending(f => f.CreationTime))
            {
                model.Packages.Add(new UploadedPackageViewModel
                {
                    FileName = file.Name,
                    FilePath = file.FullName,
                    FileSize = file.Length,
                    UploadDate = file.CreationTime
                });
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing uploaded packages");
            return View(new UploadedPackagesListViewModel());
        }
    }

    public IActionResult OpenPackage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var tempBaseFolder = Path.Combine(_env.WebRootPath, _qwkConfig.TempDirectory);
            var packagePath = Path.Combine(uploadsFolder, Path.GetFileName(fileName));

            if (!System.IO.File.Exists(packagePath))
            {
                _logger.LogWarning($"Package not found: {packagePath}");
                return RedirectToAction(nameof(Index));
            }

            // Criar pasta temporária única
            Directory.CreateDirectory(tempBaseFolder);

            try
            {
                // Descompactar o pacote
                Methods.OpenQWKPacket(packagePath, tempBaseFolder);

                // Obter informações do BBS
                var bbsInfo = Methods.GetBBSInfo(tempBaseFolder);

                var model = new OpenPackageViewModel
                {
                    PackageFileName = Path.GetFileName(packagePath),
                    BBSInfo = bbsInfo,
                };

                // Armazenar o caminho temporário na sessão para usar depois
                HttpContext.Session.SetString("CurrentTmpDir", tempBaseFolder);
                HttpContext.Session.SetString("CurrentPackagePath", packagePath);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error opening package: {packagePath}");
                // Limpar pasta temporária em caso de erro
                if (Directory.Exists(tempBaseFolder))
                {
                    Directory.Delete(tempBaseFolder, true);
                }
                ModelState.AddModelError("", $"Error opening package: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OpenPackage");
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult DeletePackage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return RedirectToAction(nameof(UploadedPackages));
        }

        try
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, Path.GetFileName(fileName));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation($"Deleted package: {fileName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting package: {fileName}");
        }

        return RedirectToAction(nameof(UploadedPackages));
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
            var model = new PackageViewModel
            {
                BBSInfo = bbsInfo
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
            var messagePointers = _qwkService.GetForumMessagePointers(forumId);
            
            var model = new ForumViewModel
            {
                ForumId = forumId,
                MessagePointers = messagePointers
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
