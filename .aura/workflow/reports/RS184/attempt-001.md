# RS184 Attempt 001 Payload Operations

- Operations: 11
- Changed: 2
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS184 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS183/attempt-001.json` | run=RS183 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=not-ready |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01 state=not-ready |
| 7 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 8 | `require.json-value-equals` | Verified | `docs/discovery/qualification/RS131-portable-protocol-qualified.json` | pointer=/total_cases |
| 9 | `require.json-value-equals` | Verified | `docs/discovery/qualification/RS131-portable-protocol-qualified.json` | pointer=/csharp_portable_envelope_conformant |
| 10 | `files.replace-from-staged` | Changed | `tests/consumers/m8-discovery-clr-seed/m8-discovery-clr-seed.csproj` | bytes=587 |
| 11 | `files.replace-from-staged` | Changed | `tests/consumers/m8-discovery-clr-seed/Program.cs` | bytes=2452 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS184 hierarchy=exact
- After SHA-256: `e1a52a9664cf81eee46bc25fa1e967ce1317c6c9bfa619d91c5de19f426cc80f`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS183/attempt-001.json`
- Summary: run=RS183 attempt=1 lineage=exact
- After SHA-256: `174077051020495f7a86c721afac418f2395074c42000675b1714f89998e010c`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/RS131-portable-protocol-qualified.json`
- Summary: pointer=/total_cases

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/RS131-portable-protocol-qualified.json`
- Summary: pointer=/csharp_portable_envelope_conformant

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m8-discovery-clr-seed/m8-discovery-clr-seed.csproj`
- Summary: bytes=587
- After SHA-256: `df4792af1dc3a67b3e11e9bd46bbca0427b4d2a856604c824a8f71d8b716cab1`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m8-discovery-clr-seed/Program.cs`
- Summary: bytes=2452
- After SHA-256: `669331919baf284f097f18852e23ef1ace27bac4ddc2a3bd2c63325a6c14d21c`

