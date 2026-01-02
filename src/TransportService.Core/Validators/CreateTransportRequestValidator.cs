using FluentValidation;
using TransportService.Core.DTOs;

namespace TransportService.Core.Validators;

public class CreateTransportRequestValidator : AbstractValidator<CreateTransportRequest>
{
    public CreateTransportRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ElasticSearchId)
            .NotEmpty()
            .WithMessage("ElasticSearchId is required")
            .MaximumLength(50)
            .WithMessage("ElasticSearchId must not exceed 50 characters");

        RuleFor(x => x.OfferId)
            .GreaterThan(0)
            .WithMessage("OfferId must be greater than 0");

        RuleFor(x => x.PurchaseId)
            .GreaterThan(0)
            .WithMessage("PurchaseId must be greater than 0");

        RuleFor(x => x.SellerId)
            .GreaterThan(0)
            .WithMessage("SellerId must be greater than 0");

        RuleFor(x => x.BuyerId)
            .GreaterThan(0)
            .WithMessage("BuyerId must be greater than 0");

        RuleFor(x => x.CarrierId)
            .GreaterThan(0)
            .WithMessage("CarrierId must be greater than 0");

        RuleFor(x => x.SellerZipCode)
            .NotEmpty()
            .WithMessage("SellerZipCode is required")
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("SellerZipCode must be a valid ZIP code format");

        RuleFor(x => x.BuyerZipCode)
            .NotEmpty()
            .WithMessage("BuyerZipCode is required")
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("BuyerZipCode must be a valid ZIP code format");

        RuleFor(x => x.ScheduleWindow)
            .NotNull()
            .WithMessage("ScheduleWindow is required");

        RuleFor(x => x.ScheduleWindow.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("ScheduleWindow StartDate must be in the future");

        RuleFor(x => x.ScheduleWindow.EndDate)
            .GreaterThan(x => x.ScheduleWindow.StartDate)
            .WithMessage("ScheduleWindow EndDate must be after StartDate");

        RuleFor(x => x.ScheduleWindow.ScheduledDate)
            .NotEmpty()
            .WithMessage("ScheduleWindow ScheduledDate is required")
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("ScheduleWindow ScheduledDate must be in the future");
    }
}