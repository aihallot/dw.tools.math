# Math / Decision — agent discovery interoperability decision

**Status:** Math-side architectural alignment decision, updated 2026-10-10 after Decision RS041. Math has not yet implemented or qualified Discovery, and no native DWF scope transition, cross-repository adoption, or AURA integration is implied.

**Authorities inspected:** `aihallot/dw.tools.math` main at RS040 success `55d861036b7f53d0ff139eb2b85c49e2e9ffce26`; `aihallot/dw.tools.decision` main at `52ebabd55c1fd7497380712f78b95f96c30828dd`, where `docs/planning/backlog.json` version `2026-10-07.37` and `docs/planning/versions/D6.md` plan `D6 — Découverte introspectable et skill agent`.

## Verified Decision RS041 alignment — 2026-10-10

**New remote authorities.** Decision `main` at commit `3c8a1b3a1ef818039a7c84ca5fa19666215c5a4b` (`feat: harmonize agent discovery contracts with Math`); completed `.aura/workflow/reports/RS041/attempt-001.json`, `outcome=succeeded`, first attempt with no reported mutation boundary violation. Canonical Decision backlog version `2026-10-10.39` now marks `D6-W01-C05` (Math/Decision harmonization) `done`. Decision's previous C01/C02 seed and GameTheory extensions are also `done`.

**Actual protocol candidate** (not merely a backlog description):

- Protocol id `dw.discovery`, version **`1.0.0-draft.1`**; manifest schema **2**. Decision preserved its historical manifest schema 1 in `docs/discovery/legacy/decision-manifest-v1.schema.json`; it must not reinterpret existing v1 as schema 2.
- Decision Discovery package and skill bundle **`1.6.0-preview.3`**, generator **`1.1.0`**. Math will have its own package/version; these numbers are **not** Math release versions.
- Normative candidate files to pin and independently qualify in Math: `docs/discovery/manifest.schema.json` and `docs/discovery/conformance.json` in the exact Decision commit above. Use the same expected accept/reject outcomes including valid Math/Decision owners, unknown schema/protocol, Documented leakage, wrong owner, stale binding fingerprint/signature, absent limits, premature BrokerReady, metadata authorization, invalid versions, duplicate IDs and catalogue-only leakage.
- Additional implementation references (not dependencies): `src/projects/dw.tools.decision.discovery/DiscoveryPublication.cs`, `tools/dw.tools.decision.skillgen/Alignment.cs`, `docs/discovery/agent-surface.json`, and generated `.agents/skills/dw-tools-decision/references/manifest.json`.
- Six deliberately selected Decision direct-.NET `AgentCallable` capabilities are in the generated skill, **zero `BrokerReady`**. `Documented` and catalogue-only scientific methods remain outside the agent-facing skill. This is verified Decision behavior, **not** evidence that the matching Math toolchain works.

**Implications for Math authoring.**

1. Adopt *that exact pinned draft protocol and portable conformance suite* as the reference for Math's first independently tested implementation. Any proposed semantic divergence must be recorded and resolved explicitly with Decision before announcing a stable shared contract; do not fork the schema silently.
2. First run an internal Math public-surface audit; publish only intentionally selected, bound and qualified operations. The inventory of helpers or `Documented` members never auto-appears in the skill or broker-ready manifest.
3. Begin with public **static** methods already available with complete CLR signatures. Decision's initial binding implementation selects public static methods; Math instance methods such as `FiniteMatrix64.Multiply` or `FiniteVector64.Dot` must either await qualified instance construction/serialization or be exposed by deliberately designed static agent-facing façades. Do not misrepresent an instance member as a static binding.
4. Example eligible static seed *candidates*, subject to formal admission: `ExactQuantityPipeline.Evaluate(IEnumerable<ExactPipelineStep>)`, `IrCanonicalJsonCodec.Encode(IrNode)`, `IrCanonicalHashes.SemanticStructuralSha256(IrNode)`, and `NumericalMatrixTransport.ToApproximateIr(FiniteMatrix64)`. These use different CLR types and guarantees. They are **not** automatically broker-ready DTO operations.
5. Preserve Math's current M2/M3 product evidence. The first Math Discovery work should introduce explicitly accepted backlog/native-DWF scope rather than modifying or renaming the existing `M3-W01-C02` numerical contract; the active Numerical T2 remains independently owed.
6. Keep the common schema ownership decision separate. If the neutral `dw.discovery` module is later established in `aihallot/dw.tools`, extract only common protocol/attributes/conformance. Neither Math nor Decision should depend on the other's analytical assembly.

