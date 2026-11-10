using System.Security.Cryptography;
using dw.quantities;
using dw.quantities.expression;
using dw.quantities.standard;

if (args.Length != 3 || args.Any(x => x.Length != 64 ||
    x.Any(c => c is not (>= '0' and <= '9' or >= 'a' and <= 'f'))))
    throw new InvalidOperationException("Require Standard, Expression and Quantities packaged DLL SHA256.");

static void Need(bool valid, string why)
{
    if (!valid) throw new InvalidOperationException("Electrochemical binary: " + why);
}
static string Hash(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

var count = 0;
void Probe(string id, Action action)
{
    try { action(); count++; Console.WriteLine("MATH-ELECTROCHEM-CHECK|" + id + "|PASS"); }
    catch (Exception error)
    {
        throw new InvalidOperationException("Electrochemical binary check " + id, error);
    }
}
var units = new (string Id, string Symbol, string Name, DimensionVector Vector)[]
{
    ("si.ampere", "A", "ampere", new(0, 0, 0, 1, 0, 0, 0, 0)),
    ("si.volt", "V", "volt", new(2, 1, -3, -1, 0, 0, 0, 0)),
    ("si.ohm", "Ω", "ohm", new(2, 1, -3, -2, 0, 0, 0, 0)),
    ("si.watt", "W", "watt", new(2, 1, -3, 0, 0, 0, 0, 0)),
    ("si.mole", "mol", "mole", new(0, 0, 0, 0, 0, 1, 0, 0)),
    ("si.mole-per-cubic-metre", "mol/m³", "mole per cubic metre",
        new(-3, 0, 0, 0, 0, 1, 0, 0))
};
foreach (var unit in units)
{
    Probe(unit.Id, () =>
    {
        Need(ElectrochemicalUnitProfile.TryGetById(unit.Id, out var found),
            "compiled profile must supply original definition");
        var actual = found ?? throw new InvalidOperationException("unexpected null");
        Need(actual.Id == unit.Id && actual.Symbol == unit.Symbol &&
             actual.Name == unit.Name && actual.Dimension == unit.Vector &&
             actual.Dimension.Information == 0 &&
             actual.System == UnitSystem.Si &&
             actual.TransformKind == UnitTransformKind.Linear &&
             actual.ScaleToBase == ExactRational.One &&
             actual.OffsetToBase == ExactRational.Zero &&
             actual.ToBase(new ExactRational(7, 2)) == new ExactRational(7, 2) &&
             actual.FromBase(new ExactRational(7, 2)) == new ExactRational(7, 2),
             "six compiled unit identities, 8D powers or linear ratios changed");
    });
}
Probe("three-loaded-dlls-and-catalogue-identity", () =>
{
    var standard = typeof(ElectrochemicalUnitProfile).Assembly;
    var expression = typeof(ExpressionParser).Assembly;
    var quantities = typeof(ExactRational).Assembly;
    Need(standard.GetName().Name == "dw.quantities.standard" &&
         expression.GetName().Name == "dw.quantities.expression" &&
         quantities.GetName().Name == "dw.quantities" &&
         Hash(standard.Location) == args[0] &&
         Hash(expression.Location) == args[1] &&
         Hash(quantities.Location) == args[2] &&
         new[] { standard.Location, expression.Location, quantities.Location }
             .Distinct(StringComparer.OrdinalIgnoreCase).Count() == 3,
         "all actual loaded assemblies must match three exact published/archive DLLs");
    Need(ElectrochemicalUnitProfile.ProfileId == "math.si-electrochemical" &&
         ElectrochemicalUnitProfile.ProfileVersion == "2026-11-electrochemical-exact-v1" &&
         ElectrochemicalUnitProfile.All.Length == 6 &&
         ElectrochemicalUnitProfile.All.Select(x => x.Id)
             .Distinct(StringComparer.Ordinal).Count() == 6 &&
         InheritedStandardUnitCatalog.Version == "0.2.0" &&
         InheritedStandardUnitCatalog.All.Length == 58 &&
         StandardUnitCatalog.CatalogVersion == "2026-10-exact-v1" &&
         StandardUnitCatalog.All.Length == 14 &&
         units.All(x =>
             !InheritedStandardUnitCatalog.TryGetById(x.Id, out _) &&
             !StandardUnitCatalog.TryGetById(x.Id, out _)),
         "historical 58/14 catalogues or opt-in profile version changed");
});
static Quantity Q(ExactRational n, UnitDefinition unit) =>
    new(unit.ToBase(n), unit.Dimension);
Probe("two-amperes-three-ohms-six-volts", () =>
{
    var output = Q(new ExactRational(2, 1), ElectrochemicalUnitProfile.Ampere)
        .Multiply(Q(new ExactRational(3, 1), ElectrochemicalUnitProfile.Ohm));
    Need(output.Dimension == new DimensionVector(2, 1, -3, -1, 0, 0, 0, 0) &&
         output.Dimension == ElectrochemicalUnitProfile.Volt.Dimension &&
         ElectrochemicalUnitProfile.Volt.FromBase(output.Value) == new ExactRational(6, 1),
         "2 A times 3 ohm should yield 6 V exactly");
});
Probe("six-volts-two-amperes-twelve-watts", () =>
{
    var output = Q(new ExactRational(6, 1), ElectrochemicalUnitProfile.Volt)
        .Multiply(Q(new ExactRational(2, 1), ElectrochemicalUnitProfile.Ampere));
    Need(output.Dimension == new DimensionVector(2, 1, -3, 0, 0, 0, 0, 0) &&
         output.Dimension == ElectrochemicalUnitProfile.Watt.Dimension &&
         ElectrochemicalUnitProfile.Watt.FromBase(output.Value) == new ExactRational(12, 1),
         "6 V times 2 A should yield 12 W exactly");
});
Probe("half-mole-per-litre-and-inverse", () =>
{
    Need(InheritedStandardUnitCatalog.TryGetById("metric.litre", out var l),
         "true inherited litre must exist");
    var litre = l ?? throw new InvalidOperationException("litre null");
    var half = Q(new ExactRational(1, 2), ElectrochemicalUnitProfile.Mole);
    var volume = Q(ExactRational.One, litre);
    var concentration = half.Divide(volume);
    Need(concentration.Dimension == new DimensionVector(-3, 0, 0, 0, 0, 1, 0, 0) &&
         ElectrochemicalUnitProfile.MolePerCubicMetre.FromBase(concentration.Value) ==
             new ExactRational(500, 1) &&
         concentration.Multiply(volume) == half,
         "half mole per litre must equal exactly 500 mol/m3 and invert");
});
Probe("kilogram-to-mole-present-but-incompatible", () =>
{
    Need(InheritedStandardUnitCatalog.TryGetById("si.kilogram", out var k) &&
         ElectrochemicalUnitProfile.TryGetById("si.mole", out var m),
         "both units must exist, not be synthetically absent");
    var kilogram = k ?? throw new InvalidOperationException("kilogram null");
    var mole = m ?? throw new InvalidOperationException("mole null");
    var refused = false;
    try { _ = PureUnitConverter.Convert(ExactRational.One, kilogram, mole); }
    catch (ArgumentException e) when (
        e.Message.Contains("dimensions are incompatible", StringComparison.Ordinal))
    { refused = true; }
    Need(refused && kilogram.Dimension != mole.Dimension,
        "a real incompatible kg-to-mole conversion must refuse");
});
Probe("electrical-dimension-and-transform-refusals", () =>
{
    var ampere = ElectrochemicalUnitProfile.Ampere;
    var volt = ElectrochemicalUnitProfile.Volt;
    var wrongDimensions = false;
    try { _ = PureUnitConverter.Convert(ExactRational.One, ampere, volt); }
    catch (ArgumentException) { wrongDimensions = true; }
    var nonLinearCounterexample = new UnitDefinition(
        "test.incompatible-transform", "V?", "incompatible test voltage",
        UnitSystem.Si, volt.Dimension, ExactRational.One, ExactRational.Zero,
        UnitTransformKind.AbsoluteTemperature);
    var wrongTransform = false;
    try { _ = PureUnitConverter.Convert(ExactRational.One, volt, nonLinearCounterexample); }
    catch (ArgumentException) { wrongTransform = true; }
    Need(wrongDimensions && wrongTransform &&
         ampere.Dimension != volt.Dimension &&
         volt.Dimension == nonLinearCounterexample.Dimension,
         "electrical dimension and transformation refusals are distinct");
});
Need(count == 12, "twelve independently emitted binary check results required");
Console.WriteLine("MATH-ELECTROCHEM-BINARY|PASS|12");
