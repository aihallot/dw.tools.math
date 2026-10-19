using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.10";
if(args.Length!=1)throw new ArgumentException("One local Math scientific boundary archive required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w02-c03-s03-t2-local-feed")
    throw new InvalidOperationException("Unqualified scientific archive path or identity.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("Missing actual net10 scientific assembly.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous scientific nuspec.");
    using var item=specs[0].Open();
    var meta=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(meta.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       meta.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       meta.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign package dependencies or identity.");
}
if(bytes.Length==0)throw new InvalidOperationException("Empty scientific NuGet archive.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C03-S03-T2-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
