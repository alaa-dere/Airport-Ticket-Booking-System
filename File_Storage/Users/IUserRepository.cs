using TASK2.Models;

namespace TASK2.File_Storage.Users
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets all stored users.
        /// </summary>
        /// <returns>All users.</returns>
        public IReadOnlyCollection<User> GetAll();

        /// <summary>
        /// Gets a user by email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The matching user, or null when no user is found.</returns>
        public User? GetUserByEmail(string email);

        /// <summary>
        /// Adds a new user to storage.
        /// </summary>
        /// <param name="user">The user to add.</param>
        public void Add(User user);
    }
}