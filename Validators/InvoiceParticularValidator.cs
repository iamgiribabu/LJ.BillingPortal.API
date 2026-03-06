using FluentValidation;
using LJ.BillingPortal.API.DTOs;

namespace LJ.BillingPortal.API.Validators;

public class CreateInvoiceParticularDtoValidator : AbstractValidator<CreateInvoiceParticularDto>
{
    public CreateInvoiceParticularDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500);

        RuleFor(x => x.HsnSac)
            .NotEmpty().WithMessage("HSN/SAC Code is required")
            .Matches(@"^\d{4,8}$").WithMessage("HSN/SAC must be numeric and 4-8 digits");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Rate cannot be negative");

        RuleFor(x => x.TaxableValue)
            .GreaterThanOrEqualTo(0).WithMessage("Taxable value cannot be negative");
    }
}

public class UpdateInvoiceParticularDtoValidator : AbstractValidator<UpdateInvoiceParticularDto>
{
    public UpdateInvoiceParticularDtoValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("Service ID must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500);

        RuleFor(x => x.HsnSac)
            .NotEmpty().WithMessage("HSN/SAC Code is required")
            .Matches(@"^\d{4,8}$");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Rate cannot be negative");

        RuleFor(x => x.TaxableValue)
            .GreaterThanOrEqualTo(0).WithMessage("Taxable value cannot be negative");
    }
}
