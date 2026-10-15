# M4-W01-C03-T2 — bounded exact transformation execution and provenance

**Status:** proposed for RS058, not yet an executed qualification or public Math release. C03 and M4-W01 close only after this scoped DWF run's evidence and validators succeed.

## Accepted mathematical and operational boundary

RS057 proved four provider-neutral rules in dw.tools.math.symbolics 0.5.0-preview.3: exact binomial-square expansion, inverse factor for the same canonical shape, closed free-symbol substitution refusing bound scopes or restricted sources, and over-real sqrt(x²)=abs(x), allowing restricted x only when an explicit x>=0 assumption is retained. These are **not** a general CAS, a proof of arbitrary algebraic equivalence, or an interpreter for external symbolic AST/text.

T2 adds a caller-controlled **synchronous bounded dispatcher**, not a new mathematical algorithm:

- The only dispatch choices are ExpandSquare, FactorSquare, SubstituteFree, and NormalizeRealSqrtSquare. Operation parameters are explicit; irrelevant extra parameters, unknown operation IDs, unproved shapes, and unknown scopes are Unsupported. No arbitrary delegate, provider string, untrusted callback or script is invoked.
- A validated SymbolicWorkBudget bounds expanded input **and output** node count (1..1024), depth (1..32), combined input+output work units (1..2048), and elapsed time (zero through 30 seconds). Defaults are 128 input/output nodes, depth 16, combined work 256 and five seconds. BudgetExceeded is distinct from Unsupported.
- A CancellationToken requested before or observed during/after the bounded synchronous operation yields Cancelled with no value or proof. A deadline exhausted before or observed after the operation yields DeadlineExceeded with no value or proof. This is **best-effort, checked before/after**, not a preemptive hard real-time guarantee. The C01 experimental child-process host, rather than this in-process wrapper, is the separately observed place where forced process-tree termination was shown on win-x64.
- All admitted results carry exactly one known, versioned T1 local rule ID, a mathematically structural semantic SHA-256 of the original **typed IR** (not a workflow, project or package hash), input/output node counts, combined work units and observed elapsed time. The provenance digest is a structural identity, **not** an equivalence theorem or provider attestation.
- The dispatcher never converts provider print output into a proof, never silently widens an ExactRational into binary64, and never emits a mathematical expression/proof/source hash for Unsupported, ContradictoryAssumptions, BudgetExceeded, Cancelled, or DeadlineExceeded.

The accepted assumption conditions and bound/free identity restrictions from T1 remain unchanged. The bounded dispatcher cannot invent a general capture-avoiding binder substitution engine, real/complex principal-branch semantics, or an external provider cancellation guarantee.

## Alternatives considered

1. Chosen: a limited exact typed IR dispatcher wrapping the already-qualified finite local rules, with independently observed input/output budgets, status refusals, cancellation and soft deadline checks.
2. Rejected: a generic recursive symbolic simplifier, reflection-based foreign operation invocation, guessed equivalence, silent fallback to double, or direct opaque external CAS result execution.
3. Deferred: hard preemptive in-process interruption, secure resource sandbox, arbitrary memory/CPU quotas, external CAS process integration, and a full agent/AURA/dw.discovery permission model.

## Falsification and run plan

A compiled reflection RED runs against the real preview.3 symbolics product, before installing the bounded dispatcher and preview.4. Failure must be the precise absence of its public Execute API, not an assembly/build/restore failure. After RED evidence is recorded, the new source and 26 GREEN tests are installed; require **27 targeted passing tests**, plus the relevant full symbolics/IR/quantities suites.

GREEN tests exercise the hand-verifiable expansion/factor forms, preserved x!=1/sign restrictions, false rule and unexpected-parameter refusal, exactly observed input count 4/output count 7/work 11 for (x+1)², independent too-small node/depth/work limits, pre-cancelled token, zero elapsed soft deadline, cancellation precedence, unknown operations, contradictory assumptions, too-large exact coefficients, binder capture refusal, invalid budget definitions, structural provenance, and no CAS/broker assembly dependencies.

A separate .NET 10 NuGet-only consumer restores only from a local three-package feed and executes eight independent oracles for correct rules/provenance, scoped work refusals, cancellation/deadline, capture avoidance, real sqrt and dependency isolation. The package graph remains dw.quantities 0.2.0-preview.1, dw.tools.math.ir 0.3.0-preview.1, and dw.tools.math.symbolics 0.5.0-preview.4. A dedicated C# verifier inspects actual NuGet nuspec dependencies, net10 binaries and bytes/SHA-256 **only** from generated local package archives.

## Scope and nonclaims

Successful RS058 completes M4-W01-C03-T2 R/G/V, T2, C03 **and M4-W01**, because its only three accepted phases C01, C02 and C03 are then all qualified. It must leave the native/product M4 milestone in-progress and M4-W02 not-ready for later calculus/solving milestones.

No external CAS, MathNet or Angouri runtime dependency enters the product, no public NuGet feed publication occurs, and no AURA, Decision, MCDM, or neutral dw.discovery skill/broker is implemented here. A qualified bounded dispatcher on the observed Windows win-x64 runtime does not guarantee hard-stop execution, perfect operational bounds for arbitrary external input, global algebraic completeness or non-Windows resource supervision.

**Review triggers:** a new operation beyond four known rules, binder alpha-renaming, a provider-specific AST adapter with typed predicates, a stricter execution deadline requiring out-of-process supervision, or new platform/scientific requirements.
