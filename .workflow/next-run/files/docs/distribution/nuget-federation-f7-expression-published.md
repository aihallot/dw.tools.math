# RS140 — Independent read-only verification of published Expression NuGet

## Authority and observed operator delivery

Math DWF RS138 delivered the original standalone dependent package candidate
on main `154d0c2b770de0d0d81efd9ee318cc6e4bc54662`. RS139 delivered its
rehearsed immutable local publisher in first pass (attempt 001) on main
`1dbfa8ce92f8c399b40868b7e6e398d195e38974`, with a separate disposable
feed, a six-check binary-only consumer and collision/digest/cancellation
refusals. Neither run published to the shared feed.

On **2026-11-03**, the owner separately executed the exact
`tools/dw.tools.math.nuget-expression-release` modes `--publish` and
`--verify` from the Math `main` checkout on MSI-02. The provided
terminal receipts report:

- `MATH-EXPRESSION-PUBLISHED|id=dw.quantities.expression|version=20260.20261102.1001|sha256=d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae|feed=NUGET_LOCAL|verification=pending`
- `MATH-EXPRESSION-VERIFY|id=dw.quantities.expression|version=20260.20261102.1001|binary=6|archive=d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae|dependency=20260.20261102.1001|feed=NUGET_LOCAL|PASS`

These are actual **operator-provided outputs**, not a machine-attested DWF
published-feed receipt. RS140 must independently re-read the actual
published archives before making that further qualification.

## Immutable package and dependency closure

Only the following exact source-owned private package identities are
admitted for this bounded check:

| Package ID | Calendar version | Archive SHA-256 | DLL SHA-256 | Archive bytes |
|---|---|---|---|---|
| `dw.quantities.expression` | `20260.20261102.1001` | `d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae` | `f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1` | 13,951 |
| `dw.quantities` | `20260.20261102.1001` | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` | `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4` | 14,747 |

The Expression `.nuspec` must declare exactly one dependency,
`dw.quantities [20260.20261102.1001]`; the base package has none.
These archives were created on 2026-11-02; publishing the exact original
Expression archive on 2026-11-03 does not authorize repacking or
changing its creation-date version. The different RS139 **rehearsal**
archive and DLL hashes must not be substituted for RS138 source bytes.

## Independent DWF observation

`tools/dw.tools.math.nuget-expression-feed-verify` is a new bounded
Math-owned, non-packable C# program with an exact
`Dw.Tools.NuGet [20260.20261026.1010]` dependency. It reads the two
physical top-level archives named by those exact PackageId/version
pairs from an existing absolute `NUGET_LOCAL`, checks source SHA-256,
length and no file redirection, then independently inspects their
`lib/net10.0` DLLs and `.nuspec` dependency graph.

The program copies **only these two archives** into a private,
GUID-owned and ignored folder; the existing published
`DotNetLocalPackageRepository` verifies their package identities,
versions and hashes there. It never enumerates or ingests the
unrelated contents of the shared mixed feed.

A freshly generated C# consumer project retains the **unchanged RS138
six-case fixture** as its only compiled source and declares exactly one
direct `PackageReference`:
`dw.quantities.expression [20260.20261102.1001]`. NuGet restores both
packages from the selected `NUGET_LOCAL` through a configuration
that clears all implicit sources, using a new private package cache
and no Math source `ProjectReference`. The compiled consumer must
load both DLLs with exact SHA-256 values and pass all six functional
and boundary checks. The physical shared-feed archives are checked
again after the run.

The RS140 payload records the genuine compiled command exit codes,
observed source and library SHA-256 values, loaded binary identity and
DWF lineage into a **new** Math-owned JSON receipt. Its only durable
product mutation is the new documentation/validator/tool/receipt.
Existing F1–F6 sources, reports and receipts remain byte-identical,
as do source project versions, the accepted backlog and all 541
completed DWF nodes. DWF never invokes the release controller's
`--publish` mode or rewrites `NUGET_LOCAL`.

## Scope restrictions and next dependency

This qualification is restricted to the two observed private,
Windows x64/.NET 10 NuGet packages and the six explicit consumer
checks. It does not grant a public license, select a release for
NuGet.org, verify every producer in the mixed feed, qualify cross-RID
ABI, prove physical installed-package rollback, or authorize
Decision/AURA/Discovery host or skill migrations. The original
historical source `Version 0.2.0-preview.1` remains unchanged.

Only after a successful, first-party DWF readback of these two exact
packages may the Math owner qualify `dw.quantities.standard`, whose
dependency closure contains both base Quantities and Expression.
The next Standard candidate must carry its own SHA-256, immutable
calendar version, rights and six-or-more independent semantic tests;
its publication likewise requires a separate owner-controlled gate.
