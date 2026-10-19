# RS096 local direct-signal boundary qualification

This is the DWF execution plan, not an assertion of completed tests.

1. On the qualified RS095 preview.7 product, stage only the independent
   M6W02C03S02T2RedTests.cs. Compile the existing solution and observe
   exactly one runtime RED for absent public FiniteSignalBoundaryAudit64,
   with no compiler-error substitution.
2. Stage the BCL-only boundary facade, scientific preview.8 metadata and
   68 independent GREEN tests. Lock-restore and build the .NET 10 solution,
   then recover RED with 69 targeted S02-T2 tests. Preserve 60 S02-T1,
   53 S01-T2, 25 S01-T1, 88 C02-T2, 80 C02-T1, 88 C01-T2,
   and 70 C01-T1 tests, plus four Math regression projects.
3. Pack scientific preview.8 to
   build/artifacts/m6-w02-c03-s02-t2-local-feed. Inspect the real
   archive's net10 DLL, nuspec identity, dependency list, bytes and SHA.
4. Restore S02-T2 audit, S02-T1 direct signal, S01-T2 geometry and
   C02-T2 special function clients independently into separate NuGet
   caches, ONLY from the cleared local feed. All must have one pinned
   PackageReference and zero ProjectReferences. Run eight probes per
   client and compare actual archive byte/SHA receipts.
5. Generate plan projections and verify native DWF adoption,
   planning/transfer validators and git diff hygiene. Close S02-T2 and
   S02 only; S03, original C03, M6-W02 and M6 must remain unfinished.

Policy acceptance of discrete samples does not prove an alias-free
physical signal. Ratio floors are conservative risk diagnostics, not
numerically certified error bounds or an FFT implementation.
