# Isolated C04 mathematical consumer — package-only usage

The RS050 executable in tests/consumers/m3-w02-c04/ is a standalone .NET 10 validation surface, intentionally **not** a project reference or a sibling repository. It validates the C04-T1 consumer acceptance recipe with 31 independent named probes using only dw.quantities 0.2.0-preview.1 and dw.tools.math.statistics 0.4.0-preview.6 from a local NuGet feed.

The DWF payload performs these build/consumer steps:
1. Restore the Math solution with the locked repository package references and build Release.
2. Pack dw.quantities and dw.tools.math.statistics to build/artifacts/m3-w02-c04-local-feed.
3. Inspect both local NuGet archive manifests for pinned package IDs/versions, dependencies and compiled net10.0 libraries.
4. Restore the consumer project with a cleared-source NuGet.Config, explicit local feed and isolated package cache; build and execute its package-only binary.
5. Require the exact marker: dw.tools.math/c04-isolated-consumer/0.4 qualified; probes=31. Record package SHA-256s, executor runtime and source identities in the Math planning evidence.

A consumer-local adapter checks ExactRational * positive integer scale and refuses fractional or out-of-Int64 results. There is no silently lossy implicit double conversion or newly exported scaling API. Sample aggregates and quantiles carry mathematically explicit Type-7 versus nearest-rank definitions; a local DTO retains the convention when a sample is shared. Undefined Pearson is typed and nullable rather than mapped to zero. SplitMix64/v1 checkpointing is deterministic but noncryptographic.

These observations justify only a **proposal** to Decision and MCDM owners to evaluate relevant mathematical primitives. Existing request drafts remain unsent and no external adoption or ranking/decision behavior is claimed. C04-T2 handles recipient acceptance boundaries and request qualification; successful RS050 alone keeps M3-W02-C04 active.
