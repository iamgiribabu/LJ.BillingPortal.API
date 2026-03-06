using FluentValidation;
using LJ.BillingPortal.API.DTOs;

namespace LJ.BillingPortal.API.Validators;

public class CreateClientDetailsDtoValidator : AbstractValidator<CreateClientDetailsDto>
{
    public CreateClientDetailsDtoValidator()
    {
        RuleFor(x => x.BilledToName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(255).WithMessage("Company name cannot exceed 255 characters");

        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address Line 1 is required")
            .MaximumLength(500).WithMessage("Address Line 1 cannot exceed 500 characters");

        RuleFor(x => x.AddressLine2)
            .NotEmpty().WithMessage("Address Line 2 is required")
            .MaximumLength(500).WithMessage("Address Line 2 cannot exceed 500 characters");

        RuleFor(x => x.AddressLine3)
            .NotEmpty().WithMessage("Address Line 3 is required")
            .MaximumLength(500).WithMessage("Address Line 3 cannot exceed 500 characters");

        RuleFor(x => x.Gstin)
            .NotEmpty().WithMessage("GSTIN is required")
            .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            .WithMessage("GSTIN format is invalid");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters");

        RuleFor(x => x.StateCode)
            .NotEmpty().WithMessage("State Code is required")
            .MaximumLength(2).WithMessage("State Code must be 2 characters");
    }
}

public class UpdateClientDetailsDtoValidator : AbstractValidator<UpdateClientDetailsDto>
{
    public UpdateClientDetailsDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Client ID must be greater than 0");

        RuleFor(x => x.BilledToName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(255).WithMessage("Company name cannot exceed 255 characters");

        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address Line 1 is required")
            .MaximumLength(500);

        RuleFor(x => x.AddressLine2)
            .NotEmpty().WithMessage("Address Line 2 is required")
            .MaximumLength(500);

        RuleFor(x => x.AddressLine3)
            .NotEmpty().WithMessage("Address Line 3 is required")
            .MaximumLength(500);

        RuleFor(x => x.Gstin)
            .NotEmpty().WithMessage("GSTIN is required")
            .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            .WithMessage("GSTIN format is invalid");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100);

        RuleFor(x => x.StateCode)
            .NotEmpty().WithMessage("State Code is required")
            .MaximumLength(2);
    }
}
