#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var validationInput = args.Length == 2 && args[0] == "--validate"
    ? Path.GetFullPath(args[1])
    : null;
if (validationInput is null && (args.Length != 1 || args[0] != "--check"))
    throw new ArgumentException("Expected --check or --validate <manifest-path>.");

const string ManifestFile = "docs/discovery/qualification/m8-discovery-manifest.json";
const string BindingsFile = "docs/discovery/qualification/m8-selected-clr-bindings.json";
const string StrictFile = "docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs";
const string StrictAccepted = "MATH-M8-STRICT-BINDING|mode=validate|capabilities=4|metadataOnly=true|brokerReady=0|PASS";

using (var doc = JsonDocument.Parse(File.ReadAllText(".aura/workflow/reports/RS292/attempt-001.json")))
{
    var report = doc.RootElement;
    var first = report.GetProperty("priorInvocationFailures");
    var done = report.GetProperty("progress").GetProperty("completions")
        .EnumerateArray().Select(x => x.GetProperty("id").GetString()).ToArray();
    Need(report.GetProperty("runId").GetString() == "RS292" &&
        report.GetProperty("outcome").GetString() == "succeeded" &&
        report.GetProperty("attempt").GetInt32() == 1 &&
        first.GetProperty("count").GetInt32() == 0 &&
        first.GetProperty("firstPassSucceeded").GetBoolean() &&
        done.Length == 2 && done.Contains("M8-W01-C01-T1") &&
        done.Contains("M8-W01-C01-T1-V"),
        "RS292 task-completion evidence differs.");
}
var manifest = JsonNode.Parse(File.ReadAllText(ManifestFile))?.AsObject() ??
    throw new InvalidDataException("RS293: Missing source-qualified manifest.");
var sourceBindings = JsonNode.Parse(File.ReadAllText(BindingsFile))?.AsObject() ??
    throw new InvalidDataException("RS293: Missing selected source bindings.");
var caps = Caps(manifest);
var pinned = sourceBindings["bindings"]?.AsArray() ??
    throw new InvalidDataException("RS293: Missing binding pins.");
Need(sourceBindings["status"]?.GetValue<string>() == "selected-clr-fingerprints-only" &&
    sourceBindings["packageConsumerQualified"]?.GetValue<bool>() == false &&
    sourceBindings["auraIntegrationVerified"]?.GetValue<bool>() == false &&
    sourceBindings["brokerReadyCount"]?.GetValue<int>() == 0 &&
    manifest["schemaVersion"]?.GetValue<int>() == 2 &&
    manifest["protocolId"]?.GetValue<string>() == "dw.discovery" &&
    manifest["protocolVersion"]?.GetValue<string>() == "1.0.0-draft.1" &&
    manifest["brokerCandidateIds"] is JsonArray candidates &&
    candidates.Count == 0 && caps.Count == 4 && pinned.Count == 4,
    "Source manifest/selection scope changed.");

var privateFeed = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(privateFeed) &&
    Path.IsPathFullyQualified(privateFeed) && Directory.Exists(privateFeed),
    "NUGET_LOCAL must be an existing absolute private feed.");
var work = Path.GetFullPath(Path.Combine("build", "artifacts",
    "m8-rs293-reflection-red", Guid.NewGuid().ToString("N")));
