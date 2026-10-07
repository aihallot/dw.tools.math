# Audit method amendments

This file contains accepted audit-method amendments that are immediately applicable but not yet integrated into the canonical audit method.

Canonical baseline: `solution-audit-method.md` v0.4

Next planned integration: v0.5

## Purpose

The audit method should remain stable enough to use without constant micro-version churn.

When a useful audit rule is discovered after a stable audit method has been accepted, record it here instead of immediately reopening the full method.

Accepted audit amendments apply immediately. They are integrated into the canonical audit method during a planned integration pass.

## Status vocabulary

- `Proposed`: idea captured, not yet accepted.
- `Accepted`: applies immediately, not yet integrated into the canonical audit method.
- `Integrated`: moved into the canonical audit method.
- `Rejected`: kept for traceability but not applied.
- `Superseded`: replaced by a better amendment.

## How to use audit amendments

Use this file when:

- an audit touches an area covered by an accepted audit-method amendment;
- the user mentions a recent audit rule that has not yet been folded into `solution-audit-method.md`;
- auditing a solution against accepted engineering amendments;
- preparing the next planned audit-method integration pass.

Accepted audit amendments clarify or supplement the canonical audit method for their targeted area.

Do not use this file as a dumping ground for vague audit ideas. Only accepted, actionable audit-method corrections belong in the accepted amendments section.

## Integration policy

Do not create a new audit-method version for every typo, clarification, example or isolated micro-rule.

Create the next audit-method version when one of these is true:

- several accepted audit amendments have accumulated;
- an amendment changes audit behavior materially;
- a new audit section or major concept is needed;
- the skill entrypoint must change;
- a grouped neutral review is needed.

When integrating audit amendments:

1. Move accepted amendments into `solution-audit-method.md`.
2. Update the audit method version number.
3. Mark integrated amendments as `Integrated` here or move them to an integration history section.
4. Update templates, checklists or README files only if their operational guidance changes.
5. Recopy accepted audit references into the skill.

## Accepted amendments

### AA001 — Audits account for accepted engineering amendments

Status: Accepted\
Target version: v0.5\
Applies to: all audit modes and all project levels\
Target section: Audit entry classification / Evidence standard / Audit dimensions

#### Guidance

When auditing a software solution, accepted engineering amendments are part of the applicable guidance for their targeted area.

An auditor should consider `../engineering-guidance-amendments.md` when the audit scope touches an area covered by an accepted engineering amendment, especially build, test, publish, repository layout, workflow scripts or task runners.

A gap against an accepted engineering amendment is not automatically a quality gate. Classify it according to evidence, project level, risk, audit mode and current scope.

#### Recommended behavior

- Mention applicable accepted engineering amendments in the audit evidence basis or classification when they affect the audit scope.
- For quick audits, mention only accepted amendments that materially affect the continue/escalate decision.
- For standard, deep, release/readiness, reprise or regression audits, include applicable accepted amendments in the relevant dimension findings.
- Distinguish gaps against canonical guidance, repository-specific conventions and accepted amendments.
- Do not load or apply unrelated amendments that do not touch the audit scope.
- If an accepted engineering amendment changes the recommended next action, state that explicitly.

#### Rationale

Accepted engineering amendments apply immediately, even before they are integrated into the next canonical engineering guidance version.

Audits must therefore evaluate relevant pending amendments, otherwise a solution could appear compliant with the older canonical text while missing an accepted rule that already applies.

#### Integration note

Fold this into the next audit method version under applicable guidance, audit entry classification and evidence handling.

## Proposed amendments

None.

## Integration history

No audit amendments have been integrated yet.
