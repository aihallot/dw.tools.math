#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string PriorReport=".aura/workflow/reports/RS126/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS126-post-m7-distribution-qualified.json";
const string PriorM3="docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json";
const string M7="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Decision="docs/planning/decisions/post-m7-complete-local-packaging.md";
const string Matrix="docs/distribution/post-m7-complete-package-matrix.md";
const string Validator="docs/planning/ValidatePostM7CompletePackaging.cs";
const string Evidence="docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json";
const string Feed="build/artifacts/rs127-complete-local-feed";
const string M3Project="tests/consumers/m3-gate/m3-gate-consumer.csproj";
const string ExactProject="tests/consumers/exact-math-smoke/exact-math-smoke.csproj";
const string Status="rs127-thirteen-fresh-local-packages-two-independent-consumers-qualified-unpublished";
const string Counts="MATH-RS127:PACKAGES=13;NEW-CONSUMERS=2;M3-PROBES=31;EXACT-SMOKE=PASS;NATIVE=541";
const string Scope="MATH-RS127:SCOPE=M0-M7-UNCHANGED;LOCAL-NUGET=true;PUBLICATION=false;ABI=false";
const string Marker="dw.tools.math/rs127-complete-current-packaging/1.0 qualified; packages=13; consumers=2; m3=31; exact=pass";

