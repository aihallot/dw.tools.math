# RS380 Attempt 001 Payload Operations

- Operations: 21
- Changed: 13
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS379/attempt-001.json` | run=RS379 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01 state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C02-T1 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C02-T1-R state=done |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C02-T1-V state=not-ready |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C02 state=not-ready |
| 7 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 8 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 9 | `files.replace-from-staged` | Changed | `docs/planning/backlog.json` | bytes=570059 |
| 10 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=14325 |
| 11 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs` | bytes=17732 |
| 12 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=6439 |
| 13 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=43404 |
| 14 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9AssessaPublishedHandoff.cs` | bytes=5241 |
| 15 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs` | bytes=3343 |
| 16 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=67823 |
| 17 | `files.replace-from-staged` | Changed | `README.md` | bytes=8334 |
| 18 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m11-streaming-moments-green-rs380.md` | bytes=1244 |
| 19 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.statistics/StreamingMoments64.cs` | bytes=4730 |
| 20 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.statistics.tests/M11StreamingMomentsTests.cs` | bytes=4405 |
| 21 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M11-W01-C02-T1-G activated=M11-W01-C02-T1-G |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS379/attempt-001.json`
- Summary: run=RS379 attempt=1 lineage=exact
- After SHA-256: `43ce64b0af6226c0acf82350bbfdcda0536428e0c78d0208895d4d0234f40dc1`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01 state=done
- After SHA-256: `790010f2390cebdd337f5214647057913d5ee133ce7890db60da8fb7da965649`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C02-T1 state=in-progress
- After SHA-256: `790010f2390cebdd337f5214647057913d5ee133ce7890db60da8fb7da965649`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C02-T1-R state=done
- After SHA-256: `790010f2390cebdd337f5214647057913d5ee133ce7890db60da8fb7da965649`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C02-T1-V state=not-ready
- After SHA-256: `790010f2390cebdd337f5214647057913d5ee133ce7890db60da8fb7da965649`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C02 state=not-ready
- After SHA-256: `790010f2390cebdd337f5214647057913d5ee133ce7890db60da8fb7da965649`

## 7. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: bytes=570059
- Before SHA-256: `a15fb9361b1c5c94f670a8575458d547b378cb7e4cb11781ab1ab5f22f7a12e3`
- After SHA-256: `fbe10136667fbb7df9789bc74f0eba58ad21f464c6221d2aac1ef67d1f488357`

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=14325
- Before SHA-256: `4900fb4272a6425ae69eae3d3c0b9fe31244bc4125bacc98b033eba245d1677c`
- After SHA-256: `034fbdf966c2b24e8359d1c1c96cedc0168c6c7ae7d8ca88d490797f57667fc9`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs`
- Summary: bytes=17732
- Before SHA-256: `8cd68e8491e9ae141cd1bdfa5fff4f4d139322b81f063fdf49adeb5bd547c54e`
- After SHA-256: `1a96008851d9156815bb10ed4a423d3efe4b756a5a1cb3b8dc73adeffb1a448a`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=6439
- Before SHA-256: `748409d7d8763c7354ca4c0bc75f7d0517628ee268c3e299ad4953fe91bbec42`
- After SHA-256: `bdf4e4bf269e510476f987b2c009d9e9d195faf93aa9b70c44eefc35bacc20c9`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=43404
- Before SHA-256: `b587b3dd1e33c5a140cf81ff3cde3e6668b840fdd6f3ab639af80ac9907c2e37`
- After SHA-256: `365b038e4f99658a33a47b8fdecd6048b9cf28dd3233d86b8922bc148ee738bf`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9AssessaPublishedHandoff.cs`
- Summary: bytes=5241
- Before SHA-256: `3a18ad233777d20977f529331857ea176866d487d206e81adef8f5c9e92f63f0`
- After SHA-256: `9b9350886e3bacb56a07f2cfcae7aeafaa8db8758f3f4f49f2c0b556cd37179e`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs`
- Summary: bytes=3343
- Before SHA-256: `7643da0fe3b8182ad986f6695126ea7a6de4a627796cbb7290406869e9c634d6`
- After SHA-256: `7e9bc8eb184026aaa62d6ee09a90384aa37b470dd1595d829be799e57051a266`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=67823
- Before SHA-256: `c61e733e96951cd0363c0f7ba7909a6cdc8998567661d1d000de5593bc3e3900`
- After SHA-256: `5b7d1aa35cfcfb7f8b60d47a1737c8bf3f2cab15ecb4e8a13fab1ebdc9f6d52d`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `README.md`
- Summary: bytes=8334
- Before SHA-256: `f4a61a51168b2681bd3daeea33c09e3b3db3a385716d1d55d99594e97560df91`
- After SHA-256: `3202d38674bd022dc404bf0499a4d7abf1d174fb1afc6be0bf7aa30793730f18`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m11-streaming-moments-green-rs380.md`
- Summary: bytes=1244
- After SHA-256: `d6e351450cd8e9c61ee4e4da9cbc373bcafcd90c26163c374a2eefa290cc0db2`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.statistics/StreamingMoments64.cs`
- Summary: bytes=4730
- After SHA-256: `3284c6f6d22b6ab2d3ea434bce440a8b9e254736275ac074cd96b0c5869f3c71`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.statistics.tests/M11StreamingMomentsTests.cs`
- Summary: bytes=4405
- After SHA-256: `dd7e2e1bd31c785ea998426c2a024aec23c4f9f8717bc70c103a676a1087c41f`

## 21. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M11-W01-C02-T1-G activated=M11-W01-C02-T1-G
- Before SHA-256: `790010f2390cebdd337f5214647057913d5ee133ce7890db60da8fb7da965649`
- After SHA-256: `df8b05aa905bcffa7dfd4b4893550375338691e60bbe770aafa1c81897bb2b2c`

