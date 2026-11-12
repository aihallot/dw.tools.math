using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS182", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS181", 1,
    "docs: qualify exact 39-node M8 admission design before native insertion",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/proposals/m8-admission-contract.json",
    "/admission/expectedTotalNativeNodes", JsonValue.Create(580));
p.Require.JsonValueEquals("docs/planning/evidence/RS180-dwf-native-topology-capability.json",
    "/supportedForNewMathMilestones", JsonValue.Create(true));

const string Backlog = "docs/planning/backlog.json";
const string Validator = "docs/planning/ValidateNativeDwfAdoption.cs";
p.Files.ReplaceFromStaged("staged/" + Backlog, Backlog);
p.ProjectPlan.EnsureMilestoneSubtree(new PayloadProjectPlanNodeSpec("M8","Foundation and Discovery", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01","Independent Math Discovery", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C01","Selected CLR bindings and portable protocol", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C01-T1","Pinned protocol and conformance", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C01-T1-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C01-T1-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C01-T1-V","Verification", "not-ready", null, null) }, null), new PayloadProjectPlanNodeSpec("M8-W01-C01-T2","Bound selected CLR operations", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C01-T2-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C01-T2-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C01-T2-V","Verification", "not-ready", null, null) }, null) }, null), new PayloadProjectPlanNodeSpec("M8-W01-C02","Deterministic skill generator and qualification", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C02-T1","Skill generator contract", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C02-T1-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C02-T1-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C02-T1-V","Verification", "not-ready", null, null) }, null), new PayloadProjectPlanNodeSpec("M8-W01-C02-T2","Skill delivery and refusal boundary", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W01-C02-T2-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C02-T2-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W01-C02-T2-V","Verification", "not-ready", null, null) }, null) }, new string[] { "M8-W01-C01" }) }, null), new PayloadProjectPlanNodeSpec("M8-W02","Independent .NET 11 and C# 15 modernization", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C01","SDK baseline and modernization audit", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C01-T1","Official API and toolchain inventory", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C01-T1-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C01-T1-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C01-T1-V","Verification", "not-ready", null, null) }, null), new PayloadProjectPlanNodeSpec("M8-W02-C01-T2","Delta risk and package graph", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C01-T2-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C01-T2-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C01-T2-V","Verification", "not-ready", null, null) }, null) }, null), new PayloadProjectPlanNodeSpec("M8-W02-C02","Targeted migration and external consumer proof", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C02-T1","Bounded modern language and framework changes", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C02-T1-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C02-T1-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C02-T1-V","Verification", "not-ready", null, null) }, null), new PayloadProjectPlanNodeSpec("M8-W02-C02-T2","Package and independent client verification", "not-ready", new PayloadProjectPlanNodeSpec[] { new PayloadProjectPlanNodeSpec("M8-W02-C02-T2-R","RED", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C02-T2-G","GREEN", "not-ready", null, null), new PayloadProjectPlanNodeSpec("M8-W02-C02-T2-V","Verification", "not-ready", null, null) }, null) }, new string[] { "M8-W02-C01" }) }, null) }, new string[] { "M7" }));
p.Files.ReplaceFromStaged("staged/" + Validator, Validator);
p.Require.JsonValueEquals(Backlog, "/plan_version", JsonValue.Create("0.1.123"));
p.ProjectPlan.RequireNodeState("M8", "not-ready");
RunPlanningGenerator();
p.Require.JsonValueEquals("docs/planning/dwf-map.json", "/plan_version", JsonValue.Create("0.1.123"));
return p.Complete();

static void RunPlanningGenerator()
{
    var start = new ProcessStartInfo("dotnet")
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = Environment.CurrentDirectory
    };
    foreach (var arg in new[] { "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write" })
        start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot start planning generator.");
    var output = process.StandardOutput.ReadToEnd();
    var errors = process.StandardError.ReadToEnd();
    if (!process.WaitForExit(180000) || process.ExitCode != 0)
    {
        if (!process.HasExited) process.Kill(entireProcessTree: true);
        throw new InvalidOperationException("Planning generator failed: " + output + errors);
    }
    Console.WriteLine(output);
}
