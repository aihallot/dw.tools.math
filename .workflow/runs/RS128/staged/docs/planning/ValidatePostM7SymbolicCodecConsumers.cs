#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string PriorReport=".aura/workflow/reports/RS127/attempt-001.json";
const string PriorReceipt="docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json";
const string M7="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Decision="docs/planning/decisions/post-m7-symbolic-codec-consumers.md";
const string Matrix="docs/distribution/post-m7-symbolic-codec-consumer-matrix.md";
const string Validator="docs/planning/ValidatePostM7SymbolicCodecConsumers.cs";
const string Evidence="docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json";
const string Feed="build/artifacts/rs128-symbolic-codec-feed";
const string M4Project="tests/consumers/m4-gate/m4-gate-consumer.csproj";
const string M5Project="tests/consumers/m5-w01-c04-t2/m5-math-display-boundary-consumer.csproj";
const string Status="rs128-four-local-symbolic-codec-packages-two-source-free-consumers-qualified-unpublished";
const string Counts="MATH-RS128:PACKAGES=4;CONSUMERS=2;M4-PROBES=32;M5-PROBES=8;NATIVE=541";
const string Scope="MATH-RS128:SCOPE=NO-PROGRESS;LOCAL-FEED=true;EXTERNAL=false;PUBLICATION=false;ABI=false";
const string Marker="dw.tools.math/rs128-symbolic-codec-consumers/1.0 qualified; packages=4; consumers=2; probes=40; public=false";

static void Need(bool ok,string reason)
{
    if(!ok) throw new InvalidOperationException("RS128 symbolics/codecs: "+reason);
}
static string Local(string path)
{
    Need(!string.IsNullOrWhiteSpace(path)&&!Path.IsPathRooted(path)&&
         !path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\')&&!path.Contains(':'),
         "Unsafe source path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(Local(path)))).ToLowerInvariant();
static string Text(JsonElement v,string name)=>v.GetProperty(name).GetString()??
    throw new InvalidOperationException("Missing string property "+name);
static bool Flag(JsonElement v,string name)=>v.GetProperty(name).GetBoolean();
static string Property(XDocument doc,string name)=>
    doc.Descendants().FirstOrDefault(x=>x.Name.LocalName==name)?.Value.Trim()??
    throw new InvalidOperationException("Missing project property "+name);
static void Has(string s,string marker,string owner)=>
    Need(s.Contains(marker,StringComparison.OrdinalIgnoreCase),
         owner+" is missing "+marker);
static bool Hex64(string s)=>s.Length==64&&s.All(c=>
    (c>='0'&&c<='9')||(c>='a'&&c<='f'));
static int Count(JsonElement node)
{
    var n=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var array))n+=array.EnumerateArray().Sum(Count);
    return n;
}
static int Remaining(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var array))n+=array.EnumerateArray().Sum(Remaining);
    return n;
}
static void CheckConsumer(string project,string id,string version,
    string programPath,int probes,string successPrefix)
{
    var xml=XDocument.Parse(Read(project));
    var declared=xml.Descendants().FirstOrDefault(x=>
        x.Name.LocalName=="TargetFramework")?.Value.Trim();
    var inherited=Property(XDocument.Parse(Read("Directory.Build.props")),
        "TargetFramework");
    Need(inherited=="net10.0"&&(declared??inherited)=="net10.0"&&
         Property(xml,"ManagePackageVersionsCentrally")=="false"&&
         Property(xml,"IsPackable")=="false"&&
         !xml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"),
         "Consumer must be net10, pack-disabled, source-free: "+project);
    var refs=xml.Descendants().Where(x=>x.Name.LocalName=="PackageReference")
        .ToArray();
    Need(refs.Length==1&&
         (string?)refs[0].Attribute("Include")==id&&
         (string?)refs[0].Attribute("Version")=="["+version+"]",
         "Consumer pinned exact NuGet PackageReference drifted: "+project);
    var config=project[..project.LastIndexOf('/')]+"/NuGet.Config";
    var sources=XDocument.Parse(Read(config)).Descendants()
        .Single(x=>x.Name.LocalName=="packageSources");
    Need(sources.Elements().Count()==1&&
         sources.Elements().Single().Name.LocalName=="clear",
         "Consumer NuGet.Config must contain clear-only local feed policy: "+project);
    var code=Read(programPath);
    Need(Regex.Matches(code,@"^Probe\(",RegexOptions.Multiline).Count==probes&&
         !code.Contains("ProjectReference",StringComparison.Ordinal),
         "Consumer has incorrect independent oracle count: "+project);
    Has(code,successPrefix,"Consumer source marker");
}
Need(args.Length==0||(args.Length==1&&
     (args[0]=="--check"||args[0]=="--packages")),
     "Read-only --check or --packages required.");
