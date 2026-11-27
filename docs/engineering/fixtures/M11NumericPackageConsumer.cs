using System.Numerics;
using System.Security.Cryptography;
using dw.quantities;
using Dw.Tools.Math.Statistics;

internal static class M11NumericChecks
{
    internal static void Run(string quantityDllSha)
    {
        var assembly = typeof(ExactRational).Assembly;
        Need(assembly.GetName().Name == "dw.quantities" &&
            Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(assembly.Location))) == quantityDllSha,
            "Loaded Quantities identity differs.");
        var rational = new ExactRational(3, 2);
        Need(rational + rational == new ExactRational(3, 1), "Historical rational golden differs.");
        var integral = ExactRationalBudget.TryScaleToInt64(rational, new BigInteger(2), 64);
        var nonintegral = ExactRationalBudget.TryScaleToInt64(rational, BigInteger.One, 64);
        Need(integral.Status == ExactInt64ScaleStatus.Success && integral.Value == 3 &&
            nonintegral.Status == ExactInt64ScaleStatus.NonIntegral && nonintegral.Value is null,
            "Exact scaling contract differs.");
        var stream = new StreamingMoments64(3);
        stream.Add(1d);
        stream.Add(3d);
        var restored = StreamingMoments64.FromSnapshot(stream.Snapshot());
        stream.Add(5d);
        restored.Add(5d);
        var moments = restored.GetResult();
        Need(restored.Snapshot() == stream.Snapshot() && moments.Count == 3 &&
            moments.Mean == 3d && moments.SampleVariance == 4d &&
            moments.PopulationVariance == 8d / 3d, "Moments replay or golden differs.");
        var values = Enumerable.Range(0, 5000).Select(i => (double)i);
        Need(QuantileStatistics64.Evaluate(values, 0.5d, QuantileConvention64.Type7, 5000) == 2499.5d &&
            QuantileStatistics64.Evaluate(values, 0.5d, QuantileConvention64.NearestRank, 5000) == 2499d,
            "Larger finite quantile differs.");
        var exact = Enumerable.Range(0, 5000).Select(i => new ExactRational(i, 1));
        Need(QuantileStatistics64.EvaluateExact(exact, new ExactRational(1, 2), QuantileConvention64.Type7, 5000) ==
            new ExactRational(4999, 2), "Larger exact quantile differs.");
        try
        {
            _ = QuantileStatistics64.Evaluate(values, 0.5d, QuantileConvention64.Type7);
            throw new InvalidDataException("Legacy count refusal missing.");
        }
        catch (ArgumentOutOfRangeException) { }
        Console.WriteLine("MATH-M11-BINARY|quantitiesDll=" + quantityDllSha + "|scale=PASS|moments=PASS|quantiles=PASS|legacy=PASS|PASS");
    }

    private static void Need(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
