# RS433 Attempt 001 Payload Operations

- Operations: 8
- Changed: 2
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS432/attempt-001.json` | run=RS432 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10 state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M12-W01-C01-T1-A state=ready |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 7 | `files.replace-from-staged` | Changed | `docs/engineering/VerifyM11NumericPackageCandidates.cs` | bytes=19911 |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m11-private-publication-authorization.md` | bytes=4655 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS432/attempt-001.json`
- Summary: run=RS432 attempt=1 lineage=exact
- After SHA-256: `3c28b8cb00a25bb951363cc831a07c542bfe2133017ba9541a4e9bb564fab988`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10 state=done
- After SHA-256: `9d75c30859ad57a76ab055341aa7abc52421a209012443964a2b0d03e60ea0e8`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11 state=done
- After SHA-256: `9d75c30859ad57a76ab055341aa7abc52421a209012443964a2b0d03e60ea0e8`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M12-W01-C01-T1-A state=ready
- After SHA-256: `9d75c30859ad57a76ab055341aa7abc52421a209012443964a2b0d03e60ea0e8`

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/VerifyM11NumericPackageCandidates.cs`
- Summary: bytes=19911
- Before SHA-256: `250ee468512d9370f0eac0b6f046921349ef7dd6488d33d1b931c2e8016fe82c`
- After SHA-256: `7d613319f228dc33abfa1b5ac772314c7c4d0eeae0389d88edbec8cdfcab3fac`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m11-private-publication-authorization.md`
- Summary: bytes=4655
- After SHA-256: `b4c40a36bd0c80382e48de70125bdbb002f80e33886e41860805025087f8dd30`

