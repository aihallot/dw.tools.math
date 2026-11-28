# Math 2.0 — automatic hierarchical Discovery and skill generation

Status: owner-requested planning scope, not implemented or qualified.
Baseline: RS429, a9547114a0ff0fc6ef6d0667bcc19f4079188ed8.
Source: [verbatim strategic audit](audit-original.fr.md), sections 4, 8 and 9, plus owner request of 2026-11-28.

## Objective

Generate a compact, navigable, complete agent-facing skill from the actual Math API, explicit semantic attributes, admitted capability contracts and authored resources. A human maintains the API and its semantics, not parallel hand-written skill pages. Generation is repeatable and incremental in development and independently verified before delivery. Optimal means demonstrably useful discovery and invocation under measured context budgets, not merely more exposed CLR methods or more documentation.

## Ownership and compatibility

- Reuse the M8 generator, manifest, canonical fingerprints, examples and portable exporter where applicable. Inventory the actual APIs and existing generator before designing replacements.
- Inspect the current dw.discovery/AURA-owned contracts with the owning agent; pin the protocol/schema and conformance corpus. Do not assume draft.1 is obsolete or upgrade it silently.
- Separate generic introspection, attribute contracts, generation and portable validation from Math-specific semantics. Prefer an existing common component; if generic work belongs upstream, prepare a source-pinned proposal for its owning agent. Math cannot modify sibling repositories or depend on aura.kernel.
- Preserve the four existing stable IDs and qualified historical bundles. Do not reinterpret AgentCallable as BrokerReady or grant permissions through a generated skill.

## Authoring model

One normalized capability descriptor is the semantic authority. Introspection discovers the full public API surface, including inherited, static, instance, generic and overloaded members, but does not admit them automatically. Explicit attributes reference an admitted contract and stable capability identity. XML documentation and versioned Markdown or structured resources supplement those contracts. The generator validates consistency; it does not invent missing semantics. Unknown or contradictory declarations are reported and excluded/fail closed.

Candidate attribute design (names and assembly ownership are to be selected after the protocol inventory):
- capability stable ID, contract version, semantic purpose, domain path, display name, aliases, selection hints;
- exact parameter and result types, semantic units/quantity kinds, domains, assumptions and numeric representation;
- declared effects, determinism, cancellation, resource and precision budgets, failure/refusal semantics;
- invocation eligibility and binding strategy; an explicit documented exclusion with reason for non-exposed APIs;
- links to versioned examples, independent oracles, constraints, conceptual resources and qualification evidence.

Do not encode executable mathematical logic in attributes or duplicate long prose in them. Machine-readable validation constraints must have a single authority. Long explanations, worked examples, domain context, formula derivations and caveats may live in authored resources with stable IDs; the normalized descriptor references them. XML docs can provide summaries and parameter meanings, not inferred authorization.

## Pipeline and extraction

1. Take a source/assembly/package-pinned inventory of declared public APIs. Use build-time metadata inspection (for example PE metadata/MetadataLoadContext or Roslyn) with no arbitrary code execution, no invoking type initializers or attribute constructors, and no ambient assembly search. Compare source descriptors to actual packed binary signatures and dependency closure. Investigate source-generated registry/binders for trimming/AOT; do not require runtime reflection.
2. Resolve explicit attributes, contract data and resource references into a canonical registry. Resolve overload identities, generic constraints, nullability, optional/default values, numeric formats and static/instance binding candidates. Reject ambiguous overloads, duplicate IDs, dangling resources and unqualified binding strategies.
3. Classify every inventory entry: admitted, documented-only, explicitly excluded, or pending review. Maintain independent eligibility and evidence states; new public entries must be classified. Require evidence for every exposed capability, not 100% public-method exposure.
4. Generate portable manifest(s), schemas, typed binding descriptors or static facades as appropriate, navigation, examples index, and skill pages from the registry. Complex CLR types need qualified DTO projections with explicit loss/refusal policies; never silently coerce exact rationals, dates, dimensions or special numbers.
5. Generate reproducible files in an isolated output root; validate deterministic ordering, canonical encoding, relative links, bounded page depth, path safety, duplicate and stale files, resources and hashes. Provide a check-only mode and a deliberate write/export mode. No machine-dependent paths, secrets or timestamps in canonical artifacts.
6. Qualify source and package-only use on admitted platforms, including independent executable positive/negative probes, version/fingerprint parity, reproducible bundles and selected trimming/AOT feasibility. Ask AURA's owning agent for a separate independent installation/invocation verdict; never infer authorization or adoption.

