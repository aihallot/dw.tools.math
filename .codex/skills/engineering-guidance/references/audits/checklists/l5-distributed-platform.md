# L5 — Distributed app or product platform audit checklist

## Use for

Use this checklist for distributed systems, product platforms, multi-service applications and systems where orchestration, contracts and operations are central.

A L5 solution should make service ownership, contracts, observability, resilience and operational handoff explicit.

## Expected checks

- Service boundaries are explicit and justified.
- Contracts between services are documented and tested.
- Orchestration is meaningful and reproducible.
- Observability supports diagnosis across services.
- Failure and retry behavior are considered.
- Compatibility and migrations are managed.
- Operational handoff is sufficient.
- Security and privacy boundaries are explicit.
- Integration and contract tests cover critical flows.

## Common quality gates

- Unclear service ownership.
- Untested contract changes.
- Missing operational visibility.
- Fragile cross-service data assumptions.
- Fragile deployment or orchestration.
- No credible handoff for operations.
- Security or privacy boundary unclear across services.

## Typical prescriptions

- **Preserve** working contracts and operationally proven flows.
- **Stabilize** contract, observability, security and deployment quality gates before platform expansion.
- **Contain** fragile service interactions behind explicit contracts or compatibility layers.
- **Align incrementally** orchestration, health, logging and tracing toward diagnosable operations.
- **Refactor locally** unclear service ownership before broad rewrites.
- **Rewrite** only when service boundaries or data-safety flaws structurally prevent safe progress.

## Escalation triggers

Escalation beyond L5 is not a normal project-level increase.

Instead, split the audit by platform area, service group, operational concern or release boundary when the system is too large for one coherent audit.
