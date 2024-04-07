using FluentValidation;
using IPParser.Models;
using System.Globalization;
using System.Net;

namespace IPParser.Validators;

public class UserDataDtoValidator : AbstractValidator<UserDataDto>
{
    public UserDataDtoValidator()
    {
        RuleFor(userData => userData.FileLog)
            .NotEmpty().WithMessage("File log is required.")
            .Matches(@"^[a-zA-Z]:\\(?:[^\\\/:*?""<>|\r\n]+\\)*[^\\\/:*?""<>|\r\n]*$").WithMessage("Invalid file log format.");

        RuleFor(userData => userData.FileOutput)
            .NotEmpty().WithMessage("File output is required.")
            .Matches(@"^[a-zA-Z]:\\(?:[^\\\/:*?""<>|\r\n]+\\)*[^\\\/:*?""<>|\r\n]*$").WithMessage("Invalid file output format.");

        RuleFor(userData => userData.AddressStart)
            .Must(address => string.IsNullOrEmpty(address) || IPAddress.TryParse(address, out _))
            .WithMessage("Invalid IP address format.");

        RuleFor(userData => userData.AddressMask)
            .Must(mask => string.IsNullOrEmpty(mask) || int.TryParse(mask, out _))
            .WithMessage("Invalid address mask format.");

        RuleFor(userData => userData.TimeStart)
            .NotEmpty().WithMessage("Time start is required.")
            .Must(BeAValidDate).WithMessage("Invalid date format.");

        RuleFor(userData => userData.TimeEnd)
            .NotEmpty().WithMessage("Time end is required.")
            .Must(BeAValidDate).WithMessage("Invalid date format.");
    }

    private bool BeAValidDate(string date)
    {
        return DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}


