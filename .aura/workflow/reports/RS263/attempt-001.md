# RS263 Attempt 001 Payload Operations

- Operations: 8
- Changed: 1
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS263 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS262/attempt-001.json` | run=RS262 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | pointer=/observation/packageVersion |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8WslNet11PublishedStatisticsBinary.cs` | bytes=6609 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS263 hierarchy=exact
- After SHA-256: `492768906455cfbf696d7fc9987d435b28d90ab40d6933cfce7ad0a85220bcd0`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS262/attempt-001.json`
- Summary: run=RS262 attempt=1 lineage=exact
- After SHA-256: `613360a6e5f681cd4e3d4dee4e14997d3eaf439e34eb9786fc94bb49a27659cd`

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
- Path: `docs/engineering/ProbeM8WslNet11PublishedStatisticsBinary.cs`
- Summary: bytes=6609
- After SHA-256: `8412e238ba75dffc7a082e27de6dc7a78ec4e0ae39e64059490a2eeb24c9b19a`

