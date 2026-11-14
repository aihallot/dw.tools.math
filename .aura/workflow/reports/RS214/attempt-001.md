# RS214 Attempt 001 Payload Operations

- Operations: 7
- Changed: 1
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS214 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS213/attempt-001.json` | run=RS213 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/engineering/InspectM8Net11SourceBaseline.cs` | bytes=3774 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS214 hierarchy=exact
- After SHA-256: `22d77d37b367c0b80446883045c397ac0031fed2cf9623d18f76851c0cf3e7a3`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS213/attempt-001.json`
- Summary: run=RS213 attempt=1 lineage=exact
- After SHA-256: `e242f051782e5be77e3edb4cfcdba3a94e409bd10e53ea5dcff8e937b8660976`

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
- Summary: bytes=3774
- After SHA-256: `fa78e813364a717a38cd5e16041a2b2decd0067f11e9192673c8d3ae14b371b3`

