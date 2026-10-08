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
        var type = typeof(UnitDefinition);
        var attempted = new List<string>();
        foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            var parameters = ctor.GetParameters();
            var values = new object?[parameters.Length];
            var supported = true;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var name = parameter.Name ?? string.Empty;
                var valueType = parameter.ParameterType;
                if (valueType == typeof(string))
                {
                    values[i] = name.Contains("symbol", StringComparison.OrdinalIgnoreCase) ? symbol
                        : name.Contains("name", StringComparison.OrdinalIgnoreCase) ? id
                        : id;
                }
                else if (valueType == typeof(DimensionVector))
                    values[i] = DimensionVector.TemperatureDimension;
                else if (valueType == typeof(ExactRational))
                    values[i] = name.Contains("offset", StringComparison.OrdinalIgnoreCase) ? offset : scale;
                else if (valueType.IsEnum)
                {
                    var choices = Enum.GetNames(valueType);
                    var preference = name.Contains("transform", StringComparison.OrdinalIgnoreCase)
                        ? new[] { "Affine", "Offset", "Linear" }
                        : new[] { "SI", "Metric", "Custom" };
                    var chosen = preference.FirstOrDefault(n => choices.Contains(n, StringComparer.OrdinalIgnoreCase))
                        ?? choices.FirstOrDefault();
                    if (chosen is null) { supported = false; break; }
                    values[i] = Enum.Parse(valueType, chosen);
                }
                else if (valueType == typeof(ImmutableArray<string>))
                    values[i] = ImmutableArray<string>.Empty;
                else if (valueType.IsAssignableFrom(typeof(string[])))
                    values[i] = Array.Empty<string>();
                else if (parameter.HasDefaultValue)
                    values[i] = parameter.DefaultValue;
                else { supported = false; break; }
            }
            if (!supported) { attempted.Add(ctor + " (unmapped parameter)"); continue; }
            try
            {
                if (ctor.Invoke(values) is UnitDefinition unit)
                {
                    if (unit.ScaleToBase == scale && unit.OffsetToBase == offset &&
                        unit.Dimension == DimensionVector.TemperatureDimension)
                        return unit;
                }
            }
            catch (TargetInvocationException ex)
            {
                attempted.Add(ctor + " (" + ex.InnerException?.GetType().Name + ")");
                continue;
            }
        }
        Assert.Fail("No public UnitDefinition constructor could materialize a verified affine " +
            "temperature unit. Constructors: " + string.Join(" | ", attempted));
        throw new InvalidOperationException("Unreachable.");
    }
}
