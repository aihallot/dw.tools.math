# M5-W02-C02-T1 — qualification of clean local package consumers

## Accepted product decision

T1 qualifies a **locally reproducible**, versioned set of seven Math NuGet
archives built for net10.0. The single pinned distribution-package consumer
restores without any ProjectReference to the repository sources. The existing
independent numerical/symbolic console sample is also restored from the same
isolated local feed. This is not equivalent to public NuGet publication.

MathLocalReleaseQualification is a bounded public BCL ZIP inspector. Its
ExpectedPackages graph is only a declaration. Audit(id, bytes) verifies actual
archive bytes, expected identity/version, direct dependency IDs and version
references, net10 assembly entry, resource limits, and embedded distribution
README/NOTICE. A qualified SHA-256 is computed from real nupkg bytes.
An independent C# helper rechecks archive graph, entries and SHA separately.
The clean NuGet consumer compares all seven verified digests with its own
read from the same archive paths, and executes the already admitted exact Math
vertical without any external CAS or agent runtime dependency.

The new distribution package embeds readme.md and NOTICE.md. The notices are
not a third-party-license waiver or an assertion of remote publication.
Other preexisting lower-level packages have not been upgraded to carry new
documentation in T1; if needed, that remains a separate admitted backlog
issue. No silently broadened source/binary compatibility or license is claimed.

## Platform truth and targeted validation

Previously observed host: Windows win-x64, .NET SDK 10.0.402 in the durable
RS075 receipt. The RS076 run records its actual runtime/SDK again.
Only net10.0 as a managed target is asserted by the package metadata;
neither Linux nor macOS has been independently qualified here.
The public package API explicitly sets RuntimeIdentifierCertified=false.

This T1 gate does not create an unrequested GitHub Actions/paid CI workflow.
It supplies local, deterministic, command-line .NET 10 validation suitable for
a later authorized CI system. Cross-OS release suites, binary/API evolution,
native dependencies, machine-path leakage audits, and full support-matrix
promotion remain M5-W02-C02-T2 obligations.

## Independent falsification

Observe one genuinely compiled missing-public-package-API RED against
distribution preview.2. Recover RED, pass independent real/hostile/archive
oracles and relevant Math regressions under a locked Release solution build.
Repack seven actual local archives and verify SHA-256, nuspec dependency
graph, metadata and embedded documentation. Restore the clean consumer with
one pinned distribution PackageReference and no external feeds, execute
eight independent probes, then rerun the prior eight-probe numerical/symbolic
console from the same local feed. No AURA installation, sibling source
checkout, external service, stable release or cross-platform certification.

After durable success complete only C02-T1 and its R/G/V children.
C02-T2, C02, M5-W02, M5 and G-M5 remain open.
