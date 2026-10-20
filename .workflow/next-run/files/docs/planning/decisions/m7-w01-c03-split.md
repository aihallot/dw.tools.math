# M7-W01-C03 — bounded size-L split before activation

## Question

How can the accepted local DAG and PlotModel research be decomposed before any size-L phase is made ready, while preserving its original M7-W01-C03 identity, acceptance oracles, AURA ownership boundary and future release gate?

## Accepted split

The original `M7-W01-C03` remains the **size-L aggregate research phase**. Its two original Q/O/D tasks remain unchanged and **not-ready**; it cannot close until its two explicit split child phases complete. The phase stays planned during RS110.

1. `M7-W01-C03-S01` — **Bounded pure DAG sharing and cycle refusal**, size M, parent chunk `M7-W01-C03`. Dependency `M7-W01-C02` (the durable RS109 tensor research decision), not on a missing precision or Tensor API. Two bounded Q/O/D research tasks remain: T1 contract/oracles and T2 boundaries/ownership. Next run independently tests a diamond DAG with one shared pure value, cycle refusal and explicit finite node/edge/depth quotas. No executor, job scheduler, asynchronous workflow engine or side-effect API.
2. `M7-W01-C03-S02` — **Finite PlotModel and discontinuity sampling**, size M, parent chunk `M7-W01-C03`. Dependency `M7-W01-C03-S01`. Two bounded Q/O/D research tasks remain: T1 deterministic numeric sampling/segment-break witnesses for `f(x)=1/x`, T2 shape/point quotas, invalid input, partial-result semantics and owner review. A segment may **not** connect across `x=0`. No UI chart renderer, AURA task engine, online service or graphics package is admitted.

The original parent acceptance preserves **both** independent witness families (diamond sharing/cycle and discontinuity), and the parent boundary still requires explicit caps and prohibits importing AURA scheduling. No new source package or deployed capability is created by the split.

## DWF structural and product plan requirements

- Stable mapping remains `release → milestone`, `work_package → workPackage`, `chunk → phase`, `task → task`, `subtask → subtask`.
- Introduce **2 split phases + 4 tasks + 12 Q/O/D subtasks = 18 native DWF nodes**. Baseline count is 523; refined full native count must be exactly **541**. Preserve every existing node and ID. These are planned future identities, **not** fabricated historical completions.
- Child chunks inherit the accepted `M7-W01-C03-R1` and `M7-W01-C03-R2` requirements via `parent_chunk`, without inventing a second independent requirement authority. Their exact existing split dependencies are declared in both backlog and native DWF plan.
- Amend M7 `required_chunks` to include S01 and S02. The accepted C03 aggregate remains listed. The native projection and rendered planning pages are regenerated inside the controlled DWF mutation.
- S01 becomes the sole **active research phase** at the end of RS110, ready for independent T1 evaluation in RS111. S02 stays `not-ready`, and the parent C03 stays `not-ready`. Neither receives a `done` achievement in RS110. No previously completed status changes.
- The BCL-only independent split checker `docs/planning/ValidateM7DagSplit.cs` checks full backlog/native parity, each split identity/parent/dependency, the exact ready boundary, zero new product capabilities and the older C01/C02 closure evidence. This is an observational planning oracle, not a computational DAG engine.

## Rejected alternatives and resource budget

Do **not** prematurely activate the original L-sized C03, silently reduce its size to M, or merge DAG calculation with graphics or orchestration. Do **not** insert a product implementation, promise a qualified DAG or PlotModel API, or infer an AURA adoption from Math planning.

One controlled RS110 authoring pass, one staged decision, one read-only C# split checker, planning regeneration and native DWF validation are sufficient. No third-party package restore, CI, external request transmission, full solution tests or sibling repository modifications are authorized.

**Next exact scope:** RS111 on `M7-W01-C03-S01-T1` after RS110 is durably pushed, with independently checked diamond DAG sharing, cycle refusal and finite quotas. Later PlotModel work is gated by S01; aggregate parent C03 closure remains separate and must not misrepresent research as delivered Math runtime functionality.
