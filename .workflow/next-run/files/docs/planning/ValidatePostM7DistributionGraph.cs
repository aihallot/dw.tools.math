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
const string PreviousReport=".aura/workflow/reports/RS125/attempt-001.json";
const string PreviousReceipt="docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json";
const string PreviousDelivery="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
const string M7="docs/planning/evidence/M7-internal-gate-qualified.json";
const string M5="docs/planning/evidence/M5-gate.json";
const string ConsumerProject="tests/consumers/m5-w02-c02-t2/m5-managed-api-boundary-consumer.csproj";
const string ConsumerProgram="tests/consumers/m5-w02-c02-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w02-c02-t2/NuGet.Config";
const string Decision="docs/planning/decisions/post-m7-distribution-graph-qualification.md";
const string Matrix="docs/distribution/post-m7-seven-package-surface-matrix.md";
const string Evidence="docs/planning/evidence/RS126-post-m7-distribution-qualified.json";
const string Feed="build/artifacts/m5-w02-c02-t2-local-feed";
const string Status="rs126-fresh-seven-package-offline-distribution-surface-qualified-unpublished";
const string Counts="MATH-RS126:PACKAGES=7;SOURCE-FREE-CONSUMERS=1;FRESH-PROBES=8;NATIVE=541";
const string Scope="MATH-RS126:SCOPE=NO-PROGRESS;LOCAL-NUGET=true;API-SHA=OBSERVED-ONLY;PUBLICATION=false";
const string Marker="dw.tools.math/rs126-multipackage-surface/1.0 qualified; archive-graph=7; probes=8; abi-certified=false";

static void Need(bool yes,string problem)
{
    if(!yes) throw new InvalidOperationException("RS126 distribution qualification: "+problem);
}
static string Local(string relative)
{
    Need(!string.IsNullOrWhiteSpace(relative)&&!Path.IsPathRooted(relative)&&
         !relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains(':')&&!relative.Contains('\\'),
         "Unsafe source path: "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(Local(path)))).ToLowerInvariant();
static string Text(JsonElement value,string key)=>
    value.GetProperty(key).GetString() ??
        throw new InvalidOperationException("Missing text: "+key);
static bool Flag(JsonElement value,string key)=>value.GetProperty(key).GetBoolean();
static void Has(string contents,string token,string source)=>
    Need(contents.Contains(token,StringComparison.OrdinalIgnoreCase),
        source+" lacks "+token);
static string XmlText(XDocument xml,string key)=>
    xml.Descendants().FirstOrDefault(x=>x.Name.LocalName==key)?.Value.Trim()??
        throw new InvalidOperationException("Missing XML element "+key);
static int Count(JsonElement node)
{
    var count=1;
    foreach(var field in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(field,out var children))
            count+=children.EnumerateArray().Sum(Count);
    return count;
}
static int Unfinished(JsonElement node)
{
    var incomplete=Text(node,"state")=="done"?0:1;
    foreach(var field in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(field,out var children))
            incomplete+=children.EnumerateArray().Sum(Unfinished);
    return incomplete;
}

Need(args.Length==0||(args.Length==1&&
     (args[0]=="--check"||args[0]=="--packages")),
     "Use only read-only --check or --packages.");
var packageMode=args.Length==1&&args[0]=="--packages";
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(PreviousReport);
using var previousDoc=Open(PreviousReceipt);
using var deliveryDoc=Open(PreviousDelivery);
using var m7Doc=Open(M7);
using var m5Doc=Open(M5);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var previous=previousDoc.RootElement;
var delivery=deliveryDoc.RootElement;
var m7=m7Doc.RootElement;
var m5=m5Doc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Text(backlog,"plan_version")=="0.1.122"&&
     Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106,
     "The accepted M0-M7 backlog cannot change during qualification.");
Need(backlog.GetProperty("releases").EnumerateArray().All(x=>
         Text(x,"status")=="done")&&
     backlog.GetProperty("work_packages").EnumerateArray().All(x=>
         Text(x,"status")=="done")&&
     backlog.GetProperty("chunks").EnumerateArray().All(x=>
         Text(x,"status")=="done"),
     "An accepted product progress node was reopened.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Unfinished)==0,
     "The completed 541-node DWF plan drifted.");
