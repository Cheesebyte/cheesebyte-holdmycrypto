namespace Cheesebyte.HoldMyCrypto.Services.Models;

/// <summary>
/// An asset pair that combines base and quote info.
/// </summary>
/// <param name="Base">Base asset (e.g. 'BTC'). Also known as 'from'.</param>
/// <param name="Quote">Quote asset (e.g. 'USDT'). Also known as 'to'.</param>
internal record AssetPair(string Base, string Quote);