using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S02T1RedTests
{
    [TestMethod]
    public void BoundedDirectDftAndLinearConvolutionPublicContractMustExist()
    {
        var assembly = typeof(FiniteGeometry64).Assembly;
        var signal = assembly.GetType("Dw.Tools.Math.Scientific.FiniteSignal64");
        Assert.IsNotNull(signal,
            "M6-W02-C03-S02-T1 RED: direct finite DFT and linear convolution public facade is absent.");
        Assert.IsNotNull(signal!.GetMethod("DirectDft"));
        Assert.IsNotNull(signal.GetMethod("LinearConvolution"));
    }
}
