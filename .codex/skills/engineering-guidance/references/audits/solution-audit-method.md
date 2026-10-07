# Solution audit method — v0.4

## Purpose

This method defines how to audit a software solution according to its complexity, risk, maturity and intended lifetime.

It complements the engineering guidance.

The engineering guidance answers:

> How should we build serious software?

The audit method answers:

> How healthy is this solution, what evidence supports that assessment, and what should be done next?

An audit must produce clarity, not automatic refactoring.

It identifies:

- what is already good;
- what should be preserved;
- what is unsafe, incorrect or fragile;
- what blocks further work;
- what should be improved soon;
- what can safely remain as follow-up;
- whether the solution is fit for its current level;
- whether it is safe to extend.

## Core principle

An audit is not permission to rewrite the world.

An audit classifies findings into:

- **strengths**: parts worth preserving;
- **quality gates**: issues that block acceptance for the audited scope;
- **important risks**: issues that should be addressed soon or explicitly accepted;
- **recommended improvements**: valuable but not blocking;
- **optional follow-ups**: nice to have, not urgent;
- **technical debt**: known weakness to track;
- **not assessed**: areas outside the audit evidence.

The output of an audit should help decide the smallest serious next action.

## Non-goals

An audit should not:

- expand the project scope by default;
- trigger a rewrite by default;
- turn every imperfection into a blocker;
- hide uncertainty;
- claim validation that was not performed;
- produce decorative scoring without actionable findings;
- criticize endlessly without recommending the next safe step;
- replace implementation work with endless evaluation;
- audit the whole repository when a bounded scope is enough.

The best audit produces a clear decision.

## 1. Audit modes

Choose the audit mode before starting.

| Mode | Use when | Output depth |
|---|---|---|
| Quick audit | Small change, early sanity check, low-risk orientation | Key risks, obvious quality gates, next action |
| Standard audit | Non-trivial feature, project reprise, refactor preparation | Full dimensions, level checklist, prescriptions |
| Deep audit | Complex, risky, long-lived or unstable solution | Evidence-heavy findings, architecture/data/security/test detail |
| Release/readiness audit | Before shipping, deployment, public use or handoff | Quality gates, operational readiness, validation status |
| Reprise audit | Before a new agent continues work | Current state, preserved behavior, workflow, next safe step |
| Regression audit | After refactor, merge or agent run | Behavior preservation, lost capabilities, validation gaps |

The audit mode controls depth. It does not lower safety requirements for high-risk areas.

## 2. Audit levels

Use the same project levels as the engineering guidance.

| Level | Solution type | Audit depth |
|---|---|---|
| L0 | Spike, experiment, one-off script | Safety, clarity, limits, no false claims |
| L1 | Internal tool or CLI utility | Reproducibility, input safety, diagnostics, core tests |
| L2 | Reusable library | API design, invariants, test coverage, dependency hygiene |
| L3 | Application with UI or presentation layer | Layering, user-facing behavior, localization, config, UI validation |
| L4 | Hosted service, API, worker or DB-backed app | Runtime config, persistence, migrations, security, observability, deployment readiness |
| L5 | Distributed app or product platform | Service boundaries, contracts, orchestration, resilience, observability, operations, handoff |

A small solution may still require a higher-risk audit if it touches dangerous areas.

Risk overrides size.

## 3. Evidence standard

Every important audit finding should be tied to evidence.

Use these evidence labels:

| Label | Meaning |
|---|---|
| Observed | Directly seen in files, docs, configuration, code, scripts, tests or outputs |
| Executed | Verified by running a command, test, script or tool |
| Inferred | Reasonable conclusion from available evidence, but not directly proven |
| Reported | Stated by user, docs or prior handoff, not independently verified |
| Not assessed | Outside current audit scope or evidence unavailable |

Do not present inferred findings as executed validation.

Do not mark something as safe only because no evidence of risk was found.

### 3.1 Evidence summary

Every standard, deep, release/readiness, reprise or regression audit should include an evidence summary.

```text
Evidence summary:
- Files inspected:
- Docs inspected:
- Tests inspected:
- Commands executed:
- Commands not executed:
- User-reported context:
- Inferences made:
- Not assessed:
```

For quick audits, a shorter evidence basis is enough.

## 4. Finding status, severity and priority

### 4.1 Finding status

Each finding should have a status.

| Status | Meaning |
|---|---|
| Pass | Meets expectation for the audited level |
| Concern | Weakness exists but does not block current scope |
| Quality gate | Blocks acceptance or safe continuation |
| Not assessed | No judgment because evidence was not gathered |
| Not applicable | Dimension does not apply to this solution or level |

### 4.2 Severity

Severity describes impact if the issue is real.

