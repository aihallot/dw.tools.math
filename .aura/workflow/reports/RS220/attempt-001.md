# RS220 Attempt 001 Payload Operations

- Operations: 8
- Changed: 1
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS220 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS219/attempt-001.json` | run=RS219 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.text-contains` | Verified | `docs/engineering/InspectM8Net11PackageGraph.cs` | semantic text found |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/InspectM8Net11PackageGraph.cs` | bytes=4000 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS220 hierarchy=exact
- After SHA-256: `fddd94ff73fe96d16bfcf7588e5e56a52f8499945635f9c23577e37863e6b13f`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS219/attempt-001.json`
- Summary: run=RS219 attempt=1 lineage=exact
- After SHA-256: `12b8b5b1b4aa8032da42a6e0443db7326c07809d0e6be243372ebf214c708335`

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

## 7. `require.text-contains`

- Status: `Verified`
- Path: `docs/engineering/InspectM8Net11PackageGraph.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/InspectM8Net11PackageGraph.cs`
- Summary: bytes=4000
- Before SHA-256: `ed0e36ec06bc2c3e70572efe3bf6f154959c0c2971d5a658875d2a25407102bd`
- After SHA-256: `fd1514dbfd6a71feab417b6de5a94dfdab5fef59e3771de21645a57af5cefd68`

