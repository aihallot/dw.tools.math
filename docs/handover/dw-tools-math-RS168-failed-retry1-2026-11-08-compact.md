# dw.tools.math — compact DWF handover after RS168 attempt 001 failure

## Authority and operator identity

- Repository: `aihallot/dw.tools.math`; canonical execution branch: `main`.
- Operator checkout: `D:\data.dev\github\ai@hallot.net\dw.tools.math` (the sole authorized local execution checkout).
- Verified remote `main` at handover: `3fe7d8265f182b0a471a72191843e19a8c0fa941` (`docs: reopen RS168 authority after preserved pre-attempt worktree refusal`). Verify fresh GitHub `main` before acting; it supersedes this projection.
- Workflow: DWF `0.1.66` from the last durable failure, repository guidance `0.26`. Read `docs/workflow/mantra.md`, `docs/workflow/constitution.md`, then `PROJECT-MANTRA.md`, `PROJECT-CONSTITUTION.md`, `AGENTS.md` and the pertinent engineering/checklist documents.
- `.aura/workflow/state.json` on `main`: `pendingRun = RS168`. No RS169.
- Expected success commit message: `test: qualify Numerics calendar NuGet candidate with published IR and Quantities`.

## Last authoritative result

- On 2026-11-08, `dwf run next` allocated **RS168 attempt 001**; outcome **failed**, stage **payload**, exit code `-532462766`, no success commit or push.
- CLI next position: **`failed [RS168 retry 1]`**. This is retry 1 of the same pending run, **not retry 0** and not attempt 001 success.
- Durable failure: `refs/heads/workflow/evidence/rs168/run-next-20261108T220321Z-1c6ac27a68ef05fe` at `8f3b0df25315d20b050d9802781b55c7d1e4f969`.
- Terminal handoff: `refs/heads/workflow/handoff/current` at `97d71c10b17e0e97fb1c39a07a4a4671ce21a8b0`; snapshot: `refs/heads/workflow/handoff/invocations/run-next-20261108T220321Z-1c6ac27a68ef05fe`.
- Local reports: `.aura/workflow/local/reports/RS168/attempt-001.json`, `attempt-001.md`; correction receipt: `.aura/workflow/local/corrections/RS168.json`.
- Error: `numerics-candidate exit=1`; `MATH-NUMERICS-CANDIDATE-REFUSED: the run-owned Numerics candidate workspace already exists`. The C# payload fails at its child process call (payload.cs line 291). Its preceding F35 source check, tool restore and tool build succeeded.
- The allocated attempt copied six staged target files into the working tree; no Numerics qualification evidence or successful run result was committed.

## Important earlier failure and retained artifacts

- Prior **pre-attempt** refusal: `run-next-20261108T203219Z-7e1b8d1047662586`, proof commit `ba98adc6186c633cd5ff324d9103fa284edffdce`. No attempt was allocated there. It counts as prior failure, so RS168 cannot be reported first-pass even if corrected.
- See `docs/distribution/rs168-preselection-authority-recovery-20261108.md`. The operator reported a successful bounded stash named `RS168-preflight-preservation` for the original six untracked/modified target paths. **Never apply, pop, drop or otherwise alter this stash automatically.**
- The program's `Qualify` method checks `build/artifacts/rs168-numerics-candidate/{producer,consumer}` for absence and refuses if either exists. The ignored `build/artifacts` contents are not visible via GitHub, so their state and provenance are **not yet established**; do not presume they can be deleted or overwritten.
- Existing package identities, candidate archives and `NUGET_LOCAL` must not be overwritten. This run qualifies an **unpublished** candidate; it does not authorize publication.

## Required next work: correct the SAME run, preserving provenance

1. Read the durable attempt-001 evidence and current remote authority first; inspect the existing `build/artifacts/rs168-numerics-candidate` directory **read-only** in the real operator checkout, distinguishing the producer/consumer scratch trees from any `candidate/*.nupkg` and capturing exact identity/SHA if present. Do not mutate foreign files or old receipts.
2. Audit the complete current RS168 material: `.workflow/runs/RS168/payload.cs`, `run.json`, `preparation-lock.json`, `preparation-proof.json`, `preparation-request.json`, and `staged/tools/dw.tools.math.nuget-numerics-candidate/Program.cs`. Determine the installed DWF correction contract from evidence rather than assuming a direct staged file change is sufficient.
3. Correct the bounded retry/workspace handling without overwriting any pre-existing package, falsifying the eleven command receipts, weakening immutable IR/Quantities package/hash checks, or changing the scope to RS169. An isolated fresh scratch directory or provenance-verified replay may be appropriate **only after** inspecting actual retained artifacts and the output-path/receipt contract.
4. Test the narrowly affected source/build/consumer path first, then DWF-required exact validations. All repository mutations go through the same RS168 correction boundary. No source-version rewrite, no publication, no sibling repository edits and no AURA D3.
5. Once the *remote correction* is in place, the operator's normal command is `dwf run next`. Verify the actual new verdict. Do not announce success, a retry index or a new prepared run from inference.

## Relevant existing success boundaries

- RS167: durable first-pass physical Composition binary qualification; earlier published IR (`20260.20261104.1001`) and Quantities (`20260.20261102.1001`) are the exact dependency closure that RS168 must preserve.
- 541 native project nodes remain completed; the F35 candidate evidence must not claim new project progress.
- Authoring checklist: `docs/engineering/dotnet-authoring-checklist.md`. Resource policy: `docs/planning/resource-cost-policy.md`; no routine broad test suite.
- Original six staged target files: `docs/distribution/ValidateNugetNumericsCandidate.cs`, `docs/distribution/nuget-federation-f35-numerics-candidate.md`, `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props`, `Program.cs`, `dw.tools.math.nuget-numerics-candidate.csproj`, `fixtures/NumericsBinaryConsumer.cs`.

## Handover limits

This is a documentary branch, **not a correction**, not a DWF run, and not a result on `main`. No local checkout inspection or .NET tests were performed during its preparation. GitHub `main` and new durable workflow evidence always override this summary.
