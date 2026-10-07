# Engineering abstract — AI-assisted software engineering principles

This abstract summarizes the canonical engineering agreement. It is not a replacement for the full conventions document. It is the compact version to read before starting meaningful work.

## 1. Build the simplest serious solution

Use the simplest serious solution.

Not the simplest toy solution.\
Not the most elaborate theoretical architecture.\
Not an endless sequence of refinements that prevents delivery.

A serious solution is:

- safe;
- correct;
- testable;
- maintainable;
- proportionate;
- traceable;
- honest;
- evolutive;
- completable.

A solution is not serious if it only appears to work because it ignores security, configuration, localization, tests, existing behavior, planning, completion criteria, abstraction levels, subsidiarity, workflow or future maintainability.

## 2. Inspect before changing

Before creating or modifying files:

- inspect the repository;
- inspect relevant docs;
- inspect existing code;
- inspect tests and scripts when relevant;
- inspect handoff or project history when available;
- respect stricter local conventions.

Never assume the project structure from memory or from another repository.

## 3. Preserve working behavior

Existing working behavior is an asset.

Before refactoring or replacing it:

- identify what currently works;
- preserve important capabilities;
- add or adapt tests where possible;
- document anything intentionally deferred, removed or simplified.

A cleaner architecture that silently drops behavior is not a successful refactor.

## 4. Do not mix abstraction levels

Do not casually mix business rules, UI rendering, persistence, parsing, configuration, infrastructure, orchestration, logging and workflow bookkeeping in the same unit.

A high-level use case should read like a use case.\
A low-level adapter should handle low-level details.\
A UI component should compose presentation, not own business policy.\
A domain rule should not know about UI, files, HTTP, SQL, JSON or shell scripts unless that is truly its domain.

When abstraction levels are mixed, future changes become fragile.

## 5. Place code at the correct level of subsidiarity

Place each responsibility at the lowest level that can correctly own it, but not lower.

Use these tests:

- If the rule would still exist without this UI, it probably does not belong in the UI.
- If the rule would still exist without this specific database, it probably does not belong in the database adapter.
- If the rule is specific to one workflow, it probably does not belong in a generic library.
- If the capability is useful across several applications, it probably belongs in a reusable library.
- If the value varies by deployment, it belongs in configuration.
- If the text is visible to humans, it belongs in localization resources or an equivalent message layer.
- If the concern is technical integration, it belongs in infrastructure or an adapter.

Good subsidiarity prevents both duplication and over-generalization.

## 6. Separate policy, mechanism and presentation

Do not confuse:

- **policy**: what should happen and why;
- **mechanism**: how it is technically done;
- **presentation**: how it is shown or triggered.

Policy usually belongs in domain/application layers.\
Mechanism usually belongs in infrastructure/adapters.\
Presentation belongs in UI, CLI, TUI, API or other boundary layers.

## 7. Plan and track meaningful work

Non-trivial work must be planned and tracked.

Use:

- phase;
- task;
- subtask.

For larger projects, use:

- work package;
- phase;
- task;
- subtask.

Planning is not bureaucracy. It preserves project memory, enables handoff, and prevents uncontrolled drift.

## 8. Define completion criteria before improving endlessly

Every non-trivial item needs concrete completion criteria.

Good criteria are measurable or inspectable:

- a regression test fails before and passes after;
- a command exits with code 0;
- a generated file contains expected fields;
- metadata is preserved in output;
- a service starts with validated configuration;
- UI behavior is verified;
- docs reflect the new workflow.

Weak criteria are not completion boundaries:

- improve architecture;
- clean things up;
- make it better;
- polish the code;
- continue refactoring.

A good agent must know how to improve, but also how to stop.

## 9. Distinguish quality gates from improvements

A quality gate blocks completion.\
An improvement backlog item does not.

Quality gates include:

- failing tests;
- data loss;
- security issues;
- broken existing behavior;
- missing required validation;
- incomplete agreed scope;
- unsafe destructive behavior;
- contradiction with repository workflow.

Improvements include:

- broader refactoring;
- optional tests;
- future UI polish;
- optional performance tuning;
- possible library extraction;
- expanded documentation beyond current need.

Do not treat every improvement as a blocker.

## 10. Control scope

Prefer the smallest coherent change set that fully solves the task.

Do not expand scope to unrelated cleanup, modernization, renaming, refactoring or architecture changes unless:

- explicitly requested;
- required for safety or correctness;
- required to prevent clear duplication or breakage;
- documented as a follow-up rather than silently done.

Improve touched areas when reasonable. Do not rewrite the world because existing code is imperfect.

## 11. Treat high-risk areas defensively

