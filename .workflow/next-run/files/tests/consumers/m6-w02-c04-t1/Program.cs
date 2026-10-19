using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.14";
if (args.Length != 1)
    throw new ArgumentException("One local effective-period financial NuGet archive required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c04-t2-local-feed")
    throw new InvalidOperationException("Only exact local scientific C04-T1 archive accepted.");
var bytes = File.ReadAllBytes(archive);
using (var memory = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(memory, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Expected actual compiled scientific net10 DLL.");
    var specs = zip.Entries.Where(x => x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specs.Length != 1) throw new InvalidOperationException("Ambiguous NuGet metadata.");
    using var file = specs[0].Open();
    var meta = XDocument.Load(file).Descendants().Single(x => x.Name.LocalName == "metadata");
    if (meta.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        meta.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        meta.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("Unqualified exact version or unadmitted dependency.");
}
static void Require(bool predicate, string reason)
{
    if (!predicate) throw new InvalidOperationException(reason);
}
static void Near(FinancialMathOutcome64 actual, double expected)
{
    var observed = actual.Value ??
        throw new InvalidOperationException("Finance success yielded no numeric value.");
    Require(actual.Succeeded &&
        global::System.Math.Abs(observed - expected) <= 1e-10d *
            global::System.Math.Max(1d, global::System.Math.Abs(expected)) &&
        !actual.ErrorBoundGuaranteed,
        "Effective-period arithmetic differs from the independent oracle.");
}
static void Refused(FinancialMathOutcome64 actual, FinancialMathStatus64 status)
{
    Require(!actual.Succeeded && actual.Status == status &&
        actual.Value is null && !actual.ErrorBoundGuaranteed,
        "Finance refusal leaked a partial result or wrong status.");
}
var probes = 0;
void Probe(string label, Action check)
{
    try { check(); probes++; }
    catch (Exception error) { throw new InvalidOperationException("Effective finance NuGet: " + label, error); }
}
Probe("effective discrete present value 1000/11", () =>
    Near(M6W02C04Contract.Discount(100d, 0.1d, 1), 1000d / 11d));
Probe("two-period discount and negative admissible rate", () =>
{
    Near(M6W02C04Contract.Discount(121d, 0.1d, 2), 100d);
    Near(M6W02C04Contract.Discount(100d, -0.5d, 1), 200d);
});
Probe("canonical single-change cashflows IRR 10 percent", () =>
    Near(M6W02C04Contract.SolveIrr([-100d, 110d], 0d, 0.5d), 0.1d));
Probe("zero middle cashflow, inverse signs and explicit bracket", () =>
{
    Near(M6W02C04Contract.SolveIrr([-100d, 0d, 121d], 0d, 0.5d), 0.1d);
    Near(M6W02C04Contract.SolveIrr([100d, -110d], 0d, 0.5d), 0.1d);
});
Probe("no bracket, multiple-sign pattern and missing sign change", () =>
{
    Refused(M6W02C04Contract.SolveIrr([-100d, 110d], 0.2d, 0.5d),
        FinancialMathStatus64.RootNotBracketed);
    Refused(M6W02C04Contract.SolveIrr([-100d, 220d, -121d], -0.9d, 4d),
        FinancialMathStatus64.AmbiguousCashflowPattern);
    Refused(M6W02C04Contract.SolveIrr([100d, 110d], 0d, 0.5d),
        FinancialMathStatus64.NoSignChange);
});
Probe("invalid rates, nonfinite values and pre-emptive cancellation", () =>
{
    Refused(M6W02C04Contract.Discount(100d, double.NaN, 1),
        FinancialMathStatus64.InvalidRate);
    Refused(M6W02C04Contract.SolveIrr([-100d, double.NaN], 0d, 0.5d),
        FinancialMathStatus64.InvalidInput);
    using var cancel = new CancellationTokenSource();
    cancel.Cancel();
    Refused(M6W02C04Contract.SolveIrr([-100d, 110d], 0d, 0.5d, cancel.Token),
        FinancialMathStatus64.Cancelled);
});
Probe("retained Euclidean, DFT and uniform-series family oracles", () =>
{
    var family = new M6W02C03Request64(
        [0d,0d],[3d,4d],[1d,0d],
        [Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],
        [1d,2d],[1d,1d],[1d,3d,6d],1d,2);
    var aggregate = M6W02C03Contract.Evaluate(family);
    Require(aggregate.Succeeded && aggregate.Norm?.Scalar is { } norm &&
        global::System.Math.Abs(norm - 5d) <= 1e-12d &&
        aggregate.Dft?.Bins is { Length: 4 } &&
        aggregate.LinearConvolution?.Samples is { Length: 3 },
        "Previously qualified cross-family scientific facade changed.");
});
Probe("Gamma, exact integer, pure data and no unadmitted dependencies", () =>
{
    var gamma = BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value-24d) <= 1e-10d,
        "Previously qualified managed Gamma changed.");
    var exact = BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded && exact.Value is { } choose && choose == 10,
        "Previously qualified exact integers changed.");
    var dependencies = typeof(M6W02C04Contract).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!dependencies.Any(x => new[]{"MathNet","Meta.Numerics","Aura",
        "Decision","MCDM","Angouri"}.Any(banned =>
        x.Contains(banned,StringComparison.OrdinalIgnoreCase))),
        "An unadmitted external provider or agent dependency was added.");
});
Require(probes == 8, "Eight independent source-free financial NuGet probes required.");
var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C04-T1-CONSUMER-SHA256:" +
    Id + ":" + Version + ":" + bytes.Length + ":" + sha);
Console.WriteLine("dw.tools.math/m6-effective-finance-consumer/1.1 qualified; probes=" + probes);
