# Pure computation DAG — finite oracle, ownership and future admission screen

**Research only:** M7-W01-C03-S01-T1. No shipped DAG API, renderer, AURA scheduler, provider package or cross-repository adoption.

## Independent diamond and refusal matrix

| Witness | Exact finite observation | Interpretation |
|---|---|---|
| Diamond vertices | `shared=2`, `left=shared+3`, `right=shared*4`, `root=left+right` | **4** identities and **4** edges |
| Shared value | `shared` executes **once** | Pure memoized reference, not side-effectful work scheduling |
| Reference result | `left=5`, `right=8`, `root=13` | Exact checked integer arithmetic |
| Maximum observed path | **3 nodes** | Internal path reference only |
| Graph admission budgets | **8 nodes**, **10 edges**, **4 depth** | Refuse excess before numerical work |
| Cycle and self-loop | `a -> b -> a`; `a -> a` | Deterministic cycle rejection; **no partial result** |
| Unknown or duplicate ID | Missing dependency, repeated `shared` node | Refuse identity ambiguity |
| Invalid operation/overflow | Unsupported operation shape, checked `long` overflow | Typed refusal, no incomplete numerical success |

The BCL-only checker `docs/planning/ValidateM7DagAdmission.cs` implements the **test witness**, not a public Math evaluator. It operates over a finite hand-authored fixture without external dependencies.

## Ownership and provider decision

| Candidate responsibility | Math/AURA disposition | Remaining evidence |
|---|---|---|
| Pure mathematical expression nodes | Math may research immutable finite contracts | Typed domain, identity/aliases, budgets, exact oracles and consumer |
| Topological sorting and memoization | Internal reference only; no public DAG runtime | DAG object semantics and deterministic failures |
| Job scheduling, agents, execution envelopes | **AURA ownership**; no Math copy | Separate owning-agent opt-in and interoperability request |
| Point/segment PlotModel | Separate S02 research, not a graphical renderer | `1/x` pole, segment breaks, quotas, explicit nonfinite policy |
| GPU/remote/distributed computation provider | **Not admitted** | Reproducible source/version/license/RID, rights, budgets, rollback, consumer |
| Third-party graph engine | **Not admitted** | Named consumer gap, immutable provider pin and independent package-only smoke |

## Subsequent T2 admission criteria

Require a named consuming Math operation, exact finite algebra/result domain, deterministic node/edge/depth allocation policy, cycle and unknown-node rejection, cancellation/no-partial statuses, immutable references versus copies, concurrency and cache invalidation policies, source/version/license/RID evidence for any external engine, offline rollback and an independent source-free consumer.

A positive local research oracle is not an owner grant. Without the admitted consumer and these bounds, **defer** a product graph engine. Keep `M7-W01-C03-S02` not-ready until S01's independent T2 disposition and separate PlotModel sampling research.

**Stop-loss:** one source-only local C# checker, small static fixtures, DWF structural validations, no build of a new engine and no broad benchmark.
