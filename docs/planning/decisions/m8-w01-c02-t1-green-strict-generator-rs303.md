# RS303 — Qualify deterministic skill GREEN with existing strict CLR admission

## Input authority

RS302 durably succeeded at attempt 001 with zero prior failures, one RED completion and C02/T1 GREEN ready. RS301's isolated RED test already demonstrated stale skill, stale bundled manifest and unauthorized execution grant refusals; do not replace the generator or duplicate the existing strict CLR validation responsibility.

## Bounded GREEN change

Update only `GenerateM8MathSkill.cs` to invoke the existing Math-owned `ValidateM8StrictBindingMetadata.cs --validate` on the canonical source manifest before writing skill or bundled manifest. The strict validator reconstructs CLR signatures from ordered types and members, checks their SHA-256 fingerprints and refuses invented execution grants. The generator must fail closed without changing outputs on invalid source metadata. Its normal deterministic text formatting remains unchanged.

A new Math-owned isolated GREEN consumer invokes the public `--write` and `--check` modes in two separate disposable roots. Both independently produced skill and manifest bytes must agree, and strict gate source fingerprints must match. Fresh, disposable fingerprint and parameter-type mutations must be refused before output writes; pristine outputs and sources are restored and rechecked. The preexisting RS301 RED probe now copies the strict validator script so its existing isolated regression checks remain meaningful.

## DWF progress

Activate only `M8-W01-C02-T1-G` from ready to in-progress. RED remains done; Verification, T2 and later branches stay not-ready. Do not claim GREEN completion, native task completion, external host installation, signed origin, runtime broker authorization, AURA integration or package publication. Product backlog 0.1.124 remains planned.
