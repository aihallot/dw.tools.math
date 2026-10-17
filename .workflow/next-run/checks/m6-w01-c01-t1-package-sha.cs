using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.1";
if(args.Length!=1)throw new ArgumentException("One locally generated Math analysis archive is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg" ||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-c01-t1-local-feed")
    throw new InvalidOperationException("Only versioned, isolated local Math analysis packages may be inspected.");
var archive=File.ReadAllBytes(path);
using(var memory=new MemoryStream(archive,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("No compiled net10 Math curve analysis assembly in archive.");
    var spec=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(spec.Length!=1)throw new InvalidOperationException("Exactly one nuspec expected.");
    using var item=spec[0].Open();
    var meta=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(meta.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id ||
       meta.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version ||
       meta.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Package identity or BCL-only dependency graph drift.");
}
if(archive.Length==0)throw new InvalidOperationException("No bytes in package.");
var digest=Convert.ToHexString(SHA256.HashData(archive)).ToLowerInvariant();
Console.WriteLine("M6-W01-C01-T1-PKG-SHA256:"+Id+":"+Version+":"+archive.Length+":"+digest);
