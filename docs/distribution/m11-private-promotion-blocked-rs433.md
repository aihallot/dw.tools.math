# M11 — Private promotion disposition after RS433

Status: **BLOCKED** (publication policy, not numerical qualification). Owner authorization is conditional and remains recorded; no private or public publication is admitted or performed.

## Exact delivered observation

RS433 succeeded, attempt 001, first-pass under DWF 0.1.81, zero prior invocation failures and zero mutation boundary violations. [Result commit](https://github.com/aihallot/dw.tools.math/commit/539e857678e8097b93b27a7c3fffad565b3c8875) and [durable report](../../.aura/workflow/reports/RS433/attempt-001.json). The read-only installed planner observed explicit feed `I:\nuget.local`, encoded candidate date 2026-11-27, zero hierarchical entries and zero overlap/preview blockers for each selected PackageId. Exact canonical targets were absent; all four original/rollback archive and embedded DLL hashes were unchanged. It returned these exact candidate previews:

| PackageId | Requested immutable version | Next ordinary calendar preview | Admission |
| --- | --- | --- | --- |
| dw.quantities | 20260.20261127.1001 | 20260.20261127.1001 | exact match only |
| dw.tools.math.statistics | 20260.20261127.1002 | 20260.20261127.1001 | blocked: skipped ordinal |

```text
MATH-RS433-POLICY|id=dw.quantities|requested=20260.20261127.1001|preview=20260.20261127.1001|result=EXACT-CANDIDATE
MATH-RS433-POLICY|id=dw.tools.math.statistics|requested=20260.20261127.1002|preview=20260.20261127.1001|result=BLOCKED
MATH-RS433-PROMOTION-POLICY|date=2026-11-27|verdict=BLOCKED|ownerApproval=conditional|publication=false|rebuild=false|rehearsal=pending|PASS
```

The statistics flat history is 20260.20261106.1001 and 20260.20261120.1001. It has no published 2026-11-27 ordinal 1001. No collision was reported in the observed histories. The nonmatching preview, not elapsed time, is the demonstrated blocker. The installed policy correctly refuses an ordinary non-next version; the candidate preparation selected 1002 in RS392 without a durable reservation. The earlier source states that these were proposed identities, not published versions; the reason for selection of 1002 is not established by that record. Do not invent a missing publication or retrospectively claim reservation.

## Decision

Preserve both archives and all numerical/recipient qualification. Do not publish Quantities alone, reconstruct or repack Statistics at 1001, create a filler predecessor, change the encoded date, relax NuGet Audit, bypass the ordinary publisher with a different writer, remove historical packages, or modify Decision. No change to SDK11/net11 Math production source. The [owner authorization](../planning/decisions/m11-private-publication-authorization.md) expressly requires policy admission, so the current condition is unsatisfied.

Prepare the [owning-agent request](../coordination/requests/MATH-XR-007-dwf-nuget-exact-candidate-promotion.md) for a generic review of exact previously qualified candidate promotion and future version allocation/reservation. The owning agent may confirm a supported existing route or propose a separately explicit, qualified policy. Math must not change DWF or shared NuGet infrastructure itself. The request is a draft for owner transmission only. No rehearsal or real release until policy admission for both archives and the original authorization conditions are demonstrably satisfied. No replacement identity without separate owner direction and targeted recipient evidence. No promise that an exception will be accepted.

## Response to Decision (operator relay; not transmitted)

Verdict **BLOCKED**. RS432 and RS433 proved current availability and exact identities of both original M11 candidates and both rollback archives. Publication is conditionally authorized by the owner but the actual DWF calendar policy rejects Statistics ordinal 1002 because its next ordinary version is 1001. Quantities matches; neither package has been published or reserved. The current private experimental feed is not a production route. Continue to preserve RS197–RS202 and the existing dispositions; do not replace production dependencies or repeat the numerical qualification. Math requests upstream policy review for the immutable pair, without forcing a net11 migration or new identity. Resume production acquisition only after a separate durable policy admission, publication and independent readback. Reference the RS433 report and the exact hashes in the [authorization record](../planning/decisions/m11-private-publication-authorization.md).

## Math 2.0 continuation

This is a maintenance disposition on completed M11, not a failed allocated run, a reopening of M11, or new product progress. Preserve the five ready native nodes established by RS431, the remaining planned roadmap and existing historical evidence. Once RS434 durably records this disposition, reconcile canonical M12 readiness from RS431 and resume the Question stage independently while the upstream response is pending. If an upstream response later admits a route, verify current archive/feed identity again and resume the bounded distribution gates; do not infer publication from a policy change.
