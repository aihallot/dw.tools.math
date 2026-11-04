# RS147 — Independent Analysis NuGet calendar candidate, no private publication

## Authority and selected source

RS146 succeeded first-pass on GitHub Math `main` at `aea1b730b0897b070239bc0e86255799c8b961f3`, DWF 0.1.63, attempt 001, with independent physical-feed verification of the exact original IR and Quantities binary packages. F13 records 15/15 binary cases, Windows x64, SDK 10.0.403, and no republication.

The F1 observed source inventory lists thirteen Math project packages. Four have separately qualified immutable local publication: `dw.quantities`, `dw.quantities.expression`, `dw.quantities.standard`, and `dw.tools.math.ir`. Among the remaining source projects, `dw.tools.math.analysis` is a bounded, independent numerical package without `ProjectReference` or external `PackageReference`. RS147 selects **only this package** for a new candidate. The original project `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` has historical source `Version 1.1.0-preview.6` and seven current C# source files. Its version, sources, lock file and tests remain unchanged.

## Qualified candidate boundaries

- Candidate package ID: `dw.tools.math.analysis`.
- Calendar candidate version: `20260.20261104.1001` (creation-date segment 2026-11-04; later physical publication may preserve this exact version only for identical original bytes).
- Candidate path (ignored operator-local artifact): `build/artifacts/rs147-analysis-candidate/candidate/dw.tools.math.analysis.20260.20261104.1001.nupkg`.
- Framework: `net10.0`.
- NuGet dependencies: **zero**; no source or consumer `ProjectReference`.
- No new public license, public registry, installed rollback qualification or mutation of `NUGET_LOCAL`.

The Math-owned verifier uses the original source project, a command-line `PackageVersion` override **only for the generated archive**, and separate real `dotnet restore`, `build`, and `pack` commands. The source `Version` remains `1.1.0-preview.6`. An offline, disposable package-source directory and locked restoration prevent transitive provider admission. The real created nupkg is inspected for a single exact ID/version, no `.nuspec` dependencies, a nonempty net10.0 DLL, bounded archive structure and SHA-256. The verifier records original source hashes, candidate bytes and SHA-256, packaged DLL SHA-256, command exits, SDK/runtime/OS/architecture, and source/version/publishing exclusions.

A **fresh source-free consumer** is compiled against only `dw.tools.math.analysis [20260.20261104.1001]` from the actual candidate folder with all other package sources cleared and a separate private cache. Its 13 independent numerical cases cover linear and natural-spline interpolation, least squares, duplicate abscissa refusal, extrapolation refusal and explicit opt-in, 33-sample budget refusal, physical unit mismatch, conditioning refusal, bracketed roots, Simpson quadrature, central differentiation and RK4 integration. The consumer checks the loaded assembly SHA-256 against the packaged DLL and refuses external AURA/Decision/MCDM/CAS provider dependencies. These are finite binary64 contracts, not general precision guarantees.

## Execution, evidence and publication gate

RS147 installs only a bounded C# candidate tool and fixture, documentation, read-only validator, and owner evidence `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`. A successful run proves an **unpublished candidate only** and must not reserve an immutable version in the shared feed. The original generated archive must remain at its exact local output path for any later owner-authorized publication; if absent it must not be silently regenerated under the same version.

A later run may rehearse the shared immutable publisher on a fresh **disposable** feed. Only after owner approval may separate guarded `--publish` and `--verify` commands write `NUGET_LOCAL`, followed by another independent readback DWF run. RS147 performs none of those steps. All 541 completed native nodes, F1–F13 receipts, old source versions, and already-published packages remain preserved. Other OS/RID builds, public license, rollback, concurrent publication, Discovery, Decision and AURA consumer adoption are not implied.
