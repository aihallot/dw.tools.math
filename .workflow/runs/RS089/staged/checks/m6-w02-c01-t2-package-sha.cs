using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.2";
if(args.Length!=1)throw new ArgumentException("One versioned local scientific archive is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w02-c01-t2-local-feed")
    throw new InvalidOperationException("Unexpected local scientific archive path or version.");
var bytes=File.ReadAllBytes(path);
using(var buffer=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(buffer,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("No actual net10 scientific library.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous nuspec.");
    using var item=specs[0].Open();
    var meta=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(meta.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       meta.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       meta.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Unexpected source-free NuGet metadata.");
}
if(bytes.Length==0)throw new InvalidOperationException("Empty local scientific package.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C01-T2-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
