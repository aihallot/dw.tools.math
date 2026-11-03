# RS142 — Standard immutable local NuGet publisher rehearsal

## Source authority and accepted dependency graph

RS141 succeeded first-pass, attempt 001, on Math main
`2ba80728052cd5611d99d0a7c5984bee4b7fdba9`.
The source-owned `dw.quantities.standard` candidate, created
2026-11-03, has the exact immutable identity:

- Package: `dw.quantities.standard [20260.20261103.1001]`.
- Candidate archive: 13,604 bytes; SHA-256
  `c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75`.
- Packaged/loaded DLL SHA-256:
  `5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d`.
- Original project version `0.2.0-preview.1`, with exactly two historical source `ProjectReference` edges, unchanged.
- Ten independently executed semantic and three-loaded-DLL checks.

The candidate declares exactly two NuGet dependencies:

| PackageId | Version | Immutable archive SHA-256 | DLL SHA-256 |
|---|---|---|---|
| `dw.quantities` | `20260.20261102.1001` | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` | `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4` |
| `dw.quantities.expression` | `20260.20261102.1001` | `d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae` | `f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1` |

Both dependencies were independently observed from the actual
`NUGET_LOCAL` feed under RS137 and RS140 respectively. RS142 neither
changes nor republishes either prerequisite. Only Standard's source-owned
package release controller is newly qualified.

## Shared publisher mechanics, not a new release engine

The new, non-packable `tools/dw.tools.math.nuget-standard-release`
controller uses the already published
`Dw.Tools.NuGet [20260.20261026.1010]` library, pinned by loaded DLL
SHA-256 `b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.
Exact archive inspection and collision/no-overwrite publication reuse
`DotNetLocalPackageRepository`. There is no second NuGet version
allocator or independent publisher implementation.

The controller separates **three explicit modes**:

- `--rehearse` — the **only mode invoked by DWF RS142**. It checks
  original RS141 source hashes, the two physical published dependency
  archives, owner receipts and the shared NuGet binary. A fresh
  GUID-owned temporary project links the original five Standard C#
  source files unchanged, replacing historical ProjectReferences only
  in that disposable project with exactly two version-pinned
  PackageReferences. It compiles and packs a real Standard package,
  then publishes it **only to a new disposable feed** through the
  existing shared publisher. It checks archive/DLL identity and exact
  `.nuspec` dependencies, duplicate-identity collision refusal,
  digest-mismatch refusal, cancellation refusal, and zero unwanted
  shared-feed changes. An independent, source-free, package-only
  consumer executes the unchanged ten-case RS141 binary oracle
  against the disposable Standard archive and the two already
  published Math dependencies. All temporary rehearsal files are
  deleted afterward.
- `--publish` — a **separate, explicit owner/operator action** after
  successful RS142 delivery. It requires a clean Math `main`
  checkout, the successful RS142 DWF report, existing private
  packaging/redistribution attestation, creation date not in the
  future, both unchanged published dependencies and the **exact
  original** RS141 physical archive at its ignored source path.
  It refuses a changed candidate byte count, SHA-256, DLL SHA-256,
  `.nuspec` graph, already occupied receipt or a feed collision.
  It then invokes shared `PreparePublication` and `Publish` with
  no overwrite, and rehashes the actual destination. The local
  publication receipt is not an independent DWF attestation.
- `--verify` — a separate read-only operator check of the physical
  Standard release and a freshly compiled exact source-free consumer
  against the selected local feed. A later DWF run must independently
  re-read the actual published package and dependency closure.

The RS142 payload and its preparation-safe validations may invoke
`--rehearse` only. They must never invoke either real publication or
post-publication verification, reserve any identity, copy a candidate
into `NUGET_LOCAL`, or edit historical package versions.

## Identity, dates and limitation of the rehearsal

A candidate version's `YYYYMMDD` denotes the actual **creation**
date, not necessarily the later physical private publication date.
The original RS141 archive was created 2026-11-03. A later explicit
publication can retain `20260.20261103.1001` only when the
**identical 13,604 original bytes** exist. It must not rebuild a new
archive under the same immutable ID/version. The rehearsal rebuild
may legitimately have a different SHA-256, as observed in earlier
Math release rehearsals; it qualifies the publisher, not bit-for-bit
reproducibility of RS141.

The source-owned private rights attestation does not choose a
public/open-source license, grant redistribution of unrelated
third-party content or remove the package README/notices debt.
No NuGet.org publication, production capability activation,
Decision/AURA modification, generated Discovery skill, installed
NuGet rollback, crash recovery, concurrent-writer assurance,
network filesystem guarantee or non-Windows RID certification
is authorized by this run. Accepted Math M0–M7 completion and 541
native nodes are unchanged; RS134–RS141 sources/receipts remain
byte-identical.

## Next bounded gate

Once the disposable rehearsal succeeds, the owner can review the
original RS141 archive, rights and private feed destination and
**separately** execute the guarded `--publish` and `--verify` modes
from a clean Math `main` checkout. An independent follow-on DWF run
must inspect the actual published archive and run the ten-case
binary-only consumer before any downstream package is admitted.
