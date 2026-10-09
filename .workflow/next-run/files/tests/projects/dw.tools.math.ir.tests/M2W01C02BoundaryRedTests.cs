using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C02BoundaryRedTests
{
    [TestMethod]
    public void PublicFactoriesMustBoundTotalDepth()
    {
        IrNode node = new IrExactScalar(ExactRational.One);
        for (var i = 0; i < 31; i++)
            node = IrApply.Create(IrOperation.Square, [node]);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => IrApply.Create(IrOperation.Square, [node]),
            "M2-W01-C02-T2 RED: nested graph depth over 32 is accepted.");
    }

    [TestMethod]
    public void PublicFactoriesMustBoundExpandedNodeBudget()
    {
        IrNode node = new IrExactScalar(ExactRational.One);
        for (var i = 0; i < 8; i++)
            node = IrApply.Create(IrOperation.Add, [node, node]);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => IrApply.Create(IrOperation.Add, [node, node]),
            "M2-W01-C02-T2 RED: shared DAG exceeds the 1024-node expanded budget.");
    }

    [TestMethod]
    public void MatrixShapeMustBeAnAdmittedPublicIRType()
    {
        var matrix = typeof(IrNode).Assembly.GetType("Dw.Tools.Math.Ir.IrMatrix");
        Assert.IsNotNull(matrix,
            "M2-W01-C02-T2 RED: typed bounded immutable matrix is absent.");
    }

    [TestMethod]
    public void SymbolDomainMustNotLeakAcrossSameTextIdentity()
    {
        var integer = IrSymbol.Free("x", "x", IrScalarDomain.Integer);
        var real = IrSymbol.Free("x", "x", IrScalarDomain.Real);
        var assumptions = IrAssumptionSet.Create([
            new IrRelation(integer, IrRelationKind.GreaterOrEqual,
                new IrExactScalar(ExactRational.Zero))]);
        Assert.IsFalse(assumptions.DeclaresNonNegative(real),
            "M2-W01-C02-T2 RED: positivity assumption leaks across symbol domains.");
    }
}
