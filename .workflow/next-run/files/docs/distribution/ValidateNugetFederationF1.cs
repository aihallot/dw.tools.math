#:property NuGetAudit=false
#:property PublishAot=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

const string Receipt = "docs/distribution/nuget-federation-f1-qualified.json";
const string Plan = ".aura/workflow/plan/project.json";
const string ToolRoot = "tools/dw.tools.math.nuget-preflight/";
const string Status = "rs134-math-f1-calendar-feed-inventory-no-publication";
static void Need(bool value, string issue)
{
    if (!value) throw new InvalidOperationException("Math NuGet F1: " + issue);
}
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string Value(JsonElement node, string key) =>
    node.GetProperty(key).GetString() ??
    throw new InvalidOperationException("Missing " + key);
static bool Flag(JsonElement node, string key) => node.GetProperty(key).GetBoolean();
static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
static int Count(JsonElement element)
{
    var count = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (element.TryGetProperty(name, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
     "Only --check is a supported mode");
using var backlog = Read("docs/planning/backlog.json");
using var plan = Read(Plan);
using var previous = Read(".aura/workflow/reports/RS133/attempt-001.json");
using var guidance = Read("docs/workflow/manifest.json");
Need(Value(backlog.RootElement, "plan_version") == "0.1.122" &&
     Value(backlog.RootElement, "status") == "done" &&
     backlog.RootElement.GetProperty("work_packages").GetArrayLength() == 17,
     "accepted product backlog changed");
Need(plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => Value(x, "state") == "done"),
     "accepted native plan must remain 541 done");
Need(Value(previous.RootElement, "runId") == "RS133" &&
     previous.RootElement.GetProperty("attempt").GetInt32() == 1 &&
     Value(previous.RootElement, "outcome") == "succeeded" &&
     previous.RootElement.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32() == 1 &&
     !Flag(previous.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded"),
     "RS133 lineage changed");
Need(Value(guidance.RootElement, "guidanceVersion") == "0.26",
     "guidance must be canonical 0.26");
string[] files =
[
    "docs/distribution/nuget-federation-f1.md",
    ToolRoot + "dw.tools.math.nuget-preflight.csproj",
    ToolRoot + "Directory.Packages.props",
    ToolRoot + "Program.cs",
    "docs/distribution/ValidateNugetFederationF1.cs"
];
foreach (var file in files)
    Need(File.Exists(file), "missing source: " + file);
var projectText = File.ReadAllText(ToolRoot + "dw.tools.math.nuget-preflight.csproj");
var centralText = File.ReadAllText(ToolRoot + "Directory.Packages.props");
Need(projectText.Contains("<PackageReference Include=\"Dw.Tools.NuGet\" />",
        StringComparison.Ordinal) &&
     centralText.Contains(
         "<PackageVersion Include=\"Dw.Tools.NuGet\" Version=\"[20260.20261026.1010]\" />",
         StringComparison.Ordinal), "exact central package reference must be pinned");
if (File.Exists(Receipt))
{
    using var record = Read(Receipt);
    var result = record.RootElement;
    Need(Value(result, "status") == Status &&
         Value(result, "feed_selector") == "NUGET_LOCAL" &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         !Flag(result, "package_published") &&
         !Flag(result, "release_authorized_by_this_run") &&
         !Flag(result, "external_requests_transmitted") &&
         !Flag(result, "source_project_versions_changed"),
         "F1 receipt cannot claim a package release or roadmap mutation");
    var sourceFiles = result.GetProperty("source_files");
    foreach (var file in files)
        Need(sourceFiles.GetProperty(file).GetString() == Sha(file),
             "staged source integrity drift: " + file);
    var observations = result.GetProperty("observed");
    Need(Value(observations, "status") ==
         "f1-source-and-feed-inventory-qualified-not-published" &&
         Value(observations, "feedSource") == "NUGET_LOCAL" &&
         Value(observations, "repositoryTool") == "Dw.Tools.NuGet" &&
         Value(observations, "repositoryPackageVersion") == "20260.20261026.1010" &&
         Value(observations, "repositoryPackageSha256") ==
         "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf" &&
         Value(observations, "loadedLibrarySha256") ==
         "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61" &&
         observations.GetProperty("packageCount").GetInt32() == 13 &&
         observations.GetProperty("msbuildPropertyQueries").GetInt32() == 13 &&
         !Flag(observations, "versionsReserved") &&
         !Flag(observations, "feedPublished"),
         "as-run shared NuGet library identity or observation is incorrect");
    var entries = observations.GetProperty("packages").EnumerateArray().ToArray();
    Need(entries.Length == 13 && entries.Select(x => Value(x, "packageId"))
            .Distinct(StringComparer.OrdinalIgnoreCase).Count() == 13,
         "Math package inventory must be unique and complete");
    foreach (var item in entries)
    {
        Need(Value(item, "effectiveTargetFramework") == "net10.0" &&
             Value(item, "effectivePackable") == "true" &&
             Value(item, "effectivePackageVersion") ==
             Value(item, "historicalSourceVersion") &&
             !Flag(item, "candidateIsReserved") &&
             Regex.IsMatch(Value(item, "candidateVersionPreview"),
                 @"^20260\.2026[0-9]{4}\.1[0-9]{3}$",
                 RegexOptions.CultureInvariant),
             "source version or bounded calendar candidate is unqualified");
    }
    var commands = result.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(commands.Length == 5 &&
         commands.All(c => c.GetProperty("exit_code").GetInt32() == 0) &&
         commands.Select(c => Value(c, "id")).SequenceEqual(
         ["restore", "build", "self-test", "inspect", "sdk"], StringComparer.Ordinal),
         "command receipts must identify five real successful processes");
}
Console.WriteLine("MATH-NUGET-F1:SOURCE=13;EFFECTIVE-MSBUILD=13;FEDERATED-CORE=1010;NATIVE=541");
Console.WriteLine("MATH-NUGET-F1:VERSION-PREVIEW=true;PUBLICATION=false;MIGRATION=false");
return 0;
