using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

// Separate post-restore verifier for the ONE observed AngouriMath 2.5.0
// NuGet input. Never touches Math or DWF workflow hashes.
const string ExpectedSha="8f194bf9ee09cc0514bac5894d413bb8c7e6c6f01071aa8274e24718bf855419";
const string Version="2.5.0";
if(args.Length!=1)throw new ArgumentException("Exactly one pinned AngouriMath package archive required.");
var expectedPath=Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
    "build","artifacts","m4-w01-c01-t2-packages",
    "angourimath",Version,"angourimath."+Version+".nupkg"));
var supplied=Path.GetFullPath(args[0]);
if(!string.Equals(expectedPath,supplied,StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Provider archive is not the fixed T2 isolated NuGet input.");
if(!File.Exists(supplied))throw new FileNotFoundException("Observed provider archive unavailable.",supplied);

using(var zip=ZipFile.OpenRead(supplied))
{
    var nuspec=zip.Entries.Single(e=>e.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase));
    using var stream=nuspec.Open();
    var metadata=XDocument.Load(stream).Descendants().Single(e=>e.Name.LocalName=="metadata");
    var id=metadata.Elements().Single(e=>e.Name.LocalName=="id").Value;
    var version=metadata.Elements().Single(e=>e.Name.LocalName=="version").Value;
    var license=metadata.Elements().Single(e=>e.Name.LocalName=="license").Value;
    var dependencies=metadata.Descendants().Where(e=>e.Name.LocalName=="dependency")
        .Select(e=>(string?)e.Attribute("id")??"")
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
    var expectedDependencies=new[]{
        "Antlr4.Runtime.Standard","GenericTensor","HonkSharp","PeterO.Numbers","System.Memory"
    }.OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).ToArray();
    if(id!="AngouriMath"||version!=Version||license!="MIT"||
       !dependencies.SequenceEqual(expectedDependencies,StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException("Pinned provider nuspec/license/dependency identity changed.");
}
using var bytes=File.OpenRead(supplied);
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
if(sha!=ExpectedSha)
    throw new InvalidOperationException("AngouriMath 2.5.0 bytes drifted from the observed RS053 archive.");
Console.WriteLine("M4-T2-ANGOURI-ARCHIVE:"+Version+":"+sha+":MIT:deps=5");
