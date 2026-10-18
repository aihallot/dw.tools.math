# RS095 local finite direct-signal package qualification

This is a .NET 10/C# local DWF recipe, not an executed validation
or a public package release.

1. With the RS094-approved T2 geometry baseline and preview.6 source,
   stage only the S02-T1 reflection RED test. Restore locked solution,
   compile and execute the targeted RED; record exactly one runtime failure
   for the absent public FiniteSignal64 facade, not a compilation failure.
2. Stage the bounded direct DFT/linear convolution source, the new 59 GREEN
   tests and preview.7 project metadata. Restore/build Release; require
   targeted 60 recovered RED + GREEN tests. Preserve 53 S01-T2, 25 S01-T1,
   88 C02-T2, 80 C02-T1, 88 C01-T2 and 70 C01-T1 tests and run relevant
   scientific, transfer and four Math regression projects.
3. Pack the real scientific preview.7 archive under
   build/artifacts/m6-w02-c03-s02-t1-local-feed. SHA-256 inspect its actual
   bytes, compiled net10 DLL, metadata version and dependency absence.
4. Restore independently into three package caches from only the local feed
   with NuGet.Config clearing external sources. All clients must pin exact
   preview.7 and have zero ProjectReferences: S02 direct DFT/convolution,
   retained S01-T2 Euclidean audit and retained C02-T2 special boundaries.
   Execute eight probes per client and require exact inspected archive
   bytes and SHA-256 in all three receipts.
5. Inside controlled DWF payload, project backlog/native task R/G/V
   completion, regenerate planning projections, and leave S02-T2 and S02
   phase unfinished alongside original C03/S03 and M6-W02/M6.

Forward DFT exponent is negative; inverse exponent is positive and divides
by N. Linear convolution zero-extends, not circularly wraps.
The library does NOT provide FFT, physical signal units, guaranteed
error bounds, or universal aliasing protection.
