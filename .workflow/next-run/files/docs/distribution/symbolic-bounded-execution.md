# Symbolic exact rules and bounded execution — local preview.4

This package is a **provider-neutral, narrow and executable exact symbolic IR** component, not a general CAS.

The new SymbolicBoundedExecution.Execute API dispatches only four previously qualified rules: ExpandSquare, FactorSquare, SubstituteFree (free real symbol and exact/scoped-safe replacement), and NormalizeRealSqrtSquare. It returns a typed SymbolicBoundedExecutionResult with a meaningful status, optional expression, local rule provenance and counted input/output work.

Budgets are finite, explicit and caller-provided: maximum 1024 nodes per input/output IR (default 128); depth 32 (default 16); total expanded work units 2048 (default 256); elapsed deadline at most 30 seconds (default five seconds). An exhausted budget, cancellation or expired deadline always reports a refusal with **no returned mathematical expression**, rather than partial success. The elapsed deadline checks before and after a synchronous, already-bounded local rule; it is a **soft** deadline and cannot forcibly interrupt a CPU-blocked call. Hard termination was only experimentally qualified in C01's separate process host, and is not used as an in-process execution promise.

The admitted provenance includes a qualified versioned rule ID, source math-ir/1 semantic structural digest, node/work counts and measured elapsed time. A digest identifies the **input representation**, not mathematical equivalence. It does not encode external provider validity or permit an agent to call an API.

Independent admission requires one compiled absent-API RED against preview.3, 26 GREEN tests for relevant mathematical and operational boundaries, and the complete symbolics/IR/quantities regression suites. Eight package-only C# consumer probes verify ordinary expansion, proof provenance, bounded work, cancellation, zero deadline, scope refusal, real abs(x) semantics and no foreign provider/broker reference.

The three observed local packages form a provider-free transitive NuGet graph:
dw.quantities 0.2.0-preview.1 -> dw.tools.math.ir 0.3.0-preview.1 -> dw.tools.math.symbolics 0.5.0-preview.4. A separate C# inspector records fresh archive bytes, manifest dependencies and SHA-256; packages are not publicly published.

Successful RS058 closes the M4-W01 symbolic provider/transform work package, not the M4 symbolic release. M4-W02 still owns formal symbolic calculus and solving, with subsequent independent gate G-M4.
