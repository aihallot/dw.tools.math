using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

// Post-build helper observes only the three fixed local product archives.
// DWF alone owns workflow, planning and evidence hash authority.
var expected=new (string Id,string Version,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.symbolics","0.5.0-preview.1",["dw.tools.math.ir"])
};
if(args.Length!=expected.Length)
    throw new ArgumentException("Exactly three pinned local Math package paths required.");
for(var i=0;i<expected.Length;i++)
{
    var (id,version,deps)=expected[i];
    var archive=Path.GetFullPath(args[i]);
    if(Path.GetFileName(archive)!=id+"."+version+".nupkg"||
       Path.GetFileName(Path.GetDirectoryName(archive))!="m4-w01-c02-t1-local-feed")
        throw new InvalidOperationException("Product archive identity or scoped feed changed.");
    if(!File.Exists(archive))throw new FileNotFoundException("Expected Math package unavailable.",archive);
    using(var zip=ZipFile.OpenRead(archive))
    {
        if(zip.GetEntry("lib/net10.0/"+id+".dll") is null)
            throw new InvalidOperationException("No compiled net10.0 library in "+id);
        var nuspecs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
        if(nuspecs.Length!=1)throw new InvalidOperationException("Unexpected nuspec count for "+id);
        using var stream=nuspecs[0].Open();
        var metadata=XDocument.Load(stream).Descendants().Single(e=>e.Name.LocalName=="metadata");
        var actualId=metadata.Elements().Single(e=>e.Name.LocalName=="id").Value;
        var actualVersion=metadata.Elements().Single(e=>e.Name.LocalName=="version").Value;
        var actualDeps=metadata.Descendants().Where(e=>e.Name.LocalName=="dependency")
            .Select(e=>(string?)e.Attribute("id")??"")
            .OrderBy(s=>s,StringComparer.OrdinalIgnoreCase).ToArray();
        if(actualId!=id||actualVersion!=version||
           !actualDeps.SequenceEqual(deps.OrderBy(s=>s,StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Unexpected package ID/version/dependency graph: "+id);
        if(actualDeps.Any(d=>d.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
           d.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
           d.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
           d.Contains("Decision",StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("External provider or broker leaked into public package "+id);
    }
    using var stream2=File.OpenRead(archive);
    var digest=Convert.ToHexString(SHA256.HashData(stream2)).ToLowerInvariant();
    var bytes=new FileInfo(archive).Length;
    if(bytes<=0||digest.Length!=64)
        throw new InvalidOperationException("Invalid package byte identity: "+id);
    Console.WriteLine("M4-IR-PKG-SHA256:"+id+":"+version+":"+
        bytes.ToString(CultureInfo.InvariantCulture)+":"+digest);
}
