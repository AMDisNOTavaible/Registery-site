using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AuthApp.Data;
using AuthApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args); 

// Добавляем необходимые сервисы
builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
});

builder.Services.AddScoped<IdentityErrorDescriber, RussianIdentityErrorDescriber>();

var app = builder.Build();

// Создание базы данных и применение миграций
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByNameAsync("admin");
        
        if (admin == null)
        {
            var adminUser = new ApplicationUser { UserName = "admin" };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create admin user: {errors}");
            }
        }
        else
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(admin);
            await userManager.ResetPasswordAsync(admin, token, "Admin123!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        throw;
    }
}

// Конфигурация HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

// Эндпоинты
app.MapPost("/login", async (HttpContext context) => 
{
    var form = await context.Request.ReadFormAsync();
    string username = form["username"].ToString() ?? string.Empty;
    string password = form["password"].ToString() ?? string.Empty;

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        context.Response.Redirect("/login.html?error=empty");
        return;
    }

    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
    var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

    var user = await userManager.FindByNameAsync(username);
    if (user == null || !(await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false)).Succeeded)
    {
        context.Response.Redirect("/login.html?error=invalid");
        return;
    }

    context.Response.Redirect("/success.html");
});

app.MapPost("/register", async (HttpContext context) => 
{
    var form = await context.Request.ReadFormAsync();
    string username = form["username"].ToString() ?? string.Empty;
    string password = form["password"].ToString() ?? string.Empty;

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Имя пользователя и пароль обязательны");
        return;
    }

    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

    var existingUser = await userManager.FindByNameAsync(username);
    if (existingUser != null)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Пользователь с таким именем уже существует");
        return;
    }

    var user = new ApplicationUser { UserName = username };
    var result = await userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        context.Response.Redirect($"/PostRegistration.html?username={Uri.EscapeDataString(username)}");
    }
    else
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
});

app.MapGet("/users", async (HttpContext context) => 
{
    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
    var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

    if (!signInManager.IsSignedIn(context.User))
    {
        context.Response.StatusCode = 401;
        return;
    }

    var currentUser = await userManager.GetUserAsync(context.User);
    if (currentUser?.UserName != "admin")
    {
        context.Response.StatusCode = 403;
        return;
    }

    var users = userManager.Users.Select(u => u.UserName).ToList();
    
    context.Response.ContentType = "text/html; charset=utf-8";
    var html = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8' />
            <title>Список пользователей</title>
            <style>
                body { 
                    font-family: Arial, sans-serif; 
                    padding: 20px;
                    background-color: #1a1a1a;
                    color: #ffffff;
                }
                h1 { 
                    color: #4a9eff; 
                    text-align: center;
                }
                ul { 
                    list-style-type: none; 
                    padding: 0;
                    max-width: 600px;
                    margin: 20px auto;
                }
                li { 
                    padding: 10px; 
                    border-bottom: 1px solid #333;
                    background-color: #2d2d2d;
                    margin-bottom: 5px;
                    border-radius: 4px;
                }
                a { 
                    color: #4a9eff; 
                    text-decoration: none;
                    display: block;
                    text-align: center;
                    margin-top: 20px;
                }
                a:hover { 
                    text-decoration: underline; 
                }
            </style>
        </head>
        <body>
            <h1>Список пользователей</h1>
            <ul>" +
            string.Join("", users.Select(u => $"<li>{u}</li>")) +
            @"</ul>
            <a href='/'>На главную</a>
        </body>
        </html>";
    
    await context.Response.WriteAsync(html);
});

await app.RunAsync();
