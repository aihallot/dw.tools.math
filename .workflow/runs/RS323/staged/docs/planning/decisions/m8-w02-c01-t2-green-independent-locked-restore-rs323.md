# RS323 — Qualify live three-archive Standard net11 locked package graph GREEN

## Evidence and existing tooling

RS322 succeeded first-pass at allocated attempt 001 under the 0.1.72 workflow executable with generated guidance 0.26. Its read-only physical check proved that the unchanged private NUGET_LOCAL contains exactly the three specified Standard/Expression/Quantities package identities, nupkg SHA-256 hashes, net10 DLL SHA-256 hashes and source references. The source-graph RED was independently closed by RS321. Existing ProbeM8Net11PublishedStandardLockedRestore.cs and tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs establish the historical bounded restore and reusable ten-check consumer. Avoid replaying the historical RS254 script, making a second package release tool, or expanding to the unqualified remaining ten libraries.

## Current independent GREEN proof

A single new C# test script uses the existing RS322 profile to copy only the three physical archives into an isolated read-only source feed under build/artifacts/l323-* and creates a package-only net11.0/C#15 consumer pinned to the existing Standard archive with RestorePackagesWithLockFile=true and central version management disabled. It copies the original ten-check fixture and validates dotnet SDK 11.0.100, exact initial lock entries/versions and direct-versus-transitive topology. A second --locked-mode restore must preserve the lock file hash; the unchanged fixture must build and execute, checking all three actually loaded DLL SHA-256 values and ten mathematical examples.

The script then changes only the copied consumer project in two independent counterexamples: it supplies an unavailable Standard version pin, and it silently promotes base Quantities to a new direct dependency. Each must be refused in --locked-mode, restore exact consumer project bytes, and then pass a clean locked restore with unchanged lock bytes. No original feed writes, repack, package publication or SDK pin edits are permitted.

## Scope and authorization boundary

RS323 adds bounded GREEN technical evidence to the already in-progress M8-W02-C01-T2-G leaf. It does not complete GREEN, ready Verification or transition any other native node. The Math packages retain their original hashes and source target net10.0; the isolated consumer targets net11.0 with stable SDK 11.0.100. The other ten packable libraries, complete numerical behavior and platform/RID parity remain unqualified. DWF guidance 0.27 consumer migration stays on the separate BL0028/RS0 hold, and no broker, public distribution or cross-repository change is authorized.
