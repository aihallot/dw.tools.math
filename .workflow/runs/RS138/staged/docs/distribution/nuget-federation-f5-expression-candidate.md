# RS138 — Exact NuGet dependency candidate for dw.quantities.expression

## Provenance and dependencies

The owner independently verified the first locally published Math package
in RS137: `dw.quantities [20260.20261102.1001]` in `NUGET_LOCAL`,
14,747 archive bytes, SHA-256
`e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`,
DLL SHA-256
`c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.
That archive was compiled and consumed under its own immutable
`PackageReference`. The original source Version `0.2.0-preview.1`
is unchanged.

The next source-owned library in dependency order is
`dw.quantities.expression`, which has a single historical
`ProjectReference` to `dw.quantities`. Its own source Version is
`0.2.0-preview.1`, and its original project/build and
`packages.lock.json` remain untouched.

**Acceptance question:** can an independent package-only consumer load
an actual new Expression calendar NuGet archive with a *single exact*
NuGet dependency on the previously published `dw.quantities` package,
while source and historical version information remain unchanged?

## Packaging projection, without source edits

`tools/dw.tools.math.nuget-expression-candidate` is a new,
non-packable, Math-owned C# qualification controller. Its NuGet
library is the already published, centrally pinned
`Dw.Tools.NuGet [20260.20261026.1010]`. It uses the unchanged RS134
preflight `--inspect` command before and after the qualification for
the actual scoped `NUGET_LOCAL` inventory and dynamic
`20260.YYYYMMDD.NNNN` version proposal. No new version allocator,
shared-feed writer, or unrelated package inventory is introduced.

The controller verifies the original `dw.quantities.expression.csproj`,
the exact single ProjectReference, and the three original C# files.
It creates a separate temporary packaging **projection** under
ignored `build/artifacts/rs138-expression-candidate/`:

- exactly those three original source files, linked without rewriting
  them or reinterpreting their namespaces;
- unchanged package, assembly, description, author and source Version
  identity from the original project;
- an explicit pack-time PackageVersion calculated by the existing
  published NuGet calendar preflight;
- exactly one `PackageReference` to
  `dw.quantities [20260.20261102.1001]` in place of the
  historical source ProjectReference;
- no other production package dependency or source project reference.

This is **not** a second maintained Math implementation.
The linked project and source-free test are ephemeral build/consumer
artifacts. The original Math source project and its locked
source-to-source graph continue to work as before. The generated
source-linked package is a candidate only; no release is approved.

The actual package must pass `Dw.Tools.NuGet` archive inspection,
the exact nuspec `<dependency>` version check, the packaged
net10.0 DLL SHA-256 check, and zero unexpected dependencies. All new
candidate versions and hashes are recorded from real as-run bytes.
No preview or earlier archive is overwritten or falsely represented
as reproducibly byte-identical.

## Independent package-only consumer

A checked-in C# fixture compiles in a new temporary project that has
**only one direct PackageReference**:
`dw.quantities.expression [candidate-version]`.
Its NuGet configuration clears all implicit/global sources and includes
only the temporary candidate and already configured `NUGET_LOCAL`.
The consumer uses a private, isolated package cache and checks actual
loaded binary SHA-256 values for **both** the candidate Expression DLL
and the published base Quantities DLL.

Six independent controls test the two loaded assembly identities,
both loaded DLL fingerprints, exact expression arithmetic
`1/3 + 1/6 = 1/2`, typed divide-by-zero refusal, and length-budget
refusal. The final RS134 preflight inspection must find the scoped
local-feed inventory and candidate version unchanged. The as-run
receipt records each original source file SHA-256 and the source
project identity, dynamic version, actual package and DLL hashes,
dependency, .NET SDK, test outcomes and command exit codes.

## Owner authority and residual boundaries

RS138 is an evidence-only M7 continuation of accepted completed
Math M0–M7 scope, with 541 native DWF nodes retained. It neither
publishes to `NUGET_LOCAL` nor chooses a public package license.
The original owner attestation permits the transferred Math code but
is not an unconstrained third-party rights grant. The new Expression
candidate must retain package-specific notice/readme and inherited
license review as a separately recognized delivery gate.

The published base package and RS134–RS137 original receipts remain
immutable. No physical migration, installed NuGet rollback, other-RID
compatibility, generated Discovery skill, agent/broker admission,
remote publication or changes to Decision/AURA are qualified here.

After success, the Math owner should review candidate identity,
notices, rights and exact closure; then qualify the separate shared
`Dw.Tools.NuGet` no-overwrite publisher gate for Expression. A
physical shared-feed publication is an explicit owner action only.
The next dependent `dw.quantities.standard` package requires both
published base and Expression packages and cannot be qualified by
merely assuming Expression has been published.
