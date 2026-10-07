# Scientific qualification and delivery

## Evidence families

CONTRACT: types, statuses, serialization, provenance, and identity.
EXACT: independent exact rational/analytic results, dimensions, and temperatures.
BOUNDARY: invalid input, limits, allocation, cancellation, ambiguity, and overflow.
NUMERIC: residual, absolute/relative error, conditioning, singular and non-finite cases.
SYMBOLIC: restrictions, domains, branches, capture-free substitution; point tests alone are insufficient proof.
INTEROP: published subset, semantic round-trip, and declared presentation loss.
PACKAGE: isolated nupkg consumer, transitive dependencies, no sibling references.
REPLAY: pinned versions/policies/seed; bit-for-bit only over a qualified scope.
PLANNING: ID integrity, graph, coverage, states, projections, and DWF mapping.

## Extraction corpus

Import useful dw.quantities.tests cases with provenance and isolate mathematical expectations from CoreMath*Tests. Keep worker, policy, help, configuration, and envelope tests in AURA.
Add a small independent corpus: 1/3+1/6=1/2; 1 inch=127/5000 m; 0 degC=27315/100 K; 32 degF=0 degC; 9 degF interval=5 K; 1 KiB=1024 B.
Use normalized comparison (exact result + semantics + error), not byte-identical localized messages by default.
Inventory possible defects instead of freezing them blindly as compatibility: default rational, direct-primitive bounds, exponents, and temperature outside the parser.

## Oracles

Calling the same implementation twice is not an oracle. Prefer hand-computable exact results, independent invariants, residuals, and licensed published reference sets.
Use a distinct provider to corroborate computation when relevant, without treating it as automatic truth.
Matrices: A*x-b and reconstruction; compare eigenvalues through invariants/residuals, not arbitrary order/sign.
Statistics: pin population/sample, quantile, missing-value, weight, and normalization conventions.
Symbolic: verify assumption scope structurally; a CAS returning an expression without guarantees does not receive invented proof.
Randomness: PRNG golden vectors, draw counts, deterministic tests; statistical tests alone do not block on random fluctuation.
Performance uses resource limits and reproducible measurements, not fragile absolute CI-time thresholds.

## Gates

Each release owns the complete list of chunks and prior gates. Done requires all tasks/subtasks and evidence plus clean gate results.
M1 qualifies transfer into Math and an isolated consumer; real AURA adoption remains a distinct external state.
M5 / 1.0 requires the full numerical + symbolic + parsing/rendering vertical, preserved restrictions, invalid composition rejection, and an AURA-free client.
An unexecuted platform remains unqualified. Artifacts record SDK, OS/architecture, provider, and scope. Windows/Linux/macOS are targets, not anticipated successes.
Release tests cover only admitted capabilities. Unfinished M6/M7 research and external integrations are not disguised as 1.0 promises.

## Durable evidence

A chunk evidence file under docs/planning/evidence/<chunk-id>.json records id, source baseline, exact commands, working directory, configuration/SDK, scopes, exit code, pass/fail/skip counts, artifact SHA-256 values, oracle, limits, and decision.
Do not predict the result commit SHA in advance: reference baseline and artifacts; DWF supplies the result commit in its receipt.
Evidence paths used by the backlog are existing local paths. A report may reference a DWF receipt and an immutable external commit.
The validator checks structural presence/coherence, not scientific truth. A reviewer must read evidence before done.
