# RS148 — Immutable private Analysis publisher rehearsal (no shared-feed publication)

## Exact candidate and operator provenance

The previous run RS147 completed on Math `main` at commit `8d4fe85ce6a545b398dac6e0230ca5f83cd348d6`, with DWF 0.1.63 and attempt 001. It succeeded **after one preselection refusal**; first-pass is false, with one preparation failure, zero failed allocated attempts, and no boundary violations.

Its independent F14 candidate `dw.tools.math.analysis [20260.20261104.1001]` is **not published**. The original candidate is 27,831 bytes at `build/artifacts/rs147-analysis-candidate/candidate/dw.tools.math.analysis.20260.20261104.1001.nupkg`, with original archive SHA-256 `d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf` and packaged DLL SHA-256 `b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7`. The original source version remains `1.1.0-preview.6`, with precisely seven unchanged C# files, no project or NuGet dependencies, and thirteen compiled binary oracles.

## Single RS148 rehearsal contract

The Math-owned non-packable release controller `tools/dw.tools.math.nuget-analysis-release` reuses the already-qualified `Dw.Tools.NuGet [20260.20261026.1010]` package and validates its actual loaded DLL SHA-256 `b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`. It does not fork or replace the shared immutable publisher.

The `--rehearse` mode is the **only mode authorized inside DWF RS148**. It checks F14, seven original source SHA-256 identities, source version and zero-dependency closure. In a fresh ignored workspace it compiles the real unchanged Analysis source files through a disposable temporary packaging project, creates a new NuGet archive with the same version, and publishes this **rehearsal archive only** to a newly created disposable feed. It verifies the real shared NuGet reader, readback archive/DLL SHA-256, collision refusal, changed digest refusal and cancellation refusal. A fresh isolated PackageReference-only consumer restores solely from the disposable feed and executes the original thirteen binary oracles against the loaded compiled Analysis DLL. No Math source ProjectReference is introduced.

The rehearsal archive and DLL **may not be byte-identical** to the original F14 candidate. Their SHA-256 values must be recorded separately. Success proves the publisher behavior, **not** reproducibility of the original pack or a private release. Neither a missing original ignored candidate nor a successful rehearsal authorizes replacement bytes under the same package ID/version.

## Separate owner publication modes — never within RS148

After a successful RS148 result and explicit human owner authorization, the controller exposes guarded `--publish` and `--verify` commands. They require clean `main`, source-owner rights attestation, exact successful RS148 report, existing absolute `NUGET_LOCAL`, and the **original** 27,831-byte F14 candidate with both original SHA-256 values. The shared `DotNetLocalPackageRepository` handles the no-overwrite publication. Any collision or stale/altered original must refuse, not silently re-pack. The owner-side receipt stays in ignored build artifacts. If publication fails after a possible feed side effect, inspect actual destination bytes before retrying.

The operator may run both guarded modes only as distinct actions **after** RS148 reaches durable `pushed`. A subsequent RS149 DWF run may then independently read and qualify the physical published Analysis archive, but only after the owner provides actual successful publication and verification outputs.

## Scope and exclusions

RS148 persists only this contract, its C# controller/project, an observational validator, and a run-owned durable F15 rehearsal receipt. The 541 completed roadmap nodes, historical source versions/references, all prior F1–F14 evidence, and existing four published Math packages remain unchanged. No shared-feed write, remote registry, public licensing decision, installed rollback qualification, other OS/RID claim, Discovery migration, Decision mutation or AURA admission is authorized.