**Convergence gate:** identical expected results from the pinned Decision fixtures, deterministic `generate/check`, verified C# signatures and actual compiled examples, stable owner-prefixed capability IDs, no helper/documented leakage, no fabricated serialization, no authorization claims. A successful Math run establishes two independent implementations of a **draft** protocol; a stable shared schema still needs an explicit owner/version decision.

## Decision

Math should adopt the **same conceptual discovery protocol** as Decision D6, rather than inventing a Math-specific alternative:

1. An **opt-in** semantic capability registry with stable identifiers independent of CLR method names.
2. An explicitly validated CLR member binding, introspected against the actual assembly and signature.
3. A versioned, deterministic, machine-readable **manifest** for brokers and a separately generated **progressive skill** for agents.
4. Full opt-in native attributes for future agent-oriented APIs; **assembly-level or external compiled bindings as a transitional bridge** for selected existing APIs. Do not force an immediate bulk refactor of stable math packages merely to add attributes.
5. Checked `generate`, `check`, `list`, `explain` modes, drift detection, source provenance, invalid-signature/duplicate-ID refusal, and separate versions for package/schema/generator/skill bundle.
6. No inverse dependency on AURA, no inferred provider availability, execution permission or broker authority.

The normative schema and attribute vocabulary must be coordinated **before a stable shared protocol is frozen**. Decision has now qualified a local *draft candidate* (RS041), but it does not establish Math interoperability without independent conformance. A neutral `dw.discovery/` project in `aihallot/dw.tools` remains a future explicit cross-project ownership decision. Until then, Math must not depend on Decision's product assembly or claim wire equivalence without running the same pinned schema and fixtures. No code or files in Decision or `dw.tools` are changed by this note.

## Critical distinction: inventory, catalogue, skill and invocability

Reflection may enumerate a package's public .NET surface **for internal coverage accounting**. Enumeration is **not publication**.

- **Unselected**: helper or ordinary .NET API; inventoried internally when useful; **absent from the generated agent skill and broker invocation list**.
- **Documented**: explicitly classified or described for an internal/developer catalogue but **not automatically included in an agent skill or callable manifest**. This level does not by itself establish agent invocation intent.
- **AgentCallable**: explicitly selected for agent discovery. Appears in the progressive skill with actual CLR binding, inputs, result, guarantees, constraints, and invocation example. It may require a C# runtime/adapter; it is not automatically a broker-ready JSON operation.
- **BrokerReady**: an AgentCallable subset with independently verified serializable inputs/outputs, stable invocation envelope, runtime adapter, cost and cancellation semantics. Exposed to the broker manifest as eligible for *consideration*, **not automatically available or authorized**.

This strengthens one detail in Decision D6: the seed may keep scientifically documented **catalog-only** methods in the internal catalogue, but the **agent skill must not advertise them as callable** without an explicit exposure decision. AURA treats `BrokerReady` as a compatibility prerequisite, not an access-control grant.

At runtime the broker alone decides whether the described operation is **installed, available, authorized, routable and executable**. No Math or Decision attribute can grant those properties.

## Candidate v1 capability record (field-level harmonization checklist)

The following is an **alignment checklist**, not a claimed finalized shared JSON Schema:

