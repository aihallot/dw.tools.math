using Dw.Tools.Math.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Providers.LinearAlgebra;

Control.UseManaged();
var providerName = LinearAlgebraControl.Provider.GetType().Name;
if (!providerName.Contains("Managed", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("M3 matrix bridge requires the managed provider.");

var source = FiniteMatrix64.Create(2, 2, [2, 1, 1, 3]);
var providerCopy = source.ToProviderArrayCopy();
var mathNet = DenseMatrix.OfArray(providerCopy);
var rhs = DenseVector.OfArray([4d, 7d]);
var solution = mathNet.Solve(rhs);
if (Math.Abs(solution[0] - 1d) > 1e-12 ||
    Math.Abs(solution[1] - 2d) > 1e-12)
    throw new InvalidOperationException("Managed Math.NET 2x2 solve oracle failed.");
var residual = mathNet * solution - rhs;
if (residual.L2Norm() > 1e-12)
    throw new InvalidOperationException("Managed matrix bridge residual exceeds tolerance.");

providerCopy[0, 0] = 99;
if (source.At(0, 0) != 2d)
    throw new InvalidOperationException("Provider copy unexpectedly aliases public matrix.");

var restored = FiniteMatrix64.FromProviderArrayCopy(mathNet.ToArray());
if (restored.At(0, 0) != 2d || restored.At(1, 1) != 3d)
    throw new InvalidOperationException("Provider matrix copy-back changed values.");

Console.WriteLine("M3-W01-C02:managed-mathnet-5.0.0-provider-copy-smoke:qualified");
