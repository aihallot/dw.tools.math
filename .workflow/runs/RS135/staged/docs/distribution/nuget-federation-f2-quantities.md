# RS135 — Calendar NuGet candidate for standalone dw.quantities

## Authority and objective

RS134 was successfully delivered on Math main at commit
e01cf477bb82f01cb3643921094f3b58b3c217b6 by DWF 0.1.63.
Its actual attempt 001 succeeded after two pre-attempt refusals, so strict
first-pass was **NO**. The read-only F1 intake observed 13 packable Math
libraries, 13 evaluated MSBuild identities, the published
Dw.Tools.NuGet [20260.20261026.1010] core, and zero preexisting Math
archives in NUGET_LOCAL at the 2026-11-02 observation. Its preview
20260.20261102.1001 for each PackageId was **not a reservation**.

RS135 adds one reproducible candidate qualification for the standalone
dw.quantities package, invoking the **unchanged, source-pinned RS134 preflight** in read-only
`--inspect` mode for feed inventory and calendar version proposals. A separate
bounded F2 C# candidate consumer uses the shared NuGet package reader without
introducing another version allocator, publisher or package parser. This
candidate has no project or external NuGet dependencies; wider quantity
packages, complex Math/Discovery bindings and all later package families
remain out of scope.

## Source, version and actual package identity

The source project's historical Version is 0.2.0-preview.1. It is not
rewritten. At execution time the tool reads the selected existing absolute
NUGET_LOCAL, restricts discovery to the thirteen owned Math PackageIds,
validates actual candidate history via the qualified shared library, and
calculates a new unreserved version 20260.YYYYMMDD.NNNN for dw.quantities.

It invokes the existing F1 preflight before and after the candidate build,
verifying identical observed shared-feed identity and on-date preview. The
original F1 Program.cs, project and all previously qualified source blobs
remain byte-for-byte unchanged. It uses an explicit **pack-time-only** PackageVersion override and a
private candidate feed under ignored build/artifacts/rs135-quantities-candidate.
The resulting .nupkg must have exactly the proposed PackageId, version,
zero NuGet dependencies, and lib/net10.0/dw.quantities.dll. The archive and
its DLL are separately hashed (SHA-256). The actual loaded NuGet federator
assembly is checked against the previously qualified DLL SHA-256. All
other Math source project/package versions remain unchanged.

The exact candidate filename and hashes become valid evidence only
after the actual DWF run. No hard-coded sample version or preflight
output is a publication receipt. The previous preview and any existing
archives remain immutable.

## Binary-only compiled consumer

A complete independent C# consumer fixture is retained as
tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs.
The qualified tool copies that file into a run-owned ignored consumer
directory, creates only a bounded version-pinned .csproj and a NuGet
configuration with no global sources, then restores the candidate via an
explicit private feed and package cache.

This consumer has **no Math ProjectReference**, and it must compile and
execute from the package DLL alone. Four independently observed checks
cover: actual loaded assembly identity; SHA-256 equality between the
loaded DLL and packaged DLL; exact 1/3 + 1/6 = 1/2; and a typed refusal of
a zero denominator. All restore/build/pack/consumer commands retain actual
exit codes, and the source Math library has a separate locked restore
and Release build. The feed's scoped package inventory is rechecked after
the test to detect any changes.

## Historical and publication boundaries

The historical two local dw.quantities 0.2.0-preview.1 archive SHA-256
values from separate disposable builds are not equated or overwritten.
The source Version remains unchanged. No physical migration of an
installed consumer is performed; consequently no physical package
rollback is claimed. The unchanged source-call pathway remains available
but is not a new binary fallback qualification.

The Math owner attestation for selected originally transferred AURA
math code allows package formation and redistribution of that bounded
material, **not** an automatic license to all third-party material or
a public license expression for every package. The current project's
PackageLicenseExpression is absent. A future local publication must
have an explicit owner-reviewed rights/notices decision, immutable
package+DLL SHA receipts, dependency-closure proof, consumer rollback,
feed collision refusal, and a **separate** authorized publication gate
using dw.tools.nuget. There is no automatic publication during DWF.

No NuGet.org/remote publication, NUGET_LOCAL write, global NuGet.Config
mutation, new skill, broker/host authorization or Discovery migration
is within RS135. Accepted Math roadmap 0.1.122 remains done and all 541
native DWF nodes retain their states. This is a bounded M7 evidence-only
continuation, not an unauthorized M3-W03 or new milestone.
