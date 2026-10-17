#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

// Read-only local Math evidence/roadmap admission. No product or DWF mutations.
const string Map="docs/planning/releases/m5-v1-local-candidate-map.json";
const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string ExpectedMarker="dw.tools.math/m5-v1-local-rc-evidence/1.0 qualified; criteria=10; predecessor-gates=5; publication=false";
static void Require(bool ok,string reason)
{
    if(!ok)throw new InvalidOperationException("M5 V1 candidate gate: "+reason);
}
static string ReadText(string path)
{
    Require(!Path.IsPathRooted(path) && !path.Contains("..",StringComparison.Ordinal) &&
        !path.Contains('\\'),"Only local normalized relative proof files are accepted: "+path);
    return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar)));
}
static JsonDocument Read(string path)=>JsonDocument.Parse(ReadText(path));
static string Text(JsonElement item,string key)=>
    item.GetProperty(key).GetString() ??
        throw new InvalidOperationException("Missing or null string: "+key);
static JsonElement Find(JsonElement array,string id)=>
    array.EnumerateArray().Single(item=>Text(item,"id")==id);
static void Status(JsonElement item,string status,string field="status") =>
    Require(Text(item,field)==status,"Unexpected "+field+" on "+Text(item,"id"));
static bool Flag(JsonElement item,string field)=>item.GetProperty(field).GetBoolean();
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(
        File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(),
            path.Replace('/',Path.DirectorySeparatorChar)))))
        .ToLowerInvariant();

Require(args.Length==0 || (args.Length==1&&args[0]=="--check"),
    "Only an observational --check invocation is admitted.");
using var mappingDoc=Read(Map);
using var backlogDoc=Read(Backlog);
using var nativeDoc=Read(Native);
var map=mappingDoc.RootElement;
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
Require(map.GetProperty("schema_version").GetInt32()==1 &&
    Text(map,"id")=="M5-W02-C03-T1-v1-local-candidate-evidence-map" &&
    Text(map,"status")=="bounded-v1-evidence-map-local-rc-not-promoted" &&
    Text(map,"candidate_label")=="1.0.0-local-rc-evidence-only" &&
    Text(map,"distribution_package_version")=="1.0.0-preview.4" &&
    Text(map,"source_coverage")=="docs/planning/coverage.md",
    "Candidate scope or origin framing has drifted.");
foreach(var field in new[]{
    "m5_final_gate_completed","public_nuget_published","stable_release_certified",
    "external_agent_adoption_observed","untested_other_rids_certified",
    "cross_version_source_or_binary_compatibility_certified",
    "planned_or_unbounded_capabilities_claimed"})
    Require(!Flag(map,field),"Unqualified capability cannot be claimed: "+field);
Require(Flag(map,"publication_requires_owner_authorization"),
    "The owner retains release authorization.");
var version=Text(backlog,"plan_version");
var baseline=version=="0.1.76";
var candidate=version=="0.1.77";
var boundary=version=="0.1.78";
var localMilestone=version=="0.1.79";
Require(baseline||candidate||boundary||localMilestone,
    "Only exact candidate, publication and local M5 milestone states are admitted.");
for(var index=0;index<5;index++)
{
    var id="M"+index;
    Status(Find(backlog.GetProperty("releases"),id),"done");
    Status(Find(native.GetProperty("milestones"),id),"done","state");
}
var m5=Find(backlog.GetProperty("releases"),"M5");
var m5native=Find(native.GetProperty("milestones"),"M5");
Status(m5,localMilestone?"done":"in_progress");
Status(m5native,localMilestone?"done":"in-progress","state");
Require(Text(m5,"gate")=="G-M5","M5 release gate identity drift.");
var w01=Find(m5native.GetProperty("workPackages"),"M5-W01");
var w02=Find(m5native.GetProperty("workPackages"),"M5-W02");
Status(w01,"done","state");
Status(w02,(boundary||localMilestone)?"done":"in-progress","state");
Status(Find(backlog.GetProperty("work_packages"),"M5-W01"),"done");
Status(Find(backlog.GetProperty("work_packages"),"M5-W02"),
    (boundary||localMilestone)?"done":"in_progress");
var phases=map.GetProperty("required_m5_phase_ids")
    .EnumerateArray().Select(x=>x.GetString()??"").ToArray();
Require(phases.Length==7 && phases.Distinct(StringComparer.Ordinal).Count()==7,
    "Expected seven canonical required M5 phases.");
