# RS101 local pure financial mathematics qualification

This is a controlled .NET 10/C# execution recipe, not a completed
test record or a public NuGet distribution.

1. On the accepted RS100 preview.12 scientific baseline, stage ONLY
   `M6W02C04T1RedTests.cs`. Locked restore/build Release, then
   observe one runtime reflection RED asserting that
   `M6W02C04Contract` is absent; a compiler failure is not RED.
2. Stage `M6W02C04Contract.cs`, local preview.13 metadata and
   66 independent GREEN tests. Locked restore/build and require
   all 67 targeted RED-recovered plus GREEN tests, and independently
   retain 837 previous scientific tests plus the full scientific
   suite and four representative Math regression projects.
3. Pack the real local scientific preview.13 into
   `build/artifacts/m6-w02-c04-t1-local-feed`. Inspect its compiled
   net10 DLL, nuspec identity/dependencies, byte length and SHA-256.
4. Independently restore six source-free clients from ONLY the
   cleared local feed into separate caches: new C04 finance,
   retained C03-T2 boundary, C03-T1 direct aggregate, S03-T2
   uniform boundary, S02-T2 direct signal boundary and C02-T2
   special-function boundary. Every consumer pins exact preview.13
   with one PackageReference, zero ProjectReferences, eight distinct
   probes, and matching actual SHA-256/byte-count receipts.
5. Validate transfer architecture, projected backlog/native DWF
   consistency and git diff hygiene. Close only C04-T1 RED/GREEN/V
   and T1. Keep C04-T2, C04 parent, C05, M6-W02 and M6 open.

The calculation assumes an effective discrete rate per caller-owned
period, not calendar years or implicit continuous compounding.
The root solver admits only one sign-change pattern with a
caller-supplied bracket; it neither infers missing market data
nor certifies global root uniqueness for arbitrary cashflows.
Neither financial recommendations nor certified floating-point
error bounds are provided.
