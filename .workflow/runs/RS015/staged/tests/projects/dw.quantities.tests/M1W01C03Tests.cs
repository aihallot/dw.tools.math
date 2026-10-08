using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C03Tests
{
    [TestMethod]
    public void ThreeAuthorizedPublicTypesAreInstalled()
    {
        var assembly = typeof(ExactRational).Assembly;
        foreach (var simpleName in new[] { "DimensionVector", "Quantity", "UnitDefinition" })
        {
            var matches = assembly.GetTypes().Where(t => t.Name == simpleName).ToArray();
            Assert.AreEqual(1, matches.Length, "Expected one compiled mathematical type: " + simpleName);
            Assert.IsTrue(matches[0].IsPublic || matches[0].IsNestedPublic,
                simpleName + " must be publicly consumable.");
        }
    }

    [TestMethod]
    public void AreaOfThreeMetresByFourMetresIsTwelveSquareMetres()
    {
        var three = new Quantity(new ExactRational(3, 1), DimensionVector.LengthDimension);
        var four = new Quantity(new ExactRational(4, 1), DimensionVector.LengthDimension);
        var area = three.Multiply(four);
        Assert.AreEqual(new ExactRational(12, 1), area.Value);
        Assert.AreEqual(DimensionVector.LengthDimension + DimensionVector.LengthDimension, area.Dimension);
    }

    [TestMethod]
    public void TenKilometresPerThirtyMinutesIsExactlyFiftyNinthsMetresPerSecond()
    {
        var distance = new Quantity(new ExactRational(10_000, 1), DimensionVector.LengthDimension);
        var duration = new Quantity(new ExactRational(30 * 60, 1), DimensionVector.TimeDimension);
        var speed = distance.Divide(duration);
        Assert.AreEqual(new ExactRational(50, 9), speed.Value);
        Assert.AreEqual(DimensionVector.LengthDimension - DimensionVector.TimeDimension, speed.Dimension);
    }

    [TestMethod]
    public void InformationDimensionRemainsIndependent()
    {
        Assert.AreNotEqual(DimensionVector.Scalar, DimensionVector.InformationDimension);
        Assert.AreNotEqual(DimensionVector.LengthDimension, DimensionVector.InformationDimension);
        var data = new Quantity(new ExactRational(1024, 1), DimensionVector.InformationDimension);
        Assert.AreEqual(new ExactRational(1024, 1), data.Value);
        Assert.AreEqual(DimensionVector.InformationDimension, data.Dimension);
    }

    [TestMethod]
    public void AdditionOfMassAndTimeIsRejectedAtThePublicQuantityBoundary()
    {
        var mass = new Quantity(ExactRational.One, DimensionVector.MassDimension);
        var time = new Quantity(ExactRational.One, DimensionVector.TimeDimension);
        Assert.IsFalse(mass.TryAdd(time, out _),
            "Quantity.TryAdd must reject mismatched mass and time dimensions.");
    }

    [TestMethod]
    public void FahrenheitAndCelsiusAbsoluteConversionsAreExact()
    {
        var f = TemperatureUnit("test-f", "F", new ExactRational(5, 9),
            new ExactRational(45967, 180));
        var c = TemperatureUnit("test-c", "C", ExactRational.One,
            new ExactRational(27315, 100));
        var fBase = f.ToBase(new ExactRational(32, 1));
        var cBase = c.ToBase(ExactRational.Zero);
        Assert.AreEqual(new ExactRational(27315, 100), fBase);
        Assert.AreEqual(cBase, fBase);
        Assert.AreEqual(new ExactRational(32, 1), f.FromBase(cBase));
        Assert.AreEqual(ExactRational.Zero, c.FromBase(fBase));
    }

    [TestMethod]
    public void NineFahrenheitDegreesOfIntervalCorrespondToFiveKelvin()
    {
        var f = TemperatureUnit("test-f", "F", new ExactRational(5, 9),
            new ExactRational(45967, 180));
        // Interval conversion must use the multiplicative scale, not the absolute offset.
        Assert.AreEqual(new ExactRational(5, 1),
            new ExactRational(9, 1) * f.ScaleToBase);
        Assert.AreNotEqual(new ExactRational(5, 1), f.ToBase(new ExactRational(9, 1)),
            "The absolute ToBase API is not an interval converter.");
    }


    private static UnitDefinition TemperatureUnit(
        string id, string symbol, ExactRational scale, ExactRational offset)
    {
        var failures = new List<string>();
        foreach (var ctor in typeof(UnitDefinition).GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            var parameters = ctor.GetParameters();
            var seed = new object?[parameters.Length];
            var enumSlots = new List<int>();
            var supported = true;

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var name = parameter.Name ?? string.Empty;
                var type = parameter.ParameterType;

                if (type == typeof(string))
                    seed[i] = name.Contains("symbol", StringComparison.OrdinalIgnoreCase) ? symbol : id;
                else if (type == typeof(DimensionVector))
                    seed[i] = DimensionVector.TemperatureDimension;
                else if (type == typeof(ExactRational))
                    seed[i] = name.Contains("offset", StringComparison.OrdinalIgnoreCase) ? offset : scale;
                else if (type == typeof(ImmutableArray<string>))
                    seed[i] = ImmutableArray<string>.Empty;
                else if (type.IsAssignableFrom(typeof(string[])))
                    seed[i] = Array.Empty<string>();
                else if (type.IsEnum)
                    enumSlots.Add(i);
                else if (parameter.HasDefaultValue)
                    seed[i] = parameter.DefaultValue;
                else
                {
                    failures.Add(ctor + ": unsupported " + name + " of type " + type.FullName);
                    supported = false;
                    break;
                }
            }
            if (!supported)
                continue;

            // The source declares UnitSystem and UnitTransformKind, but the RED
            // did not establish their admissible combinations. Inspect the bounded
            // public enum domain rather than guessing one constructor policy.
            var candidates = new List<object?[]> { seed };
            foreach (var slot in enumSlots)
            {
                var options = Enum.GetValues(parameters[slot].ParameterType).Cast<object>().ToArray();
                if (options.Length == 0 || (long)candidates.Count * options.Length > 128)
                {
                    failures.Add(ctor + ": enum candidate surface exceeds 128 configurations");
                    supported = false;
                    break;
                }
                candidates = candidates
                    .SelectMany(values => options.Select(option =>
                    {
                        var candidate = (object?[])values.Clone();
                        candidate[slot] = option;
                        return candidate;
                    }))
                    .ToList();
            }
            if (!supported)
                continue;

            foreach (var values in candidates)
            {
                try
                {
                    if (ctor.Invoke(values) is not UnitDefinition unit)
                        continue;
                    if (unit.ScaleToBase == scale &&
                        unit.OffsetToBase == offset &&
                        unit.Dimension == DimensionVector.TemperatureDimension)
                        return unit;

                    failures.Add(ctor + ": constructed different scale/offset/dimension");
                }
                catch (Exception ex) when (ex is TargetInvocationException or ArgumentException)
                {
                    var cause = ex is TargetInvocationException target ? target.InnerException ?? ex : ex;
                    failures.Add(ctor + ": " + cause.GetType().Name + ": " + cause.Message);
                }
            }
        }

        throw new AssertFailedException(
            "No public UnitDefinition constructor produced the requested affine fixture. " +
            "Observed constructor outcomes: " +
            string.Join(" | ", failures.Distinct(StringComparer.Ordinal).Take(12)));
    }

}
