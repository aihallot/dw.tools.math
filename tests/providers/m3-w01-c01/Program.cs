using MathNet.Numerics;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Providers.LinearAlgebra;
using MathNet.Numerics.Statistics;

static void Near(double actual, double expected, double tolerance, string oracle)
{
    if (!double.IsFinite(actual) || Math.Abs(actual - expected) > tolerance)
        throw new InvalidOperationException(
            $"{oracle}: expected {expected:R}, observed {actual:R}, tolerance {tolerance:R}");
}

// Always opt into the managed backend: this spike does not qualify native BLAS.
Control.UseManaged();
var provider = LinearAlgebraControl.Provider.GetType().Name;
if (!provider.Contains("Managed", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Native or unknown linear algebra provider loaded: " + provider);
Console.WriteLine("smoke-managed-provider:pass:" + provider);

static void SolveAndCheck()
{
    var matrix = DenseMatrix.OfArray(new double[,] {{2, 1}, {1, 3}});
    var rhs = DenseVector.OfArray(new[] {4d, 7d});
    var solved = matrix.Solve(rhs);
    Near(solved[0], 1, 1e-12, "2x2 x");
    Near(solved[1], 2, 1e-12, "2x2 y");
    var reconstructed = matrix * solved;
    Near(reconstructed[0], rhs[0], 1e-12, "2x2 first residual");
    Near(reconstructed[1], rhs[1], 1e-12, "2x2 second residual");
}
SolveAndCheck();
Console.WriteLine("smoke-matrix:pass");

var data = new[] {1d, 2d, 3d, 4d};
Near(Statistics.Mean(data), 2.5, 1e-14, "mean");
Near(Statistics.PopulationVariance(data), 1.25, 1e-14, "population variance");
Near(Statistics.Variance(data), 5.0 / 3.0, 1e-14, "sample variance");
Console.WriteLine("smoke-statistics:pass");

var integral = GaussLegendreRule.Integrate(x => x * x, 0d, 2d, 5);
Near(integral, 8d / 3d, 1e-12, "quadrature x^2 on [0,2]");
Console.WriteLine("smoke-integration:pass");

Parallel.For(0, 32, _ => SolveAndCheck());
Console.WriteLine("smoke-independent-parallel:pass");

var version = typeof(Control).Assembly.GetName().Version;
if (version?.Major != 5)
    throw new InvalidOperationException("Math.NET managed package assembly major version changed.");
Console.WriteLine("smoke-mathnet-assembly:" + version);
Console.WriteLine("smoke-net10-managed:M3-W01-C01:qualified");
