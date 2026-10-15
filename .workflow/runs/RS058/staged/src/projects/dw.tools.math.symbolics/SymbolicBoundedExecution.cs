using System.Diagnostics;
using System.Threading;
using Dw.Tools.Math.Ir;

namespace Dw.Tools.Math.Symbolics;

/// <summary>The only operations admitted by the local bounded T2 dispatcher.</summary>
public enum SymbolicBoundedOperation
{
    ExpandSquare,
    FactorSquare,
    SubstituteFree,
    NormalizeRealSqrtSquare
}

/// <summary>A typed refusal never carries a mathematical expression or proof.</summary>
public enum SymbolicBoundedStatus
{
    Admitted,
    Unsupported,
    ContradictoryAssumptions,
    BudgetExceeded,
    Cancelled,
    DeadlineExceeded
}

/// <summary>
/// Caller-declared finite, conservative managed in-process work limits.
/// MaxElapsed is checked before and after the already-bounded synchronous
/// operation. It is NOT a hard preemptive deadline or a secure sandbox.
/// </summary>
public sealed class SymbolicWorkBudget
{
    public SymbolicWorkBudget(
        int maximumNodes = 128, int maximumDepth = 16,
        int maximumWorkUnits = 256, TimeSpan? maximumElapsed = null)
    {
        if (maximumNodes is < 1 or > IrGraphLimits.MaximumNodes)
            throw new ArgumentOutOfRangeException(nameof(maximumNodes));
        if (maximumDepth is < 1 or > IrGraphLimits.MaximumDepth)
            throw new ArgumentOutOfRangeException(nameof(maximumDepth));
        if (maximumWorkUnits is < 1 or > 2 * IrGraphLimits.MaximumNodes)
            throw new ArgumentOutOfRangeException(nameof(maximumWorkUnits));
        var elapsed=maximumElapsed ?? TimeSpan.FromSeconds(5);
        if (elapsed < TimeSpan.Zero || elapsed > TimeSpan.FromSeconds(30))
            throw new ArgumentOutOfRangeException(nameof(maximumElapsed));
        MaximumNodes = maximumNodes;
        MaximumDepth = maximumDepth;
        MaximumWorkUnits = maximumWorkUnits;
        MaximumElapsed = elapsed;
    }

    public int MaximumNodes { get; }
    public int MaximumDepth { get; }
    public int MaximumWorkUnits { get; }
    public TimeSpan MaximumElapsed { get; }
}

/// <summary>Only an admitted status exposes the locally qualified expression.</summary>
public sealed class SymbolicBoundedExecutionResult
{
    internal SymbolicBoundedExecutionResult(
        SymbolicBoundedStatus status, IrNode? expression, string? proofRule,
        string? sourceSemanticSha256, int inputNodes, int outputNodes, TimeSpan elapsed)
    {
        Status=status;
        Expression=expression;
        ProofRule=proofRule;
        SourceSemanticSha256=sourceSemanticSha256;
        InputNodes=inputNodes;
        OutputNodes=outputNodes;
        Elapsed=elapsed;
    }

    public SymbolicBoundedStatus Status { get; }
    public IrNode? Expression { get; }
    public string? ProofRule { get; }
    public string? SourceSemanticSha256 { get; }
    public int InputNodes { get; }
    public int OutputNodes { get; }
    public int WorkUnits => InputNodes+OutputNodes;
    public TimeSpan Elapsed { get; }
    public bool IsAdmitted =>
        Status == SymbolicBoundedStatus.Admitted &&
        Expression is not null && ProofRule is not null &&
        SourceSemanticSha256 is not null;
}

