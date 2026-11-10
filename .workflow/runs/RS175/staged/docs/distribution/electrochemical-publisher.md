# RS175 — Original electrochemical archive publisher qualification

## Authority and unchanged scope

RS174 succeeded at attempt 001 on main result
`bca063866734c105f5448bba049e620b9d490c42`, after three pre-attempt
refusals. It was not a first-pass success. Its twelve binary checks and
three loaded DLL fingerprints are acquired evidence, not a campaign to replay.
The candidate is `dw.quantities.standard 20260.20261110.1001`:
archive SHA-256 `766e756e402ced1303a3b86d3faef39cfb1f40d00e34121288cc97b372968e6d`,
14,269 bytes, Standard DLL SHA-256
`9020646c909cc1a5408c19f51919681a8e21d43024ec8530a2119b328e8dd17a`.

This gate reuses the pinned `Dw.Tools.NuGet [20260.20261026.1010]`
publisher and reader. It does not add a second publication algorithm or
change the previous Standard release controller, which remains bound to
its historical five-source package. The new controller never builds or
packs a replacement candidate.

## Seven bounded controls on the exact original archive

The original archive is published only to a new run-owned disposable feed
under `build/artifacts/rs175-electrochemical-publisher/`. Seven named controls
verify preparation without file writes, exact-byte publication, independent
physical archive readback, collision refusal, digest mismatch refusal,
cancellation refusal, and preservation of the original inputs. An ordinary
child directory exercises the explicit nonrecursive directory policy.
The full five-entry ZIP inventory, including the Standard DLL, is compared
with RS174. The original Quantities, Expression and old Standard archive
hashes remain mandatory before and after rehearsal.

RS175 does not restore or rerun the twelve binary probes, the source tests,
or a general test suite. Only the new publisher controller is restored and
compiled. No package is written into `NUGET_LOCAL` by this gate.

## Original archive availability during disposable preparation

DWF's disposable preparation does not contain ignored build artifacts.
The controller first checks the exact candidate path recorded by RS174 in
its current repository root. When that file is absent, it may read only the
same pinned relative archive under the approved operator checkout
`D:\data.dev\github\ai@hallot.net\dw.tools.math`.
This is a read-only archive input, not a replacement execution checkout.
No sibling repository or disk is searched. Shared path/ancestor checks and
the exact archive size, SHA-256 and ZIP inventory apply in either case.
Missing or changed original bytes cause refusal; reconstruction is forbidden.
The receipt explicitly records the actual source and whether the operator
archive was borrowed. The operator archive is never deleted or modified.

## Native plan and next trust boundaries

The existing M1 scope remains done -> done; M3 and all 541 native nodes are
retained. There is no new completion achievement or AURA completion claim.
The source project version, six profile definitions, 58 inherited units,
14 Standard units, and two immutable dependencies remain unchanged.
AURA historical reuse rights remain unestablished.

The CLI supports only `--rehearse-publisher <repository-root>` in RS175.
After durable success, a separate authorized local-publication gate must
use these same candidate bytes, never overwrite an existing identity, and
then independently read the archives physically published in `NUGET_LOCAL`.
This rehearsal is not shared-feed publication, published-consumer evidence,
installed rollback qualification, public-release authorization, or a license
or SPDX decision. `request.json` is written last in the preparation.
