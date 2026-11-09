# RS170 — Exact original Numerics immutable private release controller

## Prior source and binary authority

RS169 succeeded on GitHub main at `4887094c0265d676b41d09f3191efa6f0b64408e` (attempt 002; one allocated failed attempt). Its F36 receipt demonstrates a private-feed-neutral, disposable Numerics publication rehearsal and fifteen source-free binary probes. It does not authorize shared-feed publication. The rebuilt F36 archive has SHA-256 `08373c347f3d869377ba78e56ef057b0f90370ccf446959c3d0435d9e16629f9`, not the original immutable F35 SHA-256.

The original F35 candidate is `dw.tools.math.numerics [20260.20261109.1001]`, **18,313 bytes**, archive SHA-256 `f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d`, packaged Numerics DLL SHA-256 `77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4`. Its single exact direct dependency is published IR `20260.20261104.1001`, archive SHA-256 `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480`; transitive Quantities `20260.20261102.1001` has archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`. These packages and source project/lockfile identities remain unchanged.

## Qualified controller and bounds

RS170 installs a Math-owned `dw.tools.math.nuget-numerics-release` tool with three distinct modes: `--qualify`, `--publish`, and `--verify`. DWF executes **only `--qualify`**, which checks F35/F36 and their successful DWF lineage, exact original source hashes, pinned NuGet reader, two published dependency package hashes, and strict refusal of an invalid candidate in an isolated scratch directory. It records whether the original ignored F35 archive was physically available in this environment; absence or failure of that optional observation never becomes an invented release claim.

The later mutating `--publish` mode must not be called by RS170. It requires a successful RS170 report, owner rights, clean operator `main`, exact original F35 archive path, physical archive byte count, full SHA-256, Nuspec ID/version and sole pinned IR dependency, and packaged DLL SHA-256. The existing shared `DotNetLocalPackageRepository` publisher uses an exact no-overwrite publication. A prior package or receipt refuses rather than being replaced. On successful publication it produces a local pending-verification receipt, not an independent physical-feed attestation.

The later `--verify` mode checks a physically installed exact F35 archive on the private feed and exercises the same fifteen binary probes through a freshly restored, package-only consumer. Independent DWF physical feed readback remains a separate successor gate.

## Release boundary

This run does not publish to shared `NUGET_LOCAL`, choose a new public license, authorize a public registry, change source versions, qualify installed rollback, modify sibling repositories or authorize AURA D3. A qualifying controller is not approval to release. **Only separate explicit owner authorization** can initiate private publication, and even then missing or mismatched F35 bytes refuse. The F36 rehearsal archive must never be substituted. All 541 native Math nodes remain complete.
