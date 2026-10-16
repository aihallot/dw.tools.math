using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M5",Work="M5-W01",Phase="M5-W01-C04",Task="M5-W01-C04-T2";
const string Red="M5-W01-C04-T2-R",Green="M5-W01-C04-T2-G",Verify="M5-W01-C04-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string C01="docs/planning/evidence/M5-W01-C01-T2-boundaries-qualified.json";
const string C02="docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json";
const string C03="docs/planning/evidence/M5-W01-C03-T2-boundaries-qualified.json";
const string Prior="docs/planning/evidence/M5-W01-C04-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M5-W01-C04-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json";
const string Aggregate="docs/planning/evidence/M5-W01-work-package-qualified.json";
const string QualifiedStatus="exact-office-omml-markdown-unicode-loss-and-format-boundaries-qualified";
const string AggregateStatus="m5-w01-exact-math-codecs-interchange-boundaries-qualified";
const string Source="src/projects/dw.tools.math.codecs/MathDisplayBoundaryAudit.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C04T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C04T2Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c04-t2-display-boundaries.md";
const string Document="docs/distribution/m5-markdown-omml-boundaries.md";
const string ConsumerProject="tests/consumers/m5-w01-c04-t2/m5-math-display-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c04-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c04-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS073/staged/checks/m5-w01-c04-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.8";
const string RedMarker="M5-W01-C04-T2 RED: independent OMML and Markdown display boundary audit API is absent.";
const string ConsumerMarker="dw.tools.math/m5-markdown-omml-boundary-consumer/1.0 qualified; probes=8";
const int IndependentGreen=38,AllTargeted=39;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS073",Milestone,target?null:Work,
    target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS072",1,
    "feat: qualify exact Markdown and original-IR Office OMML interchange",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(C01,"/status",
    JsonValue.Create("exact-safe-canonical-display-original-restrictions-and-ambiguity-qualified"));
p.Require.JsonValueEquals(C02,"/status",
    JsonValue.Create("bounded-latex-import-semantic-loss-and-adversarial-boundaries-qualified"));
p.Require.JsonValueEquals(C03,"/status",
    JsonValue.Create("exact-annotated-mathml-xml-security-and-loss-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-markdown-unicode-and-original-ir-bound-office-omml-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(24));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(25));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.0.0-preview.7"));
p.Require.JsonValueEquals(Prior,"/omml_import_requires_original_typed_ir",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_c04_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_c04_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.71"),JsonValue.Create("0.1.72"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.72":"0.1.71"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
foreach(var index in new[]{33,34,35})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/36/tasks/1/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?
    "<Version>1.0.0-preview.8</Version>":
    "<Version>1.0.0-preview.7</Version>");

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
foreach(var node in new[]{"M5-W01-C01","M5-W01-C02","M5-W01-C03"})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C04-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");

if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified,Aggregate})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-math-display-boundary-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/red_recovered_and_green_tests_passed",
        JsonValue.Create(AllTargeted));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c04_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c04_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Aggregate,"/status",JsonValue.Create(AggregateStatus));
    p.Require.JsonValueEquals(Aggregate,"/m5_w01_work_package_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Aggregate,"/m5_release_done",JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redReceipt=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redReceipt.AfterSha256 ??
    throw new InvalidOperationException("No genuine compiled Office/Markdown boundary RED source hash.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C04T2RedTests.IndependentOmmlAndMarkdownBoundaryAuditPublicApiMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A genuinely compiled absent-public-boundary-API RED is required: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C04-T2-contract-red",
        ["status"]="compiled-math-display-boundary-api-absent-red-observed",
        ["prior_exact_display_interop"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.codecs/1.0.0-preview.7",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="MathDisplayBoundaryAudit.AuditOmml(string, IrNode) and AuditMarkdown(string, MarkdownMathPlacement, IrNode)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="A missing public audit API does not prove Office document, typography or inverse fidelity."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Product,Source,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing exact math display source byte hash: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W01C04T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 39",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 39",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one recovered compiled RED and all 38 independent Office/Markdown boundary GREEN oracles.");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "Qualified canonical Math IR or quantities regression failed: "+
            regressionProject);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.codecs",Version,Product,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w01-c04-t2-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException(
            "Required freshly packed local NuGet product archive absent: "+package.Id);
    archives.Add(archive);
}
var inspectorArguments=new List<string>{"run","--file",PackageHelper,"--"};
inspectorArguments.AddRange(archives);
var packageInspectionOutput=Required(root,"dotnet",inspectorArguments.ToArray());
var products=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(packageInspectionOutput,package.Id,package.Version);
    products.Add(new JsonObject
    {
        ["id"]=package.Id,["version"]=package.Version,
        ["bytes"]=receipt.Bytes,["sha256"]=receipt.Sha,
        ["dependencies"]=p.Json.StringArray(package.DependsOn),
        ["assembly"]="lib/net10.0/"+package.Id+".dll",
        ["publication"]="local-only-no-public-feed"
    });
}
var consumerXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var references=consumerXml.Descendants()
    .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
   (string?)references[0].Attribute("Include")!="dw.tools.math.codecs" ||
   (string?)references[0].Attribute("Version")!="["+Version+"]" ||
   consumerXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "Independent Math display boundary consumer must pin one NuGet codecs prerelease.");
