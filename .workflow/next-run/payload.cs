using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string PriorReport=".aura/workflow/reports/RS130/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS130-post-m7-readme-qualified.json";
const string Backlog="docs/planning/backlog.json";
const string DiscoveryReadme="docs/discovery/README.md";
const string Decision="docs/planning/decisions/post-m7-discovery-portable-conformance.md";
const string Validator="docs/discovery/qualification/ValidateDwfPortableProtocolGate.cs";
const string Evidence="docs/discovery/qualification/RS131-portable-protocol-qualified.json";
const string PortableVerifier="docs/discovery/qualification/ValidatePortableProtocol.cs";
const string Status="rs131-math-csharp-portable-discovery-corpus-qualified-without-agent-skill";
const string Counts="MATH-DISCOVERY-RS131:CASES=13;ACCEPT=2;REJECT=11;CANDIDATE-STATIC=4;NATIVE=541";
const string Scope="MATH-DISCOVERY-RS131:PORTABLE-CSHARP=true;CLR-BINDINGS=false;SKILL=false;BROKER=false;EXTERNAL=false";
const string Marker="dw.tools.math/rs131-portable-discovery/1.0 qualified; cases=13; accepted=2; rejected=11; generated=false";
const string SchemaSha="678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170";
const string FixturesSha="5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e";
const string ProtocolGitBlob="cd589183991774d1560129139f903dc8112c74b3";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS131",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS130",1,
    "docs: reconcile current Math README with qualified local delivery",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",
    JsonValue.Create(true));
p.Require.JsonValueEquals(PriorReceipt,"/status",
    JsonValue.Create("rs130-current-math-readme-and-immutable-historical-proof-reconciled-unpublished"));
p.Require.JsonValueEquals(PriorReceipt,"/approved_sequence",JsonValue.Create("A-B-D"));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/discovery_skill_generated",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/future_program_approved",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/external_requests_transmitted",JsonValue.Create(false));
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.Require.TextContains(DiscoveryReadme,"No generated Math skill");
p.ProjectPlan.RequireNodeState("M7","done");
p.ProjectPlan.RequireNodeState("M3","done");

if(!target)
{
    foreach(var destination in new[]{Decision,Validator,Evidence})
        p.Require.FileMissing(destination);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence,"/total_cases",JsonValue.Create(13));
    p.Require.JsonValueEquals(Evidence,"/accepted_cases",JsonValue.Create(2));
    p.Require.JsonValueEquals(Evidence,"/rejected_cases",JsonValue.Create(11));
    p.Require.JsonValueEquals(Evidence,"/candidate_static_operations",JsonValue.Create(4));
    p.Require.JsonValueEquals(Evidence,"/native_nodes_retained",JsonValue.Create(541));
    foreach(var (pointer,value) in new (string,bool)[]
    {
        ("/csharp_protocol_verifier_executed",true),
        ("/csharp_portable_envelope_conformant",true),
        ("/candidate_source_pins_match",true),
        ("/selected_clr_bindings_reflected",false),
        ("/compiled_binding_examples",false),
        ("/generated_math_skill",false),
        ("/broker_ready",false),
        ("/math_decision_runtime_interoperability_certified",false),
        ("/new_discovery_work_package_admitted",false),
        ("/m0_m7_plan_mutated",false),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false)
    })
        p.Require.JsonValueEquals(Evidence,pointer,JsonValue.Create(value));
}
var readmeOutput=p.Files.ReplaceFromStaged("staged/"+DiscoveryReadme,DiscoveryReadme);
var decisionOutput=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var validatorOutput=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var readmeSha=readmeOutput.AfterSha256 ??
    throw new InvalidOperationException("Missing staged Discovery status identity.");
var decisionSha=decisionOutput.AfterSha256 ??
    throw new InvalidOperationException("Missing staged Discovery decision identity.");
var validatorSha=validatorOutput.AfterSha256 ??
    throw new InvalidOperationException("Missing staged independent CSharp checker identity.");
foreach(var item in new[]{"RS131","No generated Math skill","RS131-portable-protocol-qualified.json","issue #139"})
    p.Require.TextContains(DiscoveryReadme,item);
foreach(var item in new[]{
    "## Authorized stage B and strict scope",
    "## Immutable verified inputs",
    "## Execution, validation and evidence",
    "## Native planning boundary for next B runs",
    "## Nonclaims and next action"})
    p.Require.TextContains(Decision,item,StringComparison.OrdinalIgnoreCase);
foreach(var item in new[]{Counts,Scope,Marker})
    p.Require.TextContains(Validator,item);
var baseline=Run(root,"discovery-source-baseline","dotnet",
    "run","--file",Validator,"--","--check");
