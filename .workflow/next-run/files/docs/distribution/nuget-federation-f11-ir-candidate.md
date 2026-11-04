# RS144 — Source-independent IR calendar NuGet candidate

## Selected dependency node and authority

RS143 succeeded first-pass on `aihallot/dw.tools.math` main at
`c4290095f37dd71babe93b8097f00c703ed40f80`,
DWF 0.1.63, attempt 001. It independently verified **three**
previously published private Math NuGet archives and the ten-case
Standard consumer. The exact published base package required here is:

- `dw.quantities [20260.20261102.1001]`;
- archive SHA-256 `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d`;
- DLL SHA-256 `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4`.

The actual thirteen-package dependency graph was inspected from the
retained F1 inventory and the original Math project files. Among the
ten unpublished libraries, `dw.tools.math.ir` is an admitted next
dependency node: its **only** historical `ProjectReference` points
to `dw.quantities`. Packaging IR before the other dependent Math
libraries unlocks possible later qualifications of `numerics`,
`symbolics`, `codecs`, `composition` and `distribution`, but
does not declare any of those packages published.

IR's historical source-owned project version is
`0.3.0-preview.1`; its `AssemblyName` and `PackageId` are
`dw.tools.math.ir`, with root namespace `Dw.Tools.Math.Ir`.
The six C# sources are: `IrCanonicalHashes.cs`,
`IrCanonicalJsonCodec.cs`, `IrCanonicalJsonGuard.cs`,
`IrGraphLimits.cs`, `IrMatrix.cs` and `IrModel.cs`.
The source project, original single ProjectReference and locked
source-to-source graph remain unchanged.

## One temporary package-only projection

A new Math-owned non-packable qualification controller,
`tools/dw.tools.math.nuget-ir-candidate`, uses the already
published `Dw.Tools.NuGet [20260.20261026.1010]`, with loaded
DLL SHA-256
`b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61`.
No competing version allocator or package inspection engine is
introduced.

Before and after the candidate test, it calls the existing F1
source/feed preflight. The preflight proposes an unreserved current-day
`20260.YYYYMMDD.NNNN` package version for this exact PackageId.
The actual version is recorded from the run; no previous version
is guessed, reserved or overwritten.

The controller checks and fingerprints all six unchanged original
source files and the original project. It constructs a separate
temporary packaging-only project under
`build/artifacts/rs144-ir-candidate/producer`. Its compiled items
are **links to the exact original source files**, not copies or an
alternative IR implementation. The temporary project's only
NuGet dependency is
`dw.quantities [20260.20261102.1001]`, substituted for the
historical ProjectReference **only inside that disposable
projection**. All production source namespaces, source Version and
project metadata are retained; only the candidate PackageVersion
uses the F1 calendar proposal.

A release build and pack must produce a real, net10.0 candidate
archive in a separate ignored directory. The shared NuGet reader
must independently accept that candidate's ID, version and
SHA-256. The `.nuspec` must declare **exactly one** pinned
dependency on the published base package. The candidate's real
archive byte length, archive SHA-256, net10.0 DLL SHA-256 and all
source fingerprints are recorded from actual bytes.

## Fifteen independent binary-only IR checks

The checked-in `IrBinaryConsumer.cs` fixture is compiled in a fresh,
isolated source-free consumer with **one direct PackageReference**:
`dw.tools.math.ir [candidate-version]`. NuGet configuration
clears all implicit sources and permits only the temporary
candidate plus the observed `NUGET_LOCAL` dependency feed.
A fresh private package cache and exact assembly hashes forbid
a source ProjectReference or arbitrary previously loaded IR DLL.

Its fifteen checks cover: both loaded assembly identities,
two loaded binary SHA-256 values, canonical exact rational JSON,
finite binary64 signed-zero bit preservation, nonfinite refusal,
semantic versus presentation hash domains, bound symbol scope and
slot, typed IR application round-trip, the sound
`sqrt(square(x))` absolute-value rule, its explicit nonnegative
assumption case, rectangular exact matrices, refusal of implicit
exact/approximate matrix mixing, exact dimensioned quantity JSON
round-trip, and rejection of an unsupported JSON kind.

The qualified receipt must include as-run compiler and consumer
exit codes, runtime/SDK, the unchanged original source project and
six source hashes, the candidate and dependency package/DLL hashes,
and exact before/after F1 inventory identity.

## Publication boundary and next admissible action

RS144 is **candidate qualification only**. DWF does not publish,
reserve or overwrite any package in `NUGET_LOCAL`; no public
license or remote publication is approved. The three previously
qualified private package archives remain unmodified, along with
RS134–RS143 evidence, the accepted Math backlog and all 541
completed native DWF nodes.

Any later release must first qualify the real IR package using
the existing shared no-overwrite publisher on a disposable feed,
then require an explicit separate owner action on the original
candidate archive and an independent read-only DWF published-feed
receipt. Rights/notices, installed-package rollback, other OS/RID
ABI, Discovery skills and Decision/AURA integration remain
unqualified outside this bounded IR candidate check.
