using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

// Reads only NuGet third-party package archives fetched into isolated local
// caches. Does not read or hash DWF workflow, planning or repository authority.
var expected = new (string Name, string Version, string Cache)[]
{
    ("AngouriMath","2.5.0","m4-w01-c01-angouri-packages"),
    ("MathNet.Symbolics","0.25.0","m4-w01-c01-mathnet-packages")
};
if(args.Length!=2)throw new ArgumentException("Exactly two fixed symbolic candidate archive paths required.");

for(var i=0;i<expected.Length;i++)
{
    var item=expected[i];
    var path=Path.GetFullPath(args[i]);
    var canonicalName=item.Name.ToLowerInvariant();
    if(Path.GetFileName(path)!=canonicalName+"."+item.Version+".nupkg"||
       Path.GetFileName(Path.GetDirectoryName(path))!=item.Version||
       Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path)))!=canonicalName||
       !path.Contains(item.Cache,StringComparison.Ordinal))
        throw new InvalidOperationException("Unexpected symbolic NuGet artifact cache or identity.");
    using var archive=ZipFile.OpenRead(path);
    var metadataFile=archive.Entries.Single(entry=>entry.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase));
    using var metadataStream=metadataFile.Open();
    var doc=XDocument.Load(metadataStream);
    var metadata=doc.Descendants().Single(e=>e.Name.LocalName=="metadata");
    var foundId=metadata.Elements().Single(e=>e.Name.LocalName=="id").Value;
    var foundVersion=metadata.Elements().Single(e=>e.Name.LocalName=="version").Value;
    var license=metadata.Elements().SingleOrDefault(e=>e.Name.LocalName=="license")?.Value ?? "unattested";
    if(foundId!=item.Name||foundVersion!=item.Version||license!="MIT")
        throw new InvalidOperationException("Expected symbolic candidate MIT SPDX package metadata was not observed.");
    var deps=metadata.Descendants().Where(e=>e.Name.LocalName=="dependency")
        .Select(e=>(string?)e.Attribute("id")??"").Where(n=>n.Length>0)
        .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(n=>n,StringComparer.OrdinalIgnoreCase).ToArray();
    if(deps.Length<1)
        throw new InvalidOperationException("Actual external candidate dependency metadata is absent.");
    if(i==1 && !deps.Contains("MathNet.Numerics",StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("The Math.NET Symbolics numerical transitive boundary was not recorded.");
    // The archive bytes are from a NuGet restore, not an invented reproducible
    // cross-OS build. Identifies only this actually obtained third-party input.
    using var input=File.OpenRead(path);
    var sha=Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant();
    var bytes=new FileInfo(path).Length;
    Console.WriteLine("M4-PROVIDER-ARCHIVE:"+item.Name+":"+item.Version+":"+
        bytes.ToString(CultureInfo.InvariantCulture)+":"+sha+":"+license+":"+string.Join(";",deps));
}
