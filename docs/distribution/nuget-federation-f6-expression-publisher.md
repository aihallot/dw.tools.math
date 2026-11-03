# RS139 — Expression: qualified immutable local NuGet publisher rehearsal

## Owner evidence and target

RS138 succeeded first pass, attempt 001, on
`aihallot/dw.tools.math` main `154d0c2b770de0d0d81efd9ee318cc6e4bc54662`.
It built and verified a **candidate only** for
`dw.quantities.expression 20260.20261102.1001`, using its three
unchanged C# sources and one exact package dependency on
`dw.quantities [20260.20261102.1001]`.

The RS138 candidate is 13,951 bytes, archive SHA-256
`d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae`,
packaged/loaded DLL SHA-256
`f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1`.
Its compiled package-only consumer passed six controls. Its historical
source version remains `0.2.0-preview.1`. RS137 independently verified
the already-published base archive, SHA-256
`e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`,
and base DLL
`c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.

RS139 **qualifies the immutable publication controller for one dependent
Math package**. It does not itself authorize or perform shared-feed
publication. It is an evidence-only M7 continuation of the completed
541-node Math DWF roadmap, not a new scientific capability.

## One shared publisher, not another package engine

`tools/dw.tools.math.nuget-expression-release` is a new bounded
non-packable owner controller. It depends exactly on
`Dw.Tools.NuGet [20260.20261026.1010]`, and checks that the actually
loaded `dw.tools.nuget.dll` matches its qualified SHA-256
`b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.
The controller uses the existing `DotNetLocalPackageRepository` APIs
for package inspection, no-overwrite publication and refusal behavior;
it does not implement a second version allocator, publisher or NuGet
archive validation engine.

Three deliberately separate modes are supported:

- `--rehearse` is the **only mode called by DWF RS139**. In a fresh
  GUID-owned, ignored workspace, it compiles the original three C#
  Expression source files using a temporary project with one exact
  `PackageReference` to the published base Quantities package.
  It packs the real linked-source Expression DLL with the qualified
  package/version metadata, publishes this newly built archive to a
  disposable feed and verifies SHA-256 readback, nuspec dependency,
  duplicate-identity collision refusal, wrong-digest refusal and
  cancellation refusal. A separate compiled package-only consumer loads
  both DLLs and repeats the six functional/identity tests.
  The disposable workspace is deleted afterward.
- `--publish` is an **explicit separate owner/operator action** after
  RS139 is durably pushed. It requires a clean `main` checkout,
  successful exact RS139 report, owner rights attestation, the still
  existing RS138 original candidate file and its exact 13,951 bytes,
  original archive/DLL SHA-256, exact pinned dependency, and an
  existing absolute selected `NUGET_LOCAL`. It uses the shared
  `PreparePublication`/`Publish` no-overwrite operations. If the
  package identity collides or source bytes differ, it refuses; it
  never renumbers, overwrites or silently retries.
- `--verify` is a separate post-publication readback. It checks the
  exact physical archive and dependency from `NUGET_LOCAL`, then
  builds and runs the unchanged six-case RS138 binary consumer through
  one exact Expression `PackageReference` against that local feed.
  A later DWF run must independently reconcile the real published
  package; a successful owner-side `--verify` is not that DWF receipt.

**Important distinction:** the independently rebuilt rehearsal package
can differ byte-for-byte from the RS138 archive (as in the earlier
Quantities rehearsal). Successful disposable publishing proves the
shared publisher and dependent-package graph for those observed bytes,
not byte identity with RS138. Real `--publish` refuses anything other
than RS138's exact archived bytes.

## Calendar version and rights boundary

The federation contract's `YYYYMMDD` records the **candidate package
creation date**. This package was actually created on 2026-11-02.
Physical local publication at a later date does not change the package
creation date or authorize changing that immutable archive's version.
Unlike the earlier Quantities controller's conservative same-day
operational gate, this Expression controller preserves an already
created, independently qualified candidate after midnight, but refuses
a publication date *before* creation. If the original RS138 archive
is unavailable, **do not repack/rename it under the same identity**:
requalify a new candidate/version.

The provenance attestation permits selected Math code packaging and
private redistribution; it does not grant an open-source license or
clear unrelated third-party material. The original Expression candidate
had a missing README warning and no declared public license expression.
No NuGet.org, other remote registry, broad rights or public distribution
is authorized here. Publication into `NUGET_LOCAL` is a separate,
operator-invoked private act. `NUGET_PACKAGES` is always a separate
isolated consumer cache.

## Evidence, rollback and next gate

RS139 records the actual source-project provenance, dependent archive,
the distinct disposable archive/DLL hashes, bounded command receipts,
six binary checks, all shared publisher refusals, and the absence of a
shared feed write. Historical RS134–RS138 receipts, Math source projects,
`Directory.Packages.props` and the 541 native nodes remain unchanged.

This gate does not qualify physical installed-package downgrade,
shared-folder crash recovery, concurrent publishers, remote source
adoption, multi-OS support, or Discovery CLR binding migration. In a
post-publication crash or unknown side-effect state, inspect physical
destination bytes before any retry; do not overwrite or delete the
original `dw.quantities.expression` archive. Once an explicit
`--publish` and separate `--verify` succeed, the next DWF owner
run should independently re-read the actual `NUGET_LOCAL` archive
and execute the six-case binary consumer before planning the
two-dependency `dw.quantities.standard` package.
