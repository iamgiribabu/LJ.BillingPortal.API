using FluentValidation;
using LJ.BillingPortal.API.DTOs;

namespace LJ.BillingPortal.API.Validators;

public class CreateInvoiceDetailsDtoValidator : AbstractValidator<CreateInvoiceDetailsDto>
{
    public CreateInvoiceDetailsDtoValidator()
    {
        RuleFor(x => x.InvoiceDate)
            .NotEmpty().WithMessage("Invoice date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Invoice date cannot be in the future");

        RuleFor(x => x.PlaceOfSupply)
            .NotEmpty().WithMessage("Place of supply is required")
            .MaximumLength(100);

        RuleFor(x => x.PoNumber)
            .NotEmpty().WithMessage("PO Number is required")
            .MaximumLength(50);

        RuleFor(x => x.CraneReg)
            .NotEmpty().WithMessage("Crane Registration is required")
            .MaximumLength(50);

        RuleFor(x => x.TotalAmountBeforeTax)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount before tax must be non-negative");

        RuleFor(x => x.Cgst)
            .GreaterThanOrEqualTo(0).WithMessage("CGST cannot be negative");

        RuleFor(x => x.Sgst)
            .GreaterThanOrEqualTo(0).WithMessage("SGST cannot be negative");

        RuleFor(x => x.Igst)
            .GreaterThanOrEqualTo(0).WithMessage("IGST cannot be negative");

        RuleFor(x => x.NetAmountAfterTax)
            .GreaterThanOrEqualTo(0).WithMessage("Net amount after tax must be non-negative");
    }
}

public class UpdateInvoiceDetailsDtoValidator : AbstractValidator<UpdateInvoiceDetailsDto>
{
    public UpdateInvoiceDetailsDtoValidator()
    {
        RuleFor(x => x.InvoiceNumber)
            .NotEmpty().WithMessage("Invoice number is required")
            .MaximumLength(50);

        RuleFor(x => x.InvoiceDate)
            .NotEmpty().WithMessage("Invoice date is required");

        RuleFor(x => x.PlaceOfSupply)
            .NotEmpty().WithMessage("Place of supply is required");

        RuleFor(x => x.TotalAmountBeforeTax)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.NetAmountAfterTax)
            .GreaterThanOrEqualTo(0);
    }
}
