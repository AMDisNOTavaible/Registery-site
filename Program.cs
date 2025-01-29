using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Инициализация базы данных
string connectionString = "Data Source=Users.db";
using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();

    string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL
        )";
    var command = new SqliteCommand(createTableQuery, connection);
    command.ExecuteNonQuery();

    // Добавляем тестового пользователя
    string insertUserQuery = @"
        INSERT OR IGNORE INTO Users (Id, Username, Password)
        VALUES (1, 'admin', 'password')";
    var insertCommand = new SqliteCommand(insertUserQuery, connection);
    insertCommand.ExecuteNonQuery();
}

// Авторизация
app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    string username = form["username"];
    string password = form["password"];

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
        var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password", password);

        long userExists = (long)command.ExecuteScalar();
        if (userExists > 0)
        {
            context.Response.Redirect("/success.html");
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Неверное имя пользователя или пароль");
        }
    }
});

// Регистрация нового пользователя
app.MapPost("/register", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    string username = form["username"];
    string password = form["password"];

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        context.Response.StatusCode = 400; // Bad Request
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Имя пользователя и пароль не могут быть пустыми");
        return;
    }

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
        var checkCommand = new SqliteCommand(checkQuery, connection);
        checkCommand.Parameters.AddWithValue("@username", username);

        long userExists = (long)checkCommand.ExecuteScalar();
        if (userExists > 0)
        {
            context.Response.StatusCode = 409; // Conflict
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("Пользователь с таким именем уже существует");
            return;
        }

        string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
        var insertCommand = new SqliteCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@username", username);
        insertCommand.Parameters.AddWithValue("@password", password);
        insertCommand.ExecuteNonQuery();

        // Указываем кодировку UTF-8 в ContentType
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Пользователь успешно зарегистрирован");
    }
});

// Список пользователей
app.MapGet("/users", async (HttpContext context) =>
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        string query = "SELECT Username FROM Users";
        var command = new SqliteCommand(query, connection);
        var reader = command.ExecuteReader();

        StringBuilder users = new();
        while (reader.Read())
        {
            users.AppendLine(reader.GetString(0));
        }

        await context.Response.WriteAsync(users.ToString());
    }
});

// Сервинг статических файлов
app.UseDefaultFiles(); // index.html по умолчанию
app.UseStaticFiles();  // Для обслуживания HTML

app.Run();
