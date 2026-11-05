using System.Security.Cryptography;
using Dw.Tools.Math.Foundation;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("Expected the original packaged DLL SHA-256.");
var count = 0;
void Probe(string name, Action action)
{
    try
    {
        action();
        count++;
        Console.WriteLine("MATH-FOUNDATION-CHECK|" + name + "|PASS");
    }
    catch (Exception error)
    {
        throw new InvalidOperationException("Independent Foundation probe: " + name, error);
    }
}
static void Need(bool okay, string reason)
{
    if (!okay) throw new InvalidOperationException(reason);
}

var assembly = typeof(FoundationContract).Assembly;
Probe("assembly-and-loaded-hash", () =>
{
    Need(assembly.GetName().Name == "dw.tools.math.foundation",
        "Loaded assembly has unexpected identity.");
    var hash = Convert.ToHexStringLower(
        SHA256.HashData(File.ReadAllBytes(assembly.Location)));
    Need(hash == args[0], "Loaded assembly differs from the actual nupkg DLL.");
});
Probe("package-id", () =>
    Need(FoundationContract.PackageId == "dw.tools.math.foundation",
        "Package ID constant was changed."));
Probe("contract-version", () =>
    Need(FoundationContract.ContractVersion == "0.1",
        "The original foundation contract version changed."));
Probe("qualified-identity", () =>
    Need(FoundationContract.Identity == "dw.tools.math.foundation/0.1",
        "The public foundation identity was not preserved."));
Probe("no-foreign-assembly-references", () =>
{
    var references = assembly.GetReferencedAssemblies()
        .Select(item => item.Name ?? "").ToArray();
    Need(!references.Any(name => new[] { "Aura", "Decision", "MCDM", "Angouri", "MathNet" }
            .Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase))),
        "Found an unapproved dependency on another product/provider.");
});
Need(count == 5, "Exactly five independent Foundation package probes required.");
Console.WriteLine("MATH-FOUNDATION-BINARY|PASS|5");
