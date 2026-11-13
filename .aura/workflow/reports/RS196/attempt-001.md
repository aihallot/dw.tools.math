# RS196 Attempt 001 Payload Operations

- Operations: 9
- Changed: 1
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS196 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS195/attempt-001.json` | run=RS195 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/physically_published_package_verified |
| 9 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/AuditM8PublishedPackageReadiness.cs` | bytes=5060 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS196 hierarchy=exact
- After SHA-256: `52c36a45145717a37dbb07f045d9eea77e5a78bf36682b03f517165982faafbc`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS195/attempt-001.json`
- Summary: run=RS195 attempt=1 lineage=exact
- After SHA-256: `b751bcecaa1d8a500f17e815fe4f658f61d8d6f3f94f58b5e44455c89ca8aba7`

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
- Path: `docs/discovery/qualification/AuditM8PublishedPackageReadiness.cs`
- Summary: bytes=5060
- After SHA-256: `42314a6a77e81577ddc5d25805247c71ce5360816cfe7f76dd51e2678e7bfd09`

