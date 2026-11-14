# RS216 Attempt 001 Payload Operations

- Operations: 7
- Changed: 1
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS216 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS215/attempt-001.json` | run=RS215 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8Sdk11Compiler.cs` | bytes=3066 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS216 hierarchy=exact
- After SHA-256: `751499f1c947913d03ffe50dabfa4a7f13d3ef4d4abc6b53fc8539ae471a723b`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS215/attempt-001.json`
- Summary: run=RS215 attempt=1 lineage=exact
- After SHA-256: `b3c9d31d9a6cfeaa34e882c6a33e7eed45383bf7dd2bdf17ac29df8ba690ded8`

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
- Path: `docs/engineering/ProbeM8Sdk11Compiler.cs`
- Summary: bytes=3066
- After SHA-256: `20f0d4956b24f6bed71877300f9424fe547be09036488cef024893f04c9dc064`

