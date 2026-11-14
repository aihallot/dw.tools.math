# RS215 Attempt 001 Payload Operations

- Operations: 7
- Changed: 1
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS215 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS214/attempt-001.json` | run=RS214 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/engineering/InspectM8Net11SourceBaseline.cs` | bytes=5095 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS215 hierarchy=exact
- After SHA-256: `8cf6a8a61e9c5750da90d2b9ccca15631b203081f4c56d4edce34ebf1b425ed3`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS214/attempt-001.json`
- Summary: run=RS214 attempt=1 lineage=exact
- After SHA-256: `46712cba7d9ec60d3ce81304078d33d58ea6bb2fc780a4143746282b38528d8b`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/InspectM8Net11SourceBaseline.cs`
- Summary: bytes=5095
- Before SHA-256: `fa78e813364a717a38cd5e16041a2b2decd0067f11e9192673c8d3ae14b371b3`
- After SHA-256: `87d4ee7cc8e09e005f449c6137bd1bf4065296e2326f56a537e8217de0b39ba3`

