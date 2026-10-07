# Audit checklists

This folder contains level-specific audit checklists.

Use the checklist that matches the audited solution level:

- [`l0-spike.md`](./l0-spike.md) — spike, experiment, one-off script.
- [`l1-internal-tool.md`](./l1-internal-tool.md) — internal tool or CLI utility.
- [`l2-reusable-library.md`](./l2-reusable-library.md) — reusable library.
- [`l3-application-ui.md`](./l3-application-ui.md) — application with UI or presentation layer.
- [`l4-hosted-service.md`](./l4-hosted-service.md) — hosted service, API, worker or DB-backed app.
- [`l5-distributed-platform.md`](./l5-distributed-platform.md) — distributed app or product platform.

## Checklist item format

For formal audits, express important checks with this structure:

```text
Checklist item:
- Check:
- Status: pass / concern / quality gate / not assessed / not applicable
- Evidence:
- Severity:
- Priority:
- Prescription:
```

## Status vocabulary

- `pass`: meets expectation for the audited level.
- `concern`: weakness exists but does not block current scope.
- `quality gate`: blocks acceptance or safe continuation.
- `not assessed`: no judgment because evidence was not gathered.
- `not applicable`: dimension does not apply to this solution or level.

## Prescription types

Use the prescription types from `../solution-audit-method.md`:

- preserve;
- stabilize;
- contain;
- align incrementally;
- refactor locally;
- extract;
- defer;
- rewrite.

## Rule

The checklist is a proportional quality lens, not a bureaucratic target.

Do not turn every missed item into a quality gate. Classify findings by impact, evidence and current scope.