| Severity | Meaning |
|---|---|
| Critical | Could cause data loss, security exposure, broken core behavior or unsafe operation |
| High | Significant correctness, maintainability, operational or workflow risk |
| Medium | Important weakness, but bounded or recoverable |
| Low | Minor issue, polish, clarity or future maintainability concern |

### 4.3 Priority

Priority describes when to act.

| Priority | Meaning |
|---|---|
| P0 | Fix before continuing |
| P1 | Fix before release, handoff or major extension |
| P2 | Schedule soon |
| P3 | Track as follow-up |
| P4 | Optional improvement |

A critical issue is usually P0 or P1, but priority still depends on scope.

## 5. Audit entry classification

Before auditing, classify the target.

```text
Audit classification:
- Repository / solution:
- Scope:
- Audit mode:
- Project level:
- Intended use:
- Expected lifetime:
- Risk triggers:
- Existing workflow constraints:
- Main technologies:
- Data/persistence involved:
- Public contracts involved:
- User-facing surfaces:
- Deployment/runtime context:
- Validation available:
- Audit goal:
```

The audit goal matters.

Examples:

- decide whether a prototype can evolve;
- prepare a coding agent handoff;
- evaluate architecture health;
- check readiness before adding features;
- identify quality gates before release;
- decide whether to refactor incrementally or rewrite;
- evaluate whether previous agent work preserved behavior.

## 6. Audit process

### Step 1 — Establish scope

Define what is being audited.

Avoid auditing the entire repository when the actual question concerns one feature, project, package, service, script or workflow.

State explicitly:

- in scope;
- out of scope;
- assumptions;
- evidence sources.

### Step 2 — Classify level and risk

Determine project level L0-L5.

Then check risk triggers.

Risk triggers may increase audit depth even when project level is low.

### Step 3 — Gather evidence

Inspect relevant:

- README and project docs;
- architecture or workflow docs;
- source code;
- tests;
- scripts;
- configuration;
- CI or validation docs;
- generated artifacts;
- data formats and schemas;
- recent handoff/progress notes where available.

Run commands only when the environment and permissions allow it.

If commands are not run, state that clearly.

### Step 4 — Evaluate dimensions

Evaluate the relevant audit dimensions.

Do not force every dimension if it is not applicable.

Mark skipped dimensions as not applicable or not assessed.

### Step 5 — Apply level checklist

Use the level-specific checklist as a proportional quality lens.

The checklist is not a bureaucratic target. It is a way to avoid missing level-appropriate risks.

### Step 6 — Classify findings

Each finding should include:

- title;
- dimension;
- status;
- severity;
- priority;
- evidence;
- impact;
- recommended prescription.

### Step 7 — Produce verdict

End with a clear verdict and next action.

An audit without a decision is incomplete.

## 7. Audit dimensions

Evaluate only dimensions relevant to the level, mode and scope.

### 7.1 Functional preservation

Questions:

- What currently works?
- Are important capabilities preserved?
- Did recent changes silently remove behavior?
- Are known limitations explicit?
- Are representative usage scenarios covered?

Output:

- preserved capabilities;
- lost or uncertain capabilities;
- required regression checks.

### 7.2 Abstraction and layering

Questions:

- Are abstraction levels mixed?
- Are business rules inside UI, transport, scripts or persistence adapters?
- Are low-level details leaking into high-level workflows?
- Are application services clear use-case orchestrators or dumping grounds?
- Are endpoints/controllers boundary adapters rather than business owners?

Output:

- layering strengths;
- mixed-abstraction findings;
- suggested relocation of responsibilities.

### 7.3 Subsidiarity

Questions:

- Is each responsibility at the lowest correct level, but not lower?
- Is app-specific workflow pushed into generic libraries?
- Is reusable logic trapped in UI or scripts?
- Is configuration used for values that vary by deployment?
- Is user-facing text placed where it can be localized?

Output:

- misplaced responsibilities;
- duplication risks;
- over-generalization risks;
- correct ownership recommendation.

### 7.4 Scope and change discipline

Questions:

- Is the current change set coherent?
- Did the solution expand beyond the task?
- Are unrelated fixes mixed in?
- Are follow-ups separated from blockers?
- Are improvements being confused with quality gates?

Output:

- scope concerns;
- quality gates;
- follow-up candidates;
- recommended next smallest coherent change.

### 7.5 Configuration, constants and localization

Questions:

- Are secrets hard-coded?
- Are environment-specific values hard-coded?
- Are business thresholds configurable or intentionally constant?
- Is user-facing text localizable?
- Are resource keys or message abstractions stable enough?
- Is configuration validated early?

Output:

- hardcoded-value findings;
- config risks;
- localization readiness status.

### 7.6 Data, persistence and contracts

Questions:

- Is persisted data protected?
- Are migrations explicit and safe?
- Are file formats and schemas stable or versioned where needed?
- Are unknown/future fields preserved when appropriate?
- Are imports validated before overwrite?
- Are public contracts changed intentionally?

Output:

- data-safety risks;
- compatibility risks;
- migration risks;
- contract-breaking changes.

### 7.7 Security and privacy

Questions:

- What trust boundaries exist?
- Are inputs validated?
- Are paths normalized and constrained?
- Are secrets protected?
- Are logs safe?
- Are permissions minimal?
- Are shell/process/plugin/dynamic-code paths guarded?
- Are AI-generated inputs treated defensively?

Output:

- security quality gates;
- privacy risks;
- recommended safeguards.

### 7.8 Testing and validation

Questions:

- Are tests at the right level?
- Do tests cover domain rules, workflows, adapters, UI or scripts as appropriate?
- Are risky boundaries covered by negative tests?
- Is validation reproducible?
- Are claims of validation honest?
- Is coverage meaningful rather than theatrical?

Output:

- test gaps;
- validation gaps;
- commands to run;
- confidence level.

### 7.9 Build, run, publish and operations

Questions:

- Can the solution be restored, built, tested, run and published reproducibly?
- Are scripts safe and path-aware?
- Is cleanup guarded?
- Are build/publish outputs placed correctly?
- Are Docker/Aspire used where meaningful and not as ceremony?
- Are health checks, logs and startup validation present when needed?

Output:

- workflow risks;
- operational readiness;
- missing scripts or docs.

### 7.10 Documentation and handoff

Questions:

- Can a future agent understand the solution state?
- Are architecture decisions documented where needed?
- Are progress and roadmap docs current?
- Are lessons learned captured?
- Are handoff instructions accurate?
- Is there a single source of truth?

Output:

- documentation strengths;
- stale/conflicting docs;
- handoff readiness.

## 8. Checklist item format

When producing a detailed audit, each checklist item should use this structure.

```text
Checklist item:
- Check:
- Status: pass / concern / quality gate / not assessed / not applicable
- Evidence:
- Severity:
- Priority:
- Prescription:
```

For quick audits, do not expand every item. Mention only relevant passes, concerns and quality gates.

For standard, deep, release/readiness, reprise and regression audits, use this format for important findings or for all checklist items when a formal report is needed.

## 9. Prescription model

Audit findings should lead to prescriptions.

A prescription is not just "fix this". It classifies the next action.

Prescription types:

- **Preserve**: this part is good; avoid damaging it.
- **Stabilize**: fix quality gates before new features.
- **Contain**: isolate risky behavior behind a safer boundary.
- **Align incrementally**: improve touched areas toward conventions.
- **Refactor locally**: restructure a bounded area.
- **Extract**: move reusable capability to a better owner.
- **Defer**: record as follow-up, not blocking.
- **Rewrite**: only when incremental repair is more dangerous or expensive than replacement.

Each prescription should include:

```text
Prescription:
- Finding:
- Type:
- Priority:
- Blocks completion: yes/no
- Suggested action:
- Validation expected:
- Follow-up owner/location:
```

## 10. Rewrite guidance

Rewrite should be rare.

Prefer stabilization or incremental alignment unless evidence shows that the current structure blocks safe progress.

Rewrite may be justified when:

- core behavior cannot be preserved safely through incremental changes;
- the architecture prevents essential validation;
- security or data-safety flaws are structural;
- the solution is mostly throwaway prototype code being promoted beyond its level;
- the cost of understanding and safely patching exceeds bounded replacement cost.

Rewrite is not justified merely because:

- the code is not elegant;
- naming is imperfect;
- architecture is not ideal;
- tests are incomplete but addable;
- a cleaner design is imaginable.

## 11. Audit anti-patterns

Avoid these audit failures.

### 11.1 Auditing without evidence

Bad audit behavior:

- giving confident judgments without reading relevant files;
- assuming test status from project structure;
- claiming validation without execution;
- treating absence of evidence as evidence of safety.

Correction:

- label evidence honestly;
- distinguish observed, executed, inferred, reported and not assessed;
- make uncertainty visible.

### 11.2 Turning every weakness into a blocker

Bad audit behavior:

- classifying all imperfections as quality gates;
- blocking progress on optional polish;
- treating architectural ideals as immediate requirements.

Correction:

- reserve quality gates for issues that block acceptance or safe continuation;
- classify non-blocking issues as concerns, risks, technical debt or follow-ups.

### 11.3 Recommending rewrite too easily

Bad audit behavior:

- recommending rewrite because the code is inelegant;
- ignoring preserved behavior;
- underestimating replacement risk;
- using audit as a pretext for architectural preference.

Correction:

- prefer stabilization, containment or local refactoring;
- require evidence that incremental repair is unsafe or more expensive than bounded replacement.

