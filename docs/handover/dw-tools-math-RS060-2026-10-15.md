# dw.tools.math — compact recovery handover, 2026-10-15

**Purpose:** restart the operator loop with minimal context. This document is a handover, not a DWF execution receipt, product-qualification gate or authority that supersedes live GitHub main.

## Execution identity and policy

- Private repository: **aihallot/dw.tools.math**, branch **main**.
- Operator checkout: D:\data.dev\github\ai@hallot.net\dw.tools.math
- Command: **dwf run next**; never queue more than one pending run.
- Last observed DWF executor: **0.1.59**. Recheck current guidance if it changes.
- Owner dialogue in French; repository source, docs, names, comments, evidence and commits in English.
- Math changes only. Do not modify sibling decision, mcdm, dw.tools/workflow, neutral dw.discovery or AURA repositories. No mandatory Python/CAS, paid CI or public NuGet publication.

## Last verified durable result — RS059

GitHub product commit: **900da9f9dbbb55e1e1ba6dadd3d459f9d1dd83b4**.
Message: feat: qualify exact formal polynomial differentiation and real function domains.

DWF RS059 attempt 1: **succeeded on first invocation**, no prior preparation/attempt failures. Backlog **0.1.58**. Native plan: M4 in-progress, M4-W01 done, M4-W02 in-progress, M4-W02-C01 in-progress; T1 and its R/G/V children done; T2 not-ready. Active state before RS060: milestone M4, work M4-W02, phase M4-W02-C01, no pending run.

Scientific receipt: docs/planning/evidence/M4-W02-C01-T1-contract-qualified.json. Symbolics package 0.5.0-preview.5 qualified a provider-neutral exact real polynomial derivative: d(x^3)/dx = 3*x^2, zero derivative for independent target, formal real domain/codomain, with explicit unsupported status for units, conditional sources, complex/unqualified operations. One real RED recovered, 38 GREEN tests and eight isolated NuGet consumer probes. This is **not** a general CAS, public release, or unit-derivative capability.

## RS060 prepared scope — execution not yet claimed

Current prepared material will be on live main: .workflow/next-run/request.json and .workflow/next-run/payload.cs, with staged source in .workflow/next-run/files and helper in .workflow/next-run/checks. **The GitHub main tip after the request-only final commit is the prepared execution SHA; obtain it live**.

RS060 expected product commit: **feat: qualify exact pointwise derivative domains and close M4 W02 C01**.

Scope: complete M4-W02-C01-T2 R/G/V, T2 and C01, leaving work package M4-W02 and milestone M4 in-progress. C02/C03 remain not-ready. Accepted addition is an exact-rational, pointwise derivative contract in local symbolics preview.6. It evaluates the **previously qualified polynomial derivative** at exact rational points, such as d(x^3)/dx at x=2 = 12, with no finite differences. A small independent real-domain boundary recognizes d|x|/dx = +1 above zero, -1 below, undefined at zero; sqrt(x^2) follows the same real shape. Sqrt(x) is outside real domain for x<0, has no finite two-sided derivative at zero, and stays unsupported elsewhere until a general radical rule is independently qualified. The original rational hole (x^2-1)/(x-1) at x=1 stays excluded. Units, arbitrary quotients, retained restrictions and numerical approximations are not silently admitted.

Required scientific gate: **one compiled absent-API RED + 37 GREEN = 38 targeted tests**, relevant symbolics/IR/quantities regressions, eight isolated package-only NuGet consumer probes, three local SHA-256/manifest inspected packages. These checks are **proposed only** until the operator runs DWF successfully.

## Minimal sources at bootstrap, in authority order

1. Active workflow mantra and constitution, repository docs/workflow/mantra.md and docs/workflow/constitution.md; PROJECT-MANTRA.md and PROJECT-CONSTITUTION.md.
2. Live main HEAD, .workflow/next-run/request.json and .workflow/next-run/payload.cs. Inspect only affected staged files, not old run history.
3. docs/planning/backlog.json — M4-W02-C01 and T2; .aura/workflow/plan/project.json; .aura/workflow/state.json.
4. docs/planning/evidence/M4-W02-C01-T1-contract-qualified.json and .aura/workflow/reports/RS059/attempt-001.json.
5. For the next owner verdict, fetch only **newer** RS060 durable success report or preselection evidence. Their authority supersedes this snapshot.

## Operator loop and recovery contract

Every ordinary interaction **prepares or corrects a run script**, except a specific owner question or an important completed-cycle decision. On **pushed**, verify the actual product commit, RS060 result report, native DWF and product progress and scientific receipts. Only on confirmed RS060 success, prepare the accepted next RS061 for M4-W02-C02-T1 — bounded exact symbolic equation solving — after reading its current acceptance recipe. On **failed**, locate the newest workflow/evidence/preselection-rs060 branch or allocated-attempt report and classify the failure precisely: pre-attempt, allocated attempt, mutation boundary, delivery or internal. Correct **the same RS060**, preserve attempt numbering and report the exact retry. If failures accumulate, audit the complete payload, target re-entry, scientific tests, NuGet consumer, and evidence/progress claims before another execution.

Never reinterpret failure as permission to change DWF methodology or modify workflow/sibling repositories. Never ask the operator to edit workflow authority manually. Publish request.json **as the last GitHub write, in a commit whose only changed file is that request**; verify main HEAD, expected result commit, staged assets and state. Do not fabricate an attempt or a preflight pass.

Past fragile cases worth checking without reconstructing history: incorrect BudgetExceeded vs Unsupported outcomes, nullable C# consumer compile failures, mismatched target evidence keys, and missing aggregate evidence for completed work packages.

After a prepared or corrected run, end the reply with this verified operator contract:

    DIRECTORY: D:\data.dev\github\ai@hallot.net\dw.tools.math
    BRANCH: main
    PREPARED TIP: <verified current GitHub 40-char main SHA>
    EXPECTED COMMIT: <actual request commit message>
    STATUS: RSnnn — retry N prepared — attempts allocated X — not executed
    COMMAND: dwf run next

Do not embed a stale prepared-tip SHA in this document: the definitive tip will be the later final request commit on main.
