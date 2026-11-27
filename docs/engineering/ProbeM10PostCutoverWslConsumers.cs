#:property PublishAot=false
using System.Diagnostics;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Expected Windows-to-WSL qualification.");
const string Consumer = "docs/engineering/ProbeM10PostCutoverLockedConsumers.cs";
var root = Path.GetFullPath(".");
Need(File.Exists(Consumer), "Qualified shared consumer recipe missing.");
string Wsl(int timeout, params string[] arguments)
{
    var start = new ProcessStartInfo("wsl.exe")
    {
        UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true
    };
    foreach (var argument in arguments) start.ArgumentList.Add(argument);
    using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot start WSL.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(timeout))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("WSL timed out: " + string.Join(" ", arguments));
    }
    var output = stdout.GetAwaiter().GetResult().Trim();
    var error = stderr.GetAwaiter().GetResult();
    if (process.ExitCode != 0) throw new InvalidOperationException("WSL failed: " + string.Join(" ", arguments) + "\n" + output + "\n" + error);
    if (!string.IsNullOrWhiteSpace(error)) Console.WriteLine(error);
    return output;
}
var linuxRoot = Wsl(60000, "--exec", "wslpath", "-a", "-u", root);
Need(linuxRoot.StartsWith("/", StringComparison.Ordinal) && !linuxRoot.Contains('\n') && !linuxRoot.Contains('\r'), "WSL path mapping failed.");
string RunLinux(int timeout, params string[] arguments) =>
    Wsl(timeout, ["--exec", "bash", "-lc", "cd \"$1\" && shift && \"$@\"", "bash", linuxRoot, .. arguments]);
var kernel = RunLinux(60000, "uname", "-s");
var machine = RunLinux(60000, "uname", "-m");
var os = RunLinux(60000, "cat", "/etc/os-release");
Need(kernel == "Linux" && machine == "x86_64" && os.Contains("ID=", StringComparison.Ordinal), "Linux host identity differs.");
var sdk = RunLinux(60000, "dotnet", "--version");
Need(sdk == "11.0.100", "Actual Linux SDK selection differs: " + sdk);
Console.WriteLine("MATH-RS422-WSL-HOST|kernel=" + kernel + "|machine=" + machine + "|sdk=" + sdk + "|PASS");
Console.WriteLine(os);
var output = RunLinux(3600000, "dotnet", "run", "--file", Consumer, "--", "--linux");
Console.WriteLine(output);
Need(output.Contains("MATH-M10-LOCKED-CONSUMERS|sdk=11.0.100|rid=linux-x64|tfm=net11.0|consumers=2|packages=13|rebuild=false|publication=false|PASS", StringComparison.Ordinal),
    "Linux consumer proof absent.");
Need(output.Contains("MATH-M10-CONSUMER|mode=numeric|sdk=11.0.100|rid=linux-x64|packages=2|", StringComparison.Ordinal) &&
    output.Contains("MATH-M10-CONSUMER|mode=broad|sdk=11.0.100|rid=linux-x64|packages=12|", StringComparison.Ordinal) &&
    output.Contains("MATH-M10-BROAD-SEMANTICS|PASS", StringComparison.Ordinal) &&
    output.Contains("MATH-RS270-BOUNDED-PACKAGE|PASS|12", StringComparison.Ordinal), "Linux semantic/lock evidence absent.");
Console.WriteLine("MATH-RS422-WSL-CONSUMERS|sdk=11.0.100|rid=linux-x64|packages=13|consumers=2|recipe=RS421|publication=false|PASS");

static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS422: " + message);
}