var binaries = Path.Combine(work, "dll");
Directory.CreateDirectory(binaries);
var packages = new[]
{
    (Id: "dw.tools.math.composition", Version: "20260.20261107.1001",
     Archive: "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f",
     Binary: "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252"),
    (Id: "dw.tools.math.ir", Version: "20260.20261104.1001",
     Archive: "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480",
     Binary: "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af"),
    (Id: "dw.quantities", Version: "20260.20261102.1001",
     Archive: "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d",
     Binary: "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4")
};
foreach (var p in packages)
{
    var source = Path.Combine(privateFeed!, p.Id + "." + p.Version + ".nupkg");
    Need(File.Exists(source) &&
        (File.GetAttributes(source) & FileAttributes.ReparsePoint) == 0 &&
        HashFile(source) == p.Archive, "Private archive SHA-256 changed: " + p.Id);
    using var zip = ZipFile.OpenRead(source);
    var entry = zip.GetEntry("lib/net10.0/" + p.Id + ".dll") ??
        throw new InvalidDataException("RS293: Missing published DLL " + p.Id);
    var path = Path.Combine(binaries, p.Id + ".dll");
    entry.ExtractToFile(path);
    Need(HashFile(path) == p.Binary, "Published DLL SHA-256 changed: " + p.Id);
}
var context = new AssemblyLoadContext("m8-rs293-published-dlls", isCollectible: true);
context.Resolving += (load, name) =>
{
    if (string.IsNullOrWhiteSpace(name.Name)) return null;
    var path = Path.Combine(binaries, name.Name + ".dll");
    return File.Exists(path) ? load.LoadFromAssemblyPath(path) : null;
};
var loaded = new Dictionary<string, Assembly>(StringComparer.Ordinal);
foreach (var p in packages)
{
    var assembly = context.LoadFromAssemblyPath(Path.Combine(binaries, p.Id + ".dll"));
    var name = assembly.GetName();
    Need(name.Name == p.Id && name.Version?.ToString() ==
        (p.Id == "dw.quantities" ? "0.2.0.0" : "0.3.0.0"),
        "Unexpected published assembly identity: " + p.Id);
    loaded.Add(p.Id, assembly);
}

string[] selected =
[
    "math.quantity.exact-pipeline.evaluate",
    "math.replay.try-evaluate",
    "math.ir.canonical.encode",
    "math.ir.hash.semantic"
];
var positiveCount = AdmitRealPublishedBindings(manifest, pinned, loaded, selected);
Need(positiveCount == selected.Length, "All selected real published bindings must qualify.");
if (validationInput is not null)
{
    var external = JsonNode.Parse(File.ReadAllText(validationInput))?.AsObject() ??
        throw new InvalidDataException("RS294: Invalid input manifest.");
    var admittedCount = AdmitRealPublishedBindings(external, pinned, loaded, selected);
    Need(admittedCount == selected.Length, "Incomplete real published CLR selection.");
    Console.WriteLine("MATH-RS294-REAL-CLR-ADMISSION|mode=validate|bindings=4|archives=3|" +
        "assemblyHashes=matched|runtimeOnly=true|methodInvocations=0|brokerReady=0|PASS");
    return;
}
var unaltered = RunStrict(ManifestFile);
Need(unaltered.ExitCode == 0 &&
    unaltered.Output.Contains(StrictAccepted, StringComparison.Ordinal),
    "Metadata-only positive source manifest no longer qualifies.");

var beforeManifest = HashFile(ManifestFile);
var negative = 0;
void Falsify(string name, Action<JsonObject> change)
{
    var altered = manifest.DeepClone().AsObject();
    var first = Caps(altered)[0]?.AsObject() ??
        throw new InvalidDataException("RS293: Forgery target absent.");
    change(first);
    var signature = DeclaredSignature(first);
    Binding(first)["canonicalSignature"] = signature;
    Binding(first)["fingerprint"] = HashText(signature);
    var forgedPath = Path.Combine(work, "forged-" + name + ".json");
    File.WriteAllText(forgedPath,
        altered.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) +
        "\n", new UTF8Encoding(false));
    var accepted = RunStrict(forgedPath);
    Need(accepted.ExitCode == 0 &&
        accepted.Output.Contains(StrictAccepted, StringComparison.Ordinal),
        "Coherent forgery was not admitted by metadata-only checker: " + name +
        "\n" + accepted.Output);
    Need(!ExactMethodExists(first, loaded),
        "Published CLR accepted a coherent but false method: " + name);
    var refusedByAdmission = false;
    try
    {
        AdmitRealPublishedBindings(altered, pinned, loaded, selected);
    }
    catch (InvalidDataException error)
        when (error.Message.Contains("RS294: Real published CLR method mismatch:",
            StringComparison.Ordinal))
    {
        refusedByAdmission = true;
    }
    Need(refusedByAdmission, "Coherent forgery bypassed the real published CLR gate: " + name);
    negative++;
    Console.WriteLine("MATH-RS293-RED-FORGERY|" + name +
        "|metadata=accepted|actualCLR=refused|PASS");
}
Falsify("nonexistent-member",
    item => Binding(item)["member"] = "NonexistentSelectedMethod");
