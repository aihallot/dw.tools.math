using System.Security.Cryptography;
using Dw.Tools.Math.Analysis;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("Expected exactly one published-assembly SHA-256 argument.");
const string ExpectedId = "dw.tools.math.analysis";
var loaded = typeof(BoundedCurveAnalysis64).Assembly;
if (loaded.GetName().Name != ExpectedId)
    throw new InvalidOperationException("Loaded an unexpected analysis assembly.");
var loadedSha = Convert.ToHexStringLower(SHA256.HashData(
    File.ReadAllBytes(loaded.Location)));
if (!string.Equals(loadedSha, args[0], StringComparison.Ordinal))
    throw new InvalidOperationException("Loaded DLL bytes differ from the packaged NuGet DLL.");
var references = loaded.GetReferencedAssemblies().Select(x => x.Name ?? "").ToArray();
if (references.Any(x => new[] { "Aura", "Decision", "MCDM", "Angouri", "MathNet" }
    .Any(word => x.Contains(word, StringComparison.OrdinalIgnoreCase))))
    throw new InvalidOperationException("Unexpected external Math provider or host dependency.");

static void Require(bool okay, string reason)
{
    if (!okay) throw new InvalidOperationException(reason);
}
static void Near(double? actual, double expected, double tolerance = 1e-8)
{
    Require(actual is { } value && double.IsFinite(value) &&
        global::System.Math.Abs(value - expected) <= tolerance,
        "Independent numeric oracle mismatch.");
}
var count = 0;
void Probe(string label, Action check)
{
    try { check(); count++; Console.WriteLine("MATH-ANALYSIS-CHECK|" + label + "|PASS"); }
    catch (Exception error) { throw new InvalidOperationException("Analysis oracle " + label, error); }
}
CurvePoint64[] line = [new(0, 1), new(1, 3), new(2, 5)];
Probe("linear", () =>
{
    var actual = BoundedCurveAnalysis64.InterpolateLinear(line, 0.5);
    Require(actual.Succeeded, "Linear interpolation must succeed."); Near(actual.Value, 2);
});
Probe("natural-spline", () =>
{
    var actual = BoundedCurveAnalysis64.InterpolateNaturalSpline(
        [new(0, 0), new(1, 1), new(2, 0)], 0.5);
    Require(actual.Succeeded, "Natural spline must succeed."); Near(actual.Value, 0.6875);
});
Probe("linear-fit", () =>
{
    var actual = BoundedCurveAnalysis64.FitLeastSquares(line);
    Require(actual.Succeeded, "Least squares must succeed.");
    Near(actual.Slope, 2); Near(actual.Intercept, 1); Near(actual.SumSquaredResiduals, 0);
});
Probe("duplicate-refusal", () =>
{
    var actual = BoundedCurveAnalysis64.FitLeastSquares([new(0, 1), new(0, 2)]);
    Require(actual.Status == CurveAnalysisStatus64.DuplicateAbscissa &&
        actual.Slope is null, "Duplicate abscissa must be refused.");
});
Probe("extrapolation-refusal", () =>
{
    var actual = BoundedCurveAnalysis64.InterpolateLinear(line, 3);
    Require(actual.Status == CurveAnalysisStatus64.OutsideDomain &&
        actual.Value is null, "Unrequested extrapolation must be refused.");
});
Probe("budget-refusal", () =>
{
    var large = Enumerable.Range(0, 33).Select(i => new CurvePoint64(i, 2 * i + 1)).ToArray();
    var actual = BoundedCurveAnalysis64.InterpolateNaturalSpline(large, 0.5);
    Require(actual.Status == CurveAnalysisStatus64.BudgetExceeded,
        "Bounded source consumer must refuse 33 samples.");
});
var units = new CurveAxisUnits64("s", "m");
Probe("unit-mismatch", () =>
{
    var actual = CurveBoundaryAudit64.EvaluateLinear(line, 0.5, units, "min");
    Require(actual.Status == CurveBoundaryStatus64.IncompatibleUnit &&
        actual.Value is null, "No silent unit conversion.");
});
Probe("explicit-extrapolation", () =>
{
    var actual = CurveBoundaryAudit64.EvaluateLinear(line, 3, units, "s",
        new CurveBoundaryPolicy64(1e-12, CurveExtrapolation64.LinearSegment));
    Require(actual.Admitted && actual.Extrapolated && !actual.PredictiveGuarantee,
        "Explicit extrapolation is not predictive proof."); Near(actual.Value, 7);
});
Probe("conditioning-refusal", () =>
{
    var actual = CurveBoundaryAudit64.FitDescriptive(
        [new(1e12, 0), new(1e12 + 0.01, 1)], units);
    Require(actual.Status == CurveBoundaryStatus64.NearRankDeficiency &&
        !actual.Admitted, "Ill-conditioned descriptive fit must be refused.");
});
Probe("bracketed-root", () =>
{
    var actual = BoundedUnivariateAnalysis64.SolveBracketed(x => x * x - 2, 1, 2);
    Require(actual.Succeeded && !actual.GuaranteedRootError,
        "Observed bracket does not certify root error.");
    Near(actual.Root, global::System.Math.Sqrt(2), 1e-7);
});
Probe("quadrature", () =>
{
    var actual = BoundedUnivariateAnalysis64.IntegrateSimpson(x => x * x, 0, 1);
    Require(actual.Succeeded && !actual.ErrorBoundGuaranteed,
        "Simpson estimate must not advertise rigorous error bounds.");
    Near(actual.Integral, 1.0 / 3.0, 1e-7);
});
Probe("derivative", () =>
{
    var actual = BoundedDifferentialAnalysis64.Differentiate(x => x * x, 2);
    Require(actual.Succeeded && !actual.ErrorBoundGuaranteed,
        "Derivative must be a finite approximation."); Near(actual.Value, 4, 1e-9);
});
Probe("ode", () =>
{
    var actual = BoundedDifferentialAnalysis64.IntegrateOde((t, y) => y, 0, 1, 1);
    Require(actual.Succeeded && !actual.GlobalErrorBoundGuaranteed,
        "RK4 numerical result must not be presented as certified.");
    Near(actual.FinalValue, global::System.Math.E, 1e-7);
});
Require(count == 13, "Thirteen independent package-only cases required.");
Console.WriteLine("MATH-ANALYSIS-BINARY|PASS|13");
