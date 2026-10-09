using System.Collections.Immutable;
using System.Reflection;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C01BoundaryTests
{
    private static readonly Assembly CompositionAssembly = typeof(MathOperationResult).Assembly;
    private static MathResultProvenance Origin() =>
        MathResultProvenance.Create("matrix.inspect", "math-composition/1");

    private static ImmutableArray<MathOperationCapability> Discover()
    {
        var catalog = CompositionAssembly.GetType("Dw.Tools.Math.Composition.MathCapabilityCatalog");
        Assert.IsNotNull(catalog);
        var method = catalog.GetMethod("Discover", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(method);
        Assert.AreEqual(0, method.GetParameters().Length);
        Assert.AreEqual(typeof(ImmutableArray<MathOperationCapability>), method.ReturnType);
        return (ImmutableArray<MathOperationCapability>)method.Invoke(null, null)!;
    }

    [TestMethod]
    public void DiscoveryContractExistsWithoutProviderBinding()
    {
        var catalog = CompositionAssembly.GetType("Dw.Tools.Math.Composition.MathCapabilityCatalog");
        Assert.IsNotNull(catalog,
            "M2-W02-C01-T2 RED: public provider-independent capability discovery contract is absent.");
        var descriptors = Discover();
        Assert.AreEqual(1, descriptors.Length);
        Assert.AreEqual("matrix.inspect", descriptors[0].OperationId);
    }

    [TestMethod]
    public void RepeatedDiscoveryIsPureAndPermissionIndependent()
    {
        var first = Discover();
        var second = Discover();
        Assert.AreSame(MathOperationCapability.MatrixInspection, first[0]);
        Assert.AreSame(first[0], second[0]);
        Assert.IsTrue(first[0].Accepts(IrMatrix.Create(1, 1,
            [new IrExactScalar(new ExactRational(1, 1))])));
        Assert.IsFalse(first[0].Accepts(new IrExactScalar(new ExactRational(1, 1))));
    }

    [TestMethod]
    public void UnknownCapabilityDoesNotProduceFallbackProvider()
    {
        var catalog = CompositionAssembly.GetType("Dw.Tools.Math.Composition.MathCapabilityCatalog")!;
        var find = catalog.GetMethod("Find", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(find);
        Assert.AreEqual(typeof(MathOperationCapability), find.ReturnType);
        Assert.IsNull(find.Invoke(null, ["matrix.solve"]));
        Assert.IsNull(find.Invoke(null, [null]));
        Assert.AreSame(MathOperationCapability.MatrixInspection, find.Invoke(null, ["matrix.inspect"]));
    }

    [TestMethod]
    public void PublicTypesCannotExposeProviderOrAuthorizationSurface()
    {
        var types = CompositionAssembly.GetExportedTypes();
        Assert.IsTrue(types.Length >= 5);
        foreach (var type in types)
        {
            foreach (var forbidden in new[] { "Provider", "Aura", "Permission", "Authorization", "Dispatcher" })
                Assert.IsFalse(type.Name.Contains(forbidden, StringComparison.OrdinalIgnoreCase),
                    "Unexpected public type: " + type.FullName);
            foreach (var method in type.GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                Assert.IsFalse(method.Name.StartsWith("Execute", StringComparison.OrdinalIgnoreCase),
                    "Unexpected public execution method: " + method);
                var signature = method.ReturnType.FullName ?? method.ReturnType.Name;
                foreach (var parameter in method.GetParameters())
                    signature += ";" + (parameter.ParameterType.FullName ?? parameter.ParameterType.Name);
                foreach (var forbidden in new[] { "Provider", "Aura", "Permission", "Authorization", "Executor" })
                    Assert.IsFalse(signature.Contains(forbidden, StringComparison.OrdinalIgnoreCase),
                        "Unexpected public signature: " + method);
            }
        }
    }

    [TestMethod]
    public void FailureStatusesHaveExplicitAbsenceAndBoundedReasons()
    {
        foreach (var result in new[] {
            MathOperationResult.Unsupported("no public solver", Origin()),
            MathOperationResult.BudgetExceeded("matrix element limit reached", Origin()) })
        {
            Assert.IsFalse(result.HasFinalValue);
            Assert.IsNull(result.Value);
            Assert.IsNotNull(result.Reason);
            Assert.IsTrue(result.Reason.Length <= 256);
            Assert.AreEqual("math-composition/1", result.Provenance.ContractVersion);
        }
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            MathOperationResult.Unsupported(new string('x', 257), Origin()));
    }

    [TestMethod]
    public void SuccessfulResultRequiresAnAdmittedTypedValue()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            MathOperationResult.Exact(null!, Origin()));
        Assert.ThrowsExactly<NotSupportedException>(() =>
            MathOperationResult.Exact(IrSymbol.Free("x", "x"), Origin()));
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathOperationResult.Approximate(new IrExactScalar(new ExactRational(1, 1)), Origin()));
        var value = new IrExactScalar(new ExactRational(3, 1));
        var result = MathOperationResult.Exact(value, Origin());
        Assert.IsTrue(result.HasFinalValue);
        Assert.AreSame(value, result.Value);
        Assert.IsNull(result.Reason);
    }

    [TestMethod]
    public void ResultsCannotBeConstructedPubliclyWithInventedFinalValues()
    {
        Assert.AreEqual(0, typeof(MathOperationResult).GetConstructors().Length);
        var names = typeof(MathOperationResult).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray();
        var expected = new[] { "HasFinalValue", "Provenance", "Reason", "Status", "Value" };
        CollectionAssert.AreEqual(expected, names);
    }

    [TestMethod]
    public void ProvenanceIsNotAnExecutionTraceOrPermissionToken()
    {
        var origin = Origin();
        Assert.AreEqual("matrix.inspect", origin.OperationId);
        Assert.AreEqual("math-composition/1", origin.ContractVersion);
        Assert.AreEqual(0, typeof(MathResultProvenance).GetConstructors().Length);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            MathResultProvenance.Create(new string('z', 129), "math-composition/1"));
        var properties = typeof(MathResultProvenance).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        Assert.AreEqual(2, properties.Length);
        Assert.IsTrue(properties.Any(p => p.Name == "OperationId"));
        Assert.IsTrue(properties.Any(p => p.Name == "ContractVersion"));
    }
}
