# RS208 Attempt 001 Payload Operations

- Operations: 9
- Changed: 1
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS208 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS207/attempt-001.json` | run=RS207 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01 state=not-ready |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 7 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 8 | `require.json-value-equals` | Verified | `docs/discovery/qualification/m8-discovery-manifest.json` | pointer=/schemaVersion |
| 9 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=4916 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS208 hierarchy=exact
- After SHA-256: `5505d16e4396f3cf3390ce957553e8c0f3eb2b12cf6f7134eee0ab6786da7d79`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS207/attempt-001.json`
- Summary: run=RS207 attempt=1 lineage=exact
- After SHA-256: `f1c3ea9d08687f3a88ca427e6998bc54adddad5ab192b82c059eb10451ce8819`

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

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C02 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/m8-discovery-manifest.json`
- Summary: pointer=/schemaVersion

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=4916
- After SHA-256: `cf02b04f2c687bbc53f0e3df9bd9a3c9ae025a23b72c8c641f2d77a397094ed7`

