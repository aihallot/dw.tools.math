# L4 — Hosted service, API, worker or DB-backed app audit checklist

## Use for

Use this checklist for hosted services, APIs, workers, database-backed applications and systems with deployment or runtime concerns.

A L4 solution should be safe to run, configure, observe, deploy and evolve without losing data or breaking contracts accidentally.

## Expected checks

- Startup configuration is validated.
- Secrets are not in source.
- Persistence and migrations are safe.
- Public contracts are stable or versioned.
- Logs are structured and safe.
- Health checks exist where meaningful.
- Docker readiness exists where meaningful.
- Integration tests cover critical boundaries.
- Operational run instructions exist.

## Common quality gates

- Data loss risk.
- Unsafe migration.
- Secrets committed.
- API contract broken accidentally.
- Service cannot start reproducibly.
- Missing validation at trust boundaries.
- Logs expose sensitive information.

## Typical prescriptions

- **Stabilize** configuration, startup and data-safety quality gates before new features.
- **Contain** persistence, external integrations and trust-boundary logic behind adapters or explicit contracts.
- **Align incrementally** logs, health checks and operational docs toward runtime needs.
- **Refactor locally** boundary handlers that own business rules or persistence mechanisms.
- **Preserve** public contracts unless breaking change and migration are explicit.
- **Defer** non-critical orchestration or platform work when the service is not yet distributed.

## Escalation triggers

Escalate beyond L4 when:

- multiple services coordinate through explicit contracts;
- orchestration, resilience, distributed tracing or cross-service compatibility matter;
- deployment topology becomes part of correctness;
- operations require platform-level handoff.
