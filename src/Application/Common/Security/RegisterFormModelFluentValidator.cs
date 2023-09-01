using CleanArchitecture.Blazor.Application.Common.Configurations;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class RegisterFormModelFluentValidator : AbstractValidator<RegisterFormModel>
{
    private readonly IStringLocalizer<RegisterFormModelFluentValidator> _localizer;
    private readonly IConfiguration _configuration;

    public RegisterFormModelFluentValidator(
        IStringLocalizer<RegisterFormModelFluentValidator> localizer,
        IConfiguration configuration
        )
    {
        _localizer = localizer;
        _configuration = configuration;
        var identitySettings = new IdentitySettings();
        configuration.GetSection(nameof(IdentitySettings)).Bind(identitySettings);
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(2, 100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress();
        RuleFor(p => p.Password).NotEmpty().WithMessage(_localizer["CannotBeEmpty"])
                  .MinimumLength(identitySettings.RequiredLength).WithMessage(string.Format(_localizer["MinLength"], identitySettings.RequiredLength))
                  .MaximumLength(identitySettings.MaxLength).WithMessage(_localizer["MaxLength"])
                  .Matches(identitySettings.RequireUpperCase ? @"[A-Z]+" : string.Empty).WithMessage(_localizer["MustContainUpperCase"])
                  .Matches(identitySettings.RequireLowerCase ? @"[a-z]+" : string.Empty).WithMessage(_localizer["MustContainLowerCase"])
                  .Matches(identitySettings.RequireDigit ? @"[0-9]+" : string.Empty).WithMessage(_localizer["MustContainDigit"])
                  .Matches(identitySettings.RequireNonAlphanumeric ? @"[\@\!\?\*\.]+" : string.Empty).WithMessage(_localizer["MustContainAlphanumericCharacter"]);
        RuleFor(x => x.ConfirmPassword)
             .Equal(x => x.Password);
        RuleFor(x => x.AgreeToTerms)
            .Equal(true);
        
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<RegisterFormModel>.CreateWithOptions((RegisterFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

