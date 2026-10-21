#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string PriorReport=".aura/workflow/reports/RS130/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS130-post-m7-readme-qualified.json";
const string Schema="docs/discovery/interop/decision-1.0.0-draft.1/manifest.schema.json";
const string Fixtures="docs/discovery/interop/decision-1.0.0-draft.1/conformance.json";
const string PinnedSource="docs/discovery/interop/decision-1.0.0-draft.1/source.json";
const string HistoricalPreflight="docs/discovery/qualification/portable-protocol-preflight.json";
const string PortableVerifier="docs/discovery/qualification/ValidatePortableProtocol.cs";
const string Candidate="docs/discovery/agent-surface-candidates.json";
const string ProposedWork="docs/planning/proposals/math-agent-discovery-work-package.json";
const string DiscoveryReadme="docs/discovery/README.md";
const string Decision="docs/planning/decisions/post-m7-discovery-portable-conformance.md";
const string Validator="docs/discovery/qualification/ValidateDwfPortableProtocolGate.cs";
const string Evidence="docs/discovery/qualification/RS131-portable-protocol-qualified.json";
const string Status="rs131-math-csharp-portable-discovery-corpus-qualified-without-agent-skill";
const string SchemaSha="678b42aa871eceec5f26d9b5dbfbbb31fdd0e62ef003204d6c623ba7e7d95170";
const string FixturesSha="5344a3bce40aa58990123cd51bee1b4a5cf72feb4c2e802da072ad19ac37001e";
const string ProtocolGitBlob="cd589183991774d1560129139f903dc8112c74b3";
const string Counts="MATH-DISCOVERY-RS131:CASES=13;ACCEPT=2;REJECT=11;CANDIDATE-STATIC=4;NATIVE=541";
const string Scope="MATH-DISCOVERY-RS131:PORTABLE-CSHARP=true;CLR-BINDINGS=false;SKILL=false;BROKER=false;EXTERNAL=false";
const string Marker="dw.tools.math/rs131-portable-discovery/1.0 qualified; cases=13; accepted=2; rejected=11; generated=false";

