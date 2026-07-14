namespace TASK2.Models;

public class RegisterPassengerRequest
{
    public required string Name { get; set; }
    public required Email Email { get; set; }
    public required string Password { get; set; }
}
