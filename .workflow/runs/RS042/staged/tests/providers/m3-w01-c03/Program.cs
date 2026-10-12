using Dw.Tools.Math.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Providers.LinearAlgebra;

// The provider lives in an independent optional smoke consumer.
// None of these symbols appear in dw.tools.math.numerics.dll.
Control.UseManaged();
var name = LinearAlgebraControl.Provider.GetType().Name;
if (!name.Contains("Managed", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("M3-C03 must not qualify unknown native BLAS.");

static void Near(double observed, double expected, double tolerance, string label)
{
    if (!double.IsFinite(observed) || global::System.Math.Abs(observed - expected) > tolerance)
        throw new InvalidOperationException(
            $"{label}: {observed:R} differs from {expected:R} (tolerance {tolerance:R}).");
}

var original = FiniteMatrix64.Create(2, 2, [2, 1, 1, 3]);
var ownedCopy = original.ToProviderArrayCopy();
var managed = DenseMatrix.OfArray(ownedCopy);
var right = DenseVector.OfArray([4d, 7d]);

// Independent managed LU solve and hand-verifiable residual.
var solved = managed.LU().Solve(right);
Near(solved[0], 1d, 1e-12, "managed LU x0");
Near(solved[1], 2d, 1e-12, "managed LU x1");
var ours = BoundedLinearAlgebra64.Solve(original, FiniteVector64.Create([4, 7]));
if (!ours.HasUniqueSolution)
    throw new InvalidOperationException("Math provider-neutral LU refused a nonsingular oracle.");
Near(ours.Solution!.At(0), solved[0], 1e-12, "managed LU cross-check x0");
Near(ours.Solution.At(1), solved[1], 1e-12, "managed LU cross-check x1");

// Independent managed QR and provider-neutral reconstruction invariants.
var qr = managed.QR();
var qrProduct = qr.Q * qr.R;
for (var row = 0; row < 2; row++)
    for (var column = 0; column < 2; column++)
        Near(qrProduct[row, column], managed[row, column], 1e-12, "managed QR");
var ownQr = BoundedLinearAlgebra64.FactorQr(original);
if (!ownQr.Factored)
    throw new InvalidOperationException("Math provider-neutral QR unexpectedly refused.");
var ownReconstruction = ownQr.Q!.Multiply(ownQr.R!);
for (var row = 0; row < 2; row++)
    for (var column = 0; column < 2; column++)
        Near(ownReconstruction.At(row, column), qrProduct[row, column], 1e-11,
            "provider-neutral versus managed QR reconstruction");

// Only the 2x2 singular spectrum is currently admitted publicly.
var svd = managed.Svd(computeVectors: true);
var reconstruction = svd.U * svd.W * svd.VT;
for (var row = 0; row < 2; row++)
    for (var column = 0; column < 2; column++)
        Near(reconstruction[row, column], managed[row, column], 1e-11,
            "managed full SVD reconstruction");
var spectrum = BoundedLinearAlgebra64.SingularValues2x2(original);
if (spectrum.Status != LinearAlgebraStatus64.Success)
    throw new InvalidOperationException("Math bounded singular values unexpectedly refused.");
// Compare the invariant spectrum, not provider-specific singular-vector signs
// or the ordering of singular values reported by a factorization backend.
var providerLargest = global::System.Math.Max(svd.S[0], svd.S[1]);
var providerSmallest = global::System.Math.Min(svd.S[0], svd.S[1]);
Near(spectrum.LargestSingularValue!.Value, providerLargest, 1e-12, "largest singular value");
Near(spectrum.SmallestSingularValue!.Value, providerSmallest, 1e-12, "smallest singular value");

// The test-only provider must not be given ownership of Math matrices.
ownedCopy[0, 0] = 99d;
Near(original.At(0, 0), 2d, 0d, "provider array must not alias product input");
var copy = original.ToProviderArrayCopy();
Near(copy[0, 0], 2d, 0d, "fresh provider array must be independent");

// A reference assembly and result do not grant solver provider availability
// on any other RID, or imply a real AURA capability/broker adapter.
if (typeof(Control).Assembly.GetName().Version?.Major != 5)
    throw new InvalidOperationException("Pinned Math.NET managed assembly changed.");
Console.WriteLine("M3-W01-C03:independent-managed-lu-qr-svd-smoke:qualified");
