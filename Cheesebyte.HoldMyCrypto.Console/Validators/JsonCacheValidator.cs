using Cheesebyte.HoldMyCrypto.Caching.Models;
using FluentValidation;

namespace Cheesebyte.HoldMyCrypto.Console.Validators;

/// <summary>
/// Validator for <see cref="JsonCacheOptions"/>.
/// </summary>
public class JsonCacheValidator : AbstractValidator<JsonCacheOptions>
{
    public JsonCacheValidator()
    {
        RuleFor(x => x.BaseCachePath).NotEmpty().WithMessage("Requires a cache root path");
    }
}