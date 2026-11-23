# RS301 — Start deterministic skill-generator RED with isolated drift falsification

## State basis

RS300 succeeded first-pass, completing C01 with qualified protocol and CLR evidence, while admitting C02 to ready. The accepted C02 contract requires deterministic, opt-in skill/manifest generation, byte identity, exact selected CLR fingerprints, examples and drift refusal. Its metadata is never authority to execute a host action.

## New bounded RED evidence

Reuse the existing Math-owned `GenerateM8MathSkill.cs --check` rather than authoring a second generator. Copy the generator script, pinned source manifest, generated skill and manifest into a disposable isolated workspace. Require a pristine check to pass, then independently alter the copied skill file and copied bundled manifest, requiring each drift to be refused. Separately forge an execution grant in the isolated source manifest and require the generator's existing runtime-only selection guard to refuse it. Restore unchanged bytes and re-run clean validation. Source files, package archives and historical reports remain untouched.

## Native progression

The only DWF progression is `M8-W01-C02`, `M8-W01-C02-T1` and `M8-W01-C02-T1-R` activated to in-progress with no completion achievement. All T1 GREEN and Verification, T2 and later phases remain not-ready. C01 is done, W01 and M8 in-progress, backlog 0.1.124 planned. Later GREEN will require fresh implementation evidence, not this RED rehearsal. No external host, signed provenance or broker permission is established.
