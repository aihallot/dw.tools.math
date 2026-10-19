# RS097 uniform-series local NuGet verification

This is the intended .NET 10 / C# DWF qualification procedure.
Staged source is not proof that compilation or oracles have passed.

1. Before introducing the FiniteUniformSeries64 facade, stage only
   M6W02C03S03T1RedTests.cs into the locked-restore scientific solution.
   Compile successfully and observe one runtime missing-public-type
   RED with the exact expected assertion; compiler failure is not RED.
2. Stage the FiniteUniformSeries64 source, scientific preview.9 project
   metadata, and 60 independent GREEN tests. Locked restore, Release
   build, target 61 passing recovered-RED/GREEN tests and preserve
   the prior 533 scientific tests in their separately filtered suites.
   Run the full scientific suite plus four Math regression projects.
3. Pack the actual package under
   build/artifacts/m6-w02-c03-s03-t1-local-feed and inspect the compiled
   net10 assembly, nuspec version/dependencies, real length and SHA-256.
4. Independently source-free restore into separate package caches from
   ONLY the cleared local feed: new uniform-series client, retained
   S02-T2 signal audit, S01-T2 geometry audit and C02-T2 special
   function audit. Each has one exact preview.9 PackageReference,
   no ProjectReference, eight probes and identical inspected bytes/SHA.
5. Use DWF's native plan transition and project planning validators,
   including source transfer checks and git diff hygiene. Close only
   S03-T1 RED/GREEN/V and T1 while leaving S03-T2, S03, original C03,
   work package M6-W02 and release M6 unfinished.

Uniform sample spacing and units are obligations of the caller; neither
first difference nor moving average performs derivative estimation,
forecasting, irregular-time resampling or arbitrary-precision analysis.
