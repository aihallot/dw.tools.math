# Current .NET/C# practices reference

This reference is deliberately version-adaptive rather than a frozen list of "latest" APIs.

## Toolchain resolution order

1. Repository compatibility/deployment constraints.
2. `global.json` SDK selection and roll-forward policy.
3. Installed SDK inventory.
4. Target frameworks and language version.
5. Analyzer configuration and packages.
6. Official current .NET documentation when available.

A repository declaration that cannot resolve on the execution environment is a real compatibility problem, not something to hide by disabling restore/build validation.

## Strict analysis

When `TreatWarningsAsErrors` or equivalent CI policy is enabled, author against that policy from the first pass. Do not rely on a later cleanup phase to fix known diagnostics.

Prefer semantic fixes over suppression. Add a suppression only when the diagnostic is understood, documented and intentionally inapplicable.

## API currency

Prefer APIs supported by the resolved target framework. Verify new APIs before use when SDK/TFM boundaries matter. Avoid copying examples from previews or newer frameworks into an older target without compatibility evidence.

## Testing

Use the test framework and version already selected by repository authority. Verify current assertion/analyzer behavior before introducing patterns copied from older framework versions.

## Native stack coherence

Executable repository helpers should normally use the repository's primary runtime/toolchain. A .NET repository should not introduce Python/Node/etc. merely for planning or validation unless that dependency has a concrete benefit and lifecycle owner.

## Structural conventions

Lowercase project filesystem naming and separated build outputs are operational rules, not aesthetic suggestions. The bundled checker provides a reproducible read-only test for those two high-value conventions.
