using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.4";
if(args.Length!=1)throw new ArgumentException("One local Math .4 NuGet archive required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-c02-t2-local-feed")
    throw new InvalidOperationException("Unexpected local audit archive or feed.");
var bytes=File.ReadAllBytes(path);
using(var buffer=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(buffer,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("Missing real net10 analysis binary.");
    var specs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous nuspec count.");
    using var input=specs[0].Open();
    var metadata=XDocument.Load(input).Descendants().Single(e=>e.Name.LocalName=="metadata");
    if(metadata.Elements().Single(e=>e.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(e=>e.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(e=>e.Name.LocalName=="dependency"))
        throw new InvalidOperationException("NuGet identity/dependency contract drifted.");
}
if(bytes.Length==0)throw new InvalidOperationException("Empty local archive.");
var digest=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C02-T2-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+digest);
