using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS272", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS271", 1,
    "test: qualify bounded Statistics private release readiness",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M3-W02-C03", "done");
p.ProjectPlan.RequireNodeState("M9", "not-ready");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json",
    "/plan_version", JsonValue.Create("0.1.124"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m9-bounded-statistics-private-publish-authorization.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/VerifyM9BoundedStatisticsPrivateFeed.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tools/dw.tools.math.nuget-bounded-statistics-release/Directory.Packages.props"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tools/dw.tools.math.nuget-bounded-statistics-release/dw.tools.math.nuget-bounded-statistics-release.csproj"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("tools/dw.tools.math.nuget-bounded-statistics-release/Program.cs"));
return p.Complete();
