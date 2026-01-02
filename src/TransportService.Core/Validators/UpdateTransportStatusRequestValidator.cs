using FluentValidation;
using TransportService.Core.DTOs;

namespace TransportService.Core.Validators;

public class UpdateTransportStatusRequestValidator : AbstractValidator<UpdateTransportStatusRequest>
{
    public UpdateTransportStatusRequestValidator()
    {
        RuleFor(x => x.ElasticSearchId)
            .NotEmpty()
            .WithMessage("ElasticSearchId is required")
            .MaximumLength(50)
            .WithMessage("ElasticSearchId must not exceed 50 characters");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .MaximumLength(30)
            .WithMessage("Status must not exceed 30 characters");
    }
}