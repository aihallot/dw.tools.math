#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;

if (args.Length != 1 || args[0] is not ("--red" or "--green"))
    throw new ArgumentException("Expected --red or --green.");
string[] sources = [
    "src/projects/dw.quantities/ExactRational.cs",
    "src/projects/dw.quantities/ExactRationalBudget.cs",
    "docs/engineering/fixtures/M11ExactInt64ScalingOracle.cs"
];
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
var hashes = sources.ToDictionary(x => x, Hash, StringComparer.Ordinal);
var root = Path.GetFullPath(Path.Combine("build","artifacts","m11-int64-" + Guid.NewGuid().ToString("N")[..12]));
Directory.CreateDirectory(root);
File.WriteAllText(Path.Combine(root,"global.json"), "{\"sdk\":{\"version\":\"10.0.401\",\"rollForward\":\"latestPatch\",\"allowPrerelease\":false}}\n");
File.WriteAllText(Path.Combine(root,"Directory.Build.props"), "<Project><PropertyGroup><TargetFramework>net10.0</TargetFramework><LangVersion>14.0</LangVersion><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><BaseOutputPath>out/bin/</BaseOutputPath><BaseIntermediateOutputPath>out/obj/</BaseIntermediateOutputPath></PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root,"Directory.Build.targets"), "<Project />\n");
File.WriteAllText(Path.Combine(root,"Directory.Packages.props"), "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>\n");
File.WriteAllText(Path.Combine(root,"NuGet.Config"), "<configuration><packageSources><clear /></packageSources></configuration>\n");
File.WriteAllText(Path.Combine(root,"oracle.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><NuGetAudit>false</NuGetAudit></PropertyGroup></Project>\n");
File.Copy(sources[0],Path.Combine(root,"ExactRational.cs"));
File.Copy(sources[1],Path.Combine(root,"ExactRationalBudget.cs"));
File.Copy(sources[2],Path.Combine(root,"Program.cs"));
Run("restore","oracle.csproj","--configfile","NuGet.Config");
Run("build","oracle.csproj","-c","Release","--no-restore");
var output = Run("run","--project","oracle.csproj","-c","Release","--no-build","--no-restore","--",args[0]);
var marker = args[0] == "--red" ? "M11-INT64-RED|missingMember=true|fixedOracleCases=25|historicalArithmetic=PASS|PASS" : "M11-INT64-GREEN|fixed=25|";
if (!output.Contains(marker,StringComparison.Ordinal)) throw new InvalidDataException("Expected oracle marker missing.");
foreach(var pair in hashes)
    if(Hash(pair.Key)!=pair.Value) throw new InvalidDataException("Input changed: "+pair.Key);
Console.WriteLine("MATH-RS372-EXACT-INT64|mode="+args[0]+"|sourceMutation=false|PASS");

string Run(params string[] arguments) {
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory=root,UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true };
    foreach(var argument in arguments) info.ArgumentList.Add(argument);
    using var p=Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable.");
    var stdout=p.StandardOutput.ReadToEndAsync();
    var stderr=p.StandardError.ReadToEndAsync();
    if(!p.WaitForExit(120000)) { p.Kill(entireProcessTree:true); throw new TimeoutException("Oracle command timed out."); }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    if(p.ExitCode!=0) throw new InvalidDataException("Oracle command failed.");
    return output;
}
