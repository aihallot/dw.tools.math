using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var p = PayloadContext.Create();
p.Workflow.RequirePriorSuccess("RS366", 1, "docs: reconcile completed Math roadmap and delivery status", "0.1.78", 0, 0, 0, true);
p.ProjectPlan.RequireNodeState("M8", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1-V", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T1", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-R", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-G", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2-V", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01-T2", "done");
p.ProjectPlan.RequireNodeState("M9-W01-C01", "done");
p.ProjectPlan.RequireNodeState("M9-W01", "done");
p.ProjectPlan.RequireNodeState("M9", "done");
p.ProjectPlan.RequireNodeState("M3", "done");
p.Require.JsonValueBaselineOrTarget("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.142"), JsonValue.Create("0.1.143"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.31"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateNativeDwfAdoption.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateAssessaM9Admission.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9AssessaPublishedHandoff.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/engineering/ValidateM9RequestDeliveryReconciliation.cs"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("README.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/decisions/m10-production-net11-migration-admission-rs367.md"));
p.Files.ReplaceFromStagedMirror(PayloadPath.RepositoryFile("docs/planning/ValidateM10MigrationAdmission.cs"));
static void AddExact(JsonObject doc, string key, JsonObject entry, string identityKey, bool target)
{
    var arr = doc[key]!.AsArray();
    var id = (string?)entry[identityKey];
    var matches = arr.Where(x => (string?)x?[identityKey] == id).ToArray();
    if (target)
    {
        if (matches.Length != 1 || !JsonNode.DeepEquals(matches[0], entry)) throw new InvalidOperationException("Unexpected target entry: " + id);
        return;
    }
    if (matches.Length != 0) throw new InvalidOperationException("Unexpected baseline entry: " + id);
    arr.Add(entry);
}
p.Json.EditObject("docs/planning/backlog.json", doc =>
{
    var version = (string?)doc["plan_version"];
    if (version is not ("0.1.142" or "0.1.143")) throw new InvalidOperationException("Backlog version differs.");
    var target = version == "0.1.143";
    if ((string?)doc["status"] != (target ? "in_progress" : "done")) throw new InvalidOperationException("Unexpected aggregate baseline.");
    AddExact(doc, "releases", JsonNode.Parse("""
{
  "id": "M10",
  "version": "1.5.0-preview",
  "title": "Production .NET 11 and C# 15 migration",
  "outcome": "Migrate the actual Math production source and maintained build graph to SDK 11.0.100, net11.0 and C# 15 with evidence-backed compatibility and rollback boundaries.",
  "status": "planned",
  "gate": "G-M10",
  "required_chunks": [
    "M10-W01-C01",
    "M10-W01-C02",
    "M10-W01-C03"
  ],
  "depends_on": [
    "M8",
    "M9"
  ],
  "evidence": []
}
""")!.AsObject(), "id", target);
    AddExact(doc, "work_packages", JsonNode.Parse("""
{
  "id": "M10-W01",
  "release": "M10",
  "title": "Controlled production framework cutover",
  "status": "planned",
  "paths": [
    "global.json",
    "Directory.Build.props",
    "Directory.Packages.props",
    "dw.tools.math.slnx",
    "src/projects/",
    "tests/projects/",
    "tests/consumers/",
    "tools/",
    "docs/engineering/"
  ],
  "path_policy": "Audit actual source graph and historical fixtures before changing central defaults; preserve immutable package versions and historical probes.",
  "evidence": []
}
""")!.AsObject(), "id", target);
    AddExact(doc, "chunks", JsonNode.Parse("""
{
  "id": "M10-W01-C01",
  "tag": "modernization",
  "release": "M10",
  "work_package": "M10-W01",
  "title": "Cutover readiness and rollback baseline",
  "kind": "spike",
  "size": "M",
  "status": "planned",
  "depends_on": [
    "M8-W02-C02",
    "M9-W01-C01"
  ],
  "external_dependencies": [],
  "requirements": [
    "M10-W01-C01-R1"
  ],
  "deliverables": "Inventory current production and test project graph, SDK/TFM/language overrides, lockfiles, maintained tools and consumers; establish a reproducible isolated target graph and exact rollback baseline using M8 evidence.",
  "acceptance": "Actual target SDK selection and restored isolated source graph; fragile exact/numerical/replay tests; classify every active and historical project/consumer and every remaining migration risk; verify a clean rollback recipe.",
  "boundary_acceptance": "No production SDK or source change in this phase; old archives and historical evidence stay immutable. No assumption that net11 consumers of net10 archives establish source compatibility.",
  "test_suites": [
    "CONTRACT",
    "BOUNDARY",
    "PACKAGE",
    "REPLAY"
  ],
  "proposed_files": [
    "global.json",
    "Directory.Build.props",
    "Directory.Packages.props",
    "dw.tools.math.slnx",
    "docs/engineering/m8-net11-csharp15-migration-evidence.md"
  ],
  "proposed_commands": [
    "dotnet --version",
    "dotnet restore dw.tools.math.slnx --locked-mode"
  ],
  "files": [],
  "commands": [],
  "evidence": [],
  "tasks": [
    {
      "id": "M10-W01-C01-T1",
      "title": "Qualify cutover readiness",
      "behavior": "Reconcile real current source graph, isolated target graph, fragile tests and recovery baseline before production cutover.",
      "test_case": "Reject missing projects, incompatible SDK or lock closure and unsupported rollback or source-compatibility claims.",
      "status": "planned",
      "evidence": [],
      "subtasks": [
        {
          "id": "M10-W01-C01-T1-R",
          "title": "RED",
          "description": "Falsify cutover readiness and rollback baseline with independent bounded negative controls.",
          "status": "planned",
          "evidence": []
        },
        {
          "id": "M10-W01-C01-T1-G",
          "title": "GREEN",
          "description": "Implement the minimum changes needed for cutover readiness and rollback baseline.",
          "status": "planned",
          "evidence": []
        },
        {
          "id": "M10-W01-C01-T1-V",
          "title": "Verify and document",
          "description": "Independently qualify and document cutover readiness and rollback baseline.",
          "status": "planned",
          "evidence": []
        }
      ]
    }
  ]
}
""")!.AsObject(), "id", target);
    AddExact(doc, "chunks", JsonNode.Parse("""
{
  "id": "M10-W01-C02",
  "tag": "modernization",
  "release": "M10",
  "work_package": "M10-W01",
  "title": "Production SDK and framework cutover",
  "kind": "code",
  "size": "M",
  "status": "planned",
  "depends_on": [
    "M10-W01-C01"
  ],
  "external_dependencies": [],
  "requirements": [
    "M10-W01-C02-R1"
  ],
  "deliverables": "Switch the production SDK, central framework/language configuration and affected maintained projects/lockfiles to net11.0/C# 15; make only necessary source/build corrections.",
  "acceptance": "Actual solution selects SDK 11.0.100, all thirteen production libraries and eleven solution test projects resolve intended target, locked restore and Release build succeed, exact targeted tests pass; explicit historical fixture overrides and rollback verified.",
  "boundary_acceptance": "No blanket rewrite of historic net10 reproduction fixtures, package publication or sibling changes. Do not relax warnings-as-errors or locked restore to obtain green.",
  "test_suites": [
    "CONTRACT",
    "BOUNDARY",
    "PACKAGE",
    "REPLAY"
  ],
  "proposed_files": [
    "global.json",
    "Directory.Build.props",
    "Directory.Packages.props",
    "dw.tools.math.slnx",
    "src/projects/",
    "tests/projects/",
    "tools/"
  ],
  "proposed_commands": [
    "dotnet restore dw.tools.math.slnx --locked-mode",
    "dotnet build dw.tools.math.slnx -c Release --no-restore"
  ],
  "files": [],
  "commands": [],
  "evidence": [],
  "tasks": [
    {
      "id": "M10-W01-C02-T1",
      "title": "Migrate actual production and maintained build graph",
      "behavior": "Controlled actual SDK/TFM/language switch with exact build and semantic regression evidence.",
      "test_case": "Reject net10 production fallback, stale lock targets, failed analyzers, behavior changes and unintended historical fixture drift.",
      "status": "planned",
      "evidence": [],
      "subtasks": [
        {
          "id": "M10-W01-C02-T1-R",
          "title": "RED",
          "description": "Falsify production sdk and framework cutover with independent bounded negative controls.",
          "status": "planned",
          "evidence": []
        },
        {
          "id": "M10-W01-C02-T1-G",
          "title": "GREEN",
          "description": "Implement the minimum changes needed for production sdk and framework cutover.",
          "status": "planned",
          "evidence": []
        },
        {
          "id": "M10-W01-C02-T1-V",
          "title": "Verify and document",
          "description": "Independently qualify and document production sdk and framework cutover.",
          "status": "planned",
          "evidence": []
        }
      ]
    }
  ]
}
""")!.AsObject(), "id", target);
    AddExact(doc, "chunks", JsonNode.Parse("""
{
  "id": "M10-W01-C03",
  "tag": "modernization",
  "release": "M10",
  "work_package": "M10-W01",
  "title": "Post-cutover qualification and delivery readiness",
  "kind": "code",
  "size": "M",
  "status": "planned",
  "depends_on": [
    "M10-W01-C02"
  ],
  "external_dependencies": [],
  "requirements": [
    "M10-W01-C03-R1"
  ],
  "deliverables": "Certify the migrated solution and unpublished package graph on supported local platforms, record exact package/assembly/dependency identities, consumer behavior, regression and rollback boundaries, and update current documentation.",
  "acceptance": "Targeted fragile contracts then full solution regression at migration gate; fresh isolated unpublished net11 candidates for all thirteen libraries, exact source-free locked representative consumers, Windows and Linux/WSL evidence with explicit gaps and observed SDK/RID; verify rollback and no archive overwrite.",
  "boundary_acceptance": "No new public or private publication, stable ABI promise or downstream adoption. Qualification is bounded to actual executed tests and platforms; no unsupported historical package compatibility claim.",
  "test_suites": [
    "CONTRACT",
    "BOUNDARY",
    "PACKAGE",
    "REPLAY"
  ],
  "proposed_files": [
    "dw.tools.math.slnx",
    "src/projects/",
    "tests/projects/",
    "tests/consumers/",
    "docs/distribution/",
    "docs/engineering/"
  ],
  "proposed_commands": [
    "dotnet test dw.tools.math.slnx -c Release --no-build --no-restore"
  ],
  "files": [],
  "commands": [],
  "evidence": [],
  "tasks": [
    {
      "id": "M10-W01-C03-T1",
      "title": "Verify migrated graph and independent consumers",
      "behavior": "Prove migrated source and fresh isolated candidate graph with finite regression, platform and consumer evidence.",
      "test_case": "Refuse floating dependencies, project-reference substitutes, changed immutable archives, unproved platform support and unsupported semantic equivalence.",
      "status": "planned",
      "evidence": [],
      "subtasks": [
        {
          "id": "M10-W01-C03-T1-R",
          "title": "RED",
          "description": "Falsify post-cutover qualification and delivery readiness with independent bounded negative controls.",
          "status": "planned",
          "evidence": []
        },
        {
          "id": "M10-W01-C03-T1-G",
          "title": "GREEN",
          "description": "Implement the minimum changes needed for post-cutover qualification and delivery readiness.",
          "status": "planned",
          "evidence": []
        },
        {
          "id": "M10-W01-C03-T1-V",
          "title": "Verify and document",
          "description": "Independently qualify and document post-cutover qualification and delivery readiness.",
          "status": "planned",
          "evidence": []
        }
      ]
    }
  ]
}
""")!.AsObject(), "id", target);
    AddExact(doc, "requirements", JsonNode.Parse("""
{
  "id": "M10-W01-C01-R1",
  "title": "Cutover readiness and rollback baseline",
  "release": "M10",
  "owner": "M10-W01-C01",
  "acceptance": "Actual target SDK selection and restored isolated source graph; fragile exact/numerical/replay tests; classify every active and historical project/consumer and every remaining migration risk; verify a clean rollback recipe."
}
""")!.AsObject(), "id", target);
    AddExact(doc, "requirements", JsonNode.Parse("""
{
  "id": "M10-W01-C02-R1",
  "title": "Production SDK and framework cutover",
  "release": "M10",
  "owner": "M10-W01-C02",
  "acceptance": "Actual solution selects SDK 11.0.100, all thirteen production libraries and eleven solution test projects resolve intended target, locked restore and Release build succeed, exact targeted tests pass; explicit historical fixture overrides and rollback verified."
}
""")!.AsObject(), "id", target);
    AddExact(doc, "requirements", JsonNode.Parse("""
{
  "id": "M10-W01-C03-R1",
  "title": "Post-cutover qualification and delivery readiness",
  "release": "M10",
  "owner": "M10-W01-C03",
  "acceptance": "Targeted fragile contracts then full solution regression at migration gate; fresh isolated unpublished net11 candidates for all thirteen libraries, exact source-free locked representative consumers, Windows and Linux/WSL evidence with explicit gaps and observed SDK/RID; verify rollback and no archive overwrite."
}
""")!.AsObject(), "id", target);
    AddExact(doc, "source_coverage", JsonNode.Parse("""
{
  "source_id": "USER-M10-PRODUCTION-NET11",
  "title": "Owner-approved production .NET 11 and C# 15 migration",
  "targets": [
    "M10-W01-C01",
    "M10-W01-C02",
    "M10-W01-C03"
  ],
  "disposition": "Owner approved actual production migration following RS366; publication and downstream adoption are excluded."
}
""")!.AsObject(), "source_id", target);
    doc["status"] = "in_progress";
    doc["plan_version"] = "0.1.143";
});
p.Json.EditObject(".aura/workflow/plan/project.json", doc =>
{
    var nodes = doc["milestones"]!.AsArray();
    var prior = nodes.Where(x => (string?)x?["id"] != "M10").ToArray();
    if (prior.Length != 10 || prior.Any(x => (string?)x?["state"] != "done")) throw new InvalidOperationException("Historical native milestones differ.");
    var entry = JsonNode.Parse("""
{
  "id": "M10",
  "title": "Production .NET 11 and C# 15 migration",
  "state": "not-ready",
  "workPackages": [
    {
      "id": "M10-W01",
      "title": "Controlled production framework cutover",
      "state": "not-ready",
      "phases": [
        {
          "id": "M10-W01-C01",
          "title": "Cutover readiness and rollback baseline",
          "state": "not-ready",
          "tasks": [
            {
              "id": "M10-W01-C01-T1",
              "title": "Qualify cutover readiness",
              "state": "not-ready",
              "subtasks": [
                {
                  "id": "M10-W01-C01-T1-R",
                  "title": "RED",
                  "state": "not-ready"
                },
                {
                  "id": "M10-W01-C01-T1-G",
                  "title": "GREEN",
                  "state": "not-ready"
                },
                {
                  "id": "M10-W01-C01-T1-V",
                  "title": "Verify and document",
                  "state": "not-ready"
                }
              ]
            }
          ],
          "dependsOn": [
            "M8-W02-C02",
            "M9-W01-C01"
          ]
        },
        {
          "id": "M10-W01-C02",
          "title": "Production SDK and framework cutover",
          "state": "not-ready",
          "tasks": [
            {
              "id": "M10-W01-C02-T1",
              "title": "Migrate actual production and maintained build graph",
              "state": "not-ready",
              "subtasks": [
                {
                  "id": "M10-W01-C02-T1-R",
                  "title": "RED",
                  "state": "not-ready"
                },
                {
                  "id": "M10-W01-C02-T1-G",
                  "title": "GREEN",
                  "state": "not-ready"
                },
                {
                  "id": "M10-W01-C02-T1-V",
                  "title": "Verify and document",
                  "state": "not-ready"
                }
              ]
            }
          ],
          "dependsOn": [
            "M10-W01-C01"
          ]
        },
        {
          "id": "M10-W01-C03",
          "title": "Post-cutover qualification and delivery readiness",
          "state": "not-ready",
          "tasks": [
            {
              "id": "M10-W01-C03-T1",
              "title": "Verify migrated graph and independent consumers",
              "state": "not-ready",
              "subtasks": [
                {
                  "id": "M10-W01-C03-T1-R",
                  "title": "RED",
                  "state": "not-ready"
                },
                {
                  "id": "M10-W01-C03-T1-G",
                  "title": "GREEN",
                  "state": "not-ready"
                },
                {
                  "id": "M10-W01-C03-T1-V",
                  "title": "Verify and document",
                  "state": "not-ready"
                }
              ]
            }
          ],
          "dependsOn": [
            "M10-W01-C02"
          ]
        }
      ]
    }
  ],
  "dependsOn": [
    "M8",
    "M9"
  ]
}
""")!.AsObject();
    var matches = nodes.Where(x => (string?)x?["id"] == "M10").ToArray();
    if (matches.Length == 0) nodes.Add(entry);
    else if (matches.Length != 1 || !JsonNode.DeepEquals(matches[0], entry)) throw new InvalidOperationException("Unexpected native M10 target.");
});
using (var process = new Process { StartInfo = new ProcessStartInfo
{
    FileName = "dotnet", WorkingDirectory = p.RepositoryRoot, UseShellExecute = false,
    RedirectStandardOutput = true, RedirectStandardError = true
}})
{
    foreach (var arg in new[] { "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write" }) process.StartInfo.ArgumentList.Add(arg);
    if (!process.Start()) throw new InvalidOperationException("Renderer unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(240000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Renderer timed out."); }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if (process.ExitCode != 0) throw new InvalidOperationException("Renderer failed: " + output);
}
return p.Complete();
