# RS197 Attempt 001 Payload Operations

- Operations: 9
- Changed: 1
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS197 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS196/attempt-001.json` | run=RS196 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/physically_published_package_verified |
| 9 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/VerifyM8PrivateBinaryBindings.cs` | bytes=8467 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS197 hierarchy=exact
- After SHA-256: `8da7e929efd4b76c4bfc4802646728257c26102a84e56ff0a7510ed09893fc16`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS196/attempt-001.json`
- Summary: run=RS196 attempt=1 lineage=exact
- After SHA-256: `bdbdefeae57034219beb638362dae6325767983291a7bc8db7844d51eee22631`

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
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/VerifyM8PrivateBinaryBindings.cs`
- Summary: bytes=8467
- After SHA-256: `21557ddf79d67d212ecb4decb7691c73794671c1cc9663dccea10e338b06a5b5`