var packageMode=args.Length==1&&args[0]=="--packages";
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(PriorReport);
using var receiptDoc=Open(PriorReceipt);
using var m7Doc=Open(M7);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var previous=receiptDoc.RootElement;
var m7=m7Doc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Text(backlog,"plan_version")=="0.1.122"&&
     Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106&&
     backlog.GetProperty("releases").EnumerateArray().All(x=>Text(x,"status")=="done"),
     "Accepted Math backlog has been altered.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Remaining)==0,
     "Previously completed 541 DWF nodes must remain done.");
Need(Text(report,"runId")=="RS127"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: qualify full local Math package inventory and isolated clients"&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59",
     "Prior RS127 run must be a durable successful attempt 001.");
var failures=report.GetProperty("priorInvocationFailures");
Need(failures.GetProperty("count").GetInt32()==2&&
     failures.GetProperty("preparationFailureCount").GetInt32()==2&&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32()==0&&
     !Flag(failures,"firstPassSucceeded"),
     "Do not relabel RS127 first-pass or its two prior preparation refusals.");
Need(Text(previous,"status")==
         "rs127-thirteen-fresh-local-packages-two-independent-consumers-qualified-unpublished"&&
     previous.GetProperty("packages_packed").GetInt32()==13&&
     previous.GetProperty("source_free_consumers").GetInt32()==2&&
     previous.GetProperty("m3_fresh_probes").GetInt32()==31&&
     Flag(previous,"exact_quantity_smoke_passed")&&
     previous.GetProperty("native_nodes_retained").GetInt32()==541&&
     !Flag(previous,"publicly_published")&&
     !Flag(previous,"cross_version_abi_certified")&&
     !Flag(previous,"external_requests_transmitted")&&
     !Flag(previous,"discovery_skill_generated"),
     "RS127 local package/consumer evidence is not a published stable release.");
foreach(var (field,document) in new[]{
    ("decision_sha256","docs/planning/decisions/post-m7-complete-local-packaging.md"),
    ("distribution_sha256","docs/distribution/post-m7-complete-package-matrix.md"),
    ("validator_sha256","docs/planning/ValidatePostM7CompletePackaging.cs")
}) Need(Text(previous,field)==Sha(document),
    "Prior RS127 file source identity drifted: "+document);
Need(Text(m7,"status")==
         "m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"&&
     Flag(m7,"m7_internal_scope_done")&&
     !Flag(m7,"public_release_authorized")&&
     !Flag(m7,"publicly_published")&&
     !Flag(m7,"external_requests_transmitted")&&
     !Flag(m7,"future_program_approved"),
     "Historical internal scientific gate was promoted to public authorization.");
var selected=new (string Id,string Version,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10",["dw.tools.math.ir"]),
    ("dw.tools.math.codecs","1.0.0-preview.8",["dw.tools.math.ir"])
};
var solution=XDocument.Parse(Read("dw.tools.math.slnx"));
var known=solution.Descendants().Where(x=>x.Name.LocalName=="Project")
    .Select(x=>(string?)x.Attribute("Path")??"")
    .ToHashSet(StringComparer.Ordinal);
