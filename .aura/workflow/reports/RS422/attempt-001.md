# RS422 Attempt 001 Payload Operations

- Operations: 17
- Changed: 10
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS421/attempt-001.json` | run=RS421 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-R state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-G state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11 state=done |
| 5 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 6 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=72707 |
| 7 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs` | bytes=61038 |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=43404 |
| 9 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9AssessaPublishedHandoff.cs` | bytes=5241 |
| 10 | `files.replace-from-staged` | Changed | `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs` | bytes=3343 |
| 11 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=67823 |
| 12 | `files.replace-from-staged` | Changed | `README.md` | bytes=10857 |
| 13 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m10-post-cutover-linux-consumers-rs422.md` | bytes=970 |
| 14 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM10PostCutoverWslConsumers.cs` | bytes=3412 |
| 15 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 16 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C03-T1-R state=done |
| 17 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C03-T1-G state=in-progress |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS421/attempt-001.json`
- Summary: run=RS421 attempt=1 lineage=exact
- After SHA-256: `e1538ed5cc0a96ebca3688252f17c786232a8b564b26c64db606b8f17a8b74b7`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-R state=done
- After SHA-256: `fc36eb686c31067b4ed1f7028de97ad5902ed9f312c4b95d50e674fd72c1597f`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-G state=done
- After SHA-256: `fc36eb686c31067b4ed1f7028de97ad5902ed9f312c4b95d50e674fd72c1597f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M11 state=done
- After SHA-256: `fc36eb686c31067b4ed1f7028de97ad5902ed9f312c4b95d50e674fd72c1597f`

## 5. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 6. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=72707
- Before SHA-256: `db467a9b843b989beb079ec703f5209004689e4091b1a91f46bbc6a0050e2db1`
- After SHA-256: `26f765f9b5c553acd4671424ede1f3c7f7d9b2119ac38d2c1e468872b5b18bef`

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM11NumericSubsidiarityAdmission.cs`
- Summary: bytes=61038
- Before SHA-256: `53f9f62d2f1b4d71d28b5925d372ccf732f4f233a821a2fca3b0ac68dcc8deb2`
- After SHA-256: `909684aebcddd0209988769e67118b3d6abfd464bab955adef5532e8e343b066`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=43404
- Before SHA-256: `42fe5a4270659480c1b052db44dce3be6c0cf2f3dd796e39dbf0ba664a073247`
- After SHA-256: `93200c43a4309a9e5d31fddff7fd7b95ca596f85db548c02848ec295f6c5b0ba`

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9AssessaPublishedHandoff.cs`
- Summary: bytes=5241
- Before SHA-256: `273eff991c6e9eadf041a9019ba05839e473eb7e48d163dbf1c1fb9f368f9b67`
- After SHA-256: `b32347e1c1f7d2e9e3a6ed93c3e87f7f90a619b4993b718df442ca869415d251`

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ValidateM9RequestDeliveryReconciliation.cs`
- Summary: bytes=3343
- Before SHA-256: `a85e61d51bbb8cef877524216e1433964e7a67c4310ac91923bffd5209b6b618`
- After SHA-256: `661ef2eaed497d8a1b5b82c7eec92a9edd057e8012454142456bb3ce6bbac3e7`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=67823
- Before SHA-256: `532442acbd03d92e8fd470c5f38d277fdd1123ed2777354a7eaaa5d3be79f0fc`
- After SHA-256: `4671240bdd7f14651b475b00969b8835303f85c3f6782a858754974bf99f450d`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `README.md`
- Summary: bytes=10857
- Before SHA-256: `b41e8e0aaaac0b9abd35db3bbe92deecb48645cf12251848161c8d25285a75ad`
- After SHA-256: `6190b575c93dcdcd8aaf3607f8c9754498379c8060c5109e5636c0b6599a3a3a`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m10-post-cutover-linux-consumers-rs422.md`
- Summary: bytes=970
- After SHA-256: `337bf74a1962e34c6fe695557f4d7753587bac5826520bcc5d1e89c3f8ab6434`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM10PostCutoverWslConsumers.cs`
- Summary: bytes=3412
- After SHA-256: `32968a9511537acf61548a59452d114c81a27ed7ac5668c9b5964d747b6a0841`

## 15. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `8dfe65ee35b729d974b182037b8a0b231df0681775e91c7cb85de5dd3eb57d04`
- After SHA-256: `a0091cf098b3ff1d645612d1e7fbcd9adbdbee1010c0f4506434752bbc4fa8d1`

## 16. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C03-T1-R state=done
- After SHA-256: `fc36eb686c31067b4ed1f7028de97ad5902ed9f312c4b95d50e674fd72c1597f`

## 17. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C03-T1-G state=in-progress
- After SHA-256: `fc36eb686c31067b4ed1f7028de97ad5902ed9f312c4b95d50e674fd72c1597f`

