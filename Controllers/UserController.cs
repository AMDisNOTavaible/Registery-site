using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Hosting;

public class UserController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "users.html");
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound($"File not found at path: {filePath}");
        }
        return PhysicalFile(filePath, "text/html");
    }

    // Ваши действия для управления пользователями
} 