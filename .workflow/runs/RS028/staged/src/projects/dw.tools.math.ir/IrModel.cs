using System.Collections.Immutable;
using dw.quantities;

namespace Dw.Tools.Math.Ir;

/// <summary>Closed mathematical model; no implicit numeric widening or provider execution.</summary>
public abstract record IrNode;
public abstract record IrScalar : IrNode;

public sealed record IrExactScalar(ExactRational Value) : IrScalar;

public sealed record IrFiniteBinary64Scalar : IrScalar
{
    private IrFiniteBinary64Scalar(long ieee754Bits) => Ieee754Bits = ieee754Bits;
    public long Ieee754Bits { get; }
    public double Value => BitConverter.Int64BitsToDouble(Ieee754Bits);

    public static IrFiniteBinary64Scalar FromDouble(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Only finite binary64 values are admitted.");
        return new(BitConverter.DoubleToInt64Bits(value));
    }
}

public enum IrScalarDomain { Real, Integer }
public enum IrTemperatureKind { None, Absolute, Interval }

public sealed record IrSymbol : IrNode
{
    private IrSymbol(string identity, string displayName, IrScalarDomain domain,
        string? scopeId, int slot)
    {
        Identity = identity;
        DisplayName = displayName;
        Domain = domain;
        ScopeId = scopeId;
        Slot = slot;
    }

    public string Identity { get; }
    public string DisplayName { get; }
    public IrScalarDomain Domain { get; }
    public string? ScopeId { get; }
    public int Slot { get; }
    public bool IsBound => ScopeId is not null;

    public static IrSymbol Free(string identity, string displayName,
        IrScalarDomain domain = IrScalarDomain.Real)
    {
        ValidateText(identity, nameof(identity));
        ValidateText(displayName, nameof(displayName));
        ValidateDomain(domain);
        return new("free:" + identity, displayName, domain, null, -1);
    }

    public static IrSymbol Bound(string scopeId, int slot, string displayName,
        IrScalarDomain domain = IrScalarDomain.Real)
    {
        ValidateText(scopeId, nameof(scopeId));
        ValidateText(displayName, nameof(displayName));
        ValidateDomain(domain);
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        return new("bound:" + scopeId + ":" + slot.ToString(System.Globalization.CultureInfo.InvariantCulture),
            displayName, domain, scopeId, slot);
    }

    internal static void ValidateText(string value, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, name);
        if (value.Length > 128)
            throw new ArgumentOutOfRangeException(name, "IR identifiers and labels are limited to 128 characters.");
    }

    private static void ValidateDomain(IrScalarDomain domain)
    {
        if (!Enum.IsDefined(domain))
            throw new ArgumentOutOfRangeException(nameof(domain));
    }
}

public sealed record IrQuantityLiteral : IrNode
{
    public IrQuantityLiteral(IrScalar value, string canonicalUnitId, UnitSystem system,
        DimensionVector dimension, IrTemperatureKind temperature = IrTemperatureKind.None)
    {
        ArgumentNullException.ThrowIfNull(value);
        IrSymbol.ValidateText(canonicalUnitId, nameof(canonicalUnitId));
        if (!Enum.IsDefined(system)) throw new ArgumentOutOfRangeException(nameof(system));
        if (!Enum.IsDefined(temperature)) throw new ArgumentOutOfRangeException(nameof(temperature));
        Value = value;
        CanonicalUnitId = canonicalUnitId;
        System = system;
        Dimension = dimension;
        Temperature = temperature;
    }
    public IrScalar Value { get; }
    public string CanonicalUnitId { get; }
    public UnitSystem System { get; }
    public DimensionVector Dimension { get; }
    public IrTemperatureKind Temperature { get; }
}

public enum IrOperation { Add, Subtract, Multiply, Divide, Power, Square, Sqrt, Abs, Min, Max }

