# RS329 — Start isolated Foundation source GREEN without production migration

## Evidence and selected candidate

RS328 succeeded first-pass under DWF 0.1.72 and guidance 0.27, closing C02/T1 RED on six fail-closed migration refusals. The prospective migration profile is still blocked-not-authorized: thirteen audited source libraries, only three SHA-pinned private published package closures, ten unpublished/unqualified closures, and thirteen unqualified API/behavioral compatibility verdicts. Do not interpret an active GREEN leaf as migration authorization.

Select dw.tools.math.foundation as the smallest standalone source candidate: the existing FoundationContract.cs exposes one exact PackageId, ContractVersion and Identity, and the source csproj has no ProjectReferences. Historical RS218 verified a net11-only source consumer; RS329 adds a new independent net10/net11 differential oracle and copy-only contract-drift refusals without replaying a broad prior campaign.

## Controlled cross-SDK GREEN proof

The new Math-owned C# verifier reads the original one-file Foundation source and SDK/migration metadata, hashes them, and writes two disposable package-free consumers under short build/artifacts/f329-* roots. Both use the same unchanged source, compile and run under SDK 10/net10/C#14 and SDK 11.0.100/net11/C#15 respectively, and must emit exactly the same public identity and version oracle. The net11 copied-source branch must refuse a changed contract version and an altered PackageId at runtime, then accept restored original bytes after each test. Existing original source, project declaration, migration authorities and production SDK settings must retain their SHA-256.

## Exact state and limitations

RS329 activates only M8-W02-C02-T1-G, leaving the parent task and phase in-progress, RED done and Verification plus C02/T2 not-ready. No node is completed and no achievement is asserted. This verifies only Foundation source compilation and its public contract across two runtimes; it does not validate any new C#15 syntax, full numerical semantics, packaged binary compatibility, distribution RID/platform behavior, rollback, or production migration of all thirteen libraries. The production global.json remains SDK 10.0.401/latestPatch, central net10.0/C#14 and private NuGet archives stay immutable. No public release or external broker grant is authorized.
