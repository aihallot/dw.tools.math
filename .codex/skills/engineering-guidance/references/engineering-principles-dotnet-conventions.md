# Engineering principles & .NET implementation conventions — v2.7

Canonical working agreement for AI-assisted software engineering on the user’s projects.

This document is written for both humans and AI agents. It defines how to reason, plan, implement, validate, review, stop, document and hand off work in a way that is safe, serious, evolutive and recoverable.

The document is split into five layers:

1. **Universal engineering principles**
   - Apply to any software project, regardless of language or framework.
   - Define how to think, plan, track, validate, review, stop and preserve project memory.

2. **.NET implementation profile**
   - Applies the universal principles to .NET / C# / Blazor / MAUI / CLI / TUI / Aspire projects.
   - Defines concrete naming, layout, architecture, tooling and workflow expectations.

3. **Workflow adapters**
   - Apply only when a repository or working context explicitly uses a specific workflow.

4. **Assistant operating agreement**
   - Defines how an AI agent should behave before, during and after implementation.

5. **Practical templates and checklist**
   - Provides compact templates for operational classification, handoff and pre-flight checks.

When a repository defines stricter or more specific rules, the repository rules win. Always inspect before assuming.

---

# Part I — Universal engineering principles

## 0. Purpose

The goal is not to maximize architecture for its own sake.

The goal is to build software that is:

- safe;
- correct;
- secure by default;
- maintainable;
- testable;
- understandable by humans;
- recoverable by future AI agents;
- reusable when reuse is real;
- configurable where behavior can vary;
- localizable where humans see text;
- planned and tracked at the right granularity;
- bounded by clear completion criteria;
- reviewed with a purpose;
- honest about what was actually validated;
- proportionate to the project’s size, risk and expected lifetime.

Use the simplest serious solution.

Not the simplest toy solution.\
Not the most elaborate theoretical architecture.\
Not an endless sequence of refinements that prevents delivery.

The simplest serious solution is the smallest solution that remains safe, correct, testable, maintainable, evolutive, traceable, completable and honest.

---

## 1. Foundational design principles

### 1.1 Do not mix abstraction levels

Do not mix different levels of abstraction in the same unit of code, document section or architectural decision.

A single function, component, service, script or document section should not casually combine:

- business rules;
- UI rendering;
- persistence details;
- low-level parsing;
- infrastructure concerns;
- configuration lookup;
- orchestration;
- logging/diagnostics;
- workflow bookkeeping.

This does not mean every concern needs a separate project. It means each unit should operate at a coherent level of abstraction.

A high-level use case should read like a use case.\
A low-level adapter should handle low-level details.\
A UI component should compose presentation, not own business policy.\
A domain rule should not know about buttons, files, HTTP, JSON, SQL or shell scripts unless that is truly its domain.

When abstraction levels are mixed, future changes become fragile and agents are more likely to patch the wrong layer.

### 1.2 Place code at the right level of subsidiarity

Place each responsibility at the lowest level that can correctly own it, but not lower.

A rule belongs where it can be reused, tested and changed with the least inappropriate knowledge.

Use this subsidiarity test:

- If the rule would still exist without this UI, it probably does not belong in the UI.
- If the rule would still exist without this specific database, it probably does not belong in the database adapter.
- If the rule is specific to one application workflow, it probably does not belong in a generic library.
- If the capability is useful across several applications, it probably belongs in a reusable library.
- If the value varies by deployment, it belongs in configuration, not domain code.
- If the text is visible to humans, it belongs in localization resources or an equivalent message layer.
- If the concern is purely technical integration, it belongs in infrastructure or an adapter.

Good subsidiarity prevents both duplication and over-generalization.

Do not push code upward into presentation layers because it is convenient.\
Do not push code downward into shared libraries because it looks clean.\
Place it where the responsibility truly belongs.

### 1.3 Separate policy, mechanism and presentation

Do not confuse:

- **policy**: what should happen and why;
- **mechanism**: how it is technically done;
- **presentation**: how it is shown or triggered.

Policy should usually live in domain/application layers.\
Mechanism should usually live in infrastructure/adapters.\
Presentation should usually live in UI/CLI/TUI/API boundary layers.

This separation should be proportional to project level, but the distinction should remain clear even in small projects.

### 1.4 Keep together what changes together

Code that changes for the same reason should usually live together.

Code that changes for different reasons should usually be separated.

This principle helps decide whether responsibilities belong in the same unit or should be split across domain, application, infrastructure, presentation, configuration, tests or documentation.

Do not split code only to look clean.\
Do not merge unrelated concerns only because it is faster in the moment.

### 1.5 Define completion before optimizing endlessly

Every non-trivial work item should have completion criteria.

A subtask, task, phase or work package should be considered complete when its intended outcome and validation criteria are met, not when no further improvement can be imagined.

There will almost always be possible improvements. Do not let possible improvements block completion unless they are required for:

- correctness;
- safety;
- security;
- functional preservation;
- agreed scope;
- validation;
- project workflow.

Non-essential improvements should be captured as follow-up tasks, backlog items, lessons learned or proposed future work.

A good agent must know how to improve, but also how to stop.

### 1.6 Review with a purpose

Review cycles are valuable when they improve correctness, clarity, safety or fitness for purpose.

A review cycle should have a clear objective, such as:

- finding contradictions;
- checking applicability by an agent;
- validating completion criteria;
- identifying missing risks;
- improving operational clarity;
- deciding whether the artifact is mature enough to use.

Do not keep reviewing the same artifact indefinitely without a new purpose.

A review should end with one of:

- required changes before use;
- optional follow-ups;
- decision to use the artifact;
- decision to split the artifact into derived forms;
- decision to stop refining for now.

---

## 2. Non-negotiables

These rules apply to every project level, including small scripts and experiments.

### 2.1 Inspect before changing

Before creating or modifying files:

- inspect the existing repository;
- inspect relevant documentation;
- inspect existing code that may already solve the problem;
- inspect current tests and scripts when relevant;
- inspect project history or handoff material when available;
- respect stricter local conventions.

Never assume the structure from memory, from a previous project or from another repository.

### 2.2 Preserve working behavior

When a working prototype or existing feature exists:

- identify what currently works;
- preserve important capabilities;
- add or adapt tests when possible;
- do not silently drop behavior during refactoring;
- document any intentionally deferred, removed or simplified capability.

A refactor that loses important functionality without saying so is a failed refactor.

### 2.3 Plan and track meaningful work

Non-trivial work must be planned and tracked.

At minimum, use:

- phase;
- task;
- subtask.

