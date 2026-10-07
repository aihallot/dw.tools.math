# Execution contract — dw.tools.math

Read PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, docs/planning/implementation-handoff.md, docs/planning/resource-cost-policy.md, and the backlog before any work.

- Canonical repository language is English for committed documentation, planning, ADRs, code comments, evidence, commit messages, and developer-facing text unless an external source must be quoted verbatim. Conversation with the owner may be in French; conversational language must not become canonical repository language by default. Existing French planning material is migration debt, not precedent.
- Write scope: this repository only, even when other folders are visible. Do not modify, commit, publish, or start a migration in AURA, Decision, MCDM, or the brainstorming repository.
- Prepare external requests under docs/coordination/requests/ using the repository template. Do not send a request without explicit authorization; a drafted request is not an accepted request.
- Verify mathematical decisions with independent oracles and explicit contracts. A historically green test does not prove that a specification is correct.
- The product backlog defines scope, identifiers, dependencies, and acceptance recipes. After DWF adoption, its native plan governs execution; mapping and product status are reconciled from evidence without inventing a second execution authority.
- Do not write DWF-managed files before DWF initialization. Re-read active DWF guidance before every preparation; no frozen copy from Decision is authority for the installed runtime.
- The planning validator is .NET-only, with no Python dependency. Validation does not generate; generation belongs in the payload.
- Zero sub-agents by default; use targeted work and targeted tests. Do not open unbounded review loops.
- At every checkpoint keep code, tests, contracts, backlog, projections, and evidence coherent. Do not call an extraction delivered until provenance, rights, and an isolated consumer are qualified.
- Before 1.0, do not preserve abandoned contracts through artificial compatibility. Still respect stable commitments of external consumers; Decision already has 1.x APIs.
- Read docs/engineering/dotnet-authoring-checklist.md before non-trivial C# work.