| Group | Required semantic meaning |
| --- | --- |
| Identity | `capabilityId`, contract version, owning package, family; namespaced identifiers such as `math.numerics.matrix.multiply` and `decision.graphs.shortest-path.dijkstra` |
| CLR binding | Assembly identity/version, fully qualified type, exact method/member signature and stable binding fingerprint; fail on ambiguity or missing member |
| Exposure | `Documented`, `AgentCallable`, `BrokerReady` with deliberate opt-in and no public-method convention |
| Invocation shape | JSON-schema/DTO compatibility when applicable, parameter names/types/requiredness/defaults and result/error/status envelope |
| Mathematics | Exact vs finite binary64/approximate, units/dimensions, assumptions/domains/shapes, deterministic/replay guarantee and numerical policies |
| Operational boundaries | Side effects, cancellation scope, budgets/quotas, provider/platform requirements, failure statuses and provenance |
| Proof and versions | Evidence references, package version, schema version, generator version, skill-bundle version, source commit |

Method metadata should rely on the actual C# signature, XML documentation and independently validated semantic annotations instead of duplicating routine type descriptions by hand. An attribute cannot turn an unsupported numeric precision guarantee into a fact.

## Proposed Math seed — deliberately narrow

An initial seed should bind a few already-qualified, meaningful operations using their exact current CLR signatures, for example:

- `math.quantity.exact-pipeline.evaluate` — exact finite unit-aware composition; boundaries from the M2 gate.
- `math.numerics.matrix.multiply` — finite approximate dense matrix product with explicit shape/work quota; boundaries from RS040.
- `math.numerics.vector.dot` — finite approximate scalar product.

These are **proposed IDs, not allocated or published capabilities**. The seed should include only intentionally selected agent-callable operations. Some input types require adapter DTOs before becoming `BrokerReady`. Do not represent a raw `FiniteMatrix64` or `UnitDefinition` instance as JSON-broker-invocable until serialization and type reconstruction have been tested.

The skill would be generated at `.agents/skills/dw-tools-math/SKILL.md` with an index, per-family pages, per-capability pages, invocation guidance, `references/manifest.json` and `references/skill-version.json`. The manifest is the **broker protocol**; Markdown is explanatory progressive context, not a protocol to parse.

## Incremental delivery and non-regression

**Now:** establish this cross-repo contract, enumerate the current Math public surface internally and record potential agent operations/exclusions. Keep the already qualified M2/M3 contracts intact.

**First substantive discovery foundation:** qualified schema/bindings/generator with deterministic generation, negative tests for duplicate IDs, unresolved CLR members, ambiguous signatures, bogus versions and unexpected public-surface leakage. Include a minimal skill seed and a coverage report; do **not** claim whole-library completion.

**Next independent delivery:** enrich the admitted agent surface in the already completed Math families, gate newly selected APIs at authoring time, and provide the versioned broker-envelope projection only once its DTO/JSON compatibility is independently qualified. Historical `Documented` items remain visible in internal coverage reporting but do not block product code or appear as callable skills.

**Every subsequent product run:** explicitly decide whether new public operations are agent-oriented. If yes, add complete semantic metadata and regenerate the manifest/skill in the same run; if no, the API remains an ordinary .NET API. Existing gaps should be reported with coverage and remedied progressively, not by blanket workflow checks that turn unrelated product work into preflight failures.

## Cross-repo acceptance before claiming interoperability

- Decision D6 and Math have identical normative interpretations of exposure and broker readiness.
- Both reject the same deliberately malformed fixtures: unknown ID, duplicate ID, stale signature, non-serializable BrokerReady parameters, omitted semantic limits and schema/version mismatch.
- A reference manifest can be validated by both with the same schema version and deterministic ordering, without depending on one another's product assemblies.
- The skill contains exactly explicitly selected agent-facing capabilities, never all public helpers.
- Broker selection, authentication, authorization, confirmation, isolation, execution and logging remain AURA-owned.

**No implementation or successful interoperability is claimed by this planning decision.** The next Math DWF authoring should attach the discovery foundation to an explicitly accepted project scope rather than silently relabel an existing numerical acceptance criterion.
