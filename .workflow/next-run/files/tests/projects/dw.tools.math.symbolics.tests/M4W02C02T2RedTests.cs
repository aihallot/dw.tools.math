using System.Collections.Generic;
using System.Reflection;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C02T2RedTests
{
    [TestMethod]
    public void PublicExactOriginalRootAuditMustExist()
    {
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType("Dw.Tools.Math.Symbolics.SymbolicRootAudit",
            throwOnError:false);
        var method=type?.GetMethod("Verify",BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrNode),typeof(IrSymbol),
                typeof(IEnumerable<ExactRational>)],
            modifiers:null);
        Assert.IsNotNull(method,
            "M4-W02-C02-T2 RED: exact original equation root audit API is absent.");
    }
}