Need(Text(report,"runId")=="RS125"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: qualify fresh offline Math NuGet consumers without publication"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59"&&
     report.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0&&
     Flag(report.GetProperty("priorInvocationFailures"),"firstPassSucceeded"),
     "RS125 must be durable first-pass success, not invented lineage.");
Need(Text(previous,"status")==
         "rs125-two-fresh-isolated-source-free-nuget-consumers-qualified-unpublished"&&
     previous.GetProperty("source_free_consumers").GetInt32()==2&&
     previous.GetProperty("fresh_consumer_probes").GetInt32()==16&&
     previous.GetProperty("fresh_packages_built").GetInt32()==2&&
     previous.GetProperty("native_nodes_retained").GetInt32()==541&&
     Flag(previous,"offline_local_feeds_only")&&
     Flag(previous,"isolated_nuget_package_cache")&&
     !Flag(previous,"publicly_published")&&
     !Flag(previous,"cross_version_abi_certified")&&
     !Flag(previous,"external_requests_transmitted")&&
     !Flag(previous,"discovery_skill_generated"),
     "Prior qualified source-free consumer receipt was promoted or changed.");
foreach(var (field,path) in new[]{
    ("decision_sha256","docs/planning/decisions/post-m7-source-free-consumer-qualification.md"),
    ("distribution_sha256","docs/distribution/post-m7-source-free-consumer-matrix.md"),
    ("validator_sha256","docs/planning/ValidatePostM7ConsumerIsolation.cs")
}) Need(Text(previous,field)==Sha(path),
     "Prior source-pinned qualification material drifted: "+path);
Need(Text(delivery,"status")==
         "rs124-current-source-build-test-and-three-local-packages-qualified-unpublished"&&
     Flag(delivery,"full_solution_tests_passed")&&
     !Flag(delivery,"publicly_published"),
     "RS124 source/suite qualification cannot be promoted to public release.");
Need(Text(m7,"status")==
         "m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"&&
     Flag(m7,"m7_internal_scope_done")&&
     !Flag(m7,"public_release_authorized")&&
     !Flag(m7,"publicly_published")&&
     !Flag(m7,"external_requests_transmitted")&&
     !Flag(m7,"future_program_approved"),
     "Research completion does not authorize publication or external adoption.");
Need(Text(m5,"status")==
         "m5-local-v1-independent-nuget-evidence-gate-qualified-unpublished"&&
     m5.GetProperty("seven_local_nuget_packages_inspected").GetInt32()==7&&
     !Flag(m5,"packages_publicly_published"),
     "Historical local M5 package graph authority changed.");

var expected=new (string Id,string Version,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.codecs","1.0.0-preview.8",["dw.tools.math.ir"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10",["dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4",["dw.tools.math.ir"]),
    ("dw.tools.math.composition","0.3.0-preview.1",
        ["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.distribution","1.0.0-preview.4",
        ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
         "dw.tools.math.symbolics","dw.tools.math.numerics",
         "dw.tools.math.composition"])
};
var solution=XDocument.Parse(Read("dw.tools.math.slnx"));
var installed=solution.Descendants().Where(x=>x.Name.LocalName=="Project")
    .Select(x=>(string?)x.Attribute("Path")??"")
    .ToHashSet(StringComparer.Ordinal);
foreach(var (id,version,dependencies) in expected)
{
    var path="src/projects/"+id+"/"+id+".csproj";
    Need(installed.Contains(path)&&File.Exists(Local(path)),
        "Declared package project is missing from the solution: "+id);
    var xml=XDocument.Parse(Read(path));
    Need(XmlText(xml,"PackageId")==id&&XmlText(xml,"Version")==version&&
         XmlText(xml,"IsPackable")=="true"&&
         XmlText(xml,"RestorePackagesWithLockFile")=="true",
         "Local package identifier, version or restore contract changed: "+id);
    var found=xml.Descendants().Where(x=>x.Name.LocalName=="ProjectReference")
        .Select(x=>(string?)x.Attribute("Include")??"")
        .Select(Path.GetFileNameWithoutExtension).ToArray();
    Need(found.Length==dependencies.Length&&
         found.OrderBy(x=>x,StringComparer.Ordinal)
             .SequenceEqual(dependencies.OrderBy(x=>x,StringComparer.Ordinal)),
         "Current dependency closure is not the accepted seven-package graph: "+id);
}
Has(Read("src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs"),
    "public static ImmutableArray<MathPackageIdentity> ExpectedPackages",
    "Distribution package source contract");
