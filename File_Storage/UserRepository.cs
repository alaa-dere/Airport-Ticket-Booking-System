using TASK2.Models;

namespace TASK2.File_Storage;

public class UserRepository
{
    private const string FilePath = "users.csv";

    public List<User> GetAll()
    {
        EnsureFileExists();

        var users = new List<User>();
        var lines = File.ReadAllLines(FilePath).Skip(1);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = CsvUtility.ParseLine(line);

            if (columns.Length == 5 &&
                int.TryParse(columns[0], out var id) &&
                Enum.TryParse(columns[4], out UserRole role))
            {
                users.Add(new User
                {
                    Id = id,
                    Name = columns[1],
                    Email = columns[2],
                    Password = columns[3],
                    Role = role
                });
            }
        }

        return users;
    }

    public void AddUser(User user)
    {
        var users = GetAll();
        var maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;

        user.Id = maxId + 1;
        users.Add(user);
        SaveAll(users);
    }

    private static void SaveAll(List<User> users)
    {
        var lines = new List<string> { "Id,Name,Email,Password,Role" };

        lines.AddRange(users.Select(user => CsvUtility.ToLine(
            user.Id,
            user.Name,
            user.Email,
            user.Password,
            user.Role)));

        File.WriteAllLines(FilePath, lines);
    }

    private static void EnsureFileExists()
    {
        if (File.Exists(FilePath))
            return;

        var defaultUsers = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "System Manager",
                Email = "admin@airport.com",
                Password = "admin123",
                Role = UserRole.Manager
            }
        };

        SaveAll(defaultUsers);
    }
}
