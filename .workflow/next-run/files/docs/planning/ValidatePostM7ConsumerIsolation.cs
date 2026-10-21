#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string PreviousReport=".aura/workflow/reports/RS124/attempt-001.json";
const string PreviousReceipt="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
const string M7="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Decision="docs/planning/decisions/post-m7-source-free-consumer-qualification.md";
const string Matrix="docs/distribution/post-m7-source-free-consumer-matrix.md";
const string Evidence="docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json";
const string Status="rs125-two-fresh-isolated-source-free-nuget-consumers-qualified-unpublished";
const string Counts="MATH-RS125:PACKAGES=2;SOURCE-FREE-CONSUMERS=2;FRESH-PROBES=16;NATIVE=541";
const string Scope="MATH-RS125:SCOPE=NO-PROGRESS;OFFLINE-LOCAL-FEEDS=true;PUBLICATION=false;AGENT-DISCOVERY=false";
const string Marker="dw.tools.math/rs125-source-free-nuget/1.1 qualified; consumers=2; probes=16; current-local-packages=true; external=false";

static void Need(bool ok,string reason)
{
    if(!ok) throw new InvalidOperationException("RS125 source-free NuGet isolation: "+reason);
}
static string PathOf(string relative)
{
    Need(!string.IsNullOrWhiteSpace(relative)&&!Path.IsPathRooted(relative)&&
         !relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains(':')&&!relative.Contains('\\'),
         "Unsafe source path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(PathOf(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(PathOf(path)))).ToLowerInvariant();
static string Text(JsonElement node,string key)=>
    node.GetProperty(key).GetString()??
        throw new InvalidOperationException("Missing string field "+key);
static bool Flag(JsonElement node,string field)=>node.GetProperty(field).GetBoolean();
static void Has(string body,string term,string source)=>
    Need(body.Contains(term,StringComparison.OrdinalIgnoreCase),
         source+" lacks "+term);
static string Property(XDocument doc,string field)=>
    doc.Descendants().FirstOrDefault(x=>x.Name.LocalName==field)?.Value.Trim()??
    throw new InvalidOperationException("Missing csproj property "+field);
static int Count(JsonElement node)
{
    var n=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int Unfinished(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Unfinished);
    return n;
}
Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(PreviousReport);
using var previousDoc=Open(PreviousReceipt);
using var m7Doc=Open(M7);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var old=previousDoc.RootElement;
var m7=m7Doc.RootElement;
var target=File.Exists(PathOf(Evidence));

Need(Text(backlog,"plan_version")=="0.1.122"&&Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106,
     "Accepted product backlog was modified or re-opened.");
Need(backlog.GetProperty("releases").EnumerateArray().All(x=>
        Text(x,"status")=="done")&&
     backlog.GetProperty("work_packages").EnumerateArray().All(x=>
        Text(x,"status")=="done")&&
     backlog.GetProperty("chunks").EnumerateArray().All(x=>
        Text(x,"status")=="done"),
     "A previously accepted node was demoted.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Unfinished)==0,
     "Native 541-node plan was modified or a node was reopened.");
Need(Text(report,"runId")=="RS124"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
        "docs: qualify post-M7 local delivery inventory without publication"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59",
     "RS124 must be durable successful attempt 001.");
var failure=report.GetProperty("priorInvocationFailures");
Need(failure.GetProperty("count").GetInt32()==1&&
     failure.GetProperty("preparationFailureCount").GetInt32()==1&&
     failure.GetProperty("allocatedAttemptFailureCount").GetInt32()==0&&
     !Flag(failure,"firstPassSucceeded"),
     "Historical RS124 pre-attempt refusal must remain truthful.");
Need(Text(old,"status")==
        "rs124-current-source-build-test-and-three-local-packages-qualified-unpublished"&&
     old.GetProperty("source_projects").GetInt32()==13&&
     old.GetProperty("test_projects").GetInt32()==11&&
     old.GetProperty("historical_consumer_directories").GetInt32()==47&&
     old.GetProperty("fresh_local_package_archives").GetInt32()==3&&
     Flag(old,"locked_restore_passed")&&Flag(old,"release_build_passed")&&
     Flag(old,"focused_regressions_passed")&&Flag(old,"full_solution_tests_passed")&&
     Flag(old,"representative_local_packs_passed")&&
     !Flag(old,"fresh_source_free_consumer_rerun")&&
     !Flag(old,"publicly_published")&&
     !Flag(old,"generated_discovery_skill_delivered")&&
     !Flag(old,"external_requests_transmitted"),
     "RS124 package evidence or declared gaps changed.");
foreach(var (key,path) in new[]{
    ("decision_sha256","docs/planning/decisions/post-m7-delivery-qualification.md"),
    ("distribution_sha256","docs/distribution/post-m7-delivery-inventory.md"),
    ("validator_sha256","docs/planning/ValidatePostM7DeliveryInventory.cs")
}) Need(Text(old,key)==Sha(path),
        "Source identity of predecessor changed: "+path);
Need(Text(m7,"status")==
        "m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"&&
     Flag(m7,"m7_internal_scope_done")&&!Flag(m7,"public_release_authorized")&&
     !Flag(m7,"publicly_published")&&
     !Flag(m7,"external_requests_transmitted")&&
     !Flag(m7,"external_approvals_received")&&
     !Flag(m7,"future_program_approved"),
     "M7 internal gate cannot be a public release or external acceptance.");

var selected=new (string Id,string Version,string Folder,string Project,string Program,string Token,string Marker)[]
{
    ("dw.tools.math.analysis","1.1.0-preview.6",
     "m6-w01-c03-t2-local-feed",
     "tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj",
     "tests/consumers/m6-w01-c03-t2/Program.cs",
     "M6-W01-C03-T2-CONSUMER-SHA256:",
     "dw.tools.math/m6-differential-boundary-consumer/1.1 qualified; probes=8"),
    ("dw.tools.math.scientific","1.1.0-preview.14",
     "m6-w02-c04-t2-local-feed",
     "tests/consumers/m6-w02-c04-t2/m6-dated-finance-consumer.csproj",
     "tests/consumers/m6-w02-c04-t2/Program.cs",
     "M6-W02-C04-T2-CONSUMER-SHA256:",
     "dw.tools.math/m6-dated-finance-consumer/1.1 qualified; probes=8")
};
var localSolution=XDocument.Parse(Read("dw.tools.math.slnx"));
var included=localSolution.Descendants().Where(x=>x.Name.LocalName=="Project")
    .Select(x=>(string?)x.Attribute("Path")??"").ToHashSet(StringComparer.Ordinal);
foreach(var client in selected)
{
    var product="src/projects/"+client.Id+"/"+client.Id+".csproj";
    Need(included.Contains(product),"Missing compiled library from solution: "+client.Id);
    var library=XDocument.Parse(Read(product));
    Need(Property(library,"PackageId")==client.Id&&
         Property(library,"Version")==client.Version&&
         Property(library,"IsPackable")=="true"&&
         !library.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"),
         "Library package graph changed: "+client.Id);
    var clientProject=XDocument.Parse(Read(client.Project));
    var packRefs=clientProject.Descendants()
        .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
    Need(packRefs.Length==1&&
         (string?)packRefs[0].Attribute("Include")==client.Id&&
         (string?)packRefs[0].Attribute("Version")=="["+client.Version+"]"&&
         !clientProject.Descendants().Any(x=>x.Name.LocalName=="ProjectReference")&&
         Property(clientProject,"ManagePackageVersionsCentrally")=="false"&&
         Property(clientProject,"IsPackable")=="false"&&
         Property(clientProject,"TargetFramework")=="net10.0",
         "Consumer must have exact NuGet PackageReference only: "+client.Id);
    var config=Read(Path.GetDirectoryName(client.Project)!.Replace('\\','/')+
        "/NuGet.Config");
    var xml=XDocument.Parse(config);
    var sources=xml.Descendants().Single(x=>x.Name.LocalName=="packageSources");
    Need(sources.Elements().Count()==1&&
         sources.Elements().Single().Name.LocalName=="clear",
         "Consumer must clear remote NuGet feeds: "+client.Id);
    var program=Read(client.Program);
    Has(program,client.Token,"Consumer ZIP and SHA oracle");
    Has(program,client.Marker,"Consumer independent 8-probe marker");
    Has(program,client.Folder,"Consumer exact named offline feed");
    Has(program,"SHA256.HashData","Consumer computes archive SHA-256");
    Need(Regex.Matches(program,@"^Probe\(",RegexOptions.Multiline).Count==8,
         "Consumer must retain exactly eight probes: "+client.Id);
    Need(!program.Contains("ProjectReference",StringComparison.Ordinal),
         "Source-free consumer acquired a project reference in source.");
}
Has(Read("docs/coordination/requests/README.md"),"have not been sent",
    "Recipient requests");
Need(!Directory.Exists(PathOf(".agents/skills/dw-tools-math")),
    "Math generated agent skill is not part of this scope");
var decision=Read(Decision);
foreach(var term in new[]{"## Question and authority","## Pinned baseline",
    "## Exact controlled execution","## Acceptance and nonclaims",
    "RS124","without publication"})
    Has(decision,term,"RS125 decision");
var matrix=Read(Matrix);
foreach(var term in new[]{"## Source-to-package-to-consumer table",
    "## Historical versus fresh evidence","## Receipt and release boundary",
    "probes=8","NuGet.Config","No GitHub Actions"})
    Has(matrix,term,"RS125 consumer matrix");

if(target)
{
    using var receiptDoc=Open(Evidence);
    var receipt=receiptDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("source_free_consumers").GetInt32()==2&&
         receipt.GetProperty("fresh_consumer_probes").GetInt32()==16&&
         receipt.GetProperty("fresh_packages_built").GetInt32()==2&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         receipt.GetProperty("command_receipts").GetArrayLength()==9&&
         receipt.GetProperty("consumer_results").GetArrayLength()==2&&
         Flag(receipt,"offline_local_feeds_only")&&
         Flag(receipt,"isolated_nuget_package_cache")&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"discovery_skill_generated")&&
         !Flag(receipt,"cross_version_abi_certified")&&
         !Flag(receipt,"other_os_rids_certified")&&
         !Flag(receipt,"all_historical_consumers_rerun")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Matrix)&&
         Text(receipt,"validator_sha256")==Sha(
             "docs/planning/ValidatePostM7ConsumerIsolation.cs"),
         "Target receipt lacks exact observed local source-free qualifications.");
    var commandIds=new[]{
        "source-locked-restore","analysis-source-build","scientific-source-build",
        "analysis-local-pack","scientific-local-pack",
        "analysis-offline-restore","analysis-source-free-consumer",
        "scientific-offline-restore","scientific-source-free-consumer"
    };
    var commands=receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    for(var i=0;i<commandIds.Length;i++)
        Need(Text(commands[i],"id")==commandIds[i]&&
             commands[i].GetProperty("exit_code").GetInt32()==0,
             "Missing successful command: "+commandIds[i]);
    var results=receipt.GetProperty("consumer_results").EnumerateArray().ToArray();
    for(var i=0;i<selected.Length;i++)
    {
        var row=results[i];
        Need(Text(row,"package_id")==selected[i].Id&&
             Text(row,"version")==selected[i].Version&&
             row.GetProperty("probes").GetInt32()==8&&
             row.GetProperty("archive_bytes").GetInt64()>0&&
             Text(row,"archive_sha256").Length==64&&
             Text(row,"archive_sha256").All(c=>
                 (c>='0'&&c<='9')||(c>='a'&&c<='f'))&&
             Flag(row,"source_free")&&Flag(row,"offline_feed")&&
             Flag(row,"isolated_cache"),
             "One current-source consumer receipt is incomplete: "+selected[i].Id);
    }
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
