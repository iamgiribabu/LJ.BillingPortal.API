using FluentValidation;
using LJ.BillingPortal.API.DTOs;

namespace LJ.BillingPortal.API.Validators;

public class CreateCompleteInvoiceDtoValidator : AbstractValidator<CreateCompleteInvoiceDto>
{
    public CreateCompleteInvoiceDtoValidator()
    {
        RuleFor(x => x.ClientDetails)
            .NotNull().WithMessage("Client details are required")
            .SetValidator(new CreateClientDetailsDtoValidator());

        RuleFor(x => x.InvoiceDetails)
            .NotNull().WithMessage("Invoice details are required")
            .SetValidator(new CreateInvoiceDetailsDtoValidator());

        RuleFor(x => x.InvoiceParticulars)
            .NotEmpty().WithMessage("At least one invoice particular is required")
            .Must(p => p.All(x => x != null)).WithMessage("All invoice particulars must be valid");

        RuleForEach(x => x.InvoiceParticulars)
            .SetValidator(new CreateInvoiceParticularDtoValidator());
    }
}

public class GenerateInvoiceRequestValidator : AbstractValidator<GenerateInvoiceRequest>
{
    public GenerateInvoiceRequestValidator()
    {
        RuleFor(x => x.ClientDetails)
            .NotNull().WithMessage("Client details are required");

        RuleFor(x => x.InvoiceDetails)
            .NotNull().WithMessage("Invoice details are required");

        RuleFor(x => x.InvoiceParticulars)
            .NotEmpty().WithMessage("At least one invoice particular is required");
    }
}