Require(phases.SequenceEqual(m5.GetProperty("required_chunks")
    .EnumerateArray().Select(x=>x.GetString()??""),StringComparer.Ordinal),
    "The evidence map cannot redefine required M5 phases.");
for(var index=0;index<phases.Length;index++)
{
    var id=phases[index];
    var product=Find(backlog.GetProperty("chunks"),id);
    var n=(id.StartsWith("M5-W01-",StringComparison.Ordinal)?w01:w02);
    var workflow=Find(n.GetProperty("phases"),id);
    if(index<6)
    {
        Status(product,"done");Status(workflow,"done","state");
        Require(product.GetProperty("evidence").GetArrayLength()>0,
            "Already completed M5 phase has no evidence: "+id);
    }
    else
    {
        Status(product,baseline?"planned":(boundary||localMilestone)?"done":"in_progress");
        Status(workflow,baseline?"not-ready":(boundary||localMilestone)?"done":"in-progress","state");
        var t1=Find(product.GetProperty("tasks"),"M5-W02-C03-T1");
        Status(t1,baseline?"planned":"done");
        Status(Find(workflow.GetProperty("tasks"),"M5-W02-C03-T1"),
            baseline?"not-ready":"done","state");
        Status(Find(product.GetProperty("tasks"),"M5-W02-C03-T2"),
            (boundary||localMilestone)?"done":"planned");
        Status(Find(workflow.GetProperty("tasks"),"M5-W02-C03-T2"),
            (boundary||localMilestone)?"done":"not-ready","state");
    }
}
var gates=map.GetProperty("predecessor_gates").EnumerateArray().ToArray();
Require(gates.Length==5,"Exactly five predecessor rights/gates are required.");
for(var i=0;i<gates.Length;i++)
{
    var gate=gates[i];
    Require(Text(gate,"milestone")=="M"+i && gate.GetProperty("expected").GetBoolean(),
        "Predecessor gate list identity or expectation drift.");
    var path=Text(gate,"path");
    Require(path.StartsWith("docs/planning/evidence/",StringComparison.Ordinal),
        "Unqualified nonlocal predecessor receipt.");
    using var doc=Read(path);
    var observed=Text(gate,"field").Split('/');
    var value=observed.Aggregate(doc.RootElement,
        (node,key)=>node.GetProperty(key));
    Require(value.ValueKind==JsonValueKind.True,
        "Required gate/rights fact missing for M"+i+": "+path);
}
var sourceLines=ReadText("docs/planning/coverage.md").Split('\n')
    .Select(s=>s.TrimEnd('\r')).Where(s=>Regex.IsMatch(s,@"^\d+\.\s")).ToArray();
Require(sourceLines.Length==10,"The V1 framing source must contain exactly ten lines.");
var criteria=map.GetProperty("criteria").EnumerateArray().ToArray();
Require(criteria.Length==10,"Exactly ten mapped V1 criteria are required.");
for(var i=0;i<criteria.Length;i++)
{
    var item=criteria[i];
    var id="V1-"+(i+1).ToString("00",
        System.Globalization.CultureInfo.InvariantCulture);
    Require(Text(item,"id")==id && Text(item,"coverage_line")==sourceLines[i],
        "Criterion must preserve exact canonical framing and order: "+id);
    Require(Text(item,"scope_limit").Length>10,
        "Every V1 criterion must state a bounded nonclaim: "+id);
    var witnesses=item.GetProperty("proofs").EnumerateArray().ToArray();
    Require(witnesses.Length is >=1 and <=4,
        "Each criterion requires independently identified bounded evidence: "+id);
    foreach(var proof in witnesses)
    {
        var path=Text(proof,"path");
        Require(path.StartsWith("docs/planning/evidence/",StringComparison.Ordinal),
            "A criterion points outside durable Math scientific receipts: "+id);
        using var receipt=Read(path);
        Require(Text(receipt.RootElement,"status")==Text(proof,"expected_status"),
            "Changed certified scope or status: "+id+" "+path);
        var witnessKey=Text(proof,"witness_key");
        var actual=receipt.RootElement.GetProperty(witnessKey);
        Require(JsonElement.DeepEquals(actual,proof.GetProperty("expected")),
            "Mathematical evidence witness changed: "+id+" "+witnessKey);
    }
    var first=Text(witnesses[0],"path");
    Console.WriteLine("M5-V1-CRITERION:"+id+":"+Sha(first));
}
Console.WriteLine(ExpectedMarker);
