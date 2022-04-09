# C# API Wrapper for [Fixer.io](https://fixer.io)

> Fixer.io provides JSON API for foreign exchange rates and currency conversion

## Usage

---

```csharp
using FixerIoApiWrapper;

var wrapper = new FixerApiWrapper("API_KEY_HERE", new()
{
#if DEBUG
    EnableApiResponseLogging = true,
#else
    EnableApiResponseLogging = false,
#endif
});

// Get all symbols
var symbols = await wrapper.GetSymbolsAsync();

// Get latest conversion rates
var latestRates = await wrapper.GetLatestRatesAsync();

// Get latest conversion rates from USD to EUR and BDT
var latestRatesForDefinedCurrencies = await wrapper.GetLatestRatesAsync("USD", new[] { "EUR", "BDT" });

// Convert from one currency to another
var convertUsdToEur = await wrapper.ConvertAsync("USD", "EUR", 100);

// Convert from one currency to another with the convertion rate of the previous day
var convertUsdToEurHistorical = await wrapper.ConvertAsync("USD", "EUR", 100, DateTime.UtcNow.AddDays(-1));
```