foreach(var (id,version,deps) in selected)
{
    var source="src/projects/"+id+"/"+id+".csproj";
    Need(known.Contains(source)&&File.Exists(Local(source)),
         "Product source missing from solution: "+id);
    var xml=XDocument.Parse(Read(source));
    Need(Property(xml,"PackageId")==id&&
         Property(xml,"Version")==version&&
         Property(xml,"IsPackable")=="true"&&
         Property(xml,"RestorePackagesWithLockFile")=="true",
         "Versioned local source package identity changed: "+id);
    var references=xml.Descendants().Where(x=>x.Name.LocalName=="ProjectReference")
        .Select(x=>Path.GetFileNameWithoutExtension(
             (string?)x.Attribute("Include")??"")).ToArray();
    Need(references.Length==deps.Length&&
         references.OrderBy(x=>x,StringComparer.Ordinal)
             .SequenceEqual(deps.OrderBy(x=>x,StringComparer.Ordinal)),
         "Current direct local package dependencies changed: "+id);
}
CheckConsumer(M4Project,"dw.tools.math.symbolics","0.5.0-preview.10",
    "tests/consumers/m4-gate/Program.cs",32,
    "dw.tools.math/m4-nuget-gate/0.5 qualified; probes=");
CheckConsumer(M5Project,"dw.tools.math.codecs","1.0.0-preview.8",
    "tests/consumers/m5-w01-c04-t2/Program.cs",8,
    "dw.tools.math/m5-markdown-omml-boundary-consumer/1.0 qualified; probes=");
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","Unsent recipient drafts");
Need(!Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "No generated agent skill was admitted.");
foreach(var term in new[]{
    "## Authorization and purpose","## Verified input and exact science scope",
    "## One bounded DWF qualification chain","## Decision and remaining debts",
    "40","32","8","without publication"
}) Has(Read(Decision),term,"RS128 scientific scope decision");
foreach(var term in new[]{
    "## Four-package closed dependency graph",
    "## Two independent existing consumers",
    "## Scientific nonclaims and historical continuity",
    "probes=32","probes=8","No Python"
}) Has(Read(Matrix),term,"RS128 source-free matrix");
if(packageMode)
{
    var folder=Local(Feed);
    Need(Directory.Exists(folder),"Fresh four-package offline feed is missing.");
    Need(Directory.GetFiles(folder,"*.nupkg",SearchOption.TopDirectoryOnly).Length==4,
        "Exactly four new local preview archives are required.");
    foreach(var (id,version,deps) in selected)
    {
        var path=Feed+"/"+id+"."+version+".nupkg";
        Need(File.Exists(Local(path)),"Fresh archive missing: "+id);
        var bytes=new FileInfo(Local(path)).Length;
        Need(bytes>0&&bytes<=4_194_304,"Current package archive too large: "+id);
        using(var file=File.OpenRead(Local(path)))
        using(var zip=new ZipArchive(file,ZipArchiveMode.Read))
        {
            Need(zip.Entries.Count<=256&&
                 zip.Entries.Sum(x=>x.Length)<=16_777_216,
                 "ZIP archive is not bounded: "+id);
            Need(zip.Entries.Any(x=>x.FullName.Equals(
                     "lib/net10.0/"+id+".dll",StringComparison.OrdinalIgnoreCase)&&
                     x.Length>0),
                 "Current net10 assembly missing in local NuGet: "+id);
            var specs=zip.Entries.Where(x=>x.FullName.EndsWith(
                ".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
            Need(specs.Length==1,"Local nuspec missing/ambiguous: "+id);
            using var reader=new StreamReader(specs[0].Open());
            var metadata=XDocument.Parse(reader.ReadToEnd()).Descendants()
                .Single(x=>x.Name.LocalName=="metadata");
            var metaId=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
            var metaVersion=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
            var found=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency").ToArray();
            Need(metaId==id&&metaVersion==version&&found.Length==deps.Length&&
                 found.Select(x=>(string?)x.Attribute("id")??"")
                   .OrderBy(x=>x,StringComparer.Ordinal)
                   .SequenceEqual(deps.OrderBy(x=>x,StringComparer.Ordinal)),
                 "Local package ID/version/dependencies drifted: "+id);
            foreach(var dep in found)
            {
                var requirement=(string?)dep.Attribute("id")??"";
                var accepted=selected.Single(x=>x.Id==requirement);
                Need(((string?)dep.Attribute("version")??"")
                      .Contains(accepted.Version,StringComparison.Ordinal),
                     "Dependency version mismatch in current local NuGet: "+id);
            }
        }
        var digest=Sha(path);
        Need(Hex64(digest),"Local archive SHA-256 malformed: "+id);
        Console.WriteLine("RS128-PACK|"+id+"|"+version+"|"+bytes+"|"+digest);
    }
}
if(target)
{
    using var completedDoc=Open(Evidence);
    var r=completedDoc.RootElement;
    Need(Text(r,"status")==Status&&
         r.GetProperty("packages_packed").GetInt32()==4&&
         r.GetProperty("source_free_consumers").GetInt32()==2&&
         r.GetProperty("fresh_consumer_probes").GetInt32()==40&&
         r.GetProperty("native_nodes_retained").GetInt32()==541&&
         r.GetProperty("command_receipts").GetArrayLength()==12&&
         r.GetProperty("package_results").GetArrayLength()==4&&
         r.GetProperty("consumer_results").GetArrayLength()==2&&
         Flag(r,"offline_feed_only")&&Flag(r,"isolated_package_cache")&&
         !Flag(r,"publicly_published")&&
         !Flag(r,"external_requests_transmitted")&&
         !Flag(r,"external_approvals_received")&&
         !Flag(r,"discovery_skill_generated")&&
         !Flag(r,"cross_version_abi_certified")&&
         !Flag(r,"other_os_rids_certified")&&
         !Flag(r,"m0_m7_plan_mutated")&&
         Text(r,"decision_sha256")==Sha(Decision)&&
         Text(r,"distribution_sha256")==Sha(Matrix)&&
         Text(r,"validator_sha256")==Sha(Validator),
         "Target receipt claims an unauthorized product or lacks verified evidence.");
    var ids=new[]{
        "source-locked-restore","symbolics-release-build",
        "codecs-release-build","pack-dw.quantities",
        "pack-dw.tools.math.ir","pack-dw.tools.math.symbolics",
        "pack-dw.tools.math.codecs","independent-local-package-audit",
        "m4-offline-restore","m4-source-free-consumer",
        "m5-offline-restore","m5-source-free-consumer"
    };
    var commands=r.GetProperty("command_receipts").EnumerateArray().ToArray();
    for(var i=0;i<ids.Length;i++)
        Need(Text(commands[i],"id")==ids[i]&&
             commands[i].GetProperty("exit_code").GetInt32()==0,
             "Incomplete local source-free command: "+ids[i]);
    var products=r.GetProperty("package_results").EnumerateArray().ToArray();
    for(var i=0;i<selected.Length;i++)
        Need(Text(products[i],"package_id")==selected[i].Id&&
             Text(products[i],"version")==selected[i].Version&&
             products[i].GetProperty("archive_bytes").GetInt64()>0&&
             Hex64(Text(products[i],"archive_sha256")),
             "Recorded archive identity or hash invalid: "+selected[i].Id);
    var clients=r.GetProperty("consumer_results").EnumerateArray().ToArray();
    Need(Text(clients[0],"id")=="m4"&&
         clients[0].GetProperty("probes").GetInt32()==32&&
         Flag(clients[0],"source_free")&&
         Text(clients[1],"id")=="m5"&&
         clients[1].GetProperty("probes").GetInt32()==8&&
         Flag(clients[1],"source_free"),
         "Independent consumer receipt identities or probe numbers changed.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
