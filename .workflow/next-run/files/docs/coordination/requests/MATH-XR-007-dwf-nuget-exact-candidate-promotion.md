# MATH-XR-007 — Exact immutable candidate promotion under the NuGet calendar policy

Status: draft. Recipient: dw.tools.workflow owning agent, coordinating with the dw.tools / dw.tools.nuget owner where the shared publication infrastructure is responsible. Transmission: none. Math requests review, not implementation authorization in another repository.

## Context / source baseline

Math M11 is complete; this is distribution maintenance, not new numerical work. Math RS433 result 539e857678e8097b93b27a7c3fffad565b3c8875, succeeded attempt 001 first-pass under stable DWF 0.1.81, guidance 0.32. [Durable disposition](../../distribution/m11-private-promotion-blocked-rs433.md) and [RS433 report](https://github.com/aihallot/dw.tools.math/blob/539e857678e8097b93b27a7c3fffad565b3c8875/.aura/workflow/reports/RS433/attempt-001.json).

The owner has explicitly authorized only the original two immutable archives, conditioned on distribution-policy admission, no collision, preserved NuGet Audit and independent readback. Both candidates are physically available and were independently qualified by Math RS394 and Decision RS197–RS202. No publication has occurred.

| Package | Qualified version | Archive SHA-256 | Embedded net10 DLL SHA-256 |
| --- | --- | --- | --- |
| dw.quantities | 20260.20261127.1001 | 1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d | e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed |
| dw.tools.math.statistics | 20260.20261127.1002 | cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8 | 134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3 |

Original local location (Math checkout only): build/artifacts/m11-candidates-1aa0ad9d4f7e/feed. Shared feed selector NUGET_LOCAL resolved to I:\nuget.local. These local files are not available on GitHub. No cross-repository local-path access is required for the policy review; use synthetic archives in disposable feeds for engineering tests. Statistics' exact dependency is dw.quantities [20260.20261127.1001].

## Observed policy conflict

The actual installed DWF 0.1.81 read-only command nuget plan for each PackageId, using the encoded date 2026-11-27 and explicitly selected private feed, returned 20260.20261127.1001 for both, with zero preview blockers. The statistics feed history has only 20260.20261106.1001 and 20260.20261120.1001; no 1001 on 2026-11-27. Quantities matches its own preview; Statistics 1002 does not. The pinned stable publish-intent policy at 288385e93c4e0652be2c715aa19208b6e966e1b2 requires the next ordinary candidate and would refuse the Statistics request. Math does not ask to weaken the normal allocator, fill the ordinal with a fictitious release, or bypass the publisher. RS392 selected proposed candidate identities but did not reserve them; a retrospective reservation claim would be false.

## Requested review and generic scope

First inspect the actual current guidance, installed/qualified capability and existing shared NuGet library rather than creating a Math-specific exception. Is there an already supported explicit promotion path for an independently qualified immutable archive whose identity is not the next ordinary publication ordinal? If so, state its precise contract, eligibility, authority, installed version, commands and evidence. If not, assess whether an explicit exact-candidate promotion operation (separate from ordinary allocation/publication) is appropriate. Consider how prequalified candidate identities should be reserved/allocated prospectively, with a durable identity/provenance receipt and how abandoned candidates, skipped ordinals, release-date semantics and concurrent allocation are handled. Existing historical candidates without reservation must require a deliberate policy decision, not fabricated provenance.

Indicative owner surfaces to inspect: DWF DotNetNuGetPublishIntentReview, DotNetNuGetCalendarVersionPlanner/FeedProjection, mixed-layout inventory and existing local publisher; shared dw.tools.nuget package inspection and release infrastructure. Determine the correct responsibility owner and coordinate rather than duplicate. No owner-side code change is requested without that owner's own planning and authorization.

## Acceptance boundaries for any supported route

- Keep ordinary next-calendar allocation and existing refusals unchanged; explicit separately admitted exact promotion only. Require canonical PackageId/version, exact existing archive SHA-256, independent qualification and source provenance, explicit owner scope, source/target path and feed identity, and a recorded reason for an otherwise skipped ordinal. Do not equate current Git HEAD with package build provenance.
- Reject already-present target even for matching bytes as a new publication, noncanonical/normalized aliases, conflicting mixed-layout histories, ambiguous identities, unexpected provenance, missing/changed bytes, path escape, unsafe cache/shared-feed overlap and unauthorized scope. Preserve exact bytes, existing packages, dependency closure, NuGet Audit and immutable receipts. Separate read-only verification of an existing release from publication.
- Qualify no-overwrite, collision handling and safe partial-pair recovery without fake rollback or blind replay; distinguish ordinary local filesystem guarantees from concurrent cross-layout/network-storage/power-loss limits. No irreversible shared-feed mutation in rehearsal or preselection. Use disposable feed cases for allowed exact skipped-ordinal promotion (if accepted), ordinary skipped-ordinal refusal, hash mismatch, duplicate/alias/collision refusal and interruption/recovery. No real package publication or Math/Decision repository edit.
- Document policy, accepted planning, targeted tests, supported tool version and execution contract; no SDK/consumer migration implied. If not admissible, state BLOCKED with reason and supported alternatives. A changed archive identity is outside the present Math owner authorization.

## Expected response

Return ACCEPTED / REJECTED / NEEDS_CHANGE (or existing supported route), reason, exact owning authority and reviewed revision, implementation/result commit and qualified installed version if applicable, commands and actual validation results, limitations and receipt links, exact conditions for Math to resume publication, and any required coordination with dw.tools.nuget. No fabricated reservation, publication, downstream adoption or change to Math/Decision. Math will preserve its existing candidate and rollback archives and can continue Math 2.0 while awaiting the answer.
