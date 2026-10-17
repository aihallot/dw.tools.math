using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.5";
if(args.Length!=1)throw new ArgumentException("One local Math numerical package is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-c03-t1-local-feed")
    throw new InvalidOperationException("Unqualified package file or feed.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("No compiled net10 numerical analysis library.");
    var nuspec=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(nuspec.Length!=1)throw new InvalidOperationException("Ambiguous package identity.");
    using var stream=nuspec[0].Open();
    var metadata=XDocument.Load(stream).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign package version or dependency.");
}
if(bytes.Length==0)throw new InvalidOperationException("No actual package bytes.");
var digest=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C03-T1-PKG-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+digest);
