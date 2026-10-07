# Solution audit report template

Use this template for standard, deep, release/readiness, reprise and regression audits.

For quick audits, use [`quick-audit-protocol.md`](./quick-audit-protocol.md).

## Report sections

Use these sections for a full audit report:

1. Scope
2. Audit classification
3. Evidence summary
4. Executive summary
5. Strengths to preserve
6. Quality gates
7. Important risks
8. Findings by dimension
9. Level-specific checklist result
10. Prescriptions
11. Follow-up backlog
12. Audit verdict
13. Recommended next action

## Findings by dimension

Include only dimensions that are relevant to the level, mode and scope.

- Functional preservation
- Abstraction and layering
- Subsidiarity
- Scope and change discipline
- Configuration, constants and localization
- Data, persistence and contracts
- Security and privacy
- Testing and validation
- Build, run, publish and operations
- Documentation and handoff

## Evidence summary fields

- Files inspected
- Docs inspected
- Tests inspected
- Commands executed
- Commands not executed
- User-reported context
- Inferences made
- Not assessed

## Finding fields

- Title
- Dimension
- Status: pass / concern / quality gate / not assessed / not applicable
- Severity: critical / high / medium / low
- Priority: P0 / P1 / P2 / P3 / P4
- Evidence
- Impact
- Prescription
- Blocks acceptance: yes/no

## Checklist item fields

- Check
- Status: pass / concern / quality gate / not assessed / not applicable
- Evidence
- Severity
- Priority
- Prescription

## Audit verdict fields

- Fit for current level: yes / partial / no
- Safe to extend: yes / with conditions / no
- Requires stabilization before new features: yes / no
- Main quality gates
- Main risks
- Recommended next action
- Rewrite recommended: no / partial / yes, with justification

## Output expectations by audit mode

### Standard audit

Include evidence summary, findings by relevant dimensions, level checklist result, prescriptions and verdict.

JSON scorecard is optional but recommended if the solution will be reassessed.

### Deep audit

Include detailed evidence, detailed findings, explicit not-assessed areas, prescriptions by priority and JSON scorecard.

### Release/readiness audit

Focus on quality gates, validation executed, validation missing, operational readiness, data/contract/security risks and release verdict.

JSON scorecard recommended.

### Reprise audit

Focus on current state, what works, what must be preserved, workflow constraints, quality gates, next safe task and handoff readiness.

JSON scorecard recommended for long-lived projects.

### Regression audit

Focus on expected preserved behavior, observed preserved behavior, lost or uncertain behavior, validation commands, stabilization needs and next action.

JSON scorecard optional unless this is part of recurring project tracking.
