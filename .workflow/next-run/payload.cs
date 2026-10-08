using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C01";
const string Task = "M1-W02-C01-T2";
const string Verify = "M1-W02-C01-T2-V";
const string Inherited = "src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs";
const string Resolver = "src/projects/dw.quantities.standard/StandardExpressionUnitResolver.cs";
const string Tests = "tests/projects/dw.quantities.tests/M1W02C01SourceParityTests.cs";
const string Evidence = "docs/planning/evidence/M1-W02-C01-source-parity-qualified.json";
const string Original = "docs/planning/evidence/M1-W02-C01-AURA-StandardUnitCatalog.txt";
const string ResolverOriginal = "docs/planning/evidence/M1-W02-C01-AURA-StandardExpressionUnitResolver.txt";
const string OriginalSha = "f588545e89f51c78dd82a0d7c08ea92ab90fa48d0f5862d18666a00d0f13249a";
const string ResolverSha = "3e71ed4af6abc748665c3f382093d625974c002649a8353d82dc4b5ad02bb561";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var product = JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk = product["chunks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Phase);
var t2 = chunk["tasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Task);
var v = t2["subtasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Verify);
var baseline = (string?)product["plan_version"] == "0.1.18" &&
    (string?)chunk["status"] == "in_progress" &&
    (string?)t2["status"] == "in_progress" &&
    (string?)v["status"] == "ready";
var target = (string?)product["plan_version"] == "0.1.19" &&
    (string?)chunk["status"] == "done" &&
    (string?)t2["status"] == "done" &&
    (string?)v["status"] == "done";
if (!baseline && !target)
    throw new InvalidOperationException("RS020 product lifecycle is neither baseline nor exact completed target.");

var originalBytes = File.ReadAllBytes(Path.Combine(root,Original));
var resolverBytes = File.ReadAllBytes(Path.Combine(root,ResolverOriginal));
if (Hash(originalBytes) != OriginalSha || Hash(resolverBytes) != ResolverSha)
    throw new InvalidOperationException("The AURA source review snapshots no longer match their pinned SHA-256.");

var originalText = new UTF8Encoding(false,true).GetString(originalBytes);
var expectedAdapter = AdaptMathData(originalText);
var stagedAdapter = File.ReadAllText(Path.Combine(p.RunRoot,"staged",Inherited));
if (!string.Equals(stagedAdapter,expectedAdapter,StringComparison.Ordinal))
    throw new InvalidOperationException("Staged inherited catalogue is not the deterministic mathematical-data transform of the pinned AURA source.");

p.Files.ReplaceFromStaged("staged/" + Inherited, Inherited);
p.Files.ReplaceFromStaged("staged/" + Resolver, Resolver);
p.Files.ReplaceFromStaged("staged/" + Tests, Tests);
if (!string.Equals(File.ReadAllText(Path.Combine(root,Inherited)),expectedAdapter,StringComparison.Ordinal))
    throw new InvalidOperationException("Installed inherited catalogue differs from the approved deterministic adapter.");

RunRequired(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
RunRequired(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
foreach (var method in new[] {
    "InheritedCatalogueRetainsOriginalVersionAndAllFiftyEightUnitDefinitions",
    "AllOriginalCanonicalIdsAreAccessibleWithoutCultureInference",
    "ExactMetricAndImperialConstantsRemainSourceFaithful",
    "CompleteDecimalAndBinaryInformationPrefixesRetainExactPowers",
    "AurasThreeCupProfilesAndThreePintProfilesRequireExplicitSelection",
    "CustomInitialCatalogueAndInheritedAurasPintPolicyRemainDistinct",
    "InheritedSourceKeepsExactAffineTemperatures",
    "FrenchAliasDoesNotMeanFrenchCultureChoosesImperialUnits",
    "HostCultureNeverSelectsAmbiguousOriginalAliases",
    "AdaptedCatalogueDependsOnNoHostLocalizationAssembly",
    "EveryInheritedUnitPerformsAnExactBaseRoundTrip"
})
{
    RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M1W02C01SourceParityTests."+method,
        "--logger","console;verbosity=normal");
}
RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W02-C01-source-parity-qualified",
    ["status"] = "mathematical-source-structure-parity-qualified-with-explicit-host-exclusions",
    ["source_revision"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["source_catalog_snapshot"] = Original,
    ["source_catalog_sha256"] = OriginalSha,
    ["source_resolver_snapshot"] = ResolverOriginal,
    ["source_resolver_sha256"] = ResolverSha,
    ["adapted_source"] = Inherited,
    ["adapted_source_sha256"] = Hash(File.ReadAllBytes(Path.Combine(root,Inherited))),
    ["transform_policy"] = p.Json.StringArray(
        "Keep every unit declaration and exact scale, dimension, symbol, alias and canonical ID from the pinned original catalogue.",
        "Rename the catalogue to InheritedStandardUnitCatalog to preserve the already-qualified, separate 14-unit strict profile surface.",
        "Remove only the dw.localization import, StandardUnitMetadata declaration, LocalizedText metadata construction, TryGetMetadata and FrenchName presentation projection.",
        "Preserve the original version, all 58 unit definitions and their exact deterministic information-unit power generation."),
    ["unit_count"] = 58,
    ["prior_catalogue"] = "src/projects/dw.quantities.standard/StandardUnitCatalog.cs",
    ["resolver"] = Resolver,
    ["inherited_resolution"] = "Explicit profile or canonical ID; never implicit host-culture selection.",
    ["deliberate_differences"] = p.Json.StringArray(
        "Original resolver IExpressionUnitResolver.Resolve(token,culture) is not copied: host culture priority must not decide Math values under the M0 localization decision.",
        "The original LocalizedText metadata remains host-owned; the Math catalogue exposes pure unit data and passive labels only.",
        "Initial 14-unit catalogue keeps its named AU beer-pint 570 mL extension; inherited 0.2.0 data has no AU pint and exposes an explicit metric-international pint.",
        "Integration with the bounded expression parser is owned by M1-W02-C02 and is not claimed by this C01 qualification.",
        "No silent equivalence is claimed between legacy culture-priority resolution and explicit profile resolution."),
    ["mathematical_oracles"] = p.Json.StringArray(
        "58 unique inherited source IDs with exact original version and no scalar pseudo-units.",
        "All 58 inherited units have exact identity and source/base/self roundtrips.",
        "Imperial inch, foot and pound, metric litre/millilitre, metric speed and US/AU tablespoon constants remain exact.",
        "All eight decimal 1000^n and eight binary 1024^n information prefixes remain exact BigInteger values.",
        "Cups and pints expose all original candidates and reject unresolved ambiguity; explicit profiles recover the source IDs.",
        "Celsius/Fahrenheit absolute conversions and source aliases match independent exact oracles.",
        "No localization/host assembly dependency and no ambient-culture routing."),
    ["focused_filter"] = "FullyQualifiedName~M1W02C01SourceParityTests",
    ["focused_passed"] = true,
    ["entire_quantities_tests_passed"] = true,
    ["foundation_chain_passed"] = true,
    ["transfer_architecture_passed"] = true,
    ["limits"] = "Parities here concern the original mathematical unit definitions and the declared exact transform; presentation metadata and legacy culture-priority expression resolution are deliberately excluded and independently owned."
};
p.Files.WriteComplete(Evidence,evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json", backlog =>
{
    var c = backlog["chunks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Phase);
    var task = c["tasks"]!.AsArray().Select(x => x!.AsObject()).Single(x => (string?)x["id"] == Task);
    var verification = task["subtasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == Verify);
    backlog["plan_version"] = "0.1.19";
    c["status"] = "done";
    c["refinement"] = "RS018 qualified a strict standalone 14-unit subset. RS019 qualified bounded profiles/aliases and captured pinned source snapshots. RS020 deterministically adapts all 58 inherited AURA mathematical unit definitions into a separate host-free catalogue, tests independent exact constants, IDs, ambiguities and temperature rules, and explicitly defers expression-parser integration to M1-W02-C02.";
    var fileSet = c["files"]!.AsArray();
    foreach (var path in new[] { Inherited, Resolver, Tests, Evidence })
        if (!fileSet.Any(x => (string?)x == path))
            fileSet.Add((JsonNode?)JsonValue.Create(path));
    var cmd = c["commands"]!.AsArray();
    var focused = "dotnet test " + TestProject + " -c Release --filter FullyQualifiedName~M1W02C01SourceParityTests";
    if (!cmd.Any(x => (string?)x == focused))
        cmd.Add((JsonNode?)JsonValue.Create(focused));
    var ev = c["evidence"]!.AsArray();
    if (!ev.Any(x => (string?)x == Evidence))
        ev.Add((JsonNode?)JsonValue.Create(Evidence));
    task["status"] = "done";
    task["evidence"] = p.Json.StringArray(
        "docs/planning/evidence/M1-W02-C01-red.json",
        "docs/planning/evidence/M1-W02-C01-boundary-green.json",
        Evidence);
    verification["status"] = "done";
    verification["evidence"] = p.Json.StringArray(Evidence);
});
if (baseline)
{
    p.ProjectPlan.ActivateReadyContinuation(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Verify);
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach (var id in new[] { Verify, Task, Phase })
        p.ProjectPlan.RequireNodeState(id,"done");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string AdaptMathData(string text)
{
    text = ReplaceOnce(text,"using dw.localization;\n","");
    text = ReplaceOnce(text,"public sealed record StandardUnitMetadata(UnitDefinition Unit, LocalizedText DisplayName);\n\n","");
    text = ReplaceOnce(text,"public static class StandardUnitCatalog","public static class InheritedStandardUnitCatalog");
    text = ReplaceOnce(text,"static StandardUnitCatalog()","static InheritedStandardUnitCatalog()");
    text = RemoveBetween(text,
        "    public static ImmutableArray<StandardUnitMetadata> Metadata { get; } =",
        "    static InheritedStandardUnitCatalog()");
    text = RemoveBetween(text,
        "    public static bool TryGetMetadata(string id, out StandardUnitMetadata? metadata) =>",
        "    private static UnitDefinition Linear(");
    text = ReplaceOnce(text,"public static class InheritedStandardUnitCatalog",
        "/// <summary>\n/// Data-faithful AURA 0.2.0 catalogue with host localization removed.\n/// Unlike StandardUnitCatalog, this retains inherited unit identifiers and alias data.\n/// </summary>\npublic static class InheritedStandardUnitCatalog");
    if (text.Contains("dw.localization",StringComparison.Ordinal) ||
        text.Contains("LocalizedText",StringComparison.Ordinal) ||
        text.Contains("StandardUnitMetadata",StringComparison.Ordinal))
        throw new InvalidOperationException("A host-localization dependency remained in the adapted source.");
    return text;
}

static string ReplaceOnce(string source,string before,string after)
{
    var index=source.IndexOf(before,StringComparison.Ordinal);
    if (index<0 || source.IndexOf(before,index+before.Length,StringComparison.Ordinal)>=0)
        throw new InvalidOperationException("Expected exactly one source adaptation anchor: "+before);
    return source.Replace(before,after,StringComparison.Ordinal);
}

static string RemoveBetween(string source,string start,string end)
{
    var first=source.IndexOf(start,StringComparison.Ordinal);
    var last=first<0?-1:source.IndexOf(end,first,StringComparison.Ordinal);
    if (first<0 || last<=first ||
        source.IndexOf(start,first+start.Length,StringComparison.Ordinal)>=0 ||
        source.IndexOf(end,last+end.Length,StringComparison.Ordinal)>=0)
        throw new InvalidOperationException("Invalid localization-only block boundary.");
    return string.Concat(source.AsSpan(0,first),source.AsSpan(last));
}

static string Hash(byte[] bytes) =>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string ImportantFailure(string output)
{
    var start=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(start<0)start=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var useful=start<0?output:output[start..];
    return useful.Length>2800?useful[..2800]:useful;
}

static (int Code,string Output) Run(string root,string exe,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=exe,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+exe);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000)){
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(exe+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}

static void RunRequired(string root,string exe,params string[] args)
{
    var result=Run(root,exe,args);
    if(result.Code!=0)
        throw new InvalidOperationException(exe+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+ImportantFailure(result.Output));
}