foreach(var item in new[]{Counts,Scope,Marker})
    if(!baseline.Contains(item,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Independent RS131 baseline evidence missing: "+item);

if(!target)
{
    var verified=Run(root,"portable-csharp-13-fixtures","dotnet",
        "run","--file",PortableVerifier);
    foreach(var item in new[]{
        "DECISION_COMMIT 5d3af8580ea984425b314d589def18e031172c2c",
        "PROTOCOL dw.discovery/1.0.0-draft.1 manifest-schema=2",
        "SCHEMA_SHA256 "+SchemaSha,
        "FIXTURE_SHA256 "+FixturesSha,
        "RESULT PASS 13/13 (2 accepted, 11 rejected) independent Math C# envelope verifier"
    })
        if(!verified.Contains(item,StringComparison.Ordinal))
            throw new InvalidOperationException(
                "Actual Math C# portable protocol evidence missing: "+item);
    var expected=new (string Name,bool Accept)[]
    {
        ("valid-math-envelope",true),
        ("valid-decision-envelope",true),
        ("unknown-schema",false),
        ("unknown-protocol",false),
        ("documented-leak",false),
        ("wrong-owner",false),
        ("stale-signature",false),
        ("missing-limits",false),
        ("premature-broker-ready",false),
        ("metadata-authorization",false),
        ("bogus-version",false),
        ("duplicate-capability",false),
        ("catalogue-leak",false)
    };
    var actualLines=verified.Split('\n',
        StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith("CASE ",StringComparison.Ordinal))
        .ToArray();
    if(actualLines.Length!=expected.Length)
        throw new InvalidOperationException(
            "CSharp verifier must emit exactly thirteen scenario verdicts.");
    var cases=new List<JsonNode>();
    for(var i=0;i<expected.Length;i++)
    {
        var (name,accept)=expected[i];
        var verdict=accept?"accept":"reject";
        var expectedLine="CASE "+name+": expected="+verdict+
            " observed="+verdict+" PASS";
        if(!actualLines[i].Equals(expectedLine,StringComparison.Ordinal))
            throw new InvalidOperationException(
                "Actual portable C# scenario mismatch: "+name);
        cases.Add(new JsonObject
        {
            ["name"]=name,["expected_accept"]=accept,
            ["observed_accept"]=accept,["passed"]=true
        });
    }
    var sdk=Run(root,"observed-dotnet-sdk","dotnet","--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,["id"]="RS131-portable-protocol-qualified",
        ["status"]=Status,
        ["question"]="Can the existing independent Math C# verifier actually reproduce all thirteen Decision-pinned portable dw.discovery draft-1 envelope decisions under a controlled local DWF run?",
        ["observation"]="The original Math C# file-based verifier exited zero and emitted thirteen exact ordered PASS decisions, two accepted and eleven rejected, with pinned Decision commit, portable protocol/schema identity and checked SHA-256 source bytes. The four proposed static Math source files still match their candidate Git blob pins.",
        ["decision"]="Admit only bounded portable-envelope C# conformance evidence; postpone new CLR binding, compiled invocation examples, a deterministic Math skill/manifest, BrokerReady or AURA integration until a supported new-work-package DWF topology and independent product qualification exist.",
        ["discovery_readme"]=DiscoveryReadme,
        ["discovery_readme_sha256"]=readmeSha,
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["portable_verifier"]=PortableVerifier,
        ["portable_verifier_git_blob"]=ProtocolGitBlob,
        ["protocol_id"]="dw.discovery",["protocol_version"]="1.0.0-draft.1",
        ["manifest_schema_version"]=2,
        ["decision_source_commit"]="5d3af8580ea984425b314d589def18e031172c2c",
        ["schema_sha256"]=SchemaSha,["fixtures_sha256"]=FixturesSha,
        ["total_cases"]=13,["accepted_cases"]=2,["rejected_cases"]=11,
        ["case_results"]=p.Json.Array(cases.ToArray()),
        ["candidate_static_operations"]=4,["candidate_source_pins_match"]=true,
        ["native_nodes_retained"]=541,["backlog_plan_version"]="0.1.122",
        ["prior_rs130_first_pass"]=true,
        ["csharp_protocol_verifier_executed"]=true,
        ["csharp_portable_envelope_conformant"]=true,
        ["selected_clr_bindings_reflected"]=false,
        ["compiled_binding_examples"]=false,
        ["generated_math_skill"]=false,
        ["broker_ready"]=false,
        ["math_decision_runtime_interoperability_certified"]=false,
        ["new_discovery_work_package_admitted"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["dotnet_sdk"]=sdk.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_scope"]="Require an independently supported DWF topology admission for the proposed new Math M3-W03 work package before implementation of a selected CLR binder, compiled examples and generated Math skill. Workflow issue #139 remains a feedback request; do not hand-edit native plan or claim BrokerReady.",
        ["nonclaims"]=p.Json.StringArray(
            "The original historical JavaScript portable preflight is not relabelled as Math C# evidence.",
            "Math C# conformance of thirteen synthetic envelope fixtures is not Math/Decision runtime interoperability.",
            "Four pinned source-level static candidates are not reflected CLR bindings, compiled examples or a generated public skill.",
            "No AURA broker adapter/authorization, client migration, NuGet publication or new M3-W03 native work package is delivered.",
            "The original M0-M7 accepted 541-node plan remains done without progress transitions or achievements.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
var final=Run(root,"discovery-source-target","dotnet",
    "run","--file",Validator,"--","--check");
if(!final.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("RS131 independent protocol receipt refused.");
return p.Complete();

static string Run(string root,string id,string executable,params string[] args)
{
    using var process=new Process
    {
        StartInfo=new ProcessStartInfo
        {
            FileName=executable,WorkingDirectory=root,
            UseShellExecute=false,RedirectStandardOutput=true,
            RedirectStandardError=true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())
        throw new InvalidOperationException("RS131 command failed to start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(240000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS131 bounded command timed out: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS131 local command failed: "+id+
            " exit="+process.ExitCode+"\n"+
            output[..Math.Min(2600,output.Length)]);
    Console.WriteLine("RS131 "+id+" exit=0");
    return output;
}
