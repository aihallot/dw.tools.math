# RS150 Attempt 001 Payload Operations

- Operations: 35
- Changed: 6
- No change: 0
- Verified: 29
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS150 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS149/attempt-001.json` | run=RS149 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M0 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | pointer=/observation/compiledConsumerChecks |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 14 | `require.text-contains` | Verified | `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj` | semantic text found |
| 15 | `require.text-contains` | Verified | `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj` | semantic text found |
| 16 | `require.text-contains` | Verified | `src/projects/dw.tools.math.foundation/FoundationContract.cs` | semantic text found |
| 17 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate.md` | path is absent |
| 18 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetFoundationCandidate.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/Program.cs` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | path is absent |
| 23 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f17-foundation-candidate.md` | bytes=4200 |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetFoundationCandidate.cs` | bytes=10782 |
| 25 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj` | bytes=281 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-candidate/Program.cs` | bytes=15799 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs` | bytes=2029 |
| 28 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj` | semantic text found |
| 33 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | bytes=11019 |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M0 state=done |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS150 hierarchy=exact
- After SHA-256: `13d5813b19e3412f28769d1d5e9d5a643c83b28d57c39578fb3c086b37349e3a`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS149/attempt-001.json`
- Summary: run=RS149 attempt=1 lineage=exact
- After SHA-256: `3c9fa272230576718f0db0c4ff2441da6a4d0c956c1f08bc7255537f2e005f61`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M0 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
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
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: pointer=/observation/compiledConsumerChecks

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 14. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 15. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.foundation/FoundationContract.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate.md`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetFoundationCandidate.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/Program.cs`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: path is absent

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate.md`
- Summary: bytes=4200
- After SHA-256: `5a77bd76c2eaa5fa2dad25d06f50cdde1c614a96247ef4238704fddf27bdcb80`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetFoundationCandidate.cs`
- Summary: bytes=10782
- After SHA-256: `5b9ae3657a63dabb38e9f7ecd683a9748d72a452d8855d5bd96a4dca6e6c5ec2`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/Program.cs`
- Summary: bytes=15799
- After SHA-256: `2837890ca73fc676648640027742ef6780d64d4dea85ebdd6de10585f32f98af`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs`
- Summary: bytes=2029
- After SHA-256: `002a06230229eae81057f7933b56c425b82d9bb4b305331ffd6f8f6b1a3d367e`

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/dw.tools.math.nuget-foundation-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: bytes=11019
- After SHA-256: `fe9e6753cd6496c60ad0228320543f41d4c1f6722b7881655ee4209b3d7727ca`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M0 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

