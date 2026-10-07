---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.constitution"
documentVersion: 1
kind: "guidance"
title: "Workflow Constitution"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV018Policy.cs#Constitution"
---
# Workflow Constitution

Status: normative generic workflow-operating policy.

This constitution governs use of `dw.tools.workflow` in arbitrary software repositories. It is intentionally project-agnostic. Product architecture, framework choices, release strategy, domain rules, and repository-specific development policy belong to the project, not to the workflow engine.

`docs/workflow/mantra.md` is the short operational summary of this constitution. If they differ, this constitution wins.

## 1. Mission

`dw.tools.workflow` exists to let an agent perform bounded software-development work safely, deterministically, and with minimal operator ceremony.

The workflow improves execution discipline; it does not manufacture good intent from bad input. Success is an agent that understands the repository, authors a truthful bounded run, receives proportionate machine feedback, and converges quickly.

## 2. Authority layers

Keep workflow policy, project policy, and product truth distinct.

- The workflow-operating guidance selected by the executing tool governs workflow mechanics for the current session.
- Repository `docs/workflow` is authoritative only when it matches that active guidance. It may otherwise be older, divergent, or candidate product content.
- `.aura/workflow/plan/project.json`, workflow state, run material, reports, and evidence are repository-owned workflow authority.
- Product documentation, source, tests, schemas, deployment material, and domain evidence remain product authority.
- Optional `PROJECT-MANTRA.md` and `PROJECT-CONSTITUTION.md` are repository-owned project-development policy. They may constrain how the product is developed but must not redefine workflow mechanics.

The workflow engine must not special-case a repository because of product identity. A repository that happens to develop tooling, infrastructure, a framework, or another copy of a workflow-related product remains an ordinary consumer of the same generic workflow behavior.

When project policy conflicts with generic workflow mechanics, generic workflow policy wins for workflow operation. When workflow guidance is silent on a product decision, project/product authority wins; workflow must not invent the answer.

## 3. GIGO: education before guard proliferation

Garbage in, garbage out is a primary workflow principle.

A workflow can refuse unsafe or contradictory execution, but it cannot transform a poorly understood objective into good engineering. Therefore recurring agent mistakes should normally be addressed first through better guidance, examples, templates, discoverability, skills, planning context, or clearer error messages.

A new executable guard is justified when it protects a critical integrity, safety, provenance, recovery, or mutation boundary; when a demonstrated recurring failure remains likely after clear education; or when silent violation of a machine-verifiable invariant would materially corrupt or misrepresent repository state.

Every guard has a usability and maintenance cost. Prefer a few strong boundary guards plus excellent education over many brittle micro-guards.

## 4. Agent education is a product capability

The agent is a primary workflow consumer. Installed guidance must explain both what to do and why the boundary exists, without requiring implementation knowledge of the engine.

`dwf init` installs generic workflow guidance and workflow policy. It does not invent project-specific constitution or product strategy. Project policy is authored only from real repository/product intent.

Applicable engineering or domain skills may extend the agent's expertise. Skills complement workflow guidance; they do not justify framework-specific assumptions in the workflow core.

## 5. Repository adoption preserves product truth

Before choosing work, classify repository adoption from evidence. Greenfield, existing unmanaged, and legacy workflow repositories may require different instructions, but all modes preserve existing product truth.

Do not reconstruct fictitious history, manufacture completed progress, infer a backlog from filenames alone, overwrite an existing roadmap without an explicit decision, or advertise an unqualified migration/cutover path.

An initialized empty workflow plan is not evidence that the product itself is greenfield.

## 6. One bounded run, one truthful objective

Prepare exactly one next run. Do not stack successors.

A run should have one bounded outcome, complete and minimal mutation paths, proportionate validations, truthful contribution metadata, and no hidden dependency on unavailable future capabilities.

`.workflow/next-run/request.json` is the final GitHub write of a multi-file preparation. Prefer compact, transactional preparation history over trial-and-error commit storms.

The normal operator command remains `dwf run next`.

## 7. Validation: falsify cheaply, widen deliberately

Validation is risk-based and differential-first.

For ordinary development, use the smallest validation capable of falsifying the current change:

