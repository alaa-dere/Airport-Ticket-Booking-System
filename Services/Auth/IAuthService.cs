using TASK2.Models;
namespace TASK2.Services.Auth
{
    public interface IAuthService
    {
        public User? Login(string email, string password);
        public User RegisterPassenger(string name, string email, string password);
    }
}    
