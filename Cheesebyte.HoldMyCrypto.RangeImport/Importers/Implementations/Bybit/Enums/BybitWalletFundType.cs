namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Enums;

/// <summary>
/// https://bybit-rangeImporter.github.io/docs/futuresV2/inverse/?console#wallet-fund-type-wallet_fund_type-type
/// </summary>
public enum BybitWalletFundType
{
    Deposit,
    Withdraw,
    RealisedPNL,
    Commission,
    Refund,
    Prize,
    ExchangeOrderWithdraw,
    ExchangeOrderDeposit,
    ReturnServiceCash,
    Insurance,
    SubMember,
    Coupon,
    AccountTransfer,
    CashBack,
}