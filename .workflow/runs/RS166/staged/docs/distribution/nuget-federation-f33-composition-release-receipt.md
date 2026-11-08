# RS166 — Correct Composition private release receipt for both direct dependencies

## Durable baseline and bounded defect

RS165 succeeded on GitHub `main` at `732bf99a8a0a7e760943654acc1f34766be66732`, attempt 001 with DWF `0.1.66`. Its durable report records two pre-attempt refusals, zero allocated-attempt failures, and `firstPassSucceeded=false`. F32 certifies only a disposable-feed publisher rehearsal for the **unpublished original** `dw.tools.math.composition [20260.20261107.1001]`: original candidate 17,101 bytes with archive SHA-256 `b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f` and DLL SHA-256 `6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252`. F31 and F32 both establish exactly **two direct dependencies**: `dw.quantities [20260.20261102.1001]`, then `dw.tools.math.ir [20260.20261104.1001]`. IR's transitive reference to the same Quantities identity is not a third distinct dependency.

The immutable RS165 release controller was correctly rehearsed and guarded. An independent post-rehearsal audit found that its owner-only `--publish` receipt and `--publish`/`--verify` terminal markers inherited a single `dependencyId/dependencyVersion` shorthand naming only IR. Publishing the package with such evidence would omit its direct Quantities dependency from the operator's audit receipt, although the original package nuspec already contains both dependencies.

## Exact RS166 correction and provenance

RS166 changes **only three exact spans** in `tools/dw.tools.math.nuget-composition-release/Program.cs`: the `--publish` receipt emits `directDependencyCount = 2` and ordered `directDependencies` with `id`, `version`, `archiveSha256`, and `packagedDllSha256` for both original, previously qualified archives; both terminal markers show explicit Quantities and IR identities with calendar versions, rather than the misleading lone IR `dependency` field.

An observational C# validator compares the **complete historic RS165 controller** staged at `.workflow/runs/RS165/staged/tools/dw.tools.math.nuget-composition-release/Program.cs` to the current corrected controller by exactly those three substitutions. It pins the historical source SHA from F32's immutable `source_files` receipt and verifies no other controller content changed. It also checks the F31 candidate, F32 rehearsal, F13 IR and F4 Quantities evidence, fixed original source and DLL identities, DWF predecessor, the existing shared `Dw.Tools.NuGet [20260.20261026.1010]` reference, and 541 complete native nodes.

The payload installs this document, the read-only validator, and the corrected C# controller, executes only `dotnet restore`, `dotnet build` and SDK version against the non-packable release tool using `NUGET_LOCAL` **as a read-only package source**, and captures real process receipts. It executes no release/rehearsal mode. The original four Composition source files, its source `Version 0.3.0-preview.1`, original package archive and both dependency packages remain unchanged.

## Explicit boundaries and next gate

A successful RS166 receipt establishes only **correct source and compiled private-release receipt semantics**, not publication. The owner must explicitly authorize private publication of the **original** 17,101-byte RS164 candidate; any such publication is performed later on a clean `main` checkout via separately guarded `--publish` and `--verify` modes, followed by independent DWF physical-feed readback. A disposable rehearsal archive is never a substitute for the original candidate. No `NUGET_LOCAL` mutation, public registry, license selection, installed rollback, cross-OS qualification, sibling repository edit or AURA/Decision/Discovery migration is authorized by RS166.
