# RS153 Attempt 001 Payload Operations

- Operations: 35
- Changed: 6
- No change: 0
- Verified: 29
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS153 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS152/attempt-001.json` | run=RS152 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | pointer=/observation/compiledConsumerChecks |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 14 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 15 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 16 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/BoundedExactScientific.cs` | semantic text found |
| 17 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate.md` | path is absent |
| 18 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetScientificCandidate.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/Program.cs` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | path is absent |
| 23 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f20-scientific-candidate.md` | bytes=4816 |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetScientificCandidate.cs` | bytes=10801 |
| 25 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj` | bytes=281 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-candidate/Program.cs` | bytes=17069 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs` | bytes=5343 |
| 28 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj` | semantic text found |
| 33 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | bytes=13788 |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS153 hierarchy=exact
- After SHA-256: `7fd33a5280986afae1db07392cd31281862620179bb06c2ea409bef69cb2af97`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS152/attempt-001.json`
- Summary: run=RS152 attempt=1 lineage=exact
- After SHA-256: `bdad4a22376c8ccf555565e264e851fab3456524bf1001b8dee829cb5c9df2d7`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
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
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
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
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 15. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/BoundedExactScientific.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate.md`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetScientificCandidate.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/Program.cs`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: path is absent

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate.md`
- Summary: bytes=4816
- After SHA-256: `229312e9d185d2b8812c6b2bc338e77fcdf6e958a8940834533acff4506abe7b`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetScientificCandidate.cs`
- Summary: bytes=10801
- After SHA-256: `5b27e057c493fb21547a38d80d8dc4eed4e76144ca78fbb0bc4694f379e2a205`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/Program.cs`
- Summary: bytes=17069
- After SHA-256: `3725b9e2889fd9240be2d7ff41fbe3df87130961ac4ae0526a362cf6e7ade042`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs`
- Summary: bytes=5343
- After SHA-256: `1e1c1a70e337d2f26c230d8f459641064c6eb98478a6ed14422fda11ba4eae5a`

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/dw.tools.math.nuget-scientific-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: bytes=13788
- After SHA-256: `800ece228643768c3df611d1c142f4e3bdcf635b0ae439170b47ea92eea5dc5e`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

