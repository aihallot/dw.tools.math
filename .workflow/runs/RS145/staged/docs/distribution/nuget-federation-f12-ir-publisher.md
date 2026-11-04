# RS145 — Immutable local NuGet publisher rehearsal for Math IR

## Durable source evidence and exact dependency

RS144 succeeded first-pass on Math `main` commit
`762ca5bcd2b86e36a5f116e63b0f55148ac950dc`,
attempt 001 with DWF 0.1.63. Its new, **unpublished**
`dw.tools.math.ir` calendar NuGet candidate has the following
immutable observed identities:

- Package `dw.tools.math.ir [20260.20261104.1001]`.
- Original candidate length **21,926 bytes**.
- Original archive SHA-256
  `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480`.
- Original packaged/loaded IR DLL SHA-256
  `5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af`.
- One and only one package dependency,
  `dw.quantities [20260.20261102.1001]`, whose published archive
  SHA-256 is
  `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`
  and DLL SHA-256 is
  `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.
- Original IR source `Version 0.3.0-preview.1`, six C# source
  files and historical single ProjectReference, unchanged.

The RS144 receipt records fifteen real source-free binary checks on
`win-x64`/.NET 10.0.403, including exact rational JSON, finite
binary64 signed zero, domain assumptions, matrix shape and typed
refusals. It is a **candidate** receipt, not a private publication
receipt or public license.

## Shared publisher, three separate operator modes

`tools/dw.tools.math.nuget-ir-release` is a Math-owned,
non-packable release controller that uses the previously published
and centrally pinned
`Dw.Tools.NuGet [20260.20261026.1010]`, with approved DLL SHA-256
`b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.
Its real `DotNetLocalPackageRepository` API handles preparation,
immutable publication, verification and safety refusals; this
qualification does not create another version allocator or publisher.

The controller supports three deliberately distinct modes:

- `--rehearse` — the **only mode called by DWF RS145**. It
  verifies the original IR project and six source file SHA-256
  values against RS144. Inside a fresh ignored GUID-owned workspace,
  it builds and packs an actual IR archive from exactly those
  original source files, linked unchanged in a temporary packaging
  project with the exact published Quantities PackageReference
  replacing the historical ProjectReference only in the temporary
  project. It publishes the newly built archive **only into a fresh
  disposable feed**. It checks the exact IR package id/version,
  net10.0 DLL, single `.nuspec` dependency, readback SHA-256,
  duplicate-identity collision refusal, changed-digest refusal,
  cancellation refusal, and unchanged base archive.
  A new compiled package-only consumer references only this
  disposable IR package directly, restores its base dependency
  solely from `NUGET_LOCAL` and repeats all **15** RS144 tests.
  The disposable workspace is deleted at the end.
- `--publish` — a **separate explicit owner action after RS145
  delivery**, never called by RS145 itself. It requires the exact
  successful RS145 DWF report, original Math private rights
  attestation, clean `main`, existing absolute selected
  `NUGET_LOCAL`, original RS144 package bytes (21,926) at their
  expected ignored build path, original archive/DLL hashes and
  precisely the pinned Quantities dependency. Only the approved
  `PreparePublication` and `Publish` methods may write the
  shared feed; collision, repacking or receipt replacement are
  refused. The owner-side publication receipt remains separate from
  the later DWF independent readback.
- `--verify` — a separate owner-side, post-publication check of
  the physical original IR archive in `NUGET_LOCAL`, plus a new
  source-free consumer with one direct exact IR PackageReference
  and 15 tests. It does not create a DWF result or authorize a
  public registry release.

**Byte identity limit:** a package rebuilt during the rehearsal
may have different archive or DLL bytes from the RS144 original.
The rehearsal must record both identities separately. Its
successful disposable publication proves the publisher's actual
behavior, not byte-for-byte reproducibility of the first build.
The real `--publish` mode refuses any replacement for the original
RS144 exact 21,926 bytes.

## Dates, permission and scope boundary

The IR candidate's `20261104` version segment records its
creation date, **2026-11-04**. A later physical private
publication may preserve the exact version only while the original
candidate archive remains byte-identical; a rebuilt or renamed
archive with the same ID/version must be refused.

RS145 is an evidence-only continuation of completed Math M7 and
the accepted 541-node native plan. It may add its new C#
controller, documentation, validator and DWF receipt only. The
historical IR project and sources, two earlier published Quantities
libraries, Standard published feed archive and all RS134–RS144
canonical receipts remain immutable. It must not call `--publish`
or alter `NUGET_LOCAL` as part of DWF.

Source-owner rights attestation permits private packaging but does
not select an open-source license or clear inherited third-party
notices/README. This gate does not qualify NuGet.org,
cross-platform/RID ABI, installed package downgrade/rollback,
concurrent writers, crash recovery, Discovery skill generation,
Decision dependency migration or AURA agent admission.

After RS145 succeeds, a separate owner decision can authorize the
guarded IR `--publish` and `--verify` modes. Only their actual
outputs, followed by an independent DWF physical-feed readback,
can qualify IR as the fourth locally published Math NuGet package.