Has(Read("src/projects/dw.tools.math.distribution/MathPackageRuntimeSurfaceAudit.cs"),
    "CrossVersionBinaryCompatibilityCertified",
    "Runtime API audit rejects ABI overclaim");
var consumer=XDocument.Parse(Read(ConsumerProject));
var packageRefs=consumer.Descendants().Where(x=>
    x.Name.LocalName=="PackageReference").ToArray();
Need(packageRefs.Length==1&&
     (string?)packageRefs[0].Attribute("Include")=="dw.tools.math.distribution"&&
     (string?)packageRefs[0].Attribute("Version")=="[1.0.0-preview.4]"&&
     !consumer.Descendants().Any(x=>x.Name.LocalName=="ProjectReference")&&
     XmlText(consumer,"ManagePackageVersionsCentrally")=="false"&&
     XmlText(consumer,"IsPackable")=="false"&&
     XmlText(consumer,"TargetFramework")=="net10.0",
     "M5 package boundary consumer is not source-free and exact-versioned.");
var nuget=XDocument.Parse(Read(ConsumerConfig));
var sources=nuget.Descendants().Single(x=>x.Name.LocalName=="packageSources");
Need(sources.Elements().Count()==1&&
     sources.Elements().Single().Name.LocalName=="clear",
     "M5 NuGet consumer must exclude all inherited remote feeds.");
var sample=Read(ConsumerProgram);
Need(Regex.Matches(sample,@"^Probe\(",RegexOptions.Multiline).Count==8,
     "M5 source-free surface client needs exactly eight independent probes.");
foreach(var term in new[]{
    "m5-w02-c02-t2-local-feed",
    "M5-W02-C02-T2-SURFACE-SHA256:",
    "dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes=",
    "MathPackageRuntimeSurfaceAudit.Audit",
    "MathLocalReleaseQualification.ExpectedPackages",
    "CrossVersionBinaryCompatibilityCertified"
}) Has(sample,term,"M5 independent source-free API consumer");
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","Unsent owner coordination");
Need(!Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "Generated Math agent skill is outside RS126 accepted scope.");
var decision=Read(Decision);
foreach(var term in new[]{
    "## Question and accepted scope","## Verified source and historical evidence",
    "## Controlled evidence chain","## Decision boundary and follow-up",
    "without publication","seven-package","RS125"
}) Has(decision,term,"RS126 qualification decision");
var matrix=Read(Matrix);
foreach(var term in new[]{
    "## Closed dependency and package roster",
    "## Independent current consumer",
    "## Historical versus fresh and release restrictions",
    "No GitHub Actions","API surface","probes=8"
}) Has(matrix,term,"RS126 distribution matrix");

