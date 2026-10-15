using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C01T1Tests
{
    private static IrExactScalar Exact(long n,long d=1)=>
        new(new ExactRational(n,d));
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static CanonicalNotationResult Pass(string text)
    {
        var result=CanonicalMathNotation.Parse(text);
        Assert.IsTrue(result.IsAdmitted,
            "Expected admitted canonical text: "+text+"; status="+result.Status);
        Assert.AreEqual(CanonicalNotationStatus.Admitted,result.Status);
        Assert.AreEqual(text,result.CanonicalText);
        Assert.IsNotNull(result.Expression);
        Assert.IsNotNull(result.SemanticStructuralSha256);
        Assert.IsNotNull(result.Latex);
        Assert.AreEqual(text,CanonicalMathNotation.Render(
            result.Expression!).CanonicalText);
        return result;
    }
    private static void Refuse(string text,CanonicalNotationStatus status)
    {
        var result=CanonicalMathNotation.Parse(text);
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.Latex);
        Assert.IsNull(result.SemanticStructuralSha256);
    }

    [TestMethod]
    public void ExactPositiveHalfHasExactFractionalGoldenLatex()
    {
        var r=Pass("1/2");
        Assert.AreEqual("\\frac{1}{2}",r.Latex);
        Assert.AreEqual(new ExactRational(1,2),((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void ExactNegativeFractionHasNegativeGoldenLatex()
    {
        var r=Pass("-2/3");
        Assert.AreEqual("-\\frac{2}{3}",r.Latex);
        Assert.AreEqual(new ExactRational(-2,3),((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void ExactNegativeIntegerKeepsSignWithoutFloatingPoint() =>
        Assert.AreEqual("-7",Pass("-7").Latex);

    [TestMethod]
    public void ZeroIsCanonicalAndExactlyRepresented() =>
        Assert.AreEqual(ExactRational.Zero,((IrExactScalar)Pass("0").Expression!).Value);

    [TestMethod]
    public void CanonicalPolynomialPreservesThreeOrderedTerms()
    {
        var result=Pass("add(square(x),mul(2,x),1)");
        var expr=(IrApply)result.Expression!;
        Assert.AreEqual(IrOperation.Add,expr.Operation);
        Assert.AreEqual(3,expr.Arguments.Length);
        Assert.AreEqual(IrOperation.Square,((IrApply)expr.Arguments[0]).Operation);
        Assert.IsTrue(result.Latex!.Contains("\\cdot",StringComparison.Ordinal));
    }

    [TestMethod]
    public void CanonicalSubtractionPreservesOriginalUnsimplifiedShape()
    {
        var r=Pass("sub(square(x),1)");
        Assert.AreEqual(IrOperation.Subtract,((IrApply)r.Expression!).Operation);
        Assert.IsTrue(r.Latex!.Contains("-",StringComparison.Ordinal));
    }

    [TestMethod]
    public void DivisionRendersStructuralFractionWithoutCancelling()
    {
        var r=Pass("div(sub(square(x),1),sub(x,1))");
        Assert.AreEqual(IrOperation.Divide,((IrApply)r.Expression!).Operation);
        Assert.IsTrue(r.Latex!.StartsWith("\\frac{",StringComparison.Ordinal));
    }

    [TestMethod]
    public void CubicPowerRetainsExponentAndVariable()
    {
        var result=Pass("pow(x,3)");
        var expr=(IrApply)result.Expression!;
        Assert.AreEqual(IrOperation.Power,expr.Operation);
        Assert.AreEqual(new ExactRational(3,1),
            ((IrExactScalar)expr.Arguments[1]).Value);
        Assert.IsTrue(result.Latex!.Contains("^{3}",StringComparison.Ordinal));
    }

    [TestMethod]
    public void SupportedMaximumPowerEightIsStructuralNotNumerical() =>
        Assert.AreEqual("pow(x,8)",Pass("pow(x,8)").CanonicalText);

    [TestMethod]
    public void SquareOfPolynomialIsKeptAsAnOriginalTree() =>
        Assert.AreEqual(IrOperation.Square,
            ((IrApply)Pass("square(add(x,1))").Expression!).Operation);

    [TestMethod]
    public void RadicalFunctionRemainsAnUncomputedSqrt()
    {
        var result=Pass("sqrt(square(x))");
        Assert.AreEqual(IrOperation.Sqrt,((IrApply)result.Expression!).Operation);
        Assert.IsTrue(result.Latex!.Contains("\\sqrt",StringComparison.Ordinal));
    }

    [TestMethod]
    public void AbsoluteFunctionHasExplicitBarsButNoEvaluation()
    {
        var result=Pass("abs(x)");
        Assert.AreEqual(IrOperation.Abs,((IrApply)result.Expression!).Operation);
        Assert.AreEqual("\\left|x\\right|",result.Latex);
    }

    [TestMethod]
    public void MultipleRationalOperandsRetainTheirExactOrder() =>
        Assert.AreEqual(4,((IrApply)Pass("add(1/2,1/3,1/6,0)").Expression!).Arguments.Length);

    [TestMethod]
    public void ExactUnitInchCarriesUnitSystemAndDimension()
    {
        var r=Pass("unit(5/2,imperial.inch,None)");
        var unit=(IrQuantityLiteral)r.Expression!;
        Assert.AreEqual(UnitSystem.BritishImperial,unit.System);
        Assert.AreEqual(DimensionVector.LengthDimension,unit.Dimension);
        Assert.AreEqual(IrTemperatureKind.None,unit.Temperature);
        Assert.AreEqual(new ExactRational(5,2),((IrExactScalar)unit.Value).Value);
        Assert.IsTrue(r.Latex!.Contains("\\mathrm{in}",StringComparison.Ordinal));
    }

    [TestMethod]
    public void ExactSiMetreCarriesCanonicalDimensions() =>
        Assert.AreEqual(DimensionVector.LengthDimension,
            ((IrQuantityLiteral)Pass("unit(1/2,si.metre,None)").Expression!).Dimension);

    [TestMethod]
    public void AbsoluteKelvinTemperatureKindIsRenderedExplicitly()
    {
        var r=Pass("unit(27315/100,si.kelvin,Absolute)");
        var unit=(IrQuantityLiteral)r.Expression!;
        Assert.AreEqual(IrTemperatureKind.Absolute,unit.Temperature);
        Assert.AreEqual(DimensionVector.TemperatureDimension,unit.Dimension);
        Assert.IsTrue(r.Latex!.Contains("_{\\mathrm{absolute}}",StringComparison.Ordinal));
    }

    [TestMethod]
    public void ExclusionRemainsTypedInOriginalRationalExpression()
    {
        var r=Pass("where(div(sub(square(x),1),sub(x,1)),ne(x,1))");
        var restricted=(IrRestrictedExpression)r.Expression!;
        Assert.AreEqual(IrRelationKind.NotEqual,
            restricted.Assumptions.Conditions.Single().Kind);
        Assert.AreEqual(ExactRational.One,
            restricted.Assumptions.Conditions.Single().Right.Value);
        Assert.AreEqual(IrOperation.Divide,
            ((IrApply)restricted.Expression).Operation);
        Assert.IsTrue(r.Latex!.Contains("\\ne",StringComparison.Ordinal));
        Assert.AreEqual("free:x",
            restricted.Assumptions.Conditions.Single().Symbol.Identity);
    }

    [TestMethod]
    public void RealDomainGreaterOrEqualRestrictionIsNotErased()
    {
        var r=Pass("where(sqrt(square(x)),ge(x,0))");
        var node=(IrRestrictedExpression)r.Expression!;
        Assert.AreEqual(IrRelationKind.GreaterOrEqual,
            node.Assumptions.Conditions.Single().Kind);
        Assert.IsTrue(r.Latex!.Contains("\\ge",StringComparison.Ordinal));
    }

    [TestMethod]
    public void MultipleRestrictionsRetainIdentityOrderAndExactBounds()
    {
        var result=Pass("where(x,ne(x,1),ge(x,0))");
        var node=(IrRestrictedExpression)result.Expression!;
        Assert.AreEqual(2,node.Assumptions.Conditions.Length);
        Assert.AreEqual(IrRelationKind.NotEqual,node.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(IrRelationKind.GreaterOrEqual,node.Assumptions.Conditions[1].Kind);
    }

    [TestMethod]
    public void EqualityRestrictionIsTypedWithoutSolvingIt()
    {
        var r=Pass("where(x,eq(x,1/2))");
        Assert.AreEqual(IrRelationKind.Equal,
            ((IrRestrictedExpression)r.Expression!).Assumptions.Conditions.Single().Kind);
    }

    [TestMethod]
    public void OriginalIrStructuralJsonRoundTripsByteIdentically()
    {
        var parsed=Pass("where(add(pow(x,2),1/2),ne(x,1))");
        var encoded=IrCanonicalJsonCodec.Encode(parsed.Expression!);
        var decoded=IrCanonicalJsonCodec.Decode(encoded);
        var rendered=CanonicalMathNotation.Render(decoded);
        Assert.IsTrue(rendered.IsAdmitted);
        Assert.AreEqual(parsed.CanonicalText,rendered.CanonicalText);
        Assert.AreEqual(encoded,IrCanonicalJsonCodec.Encode(rendered.Expression!));
        Assert.AreEqual(parsed.SemanticStructuralSha256,rendered.SemanticStructuralSha256);
    }

    [TestMethod]
    public void RenderingAdmittedIrProducesSameCanonicalTextAndSemanticHash()
    {
        var source=IrApply.Create(IrOperation.Add,
            [X(),new IrExactScalar(new ExactRational(1,2))]);
        var result=CanonicalMathNotation.Render(source);
        Assert.AreEqual("add(x,1/2)",result.CanonicalText);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(source),
            result.SemanticStructuralSha256);
    }

    [TestMethod]
    public void RenderingOriginalIrRestrictionPreservesExactExclusion()
    {
        var x=X();
        var original=new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.NotEqual,Exact(1))]));
        var result=CanonicalMathNotation.Render(original);
        Assert.IsTrue(result.IsAdmitted);
        Assert.AreEqual("where(x,ne(x,1))",result.CanonicalText);
        Assert.IsTrue(result.Latex!.Contains("\\ne",StringComparison.Ordinal));
    }

    [TestMethod]
    public void RenderingFiniteBinary64CannotClaimExactRationalMeaning()
    {
        var r=CanonicalMathNotation.Render(IrFiniteBinary64Scalar.FromDouble(0.5));
        Assert.AreEqual(CanonicalNotationStatus.UnsupportedApproximation,r.Status);
        Assert.IsNull(r.Latex);
        Assert.IsNull(r.Expression);
    }

    [TestMethod]
    public void RenderingBoundSymbolRejectsLossOfScope()
    {
        var r=CanonicalMathNotation.Render(IrSymbol.Bound("scope",0,"x"));
        Assert.AreEqual(CanonicalNotationStatus.UnsupportedDomain,r.Status);
        Assert.IsNull(r.CanonicalText);
    }

    [TestMethod]
    public void RenderingNoncanonicalPresentationLabelRejectsTeXInjection()
    {
        var r=CanonicalMathNotation.Render(IrSymbol.Free("x",@"\input{secret}"));
        Assert.AreEqual(CanonicalNotationStatus.UnsupportedDomain,r.Status);
        Assert.IsNull(r.Latex);
    }

    [TestMethod]
    public void RenderingForeignUnitIsTypedRefusalWithoutInventedDimension()
    {
        var unit=new IrQuantityLiteral(Exact(1),"unknown.unit",
            UnitSystem.Si,DimensionVector.LengthDimension);
        var r=CanonicalMathNotation.Render(unit);
        Assert.AreEqual(CanonicalNotationStatus.UnsupportedUnits,r.Status);
        Assert.IsNull(r.Latex);
    }

    [TestMethod]
    public void RenderingIncorrectUnitDimensionRefusesMeaningChange()
    {
        var unit=new IrQuantityLiteral(Exact(1),"si.metre",
            UnitSystem.Si,DimensionVector.MassDimension);
        var r=CanonicalMathNotation.Render(unit);
        Assert.AreEqual(CanonicalNotationStatus.UnsupportedUnits,r.Status);
    }

    [TestMethod]
    public void NoncanonicalEquivalentFractionIsNotSilentlyAccepted() =>
        Refuse("2/4",CanonicalNotationStatus.NonCanonical);

    [TestMethod]
    public void NoncanonicalNegativeZeroDoesNotChangeCanonicalIdentity() =>
        Refuse("-0",CanonicalNotationStatus.NonCanonical);

    [TestMethod]
    public void NoncanonicalLeadingZerosAreRejected() =>
        Refuse("01",CanonicalNotationStatus.NonCanonical);

    [TestMethod]
    public void NoncanonicalFractionNestedInAnExpressionIsRejected() =>
        Refuse("add(x,2/4)",CanonicalNotationStatus.NonCanonical);

    [TestMethod]
    public void UnknownIdentifierIsNotAnArbitraryFreeSymbol() =>
        Refuse("unknown",CanonicalNotationStatus.Unsupported);

    [TestMethod]
    public void ArbitraryMacroCannotEnterAStructuralGrammar() =>
        Refuse(@"\input{file}",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void NoScriptOrHtmlIsInterpretedAsMathematics() =>
        Refuse("<script>",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void SpaceIsNotPartOfTheExactCanonicalMode() =>
        Refuse("add(x, 1)",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void MissingOperandIsNotSilentlyRepaired() =>
        Refuse("add(x)",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void MissingClosingDelimiterIsInvalid() =>
        Refuse("square(x",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void UnsupportedArbitraryFunctionIsTypedRefusal() =>
        Refuse("eval(x)",CanonicalNotationStatus.Unsupported);

    [TestMethod]
    public void UnqualifiedUnitAliasIsTypedRefusal() =>
        Refuse("unit(1,unknown.unit,None)",CanonicalNotationStatus.UnsupportedUnits);

    [TestMethod]
    public void MissingTemperatureMetadataIsInvalidInsteadOfGuessed() =>
        Refuse("unit(1,si.kelvin)",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void IntegerExponentAboveDeclaredBoundIsNotAccepted() =>
        Refuse("pow(x,9)",CanonicalNotationStatus.Unsupported);

    [TestMethod]
    public void FractionalExponentNeedsSeparateMathematicalQualification() =>
        Refuse("pow(x,1/2)",CanonicalNotationStatus.Unsupported);

    [TestMethod]
    public void DuplicateConditionsAreNotSilentlyLost() =>
        Refuse("where(x,ne(x,1),ne(x,1))",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void AbsentConditionDoesNotProduceMisleadingLaTex() =>
        Refuse("where(x)",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void MoreThanDeclaredTextLengthReturnsBudgetExceeded() =>
        Refuse(new string('x',CanonicalMathNotation.MaximumInputCharacters+1),
            CanonicalNotationStatus.BudgetExceeded);

    [TestMethod]
    public void MoreThanDeclaredParseDepthReturnsBudgetExceeded()
    {
        var text=string.Concat(Enumerable.Repeat("square(",17))+"x"+
            new string(')',17);
        Refuse(text,CanonicalNotationStatus.BudgetExceeded);
    }

    [TestMethod]
    public void OverlargeNumeratorRefusesBoundedExactTransport() =>
        Refuse((BigInteger.One<<150).ToString(System.Globalization.CultureInfo.InvariantCulture),
            CanonicalNotationStatus.BudgetExceeded);

    [TestMethod]
    public void EmptyStringHasNoFabricatedExpression() =>
        Refuse("",CanonicalNotationStatus.InvalidInput);

    [TestMethod]
    public void TwoIndependentFreeRealSymbolsAreDistinguishedByIdentity()
    {
        var x=Pass("x");var y=Pass("y");
        Assert.AreNotEqual(x.SemanticStructuralSha256,y.SemanticStructuralSha256);
        Assert.AreEqual("free:y",((IrSymbol)y.Expression!).Identity);
    }
}
