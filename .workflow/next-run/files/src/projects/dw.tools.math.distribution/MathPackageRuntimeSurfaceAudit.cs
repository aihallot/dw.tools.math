using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Dw.Tools.Math.Distribution;

/// <summary>Observed managed assembly metadata is not cross-version or cross-OS certification.</summary>
public enum MathPackageSurfaceStatus
{
    Qualified,
    InvalidPackage,
    NativeAssetsNotQualified,
    NotManagedIlOnly,
    MissingRequiredPublicApi,
    ForeignRuntimeReference,
    BudgetExceeded,
    InvalidMetadata
}

/// <summary>
/// The public API digest fingerprints THIS archive only, without promising
/// source/binary compatibility with any unobserved prior or future version.
/// </summary>
public sealed record MathPackageSurfaceResult(
    MathPackageSurfaceStatus Status,
    MathPackageIdentity? Package,
    string? ManagedAssemblySha256,
    string? PublicApiSha256,
    int PublicTypes,
    int PublicMembers,
    string? RuleId,
    string ObservedHostRid,
    bool IlOnlyMetadataVerified,
    bool NativeAssetsPresent,
    bool OtherRidsCertified,
    bool CrossVersionSourceCompatibilityCertified,
    bool CrossVersionBinaryCompatibilityCertified,
    bool PerformanceSlaCertified)
{
    public bool IsQualified =>
        Status == MathPackageSurfaceStatus.Qualified &&
        Package is not null && ManagedAssemblySha256 is not null &&
        PublicApiSha256 is not null && PublicTypes > 0 && PublicMembers > 0 &&
        RuleId is not null && IlOnlyMetadataVerified && !NativeAssetsPresent &&
        !OtherRidsCertified && !CrossVersionSourceCompatibilityCertified &&
        !CrossVersionBinaryCompatibilityCertified && !PerformanceSlaCertified;
}

/// <summary>
/// Offline metadata-only verification for the already qualified seven-package
/// net10 Math graph. Never loads assemblies, invokes native code, extracts
/// packages or infers portable binary compatibility from a single host.
/// </summary>
public static class MathPackageRuntimeSurfaceAudit
{
    public const string Rule = "dw-math-managed-public-surface-and-rid-boundary/1";
    public const int MaximumManagedAssemblyBytes = 4_194_304;
    public const int MaximumPublicEntries = 10_000;
    public const int MaximumPublicSignatureBytes = 2_097_152;

