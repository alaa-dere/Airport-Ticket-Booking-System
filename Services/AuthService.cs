using TASK2.File_Storage;
using TASK2.Models;

namespace TASK2.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public User? Login(string email, string password)
        {
            return _userRepository.GetAll()
                .FirstOrDefault(user =>
                    user.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    user.Password == password);
        }

        public bool RegisterPassenger(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                !CsvUtility.IsValidSimpleValue(name) ||
                !CsvUtility.IsValidSimpleValue(email) ||
                !CsvUtility.IsValidSimpleValue(password))
            {
                return false;
            }

            var users = _userRepository.GetAll();

            if (users.Any(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return false;

            _userRepository.AddUser(new User
            {
                Name = name,
                Email = email,
                Password = password,
                Role = UserRole.Passenger
            });

            return true;
        }
    }
}
