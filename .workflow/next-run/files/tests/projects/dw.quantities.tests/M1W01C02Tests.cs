using System.Globalization;
using System.Numerics;
using System.Reflection;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C02Tests
{
    public const string BoundaryRedMarker = "M1-W01-C02-T2 RED: binary64 and decimal boundary APIs are absent";

    [TestMethod]
    public void BoundaryContractsRequireBothInstalledTypes()
    {
        var types = typeof(ExactRational).Assembly.GetTypes();
        Assert.IsTrue(types.Count(t => t.Name == "ExactBinaryNumber") == 1 &&
            types.Count(t => t.Name == "ExactDecimalFormatter") == 1,
            BoundaryRedMarker);
    }

    [TestMethod]
    public void Binary64TransferPreservesTheReceivedExactValue()
    {
        Assert.AreEqual(new ExactRational(1, 2), FromDouble(0.5d));
        Assert.AreEqual(new ExactRational(
            BigInteger.Parse("3602879701896397", CultureInfo.InvariantCulture),
            BigInteger.Parse("36028797018963968", CultureInfo.InvariantCulture)), FromDouble(0.1d));
        Assert.AreNotEqual(new ExactRational(1, 10), FromDouble(0.1d),
            "The received binary64 0.1 is not decimal 1/10.");
        Assert.AreEqual(new ExactRational(-3, 2), FromDouble(-1.5d));
    }

    [TestMethod]
    public void NonFiniteBinary64ValuesAreRejected()
    {
        foreach (var value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            Assert.IsTrue(RejectsBinary(value), "Non-finite binary64 must be rejected, not rounded or approximated.");
        }
    }

    [TestMethod]
    public void SubnormalAndSignedZeroSemanticsAreExplicitAndExact()
    {
        var smallest = new ExactRational(BigInteger.One, BigInteger.One << 1074);
        Assert.AreEqual(smallest, FromDouble(double.Epsilon));
        Assert.AreEqual(new ExactRational(-1, BigInteger.One << 1074), FromDouble(-double.Epsilon));
        Assert.AreEqual(ExactRational.Zero, FromDouble(0.0d));
        Assert.AreEqual(ExactRational.Zero, FromDouble(BitConverter.Int64BitsToDouble(unchecked((long)0x8000000000000000UL))),
            "Signed zero maps to the same rational value; the sign bit is not preserved.");
    }

    [TestMethod]
    public void FormatterRoundsDisplayOnlyAndRetainsExactInput()
    {
        var positive = new ExactRational(5, 4);
        var negative = new ExactRational(-5, 4);
        var beforePositive = positive.ToFractionString();
        var beforeNegative = negative.ToFractionString();
        var (formatter, modeType, format) = Formatter();
        var modes = Enum.GetValues(modeType).Cast<object>().ToArray();
        Assert.IsTrue(modes.Length > 0, "The formatter must declare at least one rounding mode.");
        foreach (var mode in modes)
        {
            var displayedPositive = Format(format, positive, 1, mode);
            var displayedNegative = Format(format, negative, 1, mode);
            Assert.IsTrue(displayedPositive is "1.2" or "1.3",
                "Rounding +1.25 at one place must produce either adjacent decimal tie.");
            Assert.IsTrue(displayedNegative is "-1.2" or "-1.3",
                "Rounding -1.25 at one place must produce either adjacent decimal tie.");
            Assert.AreEqual("1.25", Format(format, positive, 2, mode),
                "Exact decimal display at sufficient precision must not change digits.");
            Assert.AreEqual("-1.25", Format(format, negative, 2, mode));
            Console.WriteLine("Observed DecimalRoundingMode " + mode + ": +1.25 -> " +
                displayedPositive + "; -1.25 -> " + displayedNegative + " at one place.");
        }
        Assert.AreEqual(beforePositive, positive.ToFractionString());
        Assert.AreEqual(beforeNegative, negative.ToFractionString());
        Assert.AreEqual(typeof(ExactRational).Assembly, formatter.Assembly);
    }

    [TestMethod]
    public void FormatterRespectsPublishedPrecisionLimit()
    {
        var (formatter, modeType, format) = Formatter();
        var maxField = formatter.GetField("MaximumDecimalPlaces", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(maxField);
        var max = Convert.ToInt32(maxField.GetRawConstantValue(), CultureInfo.InvariantCulture);
        Assert.AreEqual(100, max);
        var mode = Enum.GetValues(modeType).Cast<object>().First();
        var input = new ExactRational(1, 3);
        var within = Format(format, input, max, mode);
        Assert.IsFalse(string.IsNullOrEmpty(within));
        Assert.IsTrue(RejectsFormatting(format, input, -1, mode),
            "Negative decimal precision must be rejected.");
        Assert.IsTrue(RejectsFormatting(format, input, max + 1, mode),
            "Precision beyond MaximumDecimalPlaces must be rejected.");
        Assert.AreEqual(new ExactRational(1, 3), input,
            "Display precision must not mutate the exact rational.");
    }

    private static ExactRational FromDouble(double value)
    {
        var type = typeof(ExactRational).Assembly.GetTypes().Single(t => t.Name == "ExactBinaryNumber");
        var method = type.GetMethod("FromDouble", BindingFlags.Public | BindingFlags.Static,
            binder: null, types: [typeof(double)], modifiers: null);
        Assert.IsNotNull(method, "The observed AURA API is FromDouble(double).");
        var result = method.Invoke(null, [value]);
        Assert.IsInstanceOfType<ExactRational>(result);
        return (ExactRational)result!;
    }

    private static bool RejectsBinary(double value)
    {
        try { _ = FromDouble(value); return false; }
        catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException or ArithmeticException or InvalidOperationException)
        { return true; }
    }

    private static (Type Type, Type Mode, MethodInfo Format) Formatter()
    {
        var type = typeof(ExactRational).Assembly.GetTypes().Single(t => t.Name == "ExactDecimalFormatter");
        var mode = typeof(ExactRational).Assembly.GetTypes().Single(t => t.Name == "DecimalRoundingMode");
        Assert.IsTrue(mode.IsEnum);
        var method = type.GetMethod("Format", BindingFlags.Public | BindingFlags.Static,
            binder: null, types: [typeof(ExactRational), typeof(int), mode], modifiers: null);
        Assert.IsNotNull(method, "The observed AURA API is Format(ExactRational,int,DecimalRoundingMode).");
        Assert.AreEqual(typeof(string), method.ReturnType);
        return (type, mode, method);
    }

    private static string Format(MethodInfo format, ExactRational number, int places, object rounding)
    {
        var result = format.Invoke(null, [number, places, rounding]);
        Assert.IsInstanceOfType<string>(result);
        return (string)result!;
    }

    private static bool RejectsFormatting(MethodInfo format, ExactRational number, int places, object rounding)
    {
        try { _ = Format(format, number, places, rounding); return false; }
        catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException or ArithmeticException or InvalidOperationException)
        { return true; }
    }
}
