# RS187 Attempt 001 Payload Operations

- Operations: 10
- Changed: 1
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS187 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS186/attempt-001.json` | run=RS186 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 8 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/owner |
| 9 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-selected-clr-bindings.json` | pointer=/brokerReadyCount |
| 10 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8GeneratedManifest.cs` | bytes=7602 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS187 hierarchy=exact
- After SHA-256: `c5fd0b8f632746b333b97425062dc3e859edb2b33d3091b352b7b4d799e96063`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS186/attempt-001.json`
- Summary: run=RS186 attempt=1 lineage=exact
- After SHA-256: `c9ad3f5c2acfaa06cb31bc671cc101b7118be96ae8f29ff925a33c1e875c786d`

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

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-selected-clr-bindings.json`
- Summary: pointer=/brokerReadyCount

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8GeneratedManifest.cs`
- Summary: bytes=7602
- After SHA-256: `27399936943e5d808df395df9b518a2d6aa33fa680bf40ebf0965bc7dec5a79c`

