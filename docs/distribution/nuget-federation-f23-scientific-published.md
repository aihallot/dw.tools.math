# RS156 — Independent Scientific physical-feed readback

## Owner publication and DWF predecessor

Math `RS155` succeeded on GitHub `main` at `158b3629a71777be17660cd42038c87732f44467`, DWF `0.1.63`, attempt 001, first-pass success with no prior invocation failures. Its F22 receipt proves that the installed Scientific release controller was compiled and that its two misleading owner messages were corrected by exactly two textual substitutions from the RS154 original. F22 does not prove a shared-feed publication.

On **2026-11-06 at 08:25 Europe/Brussels**, the owner separately provided a clean-`main` MSI-02 PowerShell transcript with three successful steps. The controller's `Dw.Tools.NuGet [20260.20261026.1010]` package was restored from `NUGET_LOCAL` using an explicit `--source`; the guarded `--publish` reported:

`MATH-SCIENTIFIC-PUBLISHED|id=dw.tools.math.scientific|version=20260.20261105.1001|sha256=233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f|dependencies=0|feed=NUGET_LOCAL|verification=pending`

The separate guarded `--verify` returned:

`MATH-SCIENTIFIC-VERIFY|id=dw.tools.math.scientific|version=20260.20261105.1001|binary=15|archive=233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f|dependencies=0|feed=NUGET_LOCAL|PASS`

These are **owner-reported local terminal observations**, not DWF independent readback. RS156 must independently inspect actual package bytes without relying on the prior `PASS` as sufficient evidence.

## Physical archive and isolated binary verification

The original F20 candidate is `dw.tools.math.scientific [20260.20261105.1001]`, **37,265 bytes**, archive SHA-256 `233b88ecb4b322d84f03ac218d6f668a5aa4fb23dc1904a8ebbe757d7381d36f`, packaged `net10.0` DLL SHA-256 `604b1126554952f75318974028171a7dc9f463cae3c3d686d2075252ccce065c`, source Version `1.1.0-preview.14`, **zero package dependencies**, 16 original Scientific C# files and 18 total original SHA-256 source pins. Its calendar version encodes its 2026-11-05 **candidate date**, not the 2026-11-06 publication date.

The standalone, BCL-only `tools/dw.tools.math.nuget-scientific-feed-verify` inspects exactly the named physical archive within an existing absolute `NUGET_LOCAL` directory. It refuses an absent/redirected file, wrong archive length or SHA-256, malformed/unsafe/duplicated ZIP entries, wrong NuGet ID/version, declared dependencies, missing DLL or mismatched DLL SHA-256. It does not call `Dw.Tools.NuGet`, publish, repack or enumerate unrelated feed packages.

An isolated copy of the verified physical archive is made in fresh ignored `build/artifacts` output. The verifier creates an independent .NET 10 package consumer with exactly one pinned Scientific `PackageReference`, no original Math `ProjectReference`, and a `NuGet.Config` containing only the isolated archive source. It restores to a fresh private cache, builds, and runs the **unchanged 15-case Scientific binary fixture**. It verifies the SHA-256 of the actually loaded DLL and reruns exact integer, geometry, series, signal, special-function, finance and refusal/cancellation checks. The physical feed archive and snapshot must remain byte-identical before and after these operations.

The verifier records four actual process receipts (consumer restore, build, binary invocation and SDK), platform/runtime, package/archive identity, binary count and explicit excluded claims. F23 additionally binds F20 original-candidate evidence, F21 disposable rehearsal evidence, F22 release-message correction and exact RS155 first-pass DWF history.

## Result scope and exclusions

A successful RS156 qualifies Scientific as the **seventh locally published** Math package independently inspected from the existing private feed. DWF writes only Math-owned F23 documentation, read-only validation/verifier sources and qualification receipt. Existing Mathematical source versions, all 541 completed native nodes, the six prior qualified packages, previous F1–F22 evidence and the physical `NUGET_LOCAL` archive remain unchanged.

No package publication or overwrite, new public license, NuGet.org registry release, installed-package rollback, feed-wide integrity, concurrency guarantees, alternative OS/RID certification, Decision/Discovery migration or AURA D3 activation is part of this run. The next candidate must be selected separately from the F1 inventory with exact original package dependency closure.
