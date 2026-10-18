# Local RS094 Euclidean boundary package verification

This is a planned .NET 10/C# local qualification recipe, not an executed scientific result or public package release.

1. Compile the independent M6W02C03S01T2RedTests against the qualified preview.5 source before staging the boundary sidecar; require one runtime RED with the exact missing-public-API marker. Compiler failure does not count.
2. Stage the new FiniteGeometryBoundaryAudit64 source, project preview.6 metadata and 52 M6W02C03S01T2 GREEN tests. Restore the solution with locked packages, build Release, then run the T2 filter, retained T1, C01/C02 scientific filters and four relevant Math regressions.
3. Locally pack the actual scientific project to build/artifacts/m6-w02-c03-s01-t2-local-feed; inspect the archive's real net10 assembly, nuspec identity, dependency list, length and SHA-256.
4. Restore three independently source-free clients only from the cleared local feed, each to its own cache. Require exact preview.6 PackageReference, no ProjectReference, eight probes each and identical archive SHA/byte-length receipts: T2 geometry audit; retained T1 geometry; retained C02 special-function boundaries.
5. Run native-plan adoption, planning projections and transfer architecture checks inside DWF. Emit evidence only after successful validations; close only S01 T2 and S01, not original C03, work package or milestone.

The relative displacement and projection signal floors are conservative heuristics, not proved numerical error bounds. Binary64, coordinate-system and OS guarantees are restricted to actually verified evidence.
