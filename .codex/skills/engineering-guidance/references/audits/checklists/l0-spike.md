# L0 — Spike, experiment, one-off script audit checklist

## Use for

Use this checklist for exploratory code, throwaway experiments, one-off scripts and early prototypes.

A L0 solution does not need full architecture ceremony, but it must still be safe and honest.

## Expected checks

- Purpose is clear.
- No hard-coded secrets.
- No broad destructive behavior.
- File paths or external inputs are bounded where relevant.
- Limitations are stated.
- Results are not presented as production-ready.
- Validation claims are honest.
- Code is understandable enough to discard, repeat or evolve.

## Common quality gates

- Unsafe deletion or overwrite behavior.
- Secrets committed in code.
- Misleading claims of validation.
- Destructive behavior without guardrails.
- Unbounded filesystem or external-input behavior in a risky context.

## Typical prescriptions

- **Preserve** a working proof of concept that is clearly marked as experimental.
- **Stabilize** destructive or unsafe behavior before reuse.
- **Contain** file, shell or network operations behind explicit guards.
- **Defer** architecture cleanup when the experiment is still disposable.
- **Align incrementally** if the spike is being promoted to L1 or higher.

## Escalation triggers

Escalate beyond L0 when:

- the script is reused by others;
- it touches user data, repository roots, secrets, network calls or external systems;
- it becomes part of a build, import/export, migration or deployment workflow;
- it is being promoted into an internal tool or library.
