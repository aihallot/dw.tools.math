# L1 — Internal tool or CLI utility audit checklist

## Use for

Use this checklist for internal tools, CLI utilities, local automation, repository scripts and small operational helpers.

A L1 solution should be reproducible, safe to run and understandable by future maintainers or agents.

## Expected checks

- Commands to run the tool are documented.
- Inputs are validated or constrained.
- Dangerous paths are guarded.
- Diagnostics are understandable.
- Variable behavior is configurable.
- Core behavior has targeted tests or reproducible checks.
- Failure modes are clear.
- Scripts avoid unsafe side effects or support preview/dry-run where relevant.

## Common quality gates

- Unsafe file operations.
- Ambiguous command behavior.
- Broken core workflow.
- Missing validation for dangerous inputs.
- Hard-coded environment-specific values.
- Cleanup or overwrite logic without explicit guardrails.

## Typical prescriptions

- **Stabilize** broken core commands before adding features.
- **Contain** risky operations behind explicit paths, confirmations or dry-run behavior.
- **Align incrementally** hard-coded environment values into configuration or parameters.
- **Refactor locally** when command handling, validation and execution are tangled.
- **Defer** UI polish or broad architecture work if the tool is safe and fit for current use.

## Escalation triggers

Escalate beyond L1 when:

- the tool becomes a reusable library;
- it is used in CI, release, import/export or deployment;
- it handles persisted data, secrets or public contracts;
- it is used by multiple projects or teams.
