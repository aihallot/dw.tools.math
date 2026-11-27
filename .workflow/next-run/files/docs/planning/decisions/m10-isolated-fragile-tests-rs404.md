# RS404 - Qualify isolated fragile numerical contracts under .NET 11

RS403 succeeded first-pass at attempt 001 under DWF 0.1.81, result 4e7d6b8f375c5759592ced883485166a441ab446. Its integrated validation observed SDK 11.0.100, net11.0, C# 15, locked restore, 24 compiled assemblies, 24 lock identities and zero build warnings/errors. Preserve that report and its exact source preparation identity.

Reuse the qualified isolated solution probe with a new --focused mode. The fresh workspace requires the same locked restore and Release build before running eight existing test classes: three Quantities classes (exact Int64 scaling and independent verification, historical exact rational budget) and five Statistics classes (streaming moments, larger-sample quantiles, their independent verification, and bounded sampling/replay). Require each named class to execute, a minimum of 9 and 26 passed tests respectively, no failed or skipped results, and record TRX SHA-256 and actual counts. Rebuild is limited to the fresh isolated graph, not the production solution; no full-suite test campaign. Preserve the --check mode and existing semantics.

GREEN stays open. Maintained-tool qualification, rollback, full post-cutover certification, publication and Linux/WSL evidence are outside this slice. No production retarget or package dependency changes.
