# RS289 Attempt 001 Payload Operations

- Operations: 16
- Changed: 7
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS289 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS288/attempt-001.json` | run=RS288 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1-R state=done |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1-V state=not-ready |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 7 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 8 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 9 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 10 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=11784 |
| 11 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=7385 |
| 12 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=15904 |
| 13 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-portable-discovery-bundle.md` | bytes=7305 |
| 14 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m8-w01-c01-t1-green-pre-attestation-rs289.md` | bytes=2232 |
| 15 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs` | bytes=16334 |
| 16 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M8-W01-C01-T1-G activated=M8-W01-C01-T1-G |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS289 hierarchy=exact
- After SHA-256: `30d5fb2e52fead26b0419968fdce9d425a0aa07989d78c9d29d8da0927acbf22`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS288/attempt-001.json`
- Summary: run=RS288 attempt=1 lineage=exact
- After SHA-256: `db11ecd6c4f148c6cf3d666ad74ded5493a345ddd216c469000f3200ad7cf48c`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=in-progress
- After SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1-R state=done
- After SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1-V state=not-ready
- After SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C02 state=not-ready
- After SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`

## 7. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=not-ready
- After SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`

## 8. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=11784
- Before SHA-256: `1f6188f3718d77eac6415e0ddacabb61ea9b21e061604efe470da83280be69fe`
- After SHA-256: `5a2c1d0343b75cc57ce652ad897a67ed403ec623c51f200efd37892132e9220a`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=7385
- Before SHA-256: `3275a57d35b6ed0952c6b5c81c8b0feb14e6046e909253a011aad3b3ba4c9931`
- After SHA-256: `0f5c7e98ed8ecdaf35c7696229b49ee41550e96ae992b77369061b13184a9856`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=15904
- Before SHA-256: `395e076f5b779ef447bf7f20b3b42584729ad57748562162c3c8bd2d55a384d0`
- After SHA-256: `77af689bb82a7a4d7b6b9b609ec06f1695ec4d85eb0d25dbe9aea8457e54af00`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-portable-discovery-bundle.md`
- Summary: bytes=7305
- Before SHA-256: `4ed94beae6a6f959d16a4e70686a2026f7824c9e79845046a760491765a6ec7f`
- After SHA-256: `e46d84d84d513764900ac085fec70254526d6f42e0a83724cf6a30a74d073048`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m8-w01-c01-t1-green-pre-attestation-rs289.md`
- Summary: bytes=2232
- After SHA-256: `ea49ac370a184efbf2439e8b5418b5b976fe1a5a4869e7b2e3a88162d9c8e3b8`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs`
- Summary: bytes=16334
- Before SHA-256: `8533af92e8044eb2ad1bbcdf73085fdc29b2a132278c4246c3cf8df8da61bf96`
- After SHA-256: `8725ad0fa2888f5bec046d70190d613845bdd7724049b9ec8d213315ce98a752`

## 16. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M8-W01-C01-T1-G activated=M8-W01-C01-T1-G
- Before SHA-256: `2c326f1df256f0928f5ab79f3e6af002fefff86bf19144bff32d599fee7f3402`
- After SHA-256: `7bb93f13b86c2702cb620d1c0cc66ee410981bb4ee3bb8a735fbcae79fdd7145`

