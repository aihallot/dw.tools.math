# RS168 — Same-run preselection authority recovery after local staged-file obstruction

## Durable original RS168 identity

The accepted run remains **RS168**: `test: qualify Numerics calendar NuGet candidate with published IR and Quantities`. It was generated and selected on GitHub `main` at `d8d18ade2b4a708f53f9719982a9eec78114d8a0`. The native state declares `pendingRun = RS168`, and the committed `.workflow/runs/RS168/run.json`, payload, six staged source files, preparation proof, preparation inputs and selection lock are present and unchanged. The 52 committed input preconditions, seven read-only preparation validations and 541 already completed native project nodes retain their original authority. No attempted Numerics package publication or successful RS168 result has been recorded.

## Recorded pre-attempt failure and operator preservation

The 2026-11-08 durable failure `run-next-20261108T203219Z-7e1b8d1047662586` was published at `refs/heads/workflow/evidence/rs168/run-next-20261108T203219Z-7e1b8d1047662586` (commit `ba98adc6186c633cd5ff324d9103fa284edffdce`). It classifies as `pre-attempt / workflow-refusal`, `attemptAllocated = false`, with an unclean primary operator checkout. Its six observed local paths match precisely the six planned Numerics staged target paths. The evidence does **not** establish that local bytes are identical to staged bytes or authorize deleting them.

The operator subsequently executed an explicitly bounded, untracked-inclusive Git stash named `RS168-preflight-preservation`, targeting those six Numerics paths, and reported that it succeeded. This preserves the operator's local files without modifying GitHub product source or claiming the contents were committed. Do not drop, apply, pop, reset or otherwise alter this stash automatically.

## Same-run authority correction

The installed run-next path suppresses a repeat execution at an authority commit for which an already delivered failure receipt exists. The operator's later clean `dwf run next` invocations therefore returned `Next run has not been prepared yet` despite the remote `pendingRun = RS168`. This **documentary recovery commit** creates a new GitHub `main` authority commit with the unchanged, independently verifiable RS168 run material. It changes no selection-lock-covered input, DWF generated run file, run ID, source, fixture, package or workflow state. An updated authority commit permits normal clean `dwf run next` selection without fabricating a failed-attempt correction receipt.

Before execution, the operator checkout must have a clean Git working tree and the normal configured upstream must fast-forward to this exact correction commit. The only normal operator command after that is `dwf run next`; do not manually copy staged files into the repository. If DWF refuses, inspect the **new** published failure reference before changing anything. The prior pre-attempt refusal remains a real prior failure: even a successful allocated attempt 001 must report `firstPassSucceeded = false` and retain the earlier invocation.

This commit authorizes no RS169 preparation, manual package publication, mutation of shared `NUGET_LOCAL`, public registry delivery, source rewrite, rollback change, sibling repository mutation or AURA D3.
