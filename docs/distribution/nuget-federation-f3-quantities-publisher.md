# RS136 — Independent local NuGet publisher qualification for dw.quantities

## Status and authority

This is the third Math-owned NuGet federation checkpoint, following the
successful, first-pass RS135 at `cc88f8c69a37037ab5c1d2437accf07ff104df26`.
RS135 independently compiled and exercised the exact package-only C# consumer
against the private `dw.quantities 20260.20261102.1001` candidate. Its archive
SHA-256 is `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`
(14,747 bytes), and its packaged/loaded DLL SHA-256 is
`c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.
The standalone library has zero package dependencies and retains historical
source `Version 0.2.0-preview.1`.

This run qualifies the **publication controller**, not a physical release into
`NUGET_LOCAL`. The scope is a bounded evidence-only M7 continuation. Accepted
M0–M7 product completion (541 native DWF nodes) and RS133–RS135 receipts remain
unchanged.

## Shared implementation, no second publisher

The Math-owned non-packable C# controller is
`tools/dw.tools.math.nuget-release`. Its exact shared dependency is
`Dw.Tools.NuGet [20260.20261026.1010]`; it hashes the actually loaded
`dw.tools.nuget.dll` against the qualified
`b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.
NuGet candidate inspection, no-overwrite publication and collision control
delegate to the existing `DotNetLocalPackageRepository`, not another in-house
version allocator, ZIP release manager or publishing algorithm.

This source has three deliberate modes:

- `--rehearse`: **the only mode executed during RS136 DWF**. Restores,
  builds and packs actual `dw.quantities` source into a fresh ignored
  GUID-owned workspace. Publishes the resulting real package only to a
  fresh **disposable** feed and independently rehashes archive and DLL bytes.
  Asserts exact no-overwrite collision refusal, altered-hash refusal and
  pre-cancelled refusal. The temporary feed is deleted afterward.
- `--publish`: a separate explicit owner/operator action *after* successful
  RS136 DWF delivery. Requires clean `main`, RS136's exact successful
  report, the physical RS135 archive with its exact byte count, SHA-256,
  package identity and DLL SHA-256, the original rights attestation, an
  existing absolute `NUGET_LOCAL` and an unoccupied publication target.
  Reuses `DotNetLocalPackageRepository.PreparePublication` and
  `Publish` with no-overwrite and post-copy hash verification.
- `--verify`: separately reads the physical immutable archive from
  `NUGET_LOCAL`, verifies its archive/DLL fingerprints and executes the
  **unchanged RS135 source-free binary consumer** from the published
  package using a new local-only NuGet project/cache.

The run's prepared payload MUST call `--rehearse` only. It must never
invoke `--publish` or `--verify` as though a feed release already exists.
A successful DWF report is a qualification of the controller on a
disposable feed, not authorization for an implicit shared-feed write.

## Version date and physical-byte restrictions

The accepted 2026 format is `20260.YYYYMMDD.NNNN`. Date and daily sequence
belong to actual package creation and each PackageId, not the source's
semantic project version or workflow milestone. Because the selected RS135
candidate is `20260.20261102.1001`, the guarded `--publish` operation
**refuses when the operator's local calendar date is not 2026-11-02**.
If that condition cannot be met, the owner must requalify a new dated,
unreserved candidate with a fresh NuGet receipt. No renaming, version
reuse, shortcut or overwrite is permitted.

`--publish` also refuses when the ignored physical RS135 candidate is
missing, replaced or has changed bytes. A GitHub receipt alone never
materializes that .nupkg on a different computer. Rebuilding a new archive
under the same ID/version without comparing its physical fingerprint
does not satisfy the release gate.

The package has no declared `PackageLicenseExpression` and RS135's
`dotnet pack` emitted a missing-package-readme warning. The signed-off
2026-10-07 owner attestation authorizes the selected AURA-derived Math
code to be packaged and redistributed, but does **not** select an
open-source license or clear unrelated third-party material. This
package is restricted to explicit **private local-feed** use pending
owner review of notices/rights; no NuGet.org or public export is granted.

## Recovery and following gate

If the shared publication API reports a collision, cancelled operation
or altered bytes, it must **not** overwrite, remove, renumber or silently
retry a package. If the host crashes after an unknown side effect, the
operator must first inspect the destination read-only and reconcile the
physical bytes with the original qualified candidate.

RS136's rehearsal does not establish crash recovery, concurrent
publisher safety, network/distributed-filesystem guarantees or physical
package-to-package rollback. Math has no previously published immutable
calendar-version package; the proven unchanged source/direct-call
path is a fallback, not an installed NuGet rollback. The publication
controller returns a locally ignored post-publication record, but that
record is not itself a DWF result or independently accepted receiver
qualification. A later RS137 should independently verify the actual
shared-feed archive and binary consumer, reconcile publication evidence,
and then proceed to dependent quantities packages in source dependency
order. All external owner communications and Discovery migration remain
separate.
