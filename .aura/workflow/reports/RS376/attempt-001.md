# RS376 Attempt 001 Payload Operations

- Operations: 19
- Changed: 12
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS375/attempt-001.json` | run=RS375 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01-T1 state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01-T1-R state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01-T1-G state=done |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C02 state=not-ready |
| 6 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/backlog.json` | bytes=568461 |
| 9 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=14229 |
| 10 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs` | bytes=11098 |
| 11 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=6439 |
| 12 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=43404 |
| 13 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9AssessaPublishedHandoff.cs` | bytes=5241 |
| 14 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs` | bytes=3343 |
| 15 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=67823 |
| 16 | `files.replace-from-staged` | Changed | `README.md` | bytes=8334 |
| 17 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m11-exact-int64-scaling-verification-rs376.md` | bytes=1556 |
| 18 | `files.replace-from-staged` | Changed | `tests/projects/dw.quantities.tests/M11ExactInt64ScalingVerificationTests.cs` | bytes=3556 |
| 19 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M11-W01-C01-T1-V activated=M11-W01-C01-T1-V |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS375/attempt-001.json`
- Summary: run=RS375 attempt=1 lineage=exact
- After SHA-256: `d3826af8f9f9cad353a5104763e0c52619b27ef8025fdd339aba987244554b98`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01-T1 state=in-progress
- After SHA-256: `5d369b325828dc93a08c238a3c5eb25baf45b3ffda77267e8438f448ff5a7d6b`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01-T1-R state=done
- After SHA-256: `5d369b325828dc93a08c238a3c5eb25baf45b3ffda77267e8438f448ff5a7d6b`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01-T1-G state=done
- After SHA-256: `5d369b325828dc93a08c238a3c5eb25baf45b3ffda77267e8438f448ff5a7d6b`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C02 state=not-ready
- After SHA-256: `5d369b325828dc93a08c238a3c5eb25baf45b3ffda77267e8438f448ff5a7d6b`

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
- Summary: bytes=568461
- Before SHA-256: `f89c67fb3193eb1400170b258dbc86484f1ba8c1b453c51c6b400afe42e19be7`
- After SHA-256: `86dafbdf5a1a9c1371d88933929d31ce10ad39db1c0c03e72f93120bcf460400`

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=14229
- Before SHA-256: `f73d0a3858699360d46204eafa28b9221ba8cec87fe37de81d9c4b34e08aedf1`
- After SHA-256: `a0e819f64cc05e5d1ce9041608fdbec00cf43ef86729565e43d766014c6f98ae`

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs`
- Summary: bytes=11098
- Before SHA-256: `e6ae52e7a9b3ccd3256d24b5b6a72c5b952c696bc059c1025114ddcd670718bc`
- After SHA-256: `8b8eaaf9ad5519fd860c579413b5a22f12e7da3a39620efce182fce7625935fb`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=6439
- Before SHA-256: `da3421d5abde38089ba164ad980500f614a0c4bebe87e401bf38eeaa02e9a9eb`
- After SHA-256: `0504da7a95c088c4db1ec5616b78ecb7995feaee3f97ff1636383c2ba7f975a8`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=43404
- Before SHA-256: `a2bf67ab5ec3c0943af7b63d96e38091357e8ce980a97d1af4353da582295ba4`
- After SHA-256: `24bd11fe17290a80c34ff080de80c6d90dc41a54346994481388b060610a117d`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9AssessaPublishedHandoff.cs`
- Summary: bytes=5241
- Before SHA-256: `ac71aafd60c712755a0d71ddd841e5cf3b4338b475c79d1260169588392c66b9`
- After SHA-256: `05b12ee05a06bf56ee364936301e3718f1977419f07c7395a45b93ce1db9b956`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs`
- Summary: bytes=3343
- Before SHA-256: `6460364b7451a1a89104a6c19fc4b84431905cc80d33761ce4a98e8353db5464`
- After SHA-256: `845e8082b3303afa32ca511276e3f6b88d82bd6c06b244283592d17318e90b8c`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=67823
- Before SHA-256: `b2be43b74ac9b401422c58fb1faec8175cce8b56574ec4a08ea9905a33b479a2`
- After SHA-256: `a5b036cf3ba308ac4d7fce56cd77cffdcbb314cf36cb6f3fb219d20e4b92df6f`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `README.md`
- Summary: bytes=8334
- Before SHA-256: `be44e49179253b18d1c1451f152955ef882b85d22ed3df52c387d9441bea8b2c`
- After SHA-256: `bc4d0af4ec68188f058ea869f00f60c6519d1a085e24234b9b5df3c1d01ea1e6`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m11-exact-int64-scaling-verification-rs376.md`
- Summary: bytes=1556
- After SHA-256: `0df1299f4de7b223c7befb12fa7d44c18c9f960fd3bb902e2fa7be0c1e552594`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.quantities.tests/M11ExactInt64ScalingVerificationTests.cs`
- Summary: bytes=3556
- After SHA-256: `d5c2575e70a83f86604446e86a7cad44eeb5540e12a0b99115da33c001ddb3b3`

## 19. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M11-W01-C01-T1-V activated=M11-W01-C01-T1-V
- Before SHA-256: `5d369b325828dc93a08c238a3c5eb25baf45b3ffda77267e8438f448ff5a7d6b`
- After SHA-256: `bc0bfcf968dffa650c2ec43a69cdf048d7a7b10d33d8bfcdab9ec6afee650458`

