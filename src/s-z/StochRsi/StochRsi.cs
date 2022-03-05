namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // STOCHASTIC RSI
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StochRsiResult> GetStochRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // initialize
        List<RsiResult> rsiResults = GetRsi(quotes, rsiPeriods).ToList();
        List<StochRsiResult> results = new(rsiResults.Count);

        // convert rsi to quote format
        List<Quote> rsiQuotes = rsiResults
            .Where(x => x.Rsi != null)
            .Select(x => new Quote
            {
                Date = x.Date,
                High = (decimal)x.Rsi,
                Low = (decimal)x.Rsi,
                Close = (decimal)x.Rsi
            })
            .ToList();

        // get Stochastic of RSI
        List<StochResult> stoResults =
            GetStoch(rsiQuotes, stochPeriods, signalPeriods, smoothPeriods)
            .ToList();

        // compose
        for (int i = 0; i < rsiResults.Count; i++)
        {
            RsiResult r = rsiResults[i];
            int index = i + 1;

            StochRsiResult result = new()
            {
                Date = r.Date
            };

            if (index >= rsiPeriods + stochPeriods)
            {
                StochResult sto = stoResults[index - rsiPeriods - 1];

                result.StochRsi = sto.Oscillator;
                result.Signal = sto.Signal;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateStochRsi(
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods)
    {
        // check parameter arguments
        if (rsiPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                "RSI periods must be greater than 0 for Stochastic RSI.");
        }

        if (stochPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stochPeriods), stochPeriods,
                "STOCH periods must be greater than 0 for Stochastic RSI.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for Stochastic RSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smooth periods must be greater than 0 for Stochastic RSI.");
        }
    }
}
