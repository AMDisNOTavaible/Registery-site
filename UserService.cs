using Microsoft.Data.Sqlite;

public class UserService
{
    private readonly string _connectionString;

    public UserService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool ValidateUser(string username, string password)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            long userExists = (long)command.ExecuteScalar();
            return userExists > 0;
        }
    }

    public List<string> GetUsers()
    {
        List<string> users = new();

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Username FROM Users";
            var command = new SqliteCommand(query, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(reader.GetString(0));
            }
        }

        return users;
    }
}
