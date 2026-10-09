using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C02BoundaryTests
{
    [TestMethod]
    public void Depth32AcceptedAndDepth33Rejected()
    {
        IrNode node = new IrExactScalar(ExactRational.One);
        for (var i = 0; i < 31; i++) node = IrApply.Create(IrOperation.Square, [node]);
        IrGraphLimits.Validate(node);
        var atLimit = node;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => IrApply.Create(IrOperation.Square, [atLimit]));
    }

    [TestMethod]
    public void ExpandedDagCountsSharedNodesEachUse()
    {
        IrNode node = new IrExactScalar(ExactRational.One);
        for (var i = 0; i < 9; i++) node = IrApply.Create(IrOperation.Add, [node, node]);
        IrGraphLimits.Validate(node);
        var atLimit = node;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => IrApply.Create(IrOperation.Add, [atLimit, atLimit]));
    }

    [TestMethod]
    public void RectangularMatrixPreservesValuesWithoutAliasing()
    {
        var cells = new List<IrScalar> { Exact(1), Exact(2), Exact(3), Exact(4) };
        var matrix = IrMatrix.Create(2, 2, cells);
        cells[0] = Exact(99);
        Assert.AreEqual(2, matrix.Rows);
        Assert.AreEqual(2, matrix.Columns);
        Assert.AreEqual(new ExactRational(1,1), ((IrExactScalar)matrix.At(0,0)).Value);
        Assert.AreEqual(new ExactRational(4,1), ((IrExactScalar)matrix.At(1,1)).Value);
        IrGraphLimits.Validate(matrix);
    }

    [TestMethod]
    public void MatrixRejectsShapesAndMixedNumericKinds()
    {
        var a = Exact(1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrMatrix.Create(0,2,[a,a]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => IrMatrix.Create(int.MaxValue,2,[a]));
        Assert.ThrowsExactly<ArgumentException>(() => IrMatrix.Create(2,2,[a,a,a]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            IrMatrix.Create(1,2,[a,IrFiniteBinary64Scalar.FromDouble(1.0)]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            IrMatrix.Create(1,4097,Enumerable.Repeat<IrScalar>(a,4097)));
    }

    [TestMethod]
    public void MatrixLocalLimitAndGlobalLimitBothApply()
    {
        var scalar = Exact(1);
        var max = IrMatrix.Create(1,1023,Enumerable.Repeat<IrScalar>(scalar,1023));
        Assert.AreEqual(1023,max.Elements.Length);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            IrMatrix.Create(1,1024,Enumerable.Repeat<IrScalar>(scalar,1024)));
    }

    [TestMethod]
    public void AssumptionsCannotCrossSymbolDomainsOrBindingScopes()
    {
        var integer = IrSymbol.Free("x","x",IrScalarDomain.Integer);
        var real = IrSymbol.Free("x","x",IrScalarDomain.Real);
        var bound = IrSymbol.Bound("scope-a",0,"x");
        var guard = new IrRelation(integer,IrRelationKind.GreaterOrEqual,Exact(0));
        var assumptions = IrAssumptionSet.Create([guard]);
        Assert.IsTrue(assumptions.DeclaresNonNegative(integer));
        Assert.IsFalse(assumptions.DeclaresNonNegative(real));
        Assert.IsFalse(assumptions.DeclaresNonNegative(bound));
        var sqrt = IrApply.Create(IrOperation.Sqrt,[IrApply.Create(IrOperation.Square,[real])]);
        Assert.IsTrue(IrRealDomainRules.NormalizeSqrtOfSquare(sqrt,assumptions)
            is IrApply { Operation: IrOperation.Abs });
    }

    [TestMethod]
    public void DuplicatedAssumptionsAndOversizedArraysAreRejected()
    {
        var x = IrSymbol.Free("x","x");
        var relation = new IrRelation(x,IrRelationKind.NotEqual,Exact(1));
        Assert.ThrowsExactly<ArgumentException>(() => IrAssumptionSet.Create([relation,relation]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            IrAssumptionSet.Create(Enumerable.Repeat(relation,33)));
    }

    [TestMethod]
    public void RestrictedExpressionAlsoEnforcesDepth()
    {
        IrNode node = IrSymbol.Free("x","x");
        for(var i=0;i<31;i++) node=new IrRestrictedExpression(node,IrAssumptionSet.Empty);
        IrGraphLimits.Validate(node);
        var atLimit=node;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new IrRestrictedExpression(atLimit,IrAssumptionSet.Empty));
    }

    [TestMethod]
    public void OriginalExactAndFinitePoliciesRemainDistinct()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            IrFiniteBinary64Scalar.FromDouble(double.NaN));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            IrFiniteBinary64Scalar.FromDouble(double.PositiveInfinity));
        Assert.AreEqual(new ExactRational(1,3),new IrExactScalar(new ExactRational(1,3)).Value);
    }

    private static IrExactScalar Exact(int numerator) => new(new ExactRational(numerator,1));
}
