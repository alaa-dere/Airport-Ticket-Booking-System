namespace TASK2.Models;

public class RegisterPassengerRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}