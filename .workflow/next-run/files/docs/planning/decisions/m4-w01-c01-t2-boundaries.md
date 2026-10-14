# M4-W01-C01-T2 — symbolic provider containment and assumption boundary

**Status:** RS054 proposed boundary qualification; the technical and scientific scope becomes accepted only after its own successful DWF report and evidence. A Math-owned experimental host is NOT a public symbolic product, provider adapter, AURA action, or broker permission.

## Question: what can safely be delegated to a symbolic engine?

T1 demonstrated AngouriMath 2.5.0 on .NET 10 for a short derivative, elementary roots, and a text-rendered conditional simplification. It did not prove that the rendered Angouri "provided" clause can be translated into typed Math IR, that cancellations are cooperative, that expensive simplification will terminate, or that general real/complex assumptions retain meaning.

The accepted T2 requirement is to bound **a deliberately tiny, fixed-input experiment**, reject input or outcomes outside its representable mathematical guarantees and record operational limits without forcing a Python/runtime dependency. Three alternatives were evaluated:

1. **Selected experimental strategy:** separate AngouriMath into a child .NET process, with a parent-controlled deadline, cancellation request and process-tree termination. Admission limits are conservative and the mathematical input corpus is **closed**, not an unbounded parser.
2. **Rejected for unconditional use:** directly run arbitrary provider simplifications inside Math and assert that CancellationToken or a symbolic API protects against non-cooperative computation. Such a guarantee is not demonstrated.
3. **Deferred:** a distributed/remote or Python-dependent CAS. There is no owner authorization, installed engine or reproducible acceptance evidence requiring this at the current M4 boundary.

## Observation: falsifiable standalone host experiments

The external-only .NET 10 test project at tests/providers/m4-w01-c01-boundaries/ references exactly AngouriMath 2.5.0 and no Math product or AURA/Decision/MCDM project. It is excluded from dw.tools.math.slnx. The executable itself acts as a parent and as a fixed-case child worker to demonstrate:
- Independent input admission: at most 128 characters, parenthesis nesting at most 8 and 64 non-whitespace token characters. Only the two fixed mathematical expressions x^2 - 1 and (x^2 - 1)/(x - 1) are executable; invalid characters, empty input, unknown variables and unknown domain/codomain assumptions return **Invalid**, **BudgetExceeded** or **Unsupported** before a CAS invocation. These are **prototype controls**, not a new general public expression parser.
- Exact ordinary worker: derivative of x^2-1 has zero symbolic residual against 2*x. The parent observes its marker only on successful child termination.
- Conditional worker: the simplified quotient must still expose Angouri's observable "provided" text for the original exclusion x != 1. The separate finite-point guard refuses x=1. **No text-to-Math-IR translation is claimed.** If the condition cannot be typed and preserved across a broker/adapter, it remains **Unsupported**, not a declared unconditional success.
- A deliberately sleeping child is killed by the parent on a finite host deadline. A second delayed child is terminated after a host cancellation request. Both results are typed and have **no successful mathematical value**.
- An independently failing child exits nonzero and is typed **ProviderFailure** instead of being interpreted as an incomplete but valid result.

All 14 named boundary probes must pass and emit one exact marker:
dw.tools.math/m4-symbolic-boundaries/0.5 qualified; probes=14; timeout=host; cancellation=host; ir=unsupported

Before running the boundary harness, the T2 package is restored to an isolated local NuGet cache. A separately staged C# archive verifier pins its actual bytes to the **observed RS053 AngouriMath 2.5.0 archive SHA-256** and verifies MIT package metadata plus the observed five direct dependencies. Rebuilding or obtaining different bytes is not silently accepted. The independent Math IR and exact quantities regressions plus native/product planning and transfer architecture validators must continue passing.

## Decision and hard limitations

Admit only the **experimental evidence** that a host can reject known invalid expressions and terminate its own child process on deadline/cancellation, with clear separation from mathematically conditional provider text. Angouri remains a *provisional symbolic candidate* for a future typed adapter; it is not a guaranteed cooperative cancellation engine. The process supervision tested here is NOT a security sandbox or a universal memory/CPU quota; host-level process termination may itself have platform-specific behavior, and only the observed Windows win-x64 runtime is qualified.

The following remain **unsupported/unqualified**: general symbolic expressions, real-vs-complex principal branches, simplification of sqrt(x^2) without explicit |x|/x>=0 assumptions, correct typed IR condition translation, arbitrary provider predicate expressions, universal solver completeness, native AOT, non-Windows process-tree termination, provider thread safety, memory isolation and global execution permission.

The precise next product responsibility is **M4-W01-C02**, in which a typed Math symbolic IR bridge must preserve conditions structurally, refuse evaluation at x=1 even after cancellation, and never accept opaque provider text as mathematically verified. That work requires a controlled RED/GREEN and its own package/API contract.

**Closure gate:** RS054 may mark T2 A/B/C, T2 and C01 done only after the observable host boundaries, pinned package provenance, project isolation, independent tests and plans pass. M4 and M4-W01 stay in-progress, and C02 remains not-ready until a separately prepared run.
