#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

const string Solution="dw.tools.math.slnx";
const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior=".aura/workflow/reports/RS123/attempt-001.json";
const string M5="docs/planning/evidence/M5-gate.json";
const string M6="docs/planning/evidence/M6-gate.json";
const string M7="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Decision="docs/planning/decisions/post-m7-delivery-qualification.md";
const string Matrix="docs/distribution/post-m7-delivery-inventory.md";
const string Evidence="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
const string Status="rs124-current-source-build-test-and-three-local-packages-qualified-unpublished";
const string Marker="dw.tools.math/rs124-post-m7-delivery/1.0 inventory-qualified; source=13; tests=11; historical-consumers=47; public=false";
const string Counts="MATH-DELIVERY:SOURCE=13;TESTS=11;SOLUTION=24;HISTORICAL-CONSUMER-DIRS=47";
const string Scope="MATH-DELIVERY:SCOPE:M0-M7=UNCHANGED;NODES=541;PACKS-TARGET=3;RELEASE=false;DISCOVERY-CANDIDATE=true";

static void Need(bool ok,string reason)
{
    if(!ok) throw new InvalidOperationException("Math delivery qualification: "+reason);
}
static string Local(string path)
{
    Need(!string.IsNullOrWhiteSpace(path)&&!Path.IsPathRooted(path)&&
         !path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\')&&!path.Contains(':'),
         "Unsafe relative source path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static string Text(JsonElement value,string key)=>
    value.GetProperty(key).GetString()??
        throw new InvalidOperationException("Missing string property "+key);
static bool Flag(JsonElement value,string key)=>value.GetProperty(key).GetBoolean();
static void Has(string body,string token,string label)=>
    Need(body.Contains(token,StringComparison.OrdinalIgnoreCase),
         label+" lacks "+token);
static string Tag(XDocument xml,string name)=>
    xml.Descendants().FirstOrDefault(x=>x.Name.LocalName==name)?.Value.Trim()??
        throw new InvalidOperationException("Missing XML property "+name);
static int Count(JsonElement node)
{
    var n=1;
    foreach(var k in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(k,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int NotDone(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var k in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(k,out var children))
            n+=children.EnumerateArray().Sum(NotDone);
    return n;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
     "Only read-only --check is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
using var m5Doc=Open(M5);
using var m6Doc=Open(M6);
using var m7Doc=Open(M7);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var m5=m5Doc.RootElement;
var m6=m6Doc.RootElement;
var m7=m7Doc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")=="0.1.122"&&Text(backlog,"status")=="done",
     "The accepted M0-M7 product roadmap must remain unchanged.");
Need(backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106&&
     backlog.GetProperty("source_coverage").GetArrayLength()==20&&
     backlog.GetProperty("dispositions").GetArrayLength()==6,
     "Canonical backlog cardinalities drifted.");
Need(backlog.GetProperty("releases").EnumerateArray()
         .All(x=>Text(x,"status")=="done")&&
     backlog.GetProperty("work_packages").EnumerateArray()
         .All(x=>Text(x,"status")=="done")&&
     backlog.GetProperty("chunks").EnumerateArray()
         .All(x=>Text(x,"status")=="done"),
     "Accepted historical progress was reopened.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(NotDone)==0,
     "Every accepted DWF node must remain done without a new roadmap.");
Need(Text(prior,"runId")=="RS123"&&
     prior.GetProperty("attempt").GetInt32()==1&&
     Text(prior,"outcome")=="succeeded"&&
     !Flag(prior,"correctionMode")&&
     Text(prior.GetProperty("git"),"commitMessage")==
        "docs: qualify M7 internal scope gate without publication"&&
     Text(prior.GetProperty("executor"),"toolVersion")=="0.1.59"&&
     prior.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0&&
     Flag(prior.GetProperty("priorInvocationFailures"),"firstPassSucceeded"),
     "Previous RS123 first-pass evidence changed.");
Need(Text(m5,"status")==
       "m5-local-v1-independent-nuget-evidence-gate-qualified-unpublished"&&
     m5.GetProperty("seven_local_nuget_packages_inspected").GetInt32()==7&&
     m5.GetProperty("independent_nuget_consumers").GetInt32()==3&&
     !Flag(m5,"packages_publicly_published")&&
     !Flag(m5,"owner_stable_publication_approved"),
     "M5 local package evidence must not be rewritten as public release.");
Need(Text(m6,"status")==
       "m6-independent-local-analysis-scientific-release-gate-qualified-unpublished"&&
     m6.GetProperty("historical_source_free_consumers").GetInt32()==10&&
     m6.GetProperty("historical_source_free_probes").GetInt32()==80&&
     !Flag(m6,"packages_publicly_published")&&
     !Flag(m6,"owner_stable_publication_approved"),
     "M6 local package evidence must retain historical-only qualification.");
Need(Text(m7,"status")==
       "m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"&&
     Flag(m7,"m7_internal_scope_done")&&
     !Flag(m7,"public_release_authorized")&&
     !Flag(m7,"publicly_published")&&
     !Flag(m7,"external_requests_transmitted")&&
     !Flag(m7,"external_approvals_received")&&
     !Flag(m7,"future_program_approved")&&
     !Flag(m7,"new_advanced_public_product_delivered")&&
     m7.GetProperty("native_nodes_retained").GetInt32()==541,
     "M7 research-only publication and owner deferral drifted.");
foreach(var (key,path) in new[]{
    ("decision_sha256","docs/planning/decisions/m7-internal-gate-closure.md"),
    ("distribution_sha256","docs/distribution/m7-internal-gate-matrix.md"),
    ("validator_sha256","docs/planning/ValidateM7MilestoneGate.cs")
})
    Need(Text(m7,key)==Sha(path),"M7 scientific source SHA drifted: "+path);

var expected=new (string Id,string Version)[]
{
    ("dw.tools.math.foundation","0.1.0-preview.1"),
    ("dw.quantities","0.2.0-preview.1"),
    ("dw.quantities.standard","0.2.0-preview.1"),
    ("dw.quantities.expression","0.2.0-preview.1"),
    ("dw.tools.math.ir","0.3.0-preview.1"),
    ("dw.tools.math.composition","0.3.0-preview.1"),
    ("dw.tools.math.numerics","0.4.0-preview.4"),
    ("dw.tools.math.statistics","0.4.0-preview.6"),
    ("dw.tools.math.symbolics","0.5.0-preview.10"),
    ("dw.tools.math.codecs","1.0.0-preview.8"),
    ("dw.tools.math.distribution","1.0.0-preview.4"),
    ("dw.tools.math.analysis","1.1.0-preview.6"),
    ("dw.tools.math.scientific","1.1.0-preview.14")
};
var solution=XDocument.Parse(Read(Solution));
var projectPaths=solution.Descendants()
    .Where(x=>x.Name.LocalName=="Project")
    .Select(x=>(string?)x.Attribute("Path")??"").ToArray();
Need(projectPaths.Length==24&&
     projectPaths.Distinct(StringComparer.OrdinalIgnoreCase).Count()==24,
     "Solution must contain exactly 24 unique source/test projects.");
var sourcePaths=projectPaths.Where(x=>x.StartsWith("src/projects/",StringComparison.Ordinal)).ToArray();
var testPaths=projectPaths.Where(x=>x.StartsWith("tests/projects/",StringComparison.Ordinal)).ToArray();
Need(sourcePaths.Length==13&&testPaths.Length==11,
     "Source/test solution split differs from verified 13 + 11.");
Need(sourcePaths.Select(x=>Path.GetFileNameWithoutExtension(x))
       .OrderBy(x=>x,StringComparer.Ordinal)
       .SequenceEqual(expected.Select(x=>x.Id).OrderBy(x=>x,StringComparer.Ordinal)),
     "One of the thirteen source package IDs is absent from the solution.");
var allProjectPaths=projectPaths.ToHashSet(StringComparer.OrdinalIgnoreCase);
foreach(var path in projectPaths)
{
    Need(path.EndsWith(".csproj",StringComparison.Ordinal)&&
         File.Exists(Local(path)),"Missing declared project: "+path);
    var project=XDocument.Parse(Read(path));
    var references=project.Descendants().Where(x=>
        x.Name.LocalName=="ProjectReference").ToArray();
    foreach(var reference in references)
    {
        var link=(string?)reference.Attribute("Include")??"";
        Need(!string.IsNullOrEmpty(link)&&!Path.IsPathRooted(link)&&
             !link.Contains(':')&&!link.Contains('\\'),
             "Unsafe project reference for "+path);
        var candidate=Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Local(path))!,link.Replace('/',Path.DirectorySeparatorChar)));
        var relative=Path.GetRelativePath(Directory.GetCurrentDirectory(),candidate)
            .Replace(Path.DirectorySeparatorChar,'/');
        Need(!relative.StartsWith("../",StringComparison.Ordinal)&&
             allProjectPaths.Contains(relative)&&File.Exists(candidate),
             "Project reference escapes or is not in the solution: "+path+" -> "+link);
    }
}
foreach(var (id,version) in expected)
{
    var path="src/projects/"+id+"/"+id+".csproj";
    var xml=XDocument.Parse(Read(path));
    Need(Tag(xml,"PackageId")==id&&Tag(xml,"Version")==version&&
         Tag(xml,"IsPackable")=="true"&&
         Tag(xml,"RestorePackagesWithLockFile")=="true",
         "Unreviewed package metadata or locked restore contract: "+id);
}
var build=XDocument.Parse(Read("Directory.Build.props"));
Need(Tag(build,"TargetFramework")=="net10.0"&&
     Tag(build,"TreatWarningsAsErrors")=="true"&&
     Tag(build,"Deterministic")=="true",
     "Selected .NET framework/determinism policy changed.");
using var sdkDoc=Open("global.json");
var sdk=sdkDoc.RootElement.GetProperty("sdk");
Need(Text(sdk,"version")=="10.0.401"&&
     Text(sdk,"rollForward")=="latestPatch"&&
     !Flag(sdk,"allowPrerelease"),
     "Selected toolchain SDK policy drifted.");
var consumers=Directory.GetDirectories(Local("tests/consumers"))
    .Select(x=>Path.GetFileName(x)).ToArray();
Need(consumers.Length==47&&
     consumers.Distinct(StringComparer.OrdinalIgnoreCase).Count()==47,
     "Historical consumer folder roster changed; do not infer new runs.");
Need(File.Exists(Local("docs/discovery/README.md"))&&
     File.Exists(Local("docs/discovery/agent-surface-candidates.json"))&&
     !Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "The candidate Discovery surface was unexpectedly promoted.");
Has(Read("docs/discovery/README.md"),"No generated Math skill",
    "Discovery candidate boundary");
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","Recipient coordination boundary");
foreach(var (path,key) in new[]{
    ("docs/coordination/requests/MATH-XR-001-aura.md","MATH-XR-001"),
    ("docs/coordination/requests/MATH-XR-002-decision.md","MATH-XR-002"),
    ("docs/coordination/requests/MATH-XR-003-mcdm.md","MATH-XR-003")
})
{
    var request=Read(path);
    Has(request,key,"Owner request");
    Has(request,"draft","Owner request");
    Has(request,"Transmission","Owner request");
    Has(request,"none","Owner request");
}
Has(Read("README.md"),"Status on 2026-10-07",
    "Stale historical root README must be recorded as documentation debt");
var decision=Read(Decision);
foreach(var term in new[]{
    "## Question and authorization",
    "## Source-controlled observations at selection baseline",
    "## Execution and independent acceptance",
    "## Accepted result and next decisions",
    "13 product","11 test","47 historical","without publication"
}) Has(decision,term,"Delivery-qualification decision");
var matrix=Read(Matrix);
foreach(var term in new[]{
    "## Thirteen packable source projects in the solution",
    "## Qualified historical observations versus fresh RS124 checks",
    "## Static and dynamic verdicts",
    "dw.tools.math.scientific","47","unsent"
}) Has(matrix,term,"Delivery package inventory");

if(target)
{
    using var receiptDoc=Open(Evidence);
    var receipt=receiptDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("source_projects").GetInt32()==13&&
         receipt.GetProperty("test_projects").GetInt32()==11&&
         receipt.GetProperty("historical_consumer_directories").GetInt32()==47&&
         receipt.GetProperty("fresh_local_package_archives").GetInt32()==3&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         Flag(receipt,"locked_restore_passed")&&
         Flag(receipt,"release_build_passed")&&
         Flag(receipt,"focused_regressions_passed")&&
         Flag(receipt,"full_solution_tests_passed")&&
         Flag(receipt,"representative_local_packs_passed")&&
         !Flag(receipt,"public_release_authorized")&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"generated_discovery_skill_delivered")&&
         !Flag(receipt,"all_thirteen_packages_repacked")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Matrix)&&
         Text(receipt,"validator_sha256")==Sha(
             "docs/planning/ValidatePostM7DeliveryInventory.cs"),
         "Target receipt must prove only fresh local checks, not publication.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
