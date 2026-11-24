#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Matrix = "docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json";
const string Proposal = "docs/planning/proposals/net11-numerics-impact-matrix.json";
const string Verifier = "docs/engineering/ValidateM8W02OfficialDeltaMatrix.cs";

var originals = new SortedSet<string>(StringComparer.Ordinal)
{
    "global.json", "Directory.Build.props", Matrix, Proposal, Verifier
};
using (var file = JsonDocument.Parse(File.ReadAllText(Matrix)))
{
    foreach (var item in file.RootElement.GetProperty("packages").EnumerateArray())
    {
        var source = item.GetProperty("projectPath").GetString() ??
            throw new InvalidDataException("RS318: Package project path is absent.");
        var test = item.GetProperty("testProject").GetString() ??
            throw new InvalidDataException("RS318: Test project path is absent.");
        Need(source.StartsWith("src/projects/", StringComparison.Ordinal) &&
            test.StartsWith("tests/projects/", StringComparison.Ordinal) &&
            !source.Contains("..", StringComparison.Ordinal) &&
            !test.Contains("..", StringComparison.Ordinal),
            "Candidate paths leave the bounded Math source/test projects.");
        originals.Add(source);
        originals.Add(test);
    }
}
Need(originals.Count == 29, "Expected five shared inputs, thirteen sources and eleven tests.");

var pristine = originals.ToDictionary(path => path, Sha256, StringComparer.Ordinal);
var independentRoot = Path.GetFullPath(Path.Combine(
    "build", "artifacts", "m8-rs318-independent-delta",
    Guid.NewGuid().ToString("N")));
Directory.CreateDirectory(independentRoot);
foreach (var relative in originals)
{
    var dest = Path.Combine(independentRoot,
        relative.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(relative, dest);
    Need(Sha256(dest) == pristine[relative],
        "Independent copied source differs: " + relative);
}

ProcessResult Verify() => Run(independentRoot,
    "run", "--file", Verifier, "--", "--check");
void Clean()
{
    var result = Verify();
    Need(result.ExitCode == 0 &&
        result.Output.Contains("MATH-RS316-OFFICIAL-DELTA|packages=13|directTests=13|",
            StringComparison.Ordinal) &&
        result.Output.Contains("proposalCandidates=9|officialSources=4|",
            StringComparison.Ordinal) &&
        result.Output.Contains("compatibility=unqualified|migration=false|PASS",
            StringComparison.Ordinal),
        "Copied original verifier rejected the pristine source graph: " + result.Output);
}
Clean();

var copiedMatrix = Path.Combine(independentRoot,
    Matrix.Replace('/', Path.DirectorySeparatorChar));
var originalMatrix = File.ReadAllBytes(copiedMatrix);
var refusals = 0;
void Reject(string name, Action<JsonObject> change, string expectedReason)
{
    var document = JsonNode.Parse(File.ReadAllText(copiedMatrix))?.AsObject() ??
        throw new InvalidDataException("RS318: Copied delta matrix cannot be parsed.");
    change(document);
    try
    {
        File.WriteAllText(copiedMatrix, document.ToJsonString(
            new JsonSerializerOptions { WriteIndented = true }) + "\n",
            new UTF8Encoding(false));
        var result = Verify();
        Need(result.ExitCode != 0 &&
            result.Output.Contains("RS316: " + expectedReason, StringComparison.Ordinal),
            "Existing copied verifier did not refuse " + name + ": " + result.Output);
        refusals++;
        Console.WriteLine("MATH-RS318-INDEPENDENT-REFUSAL|" + name + "|PASS");
    }
    finally
    {
        File.WriteAllBytes(copiedMatrix, originalMatrix);
    }
    Clean();
}

JsonObject FindPackage(JsonObject document, string package)
{
    var packages = document["packages"]?.AsArray() ??
        throw new InvalidDataException("RS318: Package rows absent.");
    return packages.Select(x => x?.AsObject()).Single(x =>
        x?["packageId"]?.GetValue<string>() == package) ??
        throw new InvalidDataException("RS318: Package missing: " + package);
}

Reject("forged-candidate",
    document => FindPackage(document, "dw.quantities")["proposalCandidateIds"] =
        new JsonArray(JsonValue.Create("N11-999-FORGED")),
    "Candidate IDs do not match historical research ownership");
Reject("omitted-package",
    document =>
    {
        var packages = document["packages"]!.AsArray();
        var index = Enumerable.Range(0, packages.Count).Single(i =>
            packages[i]?["packageId"]?.GetValue<string>() == "dw.tools.math.analysis");
        packages.RemoveAt(index);
    },
    "Official delta candidate matrix does not cover every packable Math source");
Reject("false-compatibility",
    document => FindPackage(document, "dw.quantities")["compatibility"] =
        JsonValue.Create("qualified"),
    "A candidate was silently declared applicable, compatible or lacking its next oracle");
Reject("incorrect-test-owner",
    document => FindPackage(document, "dw.tools.math.numerics")["testProject"] =
        JsonValue.Create(
            "tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj"),
    "A declared direct test owner does not reference the candidate package");

Need(refusals == 4 && Sha256(copiedMatrix) == pristine[Matrix],
    "Copied candidate matrix was not restored after four refusals.");
foreach (var relative in originals)
{
    var dest = Path.Combine(independentRoot,
        relative.Replace('/', Path.DirectorySeparatorChar));
    Need(Sha256(relative) == pristine[relative] &&
        Sha256(dest) == pristine[relative],
        "Math authority or independent copied input changed: " + relative);
}

Console.WriteLine("MATH-RS318-DELTA-VERIFICATION|copies=29|sourceProjects=13|" +
    "testProjects=11|proposalCandidates=9|originalVerifier=true|" +
    "forgedCandidate=refused|omittedSource=refused|falseCompatibility=refused|" +
    "wrongTestOwner=refused|cleanRestored=4|migration=false|PASS");

static string Sha256(string path)
{
    using var source = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(source));
}
static void Need(bool valid, string reason)
{
    if (!valid) throw new InvalidDataException("RS318: " + reason);
}
static ProcessResult Run(string directory, params string[] args)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = directory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS318: Cannot launch copied verifier.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS318: Copied SDK verifier timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" +
        stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
