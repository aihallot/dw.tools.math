# RS374 Attempt 001 Payload Operations

- Operations: 20
- Changed: 13
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS373/attempt-001.json` | run=RS373 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01-T1 state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01-T1-R state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01-T1-V state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C02 state=not-ready |
| 6 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/backlog.json` | bytes=568076 |
| 9 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=14199 |
| 10 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs` | bytes=8004 |
| 11 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=6439 |
| 12 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=43404 |
| 13 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9AssessaPublishedHandoff.cs` | bytes=5241 |
| 14 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs` | bytes=3343 |
| 15 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=67823 |
| 16 | `files.replace-from-staged` | Changed | `README.md` | bytes=8334 |
| 17 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m11-exact-int64-scaling-green-rs374.md` | bytes=1224 |
| 18 | `files.replace-from-staged` | Changed | `src/projects/dw.quantities/ExactRationalBudget.cs` | bytes=5065 |
| 19 | `files.replace-from-staged` | Changed | `tests/projects/dw.quantities.tests/M11ExactInt64ScalingTests.cs` | bytes=2807 |
| 20 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M11-W01-C01-T1-G activated=M11-W01-C01-T1-G |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS373/attempt-001.json`
- Summary: run=RS373 attempt=1 lineage=exact
- After SHA-256: `1a3adb1585a446effe50a3351c842330d1e13bec78f4463e8bdfaebc0f8b4af7`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01-T1 state=in-progress
- After SHA-256: `160d99e6ce8c1cd055f3f2273d56731e51bf518b65145a5127ef899e3137340b`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01-T1-R state=done
- After SHA-256: `160d99e6ce8c1cd055f3f2273d56731e51bf518b65145a5127ef899e3137340b`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01-T1-V state=not-ready
- After SHA-256: `160d99e6ce8c1cd055f3f2273d56731e51bf518b65145a5127ef899e3137340b`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C02 state=not-ready
- After SHA-256: `160d99e6ce8c1cd055f3f2273d56731e51bf518b65145a5127ef899e3137340b`

## 6. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: bytes=568076
- Before SHA-256: `325d3b59da0ef0978c578e4c9f28da77551a0fb55ad86c0ed5b11185d747be77`
- After SHA-256: `0e82532f2995d0f86aed910dc1bb259d0860b2e83f0c3aaba0bded1613a53ae9`

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=14199
- Before SHA-256: `c399d59ec9ca5541b5a89e478a2437c0df536922138b2953204320df27519d19`
- After SHA-256: `085422a697c08dc108e91e7d4cd7e1feb7be3969745aa6db27df95b31a8f8817`

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs`
- Summary: bytes=8004
- Before SHA-256: `ead7d56f2907c8969eff3fbd74708b19994a6f716d90d5c64343a337e251f346`
- After SHA-256: `3bd629d2738550d0cdeb8b8275d3342983f1fc0a9667b05b3e7bff0f846b52f6`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=6439
- Before SHA-256: `c000a55801342b3379c0b7f71244878bbc7a475a4a267f9f86f655a62d2bf9a3`
- After SHA-256: `e6f9a073e1ad5e32ee93e31582b1e2d2ed0939d1a9034d8ae168fbbdead76e4c`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=43404
- Before SHA-256: `d2f6f34c99b81011a0ce48212c7c11554a1edbb4e4a69edbde6c9401200bd3ab`
- After SHA-256: `118995c9c289aafd58054a1e57a935cdebd3601be5e97cb66e85924a6c7da203`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9AssessaPublishedHandoff.cs`
- Summary: bytes=5241
- Before SHA-256: `75b9a53a768b02d48e870de7c2d5225373f0232cab0ff995eb59bb88e0972276`
- After SHA-256: `6ad463ba9cb093d48c2461ed6f11410bb92039fe634bb98c9d94f1ef7c0b1bab`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs`
- Summary: bytes=3343
- Before SHA-256: `369180e086d7413fb8f5a6c9d2468de02a14175d9ec4d3ab651df858e549987a`
- After SHA-256: `abc5c5334457a6685cfcb1fc3a328c1a04b8a7c3a64313861e578c84d95f0983`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=67823
- Before SHA-256: `5bf614858af5da739839a0f7fad7385cb7ceb7b90110c9cb238ed2fa5be1fe7c`
- After SHA-256: `bc9eabaad783dd7c1aa26c133cb285afdf7251ad6a8e5308f651d6d227c109aa`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `README.md`
- Summary: bytes=8334
- Before SHA-256: `f822b0ce966afce491fbb737103c5e05c13f2ba0d567061f6390416cd06988f9`
- After SHA-256: `d8150389b373f2961714907a617baedb32a59eec5bd6e04ffcd69385d96a4a0b`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m11-exact-int64-scaling-green-rs374.md`
- Summary: bytes=1224
- After SHA-256: `292f6e55b9aece25a64e7582eb9f789768ac3335a453687911b22c230a3c1ba9`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.quantities/ExactRationalBudget.cs`
- Summary: bytes=5065
- Before SHA-256: `833b53e74a2f327e39e8c527480c6df2f6a4db711b4ca677beedbca52f6438ed`
- After SHA-256: `a827d1752ef24f22fed47c39faaefea413f315e3e016475a7ade62d43877c844`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.quantities.tests/M11ExactInt64ScalingTests.cs`
- Summary: bytes=2807
- After SHA-256: `ed957f456b5dab1a2847658da3b436323fee2013bf4a6225475198995763b503`

## 20. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M11-W01-C01-T1-G activated=M11-W01-C01-T1-G
- Before SHA-256: `160d99e6ce8c1cd055f3f2273d56731e51bf518b65145a5127ef899e3137340b`
- After SHA-256: `efbfc844674a56067af5df6ff6eb70fd5b3cb70fc477b5716141db34888c0516`

