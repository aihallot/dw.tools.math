#:property PublishAot=false
#:property NuGetAudit=false
using System.Text.Json.Nodes;
try
{
    var root=Directory.GetCurrentDirectory();
    var baseline=Load(Path.Combine(root,"docs","planning","source-baseline.json"));
    var manifest=Load(Path.Combine(root,"docs","planning","evidence","M0-W02-C02-transfer-manifest.json"));
    var backlog=Load(Path.Combine(root,"docs","planning","backlog.json"));
    Require(S(manifest,"source_revision")=="82a6b435a387a7e116b47a6b2c433ae9e067bf21","Transfer manifest source revision mismatch.");
    var aura=A(baseline,"repositories").Select(x=>x!.AsObject()).Single(x=>S(x,"repository")=="aura");
    Require(S(aura,"head")==S(manifest,"source_revision"),"AURA baseline head differs from transfer manifest.");
    var pinned=A(aura,"files").Select(x=>x!.AsObject()).ToDictionary(x=>S(x,"path"),x=>S(x,"sha256"),StringComparer.Ordinal);
    var packages=A(manifest,"package_identities").Select(x=>x!.AsObject()).ToArray();
    Require(packages.Select(x=>S(x,"package_id")).SequenceEqual(new[]{"dw.quantities","dw.quantities.expression","dw.quantities.standard"},StringComparer.Ordinal),"Inherited package identities changed.");
    foreach(var package in packages)
    {
        Require(S(package,"target_root").StartsWith("src/projects/",StringComparison.Ordinal),"Package target escapes Math roots.");
        if(S(package,"package_id")=="dw.quantities.standard") Require(S(package,"localization_dependency")=="none","dw.quantities.standard must not depend on dw.localization.");
    }
    var direct=A(manifest,"direct_transfer").Select(x=>x!.AsObject()).ToArray();
    Require(direct.Length==20,"Expected 20 direct source/test transfer entries.");
    foreach(var entry in direct)
    {
        var source=S(entry,"source");
        Require(pinned.TryGetValue(source,out var hash)&&hash==S(entry,"sha256"),"Pinned hash mismatch: "+source);
        Require(!source.StartsWith("lib/dw.localization/",StringComparison.Ordinal),"dw.localization must not be directly transferred.");
        Require(S(entry,"target").StartsWith(source.StartsWith("tests/",StringComparison.Ordinal)?"tests/projects/":"src/projects/",StringComparison.Ordinal),"Target outside Math roots: "+source);
    }
    var adapted=A(manifest,"adapted_math_behavior").Select(x=>x!.AsObject()).Single();
    Require(S(adapted,"source")=="src/aura.domains/core/aura.domains.core.math/UnitConverter.cs","UnitConverter source mismatch.");
    Require(pinned[S(adapted,"source")]==S(adapted,"sha256"),"UnitConverter hash mismatch.");
    Require(S(adapted,"action")=="adapt_pure_conversion_behavior","UnitConverter must be behavior adaptation.");
    var retained=A(manifest,"retained_in_aura").Select(x=>x!.AsObject()).ToArray();
    Require(retained.Length==8,"Expected eight host-localization files retained in AURA.");
    Require(retained.Count(x=>S(x,"source").StartsWith("lib/dw.localization/",StringComparison.Ordinal))==4,"All dw.localization files must remain outside Math transfer.");
    foreach(var entry in retained)
    {
        var source=S(entry,"source");
        Require(pinned.TryGetValue(source,out var hash)&&hash==S(entry,"sha256"),"Retained source hash mismatch: "+source);
        Require(S(entry,"action")=="retain_in_aura","Retained AURA action mismatch.");
    }
    var localization=manifest["localization_decision"]?.AsObject()??throw new InvalidOperationException("Missing localization_decision.");
    Require(S(localization,"selected_option")=="passive_bcl_metadata","Passive metadata option not selected.");
    Require(S(localization,"math_contract").Contains("no dependency on dw.localization",StringComparison.Ordinal),"Math contract still permits dw.localization.");
    var converter=manifest["unit_converter_decision"]?.AsObject()??throw new InvalidOperationException("Missing unit_converter_decision.");
    Require(converter["raw_file_copy_required"]?.GetValue<bool>()==false,"UnitConverter raw copy must not be required.");
    Require(S(converter,"aura_owns").Contains("culture",StringComparison.OrdinalIgnoreCase),"AURA host boundary lost culture ownership.");
    Require(S(FindById(backlog,"chunks","M0-W02-C02"),"status")=="done","M0-W02-C02 must be done.");
    Require(S(FindById(backlog,"external_dependencies","EXT-LOCALIZATION"),"status")=="satisfied","EXT-LOCALIZATION must be satisfied.");
    Require(S(FindById(backlog,"chunks","M0-W02-C03"),"status") is "ready" or "done","M0-W02-C03 must be ready or done after transfer-architecture qualification.");
    Console.WriteLine("transfer architecture valid: 3 packages; 20 direct transfer entries; 8 host-localization files retained; UnitConverter split qualified");
    return 0;
}
catch(Exception ex){Console.Error.WriteLine("transfer architecture invalid: "+ex.Message);return 1;}
static JsonObject Load(string path)=>JsonNode.Parse(File.ReadAllText(path))?.AsObject()??throw new InvalidOperationException("Invalid or empty JSON: "+path);
static JsonArray A(JsonObject o,string key)=>o[key] as JsonArray??throw new InvalidOperationException("Missing array "+key);
static string S(JsonObject o,string key)=>o[key]?.GetValue<string>() is {Length:>0} value?value:throw new InvalidOperationException("Missing string "+key);
static JsonObject FindById(JsonObject root,string arrayName,string id)=>A(root,arrayName).Select(x=>x!.AsObject()).Single(x=>S(x,"id")==id);
static void Require(bool value,string message){if(!value)throw new InvalidOperationException(message);}
