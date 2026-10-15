using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>Only exact real polynomial calculus is admitted; no numerical fallback.</summary>
public enum SymbolicPolynomialCalculusStatus
{
    Exact,
    Unsupported,
    UnsupportedDomain,
    UnsupportedUnits,
    UnsupportedAssumptions,
    UnsupportedApproximation,
    BudgetExceeded
}

/// <summary>
/// The expression is a canonical zero-constant representative. The general
/// indefinite primitive additionally includes an arbitrary additive constant.
/// Rejected computations carry neither mathematical value nor proof.
/// </summary>
public sealed record SymbolicPolynomialIntegralResult(
    SymbolicPolynomialCalculusStatus Status,
    IrNode? ZeroConstantPrimitive,
    ExactRational? DefiniteValue,
    string? ConstantConvention,
    string? RuleId,
    string? DerivativeRuleId,
    string? SourceSemanticSha256,
    string? VariableIdentity)
{
    public bool IsExact => Status == SymbolicPolynomialCalculusStatus.Exact
        && ZeroConstantPrimitive is not null && DefiniteValue.HasValue
        && ConstantConvention is not null && RuleId is not null
        && DerivativeRuleId is not null && SourceSemanticSha256 is not null
        && VariableIdentity is not null;
}

/// <summary>Finite polynomial limits are exact by polynomial continuity.</summary>
public sealed record SymbolicPolynomialLimitResult(
    SymbolicPolynomialCalculusStatus Status,
    ExactRational? Value,
    string? RuleId,
    string? SourceSemanticSha256,
    string? VariableIdentity)
{
    public bool IsExact => Status == SymbolicPolynomialCalculusStatus.Exact
        && Value.HasValue && RuleId is not null
        && SourceSemanticSha256 is not null && VariableIdentity is not null;
}

/// <summary>
/// Closed rational polynomial integration and finite-point limits over one
/// free real symbol. The degree bound permits independent verification by
/// the previously admitted degree-eight formal differentiation contract.
/// No analytic, improper, numerical or complex integration is inferred.
/// </summary>
public static class SymbolicExactPolynomialCalculus
{
    public const string IntegralRule = "real-polynomial-antiderivative-definite-integral/exact/v1";
    public const string LimitRule = "real-polynomial-finite-limit/exact/v1";
    public const string AdditiveConstantConvention = "arbitrary-additive-constant";
    public const int MaximumDegree = 7;
    public const int MaximumRationalBits = 128;
    public const int MaximumParseDepth = 12;
    private const int CoefficientCount = MaximumDegree + 1;

