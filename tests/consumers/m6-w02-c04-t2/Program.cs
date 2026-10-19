using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id = "dw.tools.math.scientific", Version = "1.1.0-preview.14";
if (args.Length != 1)
    throw new ArgumentException("One local dated finance scientific archive is required.");
var archive = Path.GetFullPath(args[0]);
if (Path.GetFileName(archive) != Id + "." + Version + ".nupkg" ||
    Path.GetFileName(Path.GetDirectoryName(archive)) != "m6-w02-c04-t2-local-feed")
    throw new InvalidOperationException("Only the exact local C04-T2 archive is accepted.");
var bytes = File.ReadAllBytes(archive);
using (var data = new MemoryStream(bytes, writable: false))
using (var zip = new ZipArchive(data, ZipArchiveMode.Read))
{
    if (zip.GetEntry("lib/net10.0/" + Id + ".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Expected compiled net10 scientific assembly.");
    var specifications = zip.Entries.Where(x =>
        x.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (specifications.Length != 1)
        throw new InvalidOperationException("Unexpected scientific nuspec count.");
    using var specification = specifications[0].Open();
    var metadata = XDocument.Load(specification).Descendants()
        .Single(x => x.Name.LocalName == "metadata");
    if (metadata.Elements().Single(x => x.Name.LocalName == "id").Value != Id ||
        metadata.Elements().Single(x => x.Name.LocalName == "version").Value != Version ||
        metadata.Descendants().Any(x => x.Name.LocalName == "dependency"))
        throw new InvalidOperationException("NuGet archive identity or dependency drift.");
}
static void Require(bool predicate, string message)
{
    if (!predicate) throw new InvalidOperationException(message);
}
static void Near(FinancialDatedOutcome64 result, double expected,
    int days, FinancialDayCountConvention64 basis)
{
    var actual = result.Value ??
        throw new InvalidOperationException("A successful dated finance value is absent.");
    Require(result.Succeeded && result.ElapsedDays == days &&
        result.DayCount == basis && !result.ErrorBoundGuaranteed,
        "Dated finance result status, duration or explicit basis differs.");
    Require(global::System.Math.Abs(actual - expected) <= 1e-10d *
        global::System.Math.Max(1d, global::System.Math.Abs(expected)),
        "Independent annual effective finance numerical oracle failed.");
}
static void Refused(FinancialDatedOutcome64 result, FinancialDatedStatus64 status)
{
    Require(!result.Succeeded && result.Status == status &&
        result.Value is null && result.ElapsedDays is null &&
        result.DayCount == FinancialDayCountConvention64.Unspecified &&
        !result.ErrorBoundGuaranteed,
        "Dated finance refusal leaked a value or incorrect status.");
}
var start = new DateOnly(2025, 1, 1);
const FinancialDayCountConvention64 d365=FinancialDayCountConvention64.Actual365Fixed;
const FinancialDayCountConvention64 d360=FinancialDayCountConvention64.Actual360;
var count = 0;
void Probe(string label, Action action)
{
    try { action(); count++; }
    catch (Exception error)
    {
        throw new InvalidOperationException("Dated finance NuGet: " + label, error);
    }
}
Probe("ACT/365 Fixed one-year discount oracle", () =>
    Near(M6W02C04Boundary64.DiscountDated(
        100d, 0.1d, start, start.AddDays(365), d365),
        1000d / 11d, 365, d365));
Probe("ACT/360 actual-day discount and leap-year denominator", () =>
{
    Near(M6W02C04Boundary64.DiscountDated(
        100d, 0.1d, start, start.AddDays(360), d360),
        1000d / 11d, 360, d360);
    var leap = new DateOnly(2024, 1, 1);
    Near(M6W02C04Boundary64.DiscountDated(
        100d, 0.1d, leap, leap.AddDays(366), d365),
        100d/global::System.Math.Pow(1.1d, 366d/365d), 366, d365);
});
Probe("one-sign-change bracketed annual IRR 10 percent", () =>
{
    Near(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,110d], [start,start.AddDays(365)], 0d, 0.5d, d365),
        0.1d, 365, d365);
    Near(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,110d], [start,start.AddDays(360)], 0d, 0.5d, d360),
        0.1d, 360, d360);
});
Probe("potentially multiple, no sign change and all-zero IRRs distinguished", () =>
{
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,230d,-132d],
        [start,start.AddDays(365),start.AddDays(730)],
        0d, 0.5d, d365),FinancialDatedStatus64.PotentialMultipleRoots);
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [100d,110d],[start,start.AddDays(365)],
        0d,0.5d,d365),FinancialDatedStatus64.NoSignChange);
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [0d,0d],[start,start.AddDays(365)],
        0d,0.5d,d365),FinancialDatedStatus64.IndeterminateZeroFlows);
});
Probe("missing, reversed and duplicate payment dates rejected", () =>
{
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,110d],null,0d,0.5d,d365),
        FinancialDatedStatus64.MissingDates);
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,110d],[start,start],0d,0.5d,d365),
        FinancialDatedStatus64.InvalidDates);
    Refused(M6W02C04Boundary64.DiscountDated(
        100d,0.1d,start,start.AddDays(-1),d365),
        FinancialDatedStatus64.InvalidDates);
});
Probe("no root in selected bracket is not a global no-root claim", () =>
{
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,110d],[start,start.AddDays(365)],
        0.2d,0.5d,d365),FinancialDatedStatus64.RootNotBracketed);
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,110d],[start,start.AddDays(365)],
        0.5d,0d,d365),FinancialDatedStatus64.InvalidRateBounds);
});
Probe("fail-closed unspecified conventions, invalid inputs and cancellation", () =>
{
    Refused(M6W02C04Boundary64.DiscountDated(
        100d,0.1d,start,start.AddDays(365),
        FinancialDayCountConvention64.Unspecified),
        FinancialDatedStatus64.DayCountRequired);
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        [-100d,double.NaN],[start,start.AddDays(365)],0d,0.5d,d365),
        FinancialDatedStatus64.InvalidInput);
    using var cancelled=new CancellationTokenSource();
    cancelled.Cancel();
    Refused(M6W02C04Boundary64.SolveDatedIrr(
        null,null,double.NaN,double.NaN,
        FinancialDayCountConvention64.Unspecified,cancelled.Token),
        FinancialDatedStatus64.Cancelled);
});
Probe("retained finance, scientific aggregate, special and dependency isolation", () =>
{
    var original=M6W02C04Contract.Discount(100d,0.1d,1);
    Require(original.Succeeded && original.Value is { } amount &&
        global::System.Math.Abs(amount-1000d/11d)<1e-10d,
        "Previously qualified pure-period financial contract changed.");
    var irr=M6W02C04Contract.SolveIrr([-100d,110d],0d,0.5d);
    Require(irr.Succeeded && irr.Value is { } rate &&
        global::System.Math.Abs(rate-0.1d)<1e-10d,
        "Previously qualified pure-period IRR changed.");
    var aggregate=M6W02C03Contract.Evaluate(new M6W02C03Request64(
        [0d,0d],[3d,4d],[1d,0d],
        [Complex.One,Complex.Zero,Complex.Zero,Complex.Zero],
        [1d,2d],[1d,1d],[1d,3d,6d],1d,2));
    Require(aggregate.Succeeded && aggregate.Dft?.Bins is { Length: 4 },
        "Previously qualified scientific cross-family aggregate changed.");
    var gamma=BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5d);
    Require(gamma.Succeeded && gamma.Value is { } value &&
        global::System.Math.Abs(value-24d)<1e-10d,
        "Previously qualified Gamma changed.");
    var refs=typeof(M6W02C04Boundary64).Assembly.GetReferencedAssemblies()
        .Select(x => x.Name ?? "");
    Require(!refs.Any(x => new[]{"MathNet","Meta.Numerics","Aura",
        "Decision","MCDM","Angouri"}.Any(bad =>
        x.Contains(bad,StringComparison.OrdinalIgnoreCase))),
        "An unadmitted financial provider or agent dependency appeared.");
});
Require(count==8,"Eight independent dated finance NuGet probes required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C04-T2-CONSUMER-SHA256:"+
    Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-dated-finance-consumer/1.1 qualified; probes="+count);
