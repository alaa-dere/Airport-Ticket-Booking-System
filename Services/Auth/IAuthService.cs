using TASK2.Models;
namespace TASK2.Services.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The authenticated user.</returns>
        public User? Login(string email, string password);

        /// <summary>
        /// Registers a new passenger user.
        /// </summary>
        /// <param name="request">The passenger registration details.</param>
        /// <returns>The registered passenger user.</returns>
        public User RegisterPassenger(RegisterPassengerRequest request);
    }
}