Falsify("wrong-parameter",
    item => (item["parameters"]?.AsArray()?[0]?.AsObject() ??
        throw new InvalidDataException("RS293: Parameter missing."))["type"] = "System.String");
Falsify("wrong-result",
    item => (item["result"]?.AsObject() ??
        throw new InvalidDataException("RS293: Result missing."))["type"] = "System.String");
Need(negative == 3, "Exactly three new CLR falsifications are required.");

var forbidden = manifest.DeepClone().AsObject();
var original = Caps(forbidden)[0]?.AsObject() ??
    throw new InvalidDataException("RS293: Authorization target absent.");
(original["execution"]?.AsObject() ??
    throw new InvalidDataException("RS293: Execution missing."))["authorization"] = "granted";
var forbiddenPath = Path.Combine(work, "authorization-granted.json");
File.WriteAllText(forbiddenPath, forbidden.ToJsonString() + "\n",
    new UTF8Encoding(false));
var denied = RunStrict(forbiddenPath);
Need(denied.ExitCode != 0 &&
    denied.Output.Contains("RS287: Runtime-only authorization is required",
        StringComparison.Ordinal),
    "Original metadata-only authorization refusal regressed.");

Need(HashFile(ManifestFile) == beforeManifest, "Canonical manifest was mutated.");
foreach (var p in packages)
    Need(HashFile(Path.Combine(privateFeed!, p.Id + "." + p.Version + ".nupkg")) == p.Archive &&
        HashFile(Path.Combine(binaries, p.Id + ".dll")) == p.Binary,
        "Private or extracted published bytes changed: " + p.Id);
Console.WriteLine("MATH-RS293-CLR-RED|bindings=4|archives=3|" +
    "coherentForgeries=3|metadataOnlyAccepted=3|realCLRRefused=3|" +
    "authorizationRefusal=1|methodInvocations=0|brokerReady=0|PASS");
Console.WriteLine("MATH-RS294-REAL-CLR-ADMISSION|mode=check|positive=4|" +
    "coherentForgeriesRefused=3|authorizationRefused=1|brokerReady=0|PASS");

static int AdmitRealPublishedBindings(JsonObject candidate, JsonArray pinned,
    IReadOnlyDictionary<string, Assembly> binaries, IReadOnlyList<string> approved)
{
    Need(candidate["schemaVersion"]?.GetValue<int>() == 2 &&
        candidate["protocolId"]?.GetValue<string>() == "dw.discovery" &&
        candidate["protocolVersion"]?.GetValue<string>() == "1.0.0-draft.1" &&
        candidate["owner"]?.GetValue<string>() == "math" &&
        candidate["brokerCandidateIds"] is JsonArray brokerCandidates &&
        brokerCandidates.Count == 0,
        "RS294: Only source-qualified Math Discovery schema-2 metadata is admitted.");
    var list = Caps(candidate);
    Need(list.Count == approved.Count && pinned.Count == approved.Count,
        "RS294: Exactly four selected Math operations are required.");
    for (var i = 0; i < approved.Count; i++)
    {
        var item = list[i]?.AsObject() ??
            throw new InvalidDataException("RS294: Null selected capability.");
        var pin = pinned[i]?.AsObject() ??
            throw new InvalidDataException("RS294: Missing selected source binding.");
        var id = Read(item, "capabilityId");
        Need(id == approved[i] && Read(pin, "capabilityId") == id &&
            Read(item, "exposure") == "AgentCallable" &&
            !item.ContainsKey("brokerContract") &&
            Read(item, "package") == Read(Binding(item), "assembly") &&
            Read(item, "packageVersion") == "0.3.0-preview.1" &&
            Read(item["execution"]?.AsObject() ??
                throw new InvalidDataException("RS294: Execution metadata absent."),
                "authorization") == "runtime-only",
            "RS294: Selected identities, source packages and runtime-only scope differ.");
        Need(ExactMethodExists(item, binaries),
            "RS294: Real published CLR method mismatch: " + id);
        foreach (var field in new[]
        {
            "assembly", "assemblyVersion", "type", "member",
            "canonicalSignature", "fingerprint"
        })
            Need(Read(Binding(item), field) == Read(pin, field),
                "RS294: Actual runtime method does not match selected source pin: " +
                id + " field=" + field);
    }
    return list.Count;
}

