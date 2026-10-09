using System.Reflection;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C03BoundaryRedTests
{
    [TestMethod]
    public void IncompleteReplayMustHaveASeparateNonFinalPublicContract()
    {
        var type = typeof(ExactReplayRunner);
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .SingleOrDefault(m => m.Name == "TryRun");
        Assert.IsNotNull(method,
            "M2-W02-C03-T2 RED: public explicit non-final replay status contract is missing.");
    }
}
