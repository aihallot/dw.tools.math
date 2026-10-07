using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

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

ConvergeProjectProgress(payload);

return payload.Complete();

static void ConvergeProjectProgress(PayloadContext payload)
{
    const string milestone = "M0";
    const string workPackage = "M0-W01";
    const string phase = "M0-W01-C01";
    const string task = "M0-W01-C01-T1";

    var states = ReadProjectStates(payload.RepositoryRoot, milestone, workPackage, phase, task);
    var baseline =
        states[milestone] == "ready" &&
        states[workPackage] == "ready" &&
        states[phase] == "ready" &&
        states[task] == "ready";
    var target =
        states[milestone] == "in-progress" &&
        states[workPackage] == "in-progress" &&
        states[phase] == "done" &&
        states[task] == "done";

    if (baseline)
    {
        payload.ProjectPlan.TransitionNode(milestone, "ready", "in-progress");
        payload.ProjectPlan.TransitionNode(workPackage, "ready", "in-progress");
        payload.ProjectPlan.TransitionNode(phase, "ready", "in-progress");
        payload.ProjectPlan.TransitionNode(task, "ready", "in-progress");
        payload.ProjectPlan.ConvergeNodeToDone(task);
        payload.ProjectPlan.ConvergeNodeToDone(phase);
        return;
    }

    if (target)
    {
        payload.ProjectPlan.RequireNodeState(milestone, "in-progress");
        payload.ProjectPlan.RequireNodeState(workPackage, "in-progress");
        payload.ProjectPlan.RequireNodeState(phase, "done");
        payload.ProjectPlan.RequireNodeState(task, "done");
        return;
    }

    throw new InvalidOperationException(
        "Project progress is neither the declared RS001 baseline nor its completed target. " +
        $"Observed {milestone}={states[milestone]}, {workPackage}={states[workPackage]}, " +
        $"{phase}={states[phase]}, {task}={states[task]}.");
}

static Dictionary<string, string> ReadProjectStates(
    string repositoryRoot,
    params string[] nodeIds)
{
    var path = Path.Combine(repositoryRoot, ".aura", "workflow", "plan", "project.json");
    var root = JsonNode.Parse(File.ReadAllText(path))
        ?? throw new InvalidOperationException("Project plan is empty.");
    var wanted = nodeIds.ToHashSet(StringComparer.Ordinal);
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    Visit(root, wanted, result);

    foreach (var nodeId in nodeIds)
    {
        if (!result.ContainsKey(nodeId))
            throw new InvalidOperationException($"Project node '{nodeId}' is missing.");
    }

    return result;
}

static void Visit(
    JsonNode? node,
    IReadOnlySet<string> wanted,
    IDictionary<string, string> result)
{
    if (node is JsonObject obj)
    {
        var id = obj["id"]?.GetValue<string>();
        if (id is not null && wanted.Contains(id))
        {
            result[id] = obj["state"]?.GetValue<string>()
                ?? throw new InvalidOperationException($"Project node '{id}' has no state.");
        }

        foreach (var property in obj)
            Visit(property.Value, wanted, result);
        return;
    }

    if (node is JsonArray array)
    {
        foreach (var item in array)
            Visit(item, wanted, result);
    }
}

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
        throw new InvalidOperationException(
            "dotnet restore failed while generating package lock files with exit code " +
            process.ExitCode + ".");
    }
}
