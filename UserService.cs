using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AuthApp.Models;

namespace AuthApp.Services;

public class UserService
{
    private readonly string _connectionString;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(string connectionString, UserManager<ApplicationUser> userManager)
    {
        _connectionString = connectionString;
        _userManager = userManager;
    }

    public async Task<bool> ValidateUser(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return false; // Пользователь не найден
        }

        return await _userManager.CheckPasswordAsync(user, password); // Проверяем пароль
    }

    private static bool VerifyPassword(string password, string storedHash, string salt)
    {
        using var hmac = new HMACSHA512(Convert.FromBase64String(salt));
        var computedHash = Convert.ToBase64String(
            hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }

    public async Task<IdentityResult> RegisterUser(string username, string password)
    {
        var user = new ApplicationUser { UserName = username };
        var result = await _userManager.CreateAsync(user, password);
        return result; // Return the result of the registration
    }
}
