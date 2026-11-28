# Math 2.0 — implementation roadmap and admission map

Status: owner-requested prospective planning; **not yet admitted to canonical backlog or native execution authority** until a controlled DWF admission succeeds. No implementation, completion, performance claim, publication or adoption is implied.

Sources: [verbatim RS429 audit](audit-original.fr.md) and [owner's automatic hierarchical Discovery extension](discovery-generation.md). The [append-only admission input](roadmap-admission.json) contains the proposed stable IDs, dependency edges, bounded acceptance, source coverage and native projection; it is not a second editable backlog. Once admitted, docs/planning/backlog.json is the sole product roadmap and .aura/workflow/plan/project.json is its executable projection.

Baseline: RS429, a9547114a0ff0fc6ef6d0667bcc19f4079188ed8, canonical 0.1.196, 636 completed canonical and 630 completed native nodes; six historical native omissions preserved. Target admission version 0.1.197, aggregate in_progress. All new nodes planned/not-ready, no new achievements or fake run history. Version labels are indicative functional checkpoints, not dates or NuGet versions.

## Program

| Milestone | Target | Work packages | Phases | Depends on |
|---|---|---:|---:|---|
| M12 — Measured modernization and technical consolidation | 1.7.0-preview | 2 | 7 | M10, M11 |
| M13 — Agent-native hierarchical Discovery | 1.8.0-preview | 2 | 9 | M12 |
| M14 — Temporal computation, calendars and academic years | 1.9.0-preview | 2 | 8 | M13 |
| M15 — Statically typed quantities and extensible unit catalog | 1.10.0-preview | 1 | 4 | M13 |
| M16 — Typed scientific laws and metrology | 1.11.0-preview | 2 | 8 | M15 |
| M17 — Advanced statistics, probabilities and time series | 1.12.0-preview | 2 | 7 | M14, M13 |
| M18 — Advanced numerics, tensors and precision | 1.13.0-preview | 1 | 5 | M12, M15, M17 |
| M19 — Composed calculation, autodiff and mathematical representation | 1.14.0-preview | 2 | 6 | M18 |
| M20 — Math 2.0 integrated qualification and distribution | 2.0.0 | 1 | 5 | M13, M14, M15, M16, M17, M18, M19 |

Proposed extension: 319 canonical/native nodes, 59 bounded phases and requirements; 955 canonical / 949 native nodes after admission. Counts are planning granularity, not promised run counts.

## Execution discipline

Refine one eligible bounded leaf at a time with actual existing implementation and test inventory, precise mutation paths, oracle, cost and rollback. No speculative projects or duplicate engines. Every new domain must classify public APIs, qualify admitted agent-callable semantics and regenerate/check Discovery. Historical gates stay done; no weakening old oracles. In particular, scope-aware validators must distinguish historical completed M0–M11 from new planned M12–M20 while validating full hierarchy parity and preserving six historical omissions. First execute an admission-only controlled run patterned on RS367/RS370. Then prepare M12 baseline inventory; no successor before durable success. No source implementation or NuGet publication in admission. All domain candidates use explicit finite boundaries and provider admission; later refinements may split phases without altering the accepted intent. Actual package/project names and version labels are subject to qualification. External policy, host installation and BrokerReady remain outside Math's authority.

## Phase and acceptance map

### M12 — Measured modernization and technical consolidation

Establish a measured net11/C# 15 baseline and deliver evidence-driven additive modernization without semantic regressions.

#### M12-W01 — Engineering baseline and compatibility

- **M12-W01-C01 — Current source, API and evidence inventory.** Pin source and package inventory, all 13 libraries, 24 maintained projects, historical consumers and existing API/qualification owners; distinguish current facts from RS429 audit claims. **Acceptance:** Reconcile public API and project/dependency roster to pinned assemblies and source; identify existing reusable tools, stale documentation and live vs historical validators; no new test/coverage claims without evidence.
- **M12-W01-C02 — Quality and benchmark laboratory.** Create an isolated C# benchmark/quality harness after assessing existing tools: timing, allocations, memory, startup, coverage and reproducibility; select critical exact and approximate property, differential and mutation tests. **Acceptance:** Run controlled baseline on identified SDK/RID/CPU with bounded datasets and repeatability; report uncertainty and actual coverage/branch evidence; demonstrate a deliberately failing oracle and no change to runtime packages.
- **M12-W01-C03 — Compatibility, budgets and documentation consolidation.** Define shared but operation-specific numerical budgets, refusal and precision vocabulary; update stale operational net10 instructions; qualify stable business-facing names with legacy aliases and consolidate qualification reuse. **Acceptance:** Historical API and exact golden fingerprints unchanged; explicit approximate error budgets, negative limit/overflow cases, public API diff and scoped documentation/qualification consistency; no global loosened bounds.

#### M12-W02 — Evidence-driven modernization

- **M12-W02-C01 — Exact rational and parsing hot paths.** Measure BigInteger reduction-before-multiplication, comparison, long parsing, conversions and Span/TryParsePartial options; reuse RS343 and preserve exact binary64 error accounting. **Acceptance:** Independent rational and round-trip oracles, pathological length and allocation benchmarks, exact legacy error/replay equality; admit only justified changes.
- **M12-W02-C02 — Numerical and statistical kernels.** Measure selection vs sorting for isolated/multiple quantiles, contiguous storage, Span, Generic Math, SIMD/TensorPrimitives, moments merging; choose scalar reference paths and preserve conventions. **Acceptance:** Test exact/approximate parity, signed zero, NaN/refusal, cancellation and floating order policies across representative sizes; compare throughput and allocation to baseline.
- **M12-W02-C03 — IR, codecs and language modernization.** Assess actual SDK11/C#15 availability and maturity of union/closed hierarchies, decimal and complex representations, source-generated JSON, IEEE representation, new random APIs; apply only justified changes or explicit deferrals. **Acceptance:** Existing math-ir/1 canonical bytes, hashes, SplitMix64 vectors, serialization and public compatibility unchanged; independent version-pinned experiments for unsupported or speculative APIs.
- **M12-W02-C04 — AOT, trimming and modernized baseline gate.** Qualify selected trimming/Native AOT consumers and actual supported .NET 11 build, record performance changes, compatibility, remaining research and package implications. **Acceptance:** Focused and historical fragile regressions precede broad boundary validation; compare benchmark evidence and source/package identity; no untested-platform or unsupported-feature claim.

### M13 — Agent-native hierarchical Discovery

Generate a complete admitted, compact, deterministic hierarchical Math skill from safe introspection, explicit semantics and resources; qualify invocation surfaces independently.

#### M13-W01 — Discovery contracts and inventory

- **M13-W01-C01 — M8 reuse and protocol authority.** Inventory current M8 generator, protocol, four capability IDs, binder/catalog/exporter and existing upstream discovery components; request owning AURA/dw.discovery contract alignment; pin schema and conformance fixtures. **Acceptance:** Compare actual sources and immutable package metadata, prove existing IDs and historical bundles unchanged; record owner boundary, version decision and unresolved downstream facts.
- **M13-W01-C02 — Safe full API introspection.** Implement reusable build-time metadata inspection across selected source/assembly/package API surfaces; include overloads, instances, generics, nullability and signatures; no arbitrary assembly execution or ambient loading. **Acceptance:** Independent assembly roster and synthetic fixture with overloaded, inherited, unsupported and malformed cases; exact classification completeness, deterministic order and source/binary drift refusals.
- **M13-W01-C03 — Semantic attributes and normalized registry.** Define explicit opt-in capability IDs, stable taxonomy, types/units/domains, assumptions, precision, effects, budgets, error/cancellation, version/evidence and resource links. Reuse common attributes where qualified. **Acceptance:** Positive and negative descriptor fixtures, duplicate ID/ambiguity/refusal tests, explicit documented-only/excluded/pending categories and stable IDs; no inferred permissions.
- **M13-W01-C04 — Schema and binding strategy.** Generate typed parameter/result schemas and selected CLR binding descriptors; qualify static, instance/facade and generic policy, exact rational/complex/numeric and structured DTO round trips. **Acceptance:** Positive and negative independent invocation oracles; refusal for unsupported type, overload, loss, drift and invalid arguments; no dynamic arbitrary method execution.

#### M13-W02 — Hierarchical generation and qualification

- **M13-W02-C01 — Hierarchical skill and resource generation.** Generate compact root SKILL.md, semantic domain/subdomain navigation, capability pages, indexes, resources links, manifest, examples and bindings from registry. Preserve existing four IDs and legacy M8 artifacts. **Acceptance:** Two independent fresh generations byte-identical; reordered source inputs unchanged; all admitted entries reachable exactly once, no duplicate IDs, path traversal, or orphan/stale outputs; check-only mode observational.
- **M13-W02-C02 — Skill selection and token benchmark.** Build versioned intent corpus and compare existing/flat/hierarchical/alternative layouts; measure top-k, selection/refusal, valid arguments, result success, median/p95 tokens, loaded pages and latency. **Acceptance:** Pin models/config/corpus; set acceptance thresholds from measured baseline, test ambiguity, exact vs approximate and cross-domain queries; no optimization by dropping mandatory contracts.
- **M13-W02-C03 — Coverage waves for existing Math APIs.** Classify all selected public API entries and admit qualified quantity/expression/IR/composition, numerics, statistics, analysis, symbolics, scientific, financial and codecs capabilities with typed examples and explicit exclusions. **Acceptance:** 100% inventory classified, 100% admitted entries reachable and schema/evidence complete; independent representative positive/negative source calls per family and exact identity checks.
- **M13-W02-C04 — Portable net11 package and host boundary.** Generate and validate portable bundle from actual pinned package bytes; qualify selected source-free Windows/Linux calls, trimming/AOT feasibility and drift/security checks; prepare independent AURA owner intake. **Acceptance:** Exact signatures/hashes and locked consumer calls; invalid/tampered fixture refusals; distinguish AgentCallable, BrokerReady, host installation, downstream adoption and publication.
- **M13-W02-C05 — Continuous generation gate.** Define a reusable generation/drift/semantic quality gate for every future capability family, with documentation, new API classification, examples and evidence. **Acceptance:** Demonstrate gate on added/removed/changed sample APIs; no manual derived page edits; generator output and admission policy are versioned; separate broker authorization.

### M14 — Temporal computation, calendars and academic years

Deliver provider-neutral explicit temporal semantics, representations, zone-aware calendars and configurable academic/fiscal/business policies.

#### M14-W01 — Temporal foundations and interop

- **M14-W01-C01 — Temporal provider and contract admission.** Compare existing Time dimension, DateOnly/TimeOnly/DateTimeOffset/TimeZoneInfo/TimeProvider and Noda Time; qualify candidate provider versions/licenses, ranges and time scales. **Acceptance:** Produce ADR and finite oracle corpus for instant vs duration vs period vs local/offset/zoned dates, invalid transitions and provider limits; no provider types leaking into public contracts.
- **M14-W01-C02 — Instants, duration, local/offset dates and intervals.** Implement bounded distinct temporal values and calendar periods with explicit interval boundaries, comparisons, arithmetic, age, leap years, months and weeks. **Acceptance:** Golden leap, month end, negative duration, exact interval intersection/union/difference, ISO week 53 and overflow examples; no implicit month-as-30-days or local-day-as-24h.
- **M14-W01-C03 — Representation conversions and resolution.** Qualify ISO8601/RFC3339, Unix seconds/ms/us/ns, CLR ticks, OLE, Excel 1900/1904, Julian/MJD, NTP and GPS/TAI/UTC through explicitly admitted scale-specific adapters. **Acceptance:** Independent epoch, resolution, rounding, Excel phantom day, range and leap-second fixtures; refuse unsupported scales and ambiguous precision; loss recorded.
- **M14-W01-C04 — Versioned time zones.** Support IANA/TZDB versioned rules, Windows mapping, offset and historical transitions with explicit ambiguous/nonexistent local time policy. **Acceptance:** Independent DST 23/25-hour day, folds/gaps, historical and fractional offsets, version-pinned replay, cross-platform behavior; no ambient host-zone dependency.

#### M14-W02 — Calendar policies and consumer use

- **M14-W02-C01 — Calendar systems and recurrence.** Qualify Gregorian, Julian, ISO and other explicitly admitted calendars, recurrence expansion with finite budgets and business-day calculation from caller data. **Acceptance:** Leap and end-of-month recurrence, exclusions, interval boundaries, unsupported calendar and unbounded recurrence refusal; no embedded administrative rules.
- **M14-W02-C02 — Academic and fiscal years.** Deliver policy-versioned arbitrary academic/fiscal boundaries, teaching periods, sessions, holidays, transitions, teaching-day/week counts and academic year labels. **Acceptance:** Independent synthetic policies with cross-year and nonstandard dates, overlap/gap/refusal, date-to-year and period lookup, teaching counts; no assumptions about an actual university or country.
- **M14-W02-C03 — Financial and time-series calendar bridge.** Integrate ACT/ACT, ACT/360, ACT/365, 30/360 and explicit calendar windows, resampling/aggregation with existing scientific engines. **Acceptance:** Independent day-count conventions, DST-aware windows and irregular dates; preserve old finance defaults, no new implicit conventions.
- **M14-W02-C04 — Temporal contract and Discovery gate.** Qualify package boundary, all admitted temporal APIs, provider version evidence, hierarchical resources and locked consumer examples. **Acceptance:** Run temporal oracle corpus and package-only probes; classify all temporal public APIs, prove generated skill drift-free and record deferred representations/calendars.

### M15 — Statically typed quantities and extensible unit catalog

Add semantic quantity kinds and compiler-checked physical quantities over the existing exact/dynamic core.

#### M15-W01 — Quantity types and units

- **M15-W01-C01 — Semantic model and source inventory.** Inventory existing exact Quantity/DimensionVector and catalogs (including electrochemistry); design distinct numeric value, dimension, quantity kind, unit and policy; compare provider/code generation choices and BIPM/UCUM provenance. **Acceptance:** ADR and finite catalog comparison; same-dimension non-equivalence (energy/torque, Hz/Bq, angle/scalar), absolute vs delta temperature and affine/log conversions explicitly accounted for.
- **M15-W01-C02 — Canonical catalog and conversion policies.** Extend qualified SI/base/derived, SI and binary prefixes, imperial, affine/log units with stable identifiers, provenance, versioned extensibility and conversion rules. **Acceptance:** Golden exact/approximate unit conversions, inverse/round-trip policy and illegal operations; preserve historical catalog identities; no double counting or invented definitions.
- **M15-W01-C03 — Static quantity types over dynamic core.** Deliver admitted typed value contracts and conversions reusing existing engine, with compile-time operator results and semantic kind checks; support exact and explicitly approximate numeric policy. **Acceptance:** Independent compile-positive and compile-negative fixtures, runtime bridge dimension/kind refusal, Ohm-law typed example, exact conversions, no duplicate mathematical implementation.
- **M15-W01-C04 — Typed generation, interoperability and catalog gate.** Assess generated strongly typed operators/contracts, stable IR/JSON and public API compatibility, Discovery metadata/resources, catalog and package-only evidence. **Acceptance:** Verify generated types and operators are deterministic, no ambiguity for equal dimensions, dynamic/static round-trip and loss policy, stable historical API and qualified selected consumers.

### M16 — Typed scientific laws and metrology

Deliver bounded, discoverable typed scientific laws with assumptions, units, domains, measurement policies and independent scientific oracles.

#### M16-W01 — Physical and measurement foundations

- **M16-W01-C01 — Uncertainty, constants and metrology foundations.** Qualify physical constants and provenance, measurement values/uncertainties, significant figures, absolute/relative errors and first-order propagation with covariance; distinguish interval and measurement claims. **Acceptance:** Known exact constants and independent propagation fixtures including correlated terms, rounding and invalid covariance; declared uncertainty is not numerical error.
- **M16-W01-C02 — Mechanics and fluids.** Deliver typed speed, acceleration, force, energy, momentum, pressure, density, flow and viscosity laws with explicit idealized assumptions and inverse domains. **Acceptance:** Dimensional/semantic compile-time and numeric positive/negative oracles, including zero divisions and out-of-domain inputs; no universal physical simulation.
- **M16-W01-C03 — Electricity, electronics and electromagnetism.** Deliver typed Ohm law and inverses, power, series/parallel resistance, capacitance, inductance, charge, field, flux, impedance and bounded RC/RLC models. **Acceptance:** Independent analytic circuit examples, DC/AC and ideal assumptions, zero/negative/domain refusals, compile-negative kind mismatches; expose qualified law IDs.
- **M16-W01-C04 — Thermodynamics, optics and acoustics.** Deliver bounded temperature, heat/energy/entropy, wavelength/frequency, refraction/lens and acoustic intensity/dB laws with explicit reference and domain policies. **Acceptance:** Independent physical-law and unit oracles, absolute/delta temperature, sign conventions, log references and unsupported regime refusals.

#### M16-W02 — Chemistry and scientific integration

- **M16-W02-C01 — Chemical quantities and solution calculations.** Deliver molar mass, amount, concentration, dilution and pH within explicit assumptions, with caller-supplied/versioned chemical data provenance. **Acceptance:** Mass/mole and concentration oracles, dilution conservation, pH-domain and nonideal-solution refusals; no chemical database ownership assumption.
- **M16-W02-C02 — Stoichiometry and conservation.** Deliver bounded chemical equation representation, balance/conservation, limiting reagent calculations with exact arithmetic and explicit unsupported chemistry. **Acceptance:** Atom and charge conservation, known balanced equations, underdetermined/inconsistent and budget refusals; no unsafe experimental or lab procedure claims.
- **M16-W02-C03 — Electrochemistry.** Extend existing electrochemical unit profile with typed Faraday and Nernst laws under declared conditions. **Acceptance:** Independent analytic fixtures, temperature/charge/stoichiometry units, logarithm domains and zero/equilibrium cases; preserve six historical unit definitions.
- **M16-W02-C04 — Scientific law registry and qualification.** Unify scientific law metadata (inputs, result, inverses, dimensions, assumptions, evidence) and generate Discovery resources; qualify admitted typed domains and consumer boundaries. **Acceptance:** Prove all exposed laws have positive/negative oracles, invalid types fail, package-only probes and no unauthorized external adoption; record research/deferrals by family.

### M17 — Advanced statistics, probabilities and time series

Extend reproducible statistics, finite stochastic primitives and calendar-aware signal/time-series operations without importing decision policy.

#### M17-W01 — Statistics and stochastic primitives

- **M17-W01-C01 — Distribution contracts and reproducible sampling.** Qualify normal, binomial, Poisson, exponential, Gamma and beta distribution functions and sampling with explicit tails/domain/budget and algorithm versioning; preserve SplitMix64 replay. **Acceptance:** Independent analytic/reference CDF/PDF/quantile and seeded sampling fixtures, support/refusal and tail/overflow tests, historical deterministic vectors unchanged.
- **M17-W01-C02 — Estimation, robust statistics and tests.** Deliver bounded likelihood, confidence intervals, robust median/MAD and admitted comparison, independence and goodness-of-fit tests with explicit assumptions. **Acceptance:** Known data/reference results, missing/degenerate input and small sample policies, error controls and refusal; no business decision interpretation.
- **M17-W01-C03 — Regression and resampling.** Deliver multiple, polynomial and logistic regression plus bootstrap/permutation primitives with deterministic replay and finite iteration/sample budgets. **Acceptance:** Synthetic known-coefficient/fit and resampling oracles, rank separation/convergence failures and numerical tolerances; no model-selection policy ownership.
- **M17-W01-C04 — Streaming and stochastic process extensions.** Deliver bounded approximate streaming quantiles, histograms, sliding windows, accumulator merge and generic finite Markov/process primitives. **Acceptance:** Exact/reference comparison with error guarantees, merge/window/replay tests, invalid probability and budget refusals; preserve M9/M11 contracts.

#### M17-W02 — Signals and irregular temporal data

- **M17-W02-C01 — FFT and signal processing.** Qualify FFT, convolution, digital filters, spectrum and autocorrelation; extend admitted sample sizes only from measured kernels. **Acceptance:** Compare against direct DFT and analytic fixtures across even/odd/edge lengths, inverse and normalization policies, measured memory/time and explicit bounds.
- **M17-W02-C02 — Irregular time series and seasonal analysis.** Deliver timestamped series, calendar-aware resampling, windows, aggregation, generic series distance and admitted seasonal decomposition. **Acceptance:** Independent irregular/DST/missing/duplicate/order cases and analytic series fixtures, deterministic time-zone version context and no hidden imputation.
- **M17-W02-C03 — Statistics and signals gate.** Qualify all admitted stochastic, statistical and time-series contracts, reproducibility, performance and generated Discovery; package-only consumer examples. **Acceptance:** Focused + historical M9/M11 oracles, exact/approximate differentiation, classified public APIs and documented provider/algorithm limits.

### M18 — Advanced numerics, tensors and precision

Qualify scalable numeric representations and bounded advanced linear algebra, tensors and interval/precision engines.

#### M18-W01 — Advanced numerical representation

- **M18-W01-C01 — Precision and provider admission.** Revisit M7 deferred advanced precision/tensors/intervals; qualify relevant .NET 11 decimal/complex/generic math/provider APIs, exact vs approximate and error semantics. **Acceptance:** Record actual availability, rights, version/ABI and independent reference cases; finite scope and explicit deferrals, no broad provider adoption.
- **M18-W01-C02 — Interval arithmetic and directed rounding.** Implement qualified interval operations, absolute/relative errors, variable precision and directed rounding with explicit containment and singular cases. **Acceptance:** Independent exact or high-precision enclosure oracles for endpoints and zero-crossing/refusals; distinguish measurement covariance from numerical enclosure.
- **M18-W01-C03 — Dense/sparse linear algebra.** Deliver bounded larger matrices, CSR/CSC sparse formats, linear solves, norms, eigenvalue and SVD contracts with measured budgets. **Acceptance:** Analytic matrices and independent provider/reference comparison, rank deficiency, ill-conditioning, residual and convergence oracles; no universal solver guarantee.
- **M18-W01-C04 — Tensor operations and accelerated kernels.** Deliver admitted multidimensional tensors, explicit broadcasting, shape/stride and vectorized operations; qualify existing primitives/providers rather than creating unbounded NumPy/GPU equivalents. **Acceptance:** Independent scalar reference, shape mismatch, aliasing/stride and budget tests, measured allocation and throughput; GPU only by separate scope admission.
- **M18-W01-C05 — Advanced numeric and precision gate.** Qualify new representations, tensor and matrix contracts, compatibility, typed quantity integration, numeric budgets and generated Discovery. **Acceptance:** Source/package-only probes, measured deltas, interval containment and precision/refusal evidence, no silent narrowing or publication.

### M19 — Composed calculation, autodiff and mathematical representation

Deliver bounded pure compute graphs, differentiable operations, mathematical explanation and visualization models, keeping orchestration and education policy external.

#### M19-W01 — Pure computation and differentiation

- **M19-W01-C01 — Pure DAG and semantic identity.** Revisit M7 pure DAG design; support bounded acyclic computation, shared subexpressions, typed domains, deterministic replay and trace with IR compatibility. **Acceptance:** Independent evaluation graph and equivalence-by-identity fixtures, cycle/budget/domain/refusal and replay tests; no task orchestration or external effects.
- **M19-W01-C02 — Automatic differentiation.** Deliver admitted forward/reverse differentiation, gradients, Jacobians and Hessians for bounded typed operations with explicit nondifferentiability and domain restrictions. **Acceptance:** Analytic derivative and finite-difference references with tolerances, chain rule and domain/refusal cases; preserve symbolic contracts.
- **M19-W01-C03 — Structured symbolic steps and equivalence.** Deliver bounded justified transformations, solution steps, domain restrictions and equivalence checks with explicit assumptions and limits. **Acceptance:** Independent substitutions/counterexamples, domain-loss refusals, canonical IR and historical symbolics oracles; no general theorem prover or universal CAS.

#### M19-W02 — Mathematical models for applications

- **M19-W02-C01 — PlotModel and geometry.** Revisit M7 PlotModel; qualify function/curve, geometry parametric models, discontinuities, sampling and axis/unit semantics with portable representation. **Acceptance:** Golden analytic plots and discontinuities/asymptotes, invalid sampling domains and finite budgets; no UI rendering ownership.
- **M19-W02-C02 — Bounded examples and educational integration.** Deliver reproducible mathematically valid examples, structured steps and result/proof information as application-neutral models; coordinate Assessa/educational requests without owning grading. **Acceptance:** Independent exact/negative example oracles, deterministic replay and documented unsupported proof claims; no curriculum or pedagogical policy.
- **M19-W02-C03 — Computation and representation gate.** Qualify composed graph, autodiff, symbolics, PlotModel and examples together with typed contracts, package consumers and generated hierarchical Discovery. **Acceptance:** Verify actual supported operations, no lost domains, portable codecs, reproducibility and explicitly deferred formal proof scope.

### M20 — Math 2.0 integrated qualification and distribution

Qualify the exact supported 2.0 product graph, reproducible packages and skill, compatibility and rollback; separate any publication or downstream adoption.

#### M20-W01 — Release and interoperability gates

- **M20-W01-C01 — Product capability and compatibility portfolio.** Reconcile all milestones, completed evidence, research/deferrals, public API compatibility, migration guides, ownership and actual platform support; audit claims against RS429 baseline. **Acceptance:** Every accepted requirement maps to exact durable evidence or an explicit approved scope decision, no fabricated readiness or unsupported domain claim.
- **M20-W01-C02 — Integrated scientific and numerical certification.** Run delta and fragile contracts first, then broad regression, independent mathematical oracles, performance, memory, coverage, replay and deterministic serialization at this release trust boundary. **Acceptance:** Publish durable observed local results, compare agreed numerical/performance thresholds and classify failures; no bypass or routine full-suite repetition.
- **M20-W01-C03 — Immutable candidates and independent consumers.** Create isolated unpublished candidate graph with exact archive/assembly hashes and dependencies, locked package-only consumers on admitted Windows/Linux, AOT where qualified, rollback and acquisition evidence. **Acceptance:** Independent physical readback, exact dependency closure, rollback to prior archives with immutable prior identities; no silent shared feed write.
- **M20-W01-C04 — Full generated skill and AURA handoff.** Qualify complete admitted capability registry, generated hierarchy/resources, schema/binding and portable bundle against actual 2.0 candidates; request downstream AURA installation/invocation verdict. **Acceptance:** 100% classification/admitted exposure, deterministic regeneration, benchmark thresholds, locked invocation fixtures; downstream proof/permission remain separate.
- **M20-W01-C05 — Release readiness and owner-controlled distribution.** Record release candidate status, license/NOTICE/support, exact identities, publication policy, acquisition and rollback recipes; stage owner-controlled decision and handoffs to owning agents. **Acceptance:** Independent readiness evidence and explicit owner authorization required for actual private/public publication, reservation or downstream adoption; no assumed physical availability.
