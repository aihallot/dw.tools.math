# RS230 Attempt 001 Payload Operations

- Operations: 8
- Changed: 1
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS230 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS229/attempt-001.json` | run=RS229 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8Net11ScientificSource.cs` | bytes=4282 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS230 hierarchy=exact
- After SHA-256: `d73fd39ac46429fd34300ed5b691274e6b05f9eddb8498c32ab228b26b509c39`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS229/attempt-001.json`
- Summary: run=RS229 attempt=1 lineage=exact
- After SHA-256: `79a1fe27978fab562d857c1679a6bed3d97bb10f345baab2f9587118803495ea`

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
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM8Net11ScientificSource.cs`
- Summary: bytes=4282
- After SHA-256: `83fdc664e9d0310202501073e03a51178498c4888d7d334ba3865688533795e2`

