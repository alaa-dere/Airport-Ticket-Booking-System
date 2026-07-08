using System.ComponentModel.DataAnnotations;

namespace TASK2.Validation;

public class FutureOrTodayAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is DateTime date && date.Date >= DateTime.Today;
    }
}