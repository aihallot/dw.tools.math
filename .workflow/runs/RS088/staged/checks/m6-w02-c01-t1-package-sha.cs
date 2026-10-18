using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.1";
if(args.Length!=1)throw new ArgumentException("Exactly one local scientific archive is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg" ||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w02-c01-t1-local-feed")
    throw new InvalidOperationException("Unexpected exact-science package identity or feed.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var archive=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(archive.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing actual net10 scientific assembly.");
    var specs=archive.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous NuGet metadata.");
    using var item=specs[0].Open();
    var metadata=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id ||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version ||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Unexpected NuGet dependencies or version drift.");
}
if(bytes.Length==0)throw new InvalidOperationException("Local NuGet bytes absent.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C01-T1-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
