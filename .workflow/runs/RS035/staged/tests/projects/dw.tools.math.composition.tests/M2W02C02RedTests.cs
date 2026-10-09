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
        // Match the exact original contracts: cancellation-aware overloads
        // must not make these reflection-based compatibility assertions ambiguous.
        Assert.IsNotNull(pipeline.GetMethod("Evaluate",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [typeof(IEnumerable<ExactPipelineStep>)],
            modifiers: null));
        Assert.IsNotNull(pipeline.GetMethod("Evaluate",
            BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            types: Type.EmptyTypes,
            modifiers: null));
    }
}
