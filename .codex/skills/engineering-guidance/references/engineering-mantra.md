# Engineering mantra — read when drifting

Use the simplest serious solution.

Not a toy.\
Not a cathedral.\
Not an endless refinement loop.

## Before touching code

Inspect first.

Do not assume.\
Do not invent repository structure.\
Do not trust memory over files.\
Read the docs, code, tests, scripts and handoff material that matter.

## Protect what already works

Working behavior is valuable.

Do not lose features during cleanup.\
Do not call a refactor successful if it silently drops capability.\
Preserve first, improve second.

## Think in levels

Do not mix abstraction levels.

Business rules, UI rendering, persistence, parsing, configuration, orchestration and workflow bookkeeping do not belong casually in the same place.

High-level code should stay high-level.\
Low-level code should stay low-level.

## Put responsibility where it belongs

Use subsidiarity.

Place code at the lowest level that can correctly own it, but not lower.

Not everything belongs in the UI.\
Not everything belongs in a shared library.\
Not everything belongs in the domain.\
Not everything belongs in configuration.

Put the rule where it can be reused, tested and changed with the least inappropriate knowledge.

## Separate policy, mechanism and presentation

Policy: what should happen and why.\
Mechanism: how it is technically done.\
Presentation: how it is shown or triggered.

Do not let one pretend to be the other.

## Plan enough to remember

Non-trivial work needs structure.

Use phase, task, subtask.\
Use work package, phase, task, subtask for larger projects.

Planning is project memory, not ceremony.

## Define done

A task is not done because code exists.

Define concrete completion criteria.\
Define quality gates.\
Know what blocks completion and what is only improvement.

If “done” means “make it better”, the task is not defined.

## Stop when done

A good agent improves.\
A better agent also knows when to stop.

When completion criteria are met and remaining ideas are non-blocking, close the task.\
Record follow-ups.\
Move forward.

## Quality gates are not wishes

A quality gate blocks completion.

Failing tests, data loss, security issues, broken behavior, missing required validation and unsafe operations block completion.

Optional polish does not.

## Keep scope under control

Solve the task.\
Do not rewrite the world.\
Do not fix unrelated issues just because they are visible.

Leave the touched area better, but do not turn every task into a broad refactor.

## Be defensive near risk

Be stricter around deletion, files, uploads, parsing, network, authentication, credentials, personal data, process execution, migrations, persistence and AI-generated inputs.

Fail safe.\
Validate.\
Bound inputs.\
Use allow-lists.\
Avoid leaking sensitive data.

## Reuse wisely

Check existing code first.\
Then platform.\
Then own libraries.\
Then mature dependencies.\
Then custom code.

Do not reinvent risky infrastructure.\
Do not add dependencies for tiny clear needs.\
Do not pretend inaccessible libraries were inspected.

## Do not hard-code what can vary

Credentials are never hard-coded.\
Environment values belong in configuration.\
User-facing text belongs in localization.\
Business thresholds belong in options/defaults.\
True invariants may be constants.

A global constants file is not architecture.

## Test at the right level

Domain rules need domain tests.\
Application workflows need application tests.\
Adapters need integration or contract tests.\
UI behavior needs UI/component tests where practical.\
Scripts need observable validation or dry-runs.

Coverage must be useful, not theatrical.

## Tell the truth

Only claim what was actually done.

Say read, run, tested, validated, cleaned or completed only when it is true.

Say what changed.\
Say what passed.\
Say what was not run.\
Say what remains.\
Say what is follow-up.

Trust beats confidence.

## For .NET

Use modern .NET.\
Prefer LTS for durable production projects.\
Prefer `.slnx` when appropriate.\
Use lowercase command-facing paths.\
Use complete project names.\
Keep business logic out of Blazor, MAUI, TUI, CLI and API boundary code.\
Use typed options.\
Use localization-ready text.\
Use Docker for hosted services when meaningful.\
Use Aspire only when there is something real to orchestrate.\
Keep scripts safe and reproducible.

## Final reminder

The goal is not perfect code.

The goal is serious, safe, validated, maintainable progress.

If it is safe, correct enough for the level, validated honestly, tracked properly, and completable, move forward.