    public static SymbolicPolynomialIntegralResult Integrate(
        IrNode integrand, IrSymbol variable,
        ExactRational lowerBound, ExactRational upperBound)
    {
        ArgumentNullException.ThrowIfNull(integrand);
        ArgumentNullException.ThrowIfNull(variable);
        if (!FreeReal(variable))
            return RefusedIntegral(SymbolicPolynomialCalculusStatus.UnsupportedDomain);
        if (!Bounded(lowerBound) || !Bounded(upperBound))
            return RefusedIntegral(SymbolicPolynomialCalculusStatus.BudgetExceeded);

        try
        {
            IrGraphLimits.Validate(integrand);
            var issue = Inspect(integrand);
            if (issue != SymbolicPolynomialCalculusStatus.Exact)
                return RefusedIntegral(issue);
            var polynomial = Parse(integrand, variable, 1);
            if (polynomial.Status != SymbolicPolynomialCalculusStatus.Exact ||
                polynomial.Coefficients is not ExactRational[] input)
                return RefusedIntegral(polynomial.Status);

            var primitiveCoefficients = new ExactRational[MaximumDegree + 2];
            for (var degree = 0; degree < input.Length; degree++)
            {
                primitiveCoefficients[degree + 1] =
                    input[degree] / new ExactRational(degree + 1, 1);
                if (!Bounded(primitiveCoefficients[degree + 1]))
                    return RefusedIntegral(SymbolicPolynomialCalculusStatus.BudgetExceeded);
            }
            var primitive = BuildPrimitive(primitiveCoefficients, variable);
            var derivative = SymbolicExactDifferentiation.TryDifferentiate(
                primitive, variable);
            if (!derivative.IsAdmitted || derivative.Derivative is not IrNode proofBody)
                return RefusedIntegral(derivative.Status ==
                    SymbolicDifferentiationStatus.BudgetExceeded
                    ? SymbolicPolynomialCalculusStatus.BudgetExceeded
                    : SymbolicPolynomialCalculusStatus.Unsupported);

            // An independently qualified formal derivative is necessary but
            // not sufficient; also compare the canonical exact coefficients.
            var check = Parse(proofBody, variable, 1);
            if (check.Status != SymbolicPolynomialCalculusStatus.Exact ||
                check.Coefficients is not ExactRational[] recovered ||
                !input.SequenceEqual(recovered))
                return RefusedIntegral(check.Status ==
                    SymbolicPolynomialCalculusStatus.BudgetExceeded
                    ? check.Status : SymbolicPolynomialCalculusStatus.Unsupported);

            var lower = Evaluate(primitiveCoefficients, lowerBound);
            if (lower.Status != SymbolicPolynomialCalculusStatus.Exact)
                return RefusedIntegral(lower.Status);
            var upper = Evaluate(primitiveCoefficients, upperBound);
            if (upper.Status != SymbolicPolynomialCalculusStatus.Exact)
                return RefusedIntegral(upper.Status);
            var definite = upper.Value - lower.Value;
            if (!Bounded(definite))
                return RefusedIntegral(SymbolicPolynomialCalculusStatus.BudgetExceeded);

            return new(SymbolicPolynomialCalculusStatus.Exact, primitive,
                definite, AdditiveConstantConvention, IntegralRule,
                SymbolicExactDifferentiation.Rule,
                IrCanonicalHashes.SemanticStructuralSha256(integrand),
                variable.Identity);
        }
        catch (ArgumentOutOfRangeException)
        {
            return RefusedIntegral(SymbolicPolynomialCalculusStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return RefusedIntegral(SymbolicPolynomialCalculusStatus.Unsupported);
        }
    }

    public static SymbolicPolynomialLimitResult FiniteLimit(
        IrNode expression, IrSymbol variable, ExactRational point)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(variable);
        if (!FreeReal(variable))
            return RefusedLimit(SymbolicPolynomialCalculusStatus.UnsupportedDomain);
        if (!Bounded(point))
            return RefusedLimit(SymbolicPolynomialCalculusStatus.BudgetExceeded);
        try
        {
            IrGraphLimits.Validate(expression);
            var issue = Inspect(expression);
            if (issue != SymbolicPolynomialCalculusStatus.Exact)
                return RefusedLimit(issue);
            var polynomial = Parse(expression, variable, 1);
            if (polynomial.Status != SymbolicPolynomialCalculusStatus.Exact ||
                polynomial.Coefficients is not ExactRational[] values)
                return RefusedLimit(polynomial.Status);
            var evaluated = Evaluate(values, point);
            if (evaluated.Status != SymbolicPolynomialCalculusStatus.Exact)
                return RefusedLimit(evaluated.Status);
            return new(SymbolicPolynomialCalculusStatus.Exact, evaluated.Value,
                LimitRule, IrCanonicalHashes.SemanticStructuralSha256(expression),
                variable.Identity);
        }
        catch (ArgumentOutOfRangeException)
        {
            return RefusedLimit(SymbolicPolynomialCalculusStatus.BudgetExceeded);
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return RefusedLimit(SymbolicPolynomialCalculusStatus.Unsupported);
        }
    }

