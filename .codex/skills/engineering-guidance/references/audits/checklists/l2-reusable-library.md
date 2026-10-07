# L2 — Reusable library audit checklist

## Use for

Use this checklist for reusable libraries, shared packages, domain/core components and cross-project utilities.

A L2 solution should expose a clear API, avoid presentation assumptions and preserve meaningful invariants.

## Expected checks

- Public API is clear and cohesive.
- Library does not depend on UI or application-specific workflow.
- Domain invariants are documented or visible in tests.
- Errors are structured enough for consumers.
- Tests cover public behavior and edge cases.
- Dependencies are justified.
- Configuration or localization is not forced into final application form.
- Breaking changes are intentional.

## Common quality gates

- Public API unclear or unstable for intended use.
- Hidden UI or infrastructure assumptions.
- Weak tests around core behavior.
- Accidental breaking change.
- Dependency with unacceptable security, maintenance or licensing risk.
- App-specific workflow embedded in a generic library.

## Typical prescriptions

- **Preserve** stable public behavior and useful invariants.
- **Stabilize** public API or core behavior before broader adoption.
- **Extract** reusable behavior from UI, CLI or application layers when it has genuine cross-project value.
- **Contain** infrastructure-specific concerns behind adapters.
- **Align incrementally** error handling, options and tests toward library consumers.
- **Defer** over-generalized extension points until real reuse exists.

## Escalation triggers

Escalate beyond L2 when:

- the library becomes part of a hosted service or public API;
- it owns persistence, migrations or external contracts;
- it handles sensitive data or trust-boundary validation;
- breaking changes affect multiple consumers.
