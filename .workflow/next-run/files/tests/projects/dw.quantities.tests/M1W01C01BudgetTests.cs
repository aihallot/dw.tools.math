using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C01BudgetTests
{
    [TestMethod]
    public void ExplicitBudgetPreservesExactFractionResults()
    {
        var a = new ExactRational(1,3);
        var b = new ExactRational(1,6);
        Assert.AreEqual(new ExactRational(1,2), ExactRationalBudget.Add(a,b,64));
        Assert.AreEqual(new ExactRational(1,18), ExactRationalBudget.Multiply(a,b,64));
    }

    [TestMethod]
    public void ExplicitBudgetRejectsOversizedInputAndIntermediate()
    {
        var huge = new ExactRational(BigInteger.Pow(2,128),3);
        Assert.ThrowsExactly<ArithmeticException>(() => ExactRationalBudget.Add(huge, ExactRational.One,64));
        var a = new ExactRational(BigInteger.Pow(2,40),1);
        Assert.ThrowsExactly<ArithmeticException>(() => ExactRationalBudget.Multiply(a,a,64));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ExactRationalBudget.Add(ExactRational.One,ExactRational.One,0));
    }

    [TestMethod]
    public void CanonicalDefaultAndUnbudgetedHugeValuesRemainExact()
    {
        Assert.AreEqual(ExactRational.Zero, default(ExactRational));
        var large = BigInteger.Pow(2,256);
        Assert.AreEqual(new ExactRational(1,large),
            new ExactRational(large+1,large) - ExactRational.One);
    }
}
