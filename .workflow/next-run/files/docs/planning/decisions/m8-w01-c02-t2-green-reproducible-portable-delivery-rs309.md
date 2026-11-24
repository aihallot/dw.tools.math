# RS309 — Activate portable Discovery delivery GREEN with reproducible independent exports

## Authoritative prior result

RS308 succeeded first-pass at attempt 001, closing C02/T2 RED from RS307's public opt-in delivery and refusal probes and admitting C02/T2 GREEN ready. The deterministic skill-generator T1 is already done. Do not add another exporter, verifier, package, or execution authorization.

## New bounded GREEN evidence

Use the existing `AttestM8PortableDiscoveryExport.cs --export` CLI in two distinct child processes with separate fresh absolute destinations. Assert each contains exactly fourteen files and compare the sorted relative path set and SHA-256 contents byte-for-byte; inspect the generated receipts to confirm thirteen recorded hashes, `signatureStatus=unsigned`, `authorization=runtime-only`, and exact four selected runtime-only capabilities. Start a copied `verify-export.cs` from a separate working directory, verifying both independently generated destinations. Copy a complete export to an independent consumer, reject a mutated copied skill through the same verifier, restore pristine bytes, and verify again. Protect original Math source and package boundaries.

## Exact native movement

Activate only `M8-W01-C02-T2-G` from ready to in-progress, preserving RED done, Verification not-ready, parent T2/C02/W01/M8 in-progress, C01 and C02/T1 done, W02/M9 not-ready. No completion achievement is claimed by this run. This qualifies reproducible local delivery integrity only: the receipt remains unsigned and does not certify external installation, public release, AURA adoption or broker grant.
