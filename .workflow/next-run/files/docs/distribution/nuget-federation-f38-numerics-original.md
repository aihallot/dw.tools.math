# RS171 — Verify the original F35 Numerics archive with a new binary consumer

## Authority and immutable bytes

RS170 succeeded at attempt 001 on GitHub main at commit `3e94acda019b62f19078f4ab70e4d738249cfdb7`, but its preselection first-pass failed once before an attempt was allocated. F37 physically verified the F35 original Numerics archive without authorizing publication. The original `dw.tools.math.numerics [20260.20261109.1001]` has exactly 18,313 bytes, SHA-256 `f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d`, and packaged Numerics DLL SHA-256 `77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4`. The F36 rebuilt archive has different bytes and is never an acceptable substitute.

The F35 source project, three source files and lockfile remain immutable. The sole direct package dependency is IR `20260.20261104.1001`, archive SHA-256 `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480`, with transitive Quantities `20260.20261102.1001`, SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`.

## Fresh original package-only consumer

The Math-owned F38 controller checks F35/F37 predecessor identities and exact source hashes, the original physical F35 ZIP/Nuspec/dependency graph, and both private dependency archives. It copies only the original byte-identical F35 archive into a new ignored disposable flat NuGet feed (non-overwriting), then generates an independent `net10.0` consumer with only one exact Numerics PackageReference. The existing binary fixture is compiled from source inside this consumer, **not** inside the Numerics package. Its NuGet cache is new and initially empty.

The consumer restores from the disposable original archive plus the read-only published IR and Quantities dependencies, and executes fifteen independent binary probes including actual Numerics DLL SHA-256, LU/solve and approximate IR roundtrip. The source archive, disposable copy and both dependency archives are rehashed after the consumer.

DWF prepares and compiles the controller in a disposable Git clone, which does **not** contain ignored physical F35 artifacts from the operator checkout. RS171 therefore performs the physical archive and fifteen-probe qualification as a **focused post-attempt validation on the original operator checkout**. It does not run or falsely claim the binary test during preflight. The four real inner commands and the exact observed hashes are emitted as structured JSON by that validation, retained in the DWF RS171 attempt report. The source-only payload writes no premature F38-qualified evidence artifact. Any later durable F38 readback receipt must cite the successful focused DWF validation, not an assumed preflight result.

## Limits and next gate

This run does **not** rebuild or repackage Numerics, write to `NUGET_LOCAL`, publish to any registry, change source versions, select a public license, qualify installed rollback, modify other repositories or authorize AURA D3. All 541 native Math nodes remain complete. Only an explicit owner decision may authorize the already-qualified immutable original-archive private publisher; physical feed readback then requires independent evidence.
