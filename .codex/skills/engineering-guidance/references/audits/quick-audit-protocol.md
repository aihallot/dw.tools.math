# Quick audit protocol

Use a quick audit when the goal is to decide whether it is safe to continue, not to produce a full report.

A quick audit is intentionally bounded. It should identify obvious quality gates, major concerns and the next smallest serious action.

## When to use

Use this protocol for:

- small changes;
- early sanity checks;
- low-risk repository orientation;
- deciding whether to continue or escalate;
- quick reprise before a bounded next step.

Do not use it as a substitute for a standard or deep audit when risk is high.

## Quick audit questions

```text
Quick audit:
- What is the scope?
- What level is the solution or change? L0-L5
- Are there obvious risk triggers?
- What evidence was inspected?
- What appears to work?
- What could be broken or unsafe?
- Are there quality gates?
- Is it safe to continue?
- What is the next smallest serious action?
```

## Quick audit verdict

```text
Quick audit verdict:
- Safe to continue: yes / with conditions / no
- Quality gates found:
- Main concerns:
- Evidence basis:
- Recommended next action:
```

## Escalation triggers

Escalate from quick audit to standard or deep audit when:

- risk triggers are present;
- behavior preservation is uncertain;
- data, contracts, migrations, secrets or public deployment are involved;
- the repository state is unclear;
- the agent cannot identify the correct next action confidently;
- the quick audit finds a quality gate that needs broader context.

## Output template

```text
# Quick solution audit

## Scope

## Evidence basis

## Key observations

## Quality gates

## Main concerns

## Verdict

## Recommended next action
```

## Rule

A quick audit should be honest about its limits.

Do not pretend that a quick audit is exhaustive.
