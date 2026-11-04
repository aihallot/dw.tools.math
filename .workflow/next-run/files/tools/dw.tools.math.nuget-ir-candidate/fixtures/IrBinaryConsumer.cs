using System.Security.Cryptography;
using dw.quantities;
using Dw.Tools.Math.Ir;

if (args.Length != 2 || args.Any(hash => hash.Length != 64 ||
    hash.Any(c => c is not (>= '0' and <= '9' or >= 'a' and <= 'f'))))
    throw new ArgumentException("Expected IR and Quantities exact DLL SHA-256 identities.");

static void Check(bool condition, string reason)
{
    if (!condition)
        throw new InvalidOperationException("IR source-free binary consumer refused: " + reason);
}
static void ExpectThrows<TException>(Action action, string reason) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }
    throw new InvalidOperationException("Expected " + typeof(TException).Name + ": " + reason);
}
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

var irAssembly = typeof(IrCanonicalJsonCodec).Assembly;
var baseAssembly = typeof(ExactRational).Assembly;
Check(irAssembly.GetName().Name == "dw.tools.math.ir", "IR assembly identity");
Console.WriteLine("MATH-IR-CHECK|ir-assembly|PASS");
Check(baseAssembly.GetName().Name == "dw.quantities", "Quantities base identity");
Console.WriteLine("MATH-IR-CHECK|base-assembly|PASS");
Check(Sha(irAssembly.Location) == args[0] &&
      Sha(baseAssembly.Location) == args[1] &&
      !string.Equals(irAssembly.Location, baseAssembly.Location,
          StringComparison.OrdinalIgnoreCase), "two exact loaded NuGet DLL SHA-256 values");
Console.WriteLine("MATH-IR-CHECK|loaded-dll-hashes|PASS");

var half = new IrExactScalar(new ExactRational(2, 4));
var exactJson = IrCanonicalJsonCodec.Encode(half);
Check(half.Value == new ExactRational(1, 2) &&
      IrCanonicalJsonCodec.Decode(exactJson) is IrExactScalar exactRoundtrip &&
      exactRoundtrip.Value == new ExactRational(1, 2) &&
      exactJson.Contains("\"numerator\":\"1\"", StringComparison.Ordinal),
    "exact canonical rational must be reduced without numeric widening");
Console.WriteLine("MATH-IR-CHECK|exact-rational-json|PASS");

var negativeZero = IrFiniteBinary64Scalar.FromDouble(-0.0);
Check(negativeZero.Ieee754Bits != IrFiniteBinary64Scalar.FromDouble(+0.0).Ieee754Bits &&
      IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(negativeZero)) is
          IrFiniteBinary64Scalar restoredZero &&
      restoredZero.Ieee754Bits == negativeZero.Ieee754Bits,
    "finite binary64 signed zero bits must survive canonical JSON");
Console.WriteLine("MATH-IR-CHECK|signed-zero-bits|PASS");
ExpectThrows<ArgumentOutOfRangeException>(
    () => IrFiniteBinary64Scalar.FromDouble(double.NaN),
    "non-finite binary64 values must be refused");
Console.WriteLine("MATH-IR-CHECK|nan-refusal|PASS");

var x = IrSymbol.Free("x", "Original label");
var relabeled = IrSymbol.Free("x", "Different passive label");
Check(IrCanonicalHashes.SemanticStructuralSha256(x) ==
          IrCanonicalHashes.SemanticStructuralSha256(relabeled) &&
      IrCanonicalHashes.PresentationSha256(x) !=
          IrCanonicalHashes.PresentationSha256(relabeled),
    "semantic digest must be stable under a passive display-label change");
Console.WriteLine("MATH-IR-CHECK|semantic-presentation-domain|PASS");
var bound = IrSymbol.Bound("scope", 0, "item", IrScalarDomain.Integer);
Check(bound.IsBound && bound.Identity == "bound:scope:0" && bound.Slot == 0 &&
      bound.Domain == IrScalarDomain.Integer,
    "bound symbols must retain explicit scope and slot identity");
Console.WriteLine("MATH-IR-CHECK|bound-symbol|PASS");

var sum = IrApply.Create(IrOperation.Add,
    [new IrExactScalar(ExactRational.One), half]);
var sumRoundtrip = IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(sum));
Check(sumRoundtrip is IrApply { Operation: IrOperation.Add } &&
      IrCanonicalHashes.SemanticStructuralSha256(sum) ==
      IrCanonicalHashes.SemanticStructuralSha256(sumRoundtrip),
    "typed two-operand addition must retain its exact canonical IR shape");
Console.WriteLine("MATH-IR-CHECK|apply-canonical-roundtrip|PASS");

var root = IrApply.Create(IrOperation.Sqrt,
    [IrApply.Create(IrOperation.Square, [x])]);
Check(IrRealDomainRules.NormalizeSqrtOfSquare(root, IrAssumptionSet.Empty) is
      IrApply { Operation: IrOperation.Abs },
    "sqrt(square(x)) without assumptions is absolute value, not x");
Console.WriteLine("MATH-IR-CHECK|sqrt-square-abs|PASS");
var nonnegative = IrAssumptionSet.Create(
    [new IrRelation(x, IrRelationKind.GreaterOrEqual,
        new IrExactScalar(ExactRational.Zero))]);
Check(IrRealDomainRules.NormalizeSqrtOfSquare(root, nonnegative) == x,
    "explicit nonnegative assumption permits sqrt(square(x)) to normalize to x");
Console.WriteLine("MATH-IR-CHECK|sqrt-square-assumption|PASS");

var matrix = IrMatrix.Create(2, 2,
    [half, half, half, new IrExactScalar(ExactRational.One)]);
Check(matrix.Rows == 2 && matrix.Columns == 2 &&
      matrix.At(1, 1) is IrExactScalar last &&
      last.Value == ExactRational.One,
    "2x2 exact matrix must preserve shape and row-major values");
Console.WriteLine("MATH-IR-CHECK|exact-matrix-shape|PASS");
ExpectThrows<ArgumentException>(() =>
    IrMatrix.Create(1, 2, [half, IrFiniteBinary64Scalar.FromDouble(1.0)]),
    "exact and approximate scalar matrix entries cannot be silently mixed");
Console.WriteLine("MATH-IR-CHECK|matrix-widening-refusal|PASS");

var quantity = new IrQuantityLiteral(
    new IrExactScalar(new ExactRational(250, 1)), "si.gram",
    UnitSystem.Si, DimensionVector.MassDimension);
Check(IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(quantity)) is
      IrQuantityLiteral q &&
      q.CanonicalUnitId == "si.gram" &&
      q.System == UnitSystem.Si &&
      q.Dimension == DimensionVector.MassDimension,
    "canonical quantity must retain its exact base DimensionVector and unit identity");
Console.WriteLine("MATH-IR-CHECK|quantity-roundtrip|PASS");

ExpectThrows<NotSupportedException>(() =>
    IrCanonicalJsonCodec.Decode(exactJson.Replace(
        "\"kind\":\"exact\"", "\"kind\":\"mystery\"",
        StringComparison.Ordinal)),
    "unrecognized IR JSON kind must be rejected by the canonical shape guard");
Console.WriteLine("MATH-IR-CHECK|unknown-kind-refusal|PASS");
Console.WriteLine("MATH-IR-BINARY|PASS|15");
