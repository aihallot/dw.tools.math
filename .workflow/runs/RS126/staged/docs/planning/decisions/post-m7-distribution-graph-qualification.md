# RS126 — fresh seven-package distribution and public-surface consumer gate

## Question and accepted scope

After the owner's choice **A (qualify actual Math delivery without publication)**, and durable RS125 first-pass verification of the two isolated analysis/scientific consumers, can the **seven-package 1.0-preview Math distribution graph** be freshly built, packed, source-free restored and verified with its existing independent eight-probe consumer, while keeping M0–M7 complete and withholding publication?

**This is evidence-only post-M7 qualification.** It uses the already-done M7 node as the truthful DWF project scope, with **zero progress transitions and zero completion achievements**. The accepted backlog remains `0.1.122`; no speculative M8, GitHub CI, public NuGet, cross-repository mutation, outbound adoption request, new provider or irreversible license choice is authorized.

## Verified source and historical evidence

- RS125 `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` records **2 freshly executed source-free local consumers, 16 successful probes** on net10.0, win-x64, selected SDK 10.0.402. Its DWF report is `RS125/attempt-001`, **zero prior failures**, **first-pass YES**. The record is local evidence, not broad ABI or release certification.
- RS124 `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` records current source restore/build/full test suite and 3 locally packed preview archives, but also **one pre-attempt refusal** and first-pass NO. Those archives are historical evidence; RS126 must **repack** its own current seven dependencies, not reuse an assumed retained local feed.
- `docs/planning/evidence/M5-gate.json` contains historical source-free packaging, 7 archives and 3 previous consumers. A past success cannot substitute for RS126 current tests.
- `src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs` declares a **closed graph of exactly seven Math packages** and their allowed direct dependencies: `dw.quantities`, `dw.tools.math.ir`, `dw.tools.math.codecs`, `dw.tools.math.symbolics`, `dw.tools.math.numerics`, `dw.tools.math.composition`, and `dw.tools.math.distribution`. The final distribution package has `readme.md` and `NOTICE.md`.
- `tests/consumers/m5-w02-c02-t2/m5-managed-api-boundary-consumer.csproj` pins exactly `dw.tools.math.distribution [1.0.0-preview.4]`; its `NuGet.Config` clears feeds and it declares **no ProjectReference**. `Program.cs` receives **seven** concrete nupkg paths in the specified order beneath `m5-w02-c02-t2-local-feed`, validates real ZIP/nuspec/net10 metadata, asserts **eight** independent public API/runtime/mathematical/refusal oracles, outputs seven `M5-W02-C02-T2-SURFACE-SHA256` records (public API and assembly SHA-256), and emits `dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes=8`.
- `docs/planning/backlog.json` and the native DWF plan remain closed: **8 milestones, 17 work packages, 58 accepted chunks, 541 DWF nodes done**. M7's research-only deferrals and all three unsent AURA/Decision/MCDM drafts continue to apply.

## Controlled evidence chain

One read-only BCL C# checker, `docs/planning/ValidatePostM7DistributionGraph.cs`, independently verifies the exact source package list, versions, declared direct project references, consumer constraints, prior RS125 evidence and native plan invariants. It has two nonmutating modes: `--check` for the selection baseline and eventual RS126 receipt; `--packages` for the newly packed local archive directory. The package check prints ordered `RS126-PACK|id|version|bytes|sha256` rows and independently validates exact package identities, net10 assemblies, nuspec direct dependency IDs and required distribution documents.

A single controlled RS126 payload:
1. Performs a locked restore of `dw.tools.math.slnx` and a Release build of `dw.tools.math.distribution.csproj` with its six dependent projects.
2. Packs **all seven** source projects with `--no-build --no-restore` into ignored `build/artifacts/m5-w02-c02-t2-local-feed`. The folder basename is the existing consumer's explicit anti-substitution test.
3. Executes the read-only independent `--packages` check, then restores the consumer exclusively from that feed into `build/artifacts/rs126-isolated-packages` using an isolated `NUGET_PACKAGES` cache, not the global package cache or any remote NuGet source.
4. Executes the original eight-probe consumer with seven absolute fresh archive paths. It **must** print the expected success marker, seven ordered `M5-W02-C02-T2-SURFACE-SHA256` rows, and runtime evidence. Unknown, missing, repeated or invalid hashes and files refuse the run.
5. Publishes only a Math-local DWF receipt `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` containing **12 successful command receipts**, seven ZIP SHA/byte identities, seven observed per-archive public API/managed assembly SHA pairs, eight fresh probes and exact negative publication/ABI/other-RID claims. A failed command never creates a successful receipt.

This is a bounded current-archive public **surface fingerprint**, not proof of binary compatibility with an unobserved previous version. No claim is made that the eight probes cover every Math API.

## Decision boundary and follow-up

On success, admit the **observed seven-package net10 dependency graph and one independent current-source NuGet consumer** on the recorded SDK/RID. Keep all historical source-free tests distinct from these eight *fresh* probes; prior RS124 and RS125 counters must not be rewritten.

Do not infer public 1.0.0/1.2.0 package publication, source/license approval, cross-version API/ABI compatibility, other-RID availability, general CAS/formal solvers, generated Math `dw.discovery` skill, or any AURA/Decision/MCDM recipient acknowledgment.

After RS126, the owner may choose a distinct API/ABI baseline comparison (if pinned historical assemblies and compatibility semantics exist), more current source-free package consumers, or authorized recipient coordination. None is automatically scheduled as a new M8.

**Stop-loss:** one locked restore, one bounded Release graph build, seven local pack invocations, independent package audit, one offline source-free restore, one independent eight-probe consumer; zero provider installs, paid CI, sibling changes or publication.
