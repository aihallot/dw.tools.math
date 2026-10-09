using System.Collections.Immutable;

namespace Dw.Tools.Math.Ir;

/// <summary>Finite expanded tree budget, counting shared DAG nodes at each use.</summary>
public static class IrGraphLimits
{
    public const int MaximumNodes = 1024;
    public const int MaximumDepth = 32;
    public const int MaximumMatrixElements = 4096;

    public static void Validate(IrNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        var pending = new Stack<(IrNode Node, int Depth)>();
        pending.Push((root, 1));
        var visited = 0;
        while (pending.Count != 0)
        {
            var (node, depth) = pending.Pop();
            if (depth > MaximumDepth)
                throw new ArgumentOutOfRangeException(nameof(root),
                    "IR graph depth exceeds 32.");
            if (++visited > MaximumNodes)
                throw new ArgumentOutOfRangeException(nameof(root),
                    "IR expanded-node count exceeds 1024.");

            switch (node)
            {
                case IrApply application:
                    Push(application.Arguments, depth);
                    break;
                case IrRestrictedExpression restricted:
                    pending.Push((restricted.Expression, depth + 1));
                    break;
                case IrMatrix matrix:
                    Push(matrix.Elements, depth);
                    break;
            }
        }

        void Push<T>(ImmutableArray<T> nodes, int parentDepth) where T : IrNode
        {
            if (nodes.IsDefault)
                throw new ArgumentException("Default uninitialized IR collection is not admissible.", nameof(root));
            for (var index = nodes.Length - 1; index >= 0; index--)
            {
                var child = nodes[index] ?? throw new ArgumentException(
                    "Null IR graph child is forbidden.", nameof(root));
                pending.Push((child, parentDepth + 1));
            }
        }
    }
}
