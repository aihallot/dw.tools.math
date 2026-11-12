# RS191 Attempt 001 Payload Operations

- Operations: 9
- Changed: 1
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS191 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS190/attempt-001.json` | run=RS190 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 8 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/owner |
| 9 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8SkillReference.cs` | bytes=6306 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS191 hierarchy=exact
- After SHA-256: `9afe5b24e47706d39575ca25047a6cdf415e16a32ba5ffaa0b5663200c4ceb31`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS190/attempt-001.json`
- Summary: run=RS190 attempt=1 lineage=exact
- After SHA-256: `936d32f8ef0a4e3f4018467480785b40853266adf380e03c02518eaf9ca271ae`

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
- Path: `docs/discovery/qualification/ValidateM8SkillReference.cs`
- Summary: bytes=6306
- After SHA-256: `c98718eb4e9b333a9a2354ef163a1f36159c9c3706e62380d3a613fe33402b8e`

