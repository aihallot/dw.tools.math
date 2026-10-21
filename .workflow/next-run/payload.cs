using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Backlog="docs/planning/backlog.json";
const string PriorReport=".aura/workflow/reports/RS131/attempt-001.json";
const string PriorReceipt="docs/discovery/qualification/RS131-portable-protocol-qualified.json";
const string Candidate="docs/discovery/agent-surface-candidates.json";
const string Proposal="docs/planning/proposals/math-agent-discovery-work-package.json";
const string Decision="docs/discovery/qualification/RS132-clr-seed-boundary.md";
const string Matrix="docs/discovery/qualification/RS132-clr-seed-matrix.md";
const string Validator="docs/discovery/qualification/ValidateClrSeedReflection.cs";
const string Evidence="docs/discovery/qualification/RS132-clr-seed-qualified.json";
const string Status="rs132-four-candidate-static-clr-signatures-reflected-no-agent-skill";
const string Counts="MATH-RS132:STATIC-CANDIDATES=4;COMPILED-SIGNATURES=4;RS131-CASES=13;NATIVE=541";
const string Scope="MATH-RS132:REFLECTION=SOURCE-COMPILED;EXAMPLES=false;SKILL=false;BROKER=false;EXTERNAL=false";
const string Marker="dw.tools.math/rs132-clr-seed/1.0 qualified; signatures=4; examples=false; broker=false";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS132",null,null,null,null,null);
p.Workflow.RequirePriorSuccess("RS131",1,
    "docs: qualify CSharp Discovery portable protocol evidence without skill",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(PriorReceipt,"/status",JsonValue.Create(
    "rs131-math-csharp-portable-discovery-corpus-qualified-without-agent-skill"));
p.Require.JsonValueEquals(PriorReceipt,"/total_cases",JsonValue.Create(13));
p.Require.JsonValueEquals(PriorReceipt,"/accepted_cases",JsonValue.Create(2));
p.Require.JsonValueEquals(PriorReceipt,"/rejected_cases",JsonValue.Create(11));
p.Require.JsonValueEquals(PriorReceipt,"/candidate_static_operations",JsonValue.Create(4));
p.Require.JsonValueEquals(PriorReceipt,"/native_nodes_retained",JsonValue.Create(541));
p.Require.JsonValueEquals(PriorReceipt,"/selected_clr_bindings_reflected",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/compiled_binding_examples",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/generated_math_skill",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/broker_ready",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReceipt,"/publicly_published",JsonValue.Create(false));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/count",JsonValue.Create(0));
p.Require.JsonValueEquals(PriorReport,"/priorInvocationFailures/firstPassSucceeded",
    JsonValue.Create(true));
p.Require.JsonValueEquals(Candidate,"/status",JsonValue.Create(
    "proposed-not-implemented-not-admitted"));
p.Require.JsonValueEquals(Proposal,"/status",JsonValue.Create(
    "draft-not-merged-into-canonical-backlog-or-native-DWF"));
p.Require.TextContains("docs/discovery/README.md","No generated Math skill");
p.Require.TextContains("docs/coordination/requests/README.md","have not been sent");
p.ProjectPlan.RequireNodeState("M7","done");
if(!target)
{
    foreach(var file in new[]{Decision,Matrix,Validator,Evidence})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence,"/observed_static_bindings",JsonValue.Create(4));
    p.Require.JsonValueEquals(Evidence,"/prior_csharp_protocol_cases",JsonValue.Create(13));
    p.Require.JsonValueEquals(Evidence,"/native_nodes_retained",JsonValue.Create(541));
    foreach(var (ptr,flag) in new (string,bool)[]
    {
        ("/compiled_assembly_reflection_executed",true),
        ("/all_selected_signatures_exact",true),
        ("/compiled_invocation_examples",false),
        ("/generated_math_skill",false),
        ("/broker_ready",false),
        ("/json_broker_adapter_qualified",false),
        ("/math_decision_runtime_interoperability_certified",false),
        ("/new_discovery_work_package_admitted",false),
        ("/m0_m7_plan_mutated",false),
        ("/publicly_published",false),
        ("/external_requests_transmitted",false),
        ("/external_approvals_received",false)
    })
        p.Require.JsonValueEquals(Evidence,ptr,JsonValue.Create(flag));
}
var decisionChange=p.Files.ReplaceFromStaged("staged/"+Decision,Decision);
var matrixChange=p.Files.ReplaceFromStaged("staged/"+Matrix,Matrix);
var validatorChange=p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
var decisionSha=decisionChange.AfterSha256??
    throw new InvalidOperationException("Missing RS132 decision source SHA");
