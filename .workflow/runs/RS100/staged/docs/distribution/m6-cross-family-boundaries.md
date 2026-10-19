# RS100 controlled C03 cross-family boundary qualification

This file describes the planned DWF execution, not tests already run.

1. From the accepted RS099 preview.11 source, stage ONLY
   `M6W02C03T2RedTests.cs` and compile the locked .NET 10 solution.
   Observe one failing runtime reflection RED for the absent public
   `M6W02C03BoundaryAggregate64`. Compiler errors do not count.
2. Stage source `M6W02C03BoundaryAggregate64.cs`, product
   `1.1.0-preview.12`, and 78 independent GREEN tests. Locked
   restore/build Release and require **79 targeted tests** including
   the recovered RED. Retain 758 S01/S02/S03 and C01/C02 scientific
   tests plus the original C03-T1 suite; run full scientific tests,
   transfer/plan checks and four relevant Math regression projects.
3. Pack the real scientific NuGet archive into
   `build/artifacts/m6-w02-c03-t2-local-feed`. Inspect compiled
   net10 DLL and nuspec (no unadmitted dependencies), then actual
   archive byte length and SHA-256.
4. Source-free restore SIX exact preview.12 clients from ONLY the
   cleared local NuGet feed into independent caches. Every client
   must have one `PackageReference` and zero `ProjectReference`.
   Require eight probes per client and matching SHA/byte receipts:
   C03-T2 boundary aggregate; C03-T1 direct aggregate; S03-T2
   timing; S02-T2 direct signals; S01-T2 Euclidean geometry; C02-T2
   managed special functions.
5. Write qualification/RED evidence only from the DWF mutating
   payload after actual checks. Transition C03-T2-R/G/V, C03-T2
   and original parent C03 to done; regenerate planning projections,
   then validate native DWF adoption and plan. Keep M6-W02 and M6
   in-progress and remaining C04/C05 planned.

This audit does not convert physical units, guarantee alias-free
sampling, perform FFT/geodesics/forecasting, or certify binary64 error
bounds. Caller input policies acknowledge responsibility, not proof.
