---
schemaVersion: 1
documentId: "dw.tools.workflow.guidance.mantra"
documentVersion: 1
kind: "guidance"
title: "Workflow Mantra"
owner: "dw.tools.workflow"
status: "generated"
managedBy: "dw.tools.workflow/guidance-pack-v2"
sourceOfTruth: "src/dw.tools.workflow/GuidancePackV018Policy.cs#Mantra"
---
# Workflow Mantra

Read the active workflow mantra before repository-state analysis.

- GIGO first: educate the agent before adding another guard.
- The workflow-operating guidance selected by the executing tool is authority; repository guidance may be older, divergent, or candidate content.
- After workflow policy, read repository-owned `PROJECT-MANTRA.md` and `PROJECT-CONSTITUTION.md` when they exist. Project policy may constrain product development but must not redefine workflow mechanics.
- Treat every repository as an ordinary workflow consumer; product identity must not unlock hidden workflow behavior.
- Preserve product truth and classify adoption from evidence. Never invent history, backlog, progress, or support.
- Prepare one bounded run with one truthful objective and one pending run; correct the same run after failure.
- A `failed` verdict is evidence, never permission to change methodology just to get green.
- Validate the smallest likely-to-fail surface first: structural -> exact tests -> relevant packs -> broader certification.
- Before any required full suite, run the current delta and the last plausible fragile tests first. Full validation is a trust-boundary event, not a routine run cost.
- Use repository `planned` validation by default; use `explicit` only for a genuine planning gap or intentionally unusual boundary.
- Prefer guidance, skills, templates, examples, and clear errors over guard proliferation.
- Keep the operator loop simple: normally only `dwf run next`; do not transfer workflow debugging to the operator.
- If failures accumulate, stop patching symptoms and re-audit the whole run.

`docs/workflow/constitution.md` is the normative generic workflow policy. Re-read it at conversation or handover start, after an explicit philosophy/process correction, after abnormal retries, before methodology changes, and before broad qualification or promotion.
