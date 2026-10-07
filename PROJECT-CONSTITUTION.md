# Product constitution

## Authorities

The owner's request dated 2026-10-07 authorizes planning and documentation preparation; it does not assert that any migration has already occurred.
Initial framing comes from ai-chatgpt/projects/dw-tools-math/README.md.
This repository owns the Math implementation plan. Historical exploration remains a source, not a second backlog to synchronize automatically.

The JSON backlog carries requirements, dependencies, acceptance recipes, and product history; architecture.md carries contracts. Version pages are projections.
After DWF adoption, the native plan and DWF receipts are authoritative for runs and execution transitions. A product transition is a projection of evidence; any divergence blocks the affected next run until reconciled. The backlog cannot make a run admissible when DWF refuses it. DWF cannot erase a product acceptance recipe.

## Language

English is the canonical language of the repository: committed documentation, planning, ADRs, code comments, evidence, commit messages, and developer-facing text are authored in English unless verbatim external material requires another language.
Conversation with the owner may be in French and does not change canonical repository language.
The existing French planning corpus is migration debt from initial planning. It must be normalized from canonical sources such as the backlog and renderer, with projections regenerated and validated; generated projections must not be hand-translated independently.

## Boundaries

Math owns generic calculations and mathematical representations. Decision owns decision problems, MCDM owns preferences and multicriteria methods, and AURA owns exposure, resources, and invocation policies. Nutrition, generic data, and localization do not become Math domains.

Dependencies point from consumers toward Math. Math has no ProjectReference to a sibling repository and no dependency on aura.kernel. Required fixtures are versioned locally with provenance and rights. No script scans local disks to discover sibling repositories.

## Quality and publication

Exact and approximate are separate contracts. Any lossy conversion requires an explicit policy.
Domain variants, units, assumptions, precision, and budgets are part of the contract. Simplification never silently removes a restriction.
Provider qualification records version, platform, dependencies, license, edge cases, and reproducibility limits. A web page alone is insufficient.
No public publication or irreversible license choice occurs without an owner decision and verification of inherited rights.
Do not promise all mathematics: each family is delivered, researched, deferred, or assigned to another owner with a reason and a review trigger.

## Execution

The resource policy applies to both agents and operators. Versions describe functional milestones, not calendar promises.
DWF alone owns workflow mechanics. Do not restate or redefine DWF guidance in this constitution.
A run modifies Math only. Real integrations are requested and then attested by the owning repository agents.
