# RS087 Attempt 001 Payload Operations

- Operations: 61
- Changed: 11
- No change: 0
- Verified: 50
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS087 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS086/attempt-001.json` | run=RS086 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json` | pointer=/m6_w01_c01_phase_done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json` | pointer=/m6_w01_work_package_done |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/m6_w01_c02_phase_done |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/m6_w01_work_package_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/status |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/m6_w01_c03_phase_done |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/m6_w01_work_package_done |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/publicly_published |
| 16 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/41/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 27 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 28 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c01-t2/m6-curve-boundary-consumer.csproj` | semantic text found |
| 29 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c01-t2/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj` | semantic text found |
| 31 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c02-t2/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj` | semantic text found |
| 33 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c03-t2/Program.cs` | semantic text found |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=in-progress |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=done |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 41 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w01-local-numerical-gate.md` | path is absent |
| 42 | `require.file-missing` | Verified | `docs/distribution/m6-w01-local-numerical-gate.md` | path is absent |
| 43 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | path is absent |
| 44 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c01-t2/Program.cs` | bytes=5345 |
| 45 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c01-t2/m6-curve-boundary-consumer.csproj` | bytes=531 |
| 46 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c02-t2/Program.cs` | bytes=5254 |
| 47 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj` | bytes=538 |
| 48 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t2/Program.cs` | bytes=5526 |
| 49 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w01-local-numerical-gate.md` | bytes=2000 |
| 50 | `files.replace-from-staged` | Changed | `docs/distribution/m6-w01-local-numerical-gate.md` | bytes=1281 |
| 51 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | bytes=4795 |
| 52 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 53 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01 in-progress->implemented->validated->done |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=done |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 61 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS087 hierarchy=exact
- After SHA-256: `38b4099a851a00f7f615f5abc07a83aa1dc14455849bca836db3a6cfab90ca36`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS086/attempt-001.json`
- Summary: run=RS086 attempt=1 lineage=exact
- After SHA-256: `d036ebbd4d7c3c5057b8f2110cec20217c8eed4a1e3f31d97f2f883e00aaadcd`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_c01_phase_done

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_c02_phase_done

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_c03_phase_done

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/publicly_published

## 16. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/41/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 27. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c01-t2/m6-curve-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c01-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=in-progress
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 41. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w01-local-numerical-gate.md`
- Summary: path is absent

## 42. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-w01-local-numerical-gate.md`
- Summary: path is absent

## 43. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: path is absent

## 44. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c01-t2/Program.cs`
- Summary: bytes=5345
- Before SHA-256: `ab7ccab12bc05c5d39a79942e70c811ae1e98ee8ec825db16736e168cbe87633`
- After SHA-256: `8f0830aede950c744c228fd23b2dc0033c9508e95059550dee42c531a9bb63e4`

## 45. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c01-t2/m6-curve-boundary-consumer.csproj`
- Summary: bytes=531
- Before SHA-256: `ca2791a9bd1c729de46120672bdef421364811ed1d43561afce07d374a6199a1`
- After SHA-256: `49e218e000daa151dd7e8a8bd4d8fcd3dd2149f0db40ade9d8b9fffe29285067`

## 46. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c02-t2/Program.cs`
- Summary: bytes=5254
- Before SHA-256: `028124f6d8c0b9dafda5ad68644b894b7747e7a6298dbd4b8b11462d53c472c2`
- After SHA-256: `970c093fa4ee348605f05b3b2870629eb5d13fdef6018c8f6891fdb8eee16489`

## 47. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj`
- Summary: bytes=538
- Before SHA-256: `01092fe0b2213a5caaef667843aed7ce757b886fdef551e6d1b9d0776ebd6a68`
- After SHA-256: `defc086b19e62cf9197e7a97c9a4bf0b83529ba038938609a0de207e498b87c9`

## 48. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t2/Program.cs`
- Summary: bytes=5526
- Before SHA-256: `cf31fce4dcae680f650713df717034839808dc69ce3f92cf70c16895bb3431d3`
- After SHA-256: `e933d024122a293ba08896b7bd3827004ba786aff72be25f6f43f11b7ff6be30`

## 49. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w01-local-numerical-gate.md`
- Summary: bytes=2000
- After SHA-256: `b94c3e6d251ebb40e0c6ead31869faa89e5bfdee06ed9b8bbf2b834053368cf1`

## 50. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-w01-local-numerical-gate.md`
- Summary: bytes=1281
- After SHA-256: `819a372081dafe5409d98618813343db3c77e0ba39e809a02ecf1904bede3718`

## 51. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: bytes=4795
- After SHA-256: `16695773fccd394e992d8bc9abc1160c02521048cc065f5f9b1d32442696aeff`

## 52. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `cd0477f3ce6101a8d6e6c56e7a8d6c94b30cc8f0fe8a298d2ed8c51a5c6a93bc`
- After SHA-256: `f882756091f85c6b334ee2f09d968ab9cfac75855bd75520b9b5eb7cfd93cd16`

## 53. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 in-progress->implemented->validated->done
- Before SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 61. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `a2cf6af442e63266110f2d672849d6521c18c488e6328c4494a2de88422ac0d0`
- After SHA-256: `096fe42a970d3e3d9e9ec24fa3c66e6eeec858d36d00417c06e99f1565dbcfbf`

