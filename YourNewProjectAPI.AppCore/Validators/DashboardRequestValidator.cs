using FluentValidation;
using YourNewProjectAPI.AppCore.Dto;

namespace YourNewProjectAPI.AppCore.Validators;

public class DashboardRequestValidator : AbstractValidator<DashboardRequestDto>
{
    public DashboardRequestValidator()
    {
        // Module Name cannot be blank or null
        RuleFor(x => x.ModuleName)
            .NotEmpty().WithMessage("Module name is required.")
            .MaximumLength(50).WithMessage("Module name cannot exceed 50 characters.");

        // Year must be a realistic revenue reporting year
        RuleFor(x => x.Year)
            .InclusiveBetween(1947, 2100).WithMessage("Please enter a valid reporting year.");
    }
}
