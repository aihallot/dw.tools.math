using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Backlog="docs/planning/backlog.json";
const string State=".aura/workflow/state.json";
const string Evidence="docs/planning/evidence/M3-gate.json";
const string M2Gate="docs/planning/evidence/M2-gate.json";
const string GateValidator="docs/planning/ValidateM3ReleaseGate.cs";
const string Consumer="tests/consumers/m3-gate/m3-gate-consumer.csproj";
const string ConsumerSource="tests/consumers/m3-gate/Program.cs";
const string ConsumerConfig="tests/consumers/m3-gate/NuGet.Config";
const string Recipe="docs/distribution/m3-release-gate.md";
const string Decision="docs/planning/decisions/m3-gate.md";
const string IsolatedOutput="dw.tools.math/m3-nuget-gate/0.4 qualified; probes=31";
const string ScienceMarker="dw.tools.math/m3-scientific-gate/0.4 qualified; phases=7; tasks=14; subtasks=42";
const string PackageHelper=".workflow/runs/RS052/staged/checks/m3-gate-package-sha.cs";
var finalPhases=new (string Id,string File,string Status)[]
{
    ("M3-W01-C01","M3-W01-C01-boundary-qualified.json",
        "managed-mathnet-runtime-rid-license-and-native-deferral-qualified"),
    ("M3-W01-C02","M3-W01-C02-boundaries-qualified.json",
        "explicit-correctly-rounded-rational-binary64-and-copy-only-boundaries-qualified"),
    ("M3-W01-C03","M3-W01-C03-boundaries-qualified.json",
        "bounded-2x2-condition-availability-rank-pivot-policy-qualified"),
    ("M3-W02-C01","M3-W02-C01-boundaries-qualified.json",
        "frequency-weight-and-explicit-null-missing-statistics-qualified"),
    ("M3-W02-C02","M3-W02-C02-boundaries-qualified.json",
        "typed-undefined-correlation-and-pair-missing-boundaries-qualified"),
    ("M3-W02-C03","M3-W02-C03-boundaries-qualified.json",
        "finite-continuous-distributions-and-portable-prng-checkpoints-qualified"),
    ("M3-W02-C04","M3-W02-C04-boundaries-qualified.json",
        "recipient-boundary-ownership-and-unsent-requests-qualified")
};
var products=new (string Id,string Version,string Project,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.numerics","0.4.0-preview.4",
        "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj",["dw.tools.math.ir"]),
    ("dw.tools.math.statistics","0.4.0-preview.6",
        "src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj",["dw.quantities"])
};
var testProjects=new[]
{
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.statistics.tests/dw.tools.math.statistics.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj",
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj"
};

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Evidence));
p.Workflow.RequireCurrentRun("RS052",target?null:"M3",null,null,null,null);
p.Workflow.RequirePriorSuccess("RS051",1,
    "docs: qualify consumer ownership boundaries and close statistics work package",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(M2Gate,"/status",
    JsonValue.Create("local-m2-typed-ir-composition-and-isolated-consumer-qualified"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.50"),JsonValue.Create("0.1.51"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.51":"0.1.50"));
p.Require.JsonValueEquals(Backlog,"/releases/2/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/3/gate",JsonValue.Create("G-M3"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("planned"));
foreach(var i in new[]{7,8})
    p.Require.JsonValueEquals(Backlog,$"/work_packages/{i}/status",JsonValue.Create("done"));
foreach(var i in new[]{20,21,22,23,24,25,26})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{i}/status",JsonValue.Create("done"));
foreach(var (id,file,status) in finalPhases)
{
    p.Require.JsonValueEquals("docs/planning/evidence/"+file,
        "/status",JsonValue.Create(status));
    p.ProjectPlan.RequireNodeState(id,"done");
}
p.Require.JsonValueEquals("docs/planning/evidence/M3-W01-C01-provider-qualified.json",
    "/selected_package_version",JsonValue.Create("5.0.0"));
p.Require.JsonValueEquals("docs/planning/evidence/M3-W02-C04-boundaries-qualified.json",
    "/requests_transmitted",JsonValue.Create(false));
p.Require.JsonValueEquals("docs/planning/evidence/M3-W02-C04-boundaries-qualified.json",
    "/external_adoption_observed",JsonValue.Create(false));
p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState("M3",target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState("M3-W02","done");
p.ProjectPlan.RequireNodeState("M4","not-ready");

if(!target)
{
    foreach(var file in new[]{Evidence,GateValidator,Consumer,ConsumerSource,
        ConsumerConfig,Recipe,Decision})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Evidence,"/status",
        JsonValue.Create("m3-local-numerics-and-statistics-preview-gate-qualified"));
    p.Require.JsonValueEquals(Evidence,"/cross_vertical_nuget_consumer_oracles",JsonValue.Create(31));
    p.Require.JsonValueEquals(Evidence,"/external_adoption_observed",JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence,"/package_publication_observed",JsonValue.Create(false));
}

// Source-only staging is always converged using typed byte-idempotent SDK
// operations. Do not perform local SHA-256 calculations on workflow authority.
var stagedHashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{GateValidator,Consumer,ConsumerSource,ConsumerConfig,Recipe,Decision})
{
    var result=p.Files.ReplaceFromStaged("staged/"+path,path);
    stagedHashes[path]=result.AfterSha256 ??
        throw new InvalidOperationException("Missing staged source SHA-256 from typed Payload SDK for "+path);
}
var science=Required(root,"dotnet","run","--file",GateValidator);
if(!HasExactLine(science,ScienceMarker))
    throw new InvalidOperationException("M3 scientific gate did not accept seven exact native/product phase authorities.");

if(!target)
{
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");

    foreach(var project in testProjects)
    {
        var result=Required(root,"dotnet","test",project,"-c","Release",
            "--no-build","--no-restore","--logger","console;verbosity=minimal");
        if(!result.Contains("Passed",StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("No passing test summary observed for "+project);
    }

    // This is an explicit release gate: repeat the foundation's original
    // isolated package boundary, not just new feature tests.
    var foundation=Required(root,"pwsh","-NoProfile","-NonInteractive",
        "-File","scripts/verify.ps1");
    if(!foundation.Contains("M0-W01-C01 foundation chain verified.",StringComparison.Ordinal))
        throw new InvalidOperationException("Original foundation verification was not observed.");

    var providerChecks=new (string Project,string Marker)[]
    {
        ("tests/providers/m3-w01-c01/MathNetSmoke.csproj","smoke-net10-managed:M3-W01-C01:qualified"),
        ("tests/providers/m3-w01-c03/ManagedLinearAlgebraSmoke.csproj",
            "M3-W01-C03:independent-managed-lu-qr-svd-smoke:qualified"),
        ("tests/providers/m3-w02-c01/ManagedStatisticsSmoke.csproj",
            "M3-W02-C01:managed-mathnet-5.0.0-descriptive-oracles:qualified")
    };
    foreach(var smoke in providerChecks)
    {
        var output=Required(root,"dotnet","run","--project",smoke.Project,"-c","Release");
        if(!output.Contains(smoke.Marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Pinned managed-provider smoke marker missing: "+smoke.Project);
    }
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");
    Required(root,"dotnet","run","--file","docs/planning/ValidateC04ConsumerBoundaries.cs");

    // Pack exact local release-graph bytes, not a symbolic provider registry.
    var feed=Path.Combine(root,"build","artifacts","m3-gate-local-feed");
    var archives=new List<string>();
    foreach(var product in products)
    {
        Required(root,"dotnet","pack",product.Project,"-c","Release",
            "--no-build","--no-restore","-o",feed);
        var archive=Path.Combine(feed,product.Id+"."+product.Version+".nupkg");
        if(!File.Exists(archive))
            throw new InvalidOperationException("M3 expected local NuGet archive is missing: "+product.Id);
        archives.Add(archive);
    }
    var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
    helperArgs.AddRange(archives);
    var packageOutput=Required(root,"dotnet",helperArgs.ToArray());
    var packages=new List<JsonNode>();
    foreach(var product in products)
    {
        var (size,sha)=ReadProductPackageReceipt(packageOutput,product.Id,product.Version);
        packages.Add(new JsonObject
        {
            ["id"]=product.Id,
            ["version"]=product.Version,
            ["bytes"]=size,
            ["sha256"]=sha,
            ["assembly"]="lib/net10.0/"+product.Id+".dll",
            ["dependencies"]=p.Json.StringArray(product.Dependencies),
            ["mode"]="local-ignored-artifact-not-published"
        });
    }

    var consumerProjectPath=Path.Combine(root,Consumer);
    var consumerXml=XDocument.Load(consumerProjectPath);
    var refs=consumerXml.Descendants().Where(x=>x.Name.LocalName=="PackageReference")
        .Select(x=>(Id:(string?)x.Attribute("Include")??"",
            Version:(string?)x.Attribute("Version")??"")).OrderBy(x=>x.Id,StringComparer.Ordinal).ToArray();
    if(refs.Length!=2||refs[0].Id!="dw.tools.math.numerics" ||
        refs[0].Version!="[0.4.0-preview.4]"||
        refs[1].Id!="dw.tools.math.statistics"||
        refs[1].Version!="[0.4.0-preview.6]"||
        consumerXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("M3 independent consumer must use only two pinned top-level NuGet packages.");
    var nuget=XDocument.Load(Path.Combine(root,ConsumerConfig));
    var sources=nuget.Descendants().Single(x=>x.Name.LocalName=="packageSources");
    if(!sources.Elements().Any(x=>x.Name.LocalName=="clear")||
       sources.Elements().Count(x=>x.Name.LocalName=="add")!=1)
        throw new InvalidOperationException("M3 consumer must refuse external package feeds.");
    var packagesDirectory=Path.Combine(root,"build","artifacts","m3-gate-isolated-packages");
    Required(root,"dotnet","restore",Consumer,"--source",feed,
        "--configfile",ConsumerConfig,"--packages",packagesDirectory);
    Required(root,"dotnet","build",Consumer,"-c","Release","--no-restore");
    var output=Required(root,"dotnet","run","--project",Consumer,"-c","Release",
        "--no-build","--no-restore");
    if(!HasExactLine(output,IsolatedOutput))
        throw new InvalidOperationException("31 independent M3 package-only cross-vertical probes did not all pass.");

    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M3-gate",
        ["status"]="m3-local-numerics-and-statistics-preview-gate-qualified",
        ["milestone"]="M3",["gate"]="G-M3",["release_version"]="0.4.0-preview",
        ["plan_version"]="0.1.51",["previous_m2_gate"]=M2Gate,
        ["checked_chunk_ids"]=p.Json.StringArray(finalPhases.Select(x=>x.Id).ToArray()),
        ["qualified_phase_evidence"]=p.Json.StringArray(finalPhases
            .Select(x=>"docs/planning/evidence/"+x.File).ToArray()),
        ["completed_work_packages"]=p.Json.StringArray("M3-W01","M3-W02"),
        ["qualified_tasks"]=14,["qualified_subtasks"]=42,
        ["gate_scientific_validator"]=GateValidator,
        ["scientific_validator_sha256"]=stagedHashes[GateValidator],
        ["scientific_gate_marker"]=ScienceMarker,
        ["locked_solution_restore_and_build_passed"]=true,
        ["full_project_test_suites_passed"]=p.Json.StringArray(testProjects),
        ["foundation_chain_and_isolated_consumer_passed"]=true,
        ["managed_provider_smokes"]=p.Json.StringArray(
            "M3-W01-C01 managed Math.NET 5.0.0",
            "M3-W01-C03 LU QR singular spectrum",
            "M3-W02-C01 descriptive statistics"),
        ["provider_selected"]="MathNet.Numerics/5.0.0 managed on observed runtime",
        ["native_blas_admitted"]=false,
        ["local_package_only"]=true,
        ["package_sha256_verified_by_independent_staged_helper"]=PackageHelper,
        ["observed_local_packages"]=p.Json.Array(packages.ToArray()),
        ["cross_vertical_consumer_project"]=Consumer,
        ["consumer_project_sha256"]=stagedHashes[Consumer],
        ["cross_vertical_consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=stagedHashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=stagedHashes[ConsumerConfig],
        ["cross_vertical_nuget_consumer_oracles"]=31,
        ["isolated_consumer_exact_output"]=IsolatedOutput,
        ["source_only_nuget_restore"]=true,
        ["no_consumer_project_references"]=true,
        ["transfer_architecture_passed"]=true,
        ["decision_mcdm_owner_boundaries_validated"]=true,
        ["release_recipe"]=Recipe,["release_recipe_sha256"]=stagedHashes[Recipe],
        ["gate_decision"]=Decision,["gate_decision_sha256"]=stagedHashes[Decision],
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["external_adoption_observed"]=false,
        ["package_publication_observed"]=false,
        ["m4_symbolic_work_admitted"]=false,
        ["nonclaims"]=p.Json.StringArray(
            "No external Decision/MCDM/AURA package adoption, public NuGet push, agent skill or neutral dw.discovery conformance.",
            "No native BLAS, unmanaged provider, full public SVD, other unexecuted RID or universal floating-point error bound.",
            "No normal/Gaussian tail solver, cryptographic PRNG, general symbolic engine or claimed stable 1.0 product.",
            "Package hashes are SHA-256 of exact current local archives, not cross-SDK reproducibility assertions.")
    };
    p.Files.WriteComplete(Evidence,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.51";
        var milestone=plan["releases"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]=="M3");
        milestone["status"]="done";
        milestone["evidence"]=p.Json.StringArray(
            finalPhases.Select(x=>"docs/planning/evidence/"+x.File)
                .Concat(new[]{Evidence}).ToArray());
    });
    p.ProjectPlan.ConvergeNodeToDone("M3");
}

p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M3-W01","done");
p.ProjectPlan.RequireNodeState("M3-W02","done");
p.ProjectPlan.RequireNodeState("M4","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS052" ||
       (string?)state["activeMilestone"]!=(target?null:"M3")||
       state["activeWorkPackage"] is not null ||
       state["activePhase"] is not null ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS052 baseline/target milestone execution identity diverged.");
    state["activeMilestone"]=null;
});
if(!target)Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file",GateValidator);
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasExactLine(string output,string marker) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==marker);

static (long Bytes,string Sha) ReadProductPackageReceipt(string output,string id,string version)
{
    var prefix="M3-PKG-SHA256:"+id+":"+version+":";
    var lines=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(lines.Length!=1)
        throw new InvalidOperationException("Expected exactly one independent M3 package receipt: "+id);
    var payload=lines[0][prefix.Length..].Split(':');
    if(payload.Length!=2||
       !long.TryParse(payload[0],NumberStyles.None,CultureInfo.InvariantCulture,out var size) ||
       size<=0 || payload[1].Length!=64 ||
       payload[1].Any(ch=>!((ch>='0' && ch<='9')||(ch>='a' && ch<='f'))))
        throw new InvalidOperationException("Invalid independent SHA-256 product package observation: "+id);
    return(size,payload[1]);
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var excerpt=i<0?output:output[i..];
    return excerpt.Length>2400?excerpt[..2400]:excerpt;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
