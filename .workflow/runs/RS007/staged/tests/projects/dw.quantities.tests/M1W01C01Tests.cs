using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C01Tests
{
    public const string RedMarker = "M1-W01-C01 RED: Dw.Quantities.ExactRational is absent";

    [TestMethod]
    public void ExactRationalContractIsRedUntilTheAuthorizedTypeIsInstalled()
    {
        var assemblyPath = Path.Combine(AppContext.BaseDirectory, "dw.quantities.dll");
        Assert.IsTrue(File.Exists(assemblyPath), "RED harness invalid: dw.quantities.dll was not built.");

        var assembly = Assembly.LoadFrom(assemblyPath);
        var type = assembly.GetType("Dw.Quantities.ExactRational", throwOnError: false, ignoreCase: false);

        Assert.IsNotNull(type, RedMarker);
    }
}
