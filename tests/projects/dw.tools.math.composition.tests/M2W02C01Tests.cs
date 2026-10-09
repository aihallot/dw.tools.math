using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C01Tests
{
    private static IrExactScalar Exact(int value) => new(new ExactRational(value, 1));
    private static MathResultProvenance Origin() =>
        MathResultProvenance.Create("matrix.inspect", "math-composition/1");

    [TestMethod]
    public void MatrixInspectionAdvertisesAdmittedShapeAndPrecision()
    {
        var matrix = IrMatrix.Create(2, 2, [Exact(1), Exact(2), Exact(3), Exact(4)]);
        var descriptor = MathValueDescriptor.Inspect(matrix);
        var capability = MathOperationCapability.MatrixInspection;
        Assert.AreEqual(MathValueKind.Matrix, descriptor.Kind);
        Assert.AreEqual(MathPrecisionKind.Exact, descriptor.Precision);
        Assert.AreEqual(2, descriptor.Rows);
        Assert.AreEqual(2, descriptor.Columns);
        Assert.AreEqual("matrix.inspect", capability.OperationId);
        Assert.IsTrue(capability.Accepts(matrix),
            "M2-W02-C01-T1 RED: matrix inspection must admit an exact matrix shape.");
    }

    [TestMethod]
    public void ExactAndApproximateResultsStayDistinct()
    {
        var exact = Exact(17);
        var approximate = IrFiniteBinary64Scalar.FromDouble(0.25);
        var exactResult = MathOperationResult.Exact(exact, Origin());
        var approximateResult = MathOperationResult.Approximate(approximate, Origin());
        Assert.AreEqual(ComputationStatus.Exact, exactResult.Status);
        Assert.AreEqual(ComputationStatus.Approximate, approximateResult.Status);
        Assert.AreSame(exact, exactResult.Value);
        Assert.AreSame(approximate, approximateResult.Value);
        Assert.IsTrue(exactResult.HasFinalValue);
        Assert.IsTrue(approximateResult.HasFinalValue);
    }

    [TestMethod]
    public void UnsupportedAndBudgetStatusesNeverFabricateFinalResult()
    {
        var unsupported = MathOperationResult.Unsupported("no admitted operation", Origin());
        var budget = MathOperationResult.BudgetExceeded("limit 4096", Origin());
        Assert.AreEqual(ComputationStatus.Unsupported, unsupported.Status);
        Assert.AreEqual(ComputationStatus.BudgetExceeded, budget.Status);
        Assert.IsNull(unsupported.Value);
        Assert.IsNull(budget.Value);
        Assert.IsFalse(unsupported.HasFinalValue);
        Assert.IsFalse(budget.HasFinalValue);
        Assert.IsNotNull(unsupported.Reason);
        Assert.IsNotNull(budget.Reason);
    }

    [TestMethod]
    public void PrecisionMismatchIsNotImplicitlyWidened()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathOperationResult.Approximate(Exact(2), Origin()));
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathOperationResult.Exact(IrFiniteBinary64Scalar.FromDouble(2.0), Origin()));
    }

    [TestMethod]
    public void MatrixInspectionDoesNotPromiseScalarOrSymbolExecution()
    {
        var capability = MathOperationCapability.MatrixInspection;
        Assert.IsFalse(capability.Accepts(Exact(1)));
        Assert.IsFalse(capability.Accepts(IrSymbol.Free("x", "x")));
        Assert.IsFalse(capability.Accepts(null));
        Assert.ThrowsExactly<NotSupportedException>(() =>
            MathValueDescriptor.Inspect(IrSymbol.Free("y", "y")));
    }

    [TestMethod]
    public void ApproximateMatrixIsInspectedWithoutConversion()
    {
        var matrix = IrMatrix.Create(1, 2, [
            IrFiniteBinary64Scalar.FromDouble(0.5),
            IrFiniteBinary64Scalar.FromDouble(0.25)]);
        var shape = MathValueDescriptor.Inspect(matrix);
        Assert.AreEqual(MathValueKind.Matrix, shape.Kind);
        Assert.AreEqual(MathPrecisionKind.Approximate, shape.Precision);
        Assert.AreEqual(1, shape.Rows);
        Assert.AreEqual(2, shape.Columns);
        Assert.IsTrue(MathOperationCapability.MatrixInspection.Accepts(matrix));
    }

    [TestMethod]
    public void ProvenanceAndMissingResultsHaveBoundedNonemptyReasons()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathResultProvenance.Create("", "math-composition/1"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathResultProvenance.Create("matrix.inspect", " "));
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathOperationResult.Unsupported("", Origin()));
        var result = MathOperationResult.Unsupported("unsupported shape", Origin());
        Assert.AreEqual("matrix.inspect", result.Provenance.OperationId);
        Assert.AreEqual("math-composition/1", result.Provenance.ContractVersion);
    }
}