static void Need(bool ok,string reason)
{
    if(!ok) throw new InvalidOperationException("RS127 local Math package qualification: "+reason);
}
static string Local(string path)
{
    Need(!string.IsNullOrWhiteSpace(path)&&!Path.IsPathRooted(path)&&
         !path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\')&&!path.Contains(':'),
         "Unexpected or unsafe path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static string Text(JsonElement value,string key)=>value.GetProperty(key).GetString()??
    throw new InvalidOperationException("Missing text field "+key);
static bool Flag(JsonElement value,string key)=>value.GetProperty(key).GetBoolean();
static void Has(string s,string phrase,string owner)=>
    Need(s.Contains(phrase,StringComparison.OrdinalIgnoreCase),
         owner+" missing "+phrase);
static string Property(XDocument doc,string key)=>
    doc.Descendants().FirstOrDefault(x=>x.Name.LocalName==key)?.Value.Trim()??
        throw new InvalidOperationException("Missing csproj property "+key);
static int Count(JsonElement node)
{
    var n=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int Remaining(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Remaining);
    return n;
}
static void CheckClient(string project,string[] references,string configPath,
    string localFeedName,string sourcePath,string passToken,int probes)
{
    var doc=XDocument.Parse(Read(project));
    var declaredFramework=doc.Descendants().FirstOrDefault(x=>
        x.Name.LocalName=="TargetFramework")?.Value.Trim();
    var inheritedFramework=Property(
        XDocument.Parse(Read("Directory.Build.props")),"TargetFramework");
    Need(inheritedFramework=="net10.0"&&
         (declaredFramework??inheritedFramework)=="net10.0"&&
         Property(doc,"ManagePackageVersionsCentrally")=="false"&&
         Property(doc,"IsPackable")=="false"&&
         !doc.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"),
         "An independent consumer gained source references: "+project);
    var packages=doc.Descendants().Where(x=>x.Name.LocalName=="PackageReference")
        .Select(x=>((string?)x.Attribute("Include")??"")+
            "@"+((string?)x.Attribute("Version")??"")).ToArray();
    Need(packages.Length==references.Length&&
         packages.OrderBy(x=>x,StringComparer.Ordinal)
             .SequenceEqual(references.OrderBy(x=>x,StringComparer.Ordinal)),
         "Consumer NuGet dependency IDs or pinned versions drifted: "+project);
    var config=XDocument.Parse(Read(configPath));
    var elements=config.Descendants().Single(x=>x.Name.LocalName=="packageSources")
        .Elements().ToArray();
    Need(elements.Length==2&&elements[0].Name.LocalName=="clear"&&
         elements[1].Name.LocalName=="add"&&
         (((string?)elements[1].Attribute("value"))??"")
             .EndsWith("/"+localFeedName,StringComparison.Ordinal)&&
         !elements[1].Value.Contains("http",StringComparison.OrdinalIgnoreCase),
         "Consumer must have a clear local-only NuGet.Config: "+project);
    var program=Read(sourcePath);
    Has(program,passToken,"Consumer success marker");
    Need(!program.Contains("ProjectReference",StringComparison.Ordinal),
         "Consumer source contains project-reference alias");
    if(probes>0)
        Need(Regex.Matches(program,@"^Probe\(",RegexOptions.Multiline).Count==probes,
             "Consumer declared oracles are not the accepted "+probes);
}
static bool Hex64(string s)=>s.Length==64&&
    s.All(x=>(x>='0'&&x<='9')||(x>='a'&&x<='f'));

Need(args.Length==0||(args.Length==1&&
    (args[0]=="--check"||args[0]=="--packages")),
    "Only read-only --check and --packages are allowed.");
var auditPackages=args.Length==1&&args[0]=="--packages";
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(PriorReport);
using var previousDoc=Open(PriorReceipt);
using var previousM3Doc=Open(PriorM3);
using var m7Doc=Open(M7);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var prior=previousDoc.RootElement;
var priorM3=previousM3Doc.RootElement;
var m7=m7Doc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")=="0.1.122"&&Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106&&
     backlog.GetProperty("releases").EnumerateArray().All(x=>Text(x,"status")=="done"),
     "Accepted M0–M7 roadmap has changed.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Remaining)==0,
     "The 541 accepted DWF nodes must remain done.");
Need(Text(report,"runId")=="RS126"&&report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&!Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: qualify offline Math distribution package graph and API surface"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59"&&
     report.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0&&
     Flag(report.GetProperty("priorInvocationFailures"),"firstPassSucceeded"),
     "RS126 must be a durable first-pass success.");
Need(Text(prior,"status")==
         "rs126-fresh-seven-package-offline-distribution-surface-qualified-unpublished"&&
     prior.GetProperty("packages_packed").GetInt32()==7&&
     prior.GetProperty("source_free_consumers").GetInt32()==1&&
     prior.GetProperty("fresh_consumer_probes").GetInt32()==8&&
     prior.GetProperty("package_results").GetArrayLength()==7&&
     prior.GetProperty("native_nodes_retained").GetInt32()==541&&
     !Flag(prior,"publicly_published")&&!Flag(prior,"cross_version_abi_certified")&&
     !Flag(prior,"external_requests_transmitted"),
     "RS126 historical distribution qualification drifted.");
Need(Text(priorM3,"status")==
         "rs125-two-fresh-isolated-source-free-nuget-consumers-qualified-unpublished"&&
     priorM3.GetProperty("source_free_consumers").GetInt32()==2&&
     priorM3.GetProperty("fresh_consumer_probes").GetInt32()==16&&
     !Flag(priorM3,"publicly_published"),
     "RS125 previous observed clients cannot be recast.");
Need(Text(m7,"status")==
         "m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"&&
     Flag(m7,"m7_internal_scope_done")&&!Flag(m7,"public_release_authorized")&&
     !Flag(m7,"publicly_published")&&
     !Flag(m7,"external_requests_transmitted")&&
     !Flag(m7,"future_program_approved"),
     "M7 owner and publication boundaries changed.");
foreach(var (field,path) in new[]{
    ("decision_sha256","docs/planning/decisions/post-m7-distribution-graph-qualification.md"),
    ("distribution_sha256","docs/distribution/post-m7-seven-package-surface-matrix.md"),
    ("validator_sha256","docs/planning/ValidatePostM7DistributionGraph.cs")
}) Need(Text(prior,field)==Sha(path),
    "Previous source SHA identity has changed: "+path);

var expected=new (string Id,string Version,string[] Dependencies)[]
{
    ("dw.tools.math.foundation","0.1.0-preview.1",[]),
    ("dw.quantities","0.2.0-preview.1",[]),
    ("dw.quantities.standard","0.2.0-preview.1",
        ["dw.quantities","dw.quantities.expression"]),
    ("dw.quantities.expression","0.2.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.composition","0.3.0-preview.1",
        ["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4",["dw.tools.math.ir"]),
    ("dw.tools.math.statistics","0.4.0-preview.6",["dw.quantities"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10",["dw.tools.math.ir"]),
    ("dw.tools.math.codecs","1.0.0-preview.8",["dw.tools.math.ir"]),
    ("dw.tools.math.distribution","1.0.0-preview.4",
        ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
         "dw.tools.math.symbolics","dw.tools.math.numerics","dw.tools.math.composition"]),
    ("dw.tools.math.analysis","1.1.0-preview.6",[]),
    ("dw.tools.math.scientific","1.1.0-preview.14",[])
};
var solution=XDocument.Parse(Read("dw.tools.math.slnx"));
var listed=solution.Descendants().Where(x=>x.Name.LocalName=="Project")
    .Select(x=>(string?)x.Attribute("Path")??"").ToArray();
Need(listed.Length==24&&listed.Count(p=>p.StartsWith(
        "src/projects/",StringComparison.Ordinal))==13&&
     listed.Count(p=>p.StartsWith("tests/projects/",StringComparison.Ordinal))==11&&
     listed.Distinct(StringComparer.OrdinalIgnoreCase).Count()==24,
     "Expected thirteen libraries and eleven test projects.");
foreach(var (id,version,dependencies) in expected)
{
    var path="src/projects/"+id+"/"+id+".csproj";
    Need(listed.Contains(path,StringComparer.Ordinal)&&File.Exists(Local(path)),
         "Packable source is missing from solution: "+id);
    var csproj=XDocument.Parse(Read(path));
    Need(Property(csproj,"PackageId")==id&&
         Property(csproj,"Version")==version&&
         Property(csproj,"IsPackable")=="true"&&
         Property(csproj,"RestorePackagesWithLockFile")=="true",
         "Source package metadata changed: "+id);
    var found=csproj.Descendants().Where(x=>x.Name.LocalName=="ProjectReference")
        .Select(x=>Path.GetFileNameWithoutExtension(
            (string?)x.Attribute("Include")??"")).ToArray();
    Need(found.Length==dependencies.Length&&
         found.OrderBy(x=>x,StringComparer.Ordinal)
             .SequenceEqual(dependencies.OrderBy(x=>x,StringComparer.Ordinal)),
         "Unexpected direct package dependencies: "+id);
}
var m3Program="tests/consumers/m3-gate/Program.cs";
var exactProgram="tests/consumers/exact-math-smoke/Program.cs";
CheckClient(M3Project,
    ["dw.tools.math.numerics@[0.4.0-preview.4]",
     "dw.tools.math.statistics@[0.4.0-preview.6]"],
    "tests/consumers/m3-gate/NuGet.Config","m3-gate-local-feed",m3Program,
    "dw.tools.math/m3-nuget-gate/0.4 qualified; probes=",31);
CheckClient(ExactProject,
    ["dw.quantities@[0.2.0-preview.1]",
     "dw.quantities.expression@[0.2.0-preview.1]",
     "dw.quantities.standard@[0.2.0-preview.1]"],
    "tests/consumers/exact-math-smoke/NuGet.Config",
    "math-package-feed",exactProgram,
    "dw.tools.math/exact-consumer/0.2 qualified",0);
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","Unsent owner coordination");
Need(!Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "Generated Math discovery skill is not part of accepted delivery.");
var decision=Read(Decision);
foreach(var term in new[]{"## Owner-selected scope",
    "## Source-controlled acceptance inventory",
    "## DWF-controlled reproducibility",
    "## Technical meaning and next choice",
    "foundation","31","without publication"})
    Has(decision,term,"RS127 decision");
var matrix=Read(Matrix);
foreach(var term in new[]{"## Complete package graph",
    "## Fresh source-free consumers",
    "## Qualification lineage and pending gaps",
    "31","13","source-free","No Python"})
    Has(matrix,term,"RS127 package inventory");

if(auditPackages)
{
    var folder=Local(Feed);
    Need(Directory.Exists(folder),"RS127 complete local feed was not created.");
    var actual=Directory.GetFiles(folder,"*.nupkg",SearchOption.TopDirectoryOnly);
    Need(actual.Length==13,"Local package feed must contain exactly thirteen archives.");
    foreach(var (id,version,dependencies) in expected)
    {
        var relative=Feed+"/"+id+"."+version+".nupkg";
        var full=Local(relative);
        Need(File.Exists(full),"Local archive absent: "+id);
        var bytes=new FileInfo(full).Length;
        Need(bytes>0&&bytes<=4_194_304,"Archive exceeds bounded size: "+id);
        using(var input=File.OpenRead(full))
        using(var archive=new ZipArchive(input,ZipArchiveMode.Read))
        {
            Need(archive.Entries.Count<=256&&
                 archive.Entries.Sum(e=>e.Length)<=16_777_216,
                 "Archive ZIP budget exceeded: "+id);
            Need(archive.Entries.Any(e=>e.FullName.Equals(
                     "lib/net10.0/"+id+".dll",StringComparison.OrdinalIgnoreCase)&&
                     e.Length>0),
                 "Expected net10 managed DLL missing: "+id);
            var entries=archive.Entries.Where(e=>e.FullName.EndsWith(
                ".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
            Need(entries.Length==1,"Ambiguous package nuspec: "+id);
            using var reader=new StreamReader(entries[0].Open());
            var metadata=XDocument.Parse(reader.ReadToEnd()).Descendants()
                .Single(e=>e.Name.LocalName=="metadata");
            Need(metadata.Elements().Single(e=>e.Name.LocalName=="id").Value==id&&
                 metadata.Elements().Single(e=>e.Name.LocalName=="version").Value==version,
                 "Package identity mismatch in nuspec: "+id);
            var deps=metadata.Descendants().Where(e=>
                e.Name.LocalName=="dependency").ToArray();
            Need(deps.Length==dependencies.Length&&
                 deps.Select(e=>(string?)e.Attribute("id")??"")
                     .OrderBy(s=>s,StringComparer.Ordinal)
                     .SequenceEqual(dependencies.OrderBy(s=>s,StringComparer.Ordinal)),
                 "NuGet direct package dependency IDs drifted: "+id);
            foreach(var dep in deps)
            {
                var required=expected.Single(e=>e.Id==
                    ((string?)dep.Attribute("id")??""));
                Need(((string?)dep.Attribute("version")??"")
                    .Contains(required.Version,StringComparison.Ordinal),
                    "NuGet dependency version changed: "+id);
            }
            if(id=="dw.tools.math.distribution")
                Need(archive.Entries.Any(e=>e.FullName=="readme.md"&&e.Length>0)&&
                     archive.Entries.Any(e=>e.FullName=="NOTICE.md"&&e.Length>0),
                     "Distribution package has no actual notice/readme.");
        }
        Console.WriteLine("RS127-PACK|"+id+"|"+version+"|"+bytes+"|"+Sha(relative));
    }
}
if(target)
{
    using var evidenceDoc=Open(Evidence);
    var receipt=evidenceDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("source_projects").GetInt32()==13&&
         receipt.GetProperty("test_projects").GetInt32()==11&&
         receipt.GetProperty("packages_packed").GetInt32()==13&&
         receipt.GetProperty("source_free_consumers").GetInt32()==2&&
         receipt.GetProperty("m3_fresh_probes").GetInt32()==31&&
         Flag(receipt,"exact_quantity_smoke_passed")&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         receipt.GetProperty("command_receipts").GetArrayLength()==20&&
         receipt.GetProperty("package_results").GetArrayLength()==13&&
         receipt.GetProperty("consumer_results").GetArrayLength()==2&&
         Flag(receipt,"offline_feed_only")&&
         Flag(receipt,"isolated_package_cache")&&
         !Flag(receipt,"foundation_source_free_consumer_rerun")&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"cross_version_abi_certified")&&
         !Flag(receipt,"other_os_rids_certified")&&
         !Flag(receipt,"discovery_skill_generated")&&
         !Flag(receipt,"all_historical_consumers_rerun")&&
         !Flag(receipt,"m0_m7_plan_mutated")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Matrix)&&
         Text(receipt,"validator_sha256")==Sha(Validator),
         "Target receipt lacks exact current packaging and nonpublication proof.");
    var expectedIds=new[]{
        "source-locked-restore","solution-release-build",
        "pack-dw.tools.math.foundation","pack-dw.quantities",
        "pack-dw.quantities.standard","pack-dw.quantities.expression",
        "pack-dw.tools.math.ir","pack-dw.tools.math.composition",
        "pack-dw.tools.math.numerics","pack-dw.tools.math.statistics",
        "pack-dw.tools.math.symbolics","pack-dw.tools.math.codecs",
        "pack-dw.tools.math.distribution","pack-dw.tools.math.analysis",
        "pack-dw.tools.math.scientific","independent-package-audit",
        "m3-offline-restore","m3-source-free-consumer",
        "exact-offline-restore","exact-source-free-consumer"
    };
    var steps=receipt.GetProperty("command_receipts").EnumerateArray().ToArray();
    for(var i=0;i<expectedIds.Length;i++)
        Need(Text(steps[i],"id")==expectedIds[i]&&
             steps[i].GetProperty("exit_code").GetInt32()==0,
             "A required local command failed or is absent: "+expectedIds[i]);
    var products=receipt.GetProperty("package_results").EnumerateArray().ToArray();
    for(var i=0;i<expected.Length;i++)
        Need(Text(products[i],"package_id")==expected[i].Id&&
             Text(products[i],"version")==expected[i].Version&&
             products[i].GetProperty("archive_bytes").GetInt64()>0&&
             Hex64(Text(products[i],"archive_sha256")),
             "Archive result is missing or malformed: "+expected[i].Id);
    var clientResults=receipt.GetProperty("consumer_results").EnumerateArray().ToArray();
    Need(Text(clientResults[0],"id")=="m3"&&
         clientResults[0].GetProperty("probes").GetInt32()==31&&
         Flag(clientResults[0],"source_free")&&
         Text(clientResults[1],"id")=="exact"&&
         Flag(clientResults[1],"passed")&&
         Flag(clientResults[1],"source_free"),
         "Independent consumer pass markers are absent.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
