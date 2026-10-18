# Local finite geometry — RS093 T1 qualification recipe

This is a reproducible local qualification recipe, not a public release.

- Restore `dw.tools.math.slnx` with `dotnet restore dw.tools.math.slnx --locked-mode`.
- Build Release and test `M6W02C03S01T1` (1 reflection contract RED recovered, 24 GREEN).
- Run the retained `M6W02C02T2`, `M6W02C02T1`, `M6W02C01T2`, `M6W02C01T1` filters, full scientific tests and four affected Math regression projects.
- Locally pack `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` as `1.1.0-preview.5` under `build/artifacts/m6-w02-c03-s01-t1-local-feed`. Inspect the actual .nupkg contents, dependency and SHA-256.
- Restore from only that cleared local feed into independent package caches and run two package-only consumers: the geometry consumer and the retained C02 T2 boundary consumer, pinned to the same exact local preview.5 version, with no ProjectReference. Verify both receipt byte sizes and SHA-256 against the package inspector.

All scientific observations require controlled RS093 execution. Before the report,
the above are intended checks, not claims of an already achieved run.
The product's preview.4 baseline remains distinct and the new preview.5 is local only.
