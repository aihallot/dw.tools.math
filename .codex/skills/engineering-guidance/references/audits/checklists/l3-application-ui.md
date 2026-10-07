# L3 — Application with UI or presentation layer audit checklist

## Use for

Use this checklist for applications with UI, presentation, CLI/TUI screens, API boundary surfaces or user-facing workflows.

A L3 solution should keep business behavior outside presentation code and treat user-facing behavior seriously.

## Expected checks

- Business logic is outside UI components/pages/screens.
- Application workflows are separated from presentation details.
- User-facing text is localization-ready.
- Configuration uses typed or centralized settings.
- UI behavior is validated where practical.
- Accessibility is considered for relevant surfaces.
- Error messages are useful and safe.
- Existing user-facing behavior is preserved.

## Common quality gates

- Core business rule implemented only in UI.
- Lost user-facing capability after refactor.
- Hard-coded user-facing text everywhere in a mature app.
- Unvalidated critical workflow.
- Configuration scattered or unsafe.
- Sensitive details exposed in user-facing errors.

## Typical prescriptions

- **Preserve** working user-facing behavior during refactor.
- **Stabilize** critical workflows before adding presentation polish.
- **Refactor locally** business rules out of UI components or boundary handlers.
- **Align incrementally** user-facing text toward localization resources or message abstractions.
- **Contain** unsafe error details behind safe presentation messages.
- **Defer** visual polish when validation, behavior preservation or layering is not yet secure.

## Escalation triggers

Escalate beyond L3 when:

- the app becomes hosted, multi-user, public or data-backed;
- it introduces persistence, migrations, background workers or external integrations;
- it handles secrets, personal data or authentication;
- operational readiness matters.
