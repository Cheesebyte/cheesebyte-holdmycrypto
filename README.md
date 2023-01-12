# 💰 Hold My Crypto

> Proof of Concept: Track money to and from the crypto world with ledger-based transactions.

## Features

See sample output below. Not based on real data or calculations.
```cmd
---------------------------------------------------------------------------------------
* Totals on 12/01/2023
---------------------------------------------------------------------------------------
        
  Fees:            € 3,45
  Added:          € 42,12
  Expenses:          None

| Category | Quantity     | Asset | In EUR    | Date  | Side   | Source  | Tx ID      |
|-------------------------------------------------------------------------------------|
| Asset    | ₿ 0.12345678 | BTC   | € 1234,56 | 20:12 | Credit | Binance | 1234567890 |
| Expense  |    ₮ 1345,67 | USDT  | € 1234,56 | 20:12 | Debit  | Binance | 1234567890 |
| Fee      | ₿ 0.00012345 | BTC   |    € 3,45 | 20:12 | Debit  | Binance | 1234567890 |
| Asset    | ₿ 0.01234567 | BTC   |   € 30,45 | 21:32 | Credit | Bybit   | 2345678901 |
| ...      |        ₿ ... | BTC   |   € ..... | ..:.. | ...... | Bybit   | 2345678901 |
```

### ☝🏻 Insight

* Convert from any input (e.g. BTC) to output network (e.g. USD, EUR)
* Find what happened to monetary value transferred between networks
* Discover how much profit or loss was made within a configurable time period
* Get to know whether past trading strategies made up for profit in terms of hidden losses

### 👍🏻 Exchanges

Support for the following exchanges or other sources is currently available. Each column indicates the type of transaction supported.

|                                            | Withdraw | Deposit | Order | Trade | Price |
|--------------------------------------------|:--------:|:-------:|:-----:|:-----:|:-----:|
| [Binance](https://binance.com)             |    x     |    x    |   x   |   x   |       |
| [Bybit](https://bybit.com)                 |    x     |    x    |       |       |       |
| [BL3P](https://bl3p.eu)                    |    x     |    x    |       |       |       |
| [CryptoCompare](https://cryptocompare.com) |          |         |       |       |   x   |

# Setup

Create an `appsettings.json` file in the `Cheesebyte.HoldMyCrypto.Console` project directory.

```json
{
  "JsonCache": {
    "BaseCachePath": "/Path/To/YourAccount/HoldMyBudget/Cache"
  },
  "AssetExtract": {
    "TimeStart": "1/1/2021 12:00:00 AM",
    "TimeEnd": "1/1/2023 12:00:00 AM",
    "AssetBase": "BTC",
    "AssetQuote": "USDT",
    "TargetSymbol": "EUR"
  }
}
```

Set up API access with `READ` permissions for your accounts at [Binance](https://www.binance.com/en/my/settings/api-management), [Bybit](https://bybit.com/app/user/api-management), [BL3P](https://bl3p.eu/security) or [CryptoCompare](https://www.cryptocompare.com/cryptopian/api-keys).

Create an `appvault.json` file in the `Cheesebyte.HoldMyCrypto.Console` project directory and add settings for each exchange to access.

```json
{
  "Binance": {
    "Key": "your_public_key_from_binance",
    "Secret": "your_secret_key_from_binance"
  }
}
```

Another method is to set up environment variables.

```shell
> echo "Semicolon in section name translates to a double underscore"
> export Name__Of__Section=secret_value
```

Supported sections for API settings:

```cmd
Exchanges:Binance:Secret
Exchanges:Binance:Key
Exchanges:Bybit:Secret
Exchanges:Bybit:Key
Exchanges:BL3P:Secret
Exchanges:BL3P:Key
Exchanges:CryptoCompare:ApiKey
```

Optionally, user secrets are supported for `DEBUG` builds.
```shell
> dotnet user-secrets set "Name:Of:Section" "value"
```

Build the full solution with projects via the IDE or the `dotnet` tool.

# Run

> ⚠️ Rate limits might *not* be handled correctly yet. This is a work in progress, likely to contain bugs. Use it on your own risk and run with a `testnet` first.

Choose one of the following configurations to run the program with.
* `Console: force update`
* `Console: use cache`