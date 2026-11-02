# Math owner — local NuGet federation F1 qualification

**Scope:** Math-owned bounded DWF read-only package intake, not an authorized
package release. This follows the 2026-11-01 neutral federation contract in
`aihallot/dw.tools/dw.tools.nuget/docs/dwf-nuget-federation-contract-20261101.md`,
the owner-specific F1-07 intake, and Decision's source-only calendar preflight.
The federation owns package mechanics; Math owns the thirteen package identities,
their rights, source, ABI, dependencies, evidence, release and rollback.

## Identity and version policy

Use only explicitly selected `NUGET_LOCAL` as the existing absolute local
publication feed; `NUGET_PACKAGES` is a distinct isolated consumer cache.
The qualified shared NuGet engine is `Dw.Tools.NuGet` package
`[20260.20261026.1010]`. Call the qualified
`DotNetLocalPackageRepository` for actual selected ZIP/nuspec inspection
and `DotNetCalendarPackageVersion.Calculate` for proposals.

Approved 2026 NuGet versions are `20260.YYYYMMDD.NNNN`, with an independent
`1001..1999` daily sequence per PackageId. The actual execution date and
observed feed history determine the next candidate. A proposal is neither
a reservation nor permission to write bytes. Existing preview archive identities,
including the two historically distinct private `dw.quantities 0.2.0-preview.1`
hashes, remain immutable and must not be silently migrated or overwritten.
Do not conflate NuGet `PackageVersion` with source `Version`, CLR
`AssemblyVersion`, scientific/API contract version or DWF milestone number.

## Current package graph (source-declared, not published)

The thirteen previously qualified packable libraries and original source versions
are recorded in `docs/distribution/current-local-consumption.md`. Direct
source package relationships (independently re-evaluated during F1) are:

- `dw.tools.math.foundation` — standalone
- `dw.quantities` — standalone
- `dw.quantities.expression` — dw.quantities
- `dw.quantities.standard` — dw.quantities + dw.quantities.expression
- `dw.tools.math.ir` — dw.quantities
- `dw.tools.math.composition` — dw.quantities + dw.tools.math.ir
- `dw.tools.math.numerics` — dw.tools.math.ir
- `dw.tools.math.statistics` — dw.quantities
- `dw.tools.math.symbolics` — dw.tools.math.ir
- `dw.tools.math.codecs` — dw.tools.math.ir
- `dw.tools.math.distribution` — dw.quantities, dw.tools.math.ir,
  dw.tools.math.codecs, dw.tools.math.symbolics, dw.tools.math.numerics,
  dw.tools.math.composition
- `dw.tools.math.analysis` — standalone
- `dw.tools.math.scientific` — standalone

Package-specific public licenses are not inferred from empty
PackageLicenseExpression fields. The owner-origin provenance includes
`docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` for selected
AURA code; different code and any third-party material need separately verified
notices and rights before actual redistribution.

## Bounded F1 consumer

`tools/dw.tools.math.nuget-preflight` uses the exact published core library
as a PackageReference-only utility, not a second version allocator, publisher
or format parser. It enumerates exactly thirteen source projects, queries their
effective MSBuild package/version/TFM/packability properties, checks project
dependency edges and uses a bounded temporary Math-only copy of existing
top-level nupkg files from the selected feed. ZIP/nuspec validation and calendar
version calculation delegate to `Dw.Tools.NuGet`. Unrelated entries in the
mixed feed are not parsed; unknown nested packages are explicitly out of scope.

The DWF receipt must record the tool's actually observed package inventory and
per-PackageId proposals, historical versions/hashes, exact installed shared-core
version, selected SDK, command exit codes and source identities. No physical
feed publication, package packing, source `.csproj` mutation, source tree
transfer, new scientific contract, Math Discovery migration, skill or broker
authority follows from F1.

## Next stages after a successful owner receipt

Qualify a first independent candidate (prefer `dw.quantities`) with a
pack-time `PackageVersion` override and a source-free compiled exact rational
consumer (1/3 + 1/6 = 1/2). Preserve its source `Version` unless a separately
approved product contract changes it. Confirm effective licensing, package
archive/DLL SHA-256, notice and rollback. Progress next through dependency
topology with exact `PackageReference` and complete binary-only consumers;
`dw.quantities.standard` also needs 250 g = 1/4 kg. Only after verified
candidate bytes and distinct explicit local-release authorization may the owner
invoke the existing shared no-overwrite publisher against `NUGET_LOCAL`.
No preexisting package bytes are replaced, global sources modified or
remote/public feed used. The current native Math M0–M7 roadmap stays done;
separate admitted product scope is necessary for delivery work beyond
an evidence-only F1 assessment.
