# RS155 — Scientific private-release output contract correction

## Why this bounded repair is necessary

RS154 succeeded first-pass at `c6621ea1c5032c8e52da391bd233e0e0df5f1ff1`, attempt 001, with DWF `0.1.63`. F21 qualified an immutable private publisher rehearsal on a **disposable** feed: 15 independent Scientific binary cases, zero dependencies, refusal of duplicate identity, altered digest and cancellation, and no write to `NUGET_LOCAL`.

An owner-facing audit of the installed `tools/dw.tools.math.nuget-scientific-release/Program.cs` immediately after RS154 found two **diagnostic strings** inherited from the Foundation controller:

- `--publish` correctly checks the actual original **37,265-byte** Scientific candidate by size and hashes, but the refusal message erroneously names `3,863-byte`.
- `--verify` actually runs the original **fifteen** binary tests and checks the exact published archive, but its success line erroneously prints `binary=5` instead of `binary=15`.

The F20-qualified original Scientific archive is `dw.tools.math.scientific.20260.20261105.1001.nupkg`, size **37,265**, archive SHA-256 `233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f`, net10.0 DLL SHA-256 `604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c`, original source Version `1.1.0-preview.14`, exactly 16 original Scientific C# files, csproj and lockfile, zero dependencies and 15 source-free oracles.

## Exact repair and qualification boundary

RS155 performs a **two-substitution-only** correction to the Math-owned release controller:

1. `only the exact original 3,863-byte RS153 Scientific candidate may be published` becomes `only the exact original 37,265-byte RS153 Scientific candidate may be published`.
2. `|binary=5|archive=` becomes `|binary=15|archive=` in the `MATH-SCIENTIFIC-VERIFY` success message.

These strings are owner-facing evidence and refusal diagnostics, not a change in publication implementation or the admitted archive. The existing `IsExactPhysical(original, 37265, CandidateSha)`, `PackageDllHash(original) == CandidateDllSha`, `RequireCleanOwnerAndRs154`, loaded immutable `Dw.Tools.NuGet [20260.20261026.1010]` identity guard and original fifteen binary oracles must remain intact.

The run installs complete staged `Program.cs` bytes through DWF, validates the exact message contract using an observational C# source/receipt validator, then restores and compiles the non-packable release controller **only from existing `NUGET_LOCAL` dependencies** without running any `--publish`, `--verify` or `--rehearse` mode. Its F22 evidence records the actual source hash, successful restore and build receipts and preservation of the six previously published Math packages and 541 already completed native nodes.

## Limits and release next action

This repair is deliberately not a new publisher rehearsal, private package publication, feed audit or rollback qualification. F20 and F21 evidence remain unchanged, the original Scientific candidate and historical source files remain immutable, and no other repository is modified. The owner can separately invoke guarded `--publish` and `--verify` **only after RS155 is durably succeeded**, on clean `main`, with the original exact candidate and private source-owner rights. The guarded tool itself continues to require the successful RS154 report, and the future independent physical-feed readback remains a separate DWF objective after the owner provides publication and verification outputs.
