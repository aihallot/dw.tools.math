# RS262 Attempt 001 Payload Operations

- Operations: 8
- Changed: 1
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS262 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS261/attempt-001.json` | run=RS261 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | pointer=/observation/packageVersion |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8Net11PublishedStatisticsBinary.cs` | bytes=6119 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS262 hierarchy=exact
- After SHA-256: `655be14956f6e91de002e0ab0d53da367daed900e170fc47bcf96e67e8fad63b`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS261/attempt-001.json`
- Summary: run=RS261 attempt=1 lineage=exact
- After SHA-256: `f1b8ba8a5fd822fe2a7ffa4b5182cf7656f99040e60a74a4712be8a9de1fa543`

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

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f27-statistics-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM8Net11PublishedStatisticsBinary.cs`
- Summary: bytes=6119
- After SHA-256: `b0edd4f86add560a780ef0e0b8eddb67374edf64dba3b7310a34bb7500f63566`

