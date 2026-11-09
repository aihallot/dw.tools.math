# RS171 Attempt 001 Payload Operations

- Operations: 24
- Changed: 4
- No change: 0
- Verified: 20
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS171 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS170/attempt-001.json` | run=RS170 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f37-numerics-release-qualified.json` | pointer=/status |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f37-numerics-release-qualified.json` | pointer=/controller/originalCandidatePhysicallyVerified |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f37-numerics-release-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 14 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs` | file exists |
| 15 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f38-numerics-original.md` | bytes=3217 |
| 16 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetNumericsOriginal.cs` | bytes=3587 |
| 17 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-original-consumer/dw.tools.math.nuget-numerics-original-consumer.csproj` | bytes=281 |
| 18 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs` | bytes=16630 |
| 19 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f38-numerics-original.md` | semantic text found |
| 20 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs` | semantic text found |
| 21 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs` | semantic text found |
| 22 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs` | semantic text found |
| 23 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-original-consumer/dw.tools.math.nuget-numerics-original-consumer.csproj` | semantic text found |
| 24 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS171 hierarchy=exact
- After SHA-256: `51235bb24a847fa8cc21a5e731ec8e79ae5a66117fa5c758e5c59363660847ac`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS170/attempt-001.json`
- Summary: run=RS170 attempt=1 lineage=exact
- After SHA-256: `6d628d8eacc0751fa3188c6ba63b0158f173cc06e1d9759c6644dc3f3a2155a9`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
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
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release-qualified.json`
- Summary: pointer=/status

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release-qualified.json`
- Summary: pointer=/controller/originalCandidatePhysicallyVerified

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 14. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
- Summary: file exists

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f38-numerics-original.md`
- Summary: bytes=3217
- After SHA-256: `5dfe34ec66a948fbaf8bc8b3f54877b75dd161ed3a90cc3881e699402a89b3e3`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetNumericsOriginal.cs`
- Summary: bytes=3587
- After SHA-256: `3de9fec57844061235025ac769dd1922f3d9fc645e1884029beb275c410304a4`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-original-consumer/dw.tools.math.nuget-numerics-original-consumer.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs`
- Summary: bytes=16630
- After SHA-256: `ffbc9b29c4a4aed5635d1bfbbacb89312decb017806300a3a9b2e2c10d447190`

## 19. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f38-numerics-original.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 20. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 21. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 22. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-original-consumer/dw.tools.math.nuget-numerics-original-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