public sealed record IrApply : IrNode
{
    private IrApply(IrOperation operation, ImmutableArray<IrNode> arguments)
    {
        Operation = operation;
        Arguments = arguments;
    }

    public IrOperation Operation { get; }
    public ImmutableArray<IrNode> Arguments { get; }

    public static IrApply Create(IrOperation operation, IEnumerable<IrNode> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (!Enum.IsDefined(operation)) throw new ArgumentOutOfRangeException(nameof(operation));
        var nodes = arguments.Take(33).ToImmutableArray();
        if (nodes.Any(node => node is null))
            throw new ArgumentException("IR operands cannot be null.", nameof(arguments));
        var arity = operation is IrOperation.Sqrt or IrOperation.Square or IrOperation.Abs ? 1
            : operation is IrOperation.Min or IrOperation.Max or IrOperation.Add or IrOperation.Multiply ? -2 : 2;
        if (arity == -2 ? nodes.Length < 2 || nodes.Length > 32 : nodes.Length != arity)
            throw new ArgumentException("IR operation has an unsupported arity.", nameof(arguments));
        return new(operation, nodes);
    }
}

public enum IrRelationKind { Equal, NotEqual, GreaterOrEqual }

public sealed record IrRelation
{
    public IrRelation(IrSymbol symbol, IrRelationKind kind, IrExactScalar right)
    {
        ArgumentNullException.ThrowIfNull(symbol);
        ArgumentNullException.ThrowIfNull(right);
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        Symbol = symbol;
        Kind = kind;
        Right = right;
    }
    public IrSymbol Symbol { get; }
    public IrRelationKind Kind { get; }
    public IrExactScalar Right { get; }
}

public sealed record IrAssumptionSet
{
    private IrAssumptionSet(ImmutableArray<IrRelation> conditions) => Conditions = conditions;
    public ImmutableArray<IrRelation> Conditions { get; }
    public static IrAssumptionSet Empty { get; } = new(ImmutableArray<IrRelation>.Empty);
    public static IrAssumptionSet Create(IEnumerable<IrRelation> conditions)
    {
        ArgumentNullException.ThrowIfNull(conditions);
        var items = conditions.Take(33).ToImmutableArray();
        if (items.Length > 32 || items.Any(x => x is null))
            throw new ArgumentException("IR assumptions must be non-null and have at most 32 entries.", nameof(conditions));
        return new(items);
    }

    public bool DeclaresNonNegative(IrSymbol symbol) =>
        Conditions.Any(x => x.Symbol.Identity == symbol.Identity &&
            x.Kind == IrRelationKind.GreaterOrEqual &&
            x.Right.Value == ExactRational.Zero);
}

public sealed record IrRestrictedExpression : IrNode
{
    public IrRestrictedExpression(IrNode expression, IrAssumptionSet assumptions)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(assumptions);
        Expression = expression;
        Assumptions = assumptions;
    }
    public IrNode Expression { get; }
    public IrAssumptionSet Assumptions { get; }
}

/// <summary>A single sound real-domain rule, not a general symbolic solver.</summary>
public static class IrRealDomainRules
{
    public static IrNode NormalizeSqrtOfSquare(IrApply squareRoot, IrAssumptionSet assumptions)
    {
        ArgumentNullException.ThrowIfNull(squareRoot);
        ArgumentNullException.ThrowIfNull(assumptions);
        if (squareRoot.Operation != IrOperation.Sqrt ||
            squareRoot.Arguments[0] is not IrApply { Operation: IrOperation.Square } square ||
            square.Arguments[0] is not IrSymbol symbol ||
            symbol.Domain is not (IrScalarDomain.Real or IrScalarDomain.Integer))
            throw new ArgumentException("Only sqrt(square(real symbol)) is admitted.", nameof(squareRoot));

        return assumptions.DeclaresNonNegative(symbol)
            ? symbol
            : IrApply.Create(IrOperation.Abs, [symbol]);
    }
}
