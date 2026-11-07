# RS162 — Immutable Codecs NuGet publisher rehearsal with direct IR and transitive Quantities

## Exact authority and bounded outcome

RS161 succeeded on GitHub main at `0733e6018ecd5dd8afc41d4b7e5f000fef3b154d`, DWF 0.1.66, attempt 001. The report retains one corrected pre-attempt refusal, zero allocated attempt failures, and no first-pass claim. F28 qualified the **unpublished** `dw.tools.math.codecs [20260.20261107.1001]` original candidate: **35,096 bytes**, archive SHA-256 `00392fbeddebceb70286b11377aeab120bef0e3ef45f9b12dc2ba878918f8e35`, and packaged DLL SHA-256 `3e8ba8e03bf6519de28837b1e01d9842e5e76bfee6a6ad9cceee442b47f2fe74`. Its nine unchanged original Codecs C# files and original source project preserve Version `1.0.0-preview.8`, with a single `ProjectReference` to IR.

The exact privately published direct dependency is `dw.tools.math.ir [20260.20261104.1001]`, 21,926 bytes, SHA-256 `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480`, DLL `5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af`. IR's one transitive dependency is `dw.quantities [20260.20261102.1001]`, 14,747 bytes, SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`, DLL `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`. Independent F13 and F4 evidence is retained. The NuGet provider is pinned to `Dw.Tools.NuGet [20260.20261026.1010]` with exact loaded DLL identity.

## Bounded F29 rehearsal

The Math-owned `tools/dw.tools.math.nuget-codecs-release` controller validates the exact RS161 candidate receipt, original source and lockfile hashes, F13's direct IR and transitive Quantities graph, and physical unmodified IR and Quantities archives. It packages only nine unchanged Codecs source files through a temporary `net10.0` project with one pinned direct IR `PackageReference` and a transitive Quantities closure. The original Codecs project and `packages.lock.json` are not rewritten.

RS162 executes **only `--rehearse`** in the disposable DWF checkout. The shared provider's `PreparePublication` and `Publish` write a new rehearsal archive **only** into a fresh disposable feed under ignored `build/artifacts`, then read back its actual hash and exact single IR dependency. Collision, altered expected SHA-256 and cancellation are independently refused. A new source-free consumer restores only the disposable Codecs package and published dependencies; it executes the unchanged **15 independently compiled binary checks** including actual loaded DLL SHA and refusal boundaries. Seven inner real commands and four outer DWF command receipts are required. The shared `NUGET_LOCAL` feed is read-only; exact IR and Quantities archive hashes are checked before and after.

## Separate owner-only publication boundary

The controller exposes `--publish` and `--verify`, but neither is called by RS162. They require a succeeded RS162 report with its expected Git commit message, source-owner rights attestation and a clean `main` checkout. Publication is further restricted to the **original exact 35,096-byte RS161 candidate archive** and its pinned SHA, not a newly rebuilt rehearsal archive; the original archive must be physically available. An ambiguous partial publication must be inspected before retrying. A future explicitly owner-controlled private release, followed by an independent DWF physical-feed readback, is a distinct gate.

No shared private publication, remote registry, public licensing, installed rollback, unrelated repository mutation, Decision/Discovery migration, or AURA D3 integration is authorized by RS162. The 541 completed native nodes, previous eight independently verified Math private packages and existing source contracts remain unchanged.
