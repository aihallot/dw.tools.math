using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C03RedTests
{
    [TestMethod]
    public void ReplayContractMustExposeDeterministicIdentityWithoutAProvider()
    {
        var contract = typeof(ExactQuantityPipeline).Assembly.GetType(
            "Dw.Tools.Math.Composition.ExactReplayRunner");
        Assert.IsNotNull(contract,
            "M2-W02-C03-T1 RED: exact canonical replay identity contract is missing.");
        Assert.IsTrue(contract.IsAbstract && contract.IsSealed,
            "Replay contract must be a static local entrypoint, not provider execution.");
    }
}
