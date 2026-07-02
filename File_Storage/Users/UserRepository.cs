using TASK2.Models;

namespace TASK2.File_Storage.Users;
using TASK2.File_Storage.Parser;

public class UserRepository : IUserRepository
{
    private static readonly string FilePath = StoragePath.Resolve(AppConstants.UsersFileName);
    private static readonly IParser Parser = ParserFactory.GetParser(Path.GetExtension(FilePath).TrimStart('.'));
    private readonly List<User> _users;

    public UserRepository()
    {
        EnsureFileExists();
        _users = LoadUsersFromFile();
    }

    public IReadOnlyCollection<User> GetAll()
    {
        return _users.ToList();
    }

    public User? GetUserByEmail(string email)
    {
        return _users.FirstOrDefault(user =>
            user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    private List<User> LoadUsersFromFile()
    {
        var users = new List<User>();
        var lines = File.ReadAllLines(FilePath).Skip(1);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = Parser.ParseLine(line);

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

    public void Add(User user)
    {
        var maxId = _users.Count > 0 ? _users.Max(u => u.Id) : 0;

        user.Id = maxId + 1;
        _users.Add(user);
        WriteUsersToFile(_users);
    }

    private void EnsureFileExists()
    {
        if (File.Exists(FilePath))
            return;

        CreateDefaultUsersFile();
    }

    private void CreateDefaultUsersFile()
    {
        WriteUsersToFile(GetDefaultUsers());
    }

    private List<User> GetDefaultUsers()
    {
        return new List<User>
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
    }

    
    private static void WriteUsersToFile(List<User> users)
    {
        var lines = new List<string> { "Id,Name,Email,Password,Role" };

        lines.AddRange(users.Select(user => Parser.ToLine(
            user.Id,
            user.Name,
            user.Email,
            user.Password,
            user.Role)));

        File.WriteAllLines(FilePath, lines);
    }
}
