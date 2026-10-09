using System.Reflection;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C02RedTests
{
    [TestMethod]
    public void ExactQuantityPipelineMustExposeOneSharedDirectAndFluentEvaluator()
    {
        var pipeline = typeof(MathOperationResult).Assembly.GetType(
            "Dw.Tools.Math.Composition.ExactQuantityPipeline");
        Assert.IsNotNull(pipeline,
            "M2-W02-C02-T1 RED: public exact quantity pipeline direct/fluent contract is absent.");
        Assert.IsNotNull(pipeline.GetMethod("Evaluate",
            BindingFlags.Public | BindingFlags.Static));
        Assert.IsNotNull(pipeline.GetMethod("Evaluate",
            BindingFlags.Public | BindingFlags.Instance));
    }
}
