using Dw.Tools.Math.Foundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Dw.Tools.Math.Foundation.Tests;

[TestClass]
public sealed class M0W01C01Tests
{
    [TestMethod]
    public void FoundationContractIdentityIsStable()
    {
        Assert.AreEqual("dw.tools.math.foundation/0.1", FoundationContract.Identity);
    }
}
