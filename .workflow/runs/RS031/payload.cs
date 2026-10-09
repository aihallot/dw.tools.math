using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Phase="M2-W01-C03";
const string Work="M2-W01";
const string Task="M2-W01-C03-T2";
const string Red="M2-W01-C03-T2-R";
const string Green="M2-W01-C03-T2-G";
const string Verify="M2-W01-C03-T2-V";
const string Codec="src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs";
const string Guard="src/projects/dw.tools.math.ir/IrCanonicalJsonGuard.cs";
const string Hashes="src/projects/dw.tools.math.ir/IrCanonicalHashes.cs";
const string RedTests="tests/projects/dw.tools.math.ir.tests/M2W01C03BoundaryRedTests.cs";
const string Tests="tests/projects/dw.tools.math.ir.tests/M2W01C03BoundaryTests.cs";
const string Doc="docs/distribution/ir-canonical-json.md";
const string TestProject="tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj";
const string RedEvidence="docs/planning/evidence/M2-W01-C03-boundary-red.json";
const string QualifiedEvidence="docs/planning/evidence/M2-W01-C03-boundary-qualified.json";
const string Prior="docs/planning/evidence/M2-W01-C03-functional-qualified.json";
const string PackageVersion="0.3.0-preview.1";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var backlog=JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk=backlog["chunks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Phase);
var work=backlog["work_packages"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Work);
var task=chunk["tasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==Task);
string Sub(string id)=>(string?)task["subtasks"]!.AsArray().Select(x=>x!.AsObject())
    .Single(x=>(string?)x["id"]==id)["status"]
    ?? throw new InvalidOperationException("Missing JSON codec boundary subtask "+id);
var baseline=(string?)backlog["plan_version"]=="0.1.29" &&
    (string?)work["status"]=="in_progress" && (string?)chunk["status"]=="in_progress" &&
    (string?)task["status"]=="ready" &&
    Sub(Red)=="ready" && Sub(Green)=="planned" && Sub(Verify)=="planned";
var target=(string?)backlog["plan_version"]=="0.1.30" &&
    (string?)work["status"]=="done" && (string?)chunk["status"]=="done" &&
    (string?)task["status"]=="done" &&
    new[]{Red,Green,Verify}.All(x=>Sub(x)=="done");
if(!baseline && !target)
    throw new InvalidOperationException("RS031 is not in the qualified codec boundary baseline or exact target state.");
var previous=JsonNode.Parse(File.ReadAllText(Path.Combine(root,Prior)))!.AsObject();
if((string?)previous["status"]!="versioned-exact-ir-json-codec-functional-qualified-boundary-hardening-pending" ||
   previous["independent_round_trip_oracles"]?.GetValue<int>()!=10)
    throw new InvalidOperationException("RS030 canonical codec GREEN evidence is not qualified.");
if(baseline &&
   Hash(File.ReadAllBytes(Path.Combine(root,Codec)))!=(string?)previous["source_sha256"])
    throw new InvalidOperationException("RS030 qualified canonical codec source has drifted before independent RED.");
var aura=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/coordination/requests/MATH-XR-002-aura-adoption.json")))!.AsObject();
if((string?)aura["status"]!="draft" || (string?)aura["transmission"]!="none")
    throw new InvalidOperationException("Math must not claim external AURA package adoption.");

p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    foreach(var (name,marker) in new[]{
        ("UnknownNodeFieldMustBeRefusedBeforeMaterialization",
            "M2-W01-C03-T2 RED: unknown JSON node fields were accepted."),
        ("UnreferencedRestrictedSymbolMustNotBeAdmitted",
            "M2-W01-C03-T2 RED: restriction references no symbol in its expression."),
        ("DistinctVersionedHashContractsMustExist",
            "M2-W01-C03-T2 RED: distinct versioned structural and presentation hashes are absent.")
    })
    {
        var result=Run(root,"dotnet","test",TestProject,
            "-c","Release","--filter","FullyQualifiedName~M2W01C03BoundaryRedTests."+name,
            "--logger","console;verbosity=normal");
        if(result.Code==0 || !result.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Independent IR JSON boundary RED absent: "+
                name+" | "+Relevant(result.Output));
    }
}
var redProof=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C03-boundary-red",
    ["status"]="three-independent-json-boundary-reds-observed",
    ["previous_codec_evidence"]=Prior,
    ["red_methods"]=p.Json.StringArray(
        "M2W01C03BoundaryRedTests.UnknownNodeFieldMustBeRefusedBeforeMaterialization",
        "M2W01C03BoundaryRedTests.UnreferencedRestrictedSymbolMustNotBeAdmitted",
        "M2W01C03BoundaryRedTests.DistinctVersionedHashContractsMustExist"),
    ["before_change"]=p.Json.StringArray(
        "Unknown JSON properties were accepted by the admitted decoder.",
        "A decoded restriction could refer to a symbol absent from the guarded expression.",
        "No public, domain-separated semantic structural and presentation digests existed."),
    ["limit"]="RED failure proves missing defenses only; it is not evidence of consumer hardening by itself."
};
p.Files.WriteComplete(RedEvidence,redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var path in new[]{Codec,Guard,Hashes,Tests,Doc})
    p.Files.ReplaceFromStaged("staged/"+path,path);

Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
foreach(var name in new[]{
    "UnknownNodeFieldMustBeRefusedBeforeMaterialization",
    "UnreferencedRestrictedSymbolMustNotBeAdmitted",
    "DistinctVersionedHashContractsMustExist"})
    Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C03BoundaryRedTests."+name,
        "--logger","console;verbosity=normal");
