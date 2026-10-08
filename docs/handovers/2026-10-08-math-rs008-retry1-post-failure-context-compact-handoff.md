# dw.tools.math — RS008 retry-1 post-failure compact handover

Date: 2026-10-08  
Repository: `aihallot/dw.tools.math`  
Canonical branch: `main`  
Local operator directory: `D:\\data.dev\\github\\ai@hallot.net\\dw.tools.math`  
Handover branch: `handover/math-rs008-retry1-post-failure-20261008-context-compact`

## Resume rule

Read this file completely first. Then inspect GitHub `main` once and treat committed GitHub state plus newer durable DWF evidence as authority. Do not reconstruct RS001-RS007 history unless a specific inconsistency requires it.

Normal interaction loop:
1. consume the operator verdict (`pushed` / `failed`);
2. inspect durable remote evidence for that exact run;
3. on failure, correct the same run only;
4. on success, verify the result and prepare the next truthful run;
5. exception: answer a precise user question or stop at a real cycle/owner-decision boundary.

Do not ask for terminal output when durable evidence exists. Never claim that local execution happened; the operator executes `dwf run next`.

## Conversation and repository language

- User/assistant conversation: French.
- Canonical repository/project content: English.
- Preserve lowercase project/file identities such as `dw.tools.math.slnx`, `.csproj` names and project paths. Normal C# namespaces/types may remain PascalCase.

## DWF authority and authoring rules

- Executable: `dw.tools.workflow 0.1.58`.
- Embedded guidance: `0.22`.
- Read first when methodology is in doubt:
  - `docs/workflow/mantra.md`
  - `docs/workflow/constitution.md`
  - `docs/workflow/bootstrap.md`
  - then the relevant authoring/reference document.
- Product backlog `docs/planning/backlog.json` is accepted product authority.
- Native DWF plan `.aura/workflow/plan/project.json` is the executable projection.
- Every payload must update product progress and native DWF progress in the same controlled mutation.
- Achievements must equal exactly the nodes that transition to `done`.
- One pending run maximum.
- `contribution.backlogItems` refers to DWF's internal `BL####` backlog, not product IDs; normally omit it.
- Preparation-safe validations must be observational. Generators/canonical renderers/intended mutations belong in the payload.
- Use typed Payload SDK mutations; do not reimplement repository mutation with direct filesystem writes.
- After any failure, re-audit the complete run material before handing the operator another command.
- Do not expand or redesign DWF in this repository.
- Do not write to AURA, Decision, MCDM, or brainstorming sibling repositories.
- Owner-mediated coordination requests may be prepared in Math, but never claim sent/accepted/implemented without external evidence.
- Prefer bounded substantive runs; avoid tiny administrative runs and failure-inducing guard proliferation.

## Operator handoff contract

Whenever handing the operator a run command, end the response with exactly these six non-empty lines and no text after them:

**DIRECTORY:** `D:\data.dev\github\ai@hallot.net\dw.tools.math`  
**BRANCH:** `main`  
**PREPARED TIP:** `<exact SHA>`  
**EXPECTED COMMIT:** `<exact result commit message>`  
**STATUS:** `<run-id> · retry <n> · attempts alloués <allocated-attempt-count>`  
**COMMAND:** `<authority-derived command>`

A concise `Run terminé : ...` immediately before the six lines is allowed.

Retry semantics:
- use terminal-resume `run.retry`, never infer retry from guesswork;
- if terminal says `retry N`, next handoff is `retry N+1`;
- `attempts alloués` is separate and counts actual allocated attempts.

## Durable product state before RS008

M0 is complete.

RS007 succeeded:
- result commit: `7655179adc71d972f13444506bff08f3cc869b8d`
- commit message: `test: establish exact-rational RED contract`
- report: `.aura/workflow/reports/RS007/attempt-001.json`
- outcome: succeeded
- attempt: 1
- boundary violations: none
- completed node: `M1-W01-C01-T1-R`

Current durable product/native state:
- backlog version: `0.1.6`
- M1: `in_progress` / native `in-progress`
- M1-W01: `in_progress` / native `in-progress`
- M1-W01-C01: `in_progress` / native `in-progress`
- M1-W01-C01-T1: `in_progress` / native `in-progress`
- T1-R: `done`
- T1-G: `ready`
- T1-V: planned / native `not-ready`
- T2: planned / native `not-ready`

RED evidence:
- `docs/planning/evidence/M1-W01-C01-red.json`
- status `expected-red-observed`
- exact source path: `lib/dw.quantities/ExactRational.cs`
- pinned source SHA-256: `3c8d3beaa3b04884ca8424b5bbc6ee5f2d87b4adf1f6e329a7047bd974b1405a`
- accepted GREEN behavior cases:
  - `1/3 + 1/6 = 1/2`
  - `-1 1/2 = -3/2`
  - zero denominator rejected
  - `2/-4` canonicalizes to `-1/2`

## Source rights and AURA boundary

