namespace TASK2.Models
{
    public class Passenger
    {
        public Passenger(Email email, string name, string phone)
        {
            Email = email;
            Name = name;
            Phone = phone;
        }

        public Email Email { get; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}