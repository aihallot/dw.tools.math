# Solution audits

This folder contains the solution audit method for evaluating software solutions according to complexity, risk, maturity and intended lifetime.

The audit material complements the engineering guidance. It helps decide whether a solution is healthy enough for its current level, safe to extend, ready for handoff, or in need of stabilization.

## Files

- [`solution-audit-method.md`](./solution-audit-method.md)\
  Canonical audit method. Read this for the concepts, evidence standard, finding classification, prescription model, anti-patterns, scoring guidance and final verdict rules.

- [`amendments.md`](./amendments.md)\
  Accepted audit-method amendments that apply immediately but have not yet been integrated into the next audit method version.

- [`quick-audit-protocol.md`](./quick-audit-protocol.md)\
  Short protocol for fast checks before continuing work.

- [`audit-report-template.md`](./audit-report-template.md)\
  Markdown template for standard, deep, release/readiness, reprise and regression audits.

- [`audit-scorecard-template.json`](./audit-scorecard-template.json)\
  Optional machine-readable JSON scorecard template for audits that need comparison over time.

- [`checklists/`](./checklists/)\
  Level-specific audit checklists from L0 to L5.

## Recommended usage

Use this folder as follows:

1. Start with the audit goal.
2. Choose an audit mode.
3. Identify the project level L0-L5.
4. Read accepted audit-method amendments when they affect the audit scope.
5. Read accepted engineering amendments when the audited area is covered by one.
6. Read the matching checklist.
7. Apply the evidence standard from the method.
8. Produce a verdict and the next smallest serious action.

For a quick sanity check, use only `quick-audit-protocol.md`, `amendments.md` when relevant, and the most relevant level checklist.

For a standard or deep audit, use `solution-audit-method.md`, `amendments.md`, the matching checklist, and `audit-report-template.md`.

For audits that need comparison over time, also produce a JSON scorecard using the template in `audit-scorecard-template.json`.

## Amendments

Accepted audit-method amendments apply immediately, but they do not create a new audit method version until a planned integration pass.

When auditing an engineering solution, also consider accepted amendments from `../engineering-guidance-amendments.md` when they apply to the audited area.

## Source of truth

The canonical editable audit source lives in `docs/engineering/audits/`.

This skill copy should be updated only after the canonical docs have been reviewed and accepted as fit for use.

When audit guidance changes:

1. Update the relevant file in `docs/engineering/audits/` first.
2. Re-read the updated documentation as files, not as conversation text.
3. Copy accepted audit references into this skill.
4. Update the skill entrypoint only if invocation behavior changes.

## Stop rule

Audit work should not become endless meta-work.

When the method is clear enough to guide real audits, use it on real projects and improve it from concrete experience.