var matrixSha=matrixChange.AfterSha256??
    throw new InvalidOperationException("Missing RS132 matrix source SHA");
var validatorSha=validatorChange.AfterSha256??
    throw new InvalidOperationException("Missing RS132 verifier source SHA");
foreach(var word in new[]{
    "## Objective and authorization",
    "## Exact evidence boundary and candidate identities",
    "## Controlled verification","## Next gate"})
    p.Require.TextContains(Decision,word,StringComparison.OrdinalIgnoreCase);
foreach(var word in new[]{
    "## Four source-pinned selected members",
    "## Exclusions and science limits",
    "## Supported DWF continuation"})
    p.Require.TextContains(Matrix,word,StringComparison.OrdinalIgnoreCase);
foreach(var mark in new[]{Counts,Scope,Marker})
    p.Require.TextContains(Validator,mark);
var baseline=Execute(root,"seed-baseline","dotnet",
    "run","--file",Validator,"--","--check");
foreach(var mark in new[]{Counts,Scope,Marker})
    if(!baseline.Output.Contains(mark,StringComparison.Ordinal))
        throw new InvalidOperationException("RS132 baseline checker marker missing: "+mark);
if(!target)
{
    var steps=new List<JsonNode>();
    steps.Add(Execute(root,"locked-solution-restore","dotnet",
        "restore","dw.tools.math.slnx","--locked-mode").Receipt);
    steps.Add(Execute(root,"composition-release-build","dotnet",
        "build","src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj",
        "-c","Release","--no-restore").Receipt);
    var reflection=Execute(root,"four-static-clr-reflection","dotnet",
        "run","--file",Validator,"--","--reflect");
    steps.Add(reflection.Receipt);
    var observed=reflection.Output.Split('\n',
        StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries)
        .Where(x=>x.StartsWith("RS132-CLR|",StringComparison.Ordinal))
        .ToArray();
    var selected=new (string Id,string Assembly,string Type,string Method,
        string[] Parameters,string Result,string Blob)[]
    {
        ("math.quantity.exact-pipeline.evaluate","dw.tools.math.composition",
         "Dw.Tools.Math.Composition.ExactQuantityPipeline","Evaluate",
         ["System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>"],
         "Dw.Tools.Math.Composition.ExactPipelineResult",
         "1d689d8dc66691de0c9a3b7f6e7c3751be68ec7d"),
        ("math.replay.try-evaluate","dw.tools.math.composition",
         "Dw.Tools.Math.Composition.ExactReplayRunner","TryRun",
         ["System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>",
          "Dw.Tools.Math.Composition.ExactReplayContext",
          "Dw.Tools.Math.Composition.ExactReplayCache",
          "System.Threading.CancellationToken"],
         "Dw.Tools.Math.Composition.ExactReplayAttempt",
         "ffffe063ffdb698776d1bed82d683e4bd635dd40"),
        ("math.ir.canonical.encode","dw.tools.math.ir",
         "Dw.Tools.Math.Ir.IrCanonicalJsonCodec","Encode",
         ["Dw.Tools.Math.Ir.IrNode"],"System.String",
         "4e6e055d94480fb8ac449750b2e4f23eeeec09e9"),
        ("math.ir.hash.semantic","dw.tools.math.ir",
         "Dw.Tools.Math.Ir.IrCanonicalHashes","SemanticStructuralSha256",
         ["Dw.Tools.Math.Ir.IrNode"],"System.String",
         "8bbab3059acfad2ee6863a6fd98703798bac688d")
    };
    if(observed.Length!=selected.Length)
        throw new InvalidOperationException("Exactly four independently reflected CLR receipts required");
    var rows=new List<JsonNode>();
    for(var i=0;i<selected.Length;i++)
    {
        var method=selected[i];
        var columns=observed[i].Split('|');
        if(columns.Length!=4||columns[0]!="RS132-CLR"||
           columns[1]!=method.Id||columns[3]!=method.Assembly||
           !Hex64(columns[2]))
            throw new InvalidOperationException("Reflected public static signature missing: "+method.Id);
        var canonical=method.Id+"|"+method.Assembly+"|"+
            method.Type+"::"+method.Method+"("+
            string.Join(",",method.Parameters)+")=>"+
            method.Result+"|public-static";
        rows.Add(new JsonObject
        {
            ["capability_id"]=method.Id,
            ["assembly"]=method.Assembly,
            ["canonical_signature"]=canonical,
            ["signature_sha256"]=columns[2],
            ["source_git_blob"]=method.Blob,
            ["broker_ready"]=false
        });
    }
    if(steps.Count!=3||rows.Count!=4)
        throw new InvalidOperationException("RS132 requires 3 local commands and 4 reflected signatures");
    var sdk=Execute(root,"actual-sdk","dotnet","--version");
    var receipt=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="RS132-clr-seed-qualified",
        ["status"]=Status,
        ["question"]="Do four deliberately selected candidate Math static operations bind to exactly one public CLR signature each in current compiled local assemblies?",
        ["observation"]="The local Math composition/IR dependency closure built in Release. The independent BCL reflection verifier matched each exact public static method, parameter and return type, and returned four canonical signature SHA-256 identifiers.",
        ["decision"]="Admit only observed source-pinned CLR signature availability for four candidate static operations. Compiled invocation examples, deterministic skill/manifest generation, broker adapters, BrokerReady, DWF work-package admission and actual AURA interoperability are not delivered.",
        ["decision_document"]=Decision,["decision_sha256"]=decisionSha,
        ["matrix_document"]=Matrix,["matrix_sha256"]=matrixSha,
        ["validator"]=Validator,["validator_sha256"]=validatorSha,
        ["previous_portable_evidence"]=PriorReceipt,
        ["selected_seed"]=4,
        ["observed_static_bindings"]=4,
        ["prior_csharp_protocol_cases"]=13,
        ["native_nodes_retained"]=541,
        ["backlog_plan_version"]="0.1.122",
        ["compiled_assembly_reflection_executed"]=true,
        ["all_selected_signatures_exact"]=true,
        ["compiled_invocation_examples"]=false,
        ["generated_math_skill"]=false,
        ["broker_ready"]=false,
        ["json_broker_adapter_qualified"]=false,
        ["math_decision_runtime_interoperability_certified"]=false,
        ["new_discovery_work_package_admitted"]=false,
        ["m0_m7_plan_mutated"]=false,
        ["publicly_published"]=false,
        ["external_requests_transmitted"]=false,
        ["external_approvals_received"]=false,
        ["dotnet_sdk"]=sdk.Output.Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["command_receipts"]=p.Json.Array(steps.ToArray()),
        ["observed_bindings"]=p.Json.Array(rows.ToArray()),
        ["next_scope"]="The next Discovery implementation requires approved DWF work-package topology and separate compiled invocation examples, generator/manifest and skill acceptance. Coordinate AURA/Decision/MCDM only after B, with specific transmission authorization.",
        ["nonclaims"]=p.Json.StringArray(
            "Reflecting four exact methods in current compiled assemblies is not a compiled C# invocation example or a runnable Math agent tool.",
            "The prior 13/13 C# dw.discovery portable envelope fixtures are not a qualified Math/Decision CLR runtime integration.",
            "No automatically published helper, instance-matrix method, selected BrokerReady operation, generated agent skill, manifest or AURA adapter was admitted.",
            "The existing M0-M7 541-node plan remains done; no new M3-W03 work package, public package release or external transmission occurred.")
    };
    p.Files.WriteComplete(Evidence,
        receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.ProjectPlan.RequireNodeState("M7","done");
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals(Backlog,"/status",JsonValue.Create("done"));
var checkedTarget=Execute(root,"seed-target","dotnet",
    "run","--file",Validator,"--","--check");
if(!checkedTarget.Output.Contains(Marker,StringComparison.Ordinal))
    throw new InvalidOperationException("Independent CLR seed target gate rejected");
return p.Complete();

static bool Hex64(string value)=>value.Length==64&&value.All(x=>
    (x>='0'&&x<='9')||(x>='a'&&x<='f'));
static (JsonObject Receipt,string Output) Execute(string root,string id,
    string executable,params string[] args)
{
    using var process=new Process
    {
        StartInfo=new ProcessStartInfo
        {
            FileName=executable,WorkingDirectory=root,UseShellExecute=false,
            RedirectStandardOutput=true,RedirectStandardError=true
        }
    };
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    var clock=Stopwatch.StartNew();
    if(!process.Start())
        throw new InvalidOperationException("RS132 command failed to start: "+id);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException("RS132 command timed out: "+id);
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+
        stderr.GetAwaiter().GetResult();
    clock.Stop();
    if(process.ExitCode!=0)
        throw new InvalidOperationException("RS132 "+id+" failed exit="+
            process.ExitCode+"\n"+output[..Math.Min(output.Length,2500)]);
    Console.WriteLine("RS132 "+id+" exit=0 duration_ms="+clock.ElapsedMilliseconds);
    return (new JsonObject
    {
        ["id"]=id,
        ["exit_code"]=process.ExitCode,
        ["command"]=executable+" "+string.Join(" ",args),
        ["elapsed_ms"]=clock.ElapsedMilliseconds,
        ["output_excerpt"]=output[..Math.Min(output.Length,1100)]
    },output);
}
