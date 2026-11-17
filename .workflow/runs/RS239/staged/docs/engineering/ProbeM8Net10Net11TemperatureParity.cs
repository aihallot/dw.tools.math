#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string SourceDirectory = "src/projects/dw.quantities";
const string Workspace = "build/artifacts/m8-rs239-temperature-cross-runtime";
var sources = Directory.GetFiles(SourceDirectory, "*.cs", SearchOption.TopDirectoryOnly)
    .OrderBy(Path.GetFileName, StringComparer.Ordinal).ToArray();
Require(sources.Length == 9 &&
    sources.Any(p => Path.GetFileName(p) == "ExactRational.cs"),
    "Exact quantities source boundary differs from RS219.");

const string ProgramSource = """
    using System;
    using dw.quantities;

    var celsius = new UnitDefinition(
        "C", "C", "Celsius", UnitSystem.Si, DimensionVector.TemperatureDimension,
        ExactRational.One, ExactRational.ParseInvariant("273.15"),
        UnitTransformKind.AbsoluteTemperature);
    var fahrenheit = new UnitDefinition(
        "F", "F", "Fahrenheit", UnitSystem.UsCustomary,
        DimensionVector.TemperatureDimension,
        new ExactRational(5, 9), new ExactRational(45967, 180),
        UnitTransformKind.AbsoluteTemperature);
    var freezing = celsius.ToBase(TemperatureMeasurement.Absolute(ExactRational.Zero));
    var waterFreezingF = fahrenheit.FromBase(freezing);
    var interval = celsius.ToBase(TemperatureMeasurement.Interval(new ExactRational(10, 1)));
    var intervalF = fahrenheit.FromBase(interval);
    if (freezing.Kind != TemperatureKind.Absolute ||
        freezing.Value.ToFractionString() != "5463/20" ||
        waterFreezingF.Kind != TemperatureKind.Absolute ||
        waterFreezingF.Value.ToFractionString() != "32" ||
        intervalF.Kind != TemperatureKind.Interval ||
        intervalF.Value.ToFractionString() != "18")
        throw new InvalidOperationException("Affine temperature conversion drift.");
    var absoluteSumRefused = false;
    try { _ = freezing.Add(waterFreezingF); }
    catch (InvalidOperationException) { absoluteSumRefused = true; }
    var invalidDifferenceRefused = false;
    try { _ = interval.Subtract(freezing); }
    catch (InvalidOperationException) { invalidDifferenceRefused = true; }
    if (!absoluteSumRefused || !invalidDifferenceRefused)
        throw new InvalidOperationException("Temperature arithmetic refused boundary drift.");
    Console.WriteLine("MATH-RS239-TEMPERATURE|freezingK=5463/20|" +
        "freezingF=32|delta10C=18F|absoluteSum=refused|" +
        "intervalMinusAbsolute=refused|PASS");
    """;
const string Expected = "MATH-RS239-TEMPERATURE|freezingK=5463/20|" +
    "freezingF=32|delta10C=18F|absoluteSum=refused|" +
    "intervalMinusAbsolute=refused|PASS";
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
            throw new InvalidOperationException("RS239: cannot start dotnet.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(120000) || process.ExitCode != 0)
            throw new InvalidOperationException("RS239 dotnet " +
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
    Console.WriteLine("MATH-RS239-RUNTIME|net=" + major +
        "|sdk=" + sdk + "|sourceFiles=9|oracle=matched|PASS");
}
Require(results[10] == results[11], "Cross-runtime exact arithmetic contract drift.");
Console.WriteLine("MATH-RS239-CROSS-RUNTIME|net10=pass|net11=pass|" +
    "temperatureOracle=matched|published=0|productionMutation=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS239: " + message);
}
