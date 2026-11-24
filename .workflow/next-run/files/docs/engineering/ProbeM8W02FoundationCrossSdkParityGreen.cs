#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Source = "src/projects/dw.tools.math.foundation/FoundationContract.cs";
const string Project = "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj";
const string MigrationProfile = "docs/engineering/m8-w02-c02-migration-risk-rs327.json";
const string Matrix = "docs/engineering/m8-w02-c01-official-delta-candidates-rs316.json";
const string Expected = "MATH-RS329-FOUNDATION|id=dw.tools.math.foundation/0.1|PASS";
var original = File.ReadAllText(Source);
var tracked = new[] { Source, Project, MigrationProfile, Matrix,
    "global.json", "Directory.Build.props" };
var hashes = tracked.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var project = XDocument.Load(Project);
Need(Field(project, "PackageId") == "dw.tools.math.foundation" &&
    Field(project, "IsPackable") == "true" &&
    !project.Descendants().Any(x => x.Name.LocalName == "ProjectReference"),
    "The selected Foundation source must be a leaf package.");
Need(original.Contains("public const string ContractVersion = \"0.1\";", StringComparison.Ordinal) &&
    original.Contains("public const string PackageId = \"dw.tools.math.foundation\";", StringComparison.Ordinal),
    "The source public contract changed.");

using (var matrix = JsonDocument.Parse(File.ReadAllText(Matrix)))
using (var profile = JsonDocument.Parse(File.ReadAllText(MigrationProfile)))
using (var sdk = JsonDocument.Parse(File.ReadAllText("global.json")))
{
    var rows = matrix.RootElement.GetProperty("packages").EnumerateArray()
        .Where(x => x.GetProperty("packageId").GetString() == "dw.tools.math.foundation").ToArray();
    Need(rows.Length == 1 &&
        rows[0].GetProperty("compatibility").GetString() == "unqualified" &&
        rows[0].GetProperty("proposalCandidateIds").GetArrayLength() == 0 &&
        profile.RootElement.GetProperty("status").GetString() == "blocked-not-authorized" &&
        profile.RootElement.GetProperty("gates").GetProperty("migrationAuthorization").GetString() ==
            "not-granted" &&
        sdk.RootElement.GetProperty("sdk").GetProperty("version").GetString() == "10.0.401" &&
        Field(XDocument.Load("Directory.Build.props"), "TargetFramework") == "net10.0" &&
        Field(XDocument.Load("Directory.Build.props"), "LangVersion") == "14.0",
        "An unqualified SDK migration was incorrectly accepted.");
}
var workspace = Path.GetFullPath(Path.Combine("build", "artifacts",
    "f329-" + Guid.NewGuid().ToString("N")[..8]));
const string ProgramTemplate = """
    using Dw.Tools.Math.Foundation;
    if (Environment.Version.Major != __MAJOR__ ||
        FoundationContract.PackageId != "dw.tools.math.foundation" ||
        FoundationContract.ContractVersion != "0.1" ||
        FoundationContract.Identity != "dw.tools.math.foundation/0.1")
    {
        Console.Error.WriteLine("MATH-RS329-FOUNDATION-REFUSED");
        return 42;
    }
    Console.WriteLine("MATH-RS329-FOUNDATION|id=dw.tools.math.foundation/0.1|PASS");
    return 0;
    """;
