# RS344 Attempt 001 Payload Operations

- Operations: 13
- Changed: 4
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS343/attempt-001.json` | run=RS343 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C02 state=in-progress |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C02-T1 state=done |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02-C02-T2 state=done |
| 7 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 8 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 9 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 10 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM8W02Net11StatisticsCandidate.cs` | bytes=17176 |
| 11 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m8-w02-c02-integrated-net11-candidate-rs344.md` | bytes=912 |
| 12 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=75130 |
| 13 | `files.replace-from-staged` | Changed | `docs/engineering/m8-net11-csharp15-migration-evidence.md` | bytes=40137 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS343/attempt-001.json`
- Summary: run=RS343 attempt=1 lineage=exact
- After SHA-256: `b94f01227be4fc7d846f5050264580132e7361075fa9e060d4bef23188f8386b`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=in-progress
- After SHA-256: `aa38e085c6412fa5d33e31019a372279bfd3f1031007ba7cfcd954c7d22bfd99`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=in-progress
- After SHA-256: `aa38e085c6412fa5d33e31019a372279bfd3f1031007ba7cfcd954c7d22bfd99`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C02 state=in-progress
- After SHA-256: `aa38e085c6412fa5d33e31019a372279bfd3f1031007ba7cfcd954c7d22bfd99`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C02-T1 state=done
- After SHA-256: `aa38e085c6412fa5d33e31019a372279bfd3f1031007ba7cfcd954c7d22bfd99`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02-C02-T2 state=done
- After SHA-256: `aa38e085c6412fa5d33e31019a372279bfd3f1031007ba7cfcd954c7d22bfd99`

## 7. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `aa38e085c6412fa5d33e31019a372279bfd3f1031007ba7cfcd954c7d22bfd99`

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM8W02Net11StatisticsCandidate.cs`
- Summary: bytes=17176
- Before SHA-256: `268c5868bd7f74b57a366c71bad10f63faa4086cad8c9ed17b016d1b0e3a6149`
- After SHA-256: `66ca207225c19af9ac27ecb42e233909b6d24f8907539b240d355b1969dc41fd`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m8-w02-c02-integrated-net11-candidate-rs344.md`
- Summary: bytes=912
- After SHA-256: `652f548f55f0ca8a5293bcbd54b00ff26040a3e67146c9bf074cf90f53762748`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=75130
- Before SHA-256: `ca20ab806ca16c1447056cf8d89afc5b5769b2c9dea698d4c8df893b655da21f`
- After SHA-256: `c21ce5b986abb1c8dfbd709f8b75cdc9aa8a2c6c233922c38bc2b2ad0f4f6857`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/m8-net11-csharp15-migration-evidence.md`
- Summary: bytes=40137
- Before SHA-256: `bff84365c54d7daacf1c5df90fafc215d0b7f0d024b12261ca0b710ff0675eb6`
- After SHA-256: `b299b4f80dd395424867a8a15bf973f9847da52d7d48bed101db669c8677c29c`