if(packageMode)
{
    var archiveDirectory=Local(Feed);
    Need(Directory.Exists(archiveDirectory),"Local seven-package feed is absent.");
    var archives=Directory.GetFiles(archiveDirectory,"*.nupkg",
        SearchOption.TopDirectoryOnly);
    Need(archives.Length==expected.Length,
         "Exactly seven newly packed local NuGet archives are required.");
    foreach(var (id,version,dependencies) in expected)
    {
        var relative=Feed+"/"+id+"."+version+".nupkg";
        var full=Local(relative);
        Need(File.Exists(full),"Missing newly packed local archive: "+id);
        var length=new FileInfo(full).Length;
        Need(length>0&&length<=4_194_304,
             "NuGet archive violates bounded size rule: "+id);
        using(var file=File.OpenRead(full))
        using(var zip=new ZipArchive(file,ZipArchiveMode.Read))
        {
            Need(zip.Entries.Count<=256&&
                 zip.Entries.Sum(x=>x.Length)<=16_777_216,
                 "Archive exceeds the accepted ZIP bounds: "+id);
            Need(zip.Entries.Any(x=>x.FullName.Equals(
                     "lib/net10.0/"+id+".dll",StringComparison.OrdinalIgnoreCase)&&
                     x.Length>0),
                 "Compiled net10 product assembly absent: "+id);
            var nuspec=zip.Entries.Where(x=>x.FullName.EndsWith(
                ".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
            Need(nuspec.Length==1,"NuGet spec is absent or ambiguous: "+id);
            using var reader=new StreamReader(nuspec[0].Open());
            var metadata=XDocument.Parse(reader.ReadToEnd()).Descendants()
                .Single(x=>x.Name.LocalName=="metadata");
            var actualId=metadata.Elements().Single(x=>
                x.Name.LocalName=="id").Value;
            var actualVersion=metadata.Elements().Single(x=>
                x.Name.LocalName=="version").Value;
            var actualDeps=metadata.Descendants().Where(x=>
                x.Name.LocalName=="dependency").ToArray();
            Need(actualId==id&&actualVersion==version&&
                 actualDeps.Length==dependencies.Length&&
                 actualDeps.Select(x=>(string?)x.Attribute("id")??"")
                     .OrderBy(x=>x,StringComparer.Ordinal)
                     .SequenceEqual(dependencies.OrderBy(x=>x,StringComparer.Ordinal)),
                 "NuGet package metadata or direct dependencies drifted: "+id);
            foreach(var dep in actualDeps)
            {
                var depId=(string?)dep.Attribute("id")??"";
                var accepted=expected.Single(x=>x.Id==depId);
                Need(((string?)dep.Attribute("version")??"")
                         .Contains(accepted.Version,StringComparison.Ordinal),
                     "NuGet dependency version requirement differs: "+id);
            }
            if(id=="dw.tools.math.distribution")
                Need(zip.Entries.Any(x=>x.FullName=="readme.md"&&x.Length>0)&&
                     zip.Entries.Any(x=>x.FullName=="NOTICE.md"&&x.Length>0),
                     "Distribution package must carry actual readme and notice.");
        }
        var digest=Sha(relative);
        Need(digest.Length==64,"Archive SHA-256 is malformed: "+id);
        Console.WriteLine("RS126-PACK|"+id+"|"+version+"|"+length+"|"+digest);
    }
}
if(target)
{
    using var receiptDoc=Open(Evidence);
    var receipt=receiptDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("packages_packed").GetInt32()==7&&
         receipt.GetProperty("source_free_consumers").GetInt32()==1&&
         receipt.GetProperty("fresh_consumer_probes").GetInt32()==8&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         receipt.GetProperty("command_receipts").GetArrayLength()==12&&
         receipt.GetProperty("package_results").GetArrayLength()==7&&
         Flag(receipt,"offline_local_feed_only")&&
         Flag(receipt,"isolated_nuget_package_cache")&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"discovery_skill_generated")&&
         !Flag(receipt,"cross_version_abi_certified")&&
         !Flag(receipt,"other_os_rids_certified")&&
         !Flag(receipt,"all_historical_consumers_rerun")&&
         !Flag(receipt,"m0_m7_plan_mutated")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Matrix)&&
         Text(receipt,"validator_sha256")==Sha(
             "docs/planning/ValidatePostM7DistributionGraph.cs"),
         "Target success receipt overclaims public/ABI or lacks real consumers.");
    var expectedCommands=new[]{
        "source-locked-restore","distribution-release-build",
        "pack-dw.quantities","pack-dw.tools.math.ir",
        "pack-dw.tools.math.codecs","pack-dw.tools.math.symbolics",
        "pack-dw.tools.math.numerics","pack-dw.tools.math.composition",
        "pack-dw.tools.math.distribution","independent-package-audit",
        "distribution-offline-restore","distribution-source-free-consumer"
    };
    var observedCommands=receipt.GetProperty("command_receipts")
        .EnumerateArray().ToArray();
    for(var i=0;i<expectedCommands.Length;i++)
        Need(Text(observedCommands[i],"id")==expectedCommands[i]&&
             observedCommands[i].GetProperty("exit_code").GetInt32()==0,
             "Local qualification command missing or failed: "+expectedCommands[i]);
    var observedPackages=receipt.GetProperty("package_results")
        .EnumerateArray().ToArray();
    for(var i=0;i<expected.Length;i++)
    {
        var current=observedPackages[i];
        Need(Text(current,"package_id")==expected[i].Id&&
             Text(current,"version")==expected[i].Version&&
             current.GetProperty("archive_bytes").GetInt64()>0&&
             current.GetProperty("archive_bytes").GetInt64()<=4_194_304&&
             Text(current,"archive_sha256").Length==64&&
             Text(current,"public_api_sha256").Length==64&&
             Text(current,"managed_assembly_sha256").Length==64&&
             new[]{"archive_sha256","public_api_sha256","managed_assembly_sha256"}
                .All(key=>Text(current,key).All(ch=>
                    (ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))),
             "Recorded package, version or SHA is malformed: "+expected[i].Id);
    }
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
