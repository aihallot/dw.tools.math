# M0-W02-C01 — source provenance and redistribution rights

## Status

**Satisfied by owner attestation.**

The human owner has expressly authorized `dw.tools.math` to copy, modify, package, and redistribute the selected mathematical AURA source covered by `docs/planning/source-baseline.json` at AURA revision `82a6b435a387a7e116b47a6b2c433ae9e067bf21`.

The durable attestation is:

- `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`

## Ownership and license model

The owner states that the selected AURA code belongs to the owner and that no separate third-party license requirement is being imposed for this transfer decision.

This is recorded as **owner-authorized proprietary code**, not as an invented open-source license. Ownership is retained while Math is expressly authorized to:

- copy the selected source into `dw.tools.math`;
- modify it;
- package it;
- redistribute it.

The owner requires no separate license file or notice for this inherited code under this decision. If third-party material is later discovered, its own terms must still be qualified independently.

## Scope

Authorization applies only to material that:

1. concerns mathematical functionality;
2. is traced to the pinned source manifest;
3. remains within the Math ownership boundary.

There are no additional owner exclusions. Existing project exclusions still apply to non-mathematical AURA material such as secrets, private data, host/worker policies, authorization logic, and unrelated domains.

## Provenance condition

The source manifest is still a byte-level baseline, not a clean-tree claim. Any selected source whose bytes differ from the pinned snapshot must be reconciled before transfer.

## Result

`EXT-SOURCE-RIGHTS` is satisfied.
`M0-W02-C01-T1-C`, `M0-W02-C01-T1`, and `M0-W02-C01` may close.
This decision authorizes transfer; it does not itself perform source extraction or external AURA mutation.
