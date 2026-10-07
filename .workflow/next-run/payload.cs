using System.Diagnostics;

var payload = PayloadContext.Create();

var repositoryFiles = new[]
{
    "global.json",
    "Directory.Build.props",
    "Directory.Packages.props",
    "NuGet.config",
    "dw.tools.math.slnx",
    "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj",
    "src/projects/dw.tools.math.foundation/FoundationContract.cs",
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.tools.math.foundation.tests/M0W01C01Tests.cs",
    "tests/consumers/foundation-smoke/Directory.Build.props",
    "tests/consumers/foundation-smoke/Directory.Packages.props",
    "tests/consumers/foundation-smoke/foundation-smoke.csproj",
    "tests/consumers/foundation-smoke/Program.cs",
    "scripts/common.ps1",
    "scripts/restore.ps1",
    "scripts/build.ps1",
    "scripts/test.ps1",
    "scripts/pack.ps1",
    "scripts/verify-consumer.ps1",
    "scripts/verify-helpers.ps1",
    "scripts/verify.ps1"
};

foreach (var relativePath in repositoryFiles)
{
    payload.Files.ReplaceFromStaged("staged/" + relativePath, relativePath);
}

RunDotNet(payload.RepositoryRoot, "restore", "dw.tools.math.slnx", "--use-lock-file");

payload.Require.FileExists("src/projects/dw.tools.math.foundation/packages.lock.json");
payload.Require.FileExists("tests/projects/dw.tools.math.foundation.tests/packages.lock.json");

return payload.Complete();

static void RunDotNet(string repositoryRoot, params string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };

    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";

    foreach (var argument in arguments)
    {
        process.StartInfo.ArgumentList.Add(argument);
    }

    if (!process.Start())
    {
        throw new InvalidOperationException("Failed to start dotnet restore for lock-file generation.");
    }

    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();

    if (!process.WaitForExit(180_000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("dotnet restore exceeded the 180 second lock-generation budget.");
    }

    var stdout = stdoutTask.GetAwaiter().GetResult();
    var stderr = stderrTask.GetAwaiter().GetResult();

    if (!string.IsNullOrWhiteSpace(stdout))
    {
        Console.Write(stdout);
    }

    if (!string.IsNullOrWhiteSpace(stderr))
    {
        Console.Error.Write(stderr);
    }

    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException("dotnet restore failed while generating package lock files with exit code " + process.ExitCode + ".");
    }
}
