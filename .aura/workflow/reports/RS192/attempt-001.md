# RS192 Attempt 001 Payload Operations

- Operations: 9
- Changed: 1
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS192 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS191/attempt-001.json` | run=RS191 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 8 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/owner |
| 9 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/DescribeM8Discovery.cs` | bytes=5127 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS192 hierarchy=exact
- After SHA-256: `335f9d878ab07d73ddac50016b2b7f88012f754ad3117451031857e7f86a71f4`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS191/attempt-001.json`
- Summary: run=RS191 attempt=1 lineage=exact
- After SHA-256: `f76d7d29b58d3f3c69cbd1481fe1181465a23d9e30d9491f54a7c362e41d46c3`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C02 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-discovery-manifest.json`
- Summary: pointer=/schemaVersion

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-discovery-manifest.json`
- Summary: pointer=/owner

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/DescribeM8Discovery.cs`
- Summary: bytes=5127
- After SHA-256: `5ddb8c2de645b74bd3c091f312197597c115666c6671151414091372f711a2d7`

