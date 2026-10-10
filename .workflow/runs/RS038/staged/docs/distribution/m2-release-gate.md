# M2 / 0.3.0-preview — local release gate

M2 establishes the bounded typed mathematical IR and the provider-neutral composition vertical. The release gate requires the six accepted phases in M2-W01 and M2-W02 to be individually `done` with their own qualification evidence. A prior phase's green record is not proof of later integration.

## Accepted gate recipe

1. Verify canonical backlog and native DWF state for all six phases, twelve tasks, and their subtasks; require M1 as the completed prerequisite.
2. Read and validate the six final phase qualification files (study/ADR, IR types and boundaries, IR canonical JSON and structural identity, inspectable results, exact typed direct/fluent pipeline, exact replay and bounded cache). The ADR remains an ADR, not code.
3. Restore the full .NET 10 solution in locked mode; build Release with no warnings; execute independent cross-layer M2 gate tests plus the complete composition, IR and quantity suites and foundation verification.
4. Build `dw.quantities`, `dw.tools.math.ir` and `dw.tools.math.composition` packages locally. Inspect each package assembly, NuGet identity, preview version and declared dependency graph; capture SHA-256 and byte lengths of the **observed** package files.
5. Use the separate `tests/consumers/m2-gate` project installed exclusively from staged files by the typed Payload SDK. Restore **only** its `dw.tools.math.composition` PackageReference from the newly built local feed, including transitive dependencies; compile and execute exact rational + canonical IR + typed composition + bounded replay acceptance. The consumer is excluded from the main solution, has no ProjectReference, and requires no AURA dependency. Its isolation is by the NuGet dependency graph, not by claiming a temporary directory outside this repository.
6. Re-run transfer architecture, native DWF mapping, canonical backlog generation and structural checks. Only after all evidence passes may the milestone M2 become `done`.

## Meaning of this release

Qualified local capabilities are exact rational quantities, an immutable typed IR and bounded canonical JSON transport, versioned structural/presentation identities, inspectable capability/result envelopes, exact unit-aware direct and fluent pipelines, finite per-step provenance, cooperative cancellation, semantic replay and an optional volatile cache.

The gate **does not** establish algebraic equivalence, a symbolic solver, arbitrary numerical algorithms, actual provider execution, binary64 cross-platform reproducibility, disk or distributed caching, production SLAs, or integration/adoption by AURA. An unknown provider id is neither provider availability nor permission. `MATH-XR-002` remains draft/untransmitted. M3 and subsequent numerical/symbolic releases remain independent and not started.

All packages are local ignored artifacts only; hashes refer to the actual gate build, not a promised reproducible nupkg byte stream. This decision does not publish packages or change any sibling repository.
