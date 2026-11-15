# RS227 Attempt 001 Payload Operations

- Operations: 8
- Changed: 1
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS227 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS226/attempt-001.json` | run=RS226 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.text-contains` | Verified | `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj` | semantic text found |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8Net11StatisticsSource.cs` | bytes=5062 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS227 hierarchy=exact
- After SHA-256: `33f35a6e44811a29d9ecf13118574f170b82b4395066efa54febbb29b7d792ff`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS226/attempt-001.json`
- Summary: run=RS226 attempt=1 lineage=exact
- After SHA-256: `7c519e6a175deea2e0b054ed1e8ce47893bc263dd67bc0e6b7d7b1e8159cf6be`

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
- Path: `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM8Net11StatisticsSource.cs`
- Summary: bytes=5062
- After SHA-256: `33a76e8643592a619710042a1b74830bb20786dabf42cd8e8dbe94aca34d661e`

