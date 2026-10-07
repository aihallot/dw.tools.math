# Architecture and contracts

## Guiding choice

Three approaches were considered: wholesale movement of core.math (mixing policy and computation, high cross-cutting risk), provider-first rewrite (possible loss of exactness and behavior), and progressive extraction of existing libraries (selected).
The third preserves prior investment and delivers testable packages early. It accepts declared temporary duplication until consumers cut over, never two permanent owners.

## Layers

1. Exact primitives and quantities: rationals, exact conversions, dimensions, and temperatures.
2. Catalogs and bounded grammar: deterministic units and aliases, interpretation without code execution.
3. Math IR: typed objects and operations, domains, assumptions, provenance.
4. Adapters: optional numerical/symbolic families, with no provider types in consumer contracts.
5. Composition and codecs: validation, bounded execution, exchange, and rendering.
6. Independent consumers: .NET API and console sample; AURA provides its own exposure.

Layer 1 works without symbolic IR, without AURA, and without a heavy provider. Future layers must not make that foundation depend on them.

## Packages and transfer

At the first transfer, propose keeping dw.quantities, dw.quantities.expression, and dw.quantities.standard together with their namespaces so ownership transfer stays separate from model evolution. These are proposed identities, not already-published packages. Target sources live under src/projects/<package>/ and tests under tests/projects/<package>.tests/.
Future dw.tools.math.* families are created only with proven behavior and consumer evidence. Do not create dozens of empty projects.

The standard catalog currently depends on dw.localization. M0-W02-C02 compares use of an authorized shared package with replacing presentation dependencies by passive language metadata. Recommendation: separate unit data from AURA presentation rather than moving all localization into Math. The exact decision blocks catalog transfer.
No AURA redistribution-license proof exists in the observed root inventory: owner attestation and source notices precede any distributable copy.

## Values and semantics

A canonical rational has a positive denominator; zero is valid; policy for default structs and uninitialized input is explicit. BigInteger does not imply unlimited budget. Bound sizes, powers, allocations, and intermediate products.
ExactBinaryNumber preserves the received IEEE 754 binary value: FromDouble(0.1) is not the decimal rational 1/10. Never confuse decimal transport with binary value.
The inherited DimensionVector has seven SI dimensions plus Information. Exponent overflow policy and any extension to additional dimensions are explicit.
Absolute temperature and interval are distinct. The difference of absolutes is an interval; adding two absolutes or taking abs of an absolute is not justified by simple dimensional equality.
Exact comparison is the default; approximation uses inclusive symmetric |a-b| <= max(atol, rtol*max(|a|,|b|)). Tolerances are non-negative, dimensions compatible, rtol dimensionless; dimensionless zero may represent zero atol. Absolute temperatures compare on Kelvin.
Rounding policy is explicit and display-only; no rounded string decides equality, ordering, or admissibility.

## IR

Study OpenMath, Content MathML, and provider ASTs before the ADR. Minimal IR: scalars, bound symbols, operations, relations, functions, finite collections, units, domains, and assumptions. Plan schema identities and versions without inventing a universal language.
Mathematical parsing is separate from evaluation; the inherited exact grammar is not retroactively qualified as a symbolic CAS.
Preserve exclusions: (x²-1)/(x-1) may become x+1 only with x != 1. Over reals sqrt(x²)=|x|, not x without x >= 0. Substitution is capture-free, exact numbers remain distinct from approximations, and a complex principal branch is declared.
Use a deterministic JSON codec over the admitted subset; bound depth/nodes/text and reject unrecognized fields/versions. Semantic hash is separate from presentation hash; do not promise general mathematical equivalence.

## Results and providers

Distinguish exact success, approximation, conditional result, invalid input, incompatible domain, unsupported, no solution, unknown, cancellation, budget exhaustion, and provider failure. A partial result carries its scope; it is not complete success.
Provenance includes operation/version, provider/version, effective parameters, domain, assumptions, precision/tolerance, guarantee, and useful reproducibility data.
Do not force every field onto an exact addition; use lightweight common envelopes plus operation-specific results.
Capabilities are per operation, input/output types, admitted domains, numerical policies, limits, platform, and proof; discovered support is not AURA permission.
No silent fallback exact -> double or provider A -> B. Caller policy chooses; every approximation or change is visible.
A CancellationToken cannot forcibly interrupt a native engine. Qualify cooperation or use a bounded external host; AURA remains responsible for supervision and authorization.

## Composition

Direct and fluent APIs use the same contracts. Mechanical validation occurs before execution: types, dimensions, domains, matrix shapes, and assumptions. A numerically valid composition is not proof of symbolic equivalence.
The first pipeline is finite and sequential. An optional local DAG may be admitted in M7 after demonstrated need: bounded cache, acyclicity, provenance, and partial nodes. No distributed scheduler, secrets, or AURA resources belong in Math.

## Boundaries and compatibility

Descriptive statistics, decompositions, distributions, and generic rounding belong to Math.
LP/MIP, CP-SAT, decision SMT, EVPI, MDP, risk policies, and alternative optimization belong to Decision.
Criterion-orientation normalization, preference thresholds, PROMETHEE, and flow quantization belong to MCDM.
Nutrition recipes, USDA databases, record aggregation, and SQLite storage remain with their owners.
Math may offer primitives; the presence of multiplication alone never triggers ownership transfer.
Before 1.0, Math may evolve through ADR and targeted migration. Decision 1.x APIs and externally persisted data retain their guarantees until an owner decision changes them.
