# RS154 — Immutable private Scientific publisher rehearsal (no shared-feed release)

## Source authority and exact original candidate

RS153 was pushed to Math `main` at commit `16779f7aa5dfcc1cebd1c3be43aaaa2fa6ab566e`, attempt 001, `firstPassSucceeded=true`, using DWF 0.1.63. Its F20 evidence qualified the **unpublished original** `dw.tools.math.scientific [20260.20261105.1001]` NuGet candidate, `37,265` bytes, archive SHA-256 `233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f`, and packaged net10.0 DLL SHA-256 `604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c`. F20 proves 15 source-free numerical/refusal checks, zero package dependencies, and exact SHA-256 pins for all **18 source items**: sixteen C# files, the original csproj and the lockfile.

The original project remains `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`, with `Version 1.1.0-preview.14`, no external `PackageReference` or source `ProjectReference`. The original archive remains only in ignored local output at `build/artifacts/rs153-scientific-candidate/candidate/dw.tools.math.scientific.20260.20261105.1001.nupkg`; it is **not** published by RS153 or RS154.

## Single RS154 disposable-feed publisher contract

The Math-owned non-packable `tools/dw.tools.math.nuget-scientific-release` uses the shared immutable `Dw.Tools.NuGet [20260.20261026.1010]`, verifying actual loaded provider DLL SHA-256 `b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`. It does not fork publisher behavior.

The DWF payload invokes **only `--rehearse`**. That mode validates F20, the physical original source and lockfile hashes, the exact sixteen-source set, historical source version and zero dependencies, plus retained F19 private Foundation readback. A temporary SDK packaging project compiles these sixteen original source files via explicit linked paths without changing them. It packs a new rehearsal archive under the same prospective calendar version in a **fresh ignored disposable workspace** and uses the shared NuGet API to publish/read back **only inside a disposable feed**. It verifies archive and DLL SHA-256, exact ID/version and dependency closure; refuses duplicate-version collision, falsified digest and cancellation; and preserves the disposable published bytes.

A new isolated consumer restores solely from that disposable feed with a single exact `PackageReference` and private cache, loads the actual packaged DLL and reruns the **fifteen F20 binary numerical/refusal probes**. Seven internal process receipts and four outer process receipts are required. The rehearsal archive and DLL hashes are recorded separately because they may differ from original F20 bytes.

## Guarded owner-only release modes — outside RS154

Only after RS154 is durably successful and the owner explicitly authorizes a private release may the controller's guarded `--publish` and `--verify` modes be invoked. They require original F20 archive SHA/length and DLL SHA, exact source-owner packaging/redistribution attestation, successful RS154 DWF report, clean `main` checkout and existing absolute `NUGET_LOCAL`.

The shared no-overwrite publisher can write **only** the exact original 37,265-byte F20 candidate. It never substitutes temporary rehearsal archive bytes, rebuilds the original candidate or overwrites an existing destination. On ambiguous partial publication failure, inspect actual feed bytes before retry. Publication receipts stay in ignored local output, and independent physical-feed DWF qualification would require a later bounded RS155 after owner `PUBLISHED` and `VERIFY ... PASS` outputs.

## Scope and exclusions

RS154 persists only this documentation, a read-only C# validator, the pinned NuGet release controller and its project/props, plus the durable F21 rehearsal receipt. It retains all 541 previously completed Math native nodes, the original Scientific version, F1–F20 evidence and six independently published Math packages without modification.

No DWF shared `NUGET_LOCAL` write, NuGet.org release, new public license decision, installed rollback or concurrency guarantee, other OS/RID claim, feed-wide audit, Discovery migration, Decision integration or AURA admission is authorized.
