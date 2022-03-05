namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // SIMPLE MOVING AVERAGE (on CLOSE price)
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateSma(lookbackPeriods);

        // initialize
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // calculate
        return bdList.CalcSma(lookbackPeriods);
    }

    // SIMPLE MOVING AVERAGE (on specified OHLCV part)
    /// <include file='./info.xml' path='indicator/type[@name="Custom"]/*' />
    ///
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateSma(lookbackPeriods);

        // initialize
        List<BasicD> bdList = quotes.ConvertToBasic(candlePart);

        // calculate
        return bdList.CalcSma(lookbackPeriods);
    }

    // internals
    private static IEnumerable<SmaResult> CalcSma(
        this List<BasicD> bdList,
        int lookbackPeriods)
    {
        // note: pre-validated
        // initialize
        List<SmaResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicD q = bdList[i];
            int index = i + 1;

            SmaResult result = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double sumSma = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    BasicD d = bdList[p];
                    sumSma += d.Value;
                }

                result.Sma = (decimal)sumSma / lookbackPeriods;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateSma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
