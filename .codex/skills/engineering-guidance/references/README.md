# Engineering guidance

This folder contains the engineering guidance used to orient AI-assisted software work.

The documents are complementary. They should not be read as three competing sources of truth.

## Files

- [`engineering-principles-dotnet-conventions.md`](./engineering-principles-dotnet-conventions.md)\
  Full canonical reference. Use it when starting a serious project, defining repository conventions, resolving ambiguity, reviewing architecture, or deciding whether a rule is a quality gate or a follow-up.

- [`engineering-abstract.md`](./engineering-abstract.md)\
  Compact operational summary. Use it before non-trivial work, during project reprise, or when an agent needs the core rules without rereading the full reference.

- [`engineering-mantra.md`](./engineering-mantra.md)\
  Short drift-correction reminder. Use it when the work starts to over-expand, mix abstraction levels, ignore validation, lose scope, or fall into endless refinement.

## Recommended usage

For a small or low-risk task, read the mantra or abstract and apply the principles proportionally.

For a non-trivial task, use the abstract to classify the work before implementation:

- project level;
- risk triggers;
- planning granularity;
- completion criteria;
- quality gates;
- touched layers;
- abstraction and subsidiarity level;
- validation strategy;
- follow-up bucket.

For a complex, long-lived, risky, architectural, workflow-driven, or ambiguous task, read the full reference.

## Source of truth order

When guidance conflicts, use this order:

1. Explicit user instruction for the current task, as long as it is safe.
2. Repository-specific conventions and workflow docs.
3. `engineering-principles-dotnet-conventions.md`.
4. `engineering-abstract.md`.
5. `engineering-mantra.md`.

The abstract and mantra are derived from the full reference. They are meant to accelerate use, not override it.

## Updating these files

When the engineering guidance evolves:

1. Update the full canonical reference first.
2. Then update the abstract if the core operational summary changed.
3. Then update the mantra only if the drift-correction reminders changed.

Do not keep refining these files endlessly. Once the guidance is fit for use, record non-blocking ideas as follow-ups and move back to project work.