var outputs = new string[2];
string net11 = "";
for (int major = 10; major <= 11; major++)
{
    var root = Path.Combine(workspace, "x" + major);
    Directory.CreateDirectory(root);
    File.WriteAllText(Path.Combine(root, "global.json"), major == 10
        ? "{\"sdk\":{\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}}\n"
        : "{\"sdk\":{\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}}\n");
    File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
        "<Project><PropertyGroup><TargetFramework>net" + major + ".0</TargetFramework>" +
        "<LangVersion>" + (major == 10 ? "14.0" : "15.0") + "</LangVersion>" +
        "<Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings>" +
        "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
        "<BaseOutputPath>out/bin/</BaseOutputPath>" +
        "<BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath>" +
        "</PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(root, "NuGet.Config"),
        "<configuration><packageSources><clear /></packageSources></configuration>\n");
    File.WriteAllText(Path.Combine(root, "consumer.csproj"),
        "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
        "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
        "</PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(root, "FoundationContract.cs"), original);
    File.WriteAllText(Path.Combine(root, "Program.cs"),
        ProgramTemplate.Replace("__MAJOR__", major.ToString(), StringComparison.Ordinal));
    Need(Hash(Path.Combine(root, "FoundationContract.cs")) == hashes[Source],
        "The isolated Foundation source copy was not exact.");

    var sdk = Execute(root, "--version");
    Need(sdk.ExitCode == 0 &&
        (major == 10 ? sdk.Output.Trim().StartsWith("10.0.", StringComparison.Ordinal)
                     : sdk.Output.Trim() == "11.0.100"),
        "Wrong isolated SDK for net" + major + ": " + sdk.Output);
    Pass(root, "restore", "consumer.csproj", "--configfile", "NuGet.Config");
    Pass(root, "build", "consumer.csproj", "-c", "Release", "--no-restore");
    outputs[major - 10] = Pass(root, "run", "--project", "consumer.csproj",
        "-c", "Release", "--no-build", "--no-restore").Trim();
    Need(outputs[major - 10] == Expected, "Foundation identity differs on net" + major);
    if (major == 11) net11 = root;
    Console.WriteLine("MATH-RS329-FOUNDATION-TARGET|net=" + major +
        "|lang=" + (major == 10 ? 14 : 15) + "|sdk=" + sdk.Output.Trim() + "|PASS");
}
Need(outputs[0] == outputs[1], "The two isolated consumers disagree on public identity.");

int refusals = 0;
void Reject(string name, string from, string to)
{
    var source = Path.Combine(net11, "FoundationContract.cs");
    Need(original.Split(from).Length == 2, "Mutation anchor absent: " + name);
    try
    {
        File.WriteAllText(source, original.Replace(from, to, StringComparison.Ordinal));
        Pass(net11, "build", "consumer.csproj", "-c", "Release", "--no-restore");
        var result = Execute(net11, "run", "--project", "consumer.csproj",
            "-c", "Release", "--no-build", "--no-restore");
        Need(result.ExitCode != 0 &&
            result.Output.Contains("MATH-RS329-FOUNDATION-REFUSED", StringComparison.Ordinal),
            "Changed source escaped the original oracle: " + name + " " + result.Output);
        refusals++;
        Console.WriteLine("MATH-RS329-FOUNDATION-NEGATIVE|" + name + "|PASS");
    }
    finally
    {
        File.WriteAllText(source, original);
    }
    Need(Hash(source) == hashes[Source], "Copied Foundation was not restored.");
    Pass(net11, "build", "consumer.csproj", "-c", "Release", "--no-restore");
    Need(Pass(net11, "run", "--project", "consumer.csproj",
        "-c", "Release", "--no-build", "--no-restore").Trim() == Expected,
        "Restored Foundation did not satisfy the public contract.");
}
Reject("contract-version-drift",
    "public const string ContractVersion = \"0.1\";",
    "public const string ContractVersion = \"0.2\";");
Reject("package-identity-drift",
    "public const string PackageId = \"dw.tools.math.foundation\";",
    "public const string PackageId = \"dw.tools.math.foundation.forged\";");
Need(refusals == 2, "Two controlled source-only refusals are required.");

foreach (var item in hashes)
    Need(Hash(item.Key) == item.Value, "Original Math authority changed: " + item.Key);
Console.WriteLine("MATH-RS329-FOUNDATION-CROSS-SDK|net10=PASS|net11=PASS|" +
    "csharp15Compilation=PASS|publicContractParity=PASS|" +
    "negativeControls=2|cleanRestored=2|otherTwelve=unqualified|" +
    "migrationAuthorization=not-granted|productionMutation=false|PASS");

static string? Field(XDocument doc, string name) =>
    doc.Descendants().FirstOrDefault(x => x.Name.LocalName == name)?.Value.Trim();
static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool accepted, string reason)
{
    if (!accepted) throw new InvalidDataException("RS329: " + reason);
}
static string Pass(string root, params string[] args)
{
    var result = Execute(root, args);
    Need(result.ExitCode == 0,
        "Isolated dotnet " + string.Join(" ", args) + " failed: " + result.Output);
    return result.Output;
}
static ProcessResult Execute(string root, params string[] args)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS329: Could not launch isolated dotnet.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(130000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS329: Isolated compilation timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + stderr.GetAwaiter().GetResult());
}
sealed record ProcessResult(int ExitCode, string Output);
