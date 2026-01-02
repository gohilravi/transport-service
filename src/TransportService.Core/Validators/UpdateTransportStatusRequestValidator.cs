using FluentValidation;
using TransportService.Core.DTOs;

namespace TransportService.Core.Validators;

public class UpdateTransportStatusRequestValidator : AbstractValidator<UpdateTransportStatusRequest>
{
    public UpdateTransportStatusRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .MaximumLength(30)
            .WithMessage("Status must not exceed 30 characters");
    }
}