    public static MathPackageSurfaceResult Audit(string packageId, byte[] archive)
    {
        ArgumentNullException.ThrowIfNull(packageId);
        ArgumentNullException.ThrowIfNull(archive);
        var host=RuntimeInformation.RuntimeIdentifier;
        var already=MathLocalReleaseQualification.Audit(packageId,archive);
        if(!already.IsQualified || already.Package is null)
            return Refuse(already.Status==MathPackageQualificationStatus.BudgetExceeded
                    ? MathPackageSurfaceStatus.BudgetExceeded
                    : MathPackageSurfaceStatus.InvalidPackage,host);
        var expected=already.Package;
        try
        {
            using var stream=new MemoryStream(archive,writable:false);
            using var zip=new ZipArchive(stream,ZipArchiveMode.Read,leaveOpen:false);
            if(zip.Entries.Any(entry=>
                entry.FullName.StartsWith("runtimes/",StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.StartsWith("native/",StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.EndsWith(".so",StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.EndsWith(".dylib",StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.EndsWith(".a",StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.EndsWith(".lib",StringComparison.OrdinalIgnoreCase)))
                return Refuse(MathPackageSurfaceStatus.NativeAssetsNotQualified,host);
            var dll=zip.GetEntry("lib/net10.0/"+packageId+".dll");
            if(dll is null || dll.Length<=0)
                return Refuse(MathPackageSurfaceStatus.InvalidPackage,host);
            if(dll.Length>MaximumManagedAssemblyBytes)
                return Refuse(MathPackageSurfaceStatus.BudgetExceeded,host);
            var dllBytes=new byte[checked((int)dll.Length)];
            using(var dllStream=dll.Open())
                dllStream.ReadExactly(dllBytes);
            using var pe=new PEReader(new MemoryStream(dllBytes,writable:false));
            var cor=pe.PEHeaders.CorHeader;
            // ECMA-335 II.25.3.3: COMIMAGE_FLAGS_ILONLY = bit 0.
            if(!pe.HasMetadata || cor is null || (((int)cor.Flags)&1)==0)
                return Refuse(MathPackageSurfaceStatus.NotManagedIlOnly,host);
            var reader=pe.GetMetadataReader();
            if(!reader.IsAssembly ||
                !string.Equals(reader.GetString(
                    reader.GetAssemblyDefinition().Name),packageId,StringComparison.Ordinal))
                return Refuse(MathPackageSurfaceStatus.InvalidMetadata,host);

            foreach(var referenceHandle in reader.AssemblyReferences)
            {
                var reference=reader.GetAssemblyReference(referenceHandle);
                var name=reader.GetString(reference.Name);
                if(!(name.StartsWith("System.",StringComparison.Ordinal) ||
                     name=="System" || name=="netstandard" ||
                     name=="Microsoft.CSharp" || name.StartsWith("dw.",StringComparison.Ordinal)))
                    return Refuse(MathPackageSurfaceStatus.ForeignRuntimeReference,host);
            }

            var surface=new List<string>();
            var callable=new HashSet<string>(StringComparer.Ordinal);
            var publicTypes=0;
            var publicMembers=0;
            var signatureBudget=0;
            foreach(var handle in reader.TypeDefinitions)
            {
                var definition=reader.GetTypeDefinition(handle);
                var visibility=definition.Attributes & TypeAttributes.VisibilityMask;
                if(visibility is not (TypeAttributes.Public or TypeAttributes.NestedPublic))
                    continue;
                publicTypes++;
                if(publicTypes>MaximumPublicEntries)
                    return Refuse(MathPackageSurfaceStatus.BudgetExceeded,host);
                var name=reader.GetString(definition.Namespace)+"."+
                    reader.GetString(definition.Name);
                surface.Add("T:"+name+":"+((int)definition.Attributes).ToString(
                    System.Globalization.CultureInfo.InvariantCulture));
                foreach(var methodHandle in definition.GetMethods())
                {
                    var method=reader.GetMethodDefinition(methodHandle);
                    if((method.Attributes & MethodAttributes.MemberAccessMask)!=
                        MethodAttributes.Public)
                        continue;
                    var methodName=reader.GetString(method.Name);
                    var signature=reader.GetBlobBytes(method.Signature);
                    signatureBudget+=signature.Length;
                    publicMembers++;
                    if(publicMembers>MaximumPublicEntries ||
                       signatureBudget>MaximumPublicSignatureBytes)
                        return Refuse(MathPackageSurfaceStatus.BudgetExceeded,host);
                    surface.Add("M:"+name+"::"+methodName+":"+
                        Convert.ToHexString(signature));
                    callable.Add(name+"::"+methodName);
                }
                foreach(var fieldHandle in definition.GetFields())
                {
                    var field=reader.GetFieldDefinition(fieldHandle);
                    if((field.Attributes & FieldAttributes.FieldAccessMask)!=
                        FieldAttributes.Public)
                        continue;
                    var signature=reader.GetBlobBytes(field.Signature);
                    signatureBudget+=signature.Length;
                    publicMembers++;
                    if(publicMembers>MaximumPublicEntries ||
                       signatureBudget>MaximumPublicSignatureBytes)
                        return Refuse(MathPackageSurfaceStatus.BudgetExceeded,host);
                    surface.Add("F:"+name+"::"+reader.GetString(field.Name)+":"+
                        Convert.ToHexString(signature));
                }
            }
            if(publicTypes==0 || publicMembers==0)
                return Refuse(MathPackageSurfaceStatus.MissingRequiredPublicApi,host);
            if(packageId=="dw.tools.math.distribution")
            {
                foreach(var required in new[]{
                    "Dw.Tools.Math.Distribution.MathLocalReleaseQualification::Audit",
                    "Dw.Tools.Math.Distribution.MathPackageRuntimeSurfaceAudit::Audit",
                    "Dw.Tools.Math.Distribution.ExactMathVerticalRunner::Run",
                    "Dw.Tools.Math.Distribution.ExactVerticalBoundaryAudit::Audit"})
                    if(!callable.Contains(required))
                        return Refuse(MathPackageSurfaceStatus.MissingRequiredPublicApi,host);
            }
            surface.Sort(StringComparer.Ordinal);
            var fingerprint=Convert.ToHexString(SHA256.HashData(
                Encoding.UTF8.GetBytes(string.Join("\n",surface))))
                .ToLowerInvariant();
            var assemblyHash=Convert.ToHexString(SHA256.HashData(dllBytes))
                .ToLowerInvariant();
            return new(MathPackageSurfaceStatus.Qualified,expected,
                assemblyHash,fingerprint,publicTypes,publicMembers,Rule,host,
                IlOnlyMetadataVerified:true,NativeAssetsPresent:false,
                OtherRidsCertified:false,
                CrossVersionSourceCompatibilityCertified:false,
                CrossVersionBinaryCompatibilityCertified:false,
                PerformanceSlaCertified:false);
        }
        catch(Exception error) when(error is BadImageFormatException or
            InvalidDataException or IOException or ArgumentException or
            InvalidOperationException or OverflowException)
        {
            return Refuse(MathPackageSurfaceStatus.InvalidMetadata,host);
        }
    }

    private static MathPackageSurfaceResult Refuse(
        MathPackageSurfaceStatus status,string host) =>
        new(status,null,null,null,0,0,null,host,false,false,false,false,false,false);
}