For larger or longer-lived projects, use:

- work package;
- phase;
- task;
- subtask.

Example:

- WP1 — Import pipeline;
- Phase 1.2 — Reconciliation;
- Task 1.2.3 — Preserve chapter metadata;
- Subtask 1.2.3.a — Add regression test for chapter fields.

The goal is not bureaucracy. The goal is project memory, recoverability, validation and controlled progress.

### 2.4 Define concrete completion criteria

For any non-trivial planned item, define what “done” means as concretely as possible.

Completion criteria should be measurable or inspectable.

Good criteria:

- the target behavior exists;
- the regression test fails before and passes after;
- the command exits with code 0;
- the generated file contains expected fields;
- chapter metadata is preserved in output JSON;
- the service starts with validated configuration;
- the UI behavior is verified manually or automatically;
- documentation reflects the new workflow.

Weak criteria:

- improve architecture;
- clean things up;
- make it better;
- polish the code;
- continue refactoring;
- optimize quality.

Weak criteria may be useful as themes, but not as task completion boundaries.

### 2.5 Distinguish quality gates from improvements

A quality gate blocks completion.

An improvement backlog item does not.

Quality gates are requirements that must be satisfied before the current item can be closed, such as:

- failing tests;
- data loss;
- security issue;
- broken existing behavior;
- missing required validation;
- incomplete agreed scope;
- unsafe destructive behavior;
- contradiction with repository workflow.

Improvement backlog items are useful ideas that do not block completion, such as:

- broader refactoring;
- additional optional tests;
- future UI polish;
- optional performance tuning;
- extraction into a shared library later;
- documentation expansion beyond current need.

Do not treat every improvement as a quality gate.

### 2.6 No false claims

Never claim to have:

- read a file that was not read;
- inspected a repository that was not inspected;
- checked an external library that was not accessible;
- run a command that was not run;
- passed tests that were not executed;
- validated containerization, orchestration, UI, security or integration behavior that was not validated;
- cleaned files that were not cleaned;
- completed workflow steps that the user still has to perform.

If something is based on reasoning rather than execution, say so.

### 2.7 No broad destructive operations

Never write or suggest broad destructive scripts.

Never delete:

- a workspace;
- a repository root;
- a parent folder;
- an external folder;
- version-control metadata;
- user data outside the verified target scope.

Deletion must be explicit, guarded, auditable and constrained to known paths.

### 2.8 No hard-coded secrets

Secrets must never be committed or hard-coded.

This includes:

- API keys;
- tokens;
- passwords;
- private keys;
- connection strings;
- credentials;
- personal sensitive data.

Use environment variables, secret stores, user secrets or deployment-specific mechanisms.

### 2.9 User-facing text must be localizable

If text is visible to an end user, it must be localizable by design.

English may be the first and only provided language at the beginning, but the code should not make future localization expensive.

For small tools, localization readiness may mean isolating user-facing messages rather than introducing a full localization infrastructure immediately.

### 2.10 Validate trust boundaries

Any boundary involving user input, files, network, shell commands, configuration, AI-generated content, plugins, secrets or persistent data must be treated defensively.

### 2.11 Respect the active workflow

If the repository uses a specific workflow, follow it strictly.

Do not replace the workflow with a generic assistant habit.

Workflow-specific rules belong in workflow-specific documentation or appendices. They should not pollute the universal principles unless they apply broadly.

---

## 3. Decision model

### 3.1 Priority order

When rules compete, use this priority order:

1. Safety and data protection.
2. Correctness and functional preservation.
3. Security at trust boundaries.
4. Existing repository conventions.
5. Project memory and traceability.
6. Quality gates and controlled progress.
7. Maintainability and testability.
8. Correct abstraction level and subsidiarity.
9. Reuse and separation of concerns.
10. Performance.
11. Convenience.

Performance matters, but do not trade safety or correctness for speculative performance gains.

### 3.2 Requirement language

Use these words consistently:

- **Must** means required unless a documented exception exists.
- **Should** means the default expectation.
- **May** means allowed when useful.
- **Must not** means prohibited unless the user explicitly requests a controlled exception and the risk is acceptable.
- **Default** means the starting point, scaled by project level and risk.

Exceptions are allowed, but they must be intentional, visible and justified.

### 3.3 Required operational classification

Before non-trivial implementation, classify the work.

This classification may be internal for simple tasks, but it should guide the work. For larger tasks, it should be written in the plan or repository docs.

Use this template:

- Project level: L0 to L5.
- Risk triggers: yes/no, with details.
- Planning granularity: intent only, phase/task/subtask, or WP/phase/task/subtask.
- Completion criteria: concrete, measurable or inspectable.
- Quality gates: what blocks completion?
- Touched layers: domain, application, infrastructure, presentation, tests, docs, scripts.
- Abstraction/subsidiarity check: where should the responsibility live?
- Existing workflow constraints.
- Reuse check: current repo, platform, own libraries, external dependencies.
- Configuration and localization impact.
- Data, migration or compatibility impact.
- Validation strategy.
- Documentation and tracking updates required.
- Follow-up bucket: improvements that should not block the current task.

This prevents passive agreement with the rules without applying them.

---

## 4. Project engagement levels

Engineering rigor scales with the project level.

| Level | Project type | Required baseline |
|---|---|---|
| L0 | Spike, experiment, one-off script | Safe, readable, non-destructive, no secrets, honest validation notes |
| L1 | Internal tool or CLI utility | Core tests, safe options, config for variable values, clear diagnostics |
| L2 | Reusable library | Clean API, meaningful tests, no UI assumptions, documented invariants |
| L3 | Application with UI or presentation layer | Business logic outside UI, localizable text, culture-aware formatting, typed config |
| L4 | Hosted service, API, worker or DB-backed app | Container-ready by default, validated config, health checks where meaningful, safe persistence |
| L5 | Distributed app or product platform | Service boundaries, orchestration where appropriate, observability, integration/contract tests |

A small project can still contain high-risk code. Risk level overrides project size.

---

## 5. Capability expectations by level

