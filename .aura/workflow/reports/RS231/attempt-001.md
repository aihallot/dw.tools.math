# RS231 Attempt 001 Payload Operations

- Operations: 10
- Changed: 1
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS231 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS230/attempt-001.json` | run=RS230 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.text-contains` | Verified | `global.json` | semantic text found |
| 8 | `require.text-contains` | Verified | `Directory.Build.props` | semantic text found |
| 9 | `files.replace-from-staged` | Changed | `docs/engineering/m8-net11-csharp15-migration-evidence.md` | bytes=5507 |
| 10 | `require.text-contains` | Verified | `docs/engineering/m8-net11-csharp15-migration-evidence.md` | semantic text found |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS231 hierarchy=exact
- After SHA-256: `874f54d3897af8c2f3e497204b83a137cad03254532c88f18c26991cad1a3a90`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS230/attempt-001.json`
- Summary: run=RS230 attempt=1 lineage=exact
- After SHA-256: `d1a76e79f0c684ed9d46768fc64c8d7bfed0f00ff7d74f905e68d86c6c285505`

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
- Path: `global.json`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 8. `require.text-contains`

- Status: `Verified`
- Path: `Directory.Build.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/m8-net11-csharp15-migration-evidence.md`
- Summary: bytes=5507
- After SHA-256: `a3b6198a8418e23cfa412482848a4ad0b637ab5924e8a96e003de51e9a90e7b1`

## 10. `require.text-contains`

- Status: `Verified`
- Path: `docs/engineering/m8-net11-csharp15-migration-evidence.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

