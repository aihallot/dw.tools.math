---
name: engineering-guidance
description: Apply the repository engineering guidance for AI-assisted software work, especially .NET projects, architecture decisions, planning, validation, audits, handoff, and drift correction.
---

# Engineering guidance skill

Use this skill whenever an AI agent is about to perform non-trivial software engineering work in this repository or in a project that follows these conventions.

Use it also when reviewing, auditing, stabilizing, handing off, or deciding whether to extend an existing software solution.

This skill is an operational entrypoint. It does not replace the copied reference files in `references/`.

## When to use

Use this skill for:

- project reprise or repository orientation;
- .NET, C#, Blazor, MAUI, CLI, TUI, API, worker or Aspire work;
- architecture or layering decisions;
- refactoring;
- import/export, persistence, migration or data-safety work;
- scripts, build, test, publish or workflow changes;
- validation and handoff;
- solution audits, quick audits, release/readiness checks, regression checks and agent handoff reviews;
- situations where the agent is drifting, over-expanding scope, mixing abstraction levels or polishing endlessly.

## Read order

Use the copied files in `references/`.

1. Read `references/engineering-mantra.md` when the work is drifting or when a quick behavioral reset is enough.
2. Read `references/engineering-abstract.md` before non-trivial implementation work.
3. Read `references/engineering-principles-dotnet-conventions.md` for complex, risky, long-lived, architectural, workflow-driven or ambiguous work.
4. Read `references/engineering-guidance-amendments.md` when the task touches an accepted pending amendment, especially build, test, publish, repository layout, workflow scripts or task runners.
5. Read `references/audits/README.md` when the task is to audit, review, reprise, release-check, regression-check, stabilize, or decide whether an existing solution is safe to extend.

For audit work, the audit README routes to the canonical method, audit-method amendments, quick protocol, report template, scorecard template and L0-L5 checklists.

During audits, accepted engineering amendments apply for their targeted area when they affect the audit scope.

The abstract and mantra are derived from the full reference. They accelerate use; they do not override the full reference. Accepted amendments apply immediately for their targeted area. Audit references complement the engineering guidance; they do not replace it.

## Current .NET defaults

For new .NET development, unless the inspected repository or an explicit compatibility/deployment/dependency constraint requires otherwise:

- target **.NET 10** (`net10.0`);
- use **C# 14**;
- use lowercase solution names, project names, `.slnx` / `.sln` filenames, `.csproj` filenames, project directories and command-facing paths;
- do not use PascalCase or mixed-case project/solution filesystem names merely because older .NET examples commonly do so;
- keep C# namespaces and types idiomatic and PascalCase independently of lowercase filesystem/project names;
- document any intentional older framework/language baseline rather than inheriting it silently.

Accepted amendment `A002` in `references/engineering-guidance-amendments.md` owns the current operational detail until its planned integration into canonical v2.8.

## Required behavior

Before non-trivial implementation work, classify the task:

- project level;
- risk triggers;
- planning granularity;
- completion criteria;
- quality gates;
- touched layers;
- abstraction and subsidiarity level;
- existing workflow constraints;
- reuse and dependency check;
- configuration, localization and data impact;
- validation strategy;
- tracking/docs impact;
- follow-up bucket.

Before audit work, classify the audit:

- scope;
- audit mode;
- project level;
- risk triggers;
- evidence basis;
- applicable engineering amendments;
- applicable audit-method amendments;
- applicable checklist;
- quality gates;
- concerns;
- not-assessed areas;
- verdict and next smallest serious action.

During implementation:

- inspect before changing;
- preserve working behavior;
- keep the change set coherent and bounded;
- do not mix abstraction levels;
- place code at the correct level of subsidiarity;
- distinguish quality gates from optional improvements;
- keep user-facing text localizable;
- keep variable behavior configurable;
- treat trust boundaries defensively;
- test at the right level;
- be honest about what was actually validated.

During audits:

- inspect before judging;
- distinguish observed, executed, inferred, reported and not assessed evidence;
- account for accepted engineering and audit-method amendments when they apply to the audit scope;
- do not turn every weakness into a blocker;
- do not recommend rewrite unless evidence justifies it;
- identify strengths to preserve;
- end with a clear verdict and recommended next action.

Before handoff, report:

- what changed;
- whether completion criteria were met;
- remaining quality gates, if any;
- what was validated;
- what was not validated;
- risks;
- commands;
- docs/tracking updates;
- deferred follow-ups;
- convention deviations.

## Stop rule

When completion criteria are met and remaining ideas are non-blocking improvements, close the current task, record follow-ups, and move forward.

Do not keep refining the same artifact or code path unless a quality gate remains or the user explicitly asks for another refinement pass.
