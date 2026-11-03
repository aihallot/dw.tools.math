# RS141 — Calendar NuGet candidate for dw.quantities.standard

## Authority, dependency graph and scope

RS140 succeeded on Math `main` at
`c3ddede907b0dc1b5cc4ebee18dc3545769875c1`, attempt 001
using DWF 0.1.63. Its first-pass result is **NO**, because a previous
pre-attempt compilation refusal was durably recorded (no earlier
attempt allocated). RS140 independently re-read both published
`dw.quantities` and `dw.quantities.expression` archives from the
owner-selected absolute `NUGET_LOCAL` feed and executed a fresh
six-case binary-only consumer. Its retained read-only evidence is
`docs/distribution/nuget-federation-f7-expression-published-qualified.json`.

The two exact immutable dependencies are:

| Package | Version | Archive SHA-256 | Packaged DLL SHA-256 |
| --- | --- | --- | --- |
| `dw.quantities` | `20260.20261102.1001` | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` | `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4` |
| `dw.quantities.expression` | `20260.20261102.1001` | `d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae` | `f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1` |

The next graph node is `dw.quantities.standard`, with exactly
two historical source `ProjectReference` edges to the above Math
libraries. Its current source `Version` is `0.2.0-preview.1`;
its existing project file, lock file, namespaces and five original C#
sources remain intact. These source edges are historical build
authority, **not** proof that the new package already exists in
`NUGET_LOCAL`.

## Temporary packaging projection, with no source edit

A new Math-owned, non-packable qualification tool at
`tools/dw.tools.math.nuget-standard-candidate` uses the exact
`Dw.Tools.NuGet [20260.20261026.1010]` shared package, not
another version allocator. The existing RS134 F1 inventory tool
provides a fresh on-date `20260.YYYYMMDD.NNNN` proposal for this
PackageId, without reserving it.

The controller hashes the original Standard project and these five
complete files: `ExplicitProfileExpressionUnitResolver.cs`,
`InheritedStandardUnitCatalog.cs`, `PureUnitConverter.cs`,
`StandardExpressionUnitResolver.cs`, and `StandardUnitCatalog.cs`.
It generates a separate packaging-only MSBuild project inside
ignored `build/artifacts/rs141-standard-candidate/producer`.
That project links the original C# files **without copying or
rewriting their bytes** and substitutes exactly two direct,
version-pinned NuGet `PackageReference` entries for the old
`ProjectReference` edges. The original `Version`,
`PackageId`, `AssemblyName` and `RootNamespace` stay
unchanged; only the private candidate's `PackageVersion` uses
the current calendar proposal.

Restore, Release build and pack must succeed from this temporary
source-linked project, using only `NUGET_LOCAL` as its package
source. The candidate archive remains in a separate ignored
`build/artifacts/rs141-standard-candidate/candidate` directory,
not the shared feed. The NuGet shared library independently inspects
this candidate's package identity, ZIP/nuspec and archive SHA-256.
Its nuspec must declare exactly two exact dependencies on the
published Quantities and Expression versions, with no other package
dependencies. The packaged Standard DLL SHA-256 and all original
source fingerprints are recorded from the actual run.

## Independent source-free triple-DLL consumer

The checked-in fixture
`tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs`
is compiled in a completely separate temporary test project whose
only **direct** package reference is
`dw.quantities.standard [candidate-version]`. A local-only
`NuGet.Config` clears implicit sources and allows only the
candidate and the existing owner-selected `NUGET_LOCAL`. A
private isolated package cache ensures no source ProjectReference
or previously restored arbitrary assembly supplies the result.

The ten concrete tests require:

1. The loaded Standard assembly has the expected identity.
2. The loaded Expression assembly has its published identity.
3. The loaded base Quantities assembly has its published identity.
4. All three loaded DLL SHA-256 values exactly match packaged DLLs.
5. Exact `250 g = 1/4 kg` through `PureUnitConverter`.
6. The 58-entry inherited unit catalogue retains canonical mass units.
7. Parsing `250 g` yields exact `1/4` in base mass units.
8. Ambiguous unprofiled `cup` is rejected with a typed failure.
9. An explicitly profiled US customary cup resolves.
10. Exact `1 in + 1 cm = 177/5000 m` through the parser.

Real restore/build/run exit codes, Standard package and DLL hashes,
the two immutable dependency hashes, current SDK, source hashes and
the F1 shared-feed inventory fingerprint before and after the test
must be captured by RS141. No original Math project versions or
unrelated accepted product files are changed; the existing 541-node
native plan remains fully done.

## Delivery limits and next owner gate

RS141 is **candidate qualification only**. It creates neither an
immutable publication in `NUGET_LOCAL` nor a new license or
redistribution authorization. The selected transferred-code rights
attestation is retained, but inherited and third-party notices and
package-specific README remain an owner review concern. Original
RS134–RS140 receipts and published archives stay immutable.

A later bounded gate must qualify the exact Standard candidate using
the existing `Dw.Tools.NuGet` no-overwrite publisher on a disposable
feed, then require an explicit separate private local publication of
the original candidate archive and independent DWF readback. Cross-OS,
cross-version ABI, installed NuGet rollback, NuGet.org publication,
Decision/AURA integration and Discovery complex-CLR binding are all
outside RS141.
