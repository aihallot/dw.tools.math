# RS369 Attempt 001 Payload Operations

- Operations: 9
- Changed: 2
- No change: 0
- Verified: 7
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS368/attempt-001.json` | run=RS368 attempt=1 lineage=exact |
| 2 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10 state=ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M10-W01-C01-T1-R state=ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/InspectDecisionNumericSubsidiarity.cs` | bytes=5819 |
| 9 | `files.replace-from-staged` | Changed | `docs/planning/decisions/decision-numeric-subsidiarity-rs369.md` | bytes=3884 |

## 1. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS368/attempt-001.json`
- Summary: run=RS368 attempt=1 lineage=exact
- After SHA-256: `aff12c85d172f865321b595d5c1f425fa791f24614718bc684b5e254f28d46f2`

## 2. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `97dda0051b9840a0abb60efb70b1f0b556e92446a44148b32206fb272e982cc3`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=done
- After SHA-256: `97dda0051b9840a0abb60efb70b1f0b556e92446a44148b32206fb272e982cc3`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10 state=ready
- After SHA-256: `97dda0051b9840a0abb60efb70b1f0b556e92446a44148b32206fb272e982cc3`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M10-W01-C01-T1-R state=ready
- After SHA-256: `97dda0051b9840a0abb60efb70b1f0b556e92446a44148b32206fb272e982cc3`

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/InspectDecisionNumericSubsidiarity.cs`
- Summary: bytes=5819
- After SHA-256: `b9030c9756edd38472fbf3a414ac29fb6b9274dc3bf9a4be46f3ce3c3a006fed`

## 9. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/decision-numeric-subsidiarity-rs369.md`
- Summary: bytes=3884
- After SHA-256: `bc189742bdca608e49dc8e9cc5bc4131162a701e7a2931443e7c1f2d7f4f5d37`

