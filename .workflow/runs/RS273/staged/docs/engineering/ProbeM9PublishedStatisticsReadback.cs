#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Statistics = "dw.tools.math.statistics";
const string Version = "20260.20261120.1001";
const string ArchiveSha = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1";
const string DllSha = "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea";
const string BaseVersion = "20260.20261102.1001";
const string BaseSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string OldSha = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";
const string Checker = "docs/engineering/VerifyM9BoundedStatisticsPrivateFeed.cs";
void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS273: " + message);
}
string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
Require(OperatingSystem.IsWindows(), "Physical Windows NUGET_LOCAL feed required.");
var feed=Environment.GetEnvironmentVariable("NUGET_LOCAL");
Require(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
    Directory.Exists(feed), "Private package feed unavailable.");
string Archive(string id,string version) => Path.Combine(feed!,id+"."+version+".nupkg");
var published=Archive(Statistics,Version);
var old=Archive(Statistics,"20260.20261106.1001");
var dependency=Archive("dw.quantities",BaseVersion);
Require(File.Exists(published) && Hash(published)==ArchiveSha,
    "Published private Statistics SHA differs from operator publication receipt.");
Require(File.Exists(old) && Hash(old)==OldSha,
    "Immutable old Statistics archive differs.");
Require(File.Exists(dependency) && Hash(dependency)==BaseSha,
    "Immutable Quantities dependency differs.");
Require(File.Exists(Checker), "Original independent .NET 11 binary readback verifier is absent.");
var info=new ProcessStartInfo("dotnet")
{
    WorkingDirectory=Directory.GetCurrentDirectory(),
    UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true
};
foreach (var a in new[] { "run","--file",Checker,"--","--check",
    feed!,ArchiveSha,DllSha,BaseSha })
    info.ArgumentList.Add(a);
using var p=Process.Start(info) ??
    throw new InvalidOperationException("Unable to start independent readback.");
var output=p.StandardOutput.ReadToEnd();
var error=p.StandardError.ReadToEnd();
Require(p.WaitForExit(180000) && p.ExitCode==0 &&
    output.Contains("MATH-RS272-PRIVATE-READBACK|",StringComparison.Ordinal) &&
    output.Contains("|checks=12|",StringComparison.Ordinal) &&
    output.Contains("lockedRestore=true|PASS",StringComparison.Ordinal),
    "Published archive failed independent net11 binary readback:\n"+output+"\n"+error);
Require(Hash(published)==ArchiveSha && Hash(old)==OldSha && Hash(dependency)==BaseSha,
    "A published archive changed during independent readback.");
Console.WriteLine("MATH-RS273-PUBLISHED-STATISTICS|id="+Statistics+"|version="+Version+
    "|archiveSha256="+ArchiveSha+"|dllSha256="+DllSha+"|dependencySha256="+BaseSha+
    "|checks=12|lockedRestore=true|immutableHistory=true|source=physical-NUGET_LOCAL|PASS");