ProcessResult RunStrict(string manifestPath)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = Path.GetFullPath("."),
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in new[]
    {
        "run", "--file", StrictFile, "--", "--validate", Path.GetFullPath(manifestPath)
    }) start.ArgumentList.Add(arg);
    using var process = Process.Start(start) ??
        throw new InvalidOperationException("RS293: dotnet strict checker cannot start.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS293: Strict metadata check timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult());
}
static bool ExactMethodExists(JsonObject item, IReadOnlyDictionary<string, Assembly> assemblies)
{
    var b = Binding(item);
    var assemblyId = Read(b, "assembly");
    if (!assemblies.TryGetValue(assemblyId, out var assembly) ||
        assembly.GetName().Version?.ToString() != Read(b, "assemblyVersion"))
        return false;
    var owner = assembly.GetType(Read(b, "type"), throwOnError: false);
    if (owner is null) return false;
    var declared = DeclaredSignature(item);
    if (Read(b, "canonicalSignature") != declared ||
        Read(b, "fingerprint") != HashText(declared)) return false;
    return owner.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(method =>
        method.Name == Read(b, "member") &&
        MethodSignature(assemblyId, assembly, owner, method) == declared);
}
static string MethodSignature(string id, Assembly assembly, Type owner, MethodInfo method) =>
    string.Join("|", id, assembly.GetName().Version?.ToString(), owner.FullName,
        method.Name, ClrType(method.ReturnType) + "(" +
        string.Join(",", method.GetParameters().Select(p => ClrType(p.ParameterType))) + ")");

static string ClrType(Type type)
{
    if (type.IsByRef) return ClrType(type.GetElementType()!) + "&";
    if (type.IsArray) return ClrType(type.GetElementType()!) + "[]";
    if (type.IsGenericType)
    {
        var generic = type.GetGenericTypeDefinition().FullName ??
            throw new InvalidDataException("RS293: Generic CLR type name missing.");
        var tick = generic.IndexOf((char)96);
        Need(tick > 0, "Unexpected CLR generic name.");
        return generic[..tick] + "<" +
            string.Join(",", type.GetGenericArguments().Select(ClrType)) + ">";
    }
    return type.FullName ??
        throw new InvalidDataException("RS293: CLR type name missing.");
}
static string DeclaredSignature(JsonObject item)
{
    var binding = Binding(item);
    var result = item["result"]?.AsObject() ??
        throw new InvalidDataException("RS293: CLR result missing.");
    var parameters = item["parameters"]?.AsArray() ??
        throw new InvalidDataException("RS293: CLR parameters missing.");
    return string.Join("|", Read(binding, "assembly"),
        Read(binding, "assemblyVersion"), Read(binding, "type"),
        Read(binding, "member"),
        Read(result, "type") + "(" + string.Join(",", parameters.Select(x =>
            Read(x?.AsObject() ??
                throw new InvalidDataException("RS293: Invalid parameter."), "type"))) + ")");
}
static string HashText(string data) =>
    Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(data)));
static string HashFile(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static JsonArray Caps(JsonObject item) =>
    item["capabilities"]?.AsArray() ??
    throw new InvalidDataException("RS293: Missing capability array.");
static JsonObject Binding(JsonObject item) =>
    item["binding"]?.AsObject() ??
    throw new InvalidDataException("RS293: CLR binding missing.");
static string Read(JsonObject item, string name) =>
    item[name]?.GetValue<string>() is { Length: > 0 } value
        ? value : throw new InvalidDataException("RS293: Missing " + name);
static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS293: " + message);
}
sealed record ProcessResult(int ExitCode, string Output);
