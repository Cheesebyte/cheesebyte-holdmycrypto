using Cheesebyte.HoldMyCrypto.Services.Models;
using FluentValidation;

namespace Cheesebyte.HoldMyCrypto.Console.Validators;

/// <summary>
/// Validator for <see cref="TransactionOptions"/>.
/// </summary>
public class TransactionValidator : AbstractValidator<TransactionOptions>
{
    public TransactionValidator()
    {
        RuleFor(x => x.TimeStart).NotEmpty().WithMessage("Requires a start date for the first transaction");
        RuleFor(x => x.TargetSymbol).NotEmpty().WithMessage("Requires a base symbol, like EUR, USD or USDT");

        RuleFor(x => x.AssetBase).NotEmpty().WithMessage("Requires a base asset (e.g. 'BTC')");
        RuleFor(x => x.AssetQuote).NotEmpty().WithMessage("Requires a quote asset (e.g. 'USDT')");
    }
}