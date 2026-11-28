# RS434 Attempt 001 Payload Operations

- Operations: 13
- Changed: 4
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS433/attempt-001.json` | run=RS433 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10 state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M11 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M12-W01-C01-T1-A state=ready |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 7 | `require.text-occurs` | Verified | `.aura/workflow/reports/RS433/attempt-001.json` | occurrences=1 |
| 8 | `require.text-occurs` | Verified | `.aura/workflow/reports/RS433/attempt-001.json` | occurrences=1 |
| 9 | `require.text-occurs` | Verified | `.aura/workflow/reports/RS433/attempt-001.json` | occurrences=1 |
| 10 | `files.replace-from-staged` | Changed | `docs/distribution/m11-private-promotion-blocked-rs433.md` | bytes=5169 |
| 11 | `files.replace-from-staged` | Changed | `docs/coordination/requests/MATH-XR-007-dwf-nuget-exact-candidate-promotion.md` | bytes=6611 |
| 12 | `files.replace-from-staged` | Changed | `docs/coordination/requests/README.md` | bytes=697 |
| 13 | `files.replace-from-staged` | Changed | `docs/distribution/m11-private-publication-proposal.md` | bytes=8822 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS433/attempt-001.json`
- Summary: run=RS433 attempt=1 lineage=exact
- After SHA-256: `16fbb7a2d3afd81db69f0b93a7ead76739b6b2cf9011db5c6dc3533ae31c3922`

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

## 7. `require.text-occurs`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS433/attempt-001.json`
- Summary: occurrences=1

## 8. `require.text-occurs`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS433/attempt-001.json`
- Summary: occurrences=1

## 9. `require.text-occurs`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS433/attempt-001.json`
- Summary: occurrences=1

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m11-private-promotion-blocked-rs433.md`
- Summary: bytes=5169
- After SHA-256: `ccfb30818f050a31214d82a58a22c8196b765ff273e73c2164339c059bc89df6`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/coordination/requests/MATH-XR-007-dwf-nuget-exact-candidate-promotion.md`
- Summary: bytes=6611
- After SHA-256: `525e41d09c305bae02e524ec479230aef46b3cc2ed57a70fab7572009e894c60`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/coordination/requests/README.md`
- Summary: bytes=697
- Before SHA-256: `867f0c63b73236e50e26e2faec6b6651f7e8dd0b068bf6dad7ac0ff4073af60f`
- After SHA-256: `636f161661dccf3d622f97aada20e6d6d55ed2b89b3e748fd3c29199d9678059`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m11-private-publication-proposal.md`
- Summary: bytes=8822
- Before SHA-256: `d6690fb9a5f96afad60023669b79f3604ab6358421df4269a75ecbf21c1169d8`
- After SHA-256: `1b9284f332cb552362fce6ac0436e6c3073ca2edf78ce25418b779fde1f57b50`

