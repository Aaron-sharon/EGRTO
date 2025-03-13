using System.Text.RegularExpressions;
using Aaronbackend;

public class VehicleValidator : AbstractValidator<Vehicle>
{
    public VehicleValidator()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("License Plate is required.")
            .Must(IsValidLicensePlate).WithMessage("Invalid License Plate format. Expected format: AA20AA2020.");

        RuleFor(v => v.Model)
            .NotEmpty().WithMessage("Model is required.")
            .Matches(@"^\d{4}$").WithMessage("Invalid Model. Expected format: 2000.");

        RuleFor(v => v.Owner)
            .NotEmpty().WithMessage("Owner name is required.")
            .Matches(@"^[A-Za-z.\s_-]+$").WithMessage("Invalid Owner name format.");

        RuleFor(v => v.OwnerAddress)
            .NotEmpty().WithMessage("Owner address is required.")
            .Matches(@"^[A-Za-z0-9\s\[\]()_.-]+$").WithMessage("Invalid Address format.");

        RuleFor(v => v.OwnerContactNumber)
            .NotEmpty().WithMessage("Contact number is required.")
            .Matches(@"^\d{10}$").WithMessage("Field should have 10 Digits.");

        RuleFor(v => v.OwnerEmail)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,}$").WithMessage("Invalid Email format.");

        RuleFor(v => v.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }

    // Custom License Plate Validation Logic
    private bool IsValidLicensePlate(string licensePlate)
    {
        var pattern = @"^[A-Z]{2}\d{2}[A-Z]{2}\d{4}$";
        return Regex.IsMatch(licensePlate, pattern);
    }
}
