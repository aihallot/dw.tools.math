# Execution contract — dw.tools.math

Read PROJECT-MANTRA.md, PROJECT-CONSTITUTION.md, docs/planning/implementation-handoff.md, docs/planning/resource-cost-policy.md, and the backlog before any work.

- Canonical repository language is English for committed documentation, planning, ADRs, code comments, evidence, commit messages, and developer-facing text unless an external source must be quoted verbatim. Conversation with the owner may be in French; conversational language must not become canonical repository language by default. Existing French planning material is migration debt, not precedent.
- Write scope: this repository only, even when other folders are visible. Do not modify, commit, publish, or start a migration in AURA, Decision, MCDM, or the brainstorming repository.
- Prepare external requests under docs/coordination/requests/ using the repository template. Do not send a request without explicit authorization; a drafted request is not an accepted request.
- Verify mathematical decisions with independent oracles and explicit contracts. A historically green test does not prove that a specification is correct.
- The product backlog defines accepted scope, stable product IDs, dependencies, acceptance recipes, and product meaning. The native DWF plan is the executable projection of that accepted roadmap and governs run selection and lifecycle transitions. Do not maintain a permanently reduced shadow plan.
- The initial DWF bootstrap may be deliberately thin only to make the first truthful run possible. Before preparing the next implementation run, reconcile the accepted backlog hierarchy into the native DWF hierarchy using the stable mapping release -> milestone, work_package -> workPackage, chunk -> phase, task -> task, subtask -> subtask. Preserve IDs, dependency meaning, functional boundaries, and completion semantics; do not mechanically copy fields that have no native DWF meaning.
- Every product run must declare the exact native project scope it advances. Its payload must perform the intended DWF lifecycle transitions in the same controlled mutation when progress changes. contribution.achievements must match exactly every native project node that the run transitions to done: no missing completion achievement and no achievement for a node that did not complete.
- Before authoring a payload, state the expected native-plan before/after states for every scoped node and verify that the payload accepts both the declared baseline and exact completed target. Do not add progress transitions after an otherwise successful attempt merely to satisfy projection.
- After durable success, reconcile product backlog status/projections from DWF evidence. The product backlog cannot authorize a run that DWF refuses, and DWF lifecycle state cannot erase a product acceptance recipe.
- Do not write DWF-managed files before DWF initialization. Re-read active DWF guidance before every preparation; no frozen copy from Decision is authority for the installed runtime.
- The planning validator is .NET-only, with no Python dependency. Validation does not generate; generation belongs in the payload.
- Zero sub-agents by default; use targeted work and targeted tests. Do not open unbounded review loops.
- At every checkpoint keep code, tests, contracts, backlog, native DWF plan, projections, and evidence coherent. Do not call an extraction delivered until provenance, rights, and an isolated consumer are qualified.
- Before 1.0, do not preserve abandoned contracts through artificial compatibility. Still respect stable commitments of external consumers; Decision already has 1.x APIs.
- Read docs/engineering/dotnet-authoring-checklist.md before non-trivial C# work.
