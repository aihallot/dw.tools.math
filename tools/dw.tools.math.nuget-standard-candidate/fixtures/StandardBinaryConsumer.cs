using System.Security.Cryptography;
using dw.quantities;
using dw.quantities.expression;
using dw.quantities.standard;

if (args.Length != 3 ||
    args.Any(sha => sha.Length != 64 ||
        sha.Any(c => c is not (>= '0' and <= '9' or >= 'a' and <= 'f'))))
    throw new ArgumentException("Expected exactly three lower-case DLL SHA-256 values: standard, expression, base.");

static void Check(bool valid, string message)
{
    if (!valid)
        throw new InvalidOperationException("Standard package-only consumer refused: " + message);
}
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

var standardAssembly = typeof(InheritedStandardUnitCatalog).Assembly;
var expressionAssembly = typeof(ExpressionParser).Assembly;
var baseAssembly = typeof(ExactRational).Assembly;

Check(standardAssembly.GetName().Name == "dw.quantities.standard",
    "standard loaded assembly must match the exact standalone package");
Console.WriteLine("MATH-STANDARD-CHECK|standard-assembly|PASS");
Check(expressionAssembly.GetName().Name == "dw.quantities.expression",
    "expression loaded assembly must match its published package");
Console.WriteLine("MATH-STANDARD-CHECK|expression-assembly|PASS");
Check(baseAssembly.GetName().Name == "dw.quantities",
    "base quantities loaded assembly must match its published package");
Console.WriteLine("MATH-STANDARD-CHECK|base-assembly|PASS");
Check(Sha(standardAssembly.Location) == args[0] &&
      Sha(expressionAssembly.Location) == args[1] &&
      Sha(baseAssembly.Location) == args[2] &&
      new[] { standardAssembly.Location, expressionAssembly.Location, baseAssembly.Location }
        .Distinct(StringComparer.OrdinalIgnoreCase).Count() == 3,
    "all three loaded DLLs must match the exact packaged SHA-256 values");
Console.WriteLine("MATH-STANDARD-CHECK|loaded-dll-hashes|PASS");

var gram = StandardExpressionUnitResolver.ResolveInherited("si.gram");
var kilogram = StandardExpressionUnitResolver.ResolveInherited("si.kilogram");
Check(PureUnitConverter.Convert(new ExactRational(250, 1), gram, kilogram) ==
    new ExactRational(1, 4), "250 g must equal exactly one quarter kilogram");
Console.WriteLine("MATH-STANDARD-CHECK|250g-equals-quarter-kg|PASS");
Check(InheritedStandardUnitCatalog.All.Length == 58 &&
      gram.Dimension == DimensionVector.MassDimension &&
      kilogram.Dimension == DimensionVector.MassDimension,
    "the complete inherited exact unit catalogue must remain stable");
Console.WriteLine("MATH-STANDARD-CHECK|inherited-catalog-58|PASS");

var resolver = new ExplicitProfileExpressionUnitResolver();
var parsed = ExpressionParser.Evaluate("250 g", resolver);
Check(parsed is ExpressionEvaluationOutcome.Success quantity &&
      quantity.Quantity.Value == new ExactRational(1, 4) &&
      quantity.Quantity.Dimension == DimensionVector.MassDimension,
    "parsed 250 g must yield exactly 1/4 kg in the base mass dimension");
Console.WriteLine("MATH-STANDARD-CHECK|exact-mass-expression|PASS");
Check(ExpressionParser.Evaluate("1 cup", resolver) is
    ExpressionEvaluationOutcome.Failure { Kind: ExpressionFailureKind.UnitAmbiguous },
    "ambiguous cup token must refuse an implicit profile");
Console.WriteLine("MATH-STANDARD-CHECK|ambiguous-cup-refusal|PASS");
Check(ExpressionParser.Evaluate("1 cup",
    new ExplicitProfileExpressionUnitResolver(UnitSystem.UsCustomary)) is
    ExpressionEvaluationOutcome.Success, "explicit US customary cup must resolve");
Console.WriteLine("MATH-STANDARD-CHECK|explicit-us-profile|PASS");
var length = ExpressionParser.Evaluate("1 in + 1 cm", resolver);
Check(length is ExpressionEvaluationOutcome.Success exactLength &&
      exactLength.Quantity.Value == new ExactRational(177, 5000) &&
      exactLength.Quantity.Dimension == DimensionVector.LengthDimension,
    "inherited inch and centimetre must remain exact through the parser");
Console.WriteLine("MATH-STANDARD-CHECK|inch-plus-centimetre|PASS");
Console.WriteLine("MATH-STANDARD-BINARY|PASS|10");
