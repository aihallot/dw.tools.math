# M7 coverage and ownership inventory — T1 bounded snapshot

**Scope:** canonical `docs/planning/backlog.json` and locally recorded coordination state at RS120 preparation. No external recipient response has been confirmed by this audit; no product approval, public release or migration is implied.

## Structural and provenance counts

| Canonical inventory | Count / disposition | What it means |
|---|---|---|
| Accepted requirement IDs | **106** | Every requirement has one existing chunk owner and accepted release |
| Distinct requirement owner chunks | **53** | Original accepted owners; the added C03 split phases remain separate stable plan nodes |
| Framing source-coverage entries | **20** | Each entry has targets in the accepted chunk roster, not delivered-feature proof |
| Domain dispositions | **6** | 4 externally owned, 2 excluded, with reasons and review triggers |
| External-dependency gates | **4 satisfied** | Math-side local evidence only; not external consumer adoption |
| Requests to owning recipients | **3 unsent drafts** | AURA, Decision, MCDM; no acceptance or transfer inferred |
| Backlog scope at RS119 | 7 releases done, M7 active; 16 work packages done, W02 active; 57 chunks done, C02 planned | Counts are baseline snapshots, recomputed by the read-only checker |
| Native DWF roadmap | **541 nodes** | Must preserve milestone/workPackage/phase/task/subtask IDs and observed states |

The independent checker `docs/planning/ValidateM7CoverageInventory.cs` validates these cardinalities, source targets, owner references, Q/O/D scope, and exact baseline/target version. It also checks prior W01/C01 source SHA identities.

## Explicit ownership/disposition matrix

| Scope | Owner | Accepted status |
|---|---|---|
| AURA hosts, agent capabilities, permissions, scheduling, renderer | AURA | **Externally owned**; Math cannot mutate or certify client adoption |
| Decision optimizers, solver/SMT problems, risk/policies | Decision | **Externally owned**; generic Math primitives require recipient tests |
| MCDM preference, ranking, ties and flow conventions | MCDM | **Externally owned**; rank/flow changes need recipient acceptance |
| Nutrition, storage, localization framework and domain policy | Relevant domain maintainer | **Externally owned** |
| Universal home-grown theorem prover | Math boundary | **Excluded**; only bounded external interoperability research admitted |
| Unlimited parser, arbitrary macros or unbounded engine | Math boundary | **Excluded** pending new explicit scope decision |

All dispositions have an owner, reason, review trigger and target; they do not authorize changing another repository.

## Pending external requests — no invented answers

| Request document | Intended recipient | Durable observed status |
|---|---|---|
| `docs/coordination/requests/MATH-XR-001-aura.md` | AURA | **Draft, not sent**; pinned historical source/request needs recipient recheck |
| `docs/coordination/requests/MATH-XR-002-decision.md` | Decision | **Draft, not sent**; historical Math-local packages do not mean Decision cutover |
| `docs/coordination/requests/MATH-XR-003-mcdm.md` | MCDM | **Draft, not sent**; rank/flow ownership and semantics protected |

`docs/coordination/requests/README.md` explicitly marks initial requests as unsent. Local `EXT-*` gate satisfaction is not external acknowledgment. A future authorized transmission needs current recipient revision, exact artifacts/hashes, compatibility tests, approval and rollback; no automatic emailing or cross-repository commit.

## Deferred research families and possible later program

- **M7-W01** research completed: precision/interval, tensor, pure DAG, finite PlotModel; public implementations/providers deferred with named-consumer/resource/rights/source-free triggers.
- **M7-W02-C01** research completed: one exact finite `Z[x]` coefficient certificate and invalid/unproved cases; Lean/Rocq/SMT kernel/toolchain and general formal APIs deferred.
- **M7-W02-C02-T2** must decide a bounded post-research program, with honest distinction between delivered products, evidence-backed research, deferred provider ideas, excluded expansion and external owners.
- **Potential next cycles only if explicitly approved:** targeted consumer-reported mathematical gaps; formal/provider toolchain qualification; receipt-based recipient migration; platform and redistribution rights; scientific/release gates. These are proposals, not scheduled milestones or new packages.

**No release decision:** M7 remains in-progress, C02-T1 is not the M7 gate, and no public NuGet or external provider is claimed. The source-of-truth validator is local C#, no Python or paid CI.
