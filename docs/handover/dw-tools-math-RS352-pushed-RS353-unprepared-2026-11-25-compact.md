# Math compact handover — RS352 pushed / RS353 unprepared

## Authority and operator contract

- Repository: `aihallot/dw.tools.math`; operator branch: `main`.
- Operator checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math`.
- Verified main result: `f0a6bc7a6364f6a8db25340f227ec50dae6f678c`, `test: qualify existing M9 bounded sampling GREEN`.
- DWF executor observed: 0.1.76; repository guidance: 0.27 (verify on resume).
- RS352 succeeded attempt 001, retry 0, first-pass true, zero previous invocation failures, clean mutation boundary, nine structural validations. No completion claimed in RS352; GREEN transitioned ready -> in-progress.
- `.aura/workflow/state.json`: M9 / M9-W01 / M9-W01-C01 / M9-W01-C01-T1 / M9-W01-C01-T1-G active; pendingRun null.
- `.workflow/next-run/request.json`: absent (404 verified on main). RS353 is not prepared. No operator command is authorized until one successor is prepared.
- This handover is a projection only; main and actual reports outrank it. Do not checkout or execute the handover branch.

Read in order: docs/workflow/mantra.md, docs/workflow/constitution.md, PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, this handover, docs/workflow/operator-loop.md. Consult docs/workflow/run-authoring-reference.md and docs/workflow/payload-sdk-reference.md as needed. Read current main authorities before authoring. The optional docs/conversation-handover-contract.json path was not found; do not rely on it.

## Exact state and next objective

Native M8 and all 39 M8 nodes done, canonical reconciliation RS348. M9 source task T1 in-progress; RED done (RS351), GREEN in-progress (RS352), Verification not-ready. T2 and its three leaves not-ready. Canonical backlog 0.1.128: M9/W01/C01/T1 in_progress, RED done with RS351 report, GREEN ready, Verification planned. Reconcile durable RS352 activation before dependent work; do not mark GREEN canonically done before its actual completion report. Use existing ValidatePlan.cs --write inside payload (not preparation-safe validation), then --check. Preserve stable hierarchy and unrelated scope.

Prepare exactly one RS353 on main: corroborate RS352 first-pass report and compiled preparation proof (including authentic-source oracle and locked full Statistics regression); project canonical GREEN activation; complete native T1-G using ConvergeNodeToDone and declare exactly one truthful achievement, then ready T1-V using TransitionNode. Confirm target re-entry. Do not repeat expensive mutant builds or modify already-working production source. M9 source verification after RS353 should independently reconcile exact source/replay evidence and only then complete T1; proceed to T2 with actual immutable candidate/consumer evidence rather than replaying publication. Close M9 only when requirements and all native completions are evidenced; reconcile canonical backlog after durable result.

## Bounded authorities for RS353

- `.aura/workflow/reports/RS352/attempt-001.json`: succeeded, first-pass, no boundary violations, GREEN activation only.
- `.workflow/runs/RS352/preparation-proof.json`: payloadCompiled true, rehearsedValidationCount 9; validation IDs include m9-rs352-authentic-source-independent-oracle, m9-rs352-statistics-locked-regression; compare complete list to contribution.observed.validations.
- `.workflow/runs/RS352/payload.cs`: proven renderer invocation and typed activation; use as reference, not a blind copy.
- `.aura/workflow/reports/RS351/attempt-001.json`: durable RED closure; RS350 report and preparation-proof establish authentic source + three compiled mutant refusals (rejection bypass, budget overrun, signed ordering).
- `docs/planning/backlog.json` and `.aura/workflow/plan/project.json`: canonical accepted scope vs native execution progress. M9 R1 source mathematical contract; R2 independent exact package identity and consumer proof.
- Existing consumers to audit for version/lifecycle expectations: docs/planning/ValidateNativeDwfAdoption.cs; docs/planning/ValidateAssessaM9Admission.cs; docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs; docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md; docs/engineering/ValidateM9AssessaPublishedHandoff.cs; docs/engineering/ValidateM9RequestDeliveryReconciliation.cs. The frontier expects the new backlog version marker in reconciliation document. Preserve historical proof; evolve exact current-state checks deliberately. docs/planning/ValidatePlan.cs is the sole renderer (generated README, dwf-map, coverage, planning-validation and M0–M9 version pages).
- Existing source: src/projects/dw.tools.math.statistics/SplitMix64Random.cs; tests/projects/dw.tools.math.statistics.tests/M9W01C01BoundedSamplingTests.cs; docs/engineering/ProbeM9BoundedSamplingRed.cs (--check-original mode for authentic source only); docs/engineering/fixtures/M9BoundedSamplingRedOracle.cs; docs/distribution/bounded-integer-sampling.md.

## Already delivered — do not redo

M8 Discovery: thirteen protocol fixtures, four selected CLR bindings and deterministic opt-in unsigned portable skill generation; no broker grant or downstream adoption. M8 modernization: 13-library audit, measured one-line ExactRational span concat integrated RS343, net11 Statistics/Quantities experimental graph rebuilt and independently consumed RS344, bounded phase/work package/milestone closed RS345–347. Historical archive protection remains. Production SDK 10.0.401/net10/C#14 is deliberately unchanged; broader migration, cross-platform/rollback and all-library qualification not authorized. Do not equate experimental net11 package with published release.

M9 historical source implementation RS268, candidate RS269, Windows net11 package-only consumer RS270, controlled private readiness/publisher RS271–272, physical private readback RS273, Assessa handover RS274–275, Linux/WSL locked binary consumer RS277 and handover RS278. All eleven relevant reports were corroborated RS349 (some had preselection failures; do not label all first-pass). M9 native RED/GREEN/Verification lifecycle was not retroactively manufactured: RS349 ready, RS350 actual mutant falsification, RS351 RED completion, RS352 actual GREEN regression. RS353 should not introduce an unnecessary product change.

Published private immutable Statistics 20260.20261120.1001: archive SHA-256 feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1, DLL SHA-256 684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea; exact Quantities dependency [20260.20261102.1001], archive SHA-256 e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d. Source-free locked Windows/Linux net11 qualified, twelve checks in Windows fixture. Never overwrite archives or republish without owner authorization. Assessa adoption EG-A-01/T002 remains unqualified on Math authority. Read docs/coordination/requests/MATH-XR-004-assessa-bounded-integers.md and docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md as needed.

## Operational discipline

Only prepare remotely; local operator runs dwf run next. Prefer typed C# payload and mirrored complete staged files, no domain logic in shell/Python. Check preconditions against current main, exact request schema, target re-entry, staged membership and bounded validations. Publish request.json last; verify prepared main tip and files. Never execute GitHub actions for local qualification. If pushed, check report, exact result commit, state and plan then prepare one successor automatically; if failed, inspect latest durable evidence/handoff and correct same run, preserve attempt count (preselection failure before allocation retains retry ordinal; allocated failure advances it). Second failure requires incident review. No assumptions based on chat. If connector write fails verify HEAD and staged files before retry; no forced overwrite. Canonical repo English, operator French. Return exact mandatory six-field handback from docs/workflow/operator-loop.md, nothing after COMMAND. Before preparation command is none; after verified request it is dwf run next. No manual pull, push or commit in normal loop.

## Resume

The next conversation must verify main, reread authorities and prepare RS353 immediately if this baseline remains current. If changed, reconcile actual current authority instead of replaying RS353.
