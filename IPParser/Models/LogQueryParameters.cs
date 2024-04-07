using FluentValidation;
using IPParser.Validators;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace IPParser.Models;

public readonly struct LogQueryParameters
{
    public string FileLog { get; }

    public string FileOutput { get; }

    public IPAddress? AddressStart { get; }

    public int? AddressMask { get; }

    public DateOnly TimeStart { get; }

    public DateOnly TimeEnd { get; }

    public LogQueryParameters(UserDataDto userDataDto)
    {
        var validator = new UserDataDtoValidator();
        var validationResult = validator.Validate(userDataDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException($"Validation failed", validationResult.Errors);
        }
        FileLog = userDataDto.FileLog;
        FileOutput = userDataDto.FileOutput;

        // Преобразование строки IP-адреса в объект IPAddress. Если строка пуста, AddressStart будет null.
        AddressStart = !string.IsNullOrEmpty(userDataDto.AddressStart)
            ? IPAddress.Parse(userDataDto.AddressStart)
            : null;

        // Преобразование строки маски в число. Если строка пуста, AddressMask будет null.
        AddressMask = int.TryParse(userDataDto.AddressMask, out var mask)
            ? mask
            : null;

        var culture = CultureInfo.InvariantCulture;
        TimeStart = DateOnly.ParseExact(userDataDto.TimeStart, "dd.MM.yyyy", culture);
        TimeEnd = DateOnly.ParseExact(userDataDto.TimeEnd, "dd.MM.yyyy", culture);
    }

}