    private static SymbolicPolynomialCalculusStatus Inspect(IrNode source)
    {
        var pending = new Stack<IrNode>();
        pending.Push(source);
        while (pending.Count != 0)
        {
            switch (pending.Pop())
            {
                case IrQuantityLiteral:
                    return SymbolicPolynomialCalculusStatus.UnsupportedUnits;
                case IrRestrictedExpression:
                    return SymbolicPolynomialCalculusStatus.UnsupportedAssumptions;
                case IrFiniteBinary64Scalar:
                    return SymbolicPolynomialCalculusStatus.UnsupportedApproximation;
                case IrSymbol symbol when !FreeReal(symbol):
                    return SymbolicPolynomialCalculusStatus.UnsupportedDomain;
                case IrExactScalar scalar when !Bounded(scalar.Value):
                    return SymbolicPolynomialCalculusStatus.BudgetExceeded;
                case IrApply operation:
                    foreach (var child in operation.Arguments)
                        pending.Push(child);
                    break;
            }
        }
        return SymbolicPolynomialCalculusStatus.Exact;
    }

    private static (SymbolicPolynomialCalculusStatus Status,
        ExactRational[]? Coefficients) Parse(
        IrNode node, IrSymbol variable, int depth)
    {
        if (depth > MaximumParseDepth)
            return RefusedPolynomial(SymbolicPolynomialCalculusStatus.BudgetExceeded);
        var c = new ExactRational[CoefficientCount];
        switch (node)
        {
            case IrExactScalar scalar:
                c[0] = scalar.Value;
                return ValidPolynomial(c);
            case IrSymbol symbol:
                if (!Same(symbol, variable))
                    return RefusedPolynomial(SymbolicPolynomialCalculusStatus.Unsupported);
                c[1] = ExactRational.One;
                return ValidPolynomial(c);
            case IrApply { Operation: IrOperation.Square } square:
                if (square.Arguments[0] is not IrSymbol s || !Same(s, variable))
                    return RefusedPolynomial(SymbolicPolynomialCalculusStatus.Unsupported);
                c[2] = ExactRational.One;
                return ValidPolynomial(c);
            case IrApply { Operation: IrOperation.Power } power:
                if (power.Arguments[0] is not IrSymbol p || !Same(p, variable) ||
                    power.Arguments[1] is not IrExactScalar exponent ||
                    exponent.Value.Denominator != BigInteger.One ||
                    exponent.Value.Numerator < 0)
                    return RefusedPolynomial(SymbolicPolynomialCalculusStatus.Unsupported);
                if (exponent.Value.Numerator > MaximumDegree)
                    return RefusedPolynomial(SymbolicPolynomialCalculusStatus.BudgetExceeded);
                c[(int)exponent.Value.Numerator] = ExactRational.One;
                return ValidPolynomial(c);
            case IrApply { Operation: IrOperation.Add } add:
                foreach (var operand in add.Arguments)
                {
                    var term = Parse(operand, variable, depth + 1);
                    if (term.Coefficients is not ExactRational[] parsed)
                        return term;
                    for (var index = 0; index < c.Length; index++)
                    {
                        c[index] += parsed[index];
                        if (!Bounded(c[index]))
                            return RefusedPolynomial(SymbolicPolynomialCalculusStatus.BudgetExceeded);
                    }
                }
                return ValidPolynomial(c);
            case IrApply { Operation: IrOperation.Subtract } subtract:
            {
                var left = Parse(subtract.Arguments[0], variable, depth + 1);
                if (left.Coefficients is not ExactRational[] a)
                    return left;
                var right = Parse(subtract.Arguments[1], variable, depth + 1);
                if (right.Coefficients is not ExactRational[] b)
                    return right;
                for (var i = 0; i < c.Length; i++)
                {
                    c[i] = a[i] - b[i];
                    if (!Bounded(c[i]))
                        return RefusedPolynomial(SymbolicPolynomialCalculusStatus.BudgetExceeded);
                }
                return ValidPolynomial(c);
            }
            case IrApply { Operation: IrOperation.Multiply } multiply:
            {
                if (multiply.Arguments.Length != 2)
                    return RefusedPolynomial(SymbolicPolynomialCalculusStatus.Unsupported);
                var left = Parse(multiply.Arguments[0], variable, depth + 1);
                if (left.Coefficients is not ExactRational[] a)
                    return left;
                var right = Parse(multiply.Arguments[1], variable, depth + 1);
                if (right.Coefficients is not ExactRational[] b)
                    return right;
                var leftConstant = Constant(a);
                var rightConstant = Constant(b);
                if (!leftConstant && !rightConstant)
                    return RefusedPolynomial(SymbolicPolynomialCalculusStatus.Unsupported);
                var scalar = leftConstant ? a[0] : b[0];
                var values = leftConstant ? b : a;
                for (var i = 0; i < c.Length; i++)
                {
                    c[i] = scalar * values[i];
                    if (!Bounded(c[i]))
                        return RefusedPolynomial(SymbolicPolynomialCalculusStatus.BudgetExceeded);
                }
                return ValidPolynomial(c);
            }
            default:
                return RefusedPolynomial(SymbolicPolynomialCalculusStatus.Unsupported);
        }
    }