foreach(var name in new[]{
    "UnrecognizedNodePropertyMustFailInsteadOfBeingIgnored",
    "DuplicateKeysMustFailBothInEnvelopeAndNode",
    "DuplicateOrUnknownAssumptionKeysMustFail",
    "UnreferencedAssumptionSymbolMustBeRejected",
    "ReferencedSymbolAssumptionsSurviveTheirIdentityAndDomain",
    "SemanticStructuralHashExcludesPresentationOnlyLabels",
    "SymbolDomainAndExactValuesAffectStructuralHash",
    "HashesAreStableAcrossQualifiedCanonicalRoundTrip",
    "OversizedExactNumeralAndJsonPayloadMustFailEarly",
    "UnknownKindVersionAndNonfiniteBitsMustFail",
    "MalformedJsonAndInvalidBoundSymbolAreNotAccepted"})
    Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M2W01C03BoundaryTests."+name,
        "--logger","console;verbosity=normal");
Required(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
Required(root,"dotnet","test","tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore","--logger","console;verbosity=minimal");
Required(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var feed=Path.Combine(root,"build","artifacts","ir-package-feed");
Required(root,"dotnet","pack","src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",
    "-c","Release","--no-build","--no-restore","-o",feed);
var artifact=Path.Combine(feed,"dw.tools.math.ir."+PackageVersion+".nupkg");
if(!File.Exists(artifact))
    throw new InvalidOperationException("Qualified IR JSON package archive absent.");
using(var zip=ZipFile.OpenRead(artifact))
{
    if(zip.GetEntry("lib/net10.0/dw.tools.math.ir.dll") is null)
        throw new InvalidOperationException("Qualified JSON IR package lacks net10.0 assembly.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Expected one IR NuGet manifest.");
    using var stream=specs[0].Open();
    var xml=XDocument.Load(stream);
    var metadata=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    var id=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
    var version=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
    var deps=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency")
        .Select(x=>(string?)x.Attribute("id")??"").ToArray();
    if(id!="dw.tools.math.ir" || version!=PackageVersion ||
       !deps.SequenceEqual(new[]{"dw.quantities"},StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("IR package identity or dependency graph escaped its contract.");
}
var evidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M2-W01-C03-boundary-qualified",
    ["status"]="bounded-closed-ir-canonical-json-and-structural-hashes-qualified",
    ["previous_functional_evidence"]=Prior,
    ["independent_boundary_red"]=RedEvidence,
    ["schema_version_id"]="math-ir/1",
    ["codec_source"]=Codec,
    ["codec_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Codec))),
    ["json_guard_source"]=Guard,
    ["json_guard_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Guard))),
    ["hashes_source"]=Hashes,
    ["hashes_sha256"]=Hash(File.ReadAllBytes(Path.Combine(root,Hashes))),
    ["nuget_package_id"]="dw.tools.math.ir",
    ["nuget_package_version"]=PackageVersion,
    ["nuget_package_sha256"]=Hash(File.ReadAllBytes(artifact)),
    ["nuget_dependencies"]=p.Json.StringArray("dw.quantities"),
    ["boundary_reds_green_after_fix"]=3,
    ["independent_adversarial_tests_passed"]=11,
    ["full_ir_and_quantities_tests_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["locked_solution_build_passed"]=true,
    ["transfer_architecture_passed"]=true,
    ["admission_contracts"]=p.Json.StringArray(
        "JSON object shape is closed for envelope, every admitted node and relations; unknown and duplicate properties are refused before materialization.",
        "Malformed syntax, unknown versions, unknown enums and nonfinite binary64 values are rejected.",
        "Exact numerals remain canonical decimal BigInteger strings, bounded at 1024 numeral characters.",
        "A restriction with a condition referring to a symbol absent from its expression is refused during decode.",
        "The versioned SHA-256 structural semantic digest ignores passive symbol labels, while presentation digest includes them.",
        "Changing symbol domain or exact quantity changes the structural digest; identical admitted round trips yield identical digests.",
        "The existing exact rational, quantity, matrix, bound symbol and condition round-trip oracles remain qualified."),
    ["admission_limits"]=new JsonObject
    {
        ["maximum_json_characters"]=262144,
        ["maximum_exact_numeral_characters"]=1024,
        ["maximum_ir_expanded_nodes"]=1024,
        ["maximum_ir_depth"]=32,
        ["maximum_ir_assumptions"]=32
    },
    ["remaining_limits"]=p.Json.StringArray(
        "A bound symbol's declared scope metadata is preserved and internally validated; absent executable binder scopes cannot be globally proven against nonexistent declarations.",
        "Semantic structural hashes identify canonical trees, not general algebraic equivalence or alpha-equivalence.",
        "No network parser, symbolic solver, provider, OpenMath/MathML adapter or AURA host integration is claimed."),
    ["external_adoption_transmitted"]=false
};
p.Files.WriteComplete(QualifiedEvidence,evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",plan=>
{
    var c=plan["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
    var w=plan["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
    plan["plan_version"]="0.1.30";
    c["status"]="done";
    c["refinement"]="RS030 qualifies math-ir/1 lossless deterministic IR transport from RED through 10 round-trip oracles. RS031 observes three public-boundary REDs and hardens the decoder against closed-schema/duplicate keys, unrelated restriction identities, invalid literals and adversarial sizes; it adds separate versioned structural/presentation SHA-256 APIs, eleven boundary oracles and complete regressions. This closes M2-W01 while deferring executable binder and algebraic-equivalence claims.";
    var fileList=c["files"]!.AsArray();
    foreach(var rel in new[]{Codec,Guard,Hashes,RedTests,Tests,Doc,RedEvidence,QualifiedEvidence})
        if(!fileList.Any(x=>(string?)x==rel))
            fileList.Add((JsonNode?)JsonValue.Create(rel));
    var cmds=c["commands"]!.AsArray();
    foreach(var command in new[]{
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M2W01C03BoundaryRedTests",
        "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M2W01C03BoundaryTests"})
        if(!cmds.Any(x=>(string?)x==command))
            cmds.Add((JsonNode?)JsonValue.Create(command));
    var priorEvidence=c["evidence"]!.AsArray();
    foreach(var evidencePath in new[]{RedEvidence,QualifiedEvidence})
        if(!priorEvidence.Any(x=>(string?)x==evidencePath))
            priorEvidence.Add((JsonNode?)JsonValue.Create(evidencePath));
    t["status"]="done";
    t["evidence"]=p.Json.StringArray(RedEvidence,QualifiedEvidence);
    foreach(var sub in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==Red?RedEvidence:QualifiedEvidence);
    }
    w["status"]="done";
    w["evidence"]=p.Json.StringArray(
        "docs/planning/evidence/M2-W01-C01-qualified.json",
        "docs/planning/evidence/M2-W01-C02-boundary-qualified.json",
        QualifiedEvidence);
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Task,"ready","in-progress");
    p.ProjectPlan.TransitionNode(Red,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var node in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(node,"not-ready","ready");
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(node);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
    p.ProjectPlan.ConvergeNodeToDone(Work);
}
else
{
    foreach(var node in new[]{Red,Green,Verify,Task,Phase,Work})
        p.ProjectPlan.RequireNodeState(node,"done");
}
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static string Hash(byte[] bytes)=>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Relevant(string output)
{
    var position=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(position<0)position=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var slice=position<0?output:output[position..];
    return slice.Length>3100?slice[..3100]:slice;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    proc.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    proc.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000))
    {
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
static void Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)throw new InvalidOperationException(executable+" "+
        string.Join(" ",args)+" exited "+result.Code+": "+Relevant(result.Output));
}
