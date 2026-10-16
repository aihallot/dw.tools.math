using Dw.Tools.Math.Distribution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C02T2RedTests
{
    [TestMethod]
    public void PublicManagedSurfaceAndRuntimeBoundaryAuditMustExist()
    {
        var assembly=typeof(MathLocalReleaseQualification).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Distribution.MathPackageRuntimeSurfaceAudit");
        Assert.IsNotNull(audit,
            "M5-W02-C02-T2 RED: bounded managed API and native/RID surface audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("Audit"));
    }
}
