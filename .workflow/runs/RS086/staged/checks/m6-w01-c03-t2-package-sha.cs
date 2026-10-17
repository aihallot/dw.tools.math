using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.6";
if(args.Length!=1)throw new ArgumentException("Exactly one local Math audit archive is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-c03-t2-local-feed")
    throw new InvalidOperationException("Only qualified local .6 Math archive is admitted.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("Real net10 analysis assembly missing.");
    var specs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous NuGet identity.");
    using var item=specs[0].Open();
    var meta=XDocument.Load(item).Descendants().Single(e=>e.Name.LocalName=="metadata");
    if(meta.Elements().Single(e=>e.Name.LocalName=="id").Value!=Id||
       meta.Elements().Single(e=>e.Name.LocalName=="version").Value!=Version||
       meta.Descendants().Any(e=>e.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign NuGet identity or dependencies.");
}
if(bytes.Length==0)throw new InvalidOperationException("Empty local archive.");
var digest=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C03-T2-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+digest);