static void Need(bool pass,string reason)
{
    if(!pass) throw new InvalidOperationException("RS131 portable Discovery gate: "+reason);
}
static string Local(string path)
{
    Need(!string.IsNullOrWhiteSpace(path)&&!Path.IsPathRooted(path)&&
         !path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains(':')&&!path.Contains('\\'),
         "Unsafe repository source path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(Local(path)))).ToLowerInvariant();
static string Blob(string path)
{
    var bytes=Encoding.UTF8.GetBytes(Read(path).Replace("\r\n","\n",
        StringComparison.Ordinal));
    var header=Encoding.ASCII.GetBytes("blob "+bytes.Length+"\0");
    var framed=new byte[header.Length+bytes.Length];
    Buffer.BlockCopy(header,0,framed,0,header.Length);
    Buffer.BlockCopy(bytes,0,framed,header.Length,bytes.Length);
    return Convert.ToHexString(SHA1.HashData(framed)).ToLowerInvariant();
}
static string Text(JsonElement x,string key)=>
    x.GetProperty(key).GetString()??
        throw new InvalidOperationException("Missing text "+key);
static bool Flag(JsonElement x,string key)=>x.GetProperty(key).GetBoolean();
static void Has(string src,string needle,string owner)=>
    Need(src.Contains(needle,StringComparison.Ordinal),owner+" lacks "+needle);
static int Count(JsonElement node)
{
    var result=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            result+=children.EnumerateArray().Sum(Count);
    return result;
}
static int Undone(JsonElement node)
{
    var result=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            result+=children.EnumerateArray().Sum(Undone);
    return result;
}
static bool Hex40(string s)=>s.Length==40&&s.All(c=>
    (c>='0'&&c<='9')||(c>='a'&&c<='f'));

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Read-only C# checker supports --check only.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorReport=Open(PriorReport);
using var priorReceipt=Open(PriorReceipt);
using var sourceDoc=Open(PinnedSource);
using var fixturesDoc=Open(Fixtures);
using var schemaDoc=Open(Schema);
using var preflightDoc=Open(HistoricalPreflight);
using var candidateDoc=Open(Candidate);
using var workDoc=Open(ProposedWork);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=priorReport.RootElement;
var receipt=priorReceipt.RootElement;
var pinned=sourceDoc.RootElement;
var fixtures=fixturesDoc.RootElement;
var schema=schemaDoc.RootElement;
var preflight=preflightDoc.RootElement;
var candidate=candidateDoc.RootElement;
var work=workDoc.RootElement;

Need(Text(backlog,"plan_version")=="0.1.122"&&
     Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106&&
     backlog.GetProperty("releases").EnumerateArray()
        .All(x=>Text(x,"status")=="done"),
     "Accepted Math product backlog must not be reopened for an evidence gate.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Undone)==0,
     "The accepted 541 native Math DWF nodes must remain done.");
Need(Text(report,"runId")=="RS130"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: reconcile current Math README with qualified local delivery"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59"&&
     report.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0&&
     Flag(report.GetProperty("priorInvocationFailures"),"firstPassSucceeded"),
     "RS130 must be a confirmed first-pass success.");
Need(Text(receipt,"status")==
    "rs130-current-math-readme-and-immutable-historical-proof-reconciled-unpublished"&&
     Text(receipt,"approved_sequence")=="A-B-D"&&
     Flag(receipt,"readme_current")&&
     Flag(receipt,"historical_source_pins_preserved")&&
     receipt.GetProperty("source_projects").GetInt32()==13&&
     receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
     !Flag(receipt,"publicly_published")&&
     !Flag(receipt,"external_requests_transmitted")&&
     !Flag(receipt,"discovery_skill_generated")&&
     !Flag(receipt,"future_program_approved"),
     "RS130 documentation decision cannot authorize a new product milestone.");
foreach(var (field,path) in new[]{
    ("readme_sha256","README.md"),
    ("guide_sha256","docs/distribution/current-local-consumption.md"),
    ("decision_sha256","docs/planning/decisions/post-m7-readme-reconciliation.md"),
    ("validator_sha256","docs/planning/ValidatePostM7ReadmeStatus.cs")
}) Need(Text(receipt,field)==Sha(path),"Historic RS130 source identity drifted: "+path);
Has(Read("README.md"),"Current status — 2026-10-21","Current Math README");
Need(Text(pinned,"status")==
       "reference-only-verified-sha256-not-Math-CSharp-conformance"&&
     Text(pinned,"protocol_id")=="dw.discovery"&&
     Text(pinned,"protocol_version")=="1.0.0-draft.1"&&
     pinned.GetProperty("manifest_schema_version").GetInt32()==2&&
     Text(pinned,"source_commit")==
       "5d3af8580ea984425b314d589def18e031172c2c"&&
     Text(pinned.GetProperty("content_sha256_verified"),
       "docs/discovery/manifest.schema.json")==SchemaSha&&
     Text(pinned.GetProperty("content_sha256_verified"),
       "docs/discovery/conformance.json")==FixturesSha,
     "Immutable pinned Decision protocol authority was changed.");
Need(Sha(Schema)==SchemaSha&&Sha(Fixtures)==FixturesSha&&
     Text(schema,"$id")=="urn:dw:discovery:manifest:2:draft-1"&&
     fixtures.GetProperty("fixtureVersion").GetInt32()==1&&
     fixtures.GetProperty("cases").GetArrayLength()==13,
     "Pinned fixture/schema source bytes or identity drifted.");
var all=fixtures.GetProperty("cases").EnumerateArray().ToArray();
var expectedNames=new[]{
    "valid-math-envelope","valid-decision-envelope","unknown-schema",
    "unknown-protocol","documented-leak","wrong-owner","stale-signature",
    "missing-limits","premature-broker-ready","metadata-authorization",
    "bogus-version","duplicate-capability","catalogue-leak"
};
Need(all.Length==expectedNames.Length&&
     all.Select(x=>Text(x,"name"))
        .SequenceEqual(expectedNames,StringComparer.Ordinal)&&
     all.Count(x=>Flag(x,"accept"))==2&&
     !all.Skip(2).Any(x=>Flag(x,"accept")),
     "The exact 2/11 portable fixture outcomes or order were altered.");
Need(Text(preflight,"status")==
      "portable-envelope-preflight-13-of-13-passed-NOT-Math-CSharp-conformance"&&
     Text(preflight.GetProperty("math_verifier_candidate"),"execution_state")==
       "authored-not-compiled-or-executed-in-the-target-.NET-environment",
     "Historical JavaScript preflight must not be relabelled as C# evidence.");
Need(Blob(PortableVerifier)==ProtocolGitBlob&&
     Read(PortableVerifier).Contains(
       "RESULT PASS 13/13 (2 accepted, 11 rejected) independent Math C# envelope verifier",
       StringComparison.Ordinal),
     "The originally authored independent Math C# verifier source drifted.");
Need(Text(candidate,"status")=="proposed-not-implemented-not-admitted"&&
     Text(candidate,"protocol")=="dw.discovery/1.0.0-draft.1"&&
     candidate.GetProperty("proposed_agent_callable_seed").GetArrayLength()==4&&
     candidate.GetProperty("not_selected").GetArrayLength()==7&&
     candidate.GetProperty("deferred_agent_operations").GetArrayLength()==3,
     "The intentionally selected four static candidates are not accepted bindings.");
var candidates=candidate.GetProperty("proposed_agent_callable_seed")
    .EnumerateArray().ToArray();
Need(candidates.Select(x=>Text(x,"capabilityId"))
        .Distinct(StringComparer.Ordinal).Count()==4,
     "Candidate operation IDs must be distinct.");
foreach(var entry in candidates)
{
    var source=Text(entry,"source");
    var previousBlob=Text(entry,"source_blob");
    Need(Text(entry,"exposure")=="AgentCallable"&&
         Text(entry,"kind")=="public static"&&
         !Flag(entry,"brokerReady")&&
         Hex40(previousBlob)&&Blob(source)==previousBlob&&
         Text(entry,"capabilityId").StartsWith("math.",StringComparison.Ordinal),
         "A selected candidate source identity drifted or claimed BrokerReady.");
}
Need(Text(work,"status")=="draft-not-merged-into-canonical-backlog-or-native-DWF"&&
     Text(work.GetProperty("placement"),"work_package")=="M3-W03"&&
     work.GetProperty("chunks").GetArrayLength()==3,
     "M3-W03 product work package is still a proposal.");
Has(Read("docs/workflow/payload-sdk-reference.md"),
    "ProjectPlan.EnsurePhaseWithTasks(","Current installed DWF operation contract");
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","Recipient request boundary");
Need(!Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "Generated Math skill is not authorized or delivered.");
var readme=Read(DiscoveryReadme);
foreach(var term in new[]{
    "No generated Math skill",
    "RS131",
    "RS131-portable-protocol-qualified.json",
    "13 per-case verdicts",
    "M3-W03",
    "issue #139",
    "not an agent callable binding"
}) Has(readme,term,"Discovery status document");
var decision=Read(Decision);
foreach(var term in new[]{
    "## Authorized stage B and strict scope",
    "## Immutable verified inputs",
    "## Execution, validation and evidence",
    "## Native planning boundary for next B runs",
    "## Nonclaims and next action",
    "13","11","AgentCallable","without publication"
}) Has(decision,term,"RS131 authorization decision");

if(File.Exists(Local(Evidence)))
{
    using var evidenceDoc=Open(Evidence);
    var result=evidenceDoc.RootElement;
    Need(Text(result,"status")==Status&&
         Text(result,"protocol_id")=="dw.discovery"&&
         Text(result,"protocol_version")=="1.0.0-draft.1"&&
         result.GetProperty("manifest_schema_version").GetInt32()==2&&
         Text(result,"decision_source_commit")==
            "5d3af8580ea984425b314d589def18e031172c2c"&&
         Text(result,"schema_sha256")==SchemaSha&&
         Text(result,"fixtures_sha256")==FixturesSha&&
         Text(result,"portable_verifier_git_blob")==ProtocolGitBlob&&
         result.GetProperty("total_cases").GetInt32()==13&&
         result.GetProperty("accepted_cases").GetInt32()==2&&
         result.GetProperty("rejected_cases").GetInt32()==11&&
         result.GetProperty("case_results").GetArrayLength()==13&&
         result.GetProperty("candidate_static_operations").GetInt32()==4&&
         result.GetProperty("native_nodes_retained").GetInt32()==541&&
         Flag(result,"csharp_protocol_verifier_executed")&&
         Flag(result,"csharp_portable_envelope_conformant")&&
         Flag(result,"candidate_source_pins_match")&&
         !Flag(result,"selected_clr_bindings_reflected")&&
         !Flag(result,"compiled_binding_examples")&&
         !Flag(result,"generated_math_skill")&&
         !Flag(result,"broker_ready")&&
         !Flag(result,"math_decision_runtime_interoperability_certified")&&
         !Flag(result,"new_discovery_work_package_admitted")&&
         !Flag(result,"m0_m7_plan_mutated")&&
         !Flag(result,"publicly_published")&&
         !Flag(result,"external_requests_transmitted")&&
         !Flag(result,"external_approvals_received")&&
         Text(result,"discovery_readme_sha256")==Sha(DiscoveryReadme)&&
         Text(result,"decision_sha256")==Sha(Decision)&&
         Text(result,"validator_sha256")==Sha(Validator),
         "RS131 target claims unadmitted agent/runtime capabilities.");
    var recorded=result.GetProperty("case_results").EnumerateArray().ToArray();
    for(var i=0;i<all.Length;i++)
        Need(Text(recorded[i],"name")==Text(all[i],"name")&&
             Flag(recorded[i],"expected_accept")==Flag(all[i],"accept")&&
             Flag(recorded[i],"observed_accept")==Flag(all[i],"accept")&&
             Flag(recorded[i],"passed"),
             "Per-case C# execution verdict differs from pinned portable fixture "+
                 Text(all[i],"name"));
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
