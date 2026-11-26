# RS378 Attempt 001 Payload Operations

- Operations: 23
- Changed: 15
- No change: 0
- Verified: 8
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS377/attempt-001.json` | run=RS377 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11 state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11-W01-C01 state=done |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3-W02-C01 state=done |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C02 state=not-ready |
| 7 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 8 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 9 | `files.replace-from-staged` | Changed | `docs/planning/backlog.json` | bytes=569844 |
| 10 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=14280 |
| 11 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs` | bytes=14542 |
| 12 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=6439 |
| 13 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=43404 |
| 14 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9AssessaPublishedHandoff.cs` | bytes=5241 |
| 15 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs` | bytes=3343 |
| 16 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=67823 |
| 17 | `files.replace-from-staged` | Changed | `README.md` | bytes=8334 |
| 18 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m11-streaming-moments-red-rs378.md` | bytes=3206 |
| 19 | `files.replace-from-staged` | Changed | `docs/engineering/fixtures/M11StreamingMomentsOracle.cs` | bytes=5372 |
| 20 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM11StreamingMomentsRed.cs` | bytes=4008 |
| 21 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M11-W01-C02 ready->in-progress |
| 22 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M11-W01-C02-T1 ready->in-progress |
| 23 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M11-W01-C02-T1-R ready->in-progress |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS377/attempt-001.json`
- Summary: run=RS377 attempt=1 lineage=exact
- After SHA-256: `803dcd6fd639716a9abd4509608664f84e1f04ce1d3d6ea9837b289a6629994d`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11 state=in-progress
- After SHA-256: `78297b9a6f8524715e80ae46e479520fc5ceb7d9b0043f6ba5a1bbf38139629a`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01 state=in-progress
- After SHA-256: `78297b9a6f8524715e80ae46e479520fc5ceb7d9b0043f6ba5a1bbf38139629a`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C01 state=done
- After SHA-256: `78297b9a6f8524715e80ae46e479520fc5ceb7d9b0043f6ba5a1bbf38139629a`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3-W02-C01 state=done
- After SHA-256: `78297b9a6f8524715e80ae46e479520fc5ceb7d9b0043f6ba5a1bbf38139629a`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C02 state=not-ready
- After SHA-256: `78297b9a6f8524715e80ae46e479520fc5ceb7d9b0043f6ba5a1bbf38139629a`

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
- Summary: bytes=569844
- Before SHA-256: `c41df4761d2d2d9f3aa26eba67201caf6af59cd437449827197f9ded6c8405ea`
- After SHA-256: `2e1e395666992de71a21827cf790596d6b3e584f15556a86e436b4fb3172405e`

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=14280
- Before SHA-256: `a8b97dcccbb8a3a7c0e3a6589fc404b90a4abf59b0f3f9ba17b75988d96c831b`
- After SHA-256: `3e2442405152e0a2d90a144d2f21f6a5d4017751dd5fee1a26f74dfaf0a92072`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs`
- Summary: bytes=14542
- Before SHA-256: `b1d492e165a73dd7d8128a01b2e1ded75205cc0f384b213ef0d79bf6e0b4bef3`
- After SHA-256: `d5a1f1dfe64ea468f0a1581102c7311473a4ccfa5546e0b58fb79d4d9bfb5ac3`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=6439
- Before SHA-256: `f1349fd278fc2f0cc5ab940c76ba57f725dc71f8d529a2851e33a992da0bc8ad`
- After SHA-256: `5334a124c2c79bee3d59b341838ac68a4e5b0200bcee7186dbc6f7de7f5d4af2`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=43404
- Before SHA-256: `cfcd83fc61d233735194a3b9fb6e5b46400e48b5ae15d48647411d4531bdecca`
- After SHA-256: `c92a92da031e67b92dc5e053b0b9ce99888c4b96461336f6af0c7f07fb77676b`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9AssessaPublishedHandoff.cs`
- Summary: bytes=5241
- Before SHA-256: `c093a2a35586eba6ef26c4d658a74a0f809abeab462a40524b5213737f79bc98`
- After SHA-256: `f8a91bdd4a233125ef5b512800c649432a0440803916e203a4f191a61680be85`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs`
- Summary: bytes=3343
- Before SHA-256: `258119132801fae472e1b299141cde0249741caa1ad72cd5ae72185517ab37bd`
- After SHA-256: `89d96cb0a6fcb87a154c6a8705bc9ed8b3e6d078e77dfd0eb57620a4f5aa520a`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=67823
- Before SHA-256: `d7c64ab0138b24249ba2049ecab320a5dc33192b0185deeffcd16e9f25583d64`
- After SHA-256: `6cf8f25d2105f48892bb29d8d72097e821dd8e98e1450ca4295d483812b39ac3`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `README.md`
- Summary: bytes=8334
- Before SHA-256: `1ce1a1241c5bc68cae03b4919f2eae57346a7bb02002624077439f93decd2919`
- After SHA-256: `f471004f2be9ffdf2e55e9b09732214891f5d4cbeac53088f536623c2e3f7a10`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m11-streaming-moments-red-rs378.md`
- Summary: bytes=3206
- After SHA-256: `959344431c91c99959566b4eeada4924eb9a9f8e6121a8040e193e5396016bd8`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/fixtures/M11StreamingMomentsOracle.cs`
- Summary: bytes=5372
- After SHA-256: `48b6ac3b8b645b37be91192955ca1aff27339053e5325bf1ce824e3cb8f9b9c1`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM11StreamingMomentsRed.cs`
- Summary: bytes=4008
- After SHA-256: `532209d8fa513a425fa0848af03d389d6a18ab67f32eaedb9e03718b7108dcb6`

## 21. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C02 ready->in-progress
- Before SHA-256: `78297b9a6f8524715e80ae46e479520fc5ceb7d9b0043f6ba5a1bbf38139629a`
- After SHA-256: `8e296849684435b161a3a6481740acc254920de9abdaa3801a464c1f8f2b7c0d`

## 22. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C02-T1 ready->in-progress
- Before SHA-256: `8e296849684435b161a3a6481740acc254920de9abdaa3801a464c1f8f2b7c0d`
- After SHA-256: `75966eb92839df22ac0464418ad36143216966897055ec527ad837d6165c6f5d`

## 23. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11-W01-C02-T1-R ready->in-progress
- Before SHA-256: `75966eb92839df22ac0464418ad36143216966897055ec527ad837d6165c6f5d`
- After SHA-256: `01263ec54d53ec2aa3e4c6f69141d01a59e059af071d2296fd908c0f8e394968`

