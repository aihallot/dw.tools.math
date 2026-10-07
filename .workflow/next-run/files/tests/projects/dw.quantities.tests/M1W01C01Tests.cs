using System.Globalization;
using System.Numerics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C01Tests
{
    private const string TypeName = "Dw.Quantities.ExactRational";

    [TestMethod]
    public void ExactRationalTypeIsMaterialized()
    {
        Assert.IsNotNull(ExactRationalType());
    }

    [TestMethod]
    public void OneThirdPlusOneSixthIsOneHalf()
    {
        var left = CreateFraction(1, 3);
        var right = CreateFraction(1, 6);
        var add = ExactRationalType().GetMethod(
            "op_Addition",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [ExactRationalType(), ExactRationalType()],
            modifiers: null);
        Assert.IsNotNull(add, "ExactRational must expose exact addition.");
        var sum = add.Invoke(null, [left, right]);
        Assert.IsNotNull(sum);
        AssertRatio(sum, 1, 2);
    }

    [TestMethod]
    public void ZeroDenominatorIsRejected()
    {
        try
        {
            _ = CreateFraction(1, 0);
            Assert.Fail("ExactRational must reject a zero denominator.");
        }
        catch (AssertFailedException)
        {
            throw;
        }
        catch
        {
        }
    }

    [TestMethod]
    public void NegativeDenominatorIsCanonicalized()
    {
        AssertRatio(CreateFraction(2, -4), -1, 2);
    }

    [TestMethod]
    public void NegativeMixedNumberRepresentsNegativeThreeHalves()
    {
        var mixed = TryParseMixed("-1 1/2") ?? TryCreateMixed(-1, 1, 2);
        Assert.IsNotNull(mixed, "ExactRational must expose a mixed-number construction or parse path.");
        AssertRatio(mixed, -3, 2);
    }

    private static Type ExactRationalType() =>
        Assembly.Load("dw.quantities").GetType(TypeName, throwOnError: true, ignoreCase: false)
        ?? throw new AssertFailedException(TypeName + " was not found.");

    private static object CreateFraction(long numerator, long denominator)
    {
        var type = ExactRationalType();
        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length != 2 || !parameters.All(p => IsIntegralLike(p.ParameterType)))
                continue;
            return Invoke(() => constructor.Invoke([
                ConvertIntegral(numerator, parameters[0].ParameterType),
                ConvertIntegral(denominator, parameters[1].ParameterType)
            ]));
        }

        foreach (var name in new[] { "Create", "FromFraction", "FromParts", "Of" })
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == name && m.ReturnType == type))
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 2 || !parameters.All(p => IsIntegralLike(p.ParameterType)))
                    continue;
                return Invoke(() => method.Invoke(null, [
                    ConvertIntegral(numerator, parameters[0].ParameterType),
                    ConvertIntegral(denominator, parameters[1].ParameterType)
                ])!);
            }
        }

        throw new AssertFailedException("No public two-part ExactRational construction path was found.");
    }

    private static object? TryParseMixed(string text)
    {
        var type = ExactRationalType();
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "Parse" && m.ReturnType == type))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                return Invoke(() => method.Invoke(null, [text])!);
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && typeof(IFormatProvider).IsAssignableFrom(parameters[1].ParameterType))
                return Invoke(() => method.Invoke(null, [text, CultureInfo.InvariantCulture])!);
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "TryParse" && m.ReturnType == typeof(bool)))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 2 &&
                parameters[0].ParameterType == typeof(string) &&
                parameters[1].IsOut &&
                parameters[1].ParameterType.GetElementType() == type)
            {
                var args = new object?[] { text, null };
                if ((bool)Invoke(() => method.Invoke(null, args)!)!)
                    return args[1];
            }

            if (parameters.Length == 3 &&
                parameters[0].ParameterType == typeof(string) &&
                typeof(IFormatProvider).IsAssignableFrom(parameters[1].ParameterType) &&
                parameters[2].IsOut &&
                parameters[2].ParameterType.GetElementType() == type)
            {
                var args = new object?[] { text, CultureInfo.InvariantCulture, null };
                if ((bool)Invoke(() => method.Invoke(null, args)!)!)
                    return args[2];
            }
        }

        return null;
    }

    private static object? TryCreateMixed(long whole, long numerator, long denominator)
    {
        var type = ExactRationalType();
        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length != 3 || !parameters.All(p => IsIntegralLike(p.ParameterType)))
                continue;
            return Invoke(() => constructor.Invoke([
                ConvertIntegral(whole, parameters[0].ParameterType),
                ConvertIntegral(numerator, parameters[1].ParameterType),
                ConvertIntegral(denominator, parameters[2].ParameterType)
            ]));
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.ReturnType == type && m.Name.Contains("Mixed", StringComparison.OrdinalIgnoreCase)))
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 3 || !parameters.All(p => IsIntegralLike(p.ParameterType)))
                continue;
            return Invoke(() => method.Invoke(null, [
                ConvertIntegral(whole, parameters[0].ParameterType),
                ConvertIntegral(numerator, parameters[1].ParameterType),
                ConvertIntegral(denominator, parameters[2].ParameterType)
            ])!);
        }

        return null;
    }

    private static void AssertRatio(object value, long expectedNumerator, long expectedDenominator)
    {
        var type = ExactRationalType();
        var numerator = ReadPart(type, value, "Numerator");
        var denominator = ReadPart(type, value, "Denominator");

        Assert.AreEqual(new BigInteger(expectedNumerator), numerator, "Unexpected numerator.");
        Assert.AreEqual(new BigInteger(expectedDenominator), denominator, "Unexpected denominator.");
    }

    private static BigInteger ReadPart(Type type, object value, string name)
    {
        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property is not null)
            return ToBigInteger(property.GetValue(value));

        var field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field is not null)
            return ToBigInteger(field.GetValue(value));

        throw new AssertFailedException("ExactRational must expose " + name + " for canonical representation verification.");
    }

    private static BigInteger ToBigInteger(object? value)
    {
        Assert.IsNotNull(value);
        if (value is BigInteger bigInteger)
            return bigInteger;
        return BigInteger.Parse(Convert.ToString(value, CultureInfo.InvariantCulture)!, CultureInfo.InvariantCulture);
    }

    private static bool IsIntegralLike(Type type) =>
        type == typeof(BigInteger) ||
        type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(sbyte) ||
        type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort) || type == typeof(byte);

    private static object ConvertIntegral(long value, Type type)
    {
        if (type == typeof(BigInteger))
            return new BigInteger(value);
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    private static object Invoke(Func<object> action)
    {
        try
        {
            return action();
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }
}