Owner attestation is durable:
- `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- user owns the selected AURA code;
- Math is authorized to copy, modify, package, and redistribute the selected mathematical subset;
- ownership model: owner-authorized proprietary code, not an invented open-source license;
- no additional owner exclusion beyond the established Math boundary.

Pinned AURA baseline:
- revision recorded by Math: `82a6b435a387a7e116b47a6b2c433ae9e067bf21`
- observed local root: `D:/data.dev/gitlab/AURA_projects/aura`
- source baseline is a selected working-tree byte snapshot, not a clean-tree claim.

Important: GitHub repository `aihallot/aura` is NOT the same usable baseline: the pinned commit and the `lib/dw.quantities/...` paths were not present there. Do not substitute that mirror.

`MATH-XR-001` remains draft / transmission none. No AURA-side progress is implied.

## Current run — RS008

Run id: `RS008`  
Title: `Transfer and verify ExactRational GREEN`  
Expected result commit: `feat: transfer and verify ExactRational`

Current `main` / prepared material before this handover:
- `5269fea1a580dd9ab83329d06e29453e0d93c35e`
- message: `chore(workflow): finalize RS008 preparation`
- `.aura/workflow/state.json`: `pendingRun = null`
- active native hierarchy remains M1 / M1-W01 / M1-W01-C01 / T1.

Current request intent:
- install the exact authorized `ExactRational.cs` bytes only after SHA-256 verification;
- add `dw.quantities` + its test project to `dw.tools.math.slnx`;
- qualify T1 GREEN and verify;
- complete exactly:
  - `M1-W01-C01-T1-G`
  - `M1-W01-C01-T1-V`
  - `M1-W01-C01-T1`
- ready `M1-W01-C01-T2` and `T2-R`, but do not claim them done.

Mutation boundary currently declared:
- `dw.tools.math.slnx`
- `src/projects/dw.quantities/ExactRational.cs`
- `tests/projects/dw.quantities.tests/M1W01C01Tests.cs`
- `docs/planning/ValidateM1W01C01Green.cs`
- `docs/planning/evidence/M1-W01-C01-green.json`
- planning projections/backlog
- native DWF plan.

## RS008 failure history

### Retry 0 — pre-attempt, no attempt allocated

Evidence:
- ref `refs/heads/workflow/evidence/preselection-rs008/run-next-20261008T052809Z-72905a81d9c3c1c3`
- commit `6907087851a17289925a98086760eab10c35e6d0`

Failure:
- payload-authoring preflight rejected direct `Directory.CreateDirectory` / direct filesystem mutation.
- Correct methodology: repository mutation must use typed Payload SDK operations.
- This was already corrected before retry 1.

### Retry 1 — latest failure, pre-attempt, no attempt allocated

Evidence:
- ref `refs/heads/workflow/evidence/preselection-rs008/run-next-20261008T060537Z-c82d97b3a835b8e6`
- commit `aab49a60ccfd66f0efcd72f3091cf5fc8b982f2f`
- terminal resume commit `c341a1a8b981cff97adcac277ed2341f912486fa`

Terminal projection:
- `run.retry = 1`
- `attemptAllocated = false`
- therefore, after correction, next operator handoff must be `RS008 · retry 2 · attempts alloués 0`.

Failure:
`M1-W01-C01 GREEN invalid: ExactRational GREEN tests failed ... Failed ExactRationalTypeIsMaterialized`

The run reached the GREEN test after the payload had:
- found the configured AURA observed root;
- found `lib/dw.quantities/ExactRational.cs`;
- verified the pinned SHA-256;
- installed the source through Payload SDK;
- built far enough to execute `dw.quantities.tests.dll`.

The staged test currently hard-codes:
`private const string TypeName = "Dw.Quantities.ExactRational";`
and:
`Assembly.Load("dw.quantities").GetType(TypeName, throwOnError: true, ignoreCase: false)`.

This full type identity was never established from the source baseline. High-probability diagnosis: the test oracle guessed the namespace/type identity incorrectly. Do NOT rename or rewrite the transferred source to make the guessed name pass.

## First action in the new conversation

Correct RS008, not a successor.

1. Read this handover fully.
2. Inspect GitHub `main` once and only newer durable evidence than the latest RS008 failure.
3. Inspect current RS008 request/payload/staged GREEN test.
4. Establish the real compiled `ExactRational` identity before changing source:
   - preferred: inspect the authorized source text if a safe read-only route is available;
   - otherwise make the reflection oracle discover the unique type whose simple name is `ExactRational` (or enumerate candidate exported/assembly types) and record the actual full name.
5. Preserve the exact source bytes and SHA-256. Do not alter source namespace/casing merely to satisfy the test.
6. Re-check the rest of the reflection oracle against the actual public API; do not guess constructors/properties/parsers when source/reflection can establish them.
7. Correct the same RS008 material.
8. Re-audit payload authoring rules, mutation boundary, target re-entry, exact achievements, and validations.
9. Publish all material changes first and `.workflow/next-run/request.json` as the final preparation write.
10. Verify terminal retry semantics; expected next retry is 2 unless newer durable evidence says otherwise.
11. Hand the operator only the authority-derived `dwf run next` command using the exact six-line ending.

Do not prepare RS009 until RS008 is durably pushed and verified.

## Methodological lessons already paid for

Do not repeat these failures:
- unsupported validation classification `behavioral`; valid DWF classes are the guidance-defined set such as `structural`, `focused`, `integrated`, `full` and preparation variants;
- `ActivateReadyContinuation` requires an already-active hierarchy; when M1 started after completed M0, explicit `ready -> in-progress` transitions were required;
- direct filesystem repository mutation inside payload is rejected; use typed Payload SDK;
- validators must be monotone across later planning progress, not frozen to one run's terminal state;
- product backlog evidence arrays are required for done nodes;
- completed DWF nodes require exactly matching achievements.

## Minimal historical anchors

Only use these if needed:
- RS006 result: `35b9cc2fd0e719e80715a9b48c2798a55b9df568`, M0 closed, M1 readied.
- RS007 result: `7655179adc71d972f13444506bff08f3cc869b8d`, RED established.
- Current RS008 prepared material: `5269fea1a580dd9ab83329d06e29453e0d93c35e`.
- Latest RS008 failure evidence: `aab49a60ccfd66f0efcd72f3091cf5fc8b982f2f`.
- Latest terminal resume: `c341a1a8b981cff97adcac277ed2341f912486fa`.

GitHub `main` and any newer durable DWF evidence always override this handover.
