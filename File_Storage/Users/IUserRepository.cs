using TASK2.Models;

namespace TASK2.File_Storage.Users
{
    public interface IUserRepository
    {
        public IReadOnlyCollection<User> GetAll();
        public User? GetUserByEmail(string email);
        public void Add(User user);
    }
}