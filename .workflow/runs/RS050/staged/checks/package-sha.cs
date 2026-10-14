using System;
using System.IO;
using System.Security.Cryptography;

// This standalone post-packaging verifier hashes only the two generated
// NuGet product archives, never DWF workflow state, project plan or receipts.
// The C04 payload consumes these distinct observed product hashes as evidence.
const string Prefix = "C04-PKG-SHA256:";
string[] allowed =
[
    "dw.quantities.0.2.0-preview.1.nupkg",
    "dw.tools.math.statistics.0.4.0-preview.6.nupkg"
];

if (args.Length != allowed.Length)
    throw new ArgumentException("Exactly two qualified local NuGet archive paths are required.");

for (var i = 0; i < allowed.Length; i++)
{
    var path = Path.GetFullPath(args[i]);
    var filename = Path.GetFileName(path);
    if (!string.Equals(filename, allowed[i], StringComparison.Ordinal))
        throw new InvalidOperationException("Unexpected local NuGet package name: " + filename);
    if (!string.Equals(Path.GetFileName(Path.GetDirectoryName(path)),
            "m3-w02-c04-local-feed", StringComparison.Ordinal))
        throw new InvalidOperationException("Package is not in the bounded C04 local feed.");
    if (!File.Exists(path))
        throw new FileNotFoundException("Expected local NuGet package missing.", path);

    using var content = File.OpenRead(path);
    var digest = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
    Console.WriteLine(Prefix + filename + ":" + digest);
}
