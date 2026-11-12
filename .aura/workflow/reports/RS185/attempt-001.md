# RS185 Attempt 001 Payload Operations

- Operations: 11
- Changed: 2
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS185 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS184/attempt-001.json` | run=RS184 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/RS131-portable-protocol-qualified.json` | pointer=/total_cases |
| 8 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj` | bytes=571 |
| 9 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.discovery-bindings/Program.cs` | bytes=6099 |
| 10 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-selected-clr-bindings.json` | pointer=/status |
| 11 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-selected-clr-bindings.json` | pointer=/brokerReadyCount |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS185 hierarchy=exact
- After SHA-256: `e5e08c41e4387d954ef7a5bbb49216f7aff262bbd3fd9ee47d5cf66515a7fc2e`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS184/attempt-001.json`
- Summary: run=RS184 attempt=1 lineage=exact
- After SHA-256: `69f5c3d4667bf7fe58b0fd77bed0b44a04a9e6b8738eb89f7473735badde69c9`

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
- Summary: node=M8-W01-C01 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/RS131-portable-protocol-qualified.json`
- Summary: pointer=/total_cases

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj`
- Summary: bytes=571
- After SHA-256: `5dda57552166031a89b13c95236c9a7bc761f8425865b6ddba5918b000c4f7a2`

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.discovery-bindings/Program.cs`
- Summary: bytes=6099
- After SHA-256: `67fbff5f5fcf9bafe86a1409907ca4cbe29510b65b0eb00a8d316e15c519f966`

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-selected-clr-bindings.json`
- Summary: pointer=/status

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-selected-clr-bindings.json`
- Summary: pointer=/brokerReadyCount

