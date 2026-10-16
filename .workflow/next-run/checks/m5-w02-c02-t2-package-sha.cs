using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

// This independent product-only package inspector hashes only seven generated NuGet
// product archives. DWF retains exclusive authority for workflow hash receipts.
var packages=new (string Id,string Version,string[] Dependencies)[]
{
    ("dw.quantities","0.2.0-preview.1",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
    ("dw.tools.math.codecs","1.0.0-preview.8",["dw.tools.math.ir"]),
    ("dw.tools.math.symbolics","0.5.0-preview.10",["dw.tools.math.ir"]),
    ("dw.tools.math.numerics","0.4.0-preview.4",["dw.tools.math.ir"]),
    ("dw.tools.math.composition","0.3.0-preview.1",["dw.quantities","dw.tools.math.ir"]),
    ("dw.tools.math.distribution","1.0.0-preview.4",
        ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
         "dw.tools.math.symbolics","dw.tools.math.numerics",
         "dw.tools.math.composition"])
};
if(args.Length!=packages.Length)
    throw new ArgumentException("Exactly seven bounded product archive paths are required.");

for(var i=0;i<packages.Length;i++)
{
    var (id,version,expectedDeps)=packages[i];
    var path=Path.GetFullPath(args[i]);
    if(Path.GetFileName(path)!=id+"."+version+".nupkg"||
       Path.GetFileName(Path.GetDirectoryName(path))!="m5-w02-c02-t2-local-feed")
        throw new InvalidOperationException("Archive path, version or local feed is not the admitted product boundary.");
    if(!File.Exists(path))
        throw new FileNotFoundException("Required generated NuGet package missing.",path);
    using(var zip=ZipFile.OpenRead(path))
    {
        if(zip.GetEntry("lib/net10.0/"+id+".dll") is null)
            throw new InvalidOperationException("No .NET 10 product assembly: "+id);
        var nuspecs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
            StringComparison.OrdinalIgnoreCase)).ToArray();
        if(nuspecs.Length!=1)
            throw new InvalidOperationException("Exactly one product nuspec required.");
        using var stream=nuspecs[0].Open();
        var metadata=XDocument.Load(stream).Descendants()
            .Single(e=>e.Name.LocalName=="metadata");
        var actualId=metadata.Elements().Single(e=>e.Name.LocalName=="id").Value;
        var actualVersion=metadata.Elements().Single(e=>e.Name.LocalName=="version").Value;
        var directDeps=metadata.Descendants().Where(e=>e.Name.LocalName=="dependency")
            .Select(e=>(string?)e.Attribute("id")??"")
            .OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
        if(actualId!=id||actualVersion!=version||
           !directDeps.SequenceEqual(expectedDeps.OrderBy(x=>x,StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("NuGet identity/dependency graph drift for "+id);
        if(id=="dw.tools.math.distribution" &&
            (zip.GetEntry("readme.md") is not { Length: > 0 } ||
             zip.GetEntry("NOTICE.md") is not { Length: > 0 } ||
             !metadata.Elements().Any(e=>e.Name.LocalName=="readme" &&
                 e.Value=="readme.md")))
            throw new InvalidOperationException("Distribution nupkg lacks README/NOTICE.");
        if(directDeps.Any(x=>x.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
            x.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            x.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
            x.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
            x.Contains("MCDM",StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("An external engine leaked into a Math product.");
    }
    using var bytes=File.OpenRead(path);
    var digest=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    var length=new FileInfo(path).Length;
    if(length<=0||digest.Length!=64)
        throw new InvalidOperationException("Invalid generated NuGet archive receipt.");
    Console.WriteLine("M5-W02-C02-T2-PKG-SHA256:"+id+":"+version+":"+
        length.ToString(CultureInfo.InvariantCulture)+":"+digest);
}
