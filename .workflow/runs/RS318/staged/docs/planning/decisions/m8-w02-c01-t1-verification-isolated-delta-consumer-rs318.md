# RS318 — Activate independent SDK candidate-inventory Verification

## Proven prerequisite

RS317 succeeded at allocated attempt 001, first-pass, with no prior invocation failures. It completed C01/T1 GREEN for the candidate-only Math SDK 11/C# 15 official delta mapping and readied Verification, without qualifying any actual runtime compatibility. Existing RED evidence RS314/RS315 covers stable toolchain observation, thirteen packable sources, sixteen direct dependency edges and eleven directly covering test projects. Existing GREEN evidence RS316/RS317 retains nine original proposal candidate IDs, four Microsoft reference categories, and all thirteen unqualified source owners.

## Independent copied-consumer qualification

Reuse the existing Math-owned `ValidateM8W02OfficialDeltaMatrix.cs` without a second risk-mapping implementation. A new bounded C# harness copies this verifier and its minimal 29 inputs to a disposable independent consumer root under `build/artifacts/`: production `global.json` and `Directory.Build.props`, the original proposal JSON, candidate matrix JSON, the verifier, thirteen source-project declarations and eleven direct test-project declarations. Verify byte-for-byte copies and execute the original standalone `dotnet run --file` verifier against the clean copied root.

In the copied candidate matrix only, independently inject a forged proposal risk ID, remove an existing source library row, set a compatibility verdict to `qualified`, and substitute an unrelated test owner. Each injection must fail by the original verifier's existing exact refusal reason, then restoring the original bytes must allow positive verification again. Require original Math source and proposal/configuration bytes to remain unchanged, including no SDK pin or TFM modification.

## Native scope and limits

RS318 activates only `M8-W02-C01-T1-V` from ready to in-progress. It does not complete Verification or its parent task, ready T2, migrate production to .NET 11, repack existing NuGet binaries, approve new syntax or claim a broker grant. Negative controls qualify documentary identity/coverage only, not numerical behavior, platform/RID deployment or actual API applicability. All compatibility labels remain explicitly unqualified.
