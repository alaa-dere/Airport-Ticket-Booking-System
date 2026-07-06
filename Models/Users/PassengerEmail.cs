namespace TASK2.Models;

public readonly record struct PassengerEmail
{
    public PassengerEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Passenger email is required.", nameof(value));

        Value = value.Trim();
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
}