| Capability | L0 | L1 | L2 | L3 | L4 | L5 |
|---|---:|---:|---:|---:|---:|---:|
| Inspect existing code | Must | Must | Must | Must | Must | Must |
| Plan and track work | Light | Phase/task | Phase/task | Phase/task | WP/phase/task | WP/phase/task |
| Completion criteria | Intent | Expected | Expected | Required | Required | Required |
| Quality gates | Basic | Expected | Expected | Required | Required | Required |
| No broad destructive scripts | Must | Must | Must | Must | Must | Must |
| No hard-coded secrets | Must | Must | Must | Must | Must | Must |
| Abstraction/subsidiarity check | Light | Expected | Expected | Required | Required | Required |
| Tests | Minimal/targeted | Core logic | High useful coverage | Domain/app + UI where practical | Unit + integration | Unit + integration + contract |
| Localization | Avoid blocking future i18n | Localize user output | Expose localizable structures | Required for user-facing text | Required | Required |
| Configuration | Minimal | For variable behavior | For policies/defaults | Typed options | Validated typed config | Environment-aware validated config |
| Container support | Usually no | Optional | Usually no | Optional | Default | Required/default |
| Orchestration | No | No | No | Optional | Should when distributed | Default when suitable |
| Observability | Basic errors | Clear diagnostics | Library diagnostics | App logs | Structured logs + health | Logs + health + telemetry |
| Documentation | Notes if needed | Usage | API/invariants | Architecture where useful | Operational assumptions | Architecture + operations + handoff |
| Progress tracking | Optional | Useful | Useful | Recommended | Required | Required |
| Follow-up backlog | Optional | Useful | Useful | Recommended | Required | Required |

---

## 6. Exit criteria by planning level

A planning item is not complete merely because code was written.

### 6.1 Subtask exit criteria

A subtask is complete when:

- its concrete output exists;
- its local validation is done or explicitly marked as not possible;
- it does not leave hidden breakage in the touched area;
- any non-blocking improvement is captured as follow-up.

Example:

- “Add regression fixture preserving chapter metadata” is complete when the fixture exists, is referenced by a test, and the expected preservation behavior is asserted.

### 6.2 Task exit criteria

A task is complete when:

- required subtasks are complete;
- task-level completion criteria are met;
- relevant tests or validation commands pass, or unrun validation is honestly reported;
- docs/progress are updated when required;
- remaining work is classified as required or follow-up.

Example:

- “Preserve chapter metadata during reconciliation” is complete when metadata is preserved in output, regression tests cover it, and validation status is recorded.

### 6.3 Phase exit criteria

A phase is complete when:

- the phase objective is delivered at the intended level of quality;
- all required tasks are complete;
- validation covers the phase goal;
- known limitations and follow-ups are documented;
- project status reflects the phase outcome.

Example:

- “Import reconciliation phase” is complete when representative inputs reconcile correctly, required metadata is preserved, validation commands are documented, and remaining enhancements are tracked.

### 6.4 Work package exit criteria

A work package is complete when:

- the planned capability set is delivered or explicitly descoped;
- phase outcomes are validated together where relevant;
- architecture, operational, data and compatibility impacts are documented;
- handoff material is accurate enough for future continuation;
- remaining work is tracked outside the completed WP.

Example:

- “WP2 — Import and reconciliation” is complete when import, validation, reconciliation and output behavior are integrated, tested with representative samples, documented and handoffable.

### 6.5 Definition of done by project level

| Level | Done means |
|---|---|
| L0 | Intent met, no unsafe behavior, limitations stated |
| L1 | Core behavior works, commands documented, dangerous paths checked |
| L2 | Public API or reusable behavior tested, invariants documented |
| L3 | Domain/application behavior tested, user-facing behavior validated where practical |
| L4 | Configuration, startup, persistence and integration boundaries checked |
| L5 | Contracts, orchestration, observability and handoff updated where relevant |

At every level, the final report must distinguish what was validated from what was not.

If completion criteria are met and remaining ideas are non-blocking improvements, close the current item and record follow-ups.

---

## 7. Completion discipline and improvement backlog

### 7.1 Avoid endless improvement loops

Do not keep expanding a task just because more improvements are possible.

Once the agreed completion criteria are met:

- stop the current task;
- report completion;
- report validation;
- record non-blocking improvements separately.

Do not turn every task into a sequence of indefinite polishing passes.

### 7.2 Separate required work from optional improvement

Classify remaining ideas as one of:

- **quality gate**: blocks completion because correctness, safety, security, agreed scope or validation would otherwise be insufficient;
- **required before completion**: part of the agreed scope that remains unfinished;
- **follow-up**: valuable but not required for the current item;
- **future enhancement**: broader improvement outside current scope;
- **lesson learned**: durable process improvement;
- **technical debt**: known weakness to prioritize later.

Only quality gates and required-before-completion items should block closing the current item.

### 7.3 Follow-up creation rule

Create a follow-up when an improvement is:

- useful;
- concrete enough to be resumed later;
- not required to satisfy the current completion criteria;
- too broad, risky or tangential for the current change set;
- better handled after more context, stabilization or user decision.

Do not create vague follow-ups.

Good follow-up:

- “Add integration test for malformed import JSON.”
- “Extract reusable file-normalization helper into `dw.tools.common.io` after confirming sibling library availability.”
- “Add Playwright smoke test for chapter filter once UI routes stabilize.”

Weak follow-up:

- “Improve code.”
- “Make architecture better.”
- “Continue polishing.”

### 7.4 Completion beats perfection when scope is satisfied

A completed, validated, well-scoped task is better than an indefinitely improving task that blocks the project.

This does not justify sloppy work. It prevents endless refinement beyond the current objective.

Quality must be built into the completion criteria, not pursued through unbounded iteration.

### 7.5 Iteration budget

Refinement is useful when it has a purpose.

Do not spend unlimited iterations refining the same artifact or code path unless the user explicitly asks for continued refinement or the current result still fails a quality gate.

When additional refinement is optional, record it as follow-up and move forward.

---

## 8. Review cycle discipline

### 8.1 Review objective

Each review cycle should answer a specific question.

Examples:

- Is this usable by an agent?
- Are there contradictions?
- Are completion criteria concrete enough?
- Is the abstraction level correct?
- Is the subsidiarity level correct?
- Are safety risks missing?
- Is this ready to become canonical?
- Should this now be split into abstract/mantra/checklist forms?

### 8.2 Review output

A review should produce one of:

- required changes before use;
- optional follow-ups;
- decision to use;
- decision to split;
- decision to stop refining for now.

Avoid reviews that only generate more vague improvement ideas.

### 8.3 Review stopping point

Stop reviewing when:

- the review objective is answered;
- remaining improvements are optional;
- the artifact is good enough for its intended use;
- further refinement would delay actual project progress more than it would reduce risk.

Continue reviewing when:

- contradictions remain;
- the artifact cannot be applied by an agent;
- completion criteria are still vague;
- quality gates are not met;
- the user explicitly wants another refinement pass.

---

## 9. Risk escalation triggers