## Progressive skill design

One root SKILL.md contains the purpose, selection rules, main domain index, minimal invocation guidance and safety/refusal conventions. It is not an exhaustive catalog. A stable taxonomy groups capabilities by mathematical semantics rather than namespaces or project layout. Generate compact domain and subdomain indexes and capability pages only as needed; leave explanatory resources authored and referenced. Format and exact paths must follow the pinned skill/Discovery contract.

Illustrative tree (not a commitment to incompatible format):

    dw-tools-math/
      SKILL.md
      references/
        index.md
        domains/
          quantities/
            index.md
            conversion.md
          statistics/
            index.md
            descriptive.md
            distributions.md
          temporal/
            index.md
            time-zones.md
          scientific/
            index.md
            electricity.md
        manifest.json
        schemas/
        bindings/
      resources/
        concepts/
        examples/
        assumptions/
        numerical-policies/

The root points to the smallest useful next page. Cross-domain links resolve to canonical IDs, not copies. Stable paths survive CLR namespace refactors; separate mathematical meaning from invocation location. Domain index contains short descriptions, disambiguation and exact vs approximate distinctions. Capability page shows inputs/outputs, units, bounds, failure semantics, examples, provenance and invocation recipe. Long resources are fetched only when needed. Resource filenames, identifiers, nesting and links are constrained by the actual host contract.

## Acceptance and optimization

- Inventory coverage: every public candidate in the selected assemblies receives one disposition; zero silent omissions or accidental admissions. Exposure coverage: every admitted and qualified agent-callable capability appears exactly once in the registry and is reachable through the generated hierarchy; no orphan files, broken links, duplicate canonical IDs or ambiguous signatures.
- Contract coverage: 100% of exposed capabilities have valid typed schemas/binding descriptors (when invocation requires them), domain, numeric semantics, bounds, refusal policy, examples and evidence. Never fake a field to reach 100%; exclude unsupported bindings and track the gap.
- Determinism: two fresh generations from identical pinned inputs produce byte-identical canonical artifacts; reordered inputs do not change results. Drift check detects modified/removed/stale outputs. Negative fixtures cover malformed attributes, conflicting IDs, traversal, bad resources, signature drift, unsafe types, missing evidence and permission escalation.
- Quality benchmark: build a versioned intent-to-capability corpus with easy, ambiguous, cross-domain, exact-vs-approximate, invalid and refusal cases. Compare flat legacy baseline, generated hierarchy and at least one alternative navigation/partition strategy on identical cases. Measure retrieval top-1/top-k, correct selection and refusal, valid argument construction, independent numerical correctness, median/p95 tokens and loaded pages, task success and latency. Pin models/configuration and corpus; separate model variability from deterministic generation. Establish thresholds and resource ceilings from a measured baseline before declaring optimal; retain regressions, scores and failure classifications. Optimize under budgets, not by removing mandatory semantics or hiding admitted capabilities.
- Release gate: source and packed binary parity, independent package-only calls, portable bundle integrity, supported host/schema compatibility and documented exclusions. BrokerReady and AURA installation each require separate downstream evidence and authorization.
- New Math capabilities: add or reuse a capability ID, explicitly classify exposure, attach metadata/resources and negative oracles, regenerate/check artifacts, prove no unintended API or skill drift as part of each milestone. Never defer all Discovery work to M20.

## Non-goals

No automatic execution of arbitrary discovered methods; no inferred broker permission; no replacement of the numerical engines; no dependency on AURA runtime; no unbounded generic agent framework or claim of universally optimal skills. Source-level coverage, published archives, host installation and broker authorization remain separate evidence layers.
