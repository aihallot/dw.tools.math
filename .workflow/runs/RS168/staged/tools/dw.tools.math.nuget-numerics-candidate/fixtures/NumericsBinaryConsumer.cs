using System.Security.Cryptography;
using dw.quantities;
using Dw.Tools.Math.Numerics;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("The exact packaged Numerics DLL SHA-256 is required.");
var passes = 0;
void Probe(string label, Action test)
{
    try
    {
        test();
        passes++;
        Console.WriteLine("MATH-NUMERICS-CHECK|" + label + "|PASS");
    }
    catch (Exception error)
    {
        throw new InvalidOperationException("Numerics package-only consumer " + label, error);
    }
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
static bool Refuses<TException>(Action action) where TException : Exception
{
    try { action(); return false; }
    catch (TException) { return true; }
}
var a = FiniteMatrix64.Create(2, 2, [2d, 0d, 0d, 3d]);
var b = FiniteMatrix64.Create(2, 2, [1d, 2d, 3d, 4d]);
var x = FiniteVector64.Create([2d, 3d]);

Probe("loaded-numerics-dll", () =>
{
    var assembly = typeof(FiniteMatrix64).Assembly;
    Need(assembly.GetName().Name == "dw.tools.math.numerics",
        "Unexpected Numerics assembly identity.");
    Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(assembly.Location))) ==
         args[0], "Loaded Numerics assembly does not match original archive.");
});
Probe("matrix-finite-shape", () =>
{
    var exported = b.ToProviderArrayCopy();
    exported[0, 0] = 999d;
    Need(a.Rows == 2 && a.Columns == 2 &&
         a.Precision == NumericalPrecisionKind.FiniteBinary64 &&
         a.At(0, 0) == 2d && a.At(1, 1) == 3d &&
         b.At(0, 0) == 1d,
         "Finite matrix shape, precision or defensive copy drifted.");
});
Probe("exact-zero-conversion", () =>
{
    var result = RationalBinary64Conversion.TryConvert(ExactRational.Zero);
    Need(result.Status == RationalBinary64Status.Exact &&
         result.IsFiniteResult && result.Value == 0d &&
         result.AbsoluteError == ExactRational.Zero,
         "Numerics exact rational to finite binary64 conversion drifted.");
});
Probe("matrix-addition", () =>
    Need(a.Add(b).At(0, 0) == 3d && a.Add(b).At(1, 1) == 7d,
         "Finite matrix addition drifted."));
Probe("matrix-product", () =>
    Need(a.Multiply(b).At(0, 1) == 4d &&
         a.Multiply(b).At(1, 0) == 9d, "Matrix product drifted."));
Probe("matrix-vector", () =>
    Need(a.Multiply(x).At(0) == 4d &&
         a.Multiply(x).At(1) == 9d, "Matrix-vector product drifted."));
Probe("shape-refusal", () =>
    Need(Refuses<ArgumentException>(() =>
         a.Add(FiniteMatrix64.Create(1, 2, [1d, 2d]))),
         "Incompatible matrix dimensions must refuse."));
Probe("nonfinite-refusal", () =>
    Need(Refuses<ArithmeticException>(() =>
         FiniteMatrix64.Create(1, 1, [double.NaN])),
         "Nonfinite binary64 input must refuse."));
Probe("element-budget-refusal", () =>
    Need(Refuses<ArgumentOutOfRangeException>(() =>
         FiniteMatrix64.Create(65, 65, [1d])),
         "Matrix dimension budget must refuse."));
Probe("vector-dot", () =>
    Need(x.Dot(FiniteVector64.Create([5d, 7d])) == 31d,
         "Finite vector dot product drifted."));
Probe("vector-shape-refusal", () =>
    Need(Refuses<ArgumentException>(() =>
         x.Dot(FiniteVector64.Create([5d]))),
         "Mismatched vector lengths must refuse."));
Probe("sparse-to-dense", () =>
{
    var sparse = SparseMatrix64.Create(2, 2, [new SparseEntry64(0, 1, 5d)]);
    Need(sparse.NonZeroCount == 1 && sparse.ToDense().At(0, 1) == 5d &&
         sparse.ToDense().At(1, 0) == 0d, "Sparse matrix transport drifted.");
});
Probe("sparse-duplicate-refusal", () =>
    Need(Refuses<ArgumentException>(() =>
         SparseMatrix64.Create(2, 2,
             [new SparseEntry64(0, 1, 5d), new SparseEntry64(0, 1, 6d)])),
         "Duplicate sparse coordinates must refuse."));
Probe("lu-and-solve", () =>
{
    var lu = BoundedLinearAlgebra64.FactorLu(a);
    var solve = BoundedLinearAlgebra64.Solve(a, FiniteVector64.Create([4d, 9d]));
    Need(lu.Factored && lu.Status == LinearAlgebraStatus64.Success &&
         solve.HasUniqueSolution && solve.Solution!.At(0) == 2d &&
         solve.Solution.At(1) == 3d && solve.AbsoluteResidualInfinity == 0d,
         "Bounded LU/solve and residual drifted.");
});
Probe("approximate-ir-roundtrip", () =>
{
    var transported = NumericalMatrixTransport.ToApproximateIr(b);
    var restored = NumericalMatrixTransport.FromApproximateIr(transported);
    Need(restored.Rows == 2 && restored.Columns == 2 &&
         restored.At(0, 1) == 2d && restored.At(1, 0) == 3d,
         "Package-only IR/Numerics approximate roundtrip drifted.");
});
Need(passes == 15, "Exactly fifteen independent compiled Numerics binary probes are required.");
Console.WriteLine("MATH-NUMERICS-BINARY|PASS|15");