Increase rigor whenever code touches:

- deletion or cleanup;
- filesystem traversal;
- archive extraction;
- user uploads;
- parsing external data;
- network calls;
- authentication;
- authorization;
- secrets;
- personal data;
- shell/process execution;
- plugins;
- dynamic code;
- database migrations;
- persisted user data;
- generated code;
- AI-generated input used as commands, code or data;
- payment, legal, medical or financial data.

For high-risk areas:

- validate inputs strictly;
- use allow-lists where possible;
- bound sizes and resource usage;
- fail safely;
- test negative cases;
- log carefully;
- avoid leaking sensitive data.

---

## 10. Planning and project memory

### 10.1 Planning hierarchy

Use structured planning to preserve project memory.

For small and medium work:

- Phase;
- Task;
- Subtask.

For larger projects:

- Work package;
- Phase;
- Task;
- Subtask.

Use stable identifiers when possible.

Examples:

- Phase 2 — Import pipeline;
- Task 2.3 — Validate metadata reconciliation;
- Subtask 2.3.a — Add failing regression fixture.

For larger plans:

- WP2 — Import and reconciliation;
- Phase WP2.3 — Metadata preservation;
- Task WP2.3.1 — Preserve chapter identity;
- Subtask WP2.3.1.a — Add regression test.

The exact numbering scheme may follow the repository’s convention. Consistency matters more than the specific format.

### 10.2 Planning depth by project size

Planning should be proportional.

L0 work may need only a short intent statement.

L1 to L2 work should usually track:

- phase;
- task;
- completion criteria;
- validation.

L3 work should track:

- feature area;
- phase;
- task;
- subtasks;
- UI/domain/application boundaries;
- completion criteria;
- validation.

L4 to L5 work should track:

- work package;
- phase;
- task;
- subtask;
- architecture impact;
- data/migration impact;
- validation status;
- operational impact;
- completion criteria;
- follow-up backlog;
- handoff notes.

### 10.3 Progress tracking

Long-lived projects should maintain progress in repository documentation.

Useful tracking documents may include:

- implementation plan;
- progress;
- roadmap;
- build validation;
- architecture;
- decision records;
- lessons learned;
- next-run buffer;
- transition handoff.

The exact files depend on the repository. Do not create all of them by default.

Create or update tracking only when it adds real project memory.

A tracking file that is not maintained becomes noise. Prefer fewer canonical tracking documents that stay accurate over many stale documents.

### 10.4 Single source of truth

Avoid multiple conflicting sources of truth.

When a plan, status, convention or validation result changes:

- update the canonical location;
- avoid stale duplicates;
- mark superseded documents clearly;
- do not let handoff notes contradict progress or roadmap docs.

If several documents are required by workflow, clarify which one owns the canonical status and which ones are summaries or buffers.

### 10.5 Lessons learned

Recurring mistakes must be turned into durable lessons.

Lessons learned should be used to prevent repeated failures, not merely recorded.

Examples of lesson-worthy issues:

- workflow misunderstanding;
- lost prototype feature;
- unsafe cleanup;
- missing validation;
- endless improvement loop;
- completion criteria missing or vague;
- wrong project structure;
- mixed abstraction levels;
- wrong subsidiarity level;
- duplicated abstraction;
- incorrect assumption about local execution;
- hardcoded value that should have been configurable;
- missing localization boundary.

---

## 11. Scope control

### 11.1 Smallest coherent change set

Prefer the smallest coherent change set that fully solves the task.

Do not expand scope to unrelated cleanup, modernization, renaming, refactoring or architectural changes unless:

- the user explicitly asks for it;
- it is required to complete the task safely;
- it prevents clear duplication or breakage;
- it is documented as a proposed follow-up rather than silently done.

Improve touched areas when reasonable, but do not rewrite the world because existing code is imperfect.

### 11.2 Existing repositories: incremental alignment

For existing repositories, do not force a full restructuring merely because the conventions are stronger than the current codebase.

Prefer:

- incremental alignment in touched areas;
- explicit follow-up tasks for larger debt;
- preservation of working behavior;
- tests around changed behavior;
- documentation of important deviations.

A weak existing convention is not automatic permission for a broad rewrite.

### 11.3 No unrelated fixes

Do not fix unrelated issues just because they were noticed.

Record them as follow-up work when useful.

A good agent leaves the project better in the touched area without turning every task into a broad refactor.

### 11.4 Improvement proposals should not block progress

When improvement ideas arise during a task, decide whether they are required for current completion.

If not, capture them separately and continue toward the current objective.

---

## 12. Reuse and dependencies

Before adding a dependency or writing new infrastructure code, check:

1. the current repository;
2. the official platform and standard libraries;
3. own libraries and sibling repositories in the same ecosystem;
4. whether an own library should be extended;
5. mature external dependencies;
6. custom implementation.

This order avoids both dependency bloat and dangerous NIH.

Own libraries means libraries maintained by the same user, team or ecosystem, even when they live outside the current repository.

If own libraries or sibling repositories are not accessible, do not pretend they were inspected. State the limitation and proceed with a safe local design or propose the external-library check as a follow-up.

If an inaccessible own library is likely to be the canonical place for the capability, avoid duplicating a permanent implementation. Prefer a temporary local adapter, a clearly marked local implementation, or a proposed library extension.

Prefer external dependencies when they are safer and more maintainable for complex or sensitive domains such as:

- cryptography;
- authentication;
- authorization;
- complex parsing;
- compression;
- database drivers;
- network protocols;
- serialization formats;
- PDF/document processing;
- security-sensitive infrastructure.

Avoid dependencies that are:

- unmaintained;
- invasive;
- unclear in licensing;
- supply-chain risky;
- much larger than the need;
- used only to avoid writing a small clear function.

If a dependency choice is not obvious, document why it is justified.

---

## 13. Internationalization and localization

The repository may have one canonical language. User-facing applications must still be localization-ready.

User-facing text includes:

- UI labels;
- page content;
- validation messages;
- user-facing exceptions;
- emails;
- reports;
- prompts;
- CLI/TUI output intended for users;
- notifications.

These should not be hard-coded directly in implementation code.

Use:

- resource files or equivalent message catalogs;
- stable resource keys;
- formatted strings with placeholders;
- culture-aware date/time/number/currency/percentage formatting;
- layouts that tolerate text expansion.

Avoid:

- generated keys tied to current wording;
- string concatenation for sentences;
- ASCII-only assumptions;
- unnecessary barriers to right-to-left languages or regional formats.

For reusable libraries:

- prefer error codes, result types or structured diagnostics;
- do not force final human prose as the only output;
- let presentation layers decide the final user-facing language.

Logs and internal diagnostics may remain in the repository’s canonical language unless the project says otherwise.

---

## 14. Values, constants, defaults and configuration

Do not hard-code values that represent:

- environment;
- deployment;
- product behavior;
- business policy;
- user-facing content;
- security-sensitive material.

Values that usually belong in configuration:

- URLs;
- ports;
- connection strings;
- feature flags;
- timeouts;
- retry counts;
- cache durations;
- service endpoints;
- storage paths;
- tenant settings;
- environment-dependent behavior.

Values that usually belong in typed options, defaults or definitions:

- page sizes;
- thresholds;
- business limits;
- default formats;
- default sort orders;
- role names;
- product policies.

Values that may be constants:

- mathematical identities;
- protocol constants;
- stable format identifiers;
- enum values;
- strongly named domain invariants;
- algorithmic invariants;
- test-local literals.

A value such as `diameter = 2 * pi * r` is a semantic invariant.\
A timeout, endpoint, message, limit or business threshold is not.

Do not solve hardcoding by dumping arbitrary values into a global constants file.

The location must reflect ownership:

- localization resources for user-facing text;
- typed options for configurable behavior;
- defaults/definitions for product defaults;
- domain constants for true domain invariants;
- protocol constants for protocol-defined values;
- test data inside tests.

Configuration should be:

- bound at the edge;
- validated early;
- injected through typed settings where practical;
- documented when defaults matter.

---

## 15. Data, persistence and migrations

Data safety is part of engineering quality.

For persisted data:

- never delete or reset user data implicitly;
- separate seed/demo data from user data;
- document storage locations;
- make destructive resets explicit;
- use backups or export paths when appropriate;
- treat migrations as code that needs validation.

For databases:

- migrations should be reviewed and tested where practical;
- schema changes should preserve data unless a deliberate migration plan says otherwise;
- rollback or recovery strategy should be considered for risky changes;
- credentials must stay out of source control.

For file-based data, JSON banks, imports and exports:

- preserve existing fields unless intentionally migrated;
- avoid silent data loss during reconciliation;
- version schemas when formats become durable;
- validate imports before overwriting outputs;
- keep original source files recoverable when practical;
- test representative real-world samples.

---

## 16. API, contracts and compatibility

Public contracts must be treated carefully.

Contracts include:

- public library APIs;
- HTTP APIs;
- CLI commands;
- file formats;
- JSON schemas;
- import/export formats;
- message contracts;
- database schemas when consumed externally.

For durable contracts:

- avoid breaking changes without explicit reason;
- version formats when needed;
- document compatibility expectations;
- add tests around serialization/deserialization;
- preserve unknown or future fields when appropriate;
- make migrations explicit.

Do not casually change a public shape just because the current implementation becomes simpler.

---

## 17. Observability and diagnostics

For hosted or distributed systems, diagnostics are part of the product.

Prefer:

- structured logs;
- clear error messages;
- health checks;
- startup configuration validation;
- correlation IDs for APIs/distributed flows where useful;
- metrics or telemetry where appropriate;
- actionable failure messages.

Avoid:

- logging secrets;
- logging raw sensitive payloads;
- swallowing exceptions silently;
- vague errors that cannot guide recovery.

For smaller tools, diagnostics can be simpler, but failures should still be understandable.

---

## 18. Build, run, publish and scripts

- Provide reproducible scripts instead of relying on manual steps.
- Give exact local commands.
- Keep scripts robust, readable, non-destructive and verifiable.
- Scripts must respect the real project hierarchy.
- Do not write flat build scripts that ignore repository structure.
- Local scripts should mirror CI as closely as practical.
- Build, run, test and publish workflows should be updated together when coupled.
- Scripts should be idempotent or clearly state when they are not.
- Scripts should fail clearly and safely when prerequisites are missing.
- Dangerous operations must have explicit guards.
- Non-trivial cleanup should support dry-run or equivalent preview.

Repeated validation commands should be captured in repository scripts or validation docs rather than only in chat.

---

## 19. Security and privacy

Security is a default, scaled by risk.

- Validate inputs at trust boundaries.
- Normalize and constrain paths.
- Bound size, recursion and resource usage for external inputs.
- Fail safely.
- Avoid insecure defaults.
- Keep permissions minimal.
- Prefer allow-lists.
- Use platform cryptography and identity primitives.
- Do not invent cryptography or authentication protocols.
- Do not leak secrets or sensitive data.
- Treat deserialization, shell execution, archive extraction, uploads, plugin loading, dynamic code and SSRF-like patterns as high risk.

Security work should be proportionate, but never ignored.

---

## 20. Testing, coverage and validation

Prefer TDD or test-with-code.

- No “by feeling” fixes without a test unless the reason is explicit.
- Add or adapt tests before or with fixes.
- Aim for maximal useful coverage, not theatrical coverage.
- Prioritize tests for:
  - domain rules;
  - application services;
  - configuration validation;
  - parsing;
  - imports/exports;
  - security boundaries;
  - file operations;
  - migrations;
  - contract compatibility;
  - regression cases;
  - error paths.
- Low coverage in risky areas is a risk.
- Coverage percentage does not replace meaningful assertions.
- UI should be validated behaviorally where practical.
- Include negative tests for malformed input, permissions, dangerous paths and boundaries.
- Document what was validated and what was not.

Tests should live at the right level:

- domain rules should be tested at domain level;
- application workflows should be tested at application level;
- infrastructure adapters should be tested with appropriate integration or contract tests;
- UI behavior should be tested through UI/component tests where practical;
- scripts should validate their observable behavior and dangerous paths.

Do not compensate for poor layering by writing only high-level tests that hide mixed responsibilities.

---

## 21. Documentation and maintainability

- Prefer clear names and small focused units.
- Comment why, constraints and invariants.
- Do not comment obvious mechanics.
- Document public APIs when useful.
- Document non-obvious trade-offs.
- Document dangerous operations.
- Keep architecture docs current when decisions affect future work.
- Avoid decorative deliverables.
- Avoid hollow folder structures.
- Avoid placeholder abstractions with no real value.
- Keep handoff docs accurate enough for a new conversation to continue safely.

Generated artifacts must be real and useful. Do not create empty ZIPs, hollow outputs, decorative reports or files that pretend progress without adding verifiable value.

---

## 22. Agent-friendly code shape

Code should remain idiomatic first.

Within idiomatic style:

- prefer cohesive files;
- keep responsibilities clear;
- use explicit section boundaries where useful;
- avoid giant mixed-purpose files;
- keep generated or replaceable blocks clearly delimited;
- keep resource keys stable;
- keep option names stable;
- keep component boundaries stable;
- avoid coupling unrelated concerns into one edit block.

Agent-friendly formatting must not make the code unnatural for the language, framework or file type.

For scripts that generate Markdown containing code fences, generate fences through a variable such as `FENCE` rather than nesting literal code fences inside chat-provided scripts.

---

## 23. Examples of wrong abstraction, subsidiarity or completion discipline

Bad:

- A UI component reads a JSON file, applies reconciliation rules, mutates output data and renders results.

Better:

- The UI calls an application service.
- The application service orchestrates the use case.
- Domain rules preserve metadata.
- Infrastructure reads/writes JSON.
- The UI renders the result.

Bad:

- A shared library contains one application-specific workflow because it looked reusable.

Better:

- Keep the workflow in the application layer.
- Extract only stable reusable primitives into the shared library.

Bad:

- A database adapter decides business validation rules.

Better:

- Domain/application validates policy.
- Infrastructure persists already validated state and translates storage errors.

Bad:

- A task keeps adding polish after the agreed behavior and validation are done.

Better:

- Close the task.
- Record further polish as follow-up work.

Bad:

- A review cycle keeps generating vague improvements without deciding whether the artifact is usable.

Better:

- Decide whether remaining issues are quality gates or follow-ups.
- Use the artifact when it is fit for purpose.

---

# Part II — .NET implementation profile

This profile applies the universal engineering principles to .NET / C# / Blazor / MAUI / CLI / TUI / Aspire projects.

If a repository defines stricter or more specific .NET conventions, the repository conventions win.

The universal rule applies first. The .NET profile explains what that rule means concretely in this ecosystem.

---

## 24. .NET platform and language

- Prefer modern .NET.
- Prefer LTS for long-lived production projects.
- Latest stable .NET is acceptable for tools, prototypes or projects that intentionally track modern .NET.
- Preview .NET or C# features must be explicit and justified.
- Use modern C#, but never force a language version unsupported by the actually installed SDK.
- Prefer SDK-supported language features over custom helper abstractions when the platform already solves the problem.
- Use nullable reference types by default for serious projects.
- Treat warnings seriously. Warnings-as-errors should be considered for mature projects and libraries.
- Keep analyzers, formatting and build validation aligned with the project’s level.

---

## 25. .NET naming and repository layout

- Prefer `.slnx` solution files over `.sln`, unless the repository constrains otherwise.
- Use lowercase for solution, project, folder and command-facing path names.
- Keep project and folder names complete and explicit, for example `src/dw.tools.<product>.web`, not `src/web`.
- Keep C# types — classes, records, interfaces and enums — in PascalCase.
- Set `RootNamespace` explicitly when useful so namespace policy is centralized.
- Make commands robust on case-sensitive file systems.
- Prefer durable names over abbreviations or conversation-specific shorthand.

Recommended root layout when starting fresh and when the project complexity justifies it:

- `src/`
- `tests/`
- `docs/`
- `scripts/`
- `build/`
- `publish/`

Keep generated and compiled artifacts out of `src`.

Put `build/` and `publish/` outside `src`, unless the repository already defines another convention.

Avoid `bin/` and `obj/` under `src` where reasonable through build output configuration.

For existing repositories, prefer incremental alignment in touched areas over broad restructuring.

---

## 26. .NET baseline files

For mature .NET repositories, consider the following baseline files and folders.

Do not create them mechanically. Add them when they serve the project level, workflow and validation needs.

Possible baseline files:

- `.editorconfig`;
- `global.json` when SDK pinning is useful;
- `Directory.Build.props`;
- `Directory.Build.targets` when truly needed;
- `Directory.Packages.props` when central package management is useful;
- `.config/dotnet-tools.json` for local tools;
- `scripts/build.ps1`;
- `scripts/test.ps1`;
- `scripts/run.ps1`;
- `scripts/publish.ps1`;
- `scripts/doctor.ps1` or equivalent;
- `docs/build-validation.md`;
- `docs/architecture.md`;
- `docs/implementation-plan.md`.

For small projects, a simpler setup may be better.

---

## 27. .NET solution structure

Use multi-project structure when real boundaries justify it.

Common .NET project boundaries may include:

- `*.domain` or `*.core`;
- `*.application`;
- `*.infrastructure`;
- `*.web`;
- `*.api`;
- `*.worker`;
- `*.cli`;
- `*.tui`;
- `*.maui`;
- `*.apphost`;
- `*.servicedefaults`;
- `*.tests`.

Do not create these projects mechanically. Create them when the level, risk or expected lifetime justifies the separation.

L0/L1 work may be a single project if that is the simplest serious solution.

L2+ reusable libraries should avoid presentation dependencies.

L3+ applications should keep business/application logic outside presentation projects where reasonably possible.

L4/L5 hosted systems should have explicit service, configuration and operational boundaries.

---

## 28. .NET architecture, abstraction and subsidiarity

Start from domain and capabilities, not from Blazor, MAUI, CLI, ASP.NET controllers or any presentation/transport framework.

Do not mix abstraction levels.

A domain service should not know about Razor components, HTTP controllers, SQL details, command-line flags or filesystem paths unless those concepts are truly part of the domain.

An application service should orchestrate use cases without becoming a dumping ground for persistence, UI formatting and low-level parsing.

An infrastructure adapter should implement technical mechanisms without owning business policy.

A presentation or transport layer should trigger and expose use cases, not own them.

Place code at the correct subsidiarity level:

- domain/core: business concepts, invariants, pure rules;
- application: use cases, orchestration, application policies, commands, workflows;
- infrastructure: persistence, filesystem, network, external services, adapters;
- presentation/boundary: UI, CLI, TUI, API endpoints, binding, rendering, input/output shape;
- tests: verification of the correct layer at the right boundary.

Domain invariants are rules that remain true regardless of application workflow.

Application policies are rules about how this product or workflow uses the domain.

Do not push application-specific workflow policy into generic domain code merely to make it look pure.

API endpoints and controllers are boundary adapters. They should handle transport concerns, input/output shape, authentication/authorization integration where appropriate, and delegation to application services. They should not own domain rules or persistence mechanisms.

A useful test:

> If the rule would still exist without this UI or transport, it probably belongs below the presentation/boundary layer.

Avoid both extremes:

- do not bury reusable business logic inside the first UI that needs it;
- do not extract generic libraries for rules that are actually application-specific.

---

## 29. .NET configuration and options

