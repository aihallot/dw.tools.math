# RS143 — Independent read-only DWF qualification of the published Standard package

## Operator publication and separate source authority

Math RS141 produced and qualified the original private NuGet candidate
`dw.quantities.standard 20260.20261103.1001` under
`docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`.
RS142 succeeded on GitHub `main` at
`1faa208abceac82a4cb0f3ec62f98ad62ca7641a`, attempt 001,
first pass. It qualified the real shared `Dw.Tools.NuGet` publisher and
ten-case source-free consumer in a fresh **disposable** feed, without a
shared-feed write.

On **2026-11-04 at 00:31 Europe/Brussels**, the operator separately
reported successful `--publish` and `--verify` commands from a clean
Math checkout on MSI-02. The machine output stated:

- `MATH-STANDARD-PUBLISHED|id=dw.quantities.standard|version=20260.20261103.1001|sha256=c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75|dependencies=2|feed=NUGET_LOCAL|verification=pending`
- `MATH-STANDARD-VERIFY|id=dw.quantities.standard|version=20260.20261103.1001|binary=10|archive=c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75|dependencies=2|feed=NUGET_LOCAL|PASS`

These are **operator-provided outputs**, not an independent DWF
physical-feed receipt. RS143 establishes that second trust boundary,
and must refuse rather than create, rename or overwrite a missing
archive.

## Exact package and transitive dependency bytes

The following **three** packages are the full selected closure:

| Package ID | Exact version | Archive bytes | Archive SHA-256 | net10.0 DLL SHA-256 |
|---|---|---:|---|---|
| `dw.quantities.standard` | `20260.20261103.1001` | 13604 | `c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75` | `5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d` |
| `dw.quantities.expression` | `20260.20261102.1001` | 13951 | `d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae` | `f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1` |
| `dw.quantities` | `20260.20261102.1001` | 14747 | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` | `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4` |

Standard's `.nuspec` must declare exactly two pinned
dependencies, on `dw.quantities` and `dw.quantities.expression`.
Expression in turn must declare exactly one exact dependency on
`dw.quantities`. Base Quantities has no NuGet dependencies.
These were candidate **creation** dates (November 3 and November 2),
not a request to rewrite a version when physically published later.

The separately rebuilt RS142 rehearsal archive has a different SHA-256
than RS141's exact candidate. RS143 may only accept the original
RS141 package archive, not its rehearsal replacement.

## Independent read-only verification

The new non-packable owner tool
`tools/dw.tools.math.nuget-standard-feed-verify` uses
`Dw.Tools.NuGet [20260.20261026.1010]`, pinned by the actual loaded
shared DLL SHA-256
`b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.

It validates the existence, bounded byte lengths, SHA-256 and absence
of file redirection of precisely the three named top-level physical
archives in the owner-selected absolute `NUGET_LOCAL`. It inspects
their individual `.nuspec` identities and net10.0 DLLs, including
the exact dependency graph.

It copies **only** those three packages into a fresh ignored folder,
then uses the existing published
`DotNetLocalPackageRepository` read-only API to check their
three identities and package hashes. The tool does **not** enumerate
the entire mixed feed, infer published versions from filenames, or
create another archive/package publisher.

The unchanged RS141 `StandardBinaryConsumer.cs` fixture is compiled
in a new isolated project. Its only direct `PackageReference` is
`dw.quantities.standard [20260.20261103.1001]`. Restore uses
`NUGET_LOCAL` with global sources cleared and a new private package
cache. The consumer must load all three **actual NuGet DLLs** with
their exact SHA-256 values and pass all ten original cases, including
`250 g = 1/4 kg`, explicit cultural profiles, exact inherited
standard units and ambiguity refusal. The three physical shared-feed
archives are hashed and inspected again afterward to detect drift.

Only a new DWF-controlled Math documentation, executable verifier,
validation script and evidence receipt are persisted. The 541
already completed native product-plan nodes, original source
`Version 0.2.0-preview.1`, historical ProjectReferences,
and prior RS134–RS142 source/evidence remain unchanged.

## Limits and next gate

Successful RS143 proves a bounded **private local-feed** readback on the
observed Windows x64/.NET 10 platform, not public licensing,
NuGet.org release, total-feed integrity, concurrent-publisher safety,
crash recovery, package downgrade/installed rollback, other OS/RID
ABI or remote consumer adoption. It does not authorize any
Discovery skill, Decision change or AURA integration.

A following owner-controlled run can inspect the other Math packages'
source dependency graph and qualify the next specific leaf-compatible
calendar NuGet candidate with isolated binary checks and its own
immutable local release gate. Do not infer publication of any of
the other ten Math packages merely from a complete build snapshot.
