using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AuthApp.Models;
using AuthApp.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AuthApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(ApplicationUserService userService, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { success = false, message = "Введите имя пользователя и пароль" });
        }

        var isValid = await _userService.ValidateUserAsync(request.Username, request.Password);
        if (isValid)
        {
            return Ok(new { success = true });
        }

        return Unauthorized(new { success = false, message = "Неверное имя пользователя или пароль" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Имя пользователя и пароль обязательны" });
        }

        var user = new ApplicationUser { UserName = request.Username };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Ok(new { success = true });
        }

        // Формируем список ошибок в столбик с маркерами
        var errorMessage = result.Errors
            .Select(e => "• " + e.Description)
            .ToList();

        return BadRequest(new { message = string.Join("\n", errorMessage) });
    }
}

public record LoginRequest(string? Username, string? Password);
public record RegisterRequest(string? Username, string? Password); 