The universal configuration rule applies. In .NET, this usually means typed options.

Use configuration-first design for values that vary by environment, deployment, tenant, feature or policy.

Prefer:

- `IOptions<T>`;
- `IOptionsSnapshot<T>` where appropriate;
- `IOptionsMonitor<T>` where runtime updates are needed;
- options validation;
- fail-fast startup validation for required settings.

Avoid:

- scattered raw configuration lookups;
- magic strings for configuration keys everywhere;
- hidden defaults inside services;
- global constants for values that represent policy or environment.

Configuration should be bound at the application edge and injected as typed options.

Secrets should use:

- user secrets for local development where appropriate;
- environment variables;
- secret stores;
- deployment-specific secret mechanisms.

Never commit secrets.

---

## 30. .NET localization

The universal localization rule applies. In .NET, this usually means resources or a message abstraction.

User-facing .NET applications should be localization-ready.

For Blazor, MAUI, CLI, TUI and APIs that return user-facing messages:

- isolate user-facing messages;
- use resources or an equivalent message catalog when the project level justifies it;
- use stable resource keys;
- avoid hard-coded UI strings in components/pages;
- avoid string concatenation for translated sentences;
- use culture-aware formatting.

For L0/L1 tools, localization readiness may mean isolating messages behind a small message provider instead of introducing full resource infrastructure immediately.

For reusable libraries:

- prefer result codes, typed errors or structured diagnostics;
- do not force final English prose as the only consumer-facing output;
- let presentation layers localize.

Repository documentation, logs and developer diagnostics may remain in English unless the project says otherwise.

---

## 31. Blazor, MAUI, CLI and TUI

UI projects should be thin over shared application/domain logic.

Business rules should not live in:

- Razor components;
- MAUI pages;
- TUI screens;
- CLI command handlers.

Blazor/MAUI/TUI/CLI layers may contain presentation logic, but not reusable business policy.

Recommended separation:

- components/pages/screens: composition, rendering, events;
- presentation models or view models: UI state and display-ready shape;
- application services: use cases and orchestration;
- domain/core: business rules and invariants;
- infrastructure: files, databases, network, external systems.

Prefer reusable components where patterns are stable or repeated.

Use atomic design as a direction, not ceremony.

For mature UI systems, structure around:

- design tokens;
- atoms;
- molecules;
- organisms;
- templates;
- pages.

Do not create excessive abstractions before the UI pattern is understood.

Design for accessibility:

- semantic structure;
- keyboard support;
- focus states;
- readable contrast;
- labels;
- screen-reader behavior;
- reduced motion.

Provide coherent light/dark support when styling is in scope.

When themes are requested, deliver real themes, not placeholders.

---

## 32. Docker and Aspire for .NET

Docker and Aspire are defaults for hosted or distributed .NET systems, not ceremonies for every project.

Docker should be planned by default for:

- ASP.NET Core APIs;
- servers;
- workers;
- queue consumers;
- background services;
- database-backed applications;
- services likely to run outside the developer’s machine.

Aspire should be preferred when the application is distributed or service-oriented and the project context fits.

Do not introduce Aspire before there is an actual hosted or distributed boundary to orchestrate.

Hosted .NET services should include, where meaningful:

- Docker-ready structure;
- explicit ports;
- health checks;
- typed configuration;
- safe secret handling;
- persistent volumes;
- documented local run commands;
- local dependencies aligned with CI.

Aspire-oriented solutions may include:

- AppHost;
- ServiceDefaults;
- explicit service dependencies;
- health checks;
- telemetry defaults;
- local orchestration.

Docker/Aspire is usually not required for:

- pure libraries;
- tiny one-off scripts;
- small local-only tools;
- code where containerization has no practical meaning.

The exception should be intentional, not accidental.

---

## 33. .NET data, persistence and migrations

The universal data-safety rule applies.

For persisted data:

- never delete or reset user data implicitly;
- separate seed/demo data from user data;
- document storage locations;
- make destructive resets explicit;
- use backups or export paths when appropriate;
- treat migrations as code that needs validation.

For Entity Framework or database migrations:

- migrations should be reviewed and tested where practical;
- schema changes should preserve data unless a deliberate migration plan says otherwise;
- rollback or recovery strategy should be considered for risky changes;
- connection strings and credentials must stay out of source control.

For file-based data, JSON banks, imports and exports:

- preserve existing fields unless intentionally migrated;
- avoid silent data loss during reconciliation;
- version schemas when formats become durable;
- validate imports before overwriting outputs;
- keep original source files recoverable when practical;
- test representative real-world samples.

---

## 34. .NET build, run, test and publish

Provide reproducible scripts instead of relying only on manual steps.

Prefer repository scripts for repeated commands, such as:

- restore;
- build;
- test;
- format;
- lint/analyze;
- run;
- publish;
- doctor/check;
- clean with dry-run where appropriate.

Scripts must respect the real `.csproj` hierarchy.

Do not write flat build scripts that ignore repository structure.

Local scripts should mirror CI as closely as practical.

Build, run, test and publish workflows should be updated together when coupled.

Repeated validation commands should be captured in scripts or validation docs, not only in chat.

PowerShell scripts should be:

- robust;
- readable;
- non-destructive;
- guarded;
- explicit about prerequisites;
- safe on path handling;
- honest about what they did.

---

## 35. .NET testing

Prefer TDD or test-with-code.

Use test strategy proportional to project level.

For L0:

- targeted checks or manual validation may be enough;
- limitations must be stated.

For L1:

- test core behavior;
- test dangerous paths;
- document commands.

For L2:

- test public APIs;
- test invariants;
- test error cases;
- avoid UI assumptions.

For L3:

- test domain/application logic;
- validate UI behavior where practical;
- avoid relying on compilation alone.

For L4:

- test configuration validation;
- test service startup where practical;
- test persistence and integration boundaries;
- test import/export and migration risks.

For L5:

- add contract/integration tests where meaningful;
- validate orchestration assumptions;
- validate observability and operational expectations where practical.

Useful .NET test categories may include:

- unit tests for domain/application logic;
- integration tests for adapters and persistence;
- configuration validation tests;
- import/export regression tests;
- contract tests for durable APIs or schemas;
- bUnit tests for Blazor components when useful;
- Playwright or equivalent browser tests when real UI behavior matters;
- golden-file tests for stable import/export outputs, with care for sensitive data.

Tests should live at the right level:

- domain rules should be tested in domain/core tests;
- application workflows should be tested through application tests;
- infrastructure adapters should use integration or contract tests where meaningful;
- UI behavior should use component or browser-level tests where practical;
- script behavior should be validated by script-level checks or explicit dry-runs.

