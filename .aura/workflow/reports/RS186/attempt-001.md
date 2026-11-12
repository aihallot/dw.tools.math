# RS186 Attempt 001 Payload Operations

- Operations: 11
- Changed: 1
- No change: 0
- Verified: 10
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS186 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS185/attempt-001.json` | run=RS185 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-selected-clr-bindings.json` | pointer=/status |
| 8 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-selected-clr-bindings.json` | pointer=/brokerReadyCount |
| 9 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/GenerateM8DiscoveryManifest.cs` | bytes=9436 |
| 10 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 11 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/owner |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS186 hierarchy=exact
- After SHA-256: `95d042dc9204925b637ffc25b6f1c4a42f851e9ce6ee7f51a6d085c935b8aa79`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS185/attempt-001.json`
- Summary: run=RS185 attempt=1 lineage=exact
- After SHA-256: `e00a9cace9ffef09ec11631c3808e3a0a8ae26f977948d31d6dd6f53faf67fcb`

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
- Path: `docs/discovery/qualification/m8-selected-clr-bindings.json`
- Summary: pointer=/status

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-selected-clr-bindings.json`
- Summary: pointer=/brokerReadyCount

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/GenerateM8DiscoveryManifest.cs`
- Summary: bytes=9436
- After SHA-256: `f37602a30696b0aeed4e693902367ac3077265ea52a46fe4b72b14c16a22b2b1`

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-discovery-manifest.json`
- Summary: pointer=/schemaVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-discovery-manifest.json`
- Summary: pointer=/owner

