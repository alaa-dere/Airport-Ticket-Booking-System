using TASK2.File_Storage.Parser;
using TASK2.File_Storage.Users;

using TASK2.Models;

namespace TASK2.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private static readonly IParser Parser = ParserFactory.GetParser(ParserFactory.CsvParserType);

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? Login(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);

            if (user == null || user.Password != password)
                return null;

            return user;
        }

        public bool RegisterPassenger(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                !Parser.IsValidSimpleValue(name) ||
                !Parser.IsValidSimpleValue(email) ||
                !Parser.IsValidSimpleValue(password))
            {
                return false;
            }

            if (_userRepository.GetUserByEmail(email) != null)
                return false;

            _userRepository.Add(new User
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