    private static IrNode BuildPrimitive(
        ExactRational[] coefficients, IrSymbol variable)
    {
        var terms = new List<IrNode>();
        for (var degree = 1; degree < coefficients.Length; degree++)
        {
            var coefficient = coefficients[degree];
            if (coefficient == ExactRational.Zero)
                continue;
            var monomial = degree switch
            {
                1 => (IrNode)variable,
                2 => IrApply.Create(IrOperation.Square, [variable]),
                _ => IrApply.Create(IrOperation.Power,
                    [variable, new IrExactScalar(new ExactRational(degree, 1))])
            };
            terms.Add(coefficient == ExactRational.One
                ? monomial
                : IrApply.Create(IrOperation.Multiply,
                    [new IrExactScalar(coefficient), monomial]));
        }
        return terms.Count switch
        {
            0 => new IrExactScalar(ExactRational.Zero),
            1 => terms[0],
            _ => IrApply.Create(IrOperation.Add, terms)
        };
    }

    private static (SymbolicPolynomialCalculusStatus Status, ExactRational Value)
        Evaluate(ExactRational[] coefficients, ExactRational point)
    {
        var sum = ExactRational.Zero;
        for (var degree = coefficients.Length - 1; degree >= 0; degree--)
        {
            sum = sum * point + coefficients[degree];
            if (!Bounded(sum))
                return (SymbolicPolynomialCalculusStatus.BudgetExceeded,
                    ExactRational.Zero);
        }
        return (SymbolicPolynomialCalculusStatus.Exact, sum);
    }

    private static bool Constant(ExactRational[] coefficients)
    {
        for (var i = 1; i < coefficients.Length; i++)
            if (coefficients[i] != ExactRational.Zero)
                return false;
        return true;
    }

    private static bool FreeReal(IrSymbol x) =>
        !x.IsBound && x.Domain == IrScalarDomain.Real;

    private static bool Same(IrSymbol x, IrSymbol y) =>
        FreeReal(x) && FreeReal(y) &&
        x.Identity == y.Identity && x.Domain == y.Domain;

    private static bool Bounded(ExactRational x) =>
        BigInteger.Abs(x.Numerator).GetBitLength() <= MaximumRationalBits &&
        x.Denominator.GetBitLength() <= MaximumRationalBits;

    private static (SymbolicPolynomialCalculusStatus Status,
        ExactRational[]? Coefficients) ValidPolynomial(ExactRational[] coefficients) =>
        (SymbolicPolynomialCalculusStatus.Exact, coefficients);

    private static (SymbolicPolynomialCalculusStatus Status,
        ExactRational[]? Coefficients) RefusedPolynomial(SymbolicPolynomialCalculusStatus status) =>
        (status, null);

    private static SymbolicPolynomialIntegralResult RefusedIntegral(
        SymbolicPolynomialCalculusStatus status) =>
        new(status, null, null, null, null, null, null, null);

    private static SymbolicPolynomialLimitResult RefusedLimit(
        SymbolicPolynomialCalculusStatus status) =>
        new(status, null, null, null, null);
}
