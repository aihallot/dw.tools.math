# M5-W02-C02-T2 — independent API, binary, native and RID qualification

RS076 qualified seven locally generated net10 NuGet packages, their dependency
identities, digest, distribution README/NOTICE and a clean sample consumer.

RS077 introduces MathPackageRuntimeSurfaceAudit over those actual archive
bytes, using BCL PEReader/MetadataReader. The auditor never extracts,
executes or loads code. It verifies the declared net10 assembly's identity,
IL-only CLI metadata, bounded public types, fields and method signatures,
required distribution entrypoints and a closed Math/runtime assembly
reference graph. The resulting SHA-256 values fingerprint the current
managed binary and PUBLIC API of the current package only.

Foreign native runtime entries, native libraries and malformed managed PE
are explicit refusals. This is a conservative closed-scope capability
boundary, not a new package parser or external compiler.

A current API hash is NOT proof of source/binary compatibility across
different versions. The result therefore sets all cross-version source,
cross-version binary, additional RID and performance SLA certification
flags to false. The documented bounds for package size, ZIP entries,
uncompressed content, public metadata counts and signature bytes are
resource budgets, not portable latency promises.

The actual host RID and SDK are recorded by DWF at execution. Previous
durable evidence observed win-x64 with .NET SDK 10.0.402. Neither net10
nor Windows success certifies Linux, macOS or Windows ARM64.

RED is a compiled missing public API assertion before the auditor is
installed. GREEN tests use real locally built PE assemblies including
all seven Math packages, native-asset refusals, malformed/foreign
archives, budget limits and ABI nonclaims. The independent NuGet-only
consumer cross-checks actual archive assembly SHA and public API
fingerprints and executes eight mathematical/package probes. Prior
clean consumer and exact math console probes must survive preview.4.

After durable proof only C02-T2 and C02 close. M5-W02, M5 and G-M5
remain open; CI publication, new OS host matrices and stable 1.0
require separate authorization and evidence.

## Controlled retained-consumer compatibility

The prior T1 clean NuGet consumer retains its eight scientific/package
oracles. Its input directory check explicitly admits only the originally
qualified `m5-w02-c02-t1-local-feed` or the current
`m5-w02-c02-t2-local-feed`. The RS077 payload reuses the SAME seven
inspected .nupkg files directly; it does not create or copy a second
repository directory. This corrects the original pre-attempt refusal
for untracked `Directory.CreateDirectory` and `File.Copy` in payload.
All exact filename, version, SHA-256, dependency and NuGet-only constraints
remain enforced.
