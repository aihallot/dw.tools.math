# MATH-XR-001 — AURA baseline then exact-core adoption

Status: **draft**. Recipient: **AURA owning agent**. Transmission: **none**.

This file is owner-mediated. Math prepares it; a human/operator transmits it. Its existence does not mean AURA has received, acknowledged, accepted, implemented, or verified anything.

## Baseline request — M0-W02-C03

Pinned AURA revision: `82a6b435a387a7e116b47a6b2c433ae9e067bf21`.

Math evidence:

- `docs/planning/source-baseline.json`
- `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- `docs/planning/evidence/M0-W02-C02-transfer-manifest.json`
- `docs/coordination/requests/MATH-XR-001-aura-baseline.json`

### Request to the AURA agent

Read your own resource policy and current repository state. Compare the pinned selected Math-related sources/tests against your current AURA revision and report every changed selected file.

Confirm or correct:

1. the direct-transfer candidates for `dw.quantities`, `dw.quantities.expression`, and `dw.quantities.standard`;
2. the decision to keep `dw.localization`, culture resolution, localization projection, and host-facing catalog presentation in AURA;
3. the classification of `UnitConverter.cs` as a source for pure conversion behavior rather than a mandatory raw-file transfer;
4. the transitive consumers that later adoption must revalidate: core math actions, kernel, nutrition, data transforms, SQLite, JSON, workers, and CLI paths;
5. any source/test/consumer drift since the pinned revision.

Do **not** remove, replace, or migrate AURA code during this baseline request.

Expected response: accepted/rejected/needs_change, reason, reviewed source revision, changed selected files, consumer/test drift, constraints, and a result commit only if the recipient independently chose to create one.

## Later adoption request — activate only after Math M1 qualification

Package/version/hash/feed: **TO PROVIDE AFTER M1 QUALIFICATION**.
Parity and isolated-consumer evidence: **TO PROVIDE AFTER M1 QUALIFICATION**.

At that later stage, AURA may plan a cutover to qualified Math packages while preserving facades, configuration, errors, workers, budgets, culture, persisted data, and numeric exactness. The exact adoption request will be refreshed against the then-current AURA baseline.

No later adoption state is implied by this draft.

## Rollback

Baseline verification performs no mutation, so no rollback is needed. A future adoption must preserve the prior AURA source/reference baseline and define rollback before redundant source is removed.
