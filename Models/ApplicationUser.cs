using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Дополнительные свойства, если необходимо
    }

    public class ApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, password);
                return result; // Возвращает true, если пароль правильный
            }
            return false; // Пользователь не найден
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null; // Возвращаем true, если логин существует
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync(); // Get all users from the database
        }
    }
}