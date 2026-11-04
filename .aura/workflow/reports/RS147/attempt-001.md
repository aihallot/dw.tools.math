# RS147 Attempt 001 Payload Operations

- Operations: 33
- Changed: 6
- No change: 0
- Verified: 27
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS147 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS146/attempt-001.json` | run=RS146 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 13 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 14 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 15 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate.md` | path is absent |
| 16 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetAnalysisCandidate.cs` | path is absent |
| 17 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj` | path is absent |
| 18 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/Program.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | path is absent |
| 21 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f14-analysis-candidate.md` | bytes=4248 |
| 22 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetAnalysisCandidate.cs` | bytes=9629 |
| 23 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj` | bytes=281 |
| 24 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-candidate/Program.cs` | bytes=15535 |
| 25 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs` | bytes=5482 |
| 26 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate.md` | semantic text found |
| 27 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/Program.cs` | semantic text found |
| 28 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/Program.cs` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj` | semantic text found |
| 31 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | bytes=12176 |
| 32 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS147 hierarchy=exact
- After SHA-256: `c8a267c7bd4d7d1caebfd9e202ce3d8215a2c8c0ebc7342dd0b321ffe4f3f338`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS146/attempt-001.json`
- Summary: run=RS146 attempt=1 lineage=exact
- After SHA-256: `4489f8c86528453a21d54b36ce7deca34e29dcbcdb8bb1c089a866a4d20c8b3f`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 13. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 14. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate.md`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetAnalysisCandidate.cs`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/Program.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: path is absent

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate.md`
- Summary: bytes=4248
- After SHA-256: `0743ce53fbb6abc40740e5f8f70730382efdd688f800d5629205b9394005857a`

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetAnalysisCandidate.cs`
- Summary: bytes=9629
- After SHA-256: `3fb111f925c9a48b6193dae195f52a4aaa70fb664d6c9cbc1b0ad41c0bc037f3`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/Program.cs`
- Summary: bytes=15535
- After SHA-256: `6703d44806b42d38ffeb1bcf5a1e87dad5ec19772447c25d6e46c49bc8e689ef`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs`
- Summary: bytes=5482
- After SHA-256: `928bba4c8f33911f8cb303ffc42e94aed5fe87b356838eb874f7e5072b985947`

## 26. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/dw.tools.math.nuget-analysis-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: bytes=12176
- After SHA-256: `fefd3cea9b1bbebc0ee52916cf4e3608e2db0b1f32ff33e1f71e2b4a86f09c52`

## 32. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

