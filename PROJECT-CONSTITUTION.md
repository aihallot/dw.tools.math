# Product constitution

## Authorities

The owner's request dated 2026-10-07 authorizes planning and documentation preparation; it does not assert that any migration has already occurred.
Initial framing comes from ai-chatgpt/projects/dw-tools-math/README.md.
This repository owns the Math implementation plan. Historical exploration remains a source, not a second backlog to synchronize automatically.

The JSON backlog carries accepted product scope, stable identifiers, dependencies, acceptance recipes, and product history; architecture.md carries contracts. Version pages are projections.
After DWF adoption, the native DWF plan is the executable projection of that accepted roadmap and DWF receipts are authoritative for runs and execution transitions. A product transition is projected from durable evidence; any divergence blocks the affected next run until reconciled. The backlog cannot make a run admissible when DWF refuses it. DWF cannot erase a product acceptance recipe.

The native plan must not become a smaller competing roadmap. A deliberately thin initial bootstrap is temporary. Once DWF is operational, the accepted backlog hierarchy is reconciled into the fixed DWF hierarchy as release -> milestone, work package -> workPackage, chunk -> phase, task -> task, and subtask -> subtask. Stable IDs and dependency meaning are preserved. Product-only fields remain product authority instead of being forced into unrelated DWF fields. When finer-grained accepted children predate DWF but their parent is already durably completed, reconciliation may preserve a documented coarser historical projection instead of inventing unobserved child lifecycle transitions; the exception must be evidence-backed and narrowly scoped.

## Language

English is the canonical language of the repository: committed documentation, planning, ADRs, code comments, evidence, commit messages, and developer-facing text are authored in English unless verbatim external material requires another language.
Conversation with the owner may be in French and does not change canonical repository language.
The existing French planning corpus is migration debt from initial planning. It must be normalized from canonical sources such as the backlog and renderer, with projections regenerated and validated; generated projections must not be hand-translated independently.

## Planning and progress

Every controlled product run declares the native DWF nodes it advances and the expected lifecycle movement before execution.
When a run changes project progress, its payload performs those native DWF plan transitions inside the same controlled mutation. Progress is not patched into the plan after the product work has already run.
Every node transitioned to done has exactly one truthful contribution achievement for that run; achievements are not declared for nodes that remain incomplete.
Payload target re-entry must recognize the exact completed plan state as well as the declared baseline.
After durable success, backlog status and generated product projections are reconciled from DWF evidence. Planning refinement may add future native nodes between runs, but must preserve accepted product meaning and must not manufacture historical completion.

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
DWF alone owns workflow mechanics. This constitution constrains product planning discipline but does not redefine DWF run semantics, evidence, correction receipts, or terminal verdicts.
A run modifies Math only. Real integrations are requested and then attested by the owning repository agents.
