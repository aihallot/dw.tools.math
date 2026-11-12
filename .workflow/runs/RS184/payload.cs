using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Project = "tests/consumers/m8-discovery-clr-seed/m8-discovery-clr-seed.csproj";
const string Program = "tests/consumers/m8-discovery-clr-seed/Program.cs";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS184", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS183", 1,
    "docs: qualify .NET 11 numeric evolution and Math refactor opportunities",
    "0.1.66", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M7", "done");
p.ProjectPlan.RequireNodeState("M8", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01", "not-ready");
p.ProjectPlan.RequireNodeState("M8-W01-C01", "not-ready");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.123"));
p.Require.JsonValueEquals("docs/discovery/qualification/RS131-portable-protocol-qualified.json",
    "/total_cases", JsonValue.Create(13));
p.Require.JsonValueEquals("docs/discovery/qualification/RS131-portable-protocol-qualified.json",
    "/csharp_portable_envelope_conformant", JsonValue.Create(true));
p.Files.ReplaceFromStaged("staged/" + Project, Project);
p.Files.ReplaceFromStaged("staged/" + Program, Program);
return p.Complete();
