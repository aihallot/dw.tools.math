using System.Globalization;
using System.Numerics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C01Tests
{
    [TestMethod]
    public void ExactRationalTypeIsMaterialized()
    {
        var type = ExactRationalType();
        Assert.IsTrue(type.IsPublic || type.IsNestedPublic, "ExactRational must be publicly consumable.");
        Console.WriteLine("ExactRational materialized as " + (type.FullName ?? type.Name));
    }

    [TestMethod]
    public void OneThirdPlusOneSixthIsOneHalf()
    {
        var surface = FractionSurface();
        var left = surface.Create(1, 3);
        var right = surface.Create(1, 6);

        foreach (var operation in BinaryOperations(surface.Type))
        {
            try
            {
                var result = operation.Invoke(left, right);
                if (result is not null && surface.IsRatio(result, 1, 2))
                    return;
            }
            catch
            {
            }
        }

        throw new AssertFailedException(
            "No public ExactRational binary operation produced 1/2 from 1/3 and 1/6. " +
            DescribePublicSurface(surface.Type));
    }

    [TestMethod]
    public void ZeroDenominatorIsRejected()
    {
        var surface = FractionSurface();
        try
        {
            _ = surface.Create(1, 0);
        }
        catch
        {
            return;
        }

        Assert.Fail("The discovered ExactRational fraction construction path accepted a zero denominator.");
    }

    [TestMethod]
    public void NegativeDenominatorIsCanonicalized()
    {
        var surface = FractionSurface();
        Assert.IsTrue(
            surface.IsRatio(surface.Create(2, -4), -1, 2),
            "The discovered ExactRational fraction construction path did not canonicalize 2/-4 to -1/2.");
    }

    [TestMethod]
    public void NegativeMixedNumberRepresentsNegativeThreeHalves()
    {
        var surface = FractionSurface();

        foreach (var create in ThreePartFactories(surface.Type))
        {
            foreach (var values in MixedPermutations())
            {
                try
                {
                    var value = create.Invoke(values[0], values[1], values[2]);
                    if (value is not null && surface.IsRatio(value, -3, 2))
                        return;
                }
                catch
                {
                }
            }
        }

        foreach (var parse in StringParsers(surface.Type))
        {
            try
            {
                var value = parse.Invoke("-1 1/2");
                if (value is not null && surface.IsRatio(value, -3, 2))
                    return;
            }
            catch
            {
            }
        }

        throw new AssertFailedException(
            "No discovered public ExactRational mixed-number construction or parse path represented -1 1/2 as -3/2. " +
            DescribePublicSurface(surface.Type));
    }

    private static Type ExactRationalType()
    {
        var assembly = Assembly.Load("dw.quantities");
        var matches = assembly.GetTypes()
            .Where(type => string.Equals(type.Name, "ExactRational", StringComparison.Ordinal))
            .ToArray();

        if (matches.Length != 1)
        {
            var observed = string.Join(", ", assembly.GetTypes()
                .Select(type => type.FullName ?? type.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .Take(80));
            throw new AssertFailedException(
                $"Expected exactly one compiled type whose simple name is ExactRational; found {matches.Length}. Observed types: {observed}");
        }

        return matches[0];
    }

    private static FractionApi FractionSurface()
    {
        var type = ExactRationalType();
        var accessors = IntegralAccessors(type).ToArray();

        foreach (var factory in TwoPartFactories(type))
        {
            try
            {
                var oneThird = factory.Invoke(1, 3);
                var normalized = factory.Invoke(2, -4);
                if (oneThird is null || normalized is null)
                    continue;

                foreach (var numerator in accessors)
                {
                    foreach (var denominator in accessors)
                    {
                        if (ReferenceEquals(numerator, denominator))
                            continue;

                        if (numerator.TryRead(oneThird, out var n13) &&
                            denominator.TryRead(oneThird, out var d13) &&
                            numerator.TryRead(normalized, out var n24) &&
                            denominator.TryRead(normalized, out var d24) &&
                            n13 == BigInteger.One &&
                            d13 == new BigInteger(3) &&
                            n24 == new BigInteger(-1) &&
                            d24 == new BigInteger(2))
                        {
                            return new FractionApi(type, factory, numerator, denominator);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        throw new AssertFailedException(
            "No public ExactRational two-integral construction path plus public integral representation " +
            "matched 1/3 and canonicalized 2/-4 to -1/2. " + DescribePublicSurface(type));
    }

    private static IEnumerable<TwoPartFactory> TwoPartFactories(Type type)
    {
        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                     .OrderBy(c => c.ToString(), StringComparer.Ordinal))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == 2 && parameters.All(p => IsSignedIntegralLike(p.ParameterType)))
            {
                yield return new TwoPartFactory(
                    "ctor " + constructor,
                    (numerator, denominator) => Invoke(() => constructor.Invoke([
                        ConvertIntegral(numerator, parameters[0].ParameterType),
                        ConvertIntegral(denominator, parameters[1].ParameterType)
                    ])));
            }
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.ReturnType == type)
                     .OrderBy(m => m.ToString(), StringComparer.Ordinal))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 2 && parameters.All(p => IsSignedIntegralLike(p.ParameterType)))
            {
                yield return new TwoPartFactory(
                    "static " + method,
                    (numerator, denominator) => Invoke(() => method.Invoke(null, [
                        ConvertIntegral(numerator, parameters[0].ParameterType),
                        ConvertIntegral(denominator, parameters[1].ParameterType)
                    ])));
            }
        }
    }

    private static IEnumerable<ThreePartFactory> ThreePartFactories(Type type)
    {
        foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                     .OrderBy(c => c.ToString(), StringComparer.Ordinal))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == 3 && parameters.All(p => IsSignedIntegralLike(p.ParameterType)))
            {
                yield return new ThreePartFactory(
                    "ctor " + constructor,
                    (a, b, c) => Invoke(() => constructor.Invoke([
                        ConvertIntegral(a, parameters[0].ParameterType),
                        ConvertIntegral(b, parameters[1].ParameterType),
                        ConvertIntegral(c, parameters[2].ParameterType)
                    ])));
            }
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.ReturnType == type)
                     .OrderBy(m => m.ToString(), StringComparer.Ordinal))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 3 && parameters.All(p => IsSignedIntegralLike(p.ParameterType)))
            {
                yield return new ThreePartFactory(
                    "static " + method,
                    (a, b, c) => Invoke(() => method.Invoke(null, [
                        ConvertIntegral(a, parameters[0].ParameterType),
                        ConvertIntegral(b, parameters[1].ParameterType),
                        ConvertIntegral(c, parameters[2].ParameterType)
                    ])));
            }
        }
    }

    private static IEnumerable<StringParser> StringParsers(Type type)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .OrderBy(m => m.ToString(), StringComparer.Ordinal))
        {
            var parameters = method.GetParameters();

            if (method.ReturnType == type && TryBuildDirectStringArguments(parameters, "-1 1/2", out _))
            {
                yield return new StringParser(
                    "static " + method,
                    text =>
                    {
                        if (!TryBuildDirectStringArguments(parameters, text, out var args))
                            throw new InvalidOperationException("Parser shape changed.");
                        return Invoke(() => method.Invoke(null, args));
                    });
                continue;
            }

            if (method.ReturnType != typeof(bool))
                continue;

            var outIndexes = parameters
                .Select((parameter, index) => (parameter, index))
                .Where(x => x.parameter.IsOut && x.parameter.ParameterType.GetElementType() == type)
                .Select(x => x.index)
                .ToArray();

            if (outIndexes.Length != 1 ||
                !TryBuildTryParseArguments(parameters, outIndexes[0], "-1 1/2", out _))
                continue;

            var outIndex = outIndexes[0];
            yield return new StringParser(
                "static " + method,
                text =>
                {
                    if (!TryBuildTryParseArguments(parameters, outIndex, text, out var args))
                        throw new InvalidOperationException("TryParse shape changed.");
                    var ok = (bool)(Invoke(() => method.Invoke(null, args)) ?? false);
                    return ok ? args[outIndex] : null;
                });
        }
    }

    private static bool TryBuildDirectStringArguments(ParameterInfo[] parameters, string text, out object?[] args)
    {
        args = new object?[parameters.Length];
        var stringCount = 0;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.ParameterType == typeof(string))
            {
                args[i] = text;
                stringCount++;
            }
            else if (typeof(IFormatProvider).IsAssignableFrom(parameter.ParameterType))
            {
                args[i] = CultureInfo.InvariantCulture;
            }
            else if (parameter.HasDefaultValue)
            {
                args[i] = parameter.DefaultValue;
            }
            else
            {
                return false;
            }
        }

        return stringCount == 1;
    }

    private static bool TryBuildTryParseArguments(
        ParameterInfo[] parameters,
        int outIndex,
        string text,
        out object?[] args)
    {
        args = new object?[parameters.Length];
        var stringCount = 0;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (i == outIndex)
            {
                args[i] = null;
            }
            else if (parameter.ParameterType == typeof(string))
            {
                args[i] = text;
                stringCount++;
            }
            else if (typeof(IFormatProvider).IsAssignableFrom(parameter.ParameterType))
            {
                args[i] = CultureInfo.InvariantCulture;
            }
            else if (parameter.HasDefaultValue)
            {
                args[i] = parameter.DefaultValue;
            }
            else
            {
                return false;
            }
        }

        return stringCount == 1;
    }

    private static IEnumerable<BinaryOperation> BinaryOperations(Type type)
    {
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Where(m => m.ReturnType == type)
                     .OrderBy(m => m.ToString(), StringComparer.Ordinal))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 2 &&
                parameters[0].ParameterType == type &&
                parameters[1].ParameterType == type)
            {
                yield return new BinaryOperation(
                    "static " + method,
                    (left, right) => Invoke(() => method.Invoke(null, [left, right])));
            }
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                     .Where(m => m.ReturnType == type)
                     .OrderBy(m => m.ToString(), StringComparer.Ordinal))
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == type)
            {
                yield return new BinaryOperation(
                    "instance " + method,
                    (left, right) => Invoke(() => method.Invoke(left, [right])));
            }
        }
    }

    private static IEnumerable<IntegralAccessor> IntegralAccessors(Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanRead && p.GetIndexParameters().Length == 0 && IsIntegralLike(p.PropertyType))
                     .OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            yield return new IntegralAccessor(
                "property " + property.Name,
                value => property.GetValue(value));
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                     .Where(f => IsIntegralLike(f.FieldType))
                     .OrderBy(f => f.Name, StringComparer.Ordinal))
        {
            yield return new IntegralAccessor(
                "field " + field.Name,
                value => field.GetValue(value));
        }
    }

    private static long[][] MixedPermutations() =>
    [
        [-1, 1, 2],
        [-1, 2, 1],
        [1, -1, 2],
        [1, 2, -1],
        [2, -1, 1],
        [2, 1, -1]
    ];

    private static bool IsSignedIntegralLike(Type type) =>
        type == typeof(BigInteger) ||
        type == typeof(long) ||
        type == typeof(int) ||
        type == typeof(short) ||
        type == typeof(sbyte) ||
        type == typeof(Int128) ||
        type == typeof(nint);

    private static bool IsIntegralLike(Type type) =>
        IsSignedIntegralLike(type) ||
        type == typeof(ulong) ||
        type == typeof(uint) ||
        type == typeof(ushort) ||
        type == typeof(byte) ||
        type == typeof(UInt128) ||
        type == typeof(nuint);

    private static object ConvertIntegral(long value, Type type)
    {
        if (type == typeof(BigInteger))
            return new BigInteger(value);
        if (type == typeof(Int128))
            return (Int128)value;
        if (type == typeof(nint))
            return (nint)value;
        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    private static BigInteger ToBigInteger(object? value)
    {
        if (value is null)
            throw new InvalidOperationException("Integral accessor returned null.");
        if (value is BigInteger bigInteger)
            return bigInteger;
        if (value is Int128 int128)
            return BigInteger.Parse(int128.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        if (value is UInt128 uint128)
            return BigInteger.Parse(uint128.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        if (value is nint nativeInt)
            return new BigInteger((long)nativeInt);
        if (value is nuint nativeUInt)
            return new BigInteger((ulong)nativeUInt);
        return BigInteger.Parse(Convert.ToString(value, CultureInfo.InvariantCulture)!, CultureInfo.InvariantCulture);
    }

    private static object? Invoke(Func<object?> action)
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

    private static string DescribePublicSurface(Type type)
    {
        var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(member => member.MemberType is MemberTypes.Constructor or MemberTypes.Method or MemberTypes.Property or MemberTypes.Field)
            .Select(member => member.ToString() ?? member.Name)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(text => text, StringComparer.Ordinal)
            .Take(80);
        return "Compiled type: " + (type.FullName ?? type.Name) + ". Public surface: " + string.Join(" | ", members);
    }

    private sealed record TwoPartFactory(string Description, Func<long, long, object?> Invoke);
    private sealed record ThreePartFactory(string Description, Func<long, long, long, object?> Invoke);
    private sealed record StringParser(string Description, Func<string, object?> Invoke);
    private sealed record BinaryOperation(string Description, Func<object, object, object?> Invoke);

    private sealed record IntegralAccessor(string Description, Func<object, object?> Read)
    {
        public bool TryRead(object value, out BigInteger result)
        {
            try
            {
                result = ToBigInteger(Read(value));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }

    private sealed record FractionApi(
        Type Type,
        TwoPartFactory Factory,
        IntegralAccessor Numerator,
        IntegralAccessor Denominator)
    {
        public object Create(long numerator, long denominator) =>
            Factory.Invoke(numerator, denominator)
            ?? throw new AssertFailedException("The discovered ExactRational factory returned null.");

        public bool IsRatio(object value, long numerator, long denominator) =>
            Numerator.TryRead(value, out var actualNumerator) &&
            Denominator.TryRead(value, out var actualDenominator) &&
            actualNumerator == new BigInteger(numerator) &&
            actualDenominator == new BigInteger(denominator);
    }
}
