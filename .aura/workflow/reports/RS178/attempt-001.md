# RS178 Attempt 001 Payload Operations

- Operations: 16
- Changed: 4
- No change: 0
- Verified: 12
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS178 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS177/attempt-001.json` | run=RS177 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-published-readback.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-published-readback.json` | pointer=/auraIntegrationVerified |
| 7 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-published-readback.json` | pointer=/auraHistoricalReuseRightsEstablished |
| 8 | `require.file-missing` | Verified | `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md` | path is absent |
| 9 | `require.file-missing` | Verified | `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json` | path is absent |
| 10 | `require.file-missing` | Verified | `docs/coordination/requests/ValidateMathXr006AuraElectrochemical.cs` | path is absent |
| 11 | `files.replace-from-staged` | Changed | `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md` | bytes=3455 |
| 12 | `files.replace-from-staged` | Changed | `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json` | bytes=3829 |
| 13 | `files.replace-from-staged` | Changed | `docs/coordination/requests/ValidateMathXr006AuraElectrochemical.cs` | bytes=2730 |
| 14 | `files.converge-from-staged` | Changed | `docs/coordination/requests/README.md` | bytes=577 |
| 15 | `require.json-value-equals` | Verified | `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json` | pointer=/status |
| 16 | `require.json-value-equals` | Verified | `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json` | pointer=/transmission |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS178 hierarchy=exact
- After SHA-256: `1b851ef2108d14a12d4ba246f8fd14d888b11cf0e0dfc0c95be968d745ae876e`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS177/attempt-001.json`
- Summary: run=RS177 attempt=1 lineage=exact
- After SHA-256: `ab74c74a6419697695a0801babd97634f35d225ae6cbbb7852f020d6a83ce0aa`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M1 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: pointer=/auraIntegrationVerified

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: pointer=/auraHistoricalReuseRightsEstablished

## 8. `require.file-missing`

- Status: `Verified`
- Path: `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md`
- Summary: path is absent

## 9. `require.file-missing`

- Status: `Verified`
- Path: `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json`
- Summary: path is absent

## 10. `require.file-missing`

- Status: `Verified`
- Path: `docs/coordination/requests/ValidateMathXr006AuraElectrochemical.cs`
- Summary: path is absent

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md`
- Summary: bytes=3455
- After SHA-256: `fb9757360ace8f4e0a28e1a2b02218effaab403574d375dd9b793b9717fc2cc6`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json`
- Summary: bytes=3829
- After SHA-256: `5c74e85a708f184fed3cb2742657c3bee01ae8b1154db5f2662531b2dcc2dbbd`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/coordination/requests/ValidateMathXr006AuraElectrochemical.cs`
- Summary: bytes=2730
- After SHA-256: `1ff7a7423fdddd55e9e39a9209bdd08514d1809731649de37f3a18f36a6c22ef`

## 14. `files.converge-from-staged`

- Status: `Changed`
- Path: `docs/coordination/requests/README.md`
- Summary: bytes=577
- Before SHA-256: `898ebd7a2f9c6db844711d9e7c7b2770ae42feb8cf1b7e45f07160ce0a0af844`
- After SHA-256: `867f0c63b73236e50e26e2faec6b6651f7e8dd0b068bf6dad7ac0ff4073af60f`

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json`
- Summary: pointer=/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json`
- Summary: pointer=/transmission

