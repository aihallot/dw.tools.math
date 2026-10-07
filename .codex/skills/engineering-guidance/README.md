# Engineering guidance skill

This skill packages the engineering guidance as an agent-facing entrypoint.

The skill is intentionally short. The detailed guidance is copied into `references/` so the skill can remain operational while still being self-contained.

## Files

- `SKILL.md`\
  Agent entrypoint. Read this first when invoking the skill.

- `references/engineering-principles-dotnet-conventions.md`\
  Full canonical reference copied from `docs/engineering/`.

- `references/engineering-guidance-amendments.md`\
  Accepted guidance amendments copied from `docs/engineering/`. Read when a task touches an accepted pending amendment.

- `references/engineering-abstract.md`\
  Compact operational summary copied from `docs/engineering/`.

- `references/engineering-mantra.md`\
  Drift-correction reminder copied from `docs/engineering/`.

- `references/audits/`\
  Solution audit method, quick audit protocol, report template, scorecard template and L0-L5 checklists copied from `docs/engineering/audits/`.

## Amendment references

Use the amendments reference when an accepted rule applies immediately but has not yet been integrated into the canonical engineering guidance version.

This avoids unnecessary micro-version churn while keeping useful corrections available to agents.

## Audit references

Use the audit references when an agent needs to review an existing solution, decide whether it is safe to extend, prepare a handoff, check release/readiness, verify regression risk, or distinguish quality gates from follow-ups.

The canonical editable audit source remains in `docs/engineering/audits/`.

## Maintenance

The canonical editable source remains in `docs/engineering/`.

When guidance changes:

1. Update `docs/engineering/` first.
2. Recopy the changed files into `skills/engineering-guidance/references/`.
3. Update `SKILL.md` only if the operational entrypoint changes.

Do not let the skill copy drift from the canonical docs.
