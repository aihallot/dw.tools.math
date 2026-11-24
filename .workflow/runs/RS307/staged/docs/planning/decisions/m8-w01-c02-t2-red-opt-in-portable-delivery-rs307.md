# RS307 — Qualify portable skill opt-in delivery RED boundaries

## Prerequisite

RS306 succeeded first-pass in allocated attempt 001, completed copied-skill Verification and generator contract T1 with distinct native achievements, and admitted C02 delivery/refusal T2 plus RED ready. Delivery scope is Math-owned opt-in portable export; source metadata does not grant broker execution rights.

## Fresh independent RED test

Reuse the existing `AttestM8PortableDiscoveryExport.cs` public `--export` and `--verify` functions rather than a second delivery implementation. Freshly export an unsigned fourteen-file bundle to an unoccupied absolute destination. Launch the copied `verify-export.cs` script from a separate clean directory to validate its thirteen recorded SHA-256 file hashes and exact runtime-only receipt. Require an opt-in request to an already occupied destination to fail without modifying a sentinel. In an isolated copied bundle, require the copied verifier to reject a modified skill file, an unexpected extra file, and a receipt with forged `authorization=granted`. Restore each case and requalify the clean bundle, checking source bundle bytes did not change.

## Lifecycle and limits

RS307 activates C02/T2 and T2-RED to `in-progress` only. It does not complete RED or activate GREEN. T1 and C01 remain done; C02, W01 and M8 in-progress, M8-W02 and M9 not-ready. The receipt is deliberately unsigned; this test cannot establish signed provenance, third-party agent-host installation, runtime broker permission, or public package publication. Product backlog 0.1.124 remains planned.
