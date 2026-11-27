# RS404 Attempt 001 Payload Operations

- Operations: 7
- Changed: 3
- No change: 0
- Verified: 4
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS403/attempt-001.json` | run=RS403 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-G state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-V state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C02 state=not-ready |
| 5 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM10IsolatedSolutionGraph.cs` | bytes=10116 |
| 6 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m10-isolated-fragile-tests-rs404.md` | bytes=1351 |
| 7 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=20647 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS403/attempt-001.json`
- Summary: run=RS403 attempt=1 lineage=exact
- After SHA-256: `3fee0f0fdd16985a5087708f6aec4b3b99eec7e03e84040627efc3f36dab67f3`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-G state=in-progress
- After SHA-256: `57d79c69181d99ea15bbd10d132544a313059a5dcf28c306557a476aaeb8035b`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-V state=not-ready
- After SHA-256: `57d79c69181d99ea15bbd10d132544a313059a5dcf28c306557a476aaeb8035b`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C02 state=not-ready
- After SHA-256: `57d79c69181d99ea15bbd10d132544a313059a5dcf28c306557a476aaeb8035b`

## 5. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM10IsolatedSolutionGraph.cs`
- Summary: bytes=10116
- Before SHA-256: `446ac04dd50a0de1ba16a87e8ac14da1cedbda8d0ac922467986f398ff286306`
- After SHA-256: `0a5feb0ede9ff499a341cb77d9388c1dc718f9ae719cac85968111238dcdb46a`

## 6. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m10-isolated-fragile-tests-rs404.md`
- Summary: bytes=1351
- After SHA-256: `dd909e100726c41df7155eab85776b3ca262e85becca3be4dbe0b4fce8deed52`

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=20647
- Before SHA-256: `11657f1d9b333f8b1f2e3de386a6209a0f1e060074d037fe1682e48e856f6412`
- After SHA-256: `8e74dbb0d48157c9925564a2e1459eee6e396247cf18fda11013e96dee0768f0`