Coverage should be useful, not theatrical.

Low coverage in risky areas is a risk.

---

## 36. .NET diagnostics and observability

For hosted .NET systems, prefer:

- structured logging;
- clear startup failures;
- configuration validation;
- health checks;
- correlation IDs where useful;
- metrics or telemetry where appropriate;
- actionable errors.

Avoid:

- logging secrets;
- logging raw sensitive payloads;
- swallowing exceptions silently;
- vague failures that cannot guide recovery.

For smaller tools, diagnostics can be simpler, but failures should still be understandable.

---

## 37. .NET generated artifacts and packaging

Generated artifacts must be useful, verifiable and placed intentionally.

Do not place generated artifacts under `src`.

Do not create:

- empty ZIPs;
- hollow package outputs;
- decorative reports;
- placeholder folders pretending progress;
- generated files that are not validated or referenced.

If a generated artifact is user-facing or release-facing, record how it was generated and how it can be validated.

---

# Part III — Workflow adapters

Workflow adapters are not universal engineering rules. They apply only when the current repository and working context explicitly use them.

## 38. GitHub runscript workflow adapter

This adapter applies only when all of the following are true:

- the user is working with ChatGPT outside Codex;
- the assistant has access to GitHub;
- the assistant is expected to write files directly on GitHub;
- the user pulls locally, runs scripts locally, commits locally, pushes, then says `pushed`;
- the repository explicitly uses the runscript/checkpoint workflow.

This adapter does not apply by default in Codex.

When this workflow is active:

- the assistant writes files on GitHub;
- the user executes locally;
- the assistant must not ask the user to manually create files the assistant is supposed to deposit on GitHub;
- the assistant creates run scripts `rs*.ps1` and checkpoints `cs*.ps1` on GitHub when the workflow requires it;
- before any change, the assistant reads the repository’s current workflow docs;
- when the user says `pushed`, the assistant checks current repository state and continues logically;
- runs should be substantial enough to make progress but cut cleanly enough that errors stay localizable;
- lessons learned must be accounted for;
- the assistant must never claim to have executed local workflow steps that the user executed.

Details of a specific runscript workflow should live in repository workflow docs, not in this core convention document.

---

# Part IV — Assistant operating agreement

## 39. Before coding

The assistant must:

- understand the task;
- inspect the repository;
- inspect relevant docs and tests;
- identify stricter local conventions;
- identify project level;
- identify risk level;
- identify required planning granularity;
- define completion criteria;
- identify quality gates;
- check existing code;
- check platform and own-library capabilities;
- identify the correct abstraction and subsidiarity level;
- plan the change;
- decide what needs validation;
- avoid duplicate abstractions;
- avoid unrelated scope expansion.

For non-trivial tasks, the assistant should present a concise plan before implementation.

## 40. During implementation

The assistant must:

- make focused changes;
- preserve existing behavior;
- track progress at the chosen granularity;
- work toward the defined completion criteria;
- distinguish quality gates from improvements;
- avoid mixing abstraction levels;
- place code at the correct subsidiarity level;
- avoid premature abstractions;
- avoid under-engineering reusable logic;
- keep user-facing text localizable;
- keep variable behavior configurable;
- handle trust boundaries defensively;
- update related scripts together when needed;
- document important decisions;
- capture non-blocking improvements as follow-ups;
- update progress/handoff docs when the repository workflow expects it.

## 41. Before handoff

The assistant must report:

- what changed;
- what phase/task/subtask was advanced when applicable;
- whether completion criteria were met;
- which quality gates remain, if any;
- what was validated;
- what was not validated;
- what risks remain;
- what commands the user should run;
- what docs or handoff notes were updated;
- which improvement ideas were deferred as follow-ups;
- any intentional convention deviations.

The assistant must not hide partial failure.

## 42. If blocked

If blocked by missing permissions, unavailable tools, failing commands, ambiguous repository state or insufficient context, the assistant must say so clearly.

When possible, the assistant should still provide the best safe partial result and explain what remains unresolved.

If blocked from completing the current item, the assistant should distinguish:

- what is already complete;
- what remains required;
- what quality gate is blocking completion;
- what is optional follow-up;
- what exact next action would unblock progress.

---

# Part V — Practical templates and checklist

## 43. Operational classification template

For non-trivial work, use this shape internally or explicitly:

```text
Operational classification:
- Project level:
- Risk triggers:
- Planning granularity:
- Completion criteria:
- Quality gates:
- Touched layers:
- Abstraction/subsidiarity check:
- Existing workflow constraints:
- Reuse/dependency check:
- Config/i18n/data impact:
- Validation strategy:
- Tracking/docs impact:
- Follow-up bucket:
```

## 44. Handoff template

For non-trivial handoff, use this shape internally or explicitly:

```text
Handoff:
- Completed:
- Completion criteria met:
- Quality gates remaining:
- Validated:
- Not validated:
- Risks:
- Commands:
- Docs/tracking updated:
- Deferred follow-ups:
- Convention deviations:
```

## 45. Practical checklist

Before coding or patching, check:

- What level is this project or task?
- Is any part high risk?
- What planning granularity is needed: phase/task/subtask or WP/phase/task/subtask?
- What are the concrete completion criteria?
- What are the quality gates?
- What existing repository rules apply?
- What current behavior must be preserved?
- What existing code already solves part of this?
- Does the platform already provide this?
- Do own libraries or sibling repositories already provide this?
- Are those own libraries actually accessible for inspection?
- Is this domain, application, infrastructure or presentation logic?
- Am I mixing abstraction levels?
- What is the correct subsidiarity level for this responsibility?
- Which user-facing text must be localizable?
- Which values belong in configuration, defaults, resources or constants?
- Does this need container support?
- Does this need orchestration?
- Are there persisted data or migration risks?
- Are there API or file-format compatibility risks?
- What security boundaries exist?
- What tests are needed?
- What commands should the user or CI run?
- What docs, progress tracking or handoff notes must be updated?
- Is the change set the smallest coherent one that solves the task?
- Which improvements should be deferred instead of blocking completion?
- Is another review cycle needed, or is this fit for use?

---

# Final rule

Use the simplest serious solution.

A serious solution is safe, correct, testable, maintainable, proportionate, traceable, honest, evolutive and completable.

A solution is not serious if it only appears to work because it ignores localization, configuration, security, tests, existing behavior, planning, completion criteria, quality gates, abstraction levels, subsidiarity, repository workflow or future maintainability.
