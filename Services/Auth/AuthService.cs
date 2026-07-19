using System.ComponentModel.DataAnnotations;
using System.Security;
using TASK2.File_Storage.Users;
using TASK2.Extensions;
using TASK2.Models;

namespace TASK2.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public User? Login(Email email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            else if (user.Password != password)
                throw new UnauthorizedAccessException("Invalid password.");

            return user;
        }

        /// <inheritdoc />
        public User RegisterPassenger(RegisterPassengerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                !request.Name.IsValidSimpleValue() ||
                !request.Email.Value.IsValidSimpleValue() ||
                !request.Password.IsValidSimpleValue())
            {
                throw new ValidationException("Name, email, and password are required and must not contain commas or new lines.");
            }

            string name = request.Name.Trim();
            Email email = request.Email;

            if (_userRepository.GetUserByEmail(email) != null)
                throw new ValidationException("Email is already registered.");

            var user = new User
            {
                Name = name,
                Email = email.Value,
                Password = request.Password,
                Role = UserRole.Passenger
            };

            _userRepository.Add(user);
            return user;
        }
    }
}
