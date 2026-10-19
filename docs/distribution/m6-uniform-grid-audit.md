# RS098 local uniform-grid numerical and timing qualification

This is a proposed controlled .NET 10/C# DWF recipe, not proof that
staged code has compiled, executed or been published.

1. On the RS097 preview.9 scientific baseline, stage only the
   independent M6W02C03S03T2RedTests reflection RED. Restore the
   locked solution, build Release and observe one runtime failure
   asserting the public audit type is absent. Compiler errors do not
   count as independent RED.
2. Stage the new uniform-series timing/numerical audit source,
   preview.10 project metadata and 106 independent GREEN tests.
   Locked restore/build and require all 107 targeted recovered
   RED/GREEN tests. Retain 61 S03-T1, 69 S02-T2, 60 S02-T1,
   53 S01-T2, 25 S01-T1, 88 C02-T2, 80 C02-T1, 88 C01-T2
   and 70 C01-T1 tests; run all scientific and four Math regressions.
3. Pack the real archive under
   build/artifacts/m6-w02-c03-s03-t2-local-feed. Inspect actual
   bytes, net10 DLL, nuspec, dependency absence and SHA-256.
4. Restore five independently cached source-free clients only from
   that feed using a cleared NuGet.Config: new S03-T2 audit,
   retained S03-T1 uniform series, S02-T2 direct signal audit,
   S01-T2 Euclidean geometry and C02-T2 special functions.
   Every client must have an exact preview.10 PackageReference,
   zero ProjectReferences, eight actual probes and an identical
   inspected package SHA/byte-count receipt.
5. Write evidence, update native/project plans and regenerate
   projections only after these checks succeed. Close S03-T2 and
   S03; keep the original C03 T1/T2 aggregate, M6-W02 and M6 open.

All timestamps, interval units and actual uniform sampling remain
caller-managed assumptions; the audit only validates the supplied
numeric grid. Risk ratios are conservative refusal policies,
not certified binary64 error bounds or forecasting algorithms.
