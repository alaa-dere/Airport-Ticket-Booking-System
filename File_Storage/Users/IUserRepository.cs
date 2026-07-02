using TASK2.Models;

namespace TASK2.File_Storage.Users;

public interface IUserRepository
{
    IReadOnlyCollection<User> GetAll();
    User? GetUserByEmail(string email);
    void Add(User user);
}
