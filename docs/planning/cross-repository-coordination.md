# Math / AURA / Decision / MCDM coordination

This document is the canonical product protocol. An AURA-side acknowledgement note may link here; complete prompts remain in Math so DWF can work independently.

## Authority

A Math DWF run writes only under the Math repository root. Forbidden actions include mutating a sibling repository, write-mode git -C against another repository, ProjectReference escaping the root, installer scripts that patch AURA, or symlink/junction use to escape scope.
Source may be read only when explicitly accessible and authorized; no local path from the inventory is required at execution time.
The agent prepares a request file and the owner transmits it. No network message, external issue, or PR is emitted implicitly.

## Transfer chain without circular locking

1. **Baseline**: the AURA agent exports/lists sources and tests with commit, hashes, and rights; the Math agent records that input.
2. **Math availability**: Math implements and qualifies a package from a controlled snapshot without touching AURA. An AURA-free sample is enough for the local gate.
3. **Adoption request**: usable package/version/hash/feed, proposed changes, evidence, and rollback are sent to the AURA agent.
4. **Consumer adoption**: its agent changes references/facades and runs tests on its branch. Data, contracts, permissions, and resources remain under its control.
5. **Attested acknowledgement**: response includes commit, consumed version, results, and limits. Math stores a durable copy under docs/coordination/responses/.
6. **Duplicate removal**: AURA removes redundant sources only after consumer qualification and proven rollback. Math never performs that removal itself.

After the snapshot, any urgent AURA fix requires explicit notification and reconciliation. The temporary double-code window never authorizes silent divergence.
A rejection or API need blocks only the affected cutover. Independent Math work may continue according to dependencies.

## Exchange states

draft -> ready_for_owner -> sent -> acknowledged -> accepted or rejected -> implemented -> verified.
Transitions sent/acknowledged/accepted require human evidence or an external response; a Markdown file alone cannot create them.
Math feature state (planned/ready/in_progress/blocked/done) remains separate. Producing a request may close its documentation chunk, never the external integration itself.

## Request contents

Stable identity, author/recipient, status, context, source baseline, proposed contract/package, indicative recipient files, requested actions, non-goals, mathematical recipes and host tests, version/data/skill effects, rollback, evidence, and response format.
Never invent a SHA, version, or success. Fields marked TO PROVIDE must be completed before ready_for_owner.

## AURA specifics

Treat compilation graph and transitive packages together: core.math, kernel, nutrition, structured data, SQLite, and consumer tests.
Preserve public errors, configuration, exact fractions, catalog, culture, affine temperature, limits, and worker policies.
Ownership transfer grants no new resources. If commands/help/defaults/capabilities change, modify the versioned skill source and regenerate its pages; never edit generated output alone.
Update AURA canonical design, program/backlog, roadmap, execution, and guidance in its own repository.
Validation: pure math + targeted action/worker + CLI scenario and affected consumers; not the whole solution by default.

## Decision / MCDM specifics

Decision already exposes 1.x versions: preserve APIs, errors, quantile conventions, seed/PRNG, and existing guarantees, or propose a reasoned major evolution.
MCDM retains ranks, ties, orientation, preferences, and flow quantization. A floating accumulation change may alter ranking; domain oracles remain decisive.
Do not add a Math dependency for an abstraction without a concrete use. Propose a primitive only after behavior and cost comparison.

## Rollback

The consumer retains its previous baseline and dependencies in history, pins the adopted version/hash, and can return to the prior package/source without hidden data migration.
If persisted format changes, its agent must validate a standalone migration/rollback plan before adoption.
Never publish different bytes under the same PackageId/version. Feed ownership and identity transfer are attested before publication.