var configured=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!configured.Elements().Any(x=>x.Name.LocalName=="clear") ||
   configured.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException(
        "Independent NuGet consumer must clear non-local package feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c04-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent pinned NuGet-only exact Office/Markdown loss probes not observed.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C04-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_exact_omml_markdown_contract"]=Prior,
        ["compiled_missing_boundary_api_red"]=RedProof,
        ["question"]="Can standalone exact Office OMML and Unicode Markdown display preserve caller-supplied original IR while explicitly refusing extra document/layout markup, noncanonical XML, presentation ambiguity and unsupported conversions?",
        ["observation"]="One compiled missing-public-API RED recovered, 38 independent GREEN boundary oracles and eight NuGet-only consumers qualify exact source-bound OMML/Markdown structural fidelity, explicit loss and no Office runtime.",
        ["decision"]="Delegate XML to the previously qualified secure Office codec; admit only independently replayed caller-supplied typed IR with exact canonical OMML or Markdown placement. Refuse document/layout, typography inference, extra glyphs and resource limits.",
        ["source_file"]=Source,
        ["source_sha256"]=hashes[Source],
        ["product_project"]=Product,
        ["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,
        ["green_tests_sha256"]=hashes[GreenTests],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["display_boundary_rule"]="dw-math-omml-markdown-display-boundary/1",
        ["bounded_original_input_utf8_sha256"]=true,
        ["source_sha256_is_not_semantic_proof"]=true,
        ["original_ir_canonical_json_replay_qualified"]=true,
        ["exact_fraction_equation_and_ordered_sum_verified"]=true,
        ["markdown_inline_block_placement_refusals_qualified"]=true,
        ["omml_dtd_xml_document_style_positioning_refusals_qualified"]=true,
        ["omml_inverse_without_original_ir_qualified"]=false,
        ["unicode_markdown_typography_inverse_qualified"]=false,
        ["office_application_or_sdk_required"]=false,
        ["whole_office_document_conversion_qualified"]=false,
        ["font_fidelity_qualified"]=false,
        ["positioning_fidelity_qualified"]=false,
        ["refused_input_has_no_ir_semantic_hash_or_rule"]=true,
        ["locked_solution_build_passed"]=true,
        ["focused_codecs_ir_quantities_regressions_passed"]=true,
        ["local_product_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["product_package_manifest_sha_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=hashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=hashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=hashes[ConsumerConfig],
        ["consumer_marker"]=ConsumerMarker,
        ["packages_publicly_published"]=false,
        ["m5_w01_c04_t2_done"]=true,
        ["m5_w01_c04_phase_done"]=true,
        ["m5_w01_work_package_done"]=true,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W02-C01-T1: independent .NET console vertical joining qualified numeric, symbolic, expression and display IR components; keep M5 gate unpromoted",
        ["nonclaims"]=p.Json.StringArray(
            "No general Office Math interpreter, Word document conversion, font or layout fidelity, arbitrary Markdown or Unicode math parser.",
            "No typography-derived semantic identity, source-free Office import, unbounded XML or executable entities.",
            "No public stable NuGet, AURA/Decision/MCDM or dw.discovery adoption.",
            "M5-W01 alone closes; M5, M5-W02 and G-M5 remain open.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    var aggregate=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01",
        ["status"]=AggregateStatus,
        ["qualified_chains"]=p.Json.StringArray(C01,C02,C03,Prior,Qualified),
        ["exact_canonical_math_text_and_latex_qualified"]=true,
        ["exact_annotated_content_presentation_mathml_qualified"]=true,
        ["exact_omml_and_markdown_ir_bound_display_qualified"]=true,
        ["unicode_omml_typography_inverse_qualified"]=false,
        ["no_office_installation_required"]=true,
        ["no_whole_document_or_font_positioning_fidelity_qualified"]=true,
        ["compiled_red_recovered_for_c04_t2"]=true,
        ["independent_green_tests_for_c04_t2"]=IndependentGreen,
        ["nuget_only_consumer_probes_for_c04_t2"]=8,
        ["last_codecs_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["m5_w01_c01_done"]=true,
        ["m5_w01_c02_done"]=true,
        ["m5_w01_c03_done"]=true,
        ["m5_w01_c04_done"]=true,
        ["m5_w01_work_package_done"]=true,
        ["m5_release_done"]=false,
        ["scope"]="Qualified bounded exact text, LaTeX, MathML, Unicode Markdown and original-IR-bound standalone OMML; no arbitrary document conversion.",
        ["next_gate"]="M5-W02 numeric/symbolic public vertical and stable 1.0 qualification remain separate."
    };
    p.Files.WriteComplete(Aggregate,
        aggregate.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.72";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        var wp=plan["work_packages"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned" ||
           (string?)wp["status"]!="in_progress")
            throw new InvalidOperationException(
                "Accepted M5-W01 C04 T2 and work-package baseline diverged.");
        chunk["status"]="done";
        chunk["refinement"]="RS072 T1 qualified exact Markdown/Unicode inline/block display and standalone canonical Office OMML fraction/equality import bound to retained Math IR. RS073 T2 independently audits typed XML/document/style/layout and Unicode/Markdown loss boundaries, verifies source-byte SHA-256 separately from original Math IR canonical JSON/structural hash, refuses all unqualified formats and preserves no Office installation. One compiled missing-API RED, 38 independent GREEN tests, eight isolated NuGet consumer oracles and three SHA-verified local packages complete C04 and the four C01-C04 phases of M5-W01. M5 and M5-W02 remain unfinished.";
        chunk["readiness"]="C04 complete: original-IR-bound OMML equation/fraction, Unicode/Markdown placements and independent unsupported document/font/positioning/ambiguity refusals; M5-W01 complete, M5-W02 not started";
        var priorFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified,Aggregate
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C04T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var priorEvidence=chunk["evidence"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(priorEvidence.Concat(new[]{RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
        wp["status"]="done";
        wp["evidence"]=p.Json.StringArray(C01,C02,C03,Prior,Qualified,Aggregate);
    });
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var child in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"done");
foreach(var node in new[]{"M5-W01-C01","M5-W01-C02","M5-W01-C03"})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C04-T1","done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS073" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=(target?null:Work) ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS073 exact M5-W01 completion cursor diverged from baseline/target.");
    state["activeWorkPackage"]=null;
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasExactLine(string output,string expected) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);

static (long Bytes,string Sha) ReadProductReceipt(
    string output,string id,string version)
{
    var prefix="M5-W01-C04-T2-PKG-SHA256:"+id+":"+version+":";
    var found=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(found.Length!=1)
        throw new InvalidOperationException("One local independent Math package receipt required for "+id);
    var parts=found[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0 || parts[1].Length!=64 ||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid local NuGet bytes/SHA-256 receipt for "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
    return part.Length>2500?part[..2500]:part;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out during a bounded Math subprocess command.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
