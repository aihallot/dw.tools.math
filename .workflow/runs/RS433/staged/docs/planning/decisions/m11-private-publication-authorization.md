# M11 — Conditional owner authorization for exact private promotion

Status: **APPROVED-CONDITIONAL**, owner decision 2026-11-28 after RS432. This record does not itself publish or reserve a package. Publication remains prohibited until every condition below is satisfied.

## Explicit owner decision

The owner authorizes private promotion into the configured NUGET_LOCAL feed of the following **original immutable** M11 archives only:

| Package | Version | Archive SHA-256 | Embedded net10 DLL SHA-256 |
| --- | --- | --- | --- |
| dw.quantities | 20260.20261127.1001 | 1795d4c74ac29f20930b4caacb9ca99ee397520ffefe7d16350c869c43a8b30d | e28cf245a30f341c0ddf06291fb2a7f7b51d2983b63c3015246fff56940762ed |
| dw.tools.math.statistics | 20260.20261127.1002 | cd29b2d8592f65e158f7401f363cc7903fbe69b18b28f787ad3f30d0d0d9bdc8 | 134de3c802ecdf1d51b7430152e11416c794a555a8c2938c519064078526a0f3 |

Authorization is expressly conditional on preserving the qualified hashes, admission of delayed promotion by the active distribution policy, absence of collisions, preserving NuGet Audit and independent verification after publication. No reconstruction, overwrite, public publication or Decision modification is authorized. No alternative package identity, implicit version allocation, guard bypass or broader distribution is authorized. Statistics depends on exactly dw.quantities [20260.20261127.1001]; publish Quantities before Statistics only once a safe route for both is established. Preserve the two historical rollback archives. No authority to claim Decision production adoption.

## Evidence and policy

- [RS432 result](https://github.com/aihallot/dw.tools.math/commit/19af7f836ccb054ef83e4431e42da38a28412465): first-pass, attempt 001, DWF 0.1.81; actual post-attempt readback of all four archive/DLL identities, exact dependencies, rights and absence of both canonical flat/hierarchical destinations. This is point-in-time evidence, not a reservation. See [the original proposal](../../distribution/m11-private-publication-proposal.md).
- [Active stable source pin for DWF 0.1.81 publish-intent policy](https://github.com/aihallot/dw.tools.workflow/blob/288385e93c4e0652be2c715aa19208b6e966e1b2/src/dw.tools.workflow/DotNetNuGetPublishIntentReview.cs) matches the previously attested core assembly identity. It derives its review date from the requested version, not the wall clock. Therefore delayed promotion is not inherently refused merely because the date of publication differs. However, the exact requested version must still equal the calendar preview for that PackageId; legacy/normalized aliases, ambiguous/conflicting histories, skipped ordinals, date regression and existing targets remain refusals.
- [Read-only preview contract](https://github.com/aihallot/dw.tools.workflow/blob/288385e93c4e0652be2c715aa19208b6e966e1b2/src/dw.tools.workflow/DotNetNuGetCalendarPreviewCommands.cs) uses explicit dates and feed, observes both layouts and exact overlaps, emits blockers and an informational candidate. A preview match is not a publication grant or final collision guard.
- RS433 invokes the actual installed stable read-only planner for both package IDs on their encoded date 2026-11-27, with the exact resolved NUGET_LOCAL path explicitly passed to the CLI. No ordinary publisher or alternate writer is invoked. Reuse existing RS432 archive/rollback verification in a new read-only mode. The report preserves actual observed CLI output, tool version and executable hash. If both exact previews match with no blockers, the next step is a disposable exact-archive publisher rehearsal. Otherwise record BLOCKED; do not publish either archive, alter versions, create a fake predecessor or fall back to another writer to evade the policy.
- A nonzero planner observation is not an admission; it may be a policy refusal or an environmental/availability problem. A successfully delivered blocked observation is not successful promotion. Unknown response syntax fails closed.
- Subsequent release qualification must still verify explicit feed scope, complete collision/alias and path safety boundaries, provenance, no-overwrite and partial-pair recovery, rehearsal, enabled Audit and independent post-publication readback. Actual private publication remains a separately controlled external mutation, never a disposable preselection side effect. No numerical requalification or reconstruction merely to obtain a different calendar identity.

Math source remains net11/C#15; M0-M11 done, M12 ready at the exact RS431 frontier, canonical reconciliation awaits return to M12. No change to native plan, backlog or sibling repositories in RS433.
