# Math local V1 — conditional adoption, rollout and rollback recipe

**Status:** documentary local candidate, NOT executed or transmitted.
Artifact under review: dw.tools.math.distribution 1.0.0-preview.4
and seven versioned local net10 NuGet archives.

## Entry prerequisites

Owner has attested Math source rights for selected AURA material; this
does not itself authorize automatic publication of a stable NuGet 1.0.
Gate evidence: M0–M4, M5-W01, M5-W02 C01/C02, and ten bounded V1
criteria in the local candidate map. Runtime qualified only on observed
win-x64 SDK 10.0.402, not Linux/macOS or other untested RIDs. A present
public API fingerprint does not prove binary compatibility with another
version. Treat unsupported mathematical domains and unknown results as
typed refusals.

## Conditional downstream adoption (future, recipient-owned)

1. Recipient AURA, Decision or MCDM owner explicitly accepts a **sent**
   scoped request and records its own receipt. The present drafts
   MATH-XR-004/005 are NOT sent.
2. In the recipient's repository and build environment, pin exact Math
   NuGet package IDs/versions and verify SHA-256, no sibling source
   ProjectReferences, notices and license obligations. Verify actual RID,
   native/managed dependencies and exact/approximation policy.
3. Run a shadow/side-by-side adapter under recipient supervision without
   replacing active behavior. Run existing recipient scientific suites,
   real migration edge cases, budget/cancellation and compatibility tests.
4. Compare outputs, original exclusions, provenance and error policies;
   record the actual recognized baseline, alternative and ownership.
5. Only the recipient and authorized owner may approve cutover. They
   must preserve previous package/feature selection for rollback and
   record how callers identify old and new behavior.

## Mandatory abort and rollback triggers

Abort before cutover on unsupported RID, missing package/notice,
changed API contract, numerical divergence, silently lost exclusion or
rounding policy, unknown provider state, timeout, or absent explicit
authorization. Revert recipient dependency pin/adapter selection to its
previous verified state, rerun recipient acceptance, and retain source
provenance/receipts. Do not perform an automatic cross-repo revert from
dw.tools.math.

## Owner publication hold

Stable 1.0 versioning, public NuGet publication, release signatures,
licensing choice and any newly announced supported platform require
separate recorded authorization, appropriate gate evidence and actual
platform tests. The present Math T2 qualification documents refusal and
rollback conditions, not publication or rollback execution. C03 and W02
can be locally completed; M5/G-M5 remains open.
