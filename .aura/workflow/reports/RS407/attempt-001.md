# RS407 Attempt 001 Payload Operations

- Operations: 7
- Changed: 3
- No change: 0
- Verified: 4
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS406/attempt-002.json` | run=RS406 attempt=2 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-G state=in-progress |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-V state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C02 state=not-ready |
| 5 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM10IsolatedSolutionGraph.cs` | bytes=28103 |
| 6 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m10-isolated-rollback-rs407.md` | bytes=1616 |
| 7 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM10MigrationAdmission.cs` | bytes=28517 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS406/attempt-002.json`
- Summary: run=RS406 attempt=2 lineage=exact
- After SHA-256: `be59cf5b39951172fa78fb13feb9167f2ae66d0afd18ed216fb363fc5ad9606c`

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
- Summary: bytes=28103
- Before SHA-256: `0315133082ecd9dc7534e2897ad500849e099487efb18609b1edae5dfd679396`
- After SHA-256: `39d51d379f0a934ac9bfad96633114624795aadcb7698dfb4efce2fd33ef66f9`

## 6. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m10-isolated-rollback-rs407.md`
- Summary: bytes=1616
- After SHA-256: `2c4ee5043c3c9691a18174600aecf88d52d97a008236e6b1bd2d3958af86a373`

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM10MigrationAdmission.cs`
- Summary: bytes=28517
- Before SHA-256: `e1c9d436e5843f509d9de1db137fd0da70bbfd049aff5f6678fe56bfe53471e5`
- After SHA-256: `26a69e8ef2dd8c23e7dd428e4989e0d17f37db5d40aaee6210be82f5dce541f2`

