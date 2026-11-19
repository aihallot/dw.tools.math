# RS258 Attempt 001 Payload Operations

- Operations: 8
- Changed: 1
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS258 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS257/attempt-001.json` | run=RS257 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f30-codecs-published-qualified.json` | pointer=/observation/packageVersion |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8Net11PublishedCodecsBinary.cs` | bytes=6307 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS258 hierarchy=exact
- After SHA-256: `ed221b57a45639e312efbc3f680c0f50a1a167f6f2841f88a586145424328f6f`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS257/attempt-001.json`
- Summary: run=RS257 attempt=1 lineage=exact
- After SHA-256: `c3b3d492c68659d5035f7c9df00010546b841a453dbb0fc0c55695af748069ed`

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
- Path: `docs/distribution/nuget-federation-f30-codecs-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM8Net11PublishedCodecsBinary.cs`
- Summary: bytes=6307
- After SHA-256: `02b683651a6f535a61cc38d7a545f90dc19025bbb1bbcab83e7905dc3dcd17ee`

