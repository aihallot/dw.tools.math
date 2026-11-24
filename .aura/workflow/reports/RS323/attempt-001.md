# RS323 Attempt 001 Payload Operations

- Operations: 19
- Changed: 5
- No change: 0
- Verified: 14
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS322/attempt-001.json` | run=RS322 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=in-progress |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01 state=in-progress |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01-T1 state=done |
| 7 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01-T2 state=in-progress |
| 8 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01-T2-R state=done |
| 9 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01-T2-G state=in-progress |
| 10 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C01-T2-V state=not-ready |
| 11 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C02 state=not-ready |
| 12 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 13 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 14 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 15 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8W02LiveStandardLockedRestoreGreen.cs` | bytes=11021 |
| 16 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=41954 |
| 17 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=54797 |
| 18 | `files.replace-from-staged` | Changed | `docs/engineering/m8-net11-csharp15-migration-evidence.md` | bytes=21081 |
| 19 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m8-w02-c01-t2-green-independent-locked-restore-rs323.md` | bytes=2648 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS322/attempt-001.json`
- Summary: run=RS322 attempt=1 lineage=exact
- After SHA-256: `61c7a715a65effaa5a1b5089b7b466111b6bd6a0ce21d4cc74d81530456e1c50`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=in-progress
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01 state=done
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=in-progress
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01 state=in-progress
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01-T1 state=done
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 7. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01-T2 state=in-progress
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 8. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01-T2-R state=done
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 9. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01-T2-G state=in-progress
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 10. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C01-T2-V state=not-ready
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 11. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C02 state=not-ready
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 12. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `150954eb407f2336757d4fb267bba5ebb78211d3dc911e4aaa90b7d7af1106fc`

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM8W02LiveStandardLockedRestoreGreen.cs`
- Summary: bytes=11021
- After SHA-256: `5ea0d7f54ff5154f13ddcb08764e0024aa9b70b2c9bf0f81487376fe546c31b8`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=41954
- Before SHA-256: `7be78c38bd4cce1db2a71073f9ba18b0f4268d7ee615deedf2026a9b594d563f`
- After SHA-256: `76113c21b46211f697788e1642828814565a27ffaec1c348e4f2fea61bafda54`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=54797
- Before SHA-256: `6759ab8e948e9c119a3ec8952d8acbe20a6b6841a3dca40801df0ed0dab765fa`
- After SHA-256: `50579de2eafdfdd8f6d581e30306eea56f0bb62e7c79cc8d418caca3203e85c2`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/m8-net11-csharp15-migration-evidence.md`
- Summary: bytes=21081
- Before SHA-256: `985fd14c63b6c1724fdc24defb340f5a2c2f39115bd93deea79aaaa4e145df87`
- After SHA-256: `ee312972609832f8ddd2acc92dfdae6308509da1213e03c72901da64a64a0b9d`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m8-w02-c01-t2-green-independent-locked-restore-rs323.md`
- Summary: bytes=2648
- After SHA-256: `39595dc4ffaac2662edd2c6d44acd7696b7c2c6603b9910648bf16076363f9a8`

