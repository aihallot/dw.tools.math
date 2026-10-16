using Dw.Tools.Math.Distribution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C02T1RedTests
{
    [TestMethod]
    public void IndependentBoundedLocalPackageAuditPublicContractMustExist()
    {
        var assembly=typeof(ExactMathVerticalRunner).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Distribution.MathLocalReleaseQualification");
        Assert.IsNotNull(audit,
            "M5-W02-C02-T1 RED: public bounded local NuGet qualification contract is absent.");
        Assert.IsNotNull(audit!.GetMethod("Audit"));
    }
}