Escalate rigor when work touches:

- deletion or cleanup;
- filesystem traversal;
- archive extraction;
- uploads;
- parsing external data;
- network calls;
- authentication or authorization;
- secrets;
- personal data;
- shell/process execution;
- plugins or dynamic code;
- database migrations;
- persisted user data;
- AI-generated input used as commands, code or data.

For high-risk areas, validate strictly, bound inputs, fail safely, test negative cases and avoid leaking sensitive data.

## 12. Prefer reuse without over-generalizing

Before adding a dependency or writing new infrastructure code, check:

1. current repository;
2. official platform and standard libraries;
3. own libraries and sibling repositories;
4. possible extension of an own library;
5. mature external dependencies;
6. custom implementation.

Do not pretend external or sibling libraries were inspected if they were not accessible.

Use mature dependencies for complex or security-sensitive domains. Avoid dependency bloat for small clear needs.

## 13. Configuration, constants and localization

Do not hard-code secrets, environment values, deployment values, product policy, business thresholds or user-facing text.

Use the right ownership:

- localization resources for user-facing text;
- typed options or configuration for variable behavior;
- defaults/definitions for product defaults;
- domain constants for true invariants;
- protocol constants for protocol-defined values;
- test data inside tests.

Moving arbitrary values into a global constants file is not enough.

## 14. Protect data and contracts

Persisted data and public contracts require care.

Do not silently delete, reset, migrate or drop fields.\
Preserve unknown or future fields when appropriate.\
Version durable formats when needed.\
Test representative real-world samples.\
Document migrations and compatibility expectations.

Contracts include APIs, public library surfaces, CLI commands, file formats, schemas, messages and externally consumed database shapes.

## 15. Test at the right level

Prefer TDD or test-with-code.

Tests should live at the level of the behavior:

- domain rules in domain tests;
- application workflows in application tests;
- infrastructure adapters in integration or contract tests;
- UI behavior in component/browser tests where practical;
- scripts through observable behavior or dry-runs.

Do not hide mixed responsibilities behind only high-level tests.

Coverage should be useful, not theatrical.

## 16. Be honest about validation

Never claim to have read, run, tested, validated, cleaned or completed something that was not actually done.

Always distinguish:

- what changed;
- what was validated;
- what was not validated;
- what remains required;
- what is optional follow-up;
- what risks remain;
- what commands should be run.

Trust is more important than sounding confident.

## 17. .NET profile summary

For .NET projects:

- for new .NET development, default to **.NET 10 (`net10.0`) and C# 14** unless an explicit compatibility, deployment, dependency or repository constraint requires another baseline;
- if an older baseline is required, document the reason rather than silently inheriting it from an old example or template;
- prefer `.slnx` where appropriate;
- use lowercase for solution names, project names, `.slnx` / `.sln` filenames, `.csproj` filenames, project folders and other command-facing paths;
- avoid PascalCase or mixed-case filesystem/project names such as `Dw.Tools.App`; prefer `dw.tools.app` while keeping C# namespaces/types idiomatic and PascalCase;
- use complete project/folder names;
- use PascalCase C# namespaces and types;
- set `RootNamespace` when useful;
- use nullable reference types by default for serious projects;
- treat warnings seriously;
- prefer typed options for configuration;
- use resources or message abstractions for user-facing text;
- keep business logic out of Blazor/MAUI/TUI/CLI/API boundary layers;
- use Docker for hosted services when meaningful;
- use Aspire only when there is an actual hosted/distributed boundary to orchestrate;
- keep build and publish outputs outside `src`;
- keep scripts safe, reproducible and honest.

## 18. Workflow adapters

Workflow-specific rules apply only when the repository and working context explicitly use them.

The GitHub runscript workflow applies only when working with ChatGPT outside Codex, the assistant has GitHub access, the assistant writes files on GitHub, and the user pulls/runs/commits/pushes locally.

It does not apply by default in Codex.

## 19. Operational classification

Before non-trivial work, classify:

- project level;
- risk triggers;
- planning granularity;
- completion criteria;
- quality gates;
- touched layers;
- abstraction/subsidiarity level;
- reuse/dependency check;
- configuration/localization/data impact;
- validation strategy;
- tracking/docs impact;
- follow-up bucket.

## 20. Handoff

Before handoff, report:

- completed work;
- completion criteria status;
- remaining quality gates;
- validation performed;
- validation not performed;
- risks;
- commands;
- docs/tracking updated;
- deferred follow-ups;
- convention deviations.

## Final rule

Use the simplest serious solution.

When the task is complete, validated enough for its level, and remaining ideas are non-blocking improvements, close it and move forward.
