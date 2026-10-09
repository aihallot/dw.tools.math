# Inspectable composition results — math-composition/1

M2-W02-C01-T1 introduces a provider-neutral .NET 10 package `dw.tools.math.composition`, built only on the qualified `dw.tools.math.ir` package. It does not perform mathematical execution.

The result status is exactly one of `Exact`, `Approximate`, `Unsupported` or `BudgetExceeded`. Success requires a supplied typed `IrNode` with matching numeric precision; an unsuccessful outcome contains a non-empty reason and **no** final value. Neither failure mode silently manufactures a result.

`MathValueDescriptor.Inspect` admits exact and finite-binary64 scalars and homogeneous scalar matrices. It retains matrix rows/columns and exact/approximate classification without converting values. An arbitrary expression, symbol or provider-specific result has no inspectable numerical result shape.

`MathOperationCapability.MatrixInspection` declares matrix-only input, the admitted 4096-element bound and exact/approximate support. `Accepts` checks the admitted shape; discovery is not execution, scheduling, runtime binding, or authorization. A capability advertises neither a provider implementation nor AURA permissions.

`MathResultProvenance` carries a bounded operation id and contract version, not a fabricated execution trace. The first contract provides no provider identity, guarantee of precision, resource scheduler, cancellation, global scope binder, or final algebraic result. These integration and missing-result boundaries remain for M2-W02-C01-T2.

The seven independent M2W02C01Tests cover shape, status, mismatch, absence of value, provenance and nonexecution. The first test is executed RED against a deliberately misadvertised input shape, then GREEN against the intended package. No external repository is modified.
