using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.6";
if(args.Length!=1)throw new ArgumentException("One exact locally generated Math package is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg" ||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-gate-local-feed")
    throw new InvalidOperationException("Only the isolated Math W01 gate archive is admitted.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("The actual net10 analysis binary is missing.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous local archive metadata.");
    using var stream=specs[0].Open();
    var metadata=XDocument.Load(stream).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign NuGet package dependency or identity.");
}
if(bytes.Length<=0)throw new InvalidOperationException("Empty local mathematical package.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-GATE-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
