# RS284 — Admit M8 root readiness without retrospective completion

## Decision and exact state change

Promote only the native root milestone `M8` from `not-ready` to `ready` through the documented typed DWF `ProjectPlan.TransitionNode` operation. The root dependency `M7` is durably `done`. This is a **readiness scheduling transition**, not the start or completion of M8 product delivery.

`M8-W01`, `M8-W02`, their phases, tasks, and all accepted RED/GREEN/verification subtasks remain `not-ready`. M9 and its descendants stay `not-ready`. No workflow active pointer changes; M0–M7 remain `done`. This run declares no `done` achievements.

## Evidence admitted for considering M8 work

- RS131 and RS209: exact pinned `dw.discovery/1.0.0-draft.1` portable envelope, thirteen cases (two accepted, eleven refused) with stable source identities
- RS185, RS187, RS195 and RS197–RS207: selected four CLR bindings, deterministic opt-in metadata, source examples, private-packaged signatures and bounded refusal checks
- RS279 and RS280: positive independent Windows .NET 11 private-package CLR consumer and conservative acceptance frontier
- RS281: isolated portable export, succeeded attempt 001 after **three pre-attempt refusals**, `firstPassSucceeded=false`
- RS282: actual portable export exercised as a private, locked, source-free Linux .NET 11 consumer, attempt 001 first-pass
- RS283: an **unsigned** portable SHA-256 integrity receipt, separate offline verifier and three deliberate tamper refusals, attempt 001 first-pass

These results justify authoring a fresh, independently bounded M8 work cycle. They do **not** satisfy accepted M8-W01-C01/C02 task-level RED, GREEN and verification lifecycles retrospectively. A local unsigned receipt does not authenticate origin, and no external AURA or consumer-side installation/permission is granted.

## Accepted product plan and boundaries

The canonical `docs/planning/backlog.json` version `0.1.124` retains M8 and M8-W01/M8-W02 chunks as `planned`; no release status, library API, source manifest, package, public release, license or sibling repository is changed. A native DWF milestone in `ready` is not a canonical product release in `done`.

The existing native plan contains 591 nodes. This run preserves every node identity, dependency and state except the one explicitly declared `M8` readiness transition. The maintained native adoption validator and Discovery acceptance validator are updated to enforce the exact new target while preserving child and historical boundaries.

## Forward-only continuation

Next, separately refine and admit one M8-W01 task according to its current accepted requirements and DWF lifecycle; perform new RED/GREEN/verification transitions only when the corresponding new evidence exists. Do not manufacture historical completions or make the broader .NET 11 migration decision on the basis of this milestone admission.
