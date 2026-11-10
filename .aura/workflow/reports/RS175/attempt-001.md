# RS175 Attempt 001 Payload Operations

- Operations: 26
- Changed: 6
- No change: 0
- Verified: 20
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS175 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS174/attempt-001.json` | run=RS174 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/candidate/consumerChecks |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/package_published |
| 13 | `require.file-missing` | Verified | `docs/distribution/electrochemical-publisher.md` | path is absent |
| 14 | `require.file-missing` | Verified | `docs/distribution/ValidateElectrochemicalPublisher.cs` | path is absent |
| 15 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-release/dw.tools.math.nuget-electrochemical-release.csproj` | path is absent |
| 16 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-release/Directory.Packages.props` | path is absent |
| 17 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-release/Program.cs` | path is absent |
| 18 | `require.file-missing` | Verified | `docs/distribution/electrochemical-publisher-qualified.json` | path is absent |
| 19 | `files.replace-from-staged` | Changed | `docs/distribution/electrochemical-publisher.md` | bytes=3605 |
| 20 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateElectrochemicalPublisher.cs` | bytes=5639 |
| 21 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-release/dw.tools.math.nuget-electrochemical-release.csproj` | bytes=331 |
| 22 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-release/Directory.Packages.props` | bytes=241 |
| 23 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-release/Program.cs` | bytes=14811 |
| 24 | `files.write-complete` | Changed | `docs/distribution/electrochemical-publisher-qualified.json` | bytes=7051 |
| 25 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 26 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS175 hierarchy=exact
- After SHA-256: `5f43e3a83732c10bf7b1772c13e43125cb2ca7600e2ebf3a9762ccea4bb7ccaa`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS174/attempt-001.json`
- Summary: run=RS174 attempt=1 lineage=exact
- After SHA-256: `f416012a4d4d0ce0f448b44f44a41dfa75784570d96465148eddcfac3992f03d`

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
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/candidate/consumerChecks

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/package_published

## 13. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-publisher.md`
- Summary: path is absent

## 14. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateElectrochemicalPublisher.cs`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/dw.tools.math.nuget-electrochemical-release.csproj`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/Directory.Packages.props`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/Program.cs`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-publisher-qualified.json`
- Summary: path is absent

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-publisher.md`
- Summary: bytes=3605
- After SHA-256: `9d5024da8452a38e8893011767f5def0358af4e6f8be9eec1d1a17614992af27`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateElectrochemicalPublisher.cs`
- Summary: bytes=5639
- After SHA-256: `9e0f90d4bb78b7ba2b00095e79bbe6631b5febb482d30427adfd343e3e91acef`

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/dw.tools.math.nuget-electrochemical-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/Program.cs`
- Summary: bytes=14811
- After SHA-256: `44235e3344b629f8f4b9e0514f9ad787d7525c87b6ef1c81da46052006e78383`

## 24. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-publisher-qualified.json`
- Summary: bytes=7051
- After SHA-256: `e5b293355963798247bc9deba64cedb749fbec45719f38595c6fee9465a12b2c`

## 25. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M1 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 26. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

