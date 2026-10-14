using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

// Independent, post-build inspection of four fixed Math product archives.
// It never reads or rehashes DWF state, requests, plans or workflow receipts.
var admitted=new (string Id,string Version,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.numerics","0.4.0-preview.4",["dw.tools.math.ir"]),
    ("dw.tools.math.statistics","0.4.0-preview.6",["dw.quantities"])
};
if(args.Length!=admitted.Length)
    throw new ArgumentException("Four and only four local M3 NuGet paths are required.");

for(var i=0;i<admitted.Length;i++)
{
    var (id,version,deps)=admitted[i];
    var path=Path.GetFullPath(args[i]);
    if(!string.Equals(Path.GetFileName(path),id+"."+version+".nupkg",StringComparison.Ordinal)||
       !string.Equals(Path.GetFileName(Path.GetDirectoryName(path)),
           "m3-gate-local-feed",StringComparison.Ordinal))
        throw new InvalidOperationException("M3 archive outside fixed gate identity/feed.");
    if(!File.Exists(path))
        throw new FileNotFoundException("Missing M3 gate package.",path);
    using(var archive=ZipFile.OpenRead(path))
    {
        if(archive.GetEntry("lib/net10.0/"+id+".dll") is null)
            throw new InvalidOperationException("Missing managed net10.0 assembly: "+id);
        var manifest=archive.Entries.Single(x=>x.FullName.EndsWith(".nuspec",
            StringComparison.OrdinalIgnoreCase));
        using var stream=manifest.Open();
        var xml=XDocument.Load(stream);
        var metadata=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
        var identity=metadata.Elements().Single(x=>x.Name.LocalName=="id").Value;
        var observedVersion=metadata.Elements().Single(x=>x.Name.LocalName=="version").Value;
        var actualDeps=metadata.Descendants().Where(x=>x.Name.LocalName=="dependency")
            .Select(x=>(string?)x.Attribute("id")??"").OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
        if(identity!=id||observedVersion!=version||
           !actualDeps.SequenceEqual(deps.OrderBy(x=>x,StringComparer.OrdinalIgnoreCase),
               StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("NuGet package dependency and identity mismatch for "+id);
        if(actualDeps.Any(x=>x.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            x.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
            x.Contains("Decision",StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Provider/broker leaked into M3 package manifest.");
    }
    using var bytes=File.OpenRead(path);
    var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    var count=new FileInfo(path).Length;
    if(count<=0||sha.Length!=64)
        throw new InvalidOperationException("Invalid product archive size/hash.");
    Console.WriteLine("M3-PKG-SHA256:"+id+":"+version+":"+
        count.ToString(CultureInfo.InvariantCulture)+":"+sha);
}
