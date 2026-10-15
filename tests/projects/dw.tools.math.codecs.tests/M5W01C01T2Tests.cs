using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C01T2Tests
{
    private static CanonicalDisplayBoundaryResult Safe(
        string input,bool collision=false)
    {
        var result=CanonicalDisplayBoundaryAudit.AuditText(input);
        var expected=collision
            ? CanonicalDisplayBoundaryStatus.SafeDisplayWithCollision
            : CanonicalDisplayBoundaryStatus.SafeDisplay;
        Assert.AreEqual(expected,result.Status,
            "Safe display boundary rejected "+input+" with "+result.Status);
        Assert.IsTrue(result.IsAdmitted);
        Assert.AreEqual(CanonicalNotationStatus.Admitted,result.UnderlyingStatus);
        Assert.AreEqual(input,result.CanonicalText);
        Assert.IsNotNull(result.Expression);
        Assert.IsNotNull(result.DisplayLatex);
        Assert.IsNotNull(result.SemanticStructuralSha256);
        Assert.AreEqual(CanonicalDisplayBoundaryAudit.Rule,result.RuleId);
        Assert.IsFalse(result.LatexInverseQualified);
        Assert.AreEqual(collision,result.PresentationMayCollide);
        Assert.AreEqual(
            IrCanonicalHashes.SemanticStructuralSha256(result.Expression!),
            result.SemanticStructuralSha256);
        return result;
    }

    private static CanonicalDisplayBoundaryResult Refused(
        string input,CanonicalDisplayBoundaryStatus expected,
        CanonicalNotationStatus? underlying=null)
    {
        var result=CanonicalDisplayBoundaryAudit.AuditText(input);
        RefusedResult(result,expected,underlying);
        return result;
    }

    private static void RefusedResult(CanonicalDisplayBoundaryResult result,
        CanonicalDisplayBoundaryStatus status,CanonicalNotationStatus? underlying=null)
    {
        Assert.AreEqual(status,result.Status);
        Assert.AreEqual(underlying,result.UnderlyingStatus);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsFalse(result.PresentationMayCollide);
        Assert.IsFalse(result.LatexInverseQualified);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.DisplayLatex);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
    }

    [TestMethod]
    public void SafeHalfRetainsExactFractionAndWhitelistedFracMacro()
    {
        var r=Safe("1/2");
        Assert.AreEqual("\\frac{1}{2}",r.DisplayLatex);
        Assert.AreEqual(new ExactRational(1,2),((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void NegativeRationalRemainsExactAndNonExecutable()
    {
        var r=Safe("-2/3");
        Assert.AreEqual("-\\frac{2}{3}",r.DisplayLatex);
    }

    [TestMethod]
    public void ZeroHasNoNumericalOrAlgebraicInterpretation() =>
        Assert.AreEqual("0",Safe("0").DisplayLatex);

    [TestMethod]
    public void StandaloneFreeXIsAnAdmittedExactLeaf() =>
        Assert.AreEqual("free:x",((IrSymbol)Safe("x").Expression!).Identity);

    [TestMethod]
    public void StandaloneFreeYIsAnAdmittedExactLeaf() =>
        Assert.AreEqual("free:y",((IrSymbol)Safe("y").Expression!).Identity);

    [TestMethod]
    public void StandaloneFreeZIsAnAdmittedExactLeaf() =>
        Assert.AreEqual("free:z",((IrSymbol)Safe("z").Expression!).Identity);

    [TestMethod]
    public void ClosedAddOfTwoRationalsIsAdmittedWithBalancedLatex() =>
        Assert.IsTrue(Safe("add(1/2,1/3)").DisplayLatex!
            .StartsWith("\\left(",StringComparison.Ordinal));

    [TestMethod]
    public void ClosedMixedPolynomialUsesOnlyQualifiedOutputCommands()
    {
        var r=Safe("add(mul(2,x),1)");
        Assert.IsTrue(r.DisplayLatex!.Contains("\\cdot",StringComparison.Ordinal));
    }

    [TestMethod]
    public void CanonicalSubtractionRetainsBothOrderedOperands() =>
        Assert.IsTrue(Safe("sub(x,1)").DisplayLatex!
            .Contains("-",StringComparison.Ordinal));

    [TestMethod]
    public void DivisionRendersDisplayOnlyExactFraction() =>
        Assert.IsTrue(Safe("div(x,2)").DisplayLatex!
            .StartsWith("\\frac{",StringComparison.Ordinal));

    [TestMethod]
    public void PowerThreeDoesNotTriggerTheKnownPowerTwoCollision() =>
        Assert.AreEqual(CanonicalDisplayBoundaryStatus.SafeDisplay,
            Safe("pow(x,3)").Status);

    [TestMethod]
    public void MaximumAdmittedPowerEightRemainsStructural() =>
        Assert.AreEqual("pow(x,8)",Safe("pow(x,8)").CanonicalText);

    [TestMethod]
    public void SqrtRendersAllowedMacroWithoutNumericalEvaluation() =>
        Assert.AreEqual("\\sqrt{x}",Safe("sqrt(x)").DisplayLatex);

    [TestMethod]
    public void AbsoluteValueRendersBoundedBarsWithoutInterpretation() =>
        Assert.AreEqual("\\left|x\\right|",Safe("abs(x)").DisplayLatex);

    [TestMethod]
    public void DirectSquareIsExplicitlyClassifiedNonInjective()
    {
        var result=Safe("square(x)",collision:true);
        Assert.AreEqual("\\left(x\\right)^{2}",result.DisplayLatex);
    }

    [TestMethod]
    public void ExplicitPowerTwoSharesTheSamePresentationCollision()
    {
        var result=Safe("pow(x,2)",collision:true);
        Assert.AreEqual("\\left(x\\right)^{2}",result.DisplayLatex);
    }

    [TestMethod]
    public void TwoDifferentIrConstructorsHaveDifferentStructuralHashes()
    {
        var square=Safe("square(x)",collision:true);
        var power=Safe("pow(x,2)",collision:true);
        Assert.AreEqual(square.DisplayLatex,power.DisplayLatex);
        Assert.AreNotEqual(square.SemanticStructuralSha256,
            power.SemanticStructuralSha256);
        Assert.AreNotEqual(square.CanonicalText,power.CanonicalText);
        Assert.IsFalse(square.LatexInverseQualified);
        Assert.IsFalse(power.LatexInverseQualified);
    }

    [TestMethod]
    public void NestedSquareRetainsNonInjectivityFlag() =>
        Assert.IsTrue(Safe("sqrt(square(x))",collision:true).PresentationMayCollide);

    [TestMethod]
    public void NoninjectiveSquareUnderRestrictionStillPreservesDomain()
    {
        var r=Safe("where(square(x),ne(x,1))",collision:true);
        Assert.IsTrue(r.Expression is IrRestrictedExpression);
        Assert.IsTrue(r.DisplayLatex!.Contains("\\ne",StringComparison.Ordinal));
    }

    [TestMethod]
    public void OriginalRationalHoleIsPreservedAndCollisionReported()
    {
        var r=Safe("where(div(sub(square(x),1),sub(x,1)),ne(x,1))",
            collision:true);
        Assert.IsTrue(r.Expression is IrRestrictedExpression);
        Assert.IsTrue(r.DisplayLatex!.Contains("\\ne",StringComparison.Ordinal));
    }

    [TestMethod]
    public void SingleBoundedNotEqualConditionUsesKnownMacro() =>
        Assert.IsTrue(Safe("where(x,ne(x,1))").DisplayLatex!
            .Contains("\\ne",StringComparison.Ordinal));

    [TestMethod]
    public void NonnegativeRealConditionUsesWhitelistedGeMacro() =>
        Assert.IsTrue(Safe("where(x,ge(x,0))").DisplayLatex!
            .Contains("\\ge",StringComparison.Ordinal));

    [TestMethod]
    public void EqualityConditionDoesNotBecomeAnEquationSolver() =>
        Assert.IsTrue(Safe("where(x,eq(x,1))").DisplayLatex!
            .Contains("x=1",StringComparison.Ordinal));

    [TestMethod]
    public void CompatibleInequalityAndEqualityKeepOriginalConditions()
    {
        var r=Safe("where(x,eq(x,3),ge(x,2))");
        var expr=(IrRestrictedExpression)r.Expression!;
        Assert.AreEqual(2,expr.Assumptions.Conditions.Length);
        Assert.IsTrue(r.DisplayLatex!.Contains("\\ge",StringComparison.Ordinal));
    }

    [TestMethod]
    public void QualifiedQuantityMetreKeepsDimensionAndMetadata()
    {
        var r=Safe("unit(5/2,si.metre,None)");
        var quantity=(IrQuantityLiteral)r.Expression!;
        Assert.AreEqual(DimensionVector.LengthDimension,quantity.Dimension);
        Assert.AreEqual(UnitSystem.Si,quantity.System);
        Assert.IsTrue(r.DisplayLatex!.Contains("\\mathrm{m}",StringComparison.Ordinal));
    }

    [TestMethod]
    public void QualifiedQuantityInchKeepsItsUnitSystem() =>
        Assert.AreEqual(UnitSystem.BritishImperial,
            ((IrQuantityLiteral)Safe("unit(1/2,imperial.inch,None)").Expression!).System);

    [TestMethod]
    public void QualifiedAbsoluteKelvinUsesReducedCanonicalNumeral()
    {
        var r=Safe("unit(5463/20,si.kelvin,Absolute)");
        Assert.AreEqual(new ExactRational(27315,100),
            ((IrExactScalar)((IrQuantityLiteral)r.Expression!).Value).Value);
        Assert.IsTrue(r.DisplayLatex!.Contains("_{\\mathrm{absolute}}",
            StringComparison.Ordinal));
    }

    [TestMethod]
    public void AdmittedSemanticStructuralHashIsStableUnderIndependentIrCodec()
    {
        var r=Safe("where(add(x,1/2),ge(x,0))");
        var json=IrCanonicalJsonCodec.Encode(r.Expression!);
        var decoded=IrCanonicalJsonCodec.Decode(json);
        var again=CanonicalDisplayBoundaryAudit.AuditIr(decoded);
        Assert.IsTrue(again.IsAdmitted);
        Assert.AreEqual(r.SemanticStructuralSha256,again.SemanticStructuralSha256);
        Assert.AreEqual(r.CanonicalText,again.CanonicalText);
    }

    [TestMethod]
    public void AuditingDirectIrProducesSameStructuralText()
    {
        var source=IrApply.Create(IrOperation.Add,[
            IrSymbol.Free("x","x"),
            new IrExactScalar(new ExactRational(1,2))]);
        var r=CanonicalDisplayBoundaryAudit.AuditIr(source);
        Assert.IsTrue(r.IsAdmitted);
        Assert.AreEqual("add(x,1/2)",r.CanonicalText);
        Assert.IsFalse(r.LatexInverseQualified);
    }

    [TestMethod]
    public void UnboundForeignConditionOnTextCannotForgeASafeDisplay() =>
        Refused("where(x,ne(y,1))",
            CanonicalDisplayBoundaryStatus.UnboundRestriction);

    [TestMethod]
    public void UnboundForeignConditionOnIrCannotForgeProvenance()
    {
        var x=IrSymbol.Free("x","x");
        var y=IrSymbol.Free("y","y");
        var bad=new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(y,IrRelationKind.NotEqual,
                new IrExactScalar(ExactRational.One))]));
        RefusedResult(CanonicalDisplayBoundaryAudit.AuditIr(bad),
            CanonicalDisplayBoundaryStatus.UnboundRestriction);
    }

    [TestMethod]
    public void ForeignConditionOnPresentVariableIsNotConfusedWithUnbound()
    {
        var r=Safe("where(add(x,y),ne(y,1))");
        Assert.IsTrue(r.Expression is IrRestrictedExpression);
    }

    [TestMethod]
    public void ExactEqualsAndNotEqualsTheSamePointAreContradictory() =>
        Refused("where(x,eq(x,1),ne(x,1))",
            CanonicalDisplayBoundaryStatus.ContradictoryAssumptions);

    [TestMethod]
    public void TwoUnequalEqualitiesAreContradictory() =>
        Refused("where(x,eq(x,1),eq(x,2))",
            CanonicalDisplayBoundaryStatus.ContradictoryAssumptions);

    [TestMethod]
    public void ExactEqualityBelowLowerBoundIsContradictory() =>
        Refused("where(x,eq(x,1),ge(x,2))",
            CanonicalDisplayBoundaryStatus.ContradictoryAssumptions);

    [TestMethod]
    public void OriginalConditionNotEqualDifferentValueIsConsistent() =>
        Assert.IsTrue(Safe("where(x,eq(x,1),ne(x,2))").IsAdmitted);

    [TestMethod]
    public void ExactTextDoesNotAdmitExternalTeXInputMacros() =>
        Refused(@"\input{file}",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void ShellLikeTeXWrite18IsNotAParserCommand() =>
        Refused(@"\write18{file}",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void DisplayOnlyFracMacroCannotBeParsedAsMathText() =>
        Refused(@"\frac{1}{2}",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void ArbitraryFunctionNameIsRejectedNotExecuted() =>
        Refused("eval(x)",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.Unsupported);

    [TestMethod]
    public void UnknownFreeSymbolIsNotPermittedToBecomeTeX() =>
        Refused("secret",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.Unsupported);

    [TestMethod]
    public void ForeignDisplayLabelCannotInjectTeX()
    {
        var bad=IrSymbol.Free("x",@"\input{secret}");
        RefusedResult(CanonicalDisplayBoundaryAudit.AuditIr(bad),
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.UnsupportedDomain);
    }

    [TestMethod]
    public void BoundSymbolCannotBeDisplayedAsFreeRealIdentity()
    {
        var bad=IrSymbol.Bound("scope",0,"x");
        RefusedResult(CanonicalDisplayBoundaryAudit.AuditIr(bad),
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.UnsupportedDomain);
    }

    [TestMethod]
    public void FiniteBinary64MustNotPretendToBeExact()
    {
        var bad=IrFiniteBinary64Scalar.FromDouble(0.5);
        RefusedResult(CanonicalDisplayBoundaryAudit.AuditIr(bad),
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.UnsupportedApproximation);
    }

    [TestMethod]
    public void NonCanonicalRationalAliasIsARefusal() =>
        Refused("2/4",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.NonCanonical);

    [TestMethod]
    public void NoncanonicalKelvinFractionIsARefusal() =>
        Refused("unit(27315/100,si.kelvin,Absolute)",
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.NonCanonical);

    [TestMethod]
    public void OversizedCanonicalInputHasNoProofOrDisplay() =>
        Refused(new string('x',CanonicalMathNotation.MaximumInputCharacters+1),
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.BudgetExceeded);

    [TestMethod]
    public void TooDeepCanonicalInputHasNoProofOrDisplay()
    {
        var deep=string.Concat(Enumerable.Repeat("square(",17))+"x"+
            new string(')',17);
        Refused(deep,CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.BudgetExceeded);
    }

    [TestMethod]
    public void UnknownUnitHasNoInventedDimensionOrTeX()
    {
        var source=new IrQuantityLiteral(
            new IrExactScalar(ExactRational.One),
            "unknown.unit",UnitSystem.Si,DimensionVector.LengthDimension);
        RefusedResult(CanonicalDisplayBoundaryAudit.AuditIr(source),
            CanonicalDisplayBoundaryStatus.RefusedInput,
            CanonicalNotationStatus.UnsupportedUnits);
    }

    [TestMethod]
    public void NoNormalDisplayClaimsItCanBeInverseParsedFromLatex()
    {
        var r=Safe("add(x,1)");
        Assert.IsFalse(r.LatexInverseQualified);
        Assert.AreEqual(CanonicalDisplayBoundaryStatus.SafeDisplay,r.Status);
    }

    [TestMethod]
    public void SafeDisplayUsesOnlyTheNamedVersionedRule() =>
        Assert.AreEqual(
            "canonical-safe-math-display-domain-audit/v1",
            Safe("x").RuleId);
}
