#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string SourceDirectory = "src/projects/dw.quantities";
const string Workspace = "build/artifacts/m8-rs237-quantity-dimension-cross-runtime";
var sources = Directory.GetFiles(SourceDirectory, "*.cs", SearchOption.TopDirectoryOnly)
    .OrderBy(Path.GetFileName, StringComparer.Ordinal).ToArray();
Require(sources.Length == 9 &&
    sources.Any(p => Path.GetFileName(p) == "ExactRational.cs"),
    "Exact quantities source boundary differs from RS219.");

const string ProgramSource = """
    using System;
    using dw.quantities;

    var halfLength = new Quantity(new ExactRational(1, 2), DimensionVector.LengthDimension);
    var thirdLength = new Quantity(new ExactRational(1, 3), DimensionVector.LengthDimension);
    var unitMass = new Quantity(ExactRational.One, DimensionVector.MassDimension);
    var unitTime = new Quantity(ExactRational.One, DimensionVector.TimeDimension);
    if (!halfLength.TryAdd(thirdLength, out var sum) ||
        sum.Value.ToFractionString() != "5/6" ||
        sum.Dimension != DimensionVector.LengthDimension)
        throw new InvalidOperationException("Compatible dimension addition drift.");
    if (halfLength.TryAdd(unitMass, out _))
        throw new InvalidOperationException("Incompatible dimensions were accepted.");
    var product = halfLength.Multiply(unitMass);
    if (product.Value.ToFractionString() != "1/2" ||
        product.Dimension.Length != 1 || product.Dimension.Mass != 1)
        throw new InvalidOperationException("Dimension multiplication drift.");
    var quotient = halfLength.Divide(unitTime);
    if (quotient.Value.ToFractionString() != "1/2" ||
        quotient.Dimension.Length != 1 || quotient.Dimension.Time != -1)
        throw new InvalidOperationException("Dimension division drift.");
    var overflowRefused = false;
    try
    {
        _ = new DimensionVector(int.MaxValue, 0, 0, 0, 0, 0, 0) +
            DimensionVector.LengthDimension;
    }
    catch (OverflowException) { overflowRefused = true; }
    if (!overflowRefused)
        throw new InvalidOperationException("Dimension exponent overflow was accepted.");
    Console.WriteLine("MATH-RS237-QUANTITY|compatible=5/6|incompatible=refused|" +
        "multiply=1/2:length+mass|divide=1/2:length-time|" +
        "overflow=refused|PASS");
    """;
const string Expected = "MATH-RS237-QUANTITY|compatible=5/6|incompatible=refused|" +
    "multiply=1/2:length+mass|divide=1/2:length-time|overflow=refused|PASS";
var results = new Dictionary<int, string>();
foreach (var major in new[] { 10, 11 })
{
    var root = Path.GetFullPath(Path.Combine(Workspace, "net" + major));
    Directory.CreateDirectory(root);
    File.WriteAllText(Path.Combine(root, "global.json"),
        major == 10
            ? "{\n \"sdk\": {\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}\n}\n"
            : "{\n \"sdk\": {\"version\":\"11.0.100\",\"rollForward\":\"disable\",\"allowPrerelease\":false}\n}\n");
    File.WriteAllText(Path.Combine(root, "Directory.Build.props"),
        "<Project><PropertyGroup><TargetFramework>net" + major + ".0</TargetFramework>" +
        "<LangVersion>" + (major == 10 ? "14.0" : "15.0") + "</LangVersion>" +
        "<Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings>" +
        "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>" +
        "</PropertyGroup></Project>\n");
    File.WriteAllText(Path.Combine(root, "NuGet.Config"),
        "<configuration><packageSources><clear /></packageSources></configuration>\n");
    File.WriteAllText(Path.Combine(root, "consumer.csproj"),
        "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
        "<IsPackable>false</IsPackable><NuGetAudit>false</NuGetAudit>" +
        "<RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>" +
        "</PropertyGroup></Project>\n");
    foreach (var file in sources)
        File.Copy(file, Path.Combine(root, Path.GetFileName(file)), overwrite: true);
    File.WriteAllText(Path.Combine(root, "Program.cs"), ProgramSource);
    string Run(params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = root, UseShellExecute = false,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = Process.Start(start) ??
            throw new InvalidOperationException("RS237: cannot start dotnet.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(120000) || process.ExitCode != 0)
            throw new InvalidOperationException("RS237 dotnet " +
                string.Join(" ", arguments) + " failed:\n" + stdout + "\n" + stderr);
        return stdout.Trim();
    }
    var sdk = Run("--version");
    Require(sdk.StartsWith(major + ".0.", StringComparison.Ordinal) &&
        (major != 11 || sdk == "11.0.100"),
        "Unexpected isolated SDK " + sdk + " for net" + major + ".");
    Run("restore", "consumer.csproj", "--configfile", "NuGet.Config");
    Run("build", "consumer.csproj", "-c", "Release", "--no-restore");
    var result = Run("run", "--project", "consumer.csproj",
        "-c", "Release", "--no-build", "--no-restore");
    Require(result == Expected, "Mismatch against independent canonical oracle on net" + major);
    results.Add(major, result);
    Console.WriteLine("MATH-RS237-RUNTIME|net=" + major +
        "|sdk=" + sdk + "|sourceFiles=9|oracle=matched|PASS");
}
Require(results[10] == results[11], "Cross-runtime exact arithmetic contract drift.");
Console.WriteLine("MATH-RS237-CROSS-RUNTIME|net10=pass|net11=pass|" +
    "quantityDimensionOracle=matched|published=0|productionMutation=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS237: " + message);
}
