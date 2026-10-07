# Framing and request coverage

Backlog projection. Coverage means planned responsibility, not acquired qualification.

| Source | Need | Chunks |
|---|---|---|
| SEED-EXACT | Exact arithmetic, numbers, and rounding | M1-W01-C01, M1-W01-C02, M6-W02-C01 |
| SEED-UNITS | Quantities, units, and dimensions | M1-W01-C03, M1-W01-C04, M1-W02-C01 |
| SEED-EXPRESSION | Existing grammar and selection | M1-W02-C02, M1-W02-C03 |
| SEED-IR | IR, domains, assumptions, sets, equations, and functions | M2-W01-C01, M2-W01-C02, M2-W01-C03, M4-W02-C02 |
| SEED-COMPOSE | Direct calls, fluent APIs, pipelines, and graph | M2-W02-C01, M2-W02-C02, M2-W02-C03, M7-W01-C03 |
| SEED-NUMERIC | Linear algebra, matrices, vectors, and tensors | M3-W01-C01, M3-W01-C02, M3-W01-C03, M7-W01-C02 |
| SEED-STATS | Statistics, probability, regression, distributions, and randomness | M3-W02-C01, M3-W02-C02, M3-W02-C03, M6-W01-C01 |
| SEED-ANALYSIS | Calculus, roots, interpolation, and numerical methods | M6-W01-C01, M6-W01-C02, M6-W01-C03 |
| SEED-SYMBOLIC | Simplification, algebra, factorization, differential/integral calculus, and solving | M4-W01-C01, M4-W01-C02, M4-W01-C03, M4-W02-C01, M4-W02-C02, M4-W02-C03 |
| SEED-FORMATS | LaTeX, MathML, Markdown, Unicode, Office, text, and JSON | M5-W01-C01, M5-W01-C02, M5-W01-C03, M5-W01-C04, M2-W01-C03 |
| SEED-SCIENTIFIC | Geometry, trigonometry, combinatorics, sequences, signals, time series, finance, and special functions | M6-W02-C01, M6-W02-C02, M6-W02-C03, M6-W02-C04 |
| SEED-PROOF | Derivations, equivalence, independent verification, and formal proof | M4-W01-C03, M7-W02-C01 |
| SEED-ADVANCED | Arbitrary precision, intervals, and mathematical uncertainty | M7-W01-C01 |
| SEED-PLOT | PlotModel and graphical representation | M7-W01-C03 |
| SEED-PROVIDER | Reuse, licenses, independence, and portability | M0-W02-C01, M3-W01-C01, M4-W01-C01, M5-W02-C02 |
| SEED-V1 | Ten V1 framing criteria and consumption without AURA | M5-W02-C01, M5-W02-C02, M5-W02-C03 |
| USER-EXTRACT | Extract existing AURA behavior before extension | M0-W02-C01, M0-W02-C02, M1-W03-C01, M1-W03-C02 |
| USER-DWF | Complete hierarchy, .NET validation, and observational preparation | M0-W01-C01, M0-W01-C02 |
| USER-BOUNDARY | Cross-agent prompts, no cross-repository writes | M0-W02-C03, M1-W03-C02, M3-W02-C04, M6-W02-C05 |
| USER-CONSUMERS | Transitive nutrition/data/Decision/MCDM consumers | M1-W03-C02, M3-W02-C04 |

## V1 framing criteria

1. Parse notation: M5-W01-C01/C02.
2. Validate domains/assumptions: M2-W01-C02/C03.
3. Numerical and symbolic computation: M3 and M4.
4. Compose: M2-W02-C02.
5. Reject incompatible composition: M2-W02-C02.
6. Preserve exclusions: M4-W01-C02/C03.
7. Render notation: M5-W01-C01/C03/C04.
8. Results/errors/provenance: M2-W02-C01.
9. Qualified replay: M2-W02-C03.
10. Client without AURA: M1-W03-C01 and M5-W02-C01.

Gate M5-W02-C03: confront these criteria with real evidence.

## Boundaries and exclusions

- **DIS-AURA — Host resources, capabilities and supervision** — externally_owned, owner: AURA: Not mathematical primitives; integration via request. Review trigger: Math package ready.
- **DIS-DECISION — LP/MIP, CP-SAT, SMT decision problems, risk policies, EVPI, MDP and games** — externally_owned, owner: Decision: Retain decision semantics and stable API. Review trigger: Proven reusable primitive.
- **DIS-MCDM — Preferences, PROMETHEE, ELECTRE, ORESTE, AHP, TOPSIS, SMAA and ROR** — externally_owned, owner: MCDM: Retain multicriteria semantics; no automatic extraction. Review trigger: Proven numerical duplication.
- **DIS-FORMAL — Home-grown universal theorem prover** — excluded, owner: Math: Integrate qualified systems instead. Review trigger: Bounded formal interoperability need.
- **DIS-UNBOUNDED — Universal natural-language parser, arbitrary macros and unlimited engine** — excluded, owner: Math: Ambiguity and resource guarantees require declared subset. Review trigger: Explicit scope decision.
- **DIS-DOMAIN — Nutrition, record aggregation, storage, localization framework, market data** — externally_owned, owner: Respective domain maintainers: Only shared arithmetic primitive is a Math candidate. Review trigger: Consumer adoption.
