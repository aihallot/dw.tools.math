# Initial inventory — 2026-10-07

This is an observation of working files, not an exhaustive correctness audit and not proof that source repositories were clean.
[source-baseline.json](source-baseline.json) pins 54 selected files and their SHA-256 values together with observed HEADs. A local modification is characterized by its hash, not automatically attributed to HEAD.
No source is copied at this stage. M0 revalidates sources before transfer and requests a controlled export from the owning agent when DWF cannot access the source repository.

## AURA: priority transfer

Observed root: D:/data.dev/gitlab/AURA_projects/aura.

| Observed sources | Proposed destination/treatment | Chunks |
|---|---|---|
| lib/dw.quantities/ExactRational.cs | Standalone dw.quantities package, initial identity preserved | M1-W01-C01 |
| ExactBinaryNumber.cs, ExactDecimalFormatter.cs in the same folder | Exact binary64 conversion and controlled formatting | M1-W01-C02 |
| DimensionVector.cs, Quantity.cs, UnitDefinition.cs | Dimensions, quantities, and unit transformations | M1-W01-C03 |
| ApproximateEquivalence.cs | Explicit-tolerance comparison | M1-W01-C04 |
| lib/dw.quantities.standard/StandardUnitCatalog.cs, StandardExpressionUnitResolver.cs | Versioned catalog and independent resolution | M1-W02-C01 |
| lib/dw.quantities.expression/ExpressionParser.cs | Exact grammar and bounded selection functions | M1-W02-C02/C03 |
| tests/dw.quantities.tests/: ExtractedKernel, ExactFraction, ExpressionEngine, ApproximateEquivalence, StandardUnitCatalog, StandardExpressionUnitResolver | Math-importable tests after classification; preserve provenance | M1 and M1-W03-C01 |

The core uses BigInteger; the parser already has bounds (4096 characters, 256 tokens, depth 32, 256 digits, power/root 16). These limits do not prove that every direct primitive API is bounded; that remains a qualification recipe.

Observed behavior includes canonical rationals, fractions/mixed numbers, SI/imperial/US/culinary conversions, information units, affine temperature, abs/min/max, and comparison. No general CAS or universal floating engine is inferred.

## AURA: separation to decide

| Files | Decision |
|---|---|
| src/aura.domains/core/aura.domains.core.math/UnitConverter.cs | source.ToBase then target.FromBase is a candidate pure API; adapt errors on the AURA side. Verify temperature beyond dimensional equality. |
| Domain ExpressionParser.cs | Facade over dw.quantities.expression and the AURA resolver: retain/adapt the facade, the engine is already separate. |
| UnitCatalog.cs, MathCultureResolver.cs, UnitLocalization.cs, UnitProjection.cs | Classify generic data/resolution versus AURA projection/presentation; do not move wholesale. |
| lib/dw.quantities.standard/dw.quantities.standard.csproj | Depends on dw.localization: ADR required before extraction; never depend on the AURA checkout. |
| lib/dw.quantities/ExactRational.cs and DimensionVector.cs | Characterize default struct, invalid values, intermediate size, and exponent overflow; do not silently repair during transfer. |

## What remains in AURA

CalculateAction/Contracts, ConvertAction/Contracts, CompareAction/Contracts, UnitListAction, UnitInspectAction, CoreMathDomainDeclaration, host configuration/defaults, config/worker-policies/core.math.*, embedded documentation/skill, workers, authorization, and envelopes.
tests/aura.tests.conformance/CoreMath* and tests/aura.tests.integration/CoreMath* are reservoirs of mathematical cases; CLI/worker scenarios remain in AURA.

## Transitive consumers not to forget

- src/aura.kernel/aura.kernel.csproj references expression and standard.
- Nutrition domain references expression, standard, and localization.
- lib/dw.nutrition references dw.quantities; recipes, food references, and business calculations remain nutrition.
- lib/dw.data.transforms references dw.quantities; RecordTransformer and RecordAggregator own records/aggregation, not Math.
- lib/dw.data.sqlite/SqliteTableReadContracts.cs constructs rationals; preserve INTEGER/REAL paths.
- lib/aura.json/JsonTransformProcessor.cs and JsonAggregationRecords.cs: exact-number parsing/projection, exponent guards, and decimal/binary distinction.
- dw.nutrition.sqlite/usda and other transitive clients: verify serialization, persisted databases, and artifacts during AURA adoption.

The AURA agent will replay targeted searches by project references and symbols at cutover revision; this inventory does not authorize deleting every occurrence.

## Decision: candidates for sharing, not immediate transfer

Observed root: D:/data.dev/github/ai@hallot.net/dw.tools.decision.

- src/projects/dw.tools.decision.constraints/ExactDecimalScaler.cs: exact decimal -> Int64 scaling with loss/overflow refusal. Potential generic primitive; CP-SAT exceptions and constraints remain Decision.
- src/projects/dw.tools.decision.simulation/Simulation.cs: deterministic PRNG, Bernoulli/uniform sampling, simulation calculations. Separate random stream/distribution from decision Monte Carlo results/objectives; preserve versioned sequence.
- src/projects/dw.tools.decision.uncertainty/RiskAndRobustness.cs: generic quantiles/cumulative operations to study; VaR/CVaR, robustness, and policies remain Decision-owned.
- Do not move decision graphs, LP/MIP, CP-SAT, SMT, routing, EVPI, MDP, or games. The presence of mathematics is not enough to change ownership.

## MCDM: narrow boundaries

Observed root: D:/data.dev/github/ai@hallot.net/dw.tools.mcdm.

- lib/dw.tools.mcdm.core/Numerics/CompensatedSum.cs: concrete numerical candidate.
- lib/dw.tools.mcdm.core/Numerics/FlowQuantization.cs: flow-related quantization; preserve multicriteria convention.
- lib/dw.tools.mcdm.core/Relations/SquareMatrix.cs: relation-specific structure; compare against a generic-matrix need before proposing replacement.
- lib/dw.tools.mcdm.application/Acquisition/Normalization.cs: criterion orientation/transformation remains MCDM even if primitives become Math.
- PrometheeEngine, PreferenceFunction/Specification, and ranking results remain MCDM.

## Limits

No product test was run for this inventory. Licenses and anomalies are gates to investigate, not acquired qualifications. Repositories continue to evolve: hashes are a dated starting point, never authorization to overwrite later work.
