using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.3";
if(args.Length!=1)throw new ArgumentException("Exactly one local Math archive required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-c02-t1-local-feed")
    throw new InvalidOperationException("Only pinned local univariate archive admitted.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("No real analysis binary found.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous NuGet metadata.");
    using var item=specs[0].Open();
    var metadata=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Unexpected NuGet dependencies or version.");
}
if(bytes.Length==0)throw new InvalidOperationException("Empty NuGet.");
Console.WriteLine("M6-W01-C02-T1-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant());
