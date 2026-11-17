#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("RS240 is an explicit Windows-to-WSL host qualification.");

string RunLinux(string command)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo("wsl.exe")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.ArgumentList.Add("--exec");
    process.StartInfo.ArgumentList.Add("bash");
    process.StartInfo.ArgumentList.Add("-lc");
    process.StartInfo.ArgumentList.Add(command);
    if (!process.Start())
        throw new InvalidOperationException("RS240: could not start the default WSL distro.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(60000) || process.ExitCode != 0)
        throw new InvalidOperationException("RS240 WSL command failed: " + command +
            "\n" + stdout + "\n" + stderr);
    return stdout.Trim();
}

var os = RunLinux("cat /etc/os-release");
Require(os.Contains("ID=ubuntu", StringComparison.Ordinal) &&
        os.Contains("VERSION_ID=\"24.04\"", StringComparison.Ordinal),
    "Default WSL distro is not the observed Ubuntu 24.04 baseline.");
var machine = RunLinux("uname -m");
Require(machine == "x86_64", "Default WSL architecture is not Linux x64.");
var sdks = RunLinux("dotnet --list-sdks");
Require(sdks.Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Any(line => line.StartsWith("10.0.401 ", StringComparison.Ordinal)) &&
        sdks.Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Any(line => line.StartsWith("11.0.100 ", StringComparison.Ordinal)),
    "WSL does not expose the exact SDK 10.0.401 and 11.0.100 baselines.");
// A WSL invocation inherited from a Windows checkout can resolve its global.json.
// Check the default Linux toolchain from a neutral home directory instead.
var selected = RunLinux("cd \"$HOME\" && dotnet --version");
Require(selected == "11.0.100", "Default WSL home-directory SDK selection is not 11.0.100.");
var runtimes = RunLinux("dotnet --list-runtimes");
Require(runtimes.Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Any(line => line.StartsWith("Microsoft.NETCore.App 10.", StringComparison.Ordinal)) &&
        runtimes.Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Any(line => line.StartsWith("Microsoft.NETCore.App 11.", StringComparison.Ordinal)),
    "WSL host lacks required 10.x or 11.x .NET runtimes.");
Console.WriteLine("MATH-RS240-WSL-HOST|distro=ubuntu-24.04|arch=linux-x64|" +
    "sdk10=10.0.401|sdk11=11.0.100|runtime10=present|runtime11=present|" +
    "mathConsumerQualified=false|mutation=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS240: " + message);
}