### 11.4 Expanding scope during audit

Bad audit behavior:

- auditing the whole repository when the scope is one feature;
- mixing audit with unrelated cleanup;
- turning a quick audit into a deep audit without reason.

Correction:

- establish scope first;
- escalate depth only when risk or uncertainty justifies it;
- keep unrelated findings as follow-ups.

### 11.5 Ignoring what works

Bad audit behavior:

- focusing only on flaws;
- failing to identify strengths to preserve;
- recommending changes that may break working behavior.

Correction:

- list strengths explicitly;
- identify preserved capabilities;
- make preservation part of the prescription.

### 11.6 Scoring without decision

Bad audit behavior:

- producing a scorecard without verdict;
- averaging away a serious quality gate;
- using numbers as a substitute for judgment.

Correction:

- scores are optional;
- quality gates override averages;
- every audit ends with a verdict and next action.

### 11.7 Mixing audit and implementation

Bad audit behavior:

- starting fixes before completing the audit verdict;
- changing files while still discovering scope;
- silently implementing recommendations as if they were already approved.

Correction:

- finish the audit first;
- separate findings from prescriptions;
- implement only after the next action is clear or explicitly requested.

## 12. Audit verdict

Every audit should end with a clear verdict.

```text
Audit verdict:
- Fit for current level: yes / partial / no
- Safe to extend: yes / with conditions / no
- Requires stabilization before new features: yes / no
- Main quality gates:
- Main risks:
- Recommended next action:
- Rewrite recommended: no / partial / yes, with justification
```

An audit without a verdict is incomplete.

## 13. Audit scoring

Scoring is optional.

When useful, use a lightweight scorecard.

Avoid fake precision.

Recommended scale:

| Score | Meaning |
|---|---|
| 0 | Not assessed |
| 1 | Serious issue |
| 2 | Weak |
| 3 | Acceptable for level |
| 4 | Good |
| 5 | Excellent |

Suggested dimensions:

```text
Scorecard:
- Functional preservation:
- Abstraction/layering:
- Subsidiarity:
- Scope control:
- Configuration/localization:
- Data/contracts:
- Security/privacy:
- Testing/validation:
- Build/run/operations:
- Documentation/handoff:
- Overall fit for level:
```

Scores should never hide quality gates.

A solution can have a good average score and still be blocked by one serious quality gate.

## 14. JSON scorecard usage

The JSON scorecard is optional.

Use it when audits need to be compared, tracked or reassessed over time.

Produce a JSON scorecard for:

- standard audits when the project is likely to be reassessed;
- deep audits;
- release/readiness audits;
- reprise audits for long-lived projects;
- regression audits after major refactors or agent runs;
- project evaluations that will be compared over time.

Do not produce JSON by default for:

- quick audits;
- small one-off reviews;
- purely conversational assessments;
- audits where no stable score is meaningful.

The JSON scorecard is not a substitute for the Markdown audit report.

## 15. Output expectations by audit mode

### Quick audit

Use [`quick-audit-protocol.md`](./quick-audit-protocol.md).

### Standard audit

Use the full audit report template.

Include:

- evidence summary;
- findings by relevant dimensions;
- level checklist result;
- prescriptions;
- verdict.

JSON scorecard is optional but recommended if the solution will be reassessed.

### Deep audit

Use the full audit report template.

Include:

- detailed evidence;
- detailed findings;
- explicit not-assessed areas;
- prescriptions by priority;
- JSON scorecard.

### Release/readiness audit

Focus on:

- quality gates;
- validation executed;
- validation missing;
- operational readiness;
- data/contract/security risks;
- go/no-go verdict.

JSON scorecard recommended.

### Reprise audit

Focus on:

- current state;
- what works;
- what must be preserved;
- workflow constraints;
- quality gates;
- next safe task;
- handoff readiness.

JSON scorecard recommended for long-lived projects.

### Regression audit

Focus on:

- expected preserved behavior;
- observed preserved behavior;
- lost or uncertain behavior;
- validation commands;
- rollback/stabilization needs;
- next action.

JSON scorecard optional unless this is part of recurring project tracking.

## 16. Versioning and maintenance

Use lightweight versioning.

Update the version when:

- audit modes change;
- evidence standards change;
- level checklists change materially;
- report or JSON templates change;
- prescription categories change.

Do not update the version for typo-only changes.

When using the method in a real audit, record:

- audit method version;
- audit date;
- audit scope;
- evidence basis;
- verdict;
- main prescriptions.

This makes future regression or reprise audits comparable.

## Final rule

Audit to clarify, not to endlessly criticize.

The best audit produces the next smallest serious action.

If the solution is fit for its level, say so.

If it is not, identify the quality gates and prescribe the smallest safe path forward.
