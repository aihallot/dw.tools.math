# RS432 Attempt 001 Payload Operations

- Operations: 8
- Changed: 2
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS431/attempt-001.json` | run=RS431 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10 state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M12-W01-C01-T1-A state=ready |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 7 | `files.replace-from-staged` | Changed | `docs/distribution/m11-private-publication-proposal.md` | bytes=8435 |
| 8 | `files.replace-from-staged` | Changed | `docs/engineering/VerifyM11NumericPackageCandidates.cs` | bytes=13423 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS431/attempt-001.json`
- Summary: run=RS431 attempt=1 lineage=exact
- After SHA-256: `4af171918604fa687605af5a8919d5a407be56d533b23bb34905a729a0a5660f`

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
- Path: `docs/distribution/m11-private-publication-proposal.md`
- Summary: bytes=8435
- After SHA-256: `d6690fb9a5f96afad60023669b79f3604ab6358421df4269a75ecbf21c1169d8`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/VerifyM11NumericPackageCandidates.cs`
- Summary: bytes=13423
- Before SHA-256: `4f7fd74d68e7dd14e0168cfb73e2eefdc4de97c676f013f48ed11364800fa76b`
- After SHA-256: `250ee468512d9370f0eac0b6f046921349ef7dd6488d33d1b931c2e8016fe82c`

