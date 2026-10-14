using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C02T2RedTests
{
    [TestMethod]
    public void PublicProviderOutputAdmissionMustRejectUnverifiedConditions()
    {
        // Compiled RED against the existing, real preview.1 symbolics product.
        // The missing public boundary is the test failure, not a C# compile
        // error or an absent NuGet package or missing .NET assembly.
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType(
            "Dw.Tools.Math.Symbolics.SymbolicProviderResultAdmission",
            throwOnError:false);
        var method=type?.GetMethod("TryAdmit",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(IrNode),typeof(object)],modifiers:null);
        Assert.IsNotNull(method,
            "M4-W01-C02-T2 RED: untrusted symbolic provider output gate is absent.");
    }
}