1. structural and compilation checks;
2. exact tests for the changed responsibility when available;
3. relevant packs or integrated surfaces when the change crosses responsibilities;
4. broader certification only when the trust boundary actually requires it.

Repository `planned` validation is the default when mutation responsibilities are catalogued. `explicit` is an escape hatch for a genuine planning gap or intentionally unusual boundary, not a way to defeat the planner because an author is nervous or a previous run failed.

A full suite is not the default cost of `dwf run next`. Full validation belongs at a genuine broad trust boundary such as release/capability qualification, promotion, dogfood closure, or a demonstrably cross-cutting change that cannot be falsified more narrowly.

When broad certification is required, run the current delta and the most plausible remaining fragile or historically failing contracts first. Only after they pass should the expensive broad suite run.

## 8. Failure is evidence, not permission to change methodology

A `failed` verdict is durable evidence. It does not authorize changing the workflow philosophy simply to obtain green.

After failure, inspect the newest durable evidence first; distinguish pre-attempt refusal, allocated-attempt failure, mutation-boundary failure, delivery failure, and internal failure; correct the same run when correction is possible; preserve retained mutation provenance after allocated attempts; and never prepare a successor to hide failure.

A correction may repair the cause, complete an omitted consumer, or remove an invalid assumption. It should not silently widen validation, change product/release strategy, add routine operator ceremony, weaken a safety boundary, or introduce a new architecture merely to make the current run pass.

If failures accumulate, stop patching the nearest symptom and re-audit the complete run material.

If the human explicitly rejects a preselection design before any attempt is allocated, the same run id may be transparently re-authored; the durable contribution must record the redirection.

## 9. Operator experience is a hard boundary

The operator should not become the workflow debugger.

Normal operation must not require manual workflow-authority edits, large pasted shell repairs, manual commit/push choreography, internal receipt knowledge, or implementation-specific recovery decisions.

Exceptional recovery, when genuinely necessary, should be bounded, deterministic, repository-owned, and productized rather than improvised terminal surgery.

The terminal contract remains a clear durable `pushed` or `failed` verdict.

## 10. Project policy is an extension point, not workflow behavior

A project may define `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, engineering skills, domain rules, release policy, architecture constraints, review policy, or other development conventions.

These documents are product/repository authority, not workflow-managed guidance. Generic init and guidance upgrade must preserve them. The workflow must not generate them from guesses or overwrite them as part of a guidance update.

Project policy may be more restrictive than generic workflow guidance for product development. It must not silently redefine run semantics, evidence meaning, workflow state, correction provenance, or operator verdicts.

## 11. Qualification, promotion, and dogfood

Do not qualify, promote, or dogfood merely because a run completed. Use coherent capability boundaries and explicit exit criteria.

Dogfood should add information rather than ceremony. Natural useful work is preferable to manufactured no-op invocations. Broad validation, when required, is a certification event rather than a per-slice ritual.

The specific release model of a product belongs to project policy unless the workflow contract itself requires a generic compatibility boundary.

## 12. Repository and history hygiene

Preparation history is part of trust. Inspect current authority before authoring, audit consumers of shared identities, use the active documented contract, and avoid operator executions as a substitute for basic authoring analysis.

Result commits produced by workflow must remain distinguishable from preparation commits. Durable evidence must remain reviewable rather than being hidden by automatic repair or rewritten history.

## 13. Required reading cadence

For any repository using this workflow:

- read the active workflow `mantra.md` before repository-state analysis;
- read this full constitution at conversation or handover start;
- after workflow policy, read `PROJECT-MANTRA.md` and `PROJECT-CONSTITUTION.md` when present;
- re-read this constitution after an explicit philosophy/process correction, after abnormal retries, before changing validation/recovery/planning methodology, and before broad qualification or promotion.

Reading is not ceremonial. If a proposed action conflicts with the active constitution, redesign the preparation rather than rationalizing the conflict.

## 14. Amendment rule

This generic constitution evolves only through the versioned workflow guidance pack. A change must be deliberate, visible in source and guidance history, and accompanied by an updated mantra when the summarized principles change.

Repository-specific policy amendments belong to the repository and must not be smuggled into generic workflow guidance.