/// <summary>
/// Bounded dispatch over four independent exact local mathematical rules.
/// It never parses provider text, invokes supplied delegates, evaluates
/// unknown symbolic nodes or promotes an incomplete result to a success.
/// </summary>
public static class SymbolicBoundedExecution
{
    public static SymbolicBoundedExecutionResult Execute(
        SymbolicBoundedOperation operation, IrNode source,
        SymbolicWorkBudget budget,
        IrSymbol? symbol = null, IrNode? replacement = null,
        IrAssumptionSet? assumptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(budget);
        var started=Stopwatch.GetTimestamp();
        SymbolicBoundedExecutionResult Refuse(SymbolicBoundedStatus status,
            int inputNodes=0, int outputNodes=0) =>
            new(status,null,null,null,inputNodes,outputNodes,
                Stopwatch.GetElapsedTime(started));

        if (cancellationToken.IsCancellationRequested)
            return Refuse(SymbolicBoundedStatus.Cancelled);
        if (budget.MaximumElapsed==TimeSpan.Zero ||
            Stopwatch.GetElapsedTime(started)>budget.MaximumElapsed)
            return Refuse(SymbolicBoundedStatus.DeadlineExceeded);
        if (!Enum.IsDefined(operation))
            return Refuse(SymbolicBoundedStatus.Unsupported);

        // Operation-specific input surfaces reject otherwise ignored
        // arguments; an extra free symbol or irrelevant assumptions must
        // never silently alter the mathematical meaning of an operation.
        if (operation==SymbolicBoundedOperation.SubstituteFree)
        {
            if (symbol is null || replacement is null || assumptions is not null)
                return Refuse(SymbolicBoundedStatus.Unsupported);
        }
        else if (symbol is not null || replacement is not null ||
            (operation!=SymbolicBoundedOperation.NormalizeRealSqrtSquare &&
                assumptions is not null))
            return Refuse(SymbolicBoundedStatus.Unsupported);

        if (!TryMeasure(source,budget,out var inputNodes))
            return Refuse(SymbolicBoundedStatus.BudgetExceeded);
        if (inputNodes>budget.MaximumWorkUnits)
            return Refuse(SymbolicBoundedStatus.BudgetExceeded,inputNodes);
        if (cancellationToken.IsCancellationRequested)
            return Refuse(SymbolicBoundedStatus.Cancelled,inputNodes);
        if (Stopwatch.GetElapsedTime(started)>budget.MaximumElapsed)
            return Refuse(SymbolicBoundedStatus.DeadlineExceeded,inputNodes);

        SymbolicTransformResult transform;
        try
        {
            transform=operation switch
            {
                SymbolicBoundedOperation.ExpandSquare =>
                    SymbolicExactTransformations.ExpandSquare(source),
                SymbolicBoundedOperation.FactorSquare =>
                    SymbolicExactTransformations.FactorSquare(source),
                SymbolicBoundedOperation.SubstituteFree =>
                    SymbolicExactTransformations.SubstituteFree(source,symbol!,replacement!),
                SymbolicBoundedOperation.NormalizeRealSqrtSquare =>
                    SymbolicExactTransformations.NormalizeRealSqrtSquare(
                        source,assumptions ?? IrAssumptionSet.Empty),
                _ => throw new InvalidOperationException("An unrecognized operation was dispatched.")
            };
        }
        catch (ArgumentOutOfRangeException)
        {
            return Refuse(SymbolicBoundedStatus.BudgetExceeded,inputNodes);
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refuse(SymbolicBoundedStatus.Unsupported,inputNodes);
        }

        // Even if managed work finished, a cancellation or over-deadline
        // completion is a typed refusal, never a successful partial result.
        if (cancellationToken.IsCancellationRequested)
            return Refuse(SymbolicBoundedStatus.Cancelled,inputNodes);
        if (Stopwatch.GetElapsedTime(started)>budget.MaximumElapsed)
            return Refuse(SymbolicBoundedStatus.DeadlineExceeded,inputNodes);

        if (!transform.IsAdmitted || transform.Expression is null ||
            transform.ProofRule is null)
            return Refuse(transform.Status switch
            {
                SymbolicTransformStatus.ContradictoryAssumptions =>
                    SymbolicBoundedStatus.ContradictoryAssumptions,
                SymbolicTransformStatus.BudgetExceeded =>
                    SymbolicBoundedStatus.BudgetExceeded,
                _ => SymbolicBoundedStatus.Unsupported
            },inputNodes);

        if (!TryMeasure(transform.Expression,budget,out var outputNodes) ||
            inputNodes+outputNodes>budget.MaximumWorkUnits)
            return Refuse(SymbolicBoundedStatus.BudgetExceeded,inputNodes);

        // These rule identifiers are qualified in T1, not provider-supplied
        // arbitrary provenance. Never certify an unknown transformation rule.
        if (transform.ProofRule is not (
            SymbolicExactTransformations.ExpandRule or
            SymbolicExactTransformations.FactorRule or
            SymbolicExactTransformations.SubstitutionRule or
            SymbolicExactTransformations.SqrtRule))
            return Refuse(SymbolicBoundedStatus.Unsupported,inputNodes,outputNodes);

        string provenance;
        try
        {
            provenance=IrCanonicalHashes.SemanticStructuralSha256(source);
        }
        catch (Exception error) when (error is ArgumentException or FormatException
            or NotSupportedException)
        {
            return Refuse(SymbolicBoundedStatus.Unsupported,inputNodes,outputNodes);
        }
        if (cancellationToken.IsCancellationRequested)
            return Refuse(SymbolicBoundedStatus.Cancelled,inputNodes,outputNodes);
        if (Stopwatch.GetElapsedTime(started)>budget.MaximumElapsed)
            return Refuse(SymbolicBoundedStatus.DeadlineExceeded,inputNodes,outputNodes);

        return new(SymbolicBoundedStatus.Admitted,transform.Expression,
            transform.ProofRule,provenance,inputNodes,outputNodes,
            Stopwatch.GetElapsedTime(started));
    }

    private static bool TryMeasure(
        IrNode root,SymbolicWorkBudget budget,out int count)
    {
        count=0;
        var queue=new Stack<(IrNode Node,int Depth)>();
        queue.Push((root,1));
        while(queue.Count>0)
        {
            var (node,depth)=queue.Pop();
            if (depth>budget.MaximumDepth || ++count>budget.MaximumNodes)
                return false;
            switch(node)
            {
                case IrApply app:
                    foreach(var arg in app.Arguments)
                        queue.Push((arg,depth+1));
                    break;
                case IrRestrictedExpression condition:
                    queue.Push((condition.Expression,depth+1));
                    break;
                case IrMatrix matrix:
                    foreach(var item in matrix.Elements)
                        queue.Push((item,depth+1));
                    break;
            }
        }
        return true;
    }
}
