# RS137 — Independent read-only qualification of the published dw.quantities package

## Authority and reported publication

RS135 succeeded first pass on Math main `cc88f8c69a37037ab5c1d2437accf07ff104df26`
with a compiled, isolated binary consumer and the exact candidate
`dw.quantities 20260.20261102.1001`. RS136 succeeded first pass on main
`595d145e663ecec6c67b4098ba4a98e4b6fd7ec3`; it qualified the existing
`Dw.Tools.NuGet` no-overwrite publisher using a disposable feed, and deliberately
did not publish to the shared feed.

On 2026-11-02, the operator separately reported successful real commands
on MSI-02, Math checkout `main`:

- `dotnet run --project tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj -c Release --no-build --no-restore -- --publish`
- `dotnet run --project tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj -c Release --no-build --no-restore -- --verify`

The terminal's reported publication SHA-256 was
`e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`,
followed by `MATH-NUGET-VERIFY ... binary=4 ... PASS`. These human-reported
outputs are useful **operator evidence**, not an independently authenticated
live-feed DWF receipt. Do not manufacture a source-controlled publication
receipt merely from terminal text.

## Exact immutable identity and single-package scope

Math retains these original qualified identifiers without renaming or
repacking any historical version:

- PackageId: `dw.quantities`.
- Published local version: `20260.20261102.1001` (creation date 2026-11-02).
- Candidate archive: exactly 14,747 bytes; SHA-256
  `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`.
- Packaged net10.0 DLL SHA-256
  `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.
- Original source `Version`: `0.2.0-preview.1`, unchanged.
- Package dependencies: zero. Source-free exact rational consumer: four checks.
- Publication location selector: `NUGET_LOCAL` only, not a hard-coded drive.

Do not treat the RS136 disposable rehearsal archive SHA as the published
candidate's SHA; those separately packed archives are not byte-identical.

## Independent read-only DWF verification

RS137 uses a new Math-owned, non-packable C# consumer at
`tools/dw.tools.math.nuget-feed-verify`. It **does not** invoke the
RS136 controller's `--publish` or `--verify` modes, since those require
an operator-clean `main` checkout and must remain immutable. The original
RS134, RS135 and RS136 source files and receipts remain untouched.

The verifier reads the actual single archive named by the above exact
PackageId/version from the existing absolute `NUGET_LOCAL` feed. It checks
the actual archived bytes and net10.0 DLL SHA-256 against RS135, copies a
bounded snapshot to an ignored disposable directory, then invokes the
**existing published** `Dw.Tools.NuGet [20260.20261026.1010]` reader to
verify the package identity and ZIP/nuspec. It never scans the full
multi-producer feed.

A separate C# project contains only the original RS135
`QuantitiesBinaryConsumer.cs` fixture and a single exact
`PackageReference` to `dw.quantities [20260.20261102.1001]`.
The project restores directly from `NUGET_LOCAL` through its own
`NuGet.Config` with cleared global sources, and uses a new isolated
NuGet cache. No Math source or sibling project reference is permitted.
The consumer must compile and execute four checks: loaded assembly,
loaded DLL SHA, exact 1/3 + 1/6 = 1/2, and zero-denominator refusal.
The physical feed package is hashed again afterward to detect changes.

The DWF payload records the independently observed archive, loaded binary,
source-owned report lineage and actual restore/build/run exit codes in a
new JSON receipt. The source and native Math product scope do not change.
There are no package writes, package version reservations, remote registry
actions, other repository mutations, generated skills, Math Discovery
migration or product-plan progress transitions.

## Limits and following work

This qualification is a **private local NuGet distribution** on the
observed Windows x64/.NET 10 host. It neither selects a public NuGet
license nor certifies other operating systems, cross-version ABI,
third-party redistribution, concurrent publishers, filesystem crash
recovery, or installed-package rollback. No earlier immutable Math
calendar release is available for a physical rollback test; preserving
the direct source consumer is not the same as proving package downgrade.

After independently verifying RS137, subsequent owner-authorized runs
may qualify `dw.quantities.expression`, `dw.quantities.standard` and
other packages in their true dependency order. Exact versioned
`PackageReference` closures, rights, collision refusal, isolated
consumers and separate local publications must be proven individually.
The neutral `dw.discovery` complex-CLR binding gate remains separate.
